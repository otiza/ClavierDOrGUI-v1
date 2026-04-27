using Microsoft.EntityFrameworkCore;
using ClavierDOrGUI.Models;

namespace ClavierDOrGUI.Data;

public class DatabaseService
{
    private readonly AppDbContext _dbContext;
    private readonly CsvQuestionSeeder _seeder;

    public DatabaseService(AppDbContext dbContext, CsvQuestionSeeder seeder)
    {
        _dbContext = dbContext;
        _seeder = seeder;
    }

    public async Task InitializeAsync()
    {
        // Ensure database created
        await _dbContext.Database.EnsureCreatedAsync();

        // Seed data from CSV if empty
        await _seeder.SeedIfEmptyAsync();
    }
}

