
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

/// <summary>
/// スナップ処理
/// </summary>
public class quo_SnapGrid : UdonSharpBehaviour
{
    [Header("基準座標 ====")]
    [SerializeField] private Transform init;
    [SerializeField] private Transform topleft;
    [SerializeField] private Transform bottomright;

    [Header("角度スナップ")]
    [SerializeField] private bool rotSnap = true;

    [Header("行数(列数)")]
    [SerializeField] private int sectionDev = 8;

    [Header("スナップ無効距離の遠さ(0.5デフォルト、0で無効)")]
    [SerializeField] private float invalidScale = 0.5f;

    [Header("Manager")]
    [SerializeField] GameObject gmObjManager;

    Transform thisObjTrans;


    private Vector3 _unit = new Vector3(0, 0, 0);
    private float flBoardSize = 0f;

    void Start()
    {
        thisObjTrans = this.gameObject.transform;
        _unit = new Vector3((bottomright.position.x - topleft.position.x) / (sectionDev - 1),
                             (bottomright.position.y - topleft.position.y) / (sectionDev - 1),
                             (bottomright.position.z - topleft.position.z) / (sectionDev - 1));

    }
    private void OnEnable()
    {
        if (init != null)
        {
            this.gameObject.transform.position = init.position;
            this.gameObject.transform.rotation = init.rotation;
        }

    }

    public void Reset()
    {
        OnEnable();
    }

    private void Update()
    {

    }


    public override void OnDrop()
    {
        //Debug.Log("OnDropよばれた:" + this.gameObject.name);


        if (gmObjManager != null)
        {
            quo_Manager q = gmObjManager.GetComponent<quo_Manager>();
            q.LateCheck();
        }


        //位置判定
        thisObjTrans = this.gameObject.transform;
        float minDistance = 999.0f;
        Vector2 minPos = new Vector2(0, 0);

        for (int x = 0; x < sectionDev; x++)
        {
            for (int z = 0; z < sectionDev; z++)
            {
                Vector3 sectionTrans = new Vector3(topleft.position.x + (_unit.x * x),
                                                topleft.position.y,
                                                topleft.position.z + (_unit.z * z));

                float distance = Vector3.Distance(thisObjTrans.position, sectionTrans);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minPos = new Vector2(x, z);
                }

            }
        }

        //無効距離判定
        flBoardSize = Vector3.Distance(topleft.position, bottomright.position);
        if (minDistance > flBoardSize * invalidScale)
        {

            return;

        }



        //位置スナップ
        this.gameObject.transform.position = new Vector3(topleft.position.x + _unit.x * minPos.x,
                                                         topleft.position.y,
                                                         topleft.position.z + _unit.z * minPos.y);

        //角度判定・スナップ
        if (rotSnap)
        {
            float _dot = Mathf.Abs(Quaternion.Dot(thisObjTrans.rotation, topleft.rotation));

            if (_dot > 0.8f || _dot < 0.2f)
            {
                this.gameObject.transform.rotation = topleft.rotation;
            }
            else
            {
                this.gameObject.transform.rotation = bottomright.rotation;
            }
        }


        //初期位置判定
        if (init != null)
        {
            float distanceInit = Vector3.Distance(thisObjTrans.position, init.position);
            if (distanceInit < minDistance)
            {
                this.gameObject.transform.position = init.position;
                this.gameObject.transform.rotation = init.rotation;

            }
        }




    }
}

