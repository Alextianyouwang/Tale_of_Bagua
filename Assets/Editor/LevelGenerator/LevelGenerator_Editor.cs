
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using static Codice.Client.BaseCommands.Import.Commit;

public class LevelGenerator_Editor : EditorWindow
{
    private LevelGenerator _generator;
    private GameObject _levelObj;
    private Material _levelMat;
    private Mesh _levelMesh;
    private LevelDataObject _levelDataObject;
    private float _levelDepth = 15f, _prev_levelDepth;
    private string _path, _meshName;
    private int _horizontalChunks = 32, _verticalChunks = 18;

    private Cell[] _cells;

    private LevelGenerator_Visual _levelVisual;

    private bool _canEditCells = false;
    private bool _isMouseDown = false;

    private LevelGenerator_CommandStack _commandStack;
    private LevelGenerator_MouseDragAction _currentAction;
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
        _commandStack = new LevelGenerator_CommandStack();

        _levelMat = EditorUtil.GetObject<Material>("M_LevelTemplate.mat");
        _generator.Cam = FindObjectOfType<Camera>();
        if (!_generator.Cam)
            Debug.LogError("No Camera Detected");
        _generator.SetupCamera();

        _levelVisual.PrepareVisualSetup(_generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
        SceneView.duringSceneGui += OnSceneGUI;
    }


    private void OnDisable()
    {
        _levelVisual.RemoveVisualSetup();
        _levelDataObject = null;
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


        _path = EditorGUILayout.TextField("Mesh Save Path", _path == null ? "LevelData" : _path);
        _meshName = EditorGUILayout.TextField("Mesh Name", _meshName);

        EditorUtil.DrawSeparator();
        if (!_levelObj)
            EditorGUILayout.LabelField("Start With Level Template:", EditorStyles.boldLabel);
        _levelObj = (GameObject)EditorGUILayout.ObjectField("Level Card Template", _levelObj, typeof(GameObject), true) ;
    

        if (_levelObj && !_levelObj.GetComponent<MeshRenderer>())
            return;
        if (_levelObj && _levelObj.GetComponent<MeshFilter>() && _levelObj.GetComponent<MeshFilter>().sharedMesh&& _levelObj.GetComponent<MeshFilter>().sharedMesh.vertexCount == 4)
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
            _levelObj = _generator.CreatePlaneLevel(_meshName + "_foundation", _levelMesh, _levelMat);
            SaveMesh(_levelMesh, _path, _meshName + "_mesh");
        }
        if (_prev_levelDepth != _levelDepth && _levelObj != null) 
        {
            if (_levelMesh) 
            {
                _generator.AdjustQuadDepth(_levelMesh, _levelDepth);

                if (_cells != null)
                    _cells = _generator.AdjustCellData(_cells,_horizontalChunks,_verticalChunks, _levelMesh, 1f);
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
                _currentAction = null;
                _commandStack = new LevelGenerator_CommandStack();
                _canEditCells = false;
            }

        }

        if (_cells != null) 
        {
            GUI.enabled = !_canEditCells;
            if (GUILayout.Button(_canEditCells? "Paint: RMB    Erase: Alt + RMB    Enlarge: Shift    Ctrl+Z: Undo    Ctrl+Y: Redo" : "Edit Cells"    ))
            {
                _canEditCells = true;
                foreach (Cell c in _cells)
                    c.isActive = false;
                _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
            }
            GUI.enabled = true;
        }

        if (_canEditCells && _levelMesh)
            if (GUILayout.Button( "Save Level Collider"))
            {
                //Only Convex collider is supported... need optimization
                //SaveMesh(_generator.GenerateLevelMesh(_cells), _path, _name + "_levelMesh");
                LevelGenerator_ChunkOptimizer optimizer = new LevelGenerator_ChunkOptimizer(_cells,_horizontalChunks,_verticalChunks);
                optimizer.SetupUtilityCell();
                _generator.GeneratePackedLevel(optimizer.PackCells(), _meshName + "_levelObject");
            }

