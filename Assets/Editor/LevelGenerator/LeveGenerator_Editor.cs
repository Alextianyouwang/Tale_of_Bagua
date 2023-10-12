
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelGenerator_Editor : EditorWindow
{

    private LevelGenerator _generator;
    private GameObject _levelObj;

    private Material _levelMat;
    private Mesh _levelMesh;
    private float _levelDepth = 1f, _prev_levelDepth;

    private string _path;
    private string _name;
    [MenuItem("Tool/Level Generator")]
    public static void ShowWindow()
    {
        LevelGenerator_Editor window = GetWindow<LevelGenerator_Editor>("Level Generator");
        window.minSize = new Vector2(600, 800);
        window.Show();
    }

    private void OnEnable()
    {

        _generator = new LevelGenerator();
        _generator.Cam = FindObjectOfType<Camera>();

    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Generator", EditorStyles.boldLabel);

        _generator.Cam = (Camera)EditorGUILayout.ObjectField("Camera", _generator.Cam, typeof(Camera), true);
        if (!_generator.Cam)
            return;

        _path = EditorGUILayout.TextField("Save Path", _path == null? "SceneTexture" : _path);
        _name = EditorGUILayout.TextField("Name", _name);

        _levelDepth = EditorGUILayout.Slider("Level Depth", _levelDepth, _generator.Cam.nearClipPlane, _generator.Cam.farClipPlane);
        _levelMat = (Material)EditorGUILayout.ObjectField("Level Material", _levelMat, typeof(Material), true);
        _levelObj = (GameObject)EditorGUILayout.ObjectField("Level Card Template", _levelObj, typeof(GameObject), true);
        if (_levelObj && _levelObj.GetComponent<MeshFilter>() && _levelObj.GetComponent<MeshFilter>().sharedMesh.vertexCount == 4)
            _levelMesh = _levelObj.GetComponent<MeshFilter>().sharedMesh;

        if (!_levelObj)
        if (GUILayout.Button("Create Level From Camera View"))
        {
            if (!_levelObj)
            {
                _levelMesh = _generator.CreateQuad(_generator.GetScreenInWorldSpace(_levelDepth));
                _levelObj = _generator.CreatePlaneLevel(_name + "_card", _levelMesh, _levelMat);
            }
            if (_levelMesh)
                SaveMesh(_levelMesh, _path, _name + "_mesh");
            else
                Debug.LogWarning("Level Mesh is Null");
        }
        if (_prev_levelDepth != _levelDepth && _levelObj != null) 
        {
            if (_levelMesh)
                _generator.AdjustQuadDepth(_levelMesh, _levelDepth);
            else
                Debug.LogWarning("Level Mesh is Null");
        }
            
        _prev_levelDepth = _levelDepth;
    }

    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
