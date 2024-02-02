using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataCommunicator : MonoBehaviour
{
    public int GetLayerNumber() 
    {
        Level[] levels = FindObjectsOfType<Level>();
        if (levels.Length != 0)
            return levels.Length;
        else return 0;
    }
    public Vector3[] GetMirrorPositions() 
    {
        Mirror[] mirrors = FindObjectsOfType<Mirror>();
        Vector3[] positions = new Vector3[mirrors.Length];
        for (int  i = 0; i < mirrors.Length; i++) 
        {
            positions[i] = mirrors[i].transform.position;
        }
        return positions;
    }
    public void SetMirrorPositions(Vector3[] positions) 
    {
        Mirror[] mirrors = FindObjectsOfType<Mirror>();
        for (int i = 0; i < mirrors.Length; i++)
        {
            mirrors[i].transform.position = positions[i];
        }
    }
    public Vector3 GetPlayerPosition() 
    {
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
            return player.transform.position;
        else return Vector3.zero;
    }

    public void SetPlayerPosition(Vector3 position) 
    {
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player == null)
            return;
        player.transform.position = position; 
    }

    public string GetSceneName() 
    {
        return SceneManager.GetActiveScene().name;
    }

}
