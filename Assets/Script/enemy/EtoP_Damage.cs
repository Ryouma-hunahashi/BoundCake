using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtoP_Damage : MonoBehaviour
{
    private DamageAct act;
    [SerializeField] private string playerTagName = "Player";
    // Start is called before the first frame update
    void Start()
    {
        act = GameObject.FindWithTag(playerTagName).gameObject.GetComponent<DamageAct>();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Ray ray = new Ray(transform.position, Vector3.down);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, transform.localScale.y, 1 << 6))
    //    {
    //        var trans = transform.position;
    //        trans.y = 1.1f;
    //        transform.position = trans;
    //        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    }
    //}
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == playerTagName && act.getDamage == false)
        {
            StartCoroutine(TriggerStayDray(other));
        }
    }

    private IEnumerator TriggerStayDray(Collider other)
    {
        yield return null;

        if (act.getDamage == true)
        {
            yield break;
        }
        act.getDamage = true;
        StatusManager.nowHitPoint--;

    }
}
