using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioPreset", menuName = "CustomInspector/AudioPreset")]
public class SO_Audio : ScriptableObject
{
    public List<AudioClass> AudioClasses;
    public string IdScriptbleObject;
    [TextArea]
    public string Discription;
}
