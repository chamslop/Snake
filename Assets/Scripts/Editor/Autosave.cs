#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutosaveOnRun : ScriptableObject {

	static AutosaveOnRun() {
		EditorApplication.playModeStateChanged += autoSave;
	}

  private static void autoSave(PlayModeStateChange pmsc) {
    if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;
    EditorSceneManager.SaveOpenScenes();
    AssetDatabase.SaveAssets();
  }
  
}
#endif