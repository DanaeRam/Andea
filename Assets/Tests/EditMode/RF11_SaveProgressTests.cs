using NUnit.Framework;
using System.Diagnostics;


public class RF11_SaveProgressTests
{
    [Test]
    public void TC38_GuardadoNormal_GuardaProgresoCompletoEnMenosDeCincoSegundos()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        PlayerProgressData progress = new PlayerProgressData(
            "usuario_1",
            3,
            1500,
            8
        );

        Stopwatch stopwatch = Stopwatch.StartNew();

        bool saved = system.SaveProgress(progress);

        stopwatch.Stop();

        PlayerProgressData loaded = system.LoadProgress("usuario_1");

        Assert.IsTrue(saved);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(3, loaded.levelReached);
        Assert.AreEqual(1500, loaded.score);
        Assert.AreEqual(8, loaded.completedActivities);
        Assert.Less(stopwatch.Elapsed.TotalSeconds, 5);
    }

    [Test]
    public void TC39_ErrorDeGuardado_NotificaErrorYEvitaGuardarDatosIncorrectos()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        fakeDb.shouldFailSave = true;

        GameProgressSystem system = new GameProgressSystem(fakeDb);

        PlayerProgressData progress = new PlayerProgressData(
            "usuario_1",
            2,
            900,
            4
        );

        bool saved = system.SaveProgress(progress);

        PlayerProgressData loaded = system.LoadProgress("usuario_1");

        Assert.IsFalse(saved);
        Assert.IsNull(loaded);
    }

    [Test]
    public void TC40_GuardadoContinuo_MantieneIntegridadDeLosDatos()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        PlayerProgressData firstSave = new PlayerProgressData(
            "usuario_1",
            1,
            500,
            2
        );

        PlayerProgressData secondSave = new PlayerProgressData(
            "usuario_1",
            2,
            900,
            5
        );

        PlayerProgressData thirdSave = new PlayerProgressData(
            "usuario_1",
            3,
            1400,
            9
        );

        system.SaveProgress(firstSave);
        system.SaveProgress(secondSave);
        system.SaveProgress(thirdSave);

        PlayerProgressData loaded = system.LoadProgress("usuario_1");

        Assert.IsNotNull(loaded);
        Assert.AreEqual(3, loaded.levelReached);
        Assert.AreEqual(1400, loaded.score);
        Assert.AreEqual(9, loaded.completedActivities);
    }

    [Test]
    public void TC41_CierreInesperado_ConservaUltimoProgresoGuardado()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        PlayerProgressData validProgress = new PlayerProgressData(
            "usuario_1",
            2,
            1000,
            5
        );

        system.SaveProgress(validProgress);

        fakeDb.shouldFailSave = true;

        PlayerProgressData interruptedProgress = new PlayerProgressData(
            "usuario_1",
            3,
            1600,
            8
        );

        bool savedAfterFailure = system.SaveProgress(interruptedProgress);

        PlayerProgressData loaded = system.LoadProgress("usuario_1");

        Assert.IsFalse(savedAfterFailure);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(2, loaded.levelReached);
        Assert.AreEqual(1000, loaded.score);
        Assert.AreEqual(5, loaded.completedActivities);
    }

    [Test]
    public void TC42_InterrupcionDeGuardado_EvitaCorrupcionDeDatos()
    {
        FakeProgressStorage fakeDb = new FakeProgressStorage();
        GameProgressSystem system = new GameProgressSystem(fakeDb);

        PlayerProgressData originalProgress = new PlayerProgressData(
            "usuario_1",
            1,
            300,
            1
        );

        system.SaveProgress(originalProgress);

        fakeDb.shouldInterruptSave = true;

        PlayerProgressData corruptedAttempt = new PlayerProgressData(
            "usuario_1",
            99,
            999999,
            999
        );

        bool saved = system.SaveProgress(corruptedAttempt);

        PlayerProgressData loaded = system.LoadProgress("usuario_1");

        Assert.IsFalse(saved);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(1, loaded.levelReached);
        Assert.AreEqual(300, loaded.score);
        Assert.AreEqual(1, loaded.completedActivities);
    }
}
