using ClavierDOrGUI.Models;

namespace ClavierDOrGUI.Services;

public class ScoreService
{
    /// <summary>
    /// Calculate points for a question.
    /// Normal correct answer = 10, boss correct answer = 15, wrong = 0
    /// </summary>
    public int CalculatePoints(Question question, bool isCorrect)
    {
        if (!isCorrect) return 0;
        return question.IsBoss ? 15 : 10;
    }
}

