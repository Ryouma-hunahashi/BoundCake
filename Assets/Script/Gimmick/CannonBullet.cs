using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public enum STATE
    {
        STAY,
        SHOT,
        KNOCK_BACK,
    }
    
    private STATE state = STATE.STAY;
    // 状態のチェッカー、セッター
    public bool CheckState(STATE _st){ return state == _st; }
    public void SetState(STATE _st) { state = _st; }


    private float speed;
    private float destroyTime;
    private Vector3 vec;
    private CannonBase.VECTOR paramVec;
    private float elapsetTime;

    private Rigidbody rb;
    private Enemy_KB knockBackSqript;
    private Vector3 stayPos;

    


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogError("Rigidbodyをコンポーネントしなさい");
        }
        knockBackSqript = GetComponent<Enemy_KB>();
        if(knockBackSqript == null)
        {
            Debug.LogError("ノックバックの登録をしましょう");
        }
        stayPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case STATE.STAY:

                break;
            case STATE.SHOT:
                rb.velocity = vec * speed;
                elapsetTime += Time.deltaTime;
                if(elapsetTime >= destroyTime)
                {
                    End();
                }
                
                break;
            case STATE.KNOCK_BACK:
                if(knockBackSqript.fallPow<=0)
                {
                    End();
                }
                break;

            default:

                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        switch(other.tag)
        {
            case WaveManager.waveTag:

                if (!CheckState(STATE.SHOT))
                {
                    return;
                }

                var waveSqript = other.GetComponent<waveCollition>();
                if( WaveManager.instance.CheckStrongWave( waveSqript.GetMaxHight() ) &&
                    waveSqript.nowHightIndex <= 1.0f)
                {
                    HitStrongWave(other, waveSqript);
                }
                else if(WaveManager.instance.CheckStrongWave(waveSqript.nowHight))
                {
                    HitStrongWave(other, waveSqript);
                }
                else
                {
                    HitWeakWave(other, waveSqript);
                }
                break;
            case "Bullet":
                if (!CheckState(STATE.SHOT))
                {
                    return;
                }
                End();

                break;
        }

        if(other.gameObject.layer == 6||other.gameObject.layer == 8)
        {
            End();
        }
    }

    private void HitWeakWave(Collider _other, waveCollition _waveSqript)
    {
        switch(paramVec)
        {
            case CannonBase.VECTOR.RIGHT:
                if(_other.transform.position.x>transform.position.x)
                {
                    vec *= (-1);
                }
                break;
            case CannonBase.VECTOR.LEFT:
                if(_other.transform.position.x<transform.position.x)
                {
                    vec *= (-1);
                }
                break;
            case CannonBase.VECTOR.UP:
                if (_other.transform.position.y > transform.position.y)
                {
                    vec *= (-1);
                }
                break;
            case CannonBase.VECTOR.DOWN:
                if(_other.transform.position.y<transform.position.y)
                {
                    vec *= (-1);
                }
                break;
        }
    }

    private void HitStrongWave(Collider _other,waveCollition _waveSqript)
    {
        knockBackSqript.hightLevel = 1;
        knockBackSqript.hitPlayerWave = true;
        knockBackSqript.fallPow = knockBackSqript.blowOfHight[1];
        knockBackSqript.waveTest = false;
        state = STATE.KNOCK_BACK;
    }

    /// <summary>
    /// 弾を打ち出す
    /// </summary>
    /// <param name="_spd">弾の速度</param>
    /// <param name="_startPos">"弾の発射地点"</param>
    /// <param name="_vec3">弾の射出方向</param>
    /// <param name="_destroyTime">弾の消滅時間</param>
    /// <param name="_vec">進んでいる方向</param>
    // 作成日 2023/9/30
    // 高田
    public void Shot(float _spd,Vector3 _startPos,Vector3 _vec3,float _destroyTime,CannonBase.VECTOR _vec)
    {
        speed = _spd;
        transform.position = _startPos;
        vec = _vec3;
        destroyTime = _destroyTime;
        paramVec = _vec;
        state = STATE.SHOT;
    }

    public void End()
    {
        state = STATE.STAY;
        speed = 0;
        vec = Vector3.zero;
        destroyTime = 0;
        elapsetTime = 0;
        transform.position = stayPos;
        knockBackSqript.End();
    }

    

    
}
