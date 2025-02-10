#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class GridSnappingWindow : EditorWindow
{
    private float gridSize = 0.5f; // Grid size for snapping
    private bool isSnappingActive = true; // Toggle snapping on/off

    [MenuItem("Tool/Grid Snapping")]
    public static void ShowWindow()
    {
        // Open the Grid Snapping Window
        var window = GetWindow<GridSnappingWindow>("Grid Snapping");
        window.Show();
    }

    private void OnEnable()
    {
        // Subscribe to SceneView updates
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Snapping", EditorStyles.boldLabel);

        // Input field for grid size
        gridSize = EditorGUILayout.FloatField("Grid Size:", gridSize);
        gridSize = Mathf.Max(gridSize, 0.01f); // Ensure grid size is greater than 0

        // Enable/Disable real-time snapping
        isSnappingActive = EditorGUILayout.Toggle("Enable Snapping", isSnappingActive);

        GUILayout.Space(10);

        // Batch Align Button
        if (GUILayout.Button("Align Selected Objects"))
        {
            AlignSelectedObjectsToGrid();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "While this window is open, objects will snap to the grid when their position is modified in the Scene view. You can also manually align selected objects using the button above.",
            MessageType.Info
        );
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isSnappingActive || Selection.transforms.Length == 0)
            return;

        // Check if the user is actively dragging objects
        if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseUp)
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Grid");
                SnapTransformToGrid(transform);
            }

            // Repaint the Scene view to reflect snapping
            SceneView.RepaintAll();
        }
    }

    private void SnapTransformToGrid(Transform transform)
    {
        var meshRenderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            return;

        // Calculate the bottom-left corner in XZ
        var bounds = meshRenderer.bounds;
        var bottomLeft = new Vector3(bounds.min.x, transform.position.y, bounds.min.z);

        // Snap bottom-left corner to the nearest grid
        var snappedBottomLeft = new Vector3(
            Mathf.Round(bottomLeft.x / gridSize) * gridSize,
            bottomLeft.y,
            Mathf.Round(bottomLeft.z / gridSize) * gridSize
        );

        // Offset object position to align bottom-left corner with snapped grid point
        var offset = snappedBottomLeft - bottomLeft;
        transform.position += offset;
    }

    private void AlignSelectedObjectsToGrid()
    {
        foreach (var transform in Selection.transforms)
        {
            Undo.RecordObject(transform, "Align Selected Objects to Grid");
            SnapTransformToGrid(transform);
        }

        // Force Scene view to update after alignment
        SceneView.RepaintAll();
    }
}
#endif
