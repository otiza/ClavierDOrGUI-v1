using System;
using System.ComponentModel.DataAnnotations;

namespace ClavierDOrGUI.Models;

public class AnsweredQuestion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GameSessionId { get; set; }

    public GameSession? GameSession { get; set; }

    [Required]
    public int QuestionId { get; set; }

    public Question? Question { get; set; }

    // Selected choice as single letter: "A","B","C","D"
    [MaxLength(1)]
    public string? SelectedChoice { get; set; }

    public bool IsCorrect { get; set; }

    // Number of attempts for this question in the session
    public int Attempts { get; set; } = 1;

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}

