using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Parfait : MonoBehaviour
{
    // �w�̎w��
    private enum ParfaitLayer
    {
        top,    // ��w
        mid,    // ���w
        btm,    // ���w

        none,   // ���w��
    }

    // �p�t�F�̐ݒ�
    [SerializeField] private ParfaitLayer parfait = ParfaitLayer.none;

    [SerializeField] private bool test;

    public byte worldNum;
    public byte stageNum;

    SpriteRenderer spriteRenderer;
    StageSelector stageSelector;

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ɐG�ꂽ��
        if(other.CompareTag("Player"))
        {
            // �p�t�F�̎擾�����X�V����
            parfaitUpdate();

        }//----- if_stop -----
    }

    private void Start()
    {
        worldNum = Result_Manager.instance.nowWorld;
        stageNum = Result_Manager.instance.nowStage;
    }
    private void Update()
    {
        if(test)
        {
            parfaitUpdate();
            test = false;
        }
       // parfaitSecondGet();

    }

    //========================================
    //          �p�t�F�̏����X�V
    //  ���Q�[�����Ƀp�t�F��������ꂽ����s
    //========================================
    // �쐬��2023/05/20    �X�V��2023/05/21
    // �{��
    private void parfaitUpdate()
    {
        switch(parfait)
        {
            case ParfaitLayer.top:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top = true;
                //Result_Manager.instance.getParfait.top = true;
                Debug.Log("toptrue");
                break;
            case ParfaitLayer.mid:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid = true;
                //Result_Manager.instance.getParfait.mid = true;
                Debug.Log("middletrue");
                break;
            case ParfaitLayer.btm:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm = true;
                //Result_Manager.instance.getParfait.btm = true;
                Debug.Log("botoomtrue");
                break;
            case ParfaitLayer.none:
            default:
                break;

        }//----- switch_stop -----
    }

    void parfaitSecondGet()
    {
        switch (parfait)
        {
            case ParfaitLayer.top:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top)
                {
                    this.gameObject.GetComponentsInChildren<SpriteRenderer>();

                    //spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
                }
                break;
            case ParfaitLayer.mid:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid)
                {

                }
                break;
            case ParfaitLayer.btm:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm)
                {

                }
                break;
            default:
                break;
        }
    }
}
