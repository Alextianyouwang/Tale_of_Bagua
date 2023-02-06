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

    RaycastHit[] allHitsMirrors;
    public static Action OnLevelChange;
    public static int allMirrorOnTop;

    public static Action<Mirror[]> OnShareHoodMirror;

    private void OnEnable()
    {
        allMirrorOnTop = 0;
        MirrorManager.OnCheckingSlidable += CheckIfPlayerOverlapWithNextLevel;   
    }
    private void OnDisable()
    {
        MirrorManager.OnCheckingSlidable -= CheckIfPlayerOverlapWithNextLevel;
    }
    void DisableOtherLevels(Level current) 
    {
        foreach (Level l in levels) 
        {
            if (l != current)
                l.ToggleRigidColliders(false);
        }
    }
    void Update()
    {
        CheckLayers();
    }
    private bool CheckIfPlayerOverlapWithNextLevel() 
    {
        Collider[] overlapping = Physics.OverlapSphere(transform.position, 0.1f,obstacleMask);
        Level currentLevel = levels[allHitsMirrors.Length - 1],
              lastLevel = levels[allHitsMirrors.Length - 2 <= 0 ? 0 : allHitsMirrors.Length - 2],
              nextLevel = levels[allHitsMirrors.Length >=levels.Count-1?levels.Count-1:allHitsMirrors.Length];
        bool rigidCollider = false;
        foreach (Collider c in overlapping) 
        {
            if (c.gameObject.GetComponentInParent<Level>()) 
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if ( localLevel == nextLevel )
                    rigidCollider = true;
            }
        }        
        return  rigidCollider;
    }

    private bool CheckIfPlayerOverlapWithCurrentLevel()
    {
        Collider[] overlapping = Physics.OverlapSphere(transform.position, 0.1f, obstacleMask);
        Level currentLevel = levels[allHitsMirrors.Length - 1],
              lastLevel = levels[allHitsMirrors.Length - 2 <= 0 ? 0 : allHitsMirrors.Length - 2],
              lastLastLevel = levels[allHitsMirrors.Length - 3 <= 0 ? 0 : allHitsMirrors.Length - 3],
              lastLastLastLevel = levels[allHitsMirrors.Length - 4 <= 0 ? 0 : allHitsMirrors.Length - 4],
              lastLastLastlastLevel = levels[allHitsMirrors.Length - 5 <= 0 ? 0 : allHitsMirrors.Length - 5],
              nextLevel = levels[allHitsMirrors.Length >= levels.Count - 1 ? levels.Count - 1 : allHitsMirrors.Length];
        bool rigidCollider = false;
        foreach (Collider c in overlapping)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (localLevel == lastLevel &&  currentLevel != nextLevel
                    )
                    rigidCollider = true;
            }
        }

        return rigidCollider;
    }
    private void CheckLayers()
    {
        allHitsMirrors = Physics.RaycastAll(transform.position, Vector3.up, 100f, mirrorMask);
        RaycastHit[] mirrorHits = allHitsMirrors.Where(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        hoodMirrors = new Mirror[mirrorHits.Length];

        allMirrorOnTop = hoodMirrors.Length;
        if (allHitsMirrors.Length <= levels.Count) 
        {
            currentLevel = levels[allHitsMirrors.Length - 1];
            currentLevel.ToggleRigidColliders(true);
            DisableOtherLevels(currentLevel);
            if (prevLevel != currentLevel && prevLevel != null) 
            {
                OnLevelChange?.Invoke();
            }
            prevLevel = currentLevel;
        }
       
        for (int i = 0; i < hoodMirrors.Length; i++)
        {
           
            hoodMirrors[i] = mirrorHits[i].transform.gameObject.GetComponent<Mirror>();
        }
        OnShareHoodMirror?.Invoke(hoodMirrors);


        foreach (Mirror m in hoodMirrors)
        {
            if (CheckIfPlayerOverlapWithCurrentLevel())
            {
                m.ToggleBoxesRigidCollider(true);

            }
            else
            {
                m.ToggleBoxesRigidCollider(false);

            }
        }

    }

    
}