        GUILayout.FlexibleSpace();
        if (_cells != null && _canEditCells) 
        {
            if (GUILayout.Button("Save to Level Container"))
            {
                SaveLevelData();
            }
            if (GUILayout.Button("Load from Level Container"))
            {
                LoadLevelData();
                _levelVisual.UpdateVisualSetup(_cells, sizeof(float) * 5);
                _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
            }
            _levelDataObject = (LevelDataObject)EditorGUILayout.ObjectField("Level Data Container", _levelDataObject, typeof(LevelDataObject), false);
        }
    

        _prev_levelDepth = _levelDepth;
    }

    private void SaveLevelData() 
    {
        if (!_levelDataObject) 
        {
            Debug.LogWarning("No Level Data Object Assigned");
            return;
        }
        _levelDataObject.SetLevelDataArray(_cells.Select(x=>x.isActive).ToArray());
        _levelDataObject.HorizontalChunk = _horizontalChunks;
        _levelDataObject.VerticalChunk = _verticalChunks;
        Debug.Log($"Level Data Saved to {_levelDataObject.name}");
    }
    private void LoadLevelData() 
    {
        if (!_levelDataObject)
        {
            Debug.LogWarning("No Level Data Object Assigned");
            return;
        }
        if (_levelDataObject.GetLevelDataArray() == null)
        {
            Debug.LogWarning("Level Data Array has value of Null");
            return;
        }
        _horizontalChunks = _levelDataObject.HorizontalChunk;
        _verticalChunks = _levelDataObject.VerticalChunk;
        _cells = _generator.CreateChunks(_levelMesh, _horizontalChunks,_verticalChunks, 1f);
        int index = 0;
        foreach (bool b in _levelDataObject.GetLevelDataArray()) 
        {
            _cells[index].isActive = b;
            index++;
        }
        Debug.Log($"Level Data Loaded from {_levelDataObject.name}");
    }
    private void OnSceneGUI(SceneView sceneView) 
    {
        if (!_canEditCells)
            return;
        RaycastHit hit = EditorUtil. EditorClickSceneObject(sceneView);
        UpdateCellActivationVisual(hit);
        HandingUndoRedo();
    }

    private void HandingUndoRedo() 
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
        {
            e.Use();
            if (_isMouseDown)
                return;
            _commandStack.UndoAction();
            _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
        }
        else if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Y) 
        {
            e.Use();
            if (_isMouseDown)
                return;
            _commandStack.RedoAction();
            _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);
        }
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

        if (selected == null)
            return;

        float r = Mathf.Sqrt(_levelMesh.bounds.size.x * _levelMesh.bounds.size.z / 10f / Mathf.PI);
        float handleRadius = e.shift ? r : Mathf.Max(selected.size.x / 2f, selected.size.z / 2f);
        Handles.color = e.alt ? Color.red : Color.green;
        Handles.DrawWireDisc(hit.point, hit.normal, handleRadius);

        Handles.color = Color.blue;
        Handles.DrawWireCube(selected.position + selected.size.y / 2 * Vector3.up, selected.size);

        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1)
        {

            if (!_isMouseDown)
            {
                _isMouseDown = true;
                _currentAction = new LevelGenerator_MouseDragAction();
                _commandStack.ClearRedoStack();
            }
            if (e.shift)
            {
                Cell[] cellInRadius = _generator.GetCellsInRadius(_cells, hit.point, r);
                foreach (Cell c in cellInRadius)
                    _currentAction.Execute(c, !e.alt);
            }
            else
                _currentAction.Execute(selected, !e.alt);


            _levelVisual.TogglePaintAndErase(e.alt ? 1 : 0);
            _levelVisual.UpdateVisualPerFrame(_cells, _generator.Cam.pixelWidth, _generator.Cam.pixelHeight);

            e.Use();

        }
        if ((e.type == EventType.MouseUp) && e.button == 1)
        {
            if (_currentAction.ChangeMade())
            _commandStack.RecordAction(_currentAction);
            _isMouseDown = false;
            _currentAction = null;
            e.Use();
        }
    }

    public void SaveMesh(Mesh mesh, string path, string name)
    {
        AssetDatabase.CreateAsset(mesh, Path.Combine("Assets", path, name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
