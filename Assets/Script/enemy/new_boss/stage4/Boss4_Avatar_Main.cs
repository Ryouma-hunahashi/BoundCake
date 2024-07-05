using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスの分身
// ※[Setup_AvatarPoint]スクリプトが付与された
// 　オブジェクトが存在しているシーン内で使用してください
//==================================================
// 制作日2023/05/26    更新日2023/05/30
// 宮﨑
public class Boss4_Avatar_Main : MonoBehaviour
{
    // 自身の情報
    public int myID;
    public bool active = true;
    public Rigidbody avatartRb;
    public Animator avatarAnim;
    public SpriteRenderer avatarMesh;
    private Animator anim;
    private EnemyEffectManager ef_Manager;
    private BossAudio audio;

    // 出現時のアニメーション設定
    public bool startAnim;

    // 上昇の設定
    public sbyte ascendSpeed = 30;

    // 波の当たり判定
    public bool damage;
    public bool invisible;
    private GameObject waveHit;
    private GameObject waveLog;

    private void OnTriggerEnter(Collider other)
    {
        // 攻撃を受けたなら以降処理を抜ける
        if (damage || invisible) return;

        // 振動に触れた時
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                damage = true;
                waveHit = other.gameObject;

                // ダメージアニメーションを開始 ----------

                // 消滅を開始する
                StartCoroutine(RemoveAvatar());

                // 更新した波がログと一致しないなら
                if (waveHit != waveLog)
                {
                    // 波のログを更新する
                    waveLog = waveHit;

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // 振動に触れていた時
        if (other.CompareTag("Wave"))
        {
            // 触れた波が格納されているなら
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        // ０は本体になるため表示しない
        if (myID == 0) active = false;

        // 自身の情報を取得
        GameObject thisObj = this.transform.gameObject;
        avatartRb = thisObj.GetComponent<Rigidbody>();
        avatarAnim = thisObj.GetComponent<Animator>();
        avatarMesh = thisObj.GetComponent<SpriteRenderer>();
        anim = thisObj.GetComponent<Animator>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        audio = GetComponent<BossAudio>();
    }

    private void FixedUpdate()
    {
        // ０は本体になるため表示しない
        if (myID == 0) active = false;

        if (avatarMesh.enabled)
        {
            // 可視状態
            invisible = false;

            if(!startAnim)
            {
                startAnim = true;
                // 出現のアニメーション ----------
                audio.bossSource.Stop();
                audio.Boss4_DummySpawnSound();
                anim.Play("Spawn");
                Debug.Log("しゅつげん～");

            }//----- if_stop -----
        }
        else
        {
            // 不可視状態
            invisible = true;

            if (startAnim)
            {
                startAnim = false;
                // 退場のアニメーション ----------
                audio.bossSource.Stop();
                audio.Boss4_DummyDeleteSound();
                ef_Manager.StartRemoveInvisible();
                Debug.Log("たいじょう～");

            }//----- if_stop -----
        }
    }

    //==================================================
    //          分身解除関数
    // ※攻撃を受けた時、被ダメリアクション後
    // 　透明化、倒された状態を出力します
    //==================================================
    // 制作日2023/05/30
    // 宮﨑
    private IEnumerator RemoveAvatar()
    {
        // 初期値を格納する
        sbyte speed = ascendSpeed;
        anim.Play("Damage");
        
        while (true)
        {
            // 上昇速度がなくなったら処理を抜ける
            if(avatartRb.velocity.y < 0) break;

            // 速度を付与する
            avatartRb.velocity = new Vector3(avatartRb.velocity.x, speed, 0);
            speed--;

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        audio.bossSource.Stop();
        audio.Boss4_DummyDeleteSound();
        // 速度を消す
        avatartRb.velocity = Vector3.zero;
        

        // 分身を解除する
        avatarMesh.enabled = false;
        active = false;
    }
}