using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneButton
{
    public string buttonName; // ボタン名
    public string sceneName; // 切り替えるシーン名
}

public class SceneSwitcher : MonoBehaviour
{
    public SceneButton[] sceneButtons; // ボタンのリスト

#if UNITY_EDITOR
    [CustomEditor(typeof(SceneSwitcher))] // SceneSwitcherスクリプトのカスタムエディター
    public class SceneSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SceneSwitcher switcher = (SceneSwitcher)target;

            // 各シーンのボタンを表示
            if (switcher.sceneButtons != null)
            {
                foreach (var button in switcher.sceneButtons)
                {
                    if (GUILayout.Button(button.buttonName))
                    {
                        switcher.SwitchScene(button.sceneName);
                    }
                }
            }
        }
    }
#endif

    // 指定されたシーンに切り替えるメソッド
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // SceneManagerを使用してシーンを読み込む
    }
}