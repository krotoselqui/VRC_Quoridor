
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

using UnityEngine.UI;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class quo_Manager : UdonSharpBehaviour
{

    [Header("仕切り")]
    [SerializeField] GameObject[] gmRedWalls;
    [SerializeField] GameObject[] gmBlueWalls;
    [SerializeField] Transform tfWallRedInit;
    [SerializeField] Transform tfWallBlueInit;

    [Header("表示部分 Red")]
    [SerializeField] Text[] textRed;

    [Header("表示部分 Blue")]
    [SerializeField] Text[] textBlue;

    [Header("プレイヤー駒 & 初期位置 赤")]
    [SerializeField] GameObject gmObjPlayerRed;
    [SerializeField] Transform tfRedInit;

    [Header("プレイヤー駒 & 初期位置 青")]
    [SerializeField] GameObject gmObjPlayerBlue;
    [SerializeField] Transform tfBlueInit;



    [Header("ダブルクリック許容 & シングルクリック判定間隔 (sec)")]
    [SerializeField] private float dblThreshold = 0.25f;



    private bool dblClickFlag = false;
    private float dblRemainTime = 0.0f;

    private float distZeroThreshold;

    private bool lateCheckFlag = false;
    private float lChkTime = 0.0f;


    void Start()
    {
        distZeroThreshold = Vector3.Distance(tfRedInit.position, tfBlueInit.position) / 100;
        //Debug.Log(distZeroThreshold.ToString());

        WallCountRefresh();
        //late joinで動かないのでなるべく使用しない
    }

    private void Update()
    {
        if (dblClickFlag)
        {
            dblRemainTime -= Time.deltaTime;

            if (dblRemainTime <= 0.0f)
            {
                //ここにシングルインタラクト処理
                dblClickFlag = false;
            }
        }

        if (lateCheckFlag) 
        {
            lChkTime -= Time.deltaTime;

            if (lChkTime <= 0.0f)
            {
                WallCountRefresh();
                lateCheckFlag = false;
            }

        
        }
    }

    public void LateCheck()
    {
        lChkTime = 0.5f;
        lateCheckFlag = true;
    }
    public override void Interact()
    {
        if (dblClickFlag)
        {
            _Reset();
        }
        else
        {
            dblClickFlag = true;
            dblRemainTime = dblThreshold;
        }

    }

    private int CountWall(bool isRed)
    {
        int count = 0;

        if (isRed)
        {

            foreach (GameObject g in gmRedWalls)
            {
                Vector3 pos = g.transform.position;
                float dist = Vector3.Distance(pos, tfWallRedInit.position);
                //Debug.Log("dist=" + dist.ToString());

                if (dist < distZeroThreshold) count++;
            }

        }
        else
        {

            foreach (GameObject g in gmBlueWalls)
            {
                Vector3 pos = g.transform.position;
                float dist = Vector3.Distance(pos, tfWallBlueInit.position);

                if (dist < distZeroThreshold) count++;
            }

        }

        return count;
    }

    public void WallCountRefresh()
    {
        int redWallCount = CountWall(true);
        int blueWallCount = CountWall(false);

        for (int i = 0; i < textRed.Length; i++)
        {
            textRed[i].text = redWallCount == 0 ? "" : redWallCount.ToString();
        }

        for (int i = 0; i < textBlue.Length; i++)
        {
            textBlue[i].text = blueWallCount == 0 ? "" : blueWallCount.ToString();
        }
    }

    public void _Reset()
    {

        Networking.SetOwner(Networking.LocalPlayer, gmObjPlayerRed);
        Networking.SetOwner(Networking.LocalPlayer, gmObjPlayerBlue);


        for (int i = 0; i < gmBlueWalls.Length; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, gmBlueWalls[i]);
            quo_SnapGrid qoBlue = gmBlueWalls[i].GetComponent<quo_SnapGrid>();
            qoBlue.SendCustomEvent("Reset");
        }

        for (int i = 0; i < gmRedWalls.Length; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, gmRedWalls[i]);
            quo_SnapGrid qoRed = gmRedWalls[i].GetComponent<quo_SnapGrid>();
            qoRed.SendCustomEvent("Reset");
        }


        if (gmObjPlayerRed != null)
        {
            gmObjPlayerRed.transform.position = tfRedInit.position;
            gmObjPlayerRed.transform.rotation = Quaternion.identity;
        }

        if (gmObjPlayerBlue != null)
        {
            gmObjPlayerBlue.transform.position = tfBlueInit.position;
            gmObjPlayerBlue.transform.rotation = Quaternion.identity;
        }

        RequestSerialization();

        LateCheck();

    }
}
