using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClavierDOrGUI.Models;

public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public PlayerRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<GameSession> GameSessions { get; set; } = new();
}

