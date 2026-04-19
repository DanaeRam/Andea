using System.Collections.Generic;

public class FakeProgressStorage :  IProgressStorage
// Start is called once before the first execution of Update after the MonoBehaviour is created
{
    private Dictionary<string, PlayerProgressData> database = new Dictionary<string, PlayerProgressData>();

    public bool shouldFailSave = false;
    public bool shouldInterruptSave = false;

    public bool SaveProgress(PlayerProgressData progress)
    {
        if (shouldFailSave)
            return false;

        if (shouldInterruptSave)
            return false;

        database[progress.userId] = new PlayerProgressData(
            progress.userId,
            progress.levelReached,
            progress.score,
            progress.completedActivities
        );

        return true;
    }

    public PlayerProgressData LoadProgress(string userId)
    {
        if (!database.ContainsKey(userId))
            return null;

        return database[userId];
    }
}

