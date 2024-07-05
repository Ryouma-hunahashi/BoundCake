using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_A_HomingShot : MonoBehaviour
{
    // ©g‚Ìî•ñ‚ğæ“¾
    [SerializeField] private Rigidbody me_Rb;
    [SerializeField] private GameObject target;

    [SerializeField] private float speed;

    private Vector3 pl_Pos;
    private Vector3 me_Pos;

    private void Start()
    {
        me_Rb = GetComponent<Rigidbody>();
        pl_Pos = target.transform.position;
    }

    private void Update()
    {
        me_Pos = this.transform.position;
    }

    private void FixedUpdate()
    {
        transform.LookAt(pl_Pos);

        me_Rb.velocity = new Vector3(transform.forward.x * speed, transform.forward.y * speed, transform.forward.z);
    }
}
