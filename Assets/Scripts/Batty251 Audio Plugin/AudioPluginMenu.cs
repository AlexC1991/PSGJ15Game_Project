#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AudioPluginMenu
{
    [MenuItem("Audio Plugin/Create Audio Data")]
    public static void CreateAudioData()
    {
        ScriptableAudioFile audioData = ScriptableObject.CreateInstance<ScriptableAudioFile>();

         // Prompt for save options
        SaveOptionsPrompt(audioData);

        AssetDatabase.CreateAsset(audioData, "Assets/AudioData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = audioData;

    }
        
         private static void SaveOptionsPrompt(ScriptableAudioFile audioData)
    {
        // Specify the default directory and filename
        string defaultName = "NewAudioData";
        string directory = "Assets/Custom Audio Clips/";

        // Open a save file panel to get the desired path and name
        string path = EditorUtility.SaveFilePanel("Save Audio Data", directory, defaultName, "asset");

        // If the user didn't cancel the operation, save the asset
        if (!string.IsNullOrEmpty(path))
        {
            // Ensure the path is within the Assets folder
            if (path.StartsWith(Application.dataPath))
            {
                // Convert the full path to a relative path
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

                // Create the directory if it doesn't exist
                string folderPath = System.IO.Path.GetDirectoryName(relativePath);
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Custom Audio Clips");
                }

                // Save the asset at the specified location
                AssetDatabase.CreateAsset(audioData, relativePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Unable to save outside the Assets folder.");
            }
        }
    }
        
}
#endif
