using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�؂�ւ���̃J����")]
    private CinemachineVirtualCamera virtualCamera;

    // �؂�ւ���̃J�����̌��X��Priority��ێ����Ă���
    private int defaultPriority;

    // �R���[�`����ϐ��Ŋi�[����
    public IEnumerator sleep;

    private bool playercheck;
    // Start is called before the first frame update
    void Start()
    {
        // ���X�̗D��x��ۑ����Ă���
        defaultPriority = virtualCamera.Priority;
        // �R���[�`�����i�[����
        sleep = BossScene();
    }

    // Update is called once per frame
    void Update()
    {
        if(playercheck)
        {
            Debug.Log("���Ă��`");
            Invoke("changebosscamera", 3);

            Invoke("changeplayercamera", 3);
            //StartCoroutine(sleep);

           // virtualCamera.Priority = 200;

            //StartCoroutine(sleep);
            // ����priority�ɖ߂�
            virtualCamera.Priority = defaultPriority;

            playercheck = false;
        }
    }

    private void changebosscamera()
    {
        Debug.Log("3�b��������`");
        virtualCamera.Priority = 200;

    }
    private void changeplayercamera()
    {
        // ����priority�ɖ߂�
        virtualCamera.Priority = defaultPriority;
    }
    public IEnumerator BossScene()
    {
        Debug.Log("�������Ă��[");
        yield return new WaitForSeconds(1);
        Debug.Log("�R���[�`���ʂ��Ă��`");
        virtualCamera.Priority = 200;
    }
    private void OnTriggerEnter(Collider other)
    {
        // �������������"Player"�^�O���t���Ă����ꍇ
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("�������Ă��[");

            playercheck = true;

            //StartCoroutine(sleep);
            //Debug.Log("�������Ă��[");
            //// ����virtualCamera���������D��x�ɂ��邱�ƂŐ؂�ւ��
         

            //StartCoroutine(sleep);
            //// ����priority�ɖ߂�
            //virtualCamera.Priority = defaultPriority;
        }
    }
   
}
