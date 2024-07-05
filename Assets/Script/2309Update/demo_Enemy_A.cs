using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo_Enemy_A : MonoBehaviour
{
    public enum DIRECTION
    {
        LEFT,
        RIGHT,
    }

    private struct ENEMY_OBJECT
    {
        // オブジェクトについている情報
        private Rigidbody rb;
        private Animator anim;
        private EnemyAudio audio;
        private Enemy_KB knockback;

        // ゲットセッター
        public Rigidbody GetRigidbody() { return rb; }
        public Animator GetAnimator() { return anim; }
        public EnemyAudio GetAudio() { return audio; }
        public Enemy_KB GetKB() { return knockback; }
        public void SetRidbody(Rigidbody _rigidbody) { rb = _rigidbody; }
        public void SetAnimator(Animator _animator) { anim = _animator; }
        public void SetAudio(EnemyAudio _audio) { audio = _audio; }
        public void SetKB(Enemy_KB _kbScript) { knockback = _kbScript; }
    }

    Enemy_Detect c_detect;

    [SerializeField] private bool summoned;
    public bool inCamera;

    // 召喚元を取得
    public GameObject boss5;
    private demo_B5 boss5_cs;

    // 処理開始時のX座標を格納
    private float startPosition;

    // 自身の行動を開始する
    [SerializeField] private bool start;

    // 自身の向いている方向
    private DIRECTION direction;
    public DIRECTION GetDirection() { return direction; }
    public void SetDirection(DIRECTION _dir) { direction = _dir; }

    // 自身についているデータ
    [Header("＊主軸データ")]
    [SerializeField] private ENEMY_OBJECT objData;
    [SerializeField] private float EnemyScale;
    [SerializeField] private float detectionRange;  //検知範囲

    // 各判定
    private bool isSummoned;    // 召喚された
    private bool isCharge;      // 溜め中
    private bool isTackle;      // 突進中か


    public bool GetChargeState() { return isCharge; }
    public bool GetTackleState() { return isTackle; }

    [Header("＊突進の設定")]
    [SerializeField] private float chargeTime;  // 溜め時間
    [SerializeField] private float tackleTime;  // 突進時間
    [SerializeField] private float speedMag;    // 速度倍率

    [Header("＊移動の設定")]
    [SerializeField] private float speed;       // 移動速度

    [Header("＊距離の設定")]
    [SerializeField] private float distMin; // 最小位置
    [SerializeField] private float distMax; // 最大位置

    public void SetDistanceMin(float _min) { distMin = _min; }
    public void SetDistanceMax(float _max) { distMax = _max; }

    [Header("＊壁衝突の設定")]
    [SerializeField] private float rayDist;                 // レイの距離
    [SerializeField] private LayerMask gLayer = 1 << 6;     // レイヤーの設定

    [Header("＊吹き飛び時の設定")]
    [SerializeField] private Vector2 knockbackPower;    // 吹き飛び速度
    [SerializeField] private float stanTime;            // 衝突スタンの設定

    private bool isKnockback;

    private void Awake()
    {
        // 自身の情報を取得
        objData.SetRidbody(this.GetComponent<Rigidbody>());    // リジッドボディ
        objData.SetAnimator(this.GetComponent<Animator>());    // アニメーター
        objData.SetAudio(this.GetComponent<EnemyAudio>());     // ボスのオーディオ

        c_detect = GetComponent<Enemy_Detect>();

        // 召喚される敵なら以下を処理しない
        if (!summoned) return;

        // ボスのスクリプトを取得
        boss5 = GameObject.Find("boss5").gameObject;
        boss5_cs = boss5.GetComponent<demo_B5>();
    }

    private void Start()
    {
        startPosition = this.transform.position.x;

        transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
    }

    private void FixedUpdate()
    {
        Vector3 pos = this.transform.position;
        bool wallHit = HorizontalRayHitWall(pos);

        // 突進して壁に当たったとき、ノックバック中でなければスタンさせる
        if(wallHit && isTackle && !isKnockback) CollidedWall();

        // 召喚されたモブがカメラの外に出たなら
        if (!inCamera && summoned)
        {
            // 召喚状態に移行する
            boss5_cs.SetState(Enemy_Boss.STATE.ATK01);

            // 存在を消す
            Destroy(this.gameObject);
        }

        // モブが下のほうに行ったとき
        if(pos.y < -20)
        {
            // もし召喚モブだったならボスを召喚攻撃状態にする
            if(summoned) boss5_cs.SetState(Enemy_Boss.STATE.ATK01);

            // 存在を消す
            Destroy(this.gameObject);
        }

        // 開始していないなら処理を抜ける
        if (!start) return;

        // 溜めや、突進している間なら処理を抜ける
        if (isCharge || isTackle || isKnockback) return;

        // 向いている方向に進む関数を呼び出す
        MoveObject(speed);

        // 通り過ぎた時反対を向かせる関数
        PassedTheDestination(pos.x);

        // 索敵できたなら
        if(c_detect.GetDetect())
        {
            // 突進攻撃を開始する
            StartCoroutine(TackleAction());

            // 索敵できていない状態にする
            //isDetect = false;
        }
    }

    private void CollidedWall()
    {
        isKnockback = true;
        isTackle = false;

        // 現在使用しているすべてのコルーチンを止める
        StopAllCoroutines();

        // ノックバックを開始する
        StartCoroutine(WallHitKnockBack(direction));

        return;
    }

    private bool HorizontalRayHitWall(Vector3 _pos)
    {
        Ray ray = new Ray();
        RaycastHit hit;

        switch(direction)
        {
            case DIRECTION.LEFT:
                ray = new Ray(new Vector3(_pos.x, _pos.y, _pos.z), new Vector3(-1.0f, 0.0f, 0.0f));
                break;
            case DIRECTION.RIGHT:
                ray = new Ray(new Vector3(_pos.x, _pos.y, _pos.z), new Vector3(1.0f, 0.0f, 0.0f));
                break;
        }

        return Physics.Raycast(ray, out hit, rayDist, gLayer);
    }

    private IEnumerator WallHitKnockBack(DIRECTION _dir)
    {
        // 吹飛び処理に使用する各変数の作成
        double limFallSpd = -knockbackPower.y;  // 落下速度最低値
        double kbPowX = knockbackPower.x;       // 横の吹飛び速度
        double kbPowY = knockbackPower.y;       // 盾の吹飛び速度

        // アニメーションを停止する
        objData.GetAnimator().SetBool("running", false);

        // 落下速度が最低値を超えるまで繰り返す
        while (objData.GetRigidbody().velocity.y > limFallSpd)
        {
            switch (_dir)
            {
                case DIRECTION.LEFT:
                    {
                        objData.GetRigidbody().velocity = new Vector3((float)kbPowX, (float)kbPowY, 0);
                    }
                    break;
                case DIRECTION.RIGHT:
                    {
                        objData.GetRigidbody().velocity = new Vector3((float)-kbPowX, (float)kbPowY, 0);
                    }
                    break;
            }

            kbPowY -= 1;

            yield return null;
        }

        // スタン待機時間を実施
        yield return new WaitForSeconds(stanTime);

        // 自身の速度を一瞬０にする
        objData.GetRigidbody().velocity = Vector3.zero;

        //ノックバックの終了
        isKnockback = false;
    }

    // 通常移動アニメーション関数
    private void MoveAnimation()
    {
        switch(direction)
        {
            case DIRECTION.LEFT:
                transform.localScale = new Vector3(-1 * EnemyScale, EnemyScale, 1);
                break;
            case DIRECTION.RIGHT:
                transform.localScale = new Vector3(1 * EnemyScale, EnemyScale, 1);
                break;
        }

        objData.GetAnimator().SetBool("running", true);
        if(!objData.GetAudio().enemySource.isPlaying)
        {
            objData.GetAudio().WalkSound();
        }

    }

    // チャージ処理
    private IEnumerator ChargeTime()
    {
        Debug.Log("突進チャージを開始します");

        // チャージ状態にする
        isCharge = true;

        objData.GetAnimator().SetBool("running", false);

        // 移動速度をゼロにする
        objData.GetRigidbody().velocity = Vector3.zero;

        yield return new WaitForSeconds(chargeTime);

        // チャージ状態を解除する
        isCharge = false;
    }

    // 突進カウント処理
    private IEnumerator TackleTime()
    {
        // 突進状態に移行
        isTackle = true;

        yield return new WaitForSeconds(tackleTime);

        // 突進終了
        isTackle = false;
    }

    // 突進処理
    private IEnumerator TackleAction()
    {
        // チャージを待つ
        yield return StartCoroutine(ChargeTime());

        Debug.Log("突進を開始します");

        StartCoroutine(TackleTime());

        while(isTackle)
        {
            MoveObject(speed * speedMag);

            yield return null;
        }
        
    }


    // 速度をつけて動かす関数
    private void MoveObject(float _spd)
    {
        switch (direction)
        {
            case DIRECTION.LEFT:
                objData.GetRigidbody().velocity = new Vector3(-_spd, objData.GetRigidbody().velocity.y, 0);
                break;

            case DIRECTION.RIGHT:
                objData.GetRigidbody().velocity = new Vector3(_spd, objData.GetRigidbody().velocity.y, 0);
                break;
        }

        MoveAnimation();

        return;
    }

    // 座標を通り過ぎた時に反対を向かせる関数
    private void PassedTheDestination(float _pos)
    {
        float min = startPosition - distMin;  // X最小値の座標
        float max = startPosition + distMax;  // X最大値の座標

        // 自身の向いている方向
        switch (direction)
        {
            case DIRECTION.LEFT:
                {
                    // 通り過ぎた時向きを反対にする
                    if (_pos < min) ChangeDirection();
                }
                break;
            case DIRECTION.RIGHT:
                {
                    // 通り過ぎた時向きを反対にする
                    if (_pos > max) ChangeDirection();
                }
                break;
        }
    }

    // 向いている方向を反対にする関数
    private void ChangeDirection()
    {
        switch (direction)
        {
            case DIRECTION.LEFT:
                direction = DIRECTION.RIGHT;
                break;

            case DIRECTION.RIGHT:
                direction = DIRECTION.LEFT;
                break;
        }

        return;
    }
}
