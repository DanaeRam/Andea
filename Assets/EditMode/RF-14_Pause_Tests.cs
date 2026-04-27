using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public class PausaTests
{
    private GameObject go;
    private Pausa pausa;

    [SetUp]
    public void Setup()
    {
        go = new GameObject("PausaTestObject");

        go.AddComponent<UIDocument>();

        pausa = go.AddComponent<Pausa>();

        // Llamar Start manualmente
        pausa.SendMessage("Start");
    }

    [Test]
    public void TC51_PausarJuego_DetieneInteracciones()
    {
        pausa.Pausar();

        Assert.IsTrue(pausa.estaPausado);
        Assert.AreEqual(0f, Time.timeScale);
        Assert.IsTrue(AudioListener.pause);
    }

    [Test]
    public void TC52_ReanudarJuego_RestableceInteracciones()
    {
        pausa.Pausar();
        pausa.Reanudar();

        Assert.IsFalse(pausa.estaPausado);
        Assert.AreEqual(1f, Time.timeScale);
        Assert.IsFalse(AudioListener.pause);
    }

    [Test]
    public void TC53_InteraccionBloqueada_NoPermiteInteraccionEnPausa()
    {
        pausa.Pausar();

        Assert.IsFalse(!pausa.estaPausado);
    }

    [Test]
    public void TC54_PausaDuranteAccion_MantieneEstadoPausado()
    {
        pausa.Pausar();

        Assert.IsTrue(pausa.estaPausado);
    }

    [Test]
    public void TC55_UsoRepetido_PausarVariasVeces_NoRompeEstado()
    {
        pausa.Pausar();
        pausa.Pausar();
        pausa.Pausar();

        Assert.IsTrue(pausa.estaPausado);
    }

    [TearDown]
    public void Teardown()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Object.DestroyImmediate(go);
    }
}