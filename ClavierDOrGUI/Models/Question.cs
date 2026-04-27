using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClavierDOrGUI.Models;

public class Question
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string ChoiceA { get; set; } = string.Empty;
    [Required]
    public string ChoiceB { get; set; } = string.Empty;
    [Required]
    public string ChoiceC { get; set; } = string.Empty;
    [Required]
    public string ChoiceD { get; set; } = string.Empty;

    // Store correct answer as single letter: "A", "B", "C" or "D"
    [Required]
    [MaxLength(1)]
    public string Correct { get; set; } = "A";

    public bool IsBoss { get; set; } = false;
}

