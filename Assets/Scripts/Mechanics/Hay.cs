using System.Collections;
using UnityEngine;

public class Hay : RationalObject,IFlammable {

    private MeshRenderer _mr;
    private MaterialPropertyBlock _mpb;
    private Color _initialColor;
    private bool _isIngnited = false;
    [SerializeField]private float _fullyIgniteTime = 1f;
    [SerializeField] private float _burnOutTime = 5f;


    private void OnEnable()
    {
        _mr = GetComponent<MeshRenderer>();
        _initialColor = _mr.material.color;
        _mpb = new MaterialPropertyBlock();
        OnReceive += RecieveLazer;
    }
    private void RecieveLazer(RationalObject ro)
    {
        LazerEmitter l = ro.GetComponent<LazerEmitter>();
        if (l == null)
            return;

        print("Received Lazer");

        StartCoroutine(IgnitionRoutine(_fullyIgniteTime,_burnOutTime));
    }

    private IEnumerator IgnitionRoutine(float fullyIgniteTime, float burnOutTime)
    {
        float fullyIgnite = 0;

        while (fullyIgnite < fullyIgniteTime) { 
            fullyIgnite += Time.deltaTime;
            float fullyIgnitePercent = fullyIgnite / fullyIgniteTime;
            _mpb.SetColor("_BaseColor", Color.Lerp(_initialColor, Color.red, fullyIgnitePercent));
            _mr.SetPropertyBlock(_mpb);
    yield return null;
        }
        float burnOut = 0;
        float expendCheck = 0.25f;
        while (burnOut< burnOutTime)
        {
            burnOut += Time.deltaTime;
            float burnOutPercent = burnOut / burnOutTime;
            if (burnOut > expendCheck) 
            {
                ExpandFlame();
                expendCheck += 0.25f;
            }
            _mpb.SetColor("_BaseColor", Color.Lerp(Color.red, Color.black, burnOutPercent));
            _mr.SetPropertyBlock(_mpb);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void ExpandFlame()
    {
        if (_mr == null)
            return;
        Bounds b = _mr.bounds;
        Vector2 offset = new Vector2(_mr.bounds.extents.x, _mr.bounds.extents.z) * 2;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 pos = new Vector3(offset.x * x, transform.position.y, offset.y * y) + transform.position;
                Collider[] ignite = Physics.OverlapSphere(pos, 0.1f);
                foreach (Collider c in ignite) 
                {
                    IFlammable target = c.GetComponent<IFlammable>();
                    if (target != null)
                        target.Ignite(this);
                }

            }
        }
    }
    public void Ignite(RationalObject ro) 
    {
        if (ro._levelIndex != _levelIndex && (!ro.IsObjectAtCorrectLevel_strict() || !IsObjectAtCorrectLevel_mild(1f)))
        return;

            if (_isIngnited)
                return;
        _isIngnited = true;
        StartCoroutine(IgnitionRoutine(_fullyIgniteTime, _burnOutTime));

    }

}
