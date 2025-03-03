using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataCommunicator : MonoBehaviour
{
    public int GetLayerNumber() 
    {
        Level[] levels = FindObjectsOfType<Level>(false);
        if (levels.Length != 0)
            return levels.Length;
        else return 0;
    }
    public Vector3[] GetMirrorPositions() 
    {
        Mirror[] mirrors = FindObjectsOfType<Mirror>(false);
        Vector3[] positions = new Vector3[mirrors.Length];
        for (int  i = 0; i < mirrors.Length; i++) 
        {
            positions[i] = mirrors[i].transform.position;
        }
        return positions;
    }
    public void SetMirrorPositions(Vector3[] positions) 
    {
        Mirror[] mirrors = FindObjectsOfType<Mirror>(false);
        if (positions == null || positions.Length == 0)
            return;
        for (int i = 0; i < mirrors.Length; i++)
        {
            if (positions[i] == null)
                continue;
            mirrors[i].transform.position = positions[i];
        }
    }
    public Vector3 GetPlayerPosition() 
    {
        PlayerMove player = FindObjectOfType<PlayerMove>(false);
        if (player != null)
            return player.transform.position;
        else return Vector3.zero;
    }

    public void SetPlayerPosition(Vector3 position) 
    {
        PlayerMove player = FindObjectOfType<PlayerMove>(false);
        if (player == null)
            return;
        player.transform.position = position; 
    }

    public void TrySetTestObjectState(Dictionary<string, int> data) 
    {
        AchievementTest[] objs = FindObjectsOfType<AchievementTest>();
        if (objs.Length == 0) return;
        foreach (AchievementTest obj in objs) 
        {
            obj.ChangeState(data);
        }
    }
    public string GetSceneName() 
    {
        return SceneManager.GetActiveScene().name;
    }

}
