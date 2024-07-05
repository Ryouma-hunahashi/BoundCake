using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Goal : MonoBehaviour
{
    [SerializeField] OtherEffectManager effect;
    [SerializeField, TagField]
    private string ContactTag; // �w��̃^�O�ݒ�

    [SerializeField, Tooltip("�w�莞�Ԃŉ�ʑJ��")]
    private float weight;

    private bool goalflg; // �S�[������ӂ炮


    Player_Main pl_main;
    [System.Serializable]
    public enum GoalType
    {
        result1, // �X�e�[�W�P�̃��U���g
        result2, // �X�e�[�W�Q�̃��U���g
        result3, // �X�e�[�W�R�̃��U���g
        result4, // �X�e�[�W�S�̃��U���g�@
        result5, // �X�e�[�W�T�̃��U���g
    }

    public GoalType resultkinds = GoalType.result1;

    // Start is called before the first frame update
    void Start()
    {
        goalflg = false;
    }
    // Update is called once per frame
    void Update()
    {
        // �S�[�������𖞂������Ƃ�
        if (goalflg)
        {
            effect.StopCooky();

            // �e���U���g�V�[���Ɉړ�����
            switch (resultkinds)
            {
                case GoalType.result1:
                    SceneManager.LoadScene("result1");
                    break;
                case GoalType.result2:
                    SceneManager.LoadScene("result2");
                    break;
                case GoalType.result3:
                    SceneManager.LoadScene("result3");
                    break;
                case GoalType.result4:
                    SceneManager.LoadScene("result4");
                    break;
                case GoalType.result5:
                    SceneManager.LoadScene("result5");
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �w��^�O�ɐG�ꂽ��
        if (other.gameObject.tag == ContactTag) 
        {
            Debug.Log(Result_Manager.instance.nowStage);
            // pl_main.jumpButton = false;
            effect.StartCooky();
            // �R���[�`���𔭐�
            StartCoroutine("Stop");
        }
    }
    IEnumerator Stop()
    {
        //----------------
        // �G�t�F�N�g�Đ�
        //---------------
       

        // ���̎��Ԃ̊ԏ�������U��~����
        yield return new WaitForSeconds(weight);
        // �S�[������t���O��true�ɂ���
        goalflg = true;
    }
}
