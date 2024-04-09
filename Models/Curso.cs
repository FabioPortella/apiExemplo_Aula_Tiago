using System.ComponentModel.DataAnnotations;

public class Curso
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [MinLength(3)]
    [MaxLength(100, ErrorMessage = "O nome deve possuir, no máximo, 100 caracteres")]
    public string? Nome { get; set; }

    
    [Required]
    public int TipoCursoId { get; set; }

    public TipoCurso? TipoDoCurso { get; set; }
}