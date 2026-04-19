using NUnit.Framework;

public class RF10_LevelUnlockTests
{
    [Test]
    public void TC34_IntentoSinCompletar_ImpideDesbloqueo()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        bool levelCompleted = false;

        bool canUnlock = system.CanUnlockNextLevel(levelCompleted);

        Assert.IsFalse(canUnlock);
    }

    [Test]
    public void TC35_ValidacionCorrecta_DesbloqueaNivelCorrecto()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        bool levelCompleted = true;

        bool canUnlock = system.CanUnlockNextLevel(levelCompleted);

        Assert.IsTrue(canUnlock);
    }

    [Test]
    public void TC36_SecuenciaDeNiveles_DesbloqueaEnOrdenCorrecto()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        int highestUnlockedLevel = 1;

        bool completedLevel1 = true;

        if (system.CanUnlockNextLevel(completedLevel1))
            highestUnlockedLevel = 2;

        bool canAccessLevel2 = system.CanAccessLevel(2, highestUnlockedLevel);
        bool canAccessLevel3 = system.CanAccessLevel(3, highestUnlockedLevel);

        Assert.IsTrue(canAccessLevel2);
        Assert.IsFalse(canAccessLevel3);
    }

    [Test]
    public void TC37_AccesoIndebido_RestringeNivelBloqueado()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        int highestUnlockedLevel = 1;

        bool canAccessLevel2 = system.CanAccessLevel(2, highestUnlockedLevel);

        Assert.IsFalse(canAccessLevel2);
    }
}
