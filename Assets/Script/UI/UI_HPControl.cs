using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPControl : MonoBehaviour
{
    // 現HPの設定 ----- テスト
    [SerializeField]private int hitPointLog;

    [SerializeField] private bool damage;
    private bool nowFlash;

    // 各HP状態の色を設定する
    [SerializeField] private Color hitPoint_Color;
    [SerializeField] private Color damage_Color;

    [SerializeField] private AnimationCurve flashCurve;

    [SerializeField] private Sprite nomalHP;
    [SerializeField] private Sprite overHP;

    [Tooltip("※HPの上限を超えないように設定してください")]
    [SerializeField] private List<Image> listHPImage = new List<Image>();

    private DamageAct pl_Act;
    private void Start()
    {
        for (int i = 0; i < listHPImage.Count; i++)
        {
            listHPImage[i] = transform.GetChild(i).gameObject.GetComponent<Image>();
        }
        pl_Act = GameObject.FindWithTag("Player").GetComponent<DamageAct>();
    }

    private void Update()
    {
        if (hitPointLog > StatusManager.nowHitPoint)
        {
            hitPointLog = StatusManager.nowHitPoint;
            StartCoroutine(DamageFlash());
        }
        else if(hitPointLog < StatusManager.nowHitPoint)
        {
            hitPointLog = StatusManager.nowHitPoint;
            HitPointSet();

        }
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            if (StatusManager.nowHitPoint < StatusManager.maxHitPoint)
//            {
//                StatusManager.nowHitPoint++;
//            }
//        }
//#endif
        //if(damage)
        //{
        //    // こいつこるーちんにおくるよ
        //    for(int i = 0; i < listHPImage.Count;i++)
        //    {

        //    }
        //}


        //if(nowFlash)
        //{
        //    var colorLunk = flashCurve.Evaluate(pl_Act.elapsedTime);
        //    Debug.Log("nowHitPoint = "+StatusManager.nowHitPoint);
        //    var color = listHPImage[StatusManager.nowHitPoint%3].color;
        //    color.r = colorLunk;
        //    color.g = colorLunk;
        //    color.b = colorLunk;
        //    listHPImage[StatusManager.nowHitPoint%3].color = color;
        //    if(!pl_Act.getDamage)
        //    {
        //        if (StatusManager.nowHitPoint == 3 || StatusManager.nowHitPoint == 6)
        //        {
        //            listHPImage[StatusManager.nowHitPoint % 3].color = damage_Color;
        //            nowFlash = false;
        //        }
        //        else
        //        {
        //            listHPImage[StatusManager.nowHitPoint % 3].color = hitPoint_Color;
        //        }
        //    }
        //}
        //if(!pl_Act.getDamage)
        //{
        //    nowFlash = false;
        //}
    }
    private IEnumerator DamageFlash()
    {
        if(StatusManager.nowHitPoint<0)
        {
            StatusManager.nowHitPoint = 0;
        }
        for (byte i = 0; i < pl_Act.invincibleFrame; i++)
        {
            if (!pl_Act.getDamage)
            {
                yield break;
            }
            yield return null;
            
            var colorLunk = flashCurve.Evaluate(pl_Act.elapsedTime);
            var color = listHPImage[StatusManager.nowHitPoint%3].color;
            color.r = colorLunk;
            color.g = colorLunk;
            color.b = colorLunk;
            listHPImage[StatusManager.nowHitPoint%3].color = color;
        }

        HitPointSet();
    }
    private void HitPointSet()
    {
        if (StatusManager.nowHitPoint == 6)
        {
            listHPImage[0].sprite = overHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = overHP;
            listHPImage[1].color = hitPoint_Color;
            listHPImage[2].sprite = overHP;
            listHPImage[2].color = hitPoint_Color;
          
        }
        if (StatusManager.nowHitPoint == 5)
        {
            listHPImage[0].sprite = overHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = overHP;
            listHPImage[1].color = hitPoint_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = hitPoint_Color;
           
        }

        if (StatusManager.nowHitPoint == 4)
        {
            listHPImage[0].sprite = overHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = nomalHP;
            listHPImage[1].color = hitPoint_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = hitPoint_Color;
            
        }

        if (StatusManager.nowHitPoint == 3)
        {
            listHPImage[0].sprite = nomalHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = nomalHP;
            listHPImage[1].color = hitPoint_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = hitPoint_Color;
            
        }

        if (StatusManager.nowHitPoint == 2)
        {
            listHPImage[0].sprite = nomalHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = nomalHP;
            listHPImage[1].color = hitPoint_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = damage_Color;
          
        }

        if (StatusManager.nowHitPoint == 1)
        {
            listHPImage[0].sprite = nomalHP;
            listHPImage[0].color = hitPoint_Color;
            listHPImage[1].sprite = nomalHP;
            listHPImage[1].color = damage_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = damage_Color;
            
        }

        if (StatusManager.nowHitPoint == 0)
        {
            listHPImage[0].sprite = nomalHP;
            listHPImage[0].color = damage_Color;
            listHPImage[1].sprite = nomalHP;
            listHPImage[1].color = damage_Color;
            listHPImage[2].sprite = nomalHP;
            listHPImage[2].color = damage_Color;
           
        }
    }

    private void OnApplicationQuit()
    {
        listHPImage.Clear();
    }
}
