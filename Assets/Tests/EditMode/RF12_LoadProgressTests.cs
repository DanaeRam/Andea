using NUnit.Framework;


public class RF12_LoadProgressTests 
{
    [Test]
    public void TC43_RetornoAlJuego_IniciarSesion_RecuperaProgresoCorrectamente()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();

        GameProgressSystem firstSession = new GameProgressSystem(fakeDb);

        PlayerProgressData savedProgress = new PlayerProgressData(
            "usuario_1",
            4,
            2200,
            12
        );

        firstSession.SaveProgress(savedProgress);

        GameProgressSystem secondSession = new GameProgressSystem(fakeDb);

        PlayerProgressData loaded = secondSession.LoadProgress("usuario_1");

        Assert.IsNotNull(loaded);
        Assert.AreEqual(4, loaded.levelReached);
        Assert.AreEqual(2200, loaded.score);
        Assert.AreEqual(12, loaded.completedActivities);
    }

    [Test]
    public void TC44_CierreDeSesion_SalirDelJuego_ProgresoPermaneceGuardado()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();

        GameProgressSystem activeSession = new GameProgressSystem(fakeDb);

        PlayerProgressData progressBeforeExit = new PlayerProgressData(
            "usuario_1",
            5,
            3000,
            15
        );

        bool saved = activeSession.SaveProgress(progressBeforeExit);

        GameProgressSystem newSessionAfterExit = new GameProgressSystem(fakeDb);

        PlayerProgressData loaded = newSessionAfterExit.LoadProgress("usuario_1");

        Assert.IsTrue(saved);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(5, loaded.levelReached);
        Assert.AreEqual(3000, loaded.score);
        Assert.AreEqual(15, loaded.completedActivities);
    }
}
