using NUnit.Framework;

public class FakeProgressStorageRF12 : IProgressStorage
{
    public PlayerProgressData savedData;
    public bool forceFail = false;

    public bool SaveProgress(PlayerProgressData progress)
    {
        if (forceFail)
            return false;

        savedData = progress;
        return true;
    }

    public PlayerProgressData LoadProgress(string userId)
    {
        if (forceFail)
            return null;

        return savedData;
    }
}

public class RF12_LoadProgressTests
{
    // TC-43
    [Test]
    public void TC43_RetornoAlJuego_IniciarSesion_RecuperaProgresoCorrectamente()
    {
        var fakeDb = new FakeProgressStorageRF12();

        var firstSession = new GameProgressSystem(fakeDb);

        var savedProgress = new PlayerProgressData(
            "usuario_1", 4, 2200, 12
        );

        firstSession.SaveProgress(savedProgress);

        var secondSession = new GameProgressSystem(fakeDb);

        var loaded = secondSession.LoadProgress("usuario_1");

        Assert.IsNotNull(loaded);
        Assert.AreEqual(4, loaded.levelReached);
        Assert.AreEqual(2200, loaded.score);
        Assert.AreEqual(12, loaded.completedActivities);
    }

    // TC-44
    [Test]
    public void TC44_CierreDeSesion_SalirDelJuego_ProgresoPermaneceGuardado()
    {
        var fakeDb = new FakeProgressStorageRF12();

        var activeSession = new GameProgressSystem(fakeDb);

        var progressBeforeExit = new PlayerProgressData(
            "usuario_1", 5, 3000, 15
        );

        bool saved = activeSession.SaveProgress(progressBeforeExit);

        var newSession = new GameProgressSystem(fakeDb);

        var loaded = newSession.LoadProgress("usuario_1");

        Assert.IsTrue(saved);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(5, loaded.levelReached);
        Assert.AreEqual(3000, loaded.score);
        Assert.AreEqual(15, loaded.completedActivities);
    }

    // TC-45
    [Test]
    public void TC45_ErrorDeRecuperacion_SimularFallo_ElSistemaManejaError()
    {
        var fakeDb = new FakeProgressStorageRF12();

        var system = new GameProgressSystem(fakeDb);

        var progress = new PlayerProgressData(
            "usuario_1", 3, 1500, 10
        );

        system.SaveProgress(progress);

        fakeDb.forceFail = true;

        var loaded = system.LoadProgress("usuario_1");

        Assert.IsNull(loaded);
    }

    // TC-46
    [Test]
    public void TC46_ReinicioAplicacion_CerrarYAbrirJuego_ProgresoSeMantiene()
    {
        var fakeDb = new FakeProgressStorageRF12();

        var session1 = new GameProgressSystem(fakeDb);

        var progress = new PlayerProgressData(
            "usuario_1", 6, 4000, 20
        );

        session1.SaveProgress(progress);

        var session2 = new GameProgressSystem(fakeDb);

        var loaded = session2.LoadProgress("usuario_1");

        Assert.IsNotNull(loaded);
        Assert.AreEqual(6, loaded.levelReached);
        Assert.AreEqual(4000, loaded.score);
        Assert.AreEqual(20, loaded.completedActivities);
    }

    // TC-47
    [Test]
    public void TC47_RecuperacionTrasError_DatosSeMantienenConsistentes()
    {
        var fakeDb = new FakeProgressStorageRF12();

        var system = new GameProgressSystem(fakeDb);

        var progress = new PlayerProgressData(
            "usuario_1", 7, 5000, 25
        );

        system.SaveProgress(progress);

        fakeDb.forceFail = true;
        var failedLoad = system.LoadProgress("usuario_1");

        Assert.IsNull(failedLoad);

        fakeDb.forceFail = false;

        var recovered = system.LoadProgress("usuario_1");

        Assert.IsNotNull(recovered);
        Assert.AreEqual(7, recovered.levelReached);
        Assert.AreEqual(5000, recovered.score);
        Assert.AreEqual(25, recovered.completedActivities);
    }
}