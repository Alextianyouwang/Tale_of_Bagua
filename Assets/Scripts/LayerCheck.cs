using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LayerCheck : MonoBehaviour
{
    public LayerMask mirrorMask,obstacleMask;
    public List<Level> levels = new List<Level>();
    private Mirror currentMirror;
    private Level currentLevel,lastLevel,nextLevel;
    private Mirror[] hoodMirrors;
    public Mirror[] allMirrors;
    private Collider[] overlapping;

    RaycastHit[] allHitsMirrors;
<<<<<<< HEAD
    public static int allMirrorsOnTop = 0;
    public static Action OnLevelChange;
=======
    public static int allMirrorOnTop;
>>>>>>> BeforeJamBuild

    public static Action<Mirror[]> OnShareHoodMirror;
    public static bool isPlayerOnLastLevel;

    private void OnEnable()
    {
<<<<<<< HEAD
        allMirrorsOnTop = 0;
        MirrorManager.OnCheckingSlidable += CheckIfPlayerOverlapWithNextLevel;   
=======
        allMirrorOnTop = 0;
        MirrorManager.OnCheckingSlidable += CheckSelectedMirrorSlidable;
        MirrorManager.OnSharingCurrentMirror += ReceiveCurrentMirror;

>>>>>>> BeforeJamBuild
    }
    private void OnDisable()
    {
        MirrorManager.OnCheckingSlidable -= CheckSelectedMirrorSlidable;
        MirrorManager.OnSharingCurrentMirror -= ReceiveCurrentMirror;
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
        isPlayerOnLastLevel = CheckIfPlayerOnLastLevel();
    }
    void ReceiveCurrentMirror(Mirror mirror) 
    {
<<<<<<< HEAD
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
                if ( localLevel == nextLevel)
                    rigidCollider = true;
            }
        }        
        return  rigidCollider;
=======
        currentMirror = mirror;
>>>>>>> BeforeJamBuild
    }
    private bool CheckIfPlayerOnLastLevel() 
    {
        Level currentLevel = levels[allHitsMirrors.Length - 1],
              nextLevel = levels[allHitsMirrors.Length >= levels.Count - 1 ? levels.Count - 1 : allHitsMirrors.Length];
        return currentLevel == nextLevel;
    }
    private bool CheckFreeMirrorEnterable()
    {
        foreach (Collider c in overlapping)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
<<<<<<< HEAD
                if ( localLevel == lastLevel
                    /*     ||  localLevel == lastLastLevel||
                           localLevel == lastLastLastLevel ||
                           localLevel == lastLastLastlastLevel*/
                    )
                    rigidCollider = true;
=======
                if (localLevel == nextLevel)
                    return true;
>>>>>>> BeforeJamBuild
            }
        }
        return false;
    }
    private bool CheckSelectedMirrorSlidable() 
    {
        foreach (Collider c in overlapping) 
        {
            if (c.gameObject.GetComponentInParent<Level>()) 
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (currentLevel == nextLevel ? localLevel == lastLevel : localLevel == nextLevel)
                    return true;
            }
        }
        return false;
    }

    private bool CheckHoodeMirrorSliable()
    {
        foreach (Collider c in overlapping)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (localLevel == lastLevel && currentLevel != nextLevel)
                    return true;
            }
        }
        return false;
    }
    private void CheckLayers()
    {
        overlapping = Physics.OverlapSphere(transform.position, 0.1f, obstacleMask);
        allHitsMirrors = Physics.RaycastAll(transform.position, Vector3.up, 100f, mirrorMask);
        RaycastHit[] mirrorHits = allHitsMirrors.Where(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        hoodMirrors = new Mirror[mirrorHits.Length];

<<<<<<< HEAD
        allMirrorsOnTop = allHitsMirrors.Length;
        print(allMirrorsOnTop);
        if (allHitsMirrors.Length <= levels.Count) 
        {
            currentLevel = levels[allHitsMirrors.Length - 1];
            //print(currentLevel);
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
           
=======
        allMirrorOnTop = hoodMirrors.Length;
        currentLevel = levels[allHitsMirrors.Length - 1];
        lastLevel = levels[allHitsMirrors.Length - 2 <= 0 ? 0 : allHitsMirrors.Length - 2];
        nextLevel = levels[allHitsMirrors.Length >= levels.Count - 1 ? levels.Count - 1 : allHitsMirrors.Length];
        currentLevel.ToggleRigidColliders(true);
        DisableOtherLevels(currentLevel);

        for (int i = 0; i < hoodMirrors.Length; i++) 
>>>>>>> BeforeJamBuild
            hoodMirrors[i] = mirrorHits[i].transform.gameObject.GetComponent<Mirror>();
        Mirror[] freeMirror = allMirrors.Where(x => !hoodMirrors.Contains(x) && x != currentMirror && x.isActiveAndEnabled).ToArray();
        foreach (Mirror m in freeMirror) 
            m.ToggleBoxesRigidCollider(CheckFreeMirrorEnterable());
        Mirror[] notSelectedHoodMirror = hoodMirrors.Where(x => x != currentMirror).ToArray();
        OnShareHoodMirror?.Invoke(hoodMirrors);
        foreach (Mirror m in isPlayerOnLastLevel ? notSelectedHoodMirror : hoodMirrors)
            m.ToggleBoxesRigidCollider(CheckHoodeMirrorSliable());
    }
}
