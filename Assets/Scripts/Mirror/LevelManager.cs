
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public bool enableMirrorAtStart = false;
    public LayerMask mirrorMask,obstacleMask;
    public List<Level> levels = new List<Level>();
    private Level currentLevel,lastLevel,nextLevel;
    private Mirror[] hoodMirrors;
    public Mirror[] allMirrors;
    private Collider[] overlapping;
    
    RaycastHit[] allHitsMirrors;
    
    public static int allMirrorOnTop;

    public static Action<Mirror[]> OnShareHoodMirror;
    public static Action<Mirror[]> OnShareAllMirror;
    public static Action<Level[]> OnShareAllLevels;
    public static Action OnFixUpdate;

    private void OnEnable()
    {
        allMirrorOnTop = 0;
        
    }
    private void Start()
    {
        allMirrors = FindObjectsOfType<Mirror>();
        foreach (var mirror in allMirrors) 
        {
            mirror.gameObject.SetActive(enableMirrorAtStart);
         }
        OnShareAllMirror?.Invoke(allMirrors);
        OnShareAllLevels?.Invoke(levels.ToArray());
    }

    void DisableOtherLevels(Level current) 
    {
        current.ToggleRigidColliders(true);
        foreach (Level l in levels) 
        {
            if (l != current)
                l.ToggleRigidColliders(false);
        }
    }


    private void FixedUpdate()
    {
        CheckLayers();
        OnFixUpdate?.Invoke();
    }
   
   
    private void CheckLayers()
    {
        UpdateLevelInfo();
        DisableOtherLevels(currentLevel);
        LevelColliderControl();
    }

    void UpdateLevelInfo() 
    {
        overlapping = Physics.OverlapSphere(transform.position, 0.4f * transform.localScale.x, obstacleMask);
        allHitsMirrors = Physics.RaycastAll(transform.position - Vector3.up * 3f, Vector3.up, 20f, mirrorMask);
        RaycastHit[] mirrorHits = allHitsMirrors.Where(x => x.transform.gameObject.GetComponent<Mirror>()).ToArray();
        hoodMirrors = new Mirror[mirrorHits.Length];
        for (int i = 0; i < hoodMirrors.Length; i++)
            hoodMirrors[i] = mirrorHits[i].transform.gameObject.GetComponent<Mirror>();
        OnShareHoodMirror?.Invoke(hoodMirrors);

        allMirrorOnTop = hoodMirrors.Length;
        currentLevel = levels[allHitsMirrors.Length - 1];
        lastLevel = levels[allHitsMirrors.Length - 2 <= 0 ? 0 : allHitsMirrors.Length - 2];
        nextLevel = levels[allHitsMirrors.Length >= levels.Count - 1 ? levels.Count - 1 : allHitsMirrors.Length];

    }

    void LevelColliderControl() 
    {
        foreach (Mirror m in hoodMirrors)
            m.ToggleBoxesRigidCollider(CheckHoodeMirrorSliable());

        foreach (Mirror m in allMirrors.Where(x => !hoodMirrors.Contains(x)).ToArray())
            m.ToggleBoxesRigidCollider(CheckFreeMirrorEnterable());
    }

    private bool CheckFreeMirrorEnterable()
    {
        foreach (Collider c in overlapping)
        {
            if (c.gameObject.GetComponentInParent<Level>())
            {
                Level localLevel = c.gameObject.GetComponentInParent<Level>();
                if (localLevel == nextLevel)
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
                if (currentLevel == nextLevel ? localLevel == lastLevel || localLevel == nextLevel : localLevel == lastLevel)
                    return true;
            }
        }
        return false;
    }


   /* private bool IsPlayerUnderMirror(int rayCount, float radius, float passThreshold)
    {
        circularRayCasts = new RaycastHit[rayCount];
        int notPassed = 0;
        float incement = (Mathf.PI * 2) / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float x = Mathf.Cos(i * incement) * radius;
            float y = Mathf.Sin(i * incement) * radius;
            Vector3 castingPos = transform.position - Vector3.up * 3f + new Vector3(x, 0, y);
            RaycastHit[] hits = Physics.RaycastAll(castingPos, Vector3.up, 20f, mirrorMask);
            foreach (RaycastHit h in hits)
                if (h.collider.tag.Equals("Mirror"))
                    circularRayCasts[i] = h;
        }
        foreach (RaycastHit hit in circularRayCasts)
            if (hit.collider == null)
                notPassed++;

        return (notPassed / (float)rayCount) < 1 - passThreshold;
    }
    private void OnDrawGizmos()
    {
        if (circularRayCasts != null)
            foreach (RaycastHit r in circularRayCasts)
            {
                Gizmos.DrawSphere(r.point, 0.01f);
            }
    }*/
}
