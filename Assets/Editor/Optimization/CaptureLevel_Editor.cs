
using UnityEngine;
using UnityEditor;

public class SceneCaptureWindow : EditorWindow
{
    private CaptureLevel _master;
    private string _path;
    private string _name;
    private LayerMask _mask;

    private GameObject _levelObj;
    private Material _levelMat;
    private Mesh _levelMesh;
    private float _levelDepth = 1f, _prev_levelDepth;
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
        _master.Cam = FindObjectOfType<Camera>();
    }
    private void OnDisable() 
    {
    
    }
    public static void DrawSeparator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Capture", EditorStyles.boldLabel);
       
        _master.Cam = (Camera)EditorGUILayout.ObjectField("Camera", _master.Cam , typeof(Camera), true);
        if (!_master.Cam)
            return;

        _path = EditorGUILayout.TextField("Save Path", _path == null? "SceneTexture":_path);
        _name = EditorGUILayout.TextField("Name", _name);
        _mask = EditorGUILayout.MaskField("Culling Mask",_mask, UnityEditorInternal.InternalEditorUtility.layers);

        if (GUILayout.Button("Capture Scene"))
        {
            _master.CaptureCameraToTexture(_mask << 1,_path, _name);
        }

        DrawSeparator();
        _levelDepth = EditorGUILayout.Slider("Level Depth",_levelDepth, _master.Cam.nearClipPlane, _master.Cam.farClipPlane);
        _levelMat = (Material)EditorGUILayout.ObjectField("Level Material",_levelMat, typeof(Material),true);

        
        if (GUILayout.Button("Create Level From Camera View"))
        {
            if (_levelObj)
                DestroyImmediate(_levelObj);
            _levelMesh = _master.CreateQuad(_master.GetScreenInWorldSpace(_levelDepth));
            _levelObj = _master.CreatePlaneLevel("Test",_levelMesh ,_levelMat);
        }
        if (_prev_levelDepth != _levelDepth && _levelObj != null)
            _master.AdjustQuadDepth(_levelMesh, _levelDepth);
        _prev_levelDepth = _levelDepth;
    }
}