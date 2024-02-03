
using UnityEngine;
public class Main : MonoBehaviour
{
    public static Main instance;
    private void Awake()
    {
        CreateSingleton();
    }
    private void OnEnable()
    {
 
    }
    private void OnDisable()
    {

    }
    void CreateSingleton()
    {
        if (instance == null)
            instance = FindObjectOfType<Main>();
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
