using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Ball : MonoBehaviour
{
    // 親の情報を取得
    private GameObject parObj;

    // 挙動情報の取得
    private Boss2_Main parMain;

    public GameObject ball;
    public ushort SpawnDelayFrame = 90;

    public Vector3 ballSpawnPoint;

    public bool nowDelay;
    
    private void Start()
    {
        // 親の情報を取得
        parObj = transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss2_Main>();
    }

    public IEnumerator SpawnBallDelay()
    {
        nowDelay = true;

        for(int i = 0; i < SpawnDelayFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;
            if(i==SpawnDelayFrame/2)
            {
                parMain.anim.SetBool("ballThrowing",true);
                parMain.anim.SetBool("ballSetting", false);
                parMain.audio.bossSource.Stop();
                parMain.audio.bossSource.loop = false;
                parMain.audio.BossThrowBallSound();
            }
        }

        nowDelay = false;

        while (parMain.nowDmgReAct) yield return null;

        SpawnBall();
    }

    public void SpawnBall()
    {
        GameObject thisBall = Instantiate(ball);
        thisBall.transform.position = ballSpawnPoint;
        thisBall.GetComponent<Boss2_Ball_Fall>().mainBoss = parMain;
        parMain.anim.SetBool("ballThrowing", false);

        parMain.AttackDelayTime();
        StartCoroutine(parMain.AttackDelayTime());
    }
}
