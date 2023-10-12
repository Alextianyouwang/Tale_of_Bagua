
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Unity.Burst.CompilerServices;

public class LevelGenerator_Editor : EditorWindow
{

    private LevelGenerator _generator;
    private GameObject _levelObj;

    private Material _levelMat;
    private Mesh _levelMesh;
    private float _levelDepth = 1f, _prev_levelDepth;

    private string _path;
    private string _name;

    private int _horizontalChunks = 32, _verticalChunks = 18;

    private Cell[,] _cells;
    private Cell[] _cellFlattenedList;
    private ComputeShader _levelVisual_cs;
    private RenderTexture _levelVisual_tex;
    private ComputeBuffer _levelVisual_buffer;
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
        if (!_generator.Cam)
            Debug.LogError("No Camera Detected");

        SceneView.duringSceneGui += OnSceneGUI; 

        PrepareVisualSetup(_generator.Cam.pixelWidth, _generator.Cam.pixelHeight);

    }
    
    private void PrepareVisualSetup(int texWidth, int texHeight)
    {
        _levelVisual_cs = (ComputeShader)Resources.Load("CS/CS_LevelVisual");
        _levelVisual_tex = RenderTexture.GetTemporary(texWidth,texHeight , 0, RenderTextureFormat.ARGB32);
        _levelVisual_tex.filterMode = FilterMode.Point;
        _levelVisual_tex.enableRandomWrite = true;
        _levelVisual_tex.Create();

        _levelVisual_cs.SetTexture(0, "Result", _levelVisual_tex);
        _levelVisual_cs.SetVector("_screenParam", new Vector4(_generator.Cam.pixelWidth, _generator.Cam.pixelHeight, 0, 0));
    }

    private void UpdateVisualSetup(int count, int stride) 
    {
        if(_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_buffer = new ComputeBuffer(count, stride);
        _levelVisual_buffer.SetData(_cellFlattenedList.Select(x => x.cellStruct).ToArray(), 0, 0, count);
        _levelVisual_cs.SetInt("_cellCount", count);
        _levelVisual_cs.SetBuffer(0, "_CellBuffer", _levelVisual_buffer);
        _levelVisual_cs.Dispatch(0, (_generator.Cam.pixelWidth / 10), (_generator.Cam.pixelHeight / 10), 1);
    }

    private void RemoveVisualSetup()
    {
        if (_levelVisual_buffer != null)
            _levelVisual_buffer.Dispose();
        _levelVisual_tex.Release();

    }

    private void OnDisable()
    {
        RemoveVisualSetup();

        SceneView.duringSceneGui -= OnSceneGUI;
    }
   

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Generator", EditorStyles.boldLabel);

        _generator.Cam = (Camera)EditorGUILayout.ObjectField("Camera", _generator.Cam, typeof(Camera), true);
        if (!_generator.Cam)
            return;

        _levelObj = (GameObject)EditorGUILayout.ObjectField("Level Card Template", _levelObj, typeof(GameObject), true) ;
        if (_levelObj && !_levelObj.GetComponent<MeshRenderer>())
            return;

        if (_levelObj)
            _levelMat = _levelObj.GetComponent<MeshRenderer>()?.sharedMaterial;
        _levelMat?.SetTexture("_MainTex", _levelVisual_tex);
        _levelMat = (Material)EditorGUILayout.ObjectField("Level Material", _levelMat, typeof(Material), true);
        if (_levelObj && _levelObj.GetComponent<MeshFilter>() && _levelObj.GetComponent<MeshFilter>().sharedMesh.vertexCount == 4)
            _levelMesh = _levelObj.GetComponent<MeshFilter>().sharedMesh;

        _levelDepth = EditorGUILayout.Slider("Level Depth", _levelDepth, _generator.Cam.nearClipPlane, _generator.Cam.farClipPlane);

        if (!_levelObj)
        {
            _path = EditorGUILayout.TextField("Save Path", _path == null ? "SceneTexture" : _path);
            _name = EditorGUILayout.TextField("Name", _name);
        }

        if (!_levelObj)
        if (GUILayout.Button("Create Level From Camera View"))
        {
            _levelMesh = _generator.CreateQuad(_generator.GetScreenInWorldSpace(_levelDepth));
            _levelObj = _generator.CreatePlaneLevel(_name + "_card", _levelMesh, _levelMat);
            SaveMesh(_levelMesh, _path, _name + "_mesh");
        }
        if (_prev_levelDepth != _levelDepth && _levelObj != null) 
        {
            if (_levelMesh)
                _generator.AdjustQuadDepth(_levelMesh, _levelDepth);
            else
                Debug.LogWarning("Level Mesh is Null");
        }


        if ( _levelMesh) 
        {
            if (_levelObj) 
            {
                _horizontalChunks = EditorGUILayout.IntField("X Chunk", _horizontalChunks < 1 ? 1 :_horizontalChunks);
                _horizontalChunks = _horizontalChunks < 1 ? 1 : _horizontalChunks;
                _verticalChunks = EditorGUILayout.IntField("Y Chunk", _verticalChunks < 1? 1 :_verticalChunks);
                _verticalChunks = _verticalChunks < 1 ? 1 : _verticalChunks;
            }
               
            if (GUILayout.Button("Create Cells")) 
            {
                _cells = _generator.CreateChunks(_levelMesh, _horizontalChunks, _verticalChunks, 1f);
                _cellFlattenedList = new Cell[_horizontalChunks * _verticalChunks];
                for (int i = 0; i < _horizontalChunks; i++)
                    for (int j = 0; j < _verticalChunks; j++) 
                        _cellFlattenedList[i * _verticalChunks + j] = _cells[i, j];
                
                UpdateVisualSetup(_horizontalChunks * _verticalChunks, sizeof(float) * 5);
            }
        }
        _prev_levelDepth = _levelDepth;
    }

    private void OnSceneGUI(SceneView sceneView) 
    {
        RaycastHit hit =  EditorClickSceneObject(sceneView);
        UpdateCellActivationVisual(hit);
    }

    private RaycastHit EditorClickSceneObject(SceneView sceneView)
    {
        RaycastHit hit = new RaycastHit();
        Camera cam = sceneView.camera;
        if (!cam)
            return hit;
        Event e = Event.current;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(mouseRay, out hit, 100f))
            return hit;
        return hit;
    }

    private void UpdateCellActivationVisual(RaycastHit hit) 
    {
        if (!hit.transform)
            return;
        if (hit.transform.gameObject != _levelObj)
            return;
        Event e = Event.current;
        Handles.color = Color.green;
        Handles.DrawSolidDisc(hit.point, hit.normal, 0.1f);
        Cell selected = null;
        if (_cellFlattenedList != null)
            selected = _generator.GetSelectedCell(_cellFlattenedList, hit.point);

        if (selected != null)
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                selected.isActive = true;
                UpdateVisualSetup(_horizontalChunks * _verticalChunks, sizeof(float) * 5);
            }
            else if (e.type == EventType.MouseDown && e.button == 1)
            {
                selected.isActive =false;
                UpdateVisualSetup(_horizontalChunks * _verticalChunks, sizeof(float) * 5);
            }

    }



    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
