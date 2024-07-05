using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAct : MonoBehaviour
{
    // ���g�̃����_���[���擾
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField] private Color m_Color;

    // �_���[�W���󂯂���
    public bool getDamage;

    // ���G���Ԃ̐ݒ�
    public int invincibleFrame;

    // �����x�̐ݒ�
    [SerializeField, Range(0f, 255f)] private int transparency;

    // �`���Ԃ̕\��
    [SerializeField] private bool drawingState;

    [SerializeField] private AnimationCurve flashCurve;

    private float onePeriodTime;
    [System.NonSerialized] public float elapsedTime;

    private CheckPointManager CheckPointManager; // ������

    private IEnumerator InvincibleFrameCount()
    {
        for (int i = 0; i < invincibleFrame; i++)
        {//----- for_start -----

            yield return null;

            // �_���[�W�擾���肪�Ȃ��Ȃ�
            if (!getDamage)
            {//----- if_start -----

                yield break;

            }//----- if_stop -----

            if (!drawingState)
            {//----- if_start -----

                // �`���Ԃ�s����Ԃɂ���
                drawingState = true;

            }//----- if_stop -----
            else
            {//----- else_start -----

                // �`���Ԃ�����Ԃɂ���
                drawingState = false;

            }//----- else_stop -----

        }//----- for_stop -----

        // �_���[�W�擾��Ԃ�j��
        getDamage = false;
    }

    private void Start()
    {
        // ���g�̃}�e���A�����擾
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        // �����_���[���擾�ł��Ȃ������Ƃ�
        if (m_SpriteRenderer == null)
        {//----- if_start -----

            // �����_���[��擾�̃G���[��\��
            Debug.LogError("[SpriteRenderer]�������Ă��܂���I");

        }//----- if_stop -----


        var length = flashCurve.length;
        if (length < 1)
        {
            return;
        }
        onePeriodTime = flashCurve.keys[length - 1].time;

        CheckPointManager = new CheckPointManager();
    }

    private void FixedUpdate()
    {

        //m_Color.a = transparency;

        //m_MeshRenderer.material.color = m_Color;

        if (getDamage)
        {//----- if_start -----

            elapsedTime += Time.deltaTime;
            if (elapsedTime > onePeriodTime)
            {
                elapsedTime = Mathf.Repeat(elapsedTime, onePeriodTime);
            }

            var meshAlpha = flashCurve.Evaluate(elapsedTime);
            var color = m_SpriteRenderer.color;
            color.a = meshAlpha;

            m_SpriteRenderer.color = color;

            StartCoroutine(InvincibleFrameCount());

        }//----- if_stop -----
        else
        {//----- else_start -----

            //StopCoroutine(InvincibleFrameCount());
            var color = m_SpriteRenderer.color;
            color.a = 1;
            m_SpriteRenderer.color = color;
            elapsedTime = 0;
            // �`���Ԃ�����Ԃɂ���
            drawingState = false;

        }//----- else_stop -----
        if (StatusManager.nowHitPoint <= 0 && getDamage)
        {
            if (!CheckPointManager.GetIsNowRespawning())  // ������
            {
                getDamage = false;
                StartCoroutine(gameObject.GetComponent<Player_Main>().Respawn());
            }
        }
    }
}
