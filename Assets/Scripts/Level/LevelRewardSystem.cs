public class LevelRewardSystem
{
    public int TotalPoints { get; private set; }
    public int ShopCoins { get; private set; }

    public int CurrentLevelPoints { get; private set; }

    public int LastPointsEarned { get; private set; }
    public int LastShopCoinsEarned { get; private set; }
    public int LastRemainingPoints { get; private set; }

    public int PointsPerCompletedLevel { get; private set; } = 10;
    public int PointsPerEnemyDefeated { get; private set; } = 3;
    public int PointsPerCollectedCoin { get; private set; } = 2;
    public int PointsPerShopCoin { get; private set; } = 10;
    public int PenaltyPerWrongAnswer { get; private set; } = 2;
    public int PenaltyPerDeath { get; private set; } = 3;

    public int LastWrongAnswers { get; private set; }
    public int LastDeaths { get; private set; }
    public int LastPenaltyApplied { get; private set; }

    private bool levelRewardAssigned = false;
    private bool activityRewardAssigned = false;

    public LevelRewardSystem(
        int pointsPerCompletedLevel = 10,
        int pointsPerEnemyDefeated = 3,
        int pointsPerCollectedCoin = 2,
        int pointsPerShopCoin = 10,
        int penaltyPerWrongAnswer = 2,
        int penaltyPerDeath = 3)
    {
        PointsPerCompletedLevel = pointsPerCompletedLevel;
        PointsPerEnemyDefeated = pointsPerEnemyDefeated;
        PointsPerCollectedCoin = pointsPerCollectedCoin;
        PointsPerShopCoin = pointsPerShopCoin;
        PenaltyPerWrongAnswer = penaltyPerWrongAnswer;
        PenaltyPerDeath = penaltyPerDeath;

        TotalPoints = 0;
        ShopCoins = 0;
        CurrentLevelPoints = 0;
    }

    public bool RegisterEnemyDefeated()
    {
        CurrentLevelPoints += PointsPerEnemyDefeated;
        return true;
    }

    public bool RegisterCollectedCoin()
    {
        CurrentLevelPoints += PointsPerCollectedCoin;
        return true;
    }

    public bool CompleteLevel(bool levelCompleted, int wrongAnswers = 0, int deaths = 0)
    {
        if (!levelCompleted)
            return false;

        if (levelRewardAssigned)
            return false;

        CurrentLevelPoints += PointsPerCompletedLevel;
        levelRewardAssigned = true;

        LastWrongAnswers = ClampToZero(wrongAnswers);
        LastDeaths = ClampToZero(deaths);
        LastPenaltyApplied = (LastWrongAnswers * PenaltyPerWrongAnswer) + (LastDeaths * PenaltyPerDeath);
        LastPointsEarned = ClampToZero(CurrentLevelPoints - LastPenaltyApplied);

        TotalPoints += LastPointsEarned;

        int coinsBefore = ShopCoins;
        ConvertPointsToShopCoins();

        LastShopCoinsEarned = ShopCoins - coinsBefore;
        LastRemainingPoints = TotalPoints;

        CurrentLevelPoints = 0;

        return true;
    }

    public bool CompleteActivity(bool activityCompleted)
    {
        if (!activityCompleted)
            return false;

        if (activityRewardAssigned)
            return false;

        CurrentLevelPoints += PointsPerCompletedLevel;
        activityRewardAssigned = true;

        LastWrongAnswers = 0;
        LastDeaths = 0;
        LastPenaltyApplied = 0;
        LastPointsEarned = CurrentLevelPoints;

        TotalPoints += CurrentLevelPoints;

        int coinsBefore = ShopCoins;
        ConvertPointsToShopCoins();

        LastShopCoinsEarned = ShopCoins - coinsBefore;
        LastRemainingPoints = TotalPoints;

        CurrentLevelPoints = 0;

        return true;
    }

    public bool AssignFailedLevelReward()
    {
        return false;
    }

    private void ConvertPointsToShopCoins()
    {
        while (TotalPoints >= PointsPerShopCoin)
        {
            TotalPoints -= PointsPerShopCoin;
            ShopCoins++;
        }
    }
    private int ClampToZero(int value)
    {
        return value < 0 ? 0 : value;
    }

    public void ResetLevelProgress()
    {
        CurrentLevelPoints = 0;
        LastWrongAnswers = 0;
        LastDeaths = 0;
        LastPenaltyApplied = 0;
        levelRewardAssigned = false;
        activityRewardAssigned = false;
    }

    public void Reset()
    {
        TotalPoints = 0;
        ShopCoins = 0;
        CurrentLevelPoints = 0;

        LastPointsEarned = 0;
        LastShopCoinsEarned = 0;
        LastRemainingPoints = 0;
        LastWrongAnswers = 0;
        LastDeaths = 0;
        LastPenaltyApplied = 0;

        levelRewardAssigned = false;
        activityRewardAssigned = false;
    }
}