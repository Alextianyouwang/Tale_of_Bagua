
using UnityEngine;


public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    [SerializeField]
    private SceneDataObject[] sceneDatas;
    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton() 
    {
        if (instance != null)
            instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(this); 
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
