
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.Linq;
using UnityEngine.Rendering;
using Cyan;
using UnityEngine.Experimental.Rendering.Universal;
using System.IO;

public class SceneCaptureWindow : EditorWindow
{
    private CaptureLevel _master;
    private string _path;
    private string _name;
    private LayerMask _mask;
    private LayerMask _depthMask;

    private GameObject _levelObj;
    private Material _levelMat;
    private Mesh _levelMesh;
    private Texture2D _colSceneTex;
    private Texture2D _depthSceneTex;
    private bool _renderDepth,_prev_renderDepth;
    private float _levelDepth = 1f, _prev_levelDepth;

    private UniversalAdditionalCameraData _urp_cam;

    private UniversalRendererData _rendererData;
    private ScriptableRendererFeature _feature;
    private string _featureName = "Render Depth";
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
        _urp_cam = _master.Cam.GetComponent<UniversalAdditionalCameraData>();
        _rendererData = GetObject<UniversalRendererData>("URP_Renderer.asset");
        
    }
    private void OnDisable() 
    {
    
    }
    public static void DrawSeparator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
    }
    T GetObject<T>(string objectNameWithExtension) where T : Object
    {
        string[] paths = Directory.GetFiles(Application.dataPath, objectNameWithExtension, SearchOption.AllDirectories);
        if (paths.Length == 0)
        {
            Debug.Log("Couldn't find '" + objectNameWithExtension + "', please add it manually");
            return null;
        }

        string assetPath = paths[0].Replace("\\", "/").Replace(Application.dataPath, "");
        string assetFullPath = "Assets" + assetPath;

        T loadedObject = AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
        if (loadedObject == null)
        {
            Debug.Log("Failed to load asset at path: " + assetFullPath);
            return null;
        }

        return loadedObject;
    }
    private bool TryGetFeature(out ScriptableRendererFeature feature) 
    {
        feature = _rendererData.rendererFeatures.Where(f => f.name == _featureName).FirstOrDefault();
        return feature != null;
    }
    public void SaveTexture(Texture2D image, string path, string name)
    {
        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, path, name + ".png"), bytes);
        Debug.Log($"Saved camera capture to: {path}");
        AssetDatabase.Refresh();
    }

    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets",path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
            _urp_cam.renderPostProcessing = false;
            _colSceneTex = _master.CaptureCameraToTexture(_mask << 1);
            _urp_cam.renderPostProcessing = true;

            SaveTexture(_colSceneTex, _path, _name);
        }
        DrawSeparator();

        _renderDepth = EditorGUILayout.Toggle("Render Depth (URP)", _renderDepth);

        if (_renderDepth)
        {
            _rendererData = (UniversalRendererData)EditorGUILayout.ObjectField("URP Renderer", _rendererData, typeof(UniversalRendererData), false);
            _featureName = EditorGUILayout.TextField("Featrue Name", _featureName);
            _depthMask = EditorGUILayout.MaskField("Depth Capture Mask", _depthMask, UnityEditorInternal.InternalEditorUtility.layers);

            if (_feature) 
            {
                RenderObjects ro = _feature as RenderObjects;
                ro.settings.filterSettings.LayerMask = _depthMask << 1;
                ro.Create();
            }
        
            if (GUILayout.Button("Capture Depth")) {
                if (TryGetFeature(out _feature))
                    _feature.SetActive(true);
                _urp_cam.renderPostProcessing = false;
                _depthSceneTex = _master.CaptureCameraToTexture(_depthMask << 1);
                _urp_cam.renderPostProcessing = true;
                SaveTexture(_depthSceneTex, _path, _name + "_depth");
                if (TryGetFeature(out _feature))
                    _feature.SetActive(false);
            }
        }
        else
            if (TryGetFeature(out _feature))
            _feature.SetActive(false);



        DrawSeparator();

        if (_colSceneTex && _depthSceneTex) 
        {
            if (GUILayout.Button("Merge"))
                SaveTexture(_master.MergeTextures(_colSceneTex,_depthSceneTex), _path, _name + "_comp");
            DrawSeparator();
        }

        _levelDepth = EditorGUILayout.Slider("Level Depth",_levelDepth, _master.Cam.nearClipPlane, _master.Cam.farClipPlane);
        _levelMat = (Material)EditorGUILayout.ObjectField("Level Material",_levelMat, typeof(Material),true);

        
        if (GUILayout.Button("Create Level From Camera View"))
        {
            if (_levelObj)
                DestroyImmediate(_levelObj);
            _levelMesh = _master.CreateQuad(_master.GetScreenInWorldSpace(_levelDepth));
            SaveMesh(_levelMesh, _path, _name + "_mesh");
            _levelObj = _master.CreatePlaneLevel(_name+"_card",_levelMesh ,_levelMat);
        }
        if (_prev_levelDepth != _levelDepth && _levelObj != null)
            _master.AdjustQuadDepth(_levelMesh, _levelDepth);
        if(_prev_renderDepth != _renderDepth && _renderDepth)
            if (TryGetFeature(out _feature))
                _feature.SetActive(true);
        _prev_levelDepth = _levelDepth;
        _prev_renderDepth = _renderDepth;
    }
}