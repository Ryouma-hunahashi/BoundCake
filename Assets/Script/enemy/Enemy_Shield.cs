using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          GuΜhδ
//==================================================
// μ¬ϊ2023/05/19
// {ϊ±
public class Enemy_Shield : MonoBehaviour
{
    // eΜξρπζΎ
    private GameObject par_Obj;
    private Vector3 par_Scale;

    [Header("----- Μέθ -----")]
    private CapsuleCollider shieldCol;      // Μ½θ»θ
    public bool nowBreak;     // jσσΤ
    public sbyte durability;  // Οvl
    public bool unbreakable;
    [SerializeField] private byte unbreakableFrame; // jσsΒΤ

    Animator enemyAinim;

    private void OnTriggerEnter(Collider other)
    {
        // "Wave"½οΜt’½IuWFNgͺGκ½
        if(other.CompareTag("Wave"))
        {
            // ͺΆέ΅Δ’ιΖ«
            // ΟvlπΈη·±ΖͺΕ«ιΘη
            if (!nowBreak && !unbreakable)
            {
                durability--;

                // A±Ε_[Wπ^¦ηκΘ’ζ€Ι·ι
                unbreakable = true;

                // ³GΤπέ―ι
                StartCoroutine(ShieldUnbreakable());
            }
        }
        enemyAinim = transform.parent.GetComponent<Animator>();
        if(enemyAinim == null)
        {
            enemyAinim = transform.parent.parent.GetComponent<Animator>();
            if(enemyAinim == null)
            {
                Debug.LogError("Animatorͺ θάΉρ");
            }
        }
    }

    private void Start()
    {
        // eΜξρπζΎ
        par_Obj = transform.parent.gameObject;
        par_Scale = par_Obj.transform.localScale;

        // Μ½θ»θπζΎ
        shieldCol = this.GetComponent<CapsuleCollider>();

        // πLψ»·ι
        shieldCol.enabled = true;
    }

    private void FixedUpdate()
    {
        // ΟvlͺOΘΊΙΘΑ½Θη
        if((durability <= 0) && !nowBreak)
        {
            // ͺjσ³κι
            nowBreak = true;
            enemyAinim.SetBool("shieldBreak", true);
            StartCoroutine(ShieldUnbreakable());

        }//----- if_stop -----
    }

    private IEnumerator ShieldUnbreakable()
    {
        // ͺjσ³κΔ’ιΘη
        if (nowBreak)
        {
            // ½θ»θπΑΕ³Ήι
            shieldCol.enabled = false;

        }//----- if_stop -----

        // έθ³κ½³GΤΜΤ­θ©¦·
        for (int i = 0; i < unbreakableFrame; i++)
        {
            // Pt[x³Ήι
            yield return null;

        }//----- for_stop -----

        // ³GΤπ²―ιΜΕπ
        unbreakable = false;
    }
}