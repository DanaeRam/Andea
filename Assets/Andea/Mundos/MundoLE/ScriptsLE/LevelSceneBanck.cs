using UnityEngine;

[CreateAssetMenu(fileName = "LevelSceneBank", menuName = "ANDEA/Level Scene Bank")]
public class LevelSceneBank : ScriptableObject
{
    public string levelName;
    public string[] platformSceneNames;
}