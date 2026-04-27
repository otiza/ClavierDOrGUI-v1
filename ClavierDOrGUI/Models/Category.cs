using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClavierDOrGUI.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Navigation
    public List<Question> Questions { get; set; } = new();
}

