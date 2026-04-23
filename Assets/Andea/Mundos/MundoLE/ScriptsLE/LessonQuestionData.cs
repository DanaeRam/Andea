using System;
using UnityEngine;

[Serializable]
public class LessonQuestionData
{
    [TextArea(2, 5)]
    public string questionText;

    public string option1;
    public string option2;
    public string option3;
    public string option4;

    [Range(1, 4)]
    public int correctOption;

    [TextArea(2, 4)]
    public string correctFeedback;

    [TextArea(2, 4)]
    public string wrongFeedback;
}

[Serializable]
public class LessonQuestionBankJson
{
    public string levelName;
    public string lessonName;
    public LessonQuestionData[] questions;
}