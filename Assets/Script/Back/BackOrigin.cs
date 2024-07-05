using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackOrigin : MonoBehaviour
{
    // Start is called before the first frame update
    public enum BackLocate
    {
        Left = 0,
        Center,
        Right,

        Cnt,
    }

    private string playerTag = "Player";
    private Transform playerTrans;

    private Transform[] backsTrans;
    private float[] backsScaleHarfX;
    private SpriteRenderer[] backsSprite;
    private Transform cameraTrans;
    private float cameraDisY;

    

    void Start()
    {
        
        // �q�I�u�W�F�̔w�i�B���擾
        backsTrans = new Transform[transform.childCount];
        backsSprite = new SpriteRenderer[transform.childCount];
        for(int i = 0; i < backsTrans.Length; i++)
        {
            backsTrans[i] = transform.GetChild(i).GetComponent<Transform>();
            backsSprite[i] = backsTrans[i].GetComponent<SpriteRenderer>();

        }
        if(backsTrans.Length  != (int)BackLocate.Cnt)
        {
            Debug.LogError("�w�i1�Z�b�g�Ɋ܂ގq�I�u�W�F�̐���"+(int)BackLocate.Cnt+"�ɂ��Ă�������");
        }

        

        // �v���C���[�̃g�����X�t�H�[���i�[
        playerTrans = GameObject.FindWithTag(playerTag).transform;
        if(playerTrans == null)
        {
            Debug.LogError("�v���C���[��������܂���");
        }
        // �J�����̃g�����X�t�H�[���i�[
        cameraTrans = GameObject.FindWithTag("MainCamera").transform;
        if(cameraTrans == null)
        {
            Debug.LogError("�J������������܂���");
        }
        // �w�i�̃X�P�[�����i�[����z��̍쐬
        backsScaleHarfX = new float[(int)BackLocate.Cnt];

        // ���ۂ̃V�[����ł̑傫���̔������擾
        for (int i = 0; i < backsTrans.Length; i++)
        {
            backsScaleHarfX[i] = backsTrans[i].transform.lossyScale.x * backsSprite[i].size.x / 2;
        }

        // �o�u���\�[�g�ŕ��ёւ�
        BacksSort();

        // �����ʒu�𐳊m�Ȉʒu�Ɉړ�
        InitBacksPosX();

        var trans = transform.position;
        trans.y = cameraTrans.position.y;
        transform.position = trans;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTrans.position.x >
            backsTrans[(int)BackLocate.Center].position.x + backsScaleHarfX[(int)BackLocate.Center])
        {
            var transBuf = backsTrans[(int)BackLocate.Left].position;
            transBuf.x = backsTrans[(int)BackLocate.Right].position.x +
                backsScaleHarfX[(int)BackLocate.Right] +
                backsScaleHarfX[(int)BackLocate.Left];
            backsTrans[(int)BackLocate.Left].position = transBuf;

            // �o�u���\�[�g�ŕ��ёւ�
            BacksSort();
        }
        else if (cameraTrans.position.x <
             backsTrans[(int)BackLocate.Center].position.x - backsScaleHarfX[(int)BackLocate.Center])
        {
            var transBuf = backsTrans[(int)BackLocate.Right].position;
            transBuf.x = backsTrans[(int)BackLocate.Left].position.x -
                backsScaleHarfX[(int)BackLocate.Left] -
                backsScaleHarfX[(int)BackLocate.Right];
            backsTrans[(int)BackLocate.Right].position = transBuf;


            // �o�u���\�[�g�ŕ��ёւ�
            BacksSort();
        }
        var trans = transform.position;
        trans.y = cameraTrans.position.y;
        transform.position = trans;
    }

    void BacksSort()
    {
        for(int i = 0; i < (int)BackLocate.Cnt; i++)
        {
            float min = float.MaxValue;
            for (int j = i; j < (int)BackLocate.Cnt; j++)
            {
                if (backsTrans[j].transform.position.x < min)
                {
                    min = backsTrans[j].transform.position.x;

                    var transBuf = backsTrans[i];
                    backsTrans[i] = backsTrans[j];
                    backsTrans[j] = transBuf;

                    var spriteBuf = backsSprite[i];
                    backsSprite[i] = backsSprite[j];
                    backsSprite[j] = spriteBuf;

                }
            }
            
        }
        
    }

    void InitBacksPosX()
    {
        // ���̈ʒu�𐧌�
        var transBuf = backsTrans[(int)BackLocate.Left].position;
        transBuf.x = backsTrans[(int)BackLocate.Center].position.x - backsScaleHarfX[(int)BackLocate.Center]
            - backsScaleHarfX[(int)BackLocate.Left];
        transBuf.y = transform.position.y;
        backsTrans[(int)BackLocate.Left].position = transBuf;

        // �E�̈ʒu�𐧌�
        transBuf = backsTrans[(int)BackLocate.Right].position;
        transBuf.x = backsTrans[(int)BackLocate.Center].position.x + backsScaleHarfX[(int)BackLocate.Center]
            + backsScaleHarfX[(int)BackLocate.Right];
        transBuf.y = transform.position.y;
        backsTrans[(int)BackLocate.Right].position = transBuf;

        // �^�񒆂̈ʒu�𐧌�
        transBuf = backsTrans[(int)BackLocate.Center].position;
        transBuf.y = transform.position.y;
        backsTrans[(int)BackLocate.Center].position = transBuf;

    }
}
