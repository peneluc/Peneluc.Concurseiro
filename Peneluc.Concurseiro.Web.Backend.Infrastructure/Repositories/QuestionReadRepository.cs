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

        var sqlBase = new StringBuilder(@"
            FROM questions q
            JOIN subjects s ON s.id = q.subject_id
            JOIN exam_sources es ON es.id = q.exam_source_id
            WHERE 1=1
        ");

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(subject))
        {
            sqlBase.Append(" AND s.name ILIKE @Subject ");
            parameters.Add("Subject", $"%{subject}%");
        }

        if (!string.IsNullOrWhiteSpace(exam))
        {
            sqlBase.Append(" AND es.name ILIKE @Exam ");
            parameters.Add("Exam", $"%{exam}%");
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            sqlBase.Append(" AND q.difficulty = @Difficulty ");
            parameters.Add("Difficulty", difficulty);
        }

        var sqlData = new StringBuilder(@"
            SELECT 
                q.id,
                q.statement,
                q.explanation,
                q.difficulty,
                q.subject_id AS SubjectId,
                s.name AS SubjectName,
                q.exam_source_id AS ExamSourceId,
                es.name AS ExamName,
                es.year AS ExamYear
        ");

        sqlData.Append(sqlBase);

        if (!string.IsNullOrWhiteSpace(orderBy) &&
            AllowedOrderBy.Contains(orderBy.ToLower()))
        {
            sqlData.Append($" ORDER BY {orderBy} ");
        }
        else
        {
            sqlData.Append(" ORDER BY q.statement ");
        }

        sqlData.Append(" LIMIT @PageSize OFFSET @Offset ");
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", offset);

        var data = await connection.QueryAsync<QuestionEntity>(
            sqlData.ToString(), parameters);

        var total = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) " + sqlBase.ToString(),
            parameters);

        return new PagedResult<QuestionEntity>
        {
            Data = data,
            TotalRecords = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<QuestionEntity>> GetAllAsync()
    {
        using var conn = _connectionFactory.Create();

        const string sql = """
            SELECT 
                id,
                statement,
                explanation,
                difficulty,
                subject_id AS SubjectId,
                exam_source_id AS ExamSourceId
            FROM questions
            ORDER BY id DESC
        """;

        return await conn.QueryAsync<QuestionEntity>(sql);
    }

    public async Task<QuestionEntity?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.Create();

        const string sql = """
            SELECT 
                id,
                statement,
                explanation,
                difficulty,
                subject_id AS SubjectId,
                exam_source_id AS ExamSourceId
            FROM questions
            WHERE id = @Id
        """;

        return await conn.QueryFirstOrDefaultAsync<QuestionEntity>(
            sql, new { Id = id });
    }

    public async Task<Guid> InsertAsync(QuestionEntity question)
    {
        using var conn = _connectionFactory.Create();

        const string sql = """
            INSERT INTO questions 
                (statement, explanation, difficulty, subject_id, exam_source_id)
            VALUES 
                (@Statement, @Explanation, @Difficulty, @SubjectId, @ExamSourceId)
            RETURNING id;
        """;

        return await conn.ExecuteScalarAsync<Guid>(sql, question);
    }

    public async Task UpdateAsync(QuestionEntity question)
    {
        using var conn = _connectionFactory.Create();

        const string sql = """
            UPDATE questions
            SET statement = @Statement,
                explanation = @Explanation,
                difficulty = @Difficulty,
                subject_id = @SubjectId,
                exam_source_id = @ExamSourceId
            WHERE id = @Id
        """;

        await conn.ExecuteAsync(sql, question);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.Create();

        const string sql = """
            DELETE FROM questions
            WHERE id = @Id
        """;

        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
