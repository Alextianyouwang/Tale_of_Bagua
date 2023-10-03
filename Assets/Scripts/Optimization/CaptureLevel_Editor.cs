
using UnityEngine;
using UnityEditor;

public class SceneCaptureWindow : EditorWindow
{
    private CaptureLevel _master;
    private string _path;
    private string _name;
    private LayerMask _mask;
    [MenuItem("Tool/Scene Capture")]
    public static void ShowWindow()
    {
        SceneCaptureWindow window = GetWindow<SceneCaptureWindow>("Scene Capture");
        window.minSize = new Vector2(600, 800);
        window.Show();
    }
    private void OnEnable()
    {
        _master = new CaptureLevel();
    }
    private void OnDisable() 
    {
    
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Capture", EditorStyles.boldLabel);
        _master.Cam = (Camera)EditorGUILayout.ObjectField("Camera", _master.Cam, typeof(Camera), true);
        if (!_master.Cam)
            return;

        _path = EditorGUILayout.TextField("Save Path", _path == null? "SceneTexture":_path);
        _name = EditorGUILayout.TextField("Name", _name);
        _mask = EditorGUILayout.MaskField("Culling Mask",_mask, UnityEditorInternal.InternalEditorUtility.layers);

        if (GUILayout.Button("Capture Scene"))
        {
            _master.CaptureCameraToTexture(_mask << 1,_path, _name);
        }
    }
}