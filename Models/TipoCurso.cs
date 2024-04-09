using System.ComponentModel.DataAnnotations;

public class TipoCurso
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [MinLength(3)]
    [MaxLength(100, ErrorMessage = "O tamanho máximo é 100")]
    public string? Nome { get; set; }

    [Required]
    [MinLength(5, ErrorMessage = "Digite ao menos 5 caracteres")]
    [MaxLength(100)]
    public string? Descricao { get; set; }
}