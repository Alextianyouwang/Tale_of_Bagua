
using UnityEditor;
[CustomEditor(typeof(Lazer))]
[CanEditMultipleObjects]
public class LazerEmitter_Editor : Editor
{
    SerializedProperty _oriantation,_reflector;
    Lazer _main;
    int _previous_oriantation_value;
    bool _previous_reflector_value;
    private void OnEnable()
    {
        _main = target as Lazer;
        _oriantation = serializedObject.FindProperty("OriantationOptions");
       // _reflector = serializedObject.FindProperty("Reflector");
    }
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
       
   //     if (_previous_oriantation_value != _oriantation.enumValueIndex) 

       // if (_previous_reflector_value != _reflector.boolValue)
       //     _main.Editor_ChangeOriantationUI();
        _previous_oriantation_value = _oriantation.enumValueIndex;
        //_previous_reflector_value = _reflector.boolValue;
    }
}
