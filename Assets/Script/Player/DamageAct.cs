using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAct : MonoBehaviour
{
    // 自身のレンダラーを取得
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField] private Color m_Color;

    // ダメージを受けた時
    public bool getDamage;

    // 無敵時間の設定
    public int invincibleFrame;

    // 透明度の設定
    [SerializeField, Range(0f, 255f)] private int transparency;

    // 描画状態の表示
    [SerializeField] private bool drawingState;

    [SerializeField] private AnimationCurve flashCurve;

    private float onePeriodTime;
    [System.NonSerialized] public float elapsedTime;

    private CheckPointManager CheckPointManager; // あさわ

    private IEnumerator InvincibleFrameCount()
    {
        for (int i = 0; i < invincibleFrame; i++)
        {//----- for_start -----

            yield return null;

            // ダメージ取得判定がないなら
            if (!getDamage)
            {//----- if_start -----

                yield break;

            }//----- if_stop -----

            if (!drawingState)
            {//----- if_start -----

                // 描画状態を不可視状態にする
                drawingState = true;

            }//----- if_stop -----
            else
            {//----- else_start -----

                // 描画状態を可視状態にする
                drawingState = false;

            }//----- else_stop -----

        }//----- for_stop -----

        // ダメージ取得状態を破棄
        getDamage = false;
    }

    private void Start()
    {
        // 自身のマテリアルを取得
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        // レンダラーが取得できなかったとき
        if (m_SpriteRenderer == null)
        {//----- if_start -----

            // レンダラー非取得のエラーを表示
            Debug.LogError("[SpriteRenderer]が入っていません！");

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
            // 描画状態を可視状態にする
            drawingState = false;

        }//----- else_stop -----
        if (StatusManager.nowHitPoint <= 0 && getDamage)
        {
            if (!CheckPointManager.GetIsNowRespawning())  // あさわ
            {
                getDamage = false;
                StartCoroutine(gameObject.GetComponent<Player_Main>().Respawn());
            }
        }
    }
}
