
using UnityEditor;
[CustomEditor(typeof(LazerEmitter))]
[CanEditMultipleObjects]
public class LazerEmitter_Editor : Editor
{
    SerializedProperty _oriantation,_reflector;
    LazerEmitter _main;
    int _previous_oriantation_value;
    bool _previous_reflector_value;
    private void OnEnable()
    {
        _main = target as LazerEmitter;
        _oriantation = serializedObject.FindProperty("OriantationOptions");
        _reflector = serializedObject.FindProperty("Reflector");
    }
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
       
        if (_previous_oriantation_value != _oriantation.enumValueIndex) 
            _main.Editor_ChangeOriantationUI();
        if (_previous_reflector_value != _reflector.boolValue)
            _main.Editor_ChangeOriantationUI();
        _previous_oriantation_value = _oriantation.enumValueIndex;
        _previous_reflector_value = _reflector.boolValue;
    }
}
