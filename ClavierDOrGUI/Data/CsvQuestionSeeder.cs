using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using ClavierDOrGUI.Models;

namespace ClavierDOrGUI.Data;

public class CsvQuestionRecord
{
    public int category_id { get; set; }
    public string category_name { get; set; } = string.Empty;
    public string text { get; set; } = string.Empty;
    public string choice_a { get; set; } = string.Empty;
    public string choice_b { get; set; } = string.Empty;
    public string choice_c { get; set; } = string.Empty;
    public string choice_d { get; set; } = string.Empty;
    public string correct { get; set; } = string.Empty;
    public string is_boss { get; set; } = "FALSE";
}

public class CsvQuestionSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly string _csvPath;

    public CsvQuestionSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        // CSV is copied to output: Data/data.csv
        var baseDir = AppContext.BaseDirectory;
        _csvPath = Path.Combine(baseDir, "Data", "data.csv");
    }

    public async Task SeedIfEmptyAsync()
    {
        if (await _dbContext.Questions.AnyAsync())
            return; // already seeded

        if (!File.Exists(_csvPath))
            return; // nothing to seed

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var reader = new StreamReader(_csvPath);
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<CsvQuestionRecord>();

        var categories = new Dictionary<int, Category>();
        foreach (var r in records)
        {
            if (!categories.ContainsKey(r.category_id))
            {
                var cat = new Category { Id = r.category_id, Name = r.category_name };
                categories[r.category_id] = cat;
            }

            var q = new Question
            {
                CategoryId = r.category_id,
                Text = r.text,
                ChoiceA = r.choice_a,
                ChoiceB = r.choice_b,
                ChoiceC = r.choice_c,
                ChoiceD = r.choice_d,
                Correct = (r.correct ?? "A").Trim().ToUpper(),
                IsBoss = string.Equals(r.is_boss?.Trim(), "TRUE", StringComparison.OrdinalIgnoreCase)
            };

            categories[r.category_id].Questions.Add(q);
        }

        // Upsert categories and questions
        foreach (var cat in categories.Values)
        {
            // If category exists by name or id, reuse
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == cat.Id || c.Name == cat.Name);
            if (existing == null)
            {
                _dbContext.Categories.Add(cat);
            }
            else
            {
                // Add questions under existing category
                foreach (var q in cat.Questions)
                {
                    q.CategoryId = existing.Id;
                    _dbContext.Questions.Add(q);
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}

