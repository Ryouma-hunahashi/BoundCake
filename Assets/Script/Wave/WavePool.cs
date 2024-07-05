using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePool : MonoBehaviour
{
    // �g�̃f�[�^�i�[�p
    public struct WAVE_DATA
    {
        public GameObject waveObj;
        public waveCollition collision;
    }

    [Header("�g�̃R���W�����v���n�u")]
    [SerializeField] private GameObject waveObj;
    // �g�̃R���W�����v�[��
    private List<WAVE_DATA> l_waveData = new List<WAVE_DATA>();
    // ����������g���ꎞ�I�Ɋi�[����
    private WAVE_DATA spawnWave;
    [Header("�v�[�����̔g�̐�")]
    [SerializeField] private byte waveCount = 10;

    // �����Ă������̃I�u�W�F���ɑ��݂���vfxManager���ꎞ�I�Ɋi�[����B
    private vfxManager vfxManager;
    // vfxManager���瑗���Ă������������g�̔ԍ���ۑ�
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
        // �ꎞ�i�[�ϐ�
        WAVE_DATA data;

        // �g�̃I�u�W�F�N�g�����A�i�[
        data.waveObj = Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity);
        // �g�̃I�u�W�F�N�g����R���W�����p�X�N���v�g�擾
        data.collision = data.waveObj.GetComponent<waveCollition>();
        // �g�̃R���W�����X�N���v�g�ɂ��̃I�u�W�F�N�g�v�[����o�^����B
        data.collision.SetPool(this);
        // �g����悸�N�̂��̂ł��Ȃ�����
        data.collision.SetType(waveCollition.WAVE_TYPE.none);

        // �f�[�^��Ԃ�
        return data;
    }

    //====================
    // �����̏������ɔg�𐶐�����B
    // �������F�g�̑��x
    // �������F�g�̐U����
    // ��O�����F�g�̍���
    // ��l�����F�N�̔g��
    // ��܈����F�g�̊J�n�n�_X
    // ��Z�����F�g��������������Transform
    //====================
    // �����2023/9/10
    // ���c
    public bool WaveCreate(float _speed, float _width, float _hight, waveCollition.WAVE_TYPE _type,
                            float _startPosX, Transform _springTrans)
    {
        if (_springTrans.childCount == 1)
        {
            // �󂯎��������vfx���擾
            vfxManager = _springTrans.GetChild(0).GetComponent<vfxManager>();

            // �g�𐶐����Avfx��̔ԍ����擾
            vfxNum = vfxManager.WaveSpawn(_startPosX, _speed, _hight, _width, GetWavePat(_type));

            // vfx����̖߂�l�����ł����
            if (vfxNum >= 0)
            {
                // �X�^���o�C���̔g���擾
                spawnWave = GetWaveCollision();

                // �������ɔg�𐶐�����ꍇ�A���̐e�̎q�Ƃ���B
                if (_springTrans.CompareTag("Floor"))
                {
                    spawnWave.waveObj.transform.SetParent(_springTrans.parent);
                }
                // ���������鏰���̂��������̐ݒ�ɂȂ��Ă��Ȃ��ꍇ�ł��A��]���ł͐e���ړ��^�O�ɐݒ肵�Ă���B
                // ���̈אe�����āA�����ł���Ȃ�Ώ�L�Ɠ����������s���B
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
                Debug.LogError("�N�̔g����ݒ肵�Ă��܂���");
                return 10;
            default:
                Debug.LogError("�N�̔g�Ƃ��ē˂����񂾁H�@���������������Ȃ����I");
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


    // �A�v���P�[�V�����I�����Ƀ��X�g��j��
    private void OnApplicationQuit()
    {
        l_waveData.Clear();
        Debug.Log("�g�̃v�[�����");
    }
}
