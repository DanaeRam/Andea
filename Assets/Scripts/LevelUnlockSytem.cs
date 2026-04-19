public class LevelUnlockSystem
{
    public bool NextLevelUnlocked { get; private set; }

    public LevelUnlockSystem()
    {
        NextLevelUnlocked = false;
    }

    public bool CompleteLevelAndUnlockNext()
    {
        NextLevelUnlocked = true;
        return NextLevelUnlocked;
    }

    public void Reset()
    {
        NextLevelUnlocked = false;
    }
}