using Dapper;
using Peneluc.Concurseiro.Web.Backend.Domain.Entities;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Common;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;
using System.Text;

namespace Peneluc.Concurseiro.Web.Backend.Infrastructure.Repositories;

public class QuestionReadRepository : IQuestionReadRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private static readonly HashSet<string> AllowedOrderBy = new()
    {
        "statement",
        "subjectname",
        "examyear",
        "difficulty"
    };

    public QuestionReadRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<QuestionEntity>> GetQuestionsAsync(
        string? subject,
        string? exam,
        string? difficulty,
        int page,
        int pageSize,
        string? orderBy)
    {
        using var connection = _connectionFactory.Create();

        var offset = (page - 1) * pageSize;

        var sql = new StringBuilder(@"
            SELECT 
                q.id,
                q.statement,
                q.explanation,
                s.name as SubjectName,
                e.name as ExamName,
                e.year as ExamYear,
                q.difficulty
            FROM questions q
            JOIN subjects s ON s.id = q.subject_id
            JOIN exam_sources e ON e.id = q.exam_source_id
            WHERE 1=1
        ");

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(subject))
        {
            sql.Append(" AND s.name ILIKE @Subject ");
            parameters.Add("Subject", $"%{subject}%");
        }

        if (!string.IsNullOrEmpty(exam))
        {
            sql.Append(" AND e.name ILIKE @Exam ");
            parameters.Add("Exam", $"%{exam}%");
        }

        if (!string.IsNullOrEmpty(difficulty))
        {
            sql.Append(" AND q.difficulty = @Difficulty ");
            parameters.Add("Difficulty", difficulty);
        }

        if (!string.IsNullOrEmpty(orderBy) &&
            AllowedOrderBy.Contains(orderBy.ToLower()))
        {
            sql.Append($" ORDER BY {orderBy} ");
        }
        else
        {
            sql.Append(" ORDER BY q.statement ");
        }

        sql.Append(" LIMIT @PageSize OFFSET @Offset ");

        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var data = await connection.QueryAsync<QuestionEntity>(
            sql.ToString(), parameters);

        var total = await connection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(*) FROM questions
        ");

        return new PagedResult<QuestionEntity>
        {
            Data = data,
            TotalRecords = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
