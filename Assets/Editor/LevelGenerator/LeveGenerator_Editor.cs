
using UnityEngine;
using UnityEditor;
using System.IO;
public class LevelGenerator_Editor : EditorWindow
{
    private LevelGenerator _generator;
    private GameObject _levelObj;
    private Material _levelMat;
    private Mesh _levelMesh;
    private float _levelDepth = 10f, _prev_levelDepth;
    private string _path, _name;
    private int _horizontalChunks = 32, _verticalChunks = 18;

    private Cell[] _cells;

    private LevelGenerator_Visual _levelVisual;

    private bool _canEditCells = false;
    [MenuItem("Tool/Level Generator")]
    public static void ShowWindow()
    {
        LevelGenerator_Editor window = GetWindow<LevelGenerator_Editor>("Level Generator");
        window.minSize = new Vector2(600, 800);
        window.Show();
    }

    private void OnEnable()
    {
        _levelVisual = new LevelGenerator_Visual();
        _generator = new LevelGenerator();

        _levelMat = EditorUtil.GetObject<Material>("M_LevelTemplate.mat");
        _generator.Cam = FindObjectOfType<Camera>();
        if (!_generator.Cam)
            Debug.LogError("No Camera Detected");

       _levelVisual.PrepareVisualSetup(_generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
        SceneView.duringSceneGui += OnSceneGUI;
    }


    private void OnDisable()
    {
        _levelVisual.RemoveVisualSetup();
        SceneView.duringSceneGui -= OnSceneGUI;
    }
   

    private void OnGUI()
    {
        if (!_levelObj)
        {
            _levelMesh = null;
            _cells = null;
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Camera Source", EditorStyles.boldLabel);
        _generator.Cam = (Camera)EditorGUILayout.ObjectField("Camera", _generator.Cam, typeof(Camera), true);
        if (!_generator.Cam)
            return;
        EditorUtil.DrawSeparator();

        EditorGUILayout.LabelField("Start With Generated Template:", EditorStyles.boldLabel);
        _levelObj = (GameObject)EditorGUILayout.ObjectField("Level Card Template", _levelObj, typeof(GameObject), true) ;
    

        if (_levelObj && !_levelObj.GetComponent<MeshRenderer>())
            return;
        if (_levelObj && _levelObj.GetComponent<MeshFilter>() && _levelObj.GetComponent<MeshFilter>().sharedMesh.vertexCount == 4)
            _levelMesh = _levelObj.GetComponent<MeshFilter>().sharedMesh;
    
  

        if (!_levelObj)
        {
            EditorGUILayout.LabelField("Or Create New Level:", EditorStyles.boldLabel);
            _path = EditorGUILayout.TextField("Save Path", _path == null ? "SceneTexture" : _path);
            _name = EditorGUILayout.TextField("Name", _name);
  
        }

        if (_levelObj)
            _levelMat = _levelObj.GetComponent<MeshRenderer>()?.sharedMaterial;
        _levelMat?.SetTexture("_MainTex", _levelVisual. _levelVisual_tex);
        _levelMat = (Material)EditorGUILayout.ObjectField("Level Material", _levelMat, typeof(Material), true);


        if (_levelMesh)
            _levelDepth = EditorGUILayout.Slider("Level Depth", _levelDepth, _generator.Cam.nearClipPlane, _generator.Cam.farClipPlane);

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
            {
                _cells = _generator.AdjustCellData(_cells,_horizontalChunks,_verticalChunks, _levelMesh, 1f);
                _generator.AdjustQuadDepth(_levelMesh, _levelDepth);
                _levelObj.GetComponent<MeshCollider>().sharedMesh = _levelMesh;
            }
               
            else
                Debug.LogWarning("Level Mesh is Null");
        }
 

        if ( _levelMesh) 
        {
                _horizontalChunks = EditorGUILayout.IntField("X Chunk", _horizontalChunks < 1 ? 1 :_horizontalChunks);
                _horizontalChunks = _horizontalChunks < 1 ? 1 : _horizontalChunks;
                _verticalChunks = EditorGUILayout.IntField("Y Chunk", _verticalChunks < 1? 1 :_verticalChunks);
                _verticalChunks = _verticalChunks < 1 ? 1 : _verticalChunks;

            if (GUILayout.Button("Create Cells"))
            {
                _cells = _generator.CreateChunks(_levelMesh, _horizontalChunks, _verticalChunks, 1f);
                _levelVisual.UpdateVisualSetup(_cells, sizeof(float) * 5,_generator.Cam.pixelWidth,_generator.Cam.pixelHeight);
                _canEditCells = false;
            }

        }

        if (_cells != null) 
        {
            GUI.enabled = !_canEditCells;
            if (GUILayout.Button(_canEditCells? "Paint: RMB     Erase: Alt + RMB ": "Edit Cells"))
            {
                _canEditCells = true;
                foreach (Cell c in _cells)
                    c.isActive = true;
                _levelVisual.UpdateVisualSetup(_cells, sizeof(float) * 5, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
            }
            GUI.enabled = true;
        }
           


        _prev_levelDepth = _levelDepth;
    }
    private void OnSceneGUI(SceneView sceneView) 
    {
        if (!_canEditCells)
            return;
        RaycastHit hit = EditorUtil. EditorClickSceneObject(sceneView);
        UpdateCellActivationVisual(hit);
    }

    private void UpdateCellActivationVisual(RaycastHit hit) 
    {
        if (!hit.transform)
            return;
        if (hit.transform.gameObject != _levelObj)
            return;
        Event e = Event.current;
        Handles.color = e.alt ? Color.green : Color.red;
        Cell selected = null;
        if (_cells != null)
            selected = _generator.GetSelectedCell(_cells, hit.point);

        if (selected != null)
            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1)
            {
                selected.isActive = e.alt;
                _levelVisual.UpdateVisualSetup(_cells, sizeof(float) * 5, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
                e.Use();
            }
        Handles.DrawSolidDisc(hit.point, hit.normal, 0.2f);

    }



    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
