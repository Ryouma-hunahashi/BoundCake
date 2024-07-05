using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          “Gƒ‚ƒu‚Ì‚–hŒä
//==================================================
// ì¬“ú2023/05/19
// ‹{ú±
public class Enemy_Shield : MonoBehaviour
{
    // e‚Ìî•ñ‚ğæ“¾
    private GameObject par_Obj;
    private Vector3 par_Scale;

    [Header("----- ‚‚Ìİ’è -----")]
    private CapsuleCollider shieldCol;      // ‚‚Ì“–‚½‚è”»’è
    public bool nowBreak;     // ”j‰óó‘Ô
    public sbyte durability;  // ‘Ï‹v’l
    public bool unbreakable;
    [SerializeField] private byte unbreakableFrame; // ”j‰ó•s‰ÂŠÔ

    Animator enemyAinim;

    private void OnTriggerEnter(Collider other)
    {
        // "Wave"‚½‹ï‚Ì•t‚¢‚½ƒIƒuƒWƒFƒNƒg‚ªG‚ê‚½
        if(other.CompareTag("Wave"))
        {
            // ‚‚ª‘¶İ‚µ‚Ä‚¢‚é‚Æ‚«
            // ‘Ï‹v’l‚ğŒ¸‚ç‚·‚±‚Æ‚ª‚Å‚«‚é‚È‚ç
            if (!nowBreak && !unbreakable)
            {
                durability--;

                // ˜A‘±‚Åƒ_ƒ[ƒW‚ğ—^‚¦‚ç‚ê‚È‚¢‚æ‚¤‚É‚·‚é
                unbreakable = true;

                // –³“GŠÔ‚ğİ‚¯‚é
                StartCoroutine(ShieldUnbreakable());
            }
        }
        enemyAinim = transform.parent.GetComponent<Animator>();
        if(enemyAinim == null)
        {
            enemyAinim = transform.parent.parent.GetComponent<Animator>();
            if(enemyAinim == null)
            {
                Debug.LogError("Animator‚ª‚ ‚è‚Ü‚¹‚ñ");
            }
        }
    }

    private void Start()
    {
        // e‚Ìî•ñ‚ğæ“¾
        par_Obj = transform.parent.gameObject;
        par_Scale = par_Obj.transform.localScale;

        // ‚‚Ì“–‚½‚è”»’è‚ğæ“¾
        shieldCol = this.GetComponent<CapsuleCollider>();

        // ‚‚ğ—LŒø‰»‚·‚é
        shieldCol.enabled = true;
    }

    private void FixedUpdate()
    {
        // ‘Ï‹v’l‚ª‚OˆÈ‰º‚É‚È‚Á‚½‚È‚ç
        if((durability <= 0) && !nowBreak)
        {
            // ‚‚ª”j‰ó‚³‚ê‚é
            nowBreak = true;
            enemyAinim.SetBool("shieldBreak", true);
            StartCoroutine(ShieldUnbreakable());

        }//----- if_stop -----
    }

    private IEnumerator ShieldUnbreakable()
    {
        // ‚‚ª”j‰ó‚³‚ê‚Ä‚¢‚é‚È‚ç
        if (nowBreak)
        {
            // “–‚½‚è”»’è‚ğÁ–Å‚³‚¹‚é
            shieldCol.enabled = false;

        }//----- if_stop -----

        // İ’è‚³‚ê‚½–³“GŠÔ‚ÌŠÔ‚­‚è‚©‚¦‚·
        for (int i = 0; i < unbreakableFrame; i++)
        {
            // ‚PƒtƒŒ[ƒ€’x‰„‚³‚¹‚é
            yield return null;

        }//----- for_stop -----

        // –³“GŠÔ‚ğ”²‚¯‚é‚Ì‚Å‰ğœ
        unbreakable = false;
    }
}