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

    private bool levelRewardAssigned = false;
    private bool activityRewardAssigned = false;

    public LevelRewardSystem(
        int pointsPerCompletedLevel = 10,
        int pointsPerEnemyDefeated = 3,
        int pointsPerCollectedCoin = 2,
        int pointsPerShopCoin = 10)
    {
        PointsPerCompletedLevel = pointsPerCompletedLevel;
        PointsPerEnemyDefeated = pointsPerEnemyDefeated;
        PointsPerCollectedCoin = pointsPerCollectedCoin;
        PointsPerShopCoin = pointsPerShopCoin;

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

    public bool CompleteLevel(bool levelCompleted)
    {
        if (!levelCompleted)
            return false;

        if (levelRewardAssigned)
            return false;

        CurrentLevelPoints += PointsPerCompletedLevel;
        levelRewardAssigned = true;

        LastPointsEarned = CurrentLevelPoints;

        TotalPoints += CurrentLevelPoints;

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

    public void ResetLevelProgress()
    {
        CurrentLevelPoints = 0;
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

        levelRewardAssigned = false;
        activityRewardAssigned = false;
    }
}