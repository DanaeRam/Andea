using UnityEngine;

[CreateAssetMenu(fileName = "LessonQuestionBank", menuName = "ANDEA/Lesson Question Bank")]
public class LessonQuestionBank : ScriptableObject
{
    public string levelName;
    public string lessonName;
    public LessonQuestionData[] questions;
}