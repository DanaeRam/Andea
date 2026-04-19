using NUnit.Framework;

public class QuizAttemptSystemTests
{
    [Test]
    public void TC23_PrimerIntentoFallido_PermiteNuevoIntentoYMuestraPista()
    {
        QuizAttemptSystem quiz = new QuizAttemptSystem(3);

        quiz.RegisterWrongAnswer();

        Assert.IsTrue(quiz.CanAnswer());
        Assert.IsTrue(quiz.HintShown);
        Assert.AreEqual(1, quiz.AttemptsUsed);
        Assert.AreEqual(2, quiz.RemainingAttempts);
    }
    [Test]
    public void TC24_LimiteDeIntentos_AlFallarTresVeces_BloqueaNuevosIntentos()
    {
        QuizAttemptSystem quiz = new QuizAttemptSystem(3);

        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();

        Assert.IsTrue(quiz.IsBlocked);
        Assert.IsFalse(quiz.CanAnswer());
        Assert.AreEqual(0, quiz.RemainingAttempts);
    }

    [Test]
    public void TC25_VisualizacionDePista_AlFallarUnaVez_MuestraPista()
    {
        QuizAttemptSystem quiz = new QuizAttemptSystem(3);

        quiz.RegisterWrongAnswer();

        Assert.IsTrue(quiz.HintShown);
        Assert.IsTrue(quiz.CanAnswer());
        Assert.AreEqual(2, quiz.RemainingAttempts);
    }

    [Test]
    public void TC26_TercerIntento_AlFallarDosVeces_PermiteUltimoIntento()
    {
        QuizAttemptSystem quiz = new QuizAttemptSystem(3);

        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();

        Assert.IsTrue(quiz.IsThirdAttemptAvailable());
        Assert.IsTrue(quiz.CanAnswer());
        Assert.AreEqual(1, quiz.RemainingAttempts);
    }

    [Test]
    public void TC27_ExcesoDeIntentos_AlFallarTresVeces_NoPermiteIntentosExtra()
    {
        QuizAttemptSystem quiz = new QuizAttemptSystem(3);

        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();
        quiz.RegisterWrongAnswer();

        Assert.IsTrue(quiz.IsBlocked);
        Assert.IsFalse(quiz.CanAnswer());
        Assert.AreEqual(3, quiz.AttemptsUsed);
    }
}