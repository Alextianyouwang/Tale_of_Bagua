using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(menuName = "TTOBG/StateObject")]
public class StateDataObject : ScriptableObject
{
    public string Name;
    public SceneDataObject SceneToLoad;
    public bool IsGameSession = false;
    public void LoadState() 
    {
        if (SceneToLoad == null)
            return;
        SceneManager.LoadSceneAsync(SceneToLoad.Name,LoadSceneMode.Single);
    }
}
