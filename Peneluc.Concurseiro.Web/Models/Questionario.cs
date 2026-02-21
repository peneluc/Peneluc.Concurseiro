
using System.ComponentModel.DataAnnotations;

namespace Peneluc.Concurseiro.Web.Models;

public class Questionario
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; }

    [StringLength(1000)]
    public string Descricao { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public bool Ativo { get; set; } = true;
}
