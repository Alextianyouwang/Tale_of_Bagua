using UnityEngine;
using UnityEngine.UI;

public class SinWaveUI_Col : MonoBehaviour
{
    public Color one;
    public Color two;
    public Image target;
    void Update()
    {
        target.color = Color.Lerp(one, two, Mathf.Sin(Time.deltaTime * 2) * 0.5f + 0.5f) ;
    }
}
