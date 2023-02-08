using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorController : MonoBehaviour
{
    public GameObject NextItem;
    public GameObject NewMirror;
    public GameObject Panel;
    public int CanBeEnableWhenMirroEqual;

    public void OnCollisionEnter(Collision other)
    {
        if (CanBeEnableWhenMirroEqual == LayerCheck.allMirrorOnTop)
        if (other.gameObject.tag =="Player")
        {
                if (LayerCheck.allMirrorOnTop == 4)
                {
                    SceneManager.LoadScene("Closing Cut");
                }
                else 
                {
                    NextItem.SetActive(true);
                    NewMirror.SetActive(true);
                    Panel.SetActive(true);

                    gameObject.SetActive(false);
                }
        }
        
    }
}
