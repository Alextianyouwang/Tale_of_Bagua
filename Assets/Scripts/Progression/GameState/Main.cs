
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Main : MonoBehaviour
{
    public static Main instance;

    private void Awake()
    {
        CreateSingleton();

    }
    void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Start() 
    {

    }

 
  
    void Update()
    {
        
    }
}
