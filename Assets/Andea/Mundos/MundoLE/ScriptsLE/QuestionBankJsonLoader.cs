using UnityEngine;

public static class QuestionBankJsonLoader
{
    public static LessonQuestionBankJson LoadQuestionBank(string resourcePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

        if (jsonFile == null)
        {
            Debug.LogError("No se encontró el archivo JSON en Resources: " + resourcePath);
            return null;
        }

        LessonQuestionBankJson bank = JsonUtility.FromJson<LessonQuestionBankJson>(jsonFile.text);

        if (bank == null)
        {
            Debug.LogError("No se pudo convertir el JSON a LessonQuestionBankJson.");
            return null;
        }

        return bank;
    }
}
