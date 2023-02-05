using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System;

public class LayerCheck : MonoBehaviour
{
    public LayerMask mirrorMask,obstacleMask;
    public List<Level> levels = new List<Level>();
    private Level currentLevel;

    private void OnEnable()
    {
        MirrorManager.OnCheckingSlidable += CheckIfStuckInLayer;   
    }
    private void OnDisable()
    {
        MirrorManager.OnCheckingSlidable -= CheckIfStuckInLayer;
    }
    void DisableOtherLevels(Level current) 
    {
        foreach (Level l in levels) 
        {
            if (l != current)
                l.ToggleRigidColliders(false);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        CheckLayers();
       print( CheckIfStuckInLayer());
    }

    private void FixedUpdate()
    {
        
    }

    private bool CheckIfStuckInLayer() 
    {
        Collider[] overlapping = Physics.OverlapSphere(transform.position, 0.1f);
        return overlapping.Length != 0?true : false;
    }
    private void CheckLayers()
    {
        RaycastHit[] allHits;

        allHits = Physics.RaycastAll(transform.position, Vector3.up, 100f, mirrorMask);
        if (allHits.Length <= levels.Count) 
        {
            currentLevel = levels[allHits.Length - 1];
            currentLevel.ToggleRigidColliders(true);
            DisableOtherLevels(currentLevel);
        }

        
    }

    
}
