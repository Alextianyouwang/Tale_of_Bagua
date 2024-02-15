using UnityEngine;
public class MaterialColorChanger : MonoBehaviour
{
    private MaterialPropertyBlock _mpb;

    public void ChangeToYellow() {
        _mpb = new MaterialPropertyBlock();
        _mpb.SetColor("_BaseColor", Color.yellow);
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.yellow);
    }
    public void ChangeToGreen() {
        _mpb = new MaterialPropertyBlock();
        _mpb.SetColor("_BaseColor", Color.green);

        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);

    }
}
