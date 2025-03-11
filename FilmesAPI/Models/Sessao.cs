using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Sessão
{
    [Key]
    [Required]
    public int Id { get; set; }
}
