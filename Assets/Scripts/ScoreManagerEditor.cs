using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScoreManager))]
public class ScoreManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 繪製默認的Inspector

        ScoreManager scoreManager = (ScoreManager)target;

        if (GUILayout.Button("Add Score to Player 1"))
        {
            scoreManager.AddScorePlayer1(1);
        }

        if (GUILayout.Button("Add Score to Player 2"))
        {
            scoreManager.AddScorePlayer2(1);
        }

        if (GUILayout.Button("Show Result Menu"))
        {
            scoreManager.ShowResultMenu();
        }
    }
}
