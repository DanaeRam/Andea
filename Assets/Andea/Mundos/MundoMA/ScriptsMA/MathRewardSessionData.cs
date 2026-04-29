public static class MathRewardSessionData
{
    public static int correctAnswers = 0;
    public static int wrongAnswers = 0;

    public static int pointsPerCorrectAnswer = 3;
    public static int completionBonus = 10;

    public static int TotalQuestionPoints
    {
        get { return correctAnswers * pointsPerCorrectAnswer; }
    }

    public static int TotalPoints
    {
        get { return TotalQuestionPoints + completionBonus; }
    }

    public static void Reset()
    {
        correctAnswers = 0;
        wrongAnswers = 0;
    }

    public static void RegisterCorrect()
    {
        correctAnswers++;
    }

    public static void RegisterWrong()
    {
        wrongAnswers++;
    }
}