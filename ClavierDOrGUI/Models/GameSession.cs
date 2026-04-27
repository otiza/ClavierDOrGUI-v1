using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClavierDOrGUI.Models;

public class GameSession
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PlayerId { get; set; }

    public Player? Player { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    public bool IsFinished { get; set; } = false;

    // Track score (sum of points)
    public int Score { get; set; } = 0;

    // Progress tracking (optional index or question count)
    public int CurrentQuestionIndex { get; set; } = 0;

    // Navigation
    public List<AnsweredQuestion> AnsweredQuestions { get; set; } = new();
}

