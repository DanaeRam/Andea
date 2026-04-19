using System;

public class PlayerProgressData
{
    public string userId;
    public int levelReached;
    public int score;
    public int completedActivities;

    public PlayerProgressData(string userId, int levelReached, int score, int completedActivities)
    {
        this.userId = userId;
        this.levelReached = levelReached;
        this.score = score;
        this.completedActivities = completedActivities;
    }
}

public interface IProgressStorage
{
    bool SaveProgress(PlayerProgressData progress);
    PlayerProgressData LoadProgress(string userId);
}

public class GameProgressSystem
{
    private readonly IProgressStorage storage;

    public GameProgressSystem(IProgressStorage storage)
    {
        this.storage = storage;
    }

    public bool CanUnlockNextLevel(bool currentLevelCompleted)
    {
        return currentLevelCompleted;
    }

    public bool CanAccessLevel(int requestedLevel, int highestUnlockedLevel)
    {
        return requestedLevel <= highestUnlockedLevel;
    }

    public bool SaveProgress(PlayerProgressData progress)
    {
        if (progress == null)
            return false;

        if (string.IsNullOrWhiteSpace(progress.userId))
            return false;

        if (progress.levelReached <= 0)
            return false;

        if (progress.score < 0)
            return false;

        if (progress.completedActivities < 0)
            return false;

        return storage.SaveProgress(progress);
    }

    public PlayerProgressData LoadProgress(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        return storage.LoadProgress(userId);
    }
}