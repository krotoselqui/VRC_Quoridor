
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class quo_Collidor : UdonSharpBehaviour
{

    [SerializeField] UdonBehaviour udManager;
    void Start()
    {

    }


    public void OnTriggerEnter(Collider other)
    {

        //Debug.Log("OnTriggerEnterよばれた:" + this.gameObject.name);
        udManager.SendCustomEvent("WallCountRefresh");

    }
    public void OnTriggerExit(Collider other)
    {

        //Debug.Log("OnTriggerExitよばれた:" + this.gameObject.name);
        udManager.SendCustomEvent("WallCountRefresh");

    }

}
