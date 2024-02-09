
using System;
using UnityEngine;
[DefaultExecutionOrder(-10000)]
public class Main : PersistentSingleton<Main>
{
    public Coroutine LoadingGame_co;
    public Action OnStartTicked;
    
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
 
    }
    private void OnDisable()
    {

    }
    private void Start() 
    {
        OnStartTicked?.Invoke();
    }
    void Update()
    {
        
    }
}
