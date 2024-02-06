
using System;
using UnityEngine;
[DefaultExecutionOrder(-10000)]
public class Main : MonoBehaviour
{
    public static Main instance;
    public Coroutine LoadingGame_co;
    public Action OnStartTicked;
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
     
        OnStartTicked?.Invoke();
    }

 
  
    void Update()
    {
        
    }
}
