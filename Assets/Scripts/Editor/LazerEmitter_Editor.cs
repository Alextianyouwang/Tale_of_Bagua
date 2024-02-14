
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LazerEmitter))]
[CanEditMultipleObjects]
public class LazerEmitter_Editor : Editor
{
    SerializedProperty _oriantation;
    LazerEmitter _main;
    int _previous_oriantation_value;
    private void OnEnable()
    {
        _main = target as LazerEmitter;
        _oriantation = serializedObject.FindProperty("OriantationOptions");
    }
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
       
        if (_previous_oriantation_value != _oriantation.enumValueIndex) 
            _main.Editor_ChangeOriantationUI();
        _previous_oriantation_value = _oriantation.enumValueIndex;
    }
}
