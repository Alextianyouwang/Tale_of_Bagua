using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LayerCheck : MonoBehaviour
{
    public LayerMask mirrorMask,obstacleMask;
    public List<Level> levels = new List<Level>();
    private Level currentLevel,prevLevel;

    private Mirror[] hoodMirrors;
  

    public static Action OnLevelChange;

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
    }

    private void FixedUpdate()
    {
        
    }

    private bool CheckIfStuckInLayer() 
    {
        Collider[] overlapping = Physics.OverlapSphere(transform.position, 0.1f,obstacleMask);
        return overlapping.Length != 0?true : false;
    }
    private void CheckLayers()
    {
        RaycastHit[] allHits;

        allHits = Physics.RaycastAll(transform.position, Vector3.up, 100f, mirrorMask);
        RaycastHit[] mirrorHits = allHits.Where(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        hoodMirrors = new Mirror[mirrorHits.Length];
        for(int  i = 0; i < hoodMirrors.Length; i++)
        {
            hoodMirrors[i] = mirrorHits[i].transform.gameObject.GetComponent<Mirror>();
        }

        
        foreach (Mirror m in hoodMirrors) 
        {
            if (CheckIfStuckInLayer())
            {
                m.ToggleBoxesRigidCollider(true);

            }
            else 
            {
                m.ToggleBoxesRigidCollider(false);

            }
        }

        if (allHits.Length <= levels.Count) 
        {
            currentLevel = levels[allHits.Length - 1];
            currentLevel.ToggleRigidColliders(true);
            DisableOtherLevels(currentLevel);
            if (prevLevel != currentLevel && prevLevel != null) 
            {
                OnLevelChange?.Invoke();
                //print(currentLevel.name);
            }
            prevLevel = currentLevel;
        }

        
    }

    
}
