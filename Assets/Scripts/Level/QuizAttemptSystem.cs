public class QuizAttemptSystem
{
    public int MaxAttempts { get; private set; } = 3;
    public int AttemptsUsed { get; private set; } = 0;

    public bool IsBlocked { get; private set; } = false;
    public bool HintShown { get; private set; } = false;

    public int RemainingAttempts
    {
        get { return MaxAttempts - AttemptsUsed; }
    }

    public QuizAttemptSystem(int maxAttempts = 3)
    {
        MaxAttempts = maxAttempts;
        AttemptsUsed = 0;
        IsBlocked = false;
        HintShown = false;
    }

    public void RegisterWrongAnswer()
    {
        if (IsBlocked)
            return;

        AttemptsUsed++;

        if (AttemptsUsed == 1)
        {
            HintShown = true;
        }

        if (AttemptsUsed >= MaxAttempts)
        {
            IsBlocked = true;
        }
    }

    public bool CanAnswer()
    {
        return !IsBlocked && AttemptsUsed < MaxAttempts;
    }

    public bool IsThirdAttemptAvailable()
    {
        return AttemptsUsed == 2 && !IsBlocked;
    }

    public void Reset()
    {
        AttemptsUsed = 0;
        IsBlocked = false;
        HintShown = false;
    }
}