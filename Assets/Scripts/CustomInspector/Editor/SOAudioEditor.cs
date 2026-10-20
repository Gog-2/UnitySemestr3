using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Audio))]
public class SOAudioEditor : Editor
{
    private SerializedProperty _sOListData;
    private SerializedProperty _sOId;
    private SerializedProperty _sODiscription;
    private string soListDrawData = "";
    private string soIdDrawData = "";
    private string soDrawDiscription = "";
    
    public override void OnInspectorGUI()
    {
        SO_Audio soAudio = (SO_Audio)target;

        EditorGUILayout.HelpBox($"List Data:\n {soListDrawData}", MessageType.Info);
        EditorGUILayout.LabelField("Id Data:", soIdDrawData);
        EditorGUILayout.HelpBox($"Discription Data:\n{soDrawDiscription}",MessageType.Info);
        
        if (GUILayout.Button("Draw List"))
        {
            StringBuilder sb = new StringBuilder();

            foreach (var audio in soAudio.AudioClasses)
            {
                string clipName = audio.Clip != null ? audio.Clip.name : "null";
                sb.AppendLine($"Name: {clipName}");
                sb.AppendLine($"Volume: {audio.Volume}");
                sb.AppendLine();
            }

            soListDrawData = sb.ToString();
        }
        if (GUILayout.Button("Draw Info"))
        {
            soIdDrawData = soAudio.IdScriptbleObject;
            soDrawDiscription = soAudio.Discription;
        }
        DrawDefaultInspector();
    }
}
