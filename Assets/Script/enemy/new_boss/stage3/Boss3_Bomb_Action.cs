using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          爆弾本体の処理
// ※出現した瞬間にカウントを開始して、
// 　爆発した瞬間、自身を中心に
// 　両方向へ振動を発生させます　
//==================================================
// 制作日2023/05/26    更新日2023/05/28
// 宮﨑 高田
public class Boss3_Bomb_Action : MonoBehaviour
{
    // 自身の指令を取得
    private Setup_BombPointer opMain;
    public Boss3_Main bossMain;
    public Animator bombAnim;
    private BossAudio bombAudio;

    // 爆発までの時間
    [SerializeField] private byte bombCount;

    // 波の設定
    [Header("----- 波の設定 -----")]
    private WavePool pool;
    public Transform ground;   // 爆弾が作用する糸のvfxManager
    private sbyte arrayNum = 0;     // 波を生成するとき戻り値をクローンへ
    [SerializeField] private float waveSpeed = 7.5f;    // 波の速度
    [SerializeField] private float waveWidth = 0.225f;  // 波の振動数
    [SerializeField] private float waveHight = 2.0f;    // 波の高さ
    [SerializeField] private GameObject waveObj;        // 波の判定プレハブ
    public int waveCount = 3;   // プールに生成する波の数
    public int waveAngle = 1;  // 波の方向

    //// 波のコリジョンのオブジェクトプール
    //[System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    //// 波コリジョンにコンポーネントされている波判定のプール
    //// 添え字は波判定のプールに対応
    //private List<waveCollition> l_waveCollitions = new List<waveCollition>();


    // 現在の状態

    private void Start()
    {
        // 自身の指令を取得
        GameObject opObj = GameObject.Find("BombPointOperator").gameObject;
        opMain = opObj.GetComponent<Setup_BombPointer>();
        bombAnim = GetComponent<Animator>();
        if (bombAnim == null) Debug.LogError("爆弾にアニメーターがない");
        bombAudio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();
        //// 波の各情報を取得
        //for (int i = 0; i < waveCount; i++)
        //{
        //    l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
        //    l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
        //    l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        //}//----- for_stop -----

        //StartCoroutine(BombCountDown());
    }

    public IEnumerator BombCountDown()
    {
        
        
        for(int i = 0; i < bombCount; i++)
        {
            if(i==bombCount-60)
            {
                Debug.Log("Bomb!!");
                bombAudio.Boss3_ExplosionSound();
            }
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----
        
        // 爆発を開始する
        BlastWaveAttack();
    }

    private void BlastWaveAttack()
    {
        //// 爆発する為見た目を非表示にする
        //MeshRenderer myMesh = this.gameObject.GetComponent<MeshRenderer>();
        //myMesh.enabled = true;

        bombAnim.SetBool("Bombing", true);
        

        ground = transform.parent.transform.parent.GetChild(0);

        // 自身を中心に両方向へ振動を発生させる ---------- お願いします。
        StartCoroutine(WaveWideSpawn());


        
    }

    private IEnumerator WaveWideSpawn()
    {
        

        waveAngle = 1;
        pool.WaveCreate(waveSpeed*waveAngle,waveWidth,waveHight,waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x + 0.25f, ground);

        for (byte j = 0; j < 3; j++)
        {
            yield return null;
        }

        
        waveAngle = -1;
        pool.WaveCreate(waveSpeed * waveAngle, waveWidth, waveHight, waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x - 0.25f, ground);

        for (byte i = 0;i<10; i++)
        {
            yield return null;
        }
        // 振動発生後,自身を破壊する
        opMain.blasted = true;
        transform.position = new Vector3(20, 0, 50);
        bombAnim.SetBool("Bombing", false);
        
        transform.parent = null;
        ground = null;
    }
    //public void WaveCreate(float startPosX, float startPosY)
    //{
    //    // 配列番号を指定する
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);
    //    Debug.Log("ボム波発生");
    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            l_waveCollisionObj[i].transform.SetParent(transform);
    //            // 当たり判定に番号を指定
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // クローンに対応させる vfxManager を設定
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // 波をプレイヤーが発生させた波に設定
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // コリジョンを動かす
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // コリジョンの高さを波の高さに設定
    //            l_waveCollitions[i].transform.localScale = new Vector3(0, 0, 1);
    //            // コリジョンの発生位置を波の発生位置に設定
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0);

    //            // コリジョンの最大高度を設定
    //            l_waveCollitions[i].maxWaveHight = waveHight;

    //            break;
    //        }//----- if_stop -----
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}

    //private void OnApplicationQuit()
    //{
    //    l_waveCollisionObj.Clear();
    //    l_waveCollitions.Clear();
    //}


}
