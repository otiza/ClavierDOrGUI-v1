using Microsoft.EntityFrameworkCore;
using ClavierDOrGUI.Data;
using ClavierDOrGUI.Models;

namespace ClavierDOrGUI.Services;

public class GameService
{
    private readonly AppDbContext _db;
    private readonly ScoreService _scoreService;

    public GameService(AppDbContext db, ScoreService scoreService)
    {
        _db = db;
        _scoreService = scoreService;
    }

    // Start a new game for a player (create player if missing)
    public async Task<GameSession> StartNewGameAsync(string playerName, PlayerRole role)
    {
        // find or create player
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Name == playerName);
        if (player == null)
        {
            player = new Player { Name = playerName, Role = role };
            _db.Players.Add(player);
            await _db.SaveChangesAsync();
        }

        var session = new GameSession
        {
            PlayerId = player.Id,
            StartedAt = DateTime.UtcNow,
            IsFinished = false,
            Score = 0,
            CurrentQuestionIndex = 0
        };

        _db.GameSessions.Add(session);
        await _db.SaveChangesAsync();

        return session;
    }

    // Resume latest unfinished session for a player
    public async Task<GameSession?> ResumeLatestSessionAsync(string playerName)
    {
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Name == playerName);
        if (player == null) return null;

        var session = await _db.GameSessions
            .Where(s => s.PlayerId == player.Id && !s.IsFinished)
            .Include(s => s.AnsweredQuestions)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();

        return session;
    }

    // Save session (persist changes)
    public async Task SaveSessionAsync(GameSession session)
    {
        _db.GameSessions.Update(session);
        await _db.SaveChangesAsync();
    }

    // Mark session as finished and persist
    public async Task MarkFinishedAsync(GameSession session)
    {
        session.IsFinished = true;
        session.FinishedAt = DateTime.UtcNow;
        // Recalculate score from answered questions to be safe
        session.Score = await RecalculateScoreAsync(session.Id);
        _db.GameSessions.Update(session);
        await _db.SaveChangesAsync();
    }

    // Answer a question within a session
    // selectedChoice should be "A","B","C","D" (case-insensitive)
    public async Task<(bool IsCorrect, int Points)> AnswerQuestionAsync(int sessionId, int questionId, string selectedChoice)
    {
        selectedChoice = (selectedChoice ?? string.Empty).Trim().ToUpper();

        var session = await _db.GameSessions
            .Include(s => s.AnsweredQuestions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null) throw new InvalidOperationException("Session not found");

        var question = await _db.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
        if (question == null) throw new InvalidOperationException("Question not found");

        var existing = session.AnsweredQuestions.FirstOrDefault(a => a.QuestionId == questionId);
        if (existing != null)
        {
            // increment attempts
            existing.Attempts++;
            existing.SelectedChoice = selectedChoice;
            existing.IsCorrect = string.Equals(question.Correct?.Trim(), selectedChoice, StringComparison.OrdinalIgnoreCase);
            existing.AnsweredAt = DateTime.UtcNow;
            _db.AnsweredQuestions.Update(existing);
        }
        else
        {
            existing = new AnsweredQuestion
            {
                GameSessionId = session.Id,
                QuestionId = question.Id,
                SelectedChoice = selectedChoice,
                IsCorrect = string.Equals(question.Correct?.Trim(), selectedChoice, StringComparison.OrdinalIgnoreCase),
                Attempts = 1,
                AnsweredAt = DateTime.UtcNow
            };
            _db.AnsweredQuestions.Add(existing);
            session.AnsweredQuestions.Add(existing);
        }

        // Calculate points for this answer and update session score
        var points = _scoreService.CalculatePoints(question, existing.IsCorrect);

        // Recalculate session score by summing answered questions
        session.Score = await RecalculateScoreAsync(session.Id);

        // Optionally advance CurrentQuestionIndex
        session.CurrentQuestionIndex = session.AnsweredQuestions.Count;

        _db.GameSessions.Update(session);
        await _db.SaveChangesAsync();

        return (existing.IsCorrect, points);
    }

    // Helper: recalculate total score for a session
    public async Task<int> RecalculateScoreAsync(int sessionId)
    {
        var answered = await _db.AnsweredQuestions
            .Where(a => a.GameSessionId == sessionId)
            .Include(a => a.Question)
            .ToListAsync();

        int total = 0;
        foreach (var a in answered)
        {
            if (a.Question == null) continue;
            total += _scoreService.CalculatePoints(a.Question, a.IsCorrect);
        }

        return total;
    }

    // Get game history (finished sessions) for a player
    public async Task<List<GameSession>> GetHistoryAsync(string playerName)
    {
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Name == playerName);
        if (player == null) return new List<GameSession>();

        return await _db.GameSessions
            .Where(s => s.PlayerId == player.Id && s.IsFinished)
            .OrderByDescending(s => s.FinishedAt)
            .Include(s => s.AnsweredQuestions)
            .ToListAsync();
    }
}

