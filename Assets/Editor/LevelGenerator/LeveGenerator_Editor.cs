
using UnityEngine;
using UnityEditor;
using System.IO;
using static Cell;

public class LevelGenerator_Editor : EditorWindow
{
    private LevelGenerator _generator;
    private GameObject _levelObj;
    private Material _levelMat;
    private Mesh _levelMesh;
    private float _levelDepth = 15f, _prev_levelDepth;
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


        _path = EditorGUILayout.TextField("Save Path", _path == null ? "LevelData" : _path);
        _name = EditorGUILayout.TextField("Name", _name);

        EditorUtil.DrawSeparator();
        if (!_levelObj)
            EditorGUILayout.LabelField("Start With Level Template:", EditorStyles.boldLabel);
        _levelObj = (GameObject)EditorGUILayout.ObjectField("Level Card Template", _levelObj, typeof(GameObject), true) ;
    

        if (_levelObj && !_levelObj.GetComponent<MeshRenderer>())
            return;
        if (_levelObj && _levelObj.GetComponent<MeshFilter>() && _levelObj.GetComponent<MeshFilter>().sharedMesh.vertexCount == 4)
            _levelMesh = _levelObj.GetComponent<MeshFilter>().sharedMesh;

        if (!_levelObj)
            EditorGUILayout.LabelField("Or Generate New Level:", EditorStyles.boldLabel);

        if (_levelObj)
            _levelMat = _levelObj.GetComponent<MeshRenderer>()?.sharedMaterial;
        _levelMat?.SetTexture("_MainTex", _levelVisual._levelVisual_tex);
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
                if (_cells != null)
                    _cells = _generator.AdjustCellData(_cells,_horizontalChunks,_verticalChunks, _levelMesh, 1f);
                _generator.AdjustQuadDepth(_levelMesh, _levelDepth);
                _levelObj.GetComponent<BoxCollider>().size = _levelMesh.bounds.size;
                _levelObj.GetComponent<BoxCollider>().center = _levelMesh.bounds.center;
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
                _levelVisual.UpdateVisualSetup(_cells, sizeof(float) * 5);
                _levelVisual.UpdateSearchClosestSetup(_cells.Length);
                _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
                _canEditCells = false;
            }

        }

        if (_cells != null) 
        {
            GUI.enabled = !_canEditCells;
            if (GUILayout.Button(_canEditCells? "Paint: RMB    Erase: Alt + RMB    Enlarge: Shift": "Edit Cells"    ))
            {
                _canEditCells = true;
                foreach (Cell c in _cells)
                    c.isActive = true;
                _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
            }
            GUI.enabled = true;
        }

        if (_canEditCells && _levelMesh)
            if (GUILayout.Button( "Save Level Collider"))
            {
                //Only Convex collider is supported... need optimization
                //SaveMesh(_generator.GenerateLevelMesh(_cells), _path, _name + "_levelMesh");
                _generator.GenerateLevelObject(_cells,_name + "_levelObject");
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

        Cell selected = null;
        if (_cells != null)
            selected = _generator.GetSelectedCell(_cells, hit.point);
      
        if (selected != null)
        {
            float r = Mathf.Sqrt(_levelMesh.bounds.size.x * _levelMesh.bounds.size.z / 10f / Mathf.PI);
            float handleRadius = e.shift ? r : Mathf.Max(selected.size.x/2f, selected.size.z/2f);
            _levelVisual.SetPaintRadius(r);
            Handles.color = e.alt ? Color.green : Color.red;
            Handles.DrawWireDisc(hit.point, hit.normal, handleRadius);

            Handles.color = Color.blue;
            Handles.DrawWireCube(selected.position + selected.size.y/2 * Vector3.up, selected.size);
            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1)
            {

                selected.isActive = e.alt;
          
                _levelVisual.TogglePaintAndErase(e.alt ? 0 : 1);
                if (e.shift) 
                    _levelVisual.UpdateSearchClosestPerFrame(hit.point, _cells);
                _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);

                e.Use();
            }
        }
     

    }



    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
