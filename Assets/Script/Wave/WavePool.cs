using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePool : MonoBehaviour
{
    // 波のデータ格納用
    public struct WAVE_DATA
    {
        public GameObject waveObj;
        public waveCollition collision;
    }

    [Header("波のコリジョンプレハブ")]
    [SerializeField] private GameObject waveObj;
    // 波のコリジョンプール
    private List<WAVE_DATA> l_waveData = new List<WAVE_DATA>();
    // 発生させる波を一時的に格納する
    private WAVE_DATA spawnWave;
    [Header("プール内の波の数")]
    [SerializeField] private byte waveCount = 10;

    // 送られてきた糸のオブジェ内に存在するvfxManagerを一時的に格納する。
    private vfxManager vfxManager;
    // vfxManagerから送られてきた発生した波の番号を保存
    private sbyte vfxNum;

    // Start is called before the first frame update
    void Awake()
    {
        for (byte i = 0; i < waveCount; i++)
        {
            l_waveData.Add(InitCollisionPool());

        }//-----for_stop-----
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private WAVE_DATA InitCollisionPool()
    {
        // 一時格納変数
        WAVE_DATA data;

        // 波のオブジェクト生成、格納
        data.waveObj = Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity);
        // 波のオブジェクトからコリジョン用スクリプト取得
        data.collision = data.waveObj.GetComponent<waveCollition>();
        // 波のコリジョンスクリプトにこのオブジェクトプールを登録する。
        data.collision.SetPool(this);
        // 波を一先ず誰のものでもなくする
        data.collision.SetType(waveCollition.WAVE_TYPE.none);

        // データを返す
        return data;
    }

    //====================
    // 引数の情報を元に波を生成する。
    // 第一引数：波の速度
    // 第二引数：波の振動数
    // 第三引数：波の高さ
    // 第四引数：誰の波か
    // 第五引数：波の開始地点X
    // 第六引数：波が発生した糸のTransform
    //====================
    // 制作日2023/9/10
    // 高田
    public bool WaveCreate(float _speed, float _width, float _hight, waveCollition.WAVE_TYPE _type,
                            float _startPosX, Transform _springTrans)
    {
        if (_springTrans.childCount == 1)
        {
            // 受け取った糸のvfxを取得
            vfxManager = _springTrans.GetChild(0).GetComponent<vfxManager>();

            // 波を生成し、vfx上の番号を取得
            vfxNum = vfxManager.WaveSpawn(_startPosX, _speed, _hight, _width, GetWavePat(_type));

            // vfxからの戻り値が正であれば
            if (vfxNum >= 0)
            {
                // スタンバイ中の波を取得
                spawnWave = GetWaveCollision();

                // 動く床に波を生成する場合、その親の子とする。
                if (_springTrans.CompareTag("Floor"))
                {
                    spawnWave.waveObj.transform.SetParent(_springTrans.parent);
                }
                // 発生させる床自体が動く床の設定になっていない場合でも、回転床では親を移動タグに設定している。
                // その為親を見て、そうであるならば上記と同じ処理を行う。
                else if (_springTrans.parent != null &&
                    _springTrans.parent.CompareTag("Floor"))
                {
                    spawnWave.waveObj.transform.SetParent(_springTrans.parent);
                }

                

                spawnWave.collision.SetVFXNum(vfxNum);
                spawnWave.collision.vfxManager = vfxManager;
                spawnWave.collision.SetMode(waveCollition.WAVE_MODE.SETUP);
                spawnWave.collision.SetType(_type);
                spawnWave.waveObj.transform.localScale = new Vector3(0, 0, 1);
                spawnWave.waveObj.transform.position = new Vector3(_startPosX, _springTrans.position.y, 0);
                spawnWave.collision.SetStartPos(spawnWave.waveObj.transform.position);
                spawnWave.collision.SetMaxHight(_hight);
                return true;
            }
            


        }
        Debug.Log(false);
        return false;
    }



    public WAVE_DATA GetWaveCollision()
    {
        byte i;
        for (i = 0; i < waveCount; i++)
        {
            if (l_waveData[i].collision.CheckMode(waveCollition.WAVE_MODE.STANDBY))
            {
                break;
            }//-----if_stop-----
            else if(i == waveCount - 1)
            {
                l_waveData.Add(InitCollisionPool());
                waveCount++;
            }//-----elseif_stop-----
        }//-----for_stop-----

        return l_waveData[i];  
    }

    private byte GetWavePat(waveCollition.WAVE_TYPE _type)
    {
        switch (_type)
        {
            case waveCollition.WAVE_TYPE.PLAYER:
                return 0;
            case waveCollition.WAVE_TYPE.ENEMY:
                return 1;
            case waveCollition.WAVE_TYPE.PLAYER_ENEMY:
                return 0;
            case waveCollition.WAVE_TYPE.PLAYER_POWERUP:
                return 0;
            case waveCollition.WAVE_TYPE.GIMMICK:
                return 1;
            case waveCollition.WAVE_TYPE.none:
                Debug.LogError("誰の波かを設定していません");
                return 10;
            default:
                Debug.LogError("誰の波として突っ込んだ？　条件式等見直しなさい！");
                return 10;

        }//-----switch_stop-----
    }

    public void AddEtoPDamage()
    {
        for(int i = 0; i < waveCount; i++)
        {
            l_waveData[i].waveObj.AddComponent<EtoP_Damage>();
        }
    }


    // アプリケーション終了時にリストを破棄
    private void OnApplicationQuit()
    {
        l_waveData.Clear();
        Debug.Log("波のプール解放");
    }
}
