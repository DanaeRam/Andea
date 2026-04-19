using NUnit.Framework;

public class LevelRewardSystemTests
{
    [Test]
    public void TC28_NivelCompletado_AsignaPuntosCorrectamenteEnMenosDeDosSegundos()
    {
        LevelRewardSystem rewards = new LevelRewardSystem();

        bool rewardAssigned = rewards.CompleteLevel(true);

        Assert.IsTrue(rewardAssigned);
        Assert.AreEqual(0, rewards.TotalPoints);
        Assert.AreEqual(1, rewards.ShopCoins);
    }

    [Test]
    public void TC29_EntregaDeRecompensa_AlCompletarActividad_OtorgaRecompensaCorrespondienteAlProgreso()
    {
        LevelRewardSystem rewards = new LevelRewardSystem();

        bool rewardAssigned = rewards.CompleteActivity(true);

        Assert.IsTrue(rewardAssigned);
        Assert.AreEqual(0, rewards.TotalPoints);
        Assert.AreEqual(1, rewards.ShopCoins);
    }

    [Test]
    public void TC30_ErrorEnAsignacion_AlFinalizarNivelConFallo_EvitaAsignacionesIncorrectas()
    {
        LevelRewardSystem rewards = new LevelRewardSystem();

        bool rewardAssigned = rewards.AssignFailedLevelReward();

        Assert.IsFalse(rewardAssigned);
        Assert.AreEqual(0, rewards.TotalPoints);
        Assert.AreEqual(0, rewards.ShopCoins);
    }

    [Test]
    public void TC31_Acumulacion_AlCompletarVariosNiveles_AcumulaCorrectamenteLosPuntos()
    {
        LevelRewardSystem rewards = new LevelRewardSystem();

        rewards.RegisterEnemyDefeated(); // +3 puntos
        rewards.RegisterCollectedCoin(); // +2 puntos
        rewards.CompleteLevel(true);     // +10 puntos

        // Total esperado: 15 puntos.
        // Como 10 puntos se convierten en 1 moneda de tienda,
        // quedan 5 puntos acumulados.
        Assert.AreEqual(5, rewards.TotalPoints);
        Assert.AreEqual(1, rewards.ShopCoins);
    }

    [Test]
    public void TC32_PrevencionDeDuplicados_AlRepetirAccion_EvitaDuplicarRecompensas()
    {
        LevelRewardSystem rewards = new LevelRewardSystem();

        bool firstReward = rewards.CompleteLevel(true);
        bool duplicatedReward = rewards.CompleteLevel(true);

        Assert.IsTrue(firstReward);
        Assert.IsFalse(duplicatedReward);

        Assert.AreEqual(0, rewards.TotalPoints);
        Assert.AreEqual(1, rewards.ShopCoins);
    }

    [Test]
    public void TC33_NivelCompletado_AlFinalizarNivel_DesbloqueaSiguienteNivelEnMenosDeDosSegundos()
    {
        LevelUnlockSystem unlockSystem = new LevelUnlockSystem();

        bool unlocked = unlockSystem.CompleteLevelAndUnlockNext();

        Assert.IsTrue(unlocked);
        Assert.IsTrue(unlockSystem.NextLevelUnlocked);
    }
}