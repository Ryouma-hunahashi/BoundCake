using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Ball_Fall : MonoBehaviour
{
    // é©êgÇÃèÓïÒÇéÊìæ
    private Rigidbody rb;
    private Vector3 pos;
    public Boss2_Main mainBoss;

    private IEnumerator cor_Destroy;

    private LayerMask groundLayer = 1 << 6;
    [SerializeField] private float rayDistance = 0.6f;

    [SerializeField] private byte destroyDelayFrame = 90;
    [SerializeField] private float maxFallSpeed = 18;

    public bool onGround;

    public bool nowFall;
    public bool nowDelay;

    RaycastHit underRayHit;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cor_Destroy = DestroyDelay();
    }

    private void FixedUpdate()
    {
        pos = this.transform.position;

        Ray underRay = new Ray(new Vector3(pos.x, pos.y, pos.z), new Vector3(0.0f, -1.0f, 0.0f));
        

        bool underRayFlag = Physics.Raycast(underRay, out underRayHit, rayDistance, groundLayer);

        onGround = underRayFlag;

        if(!nowFall)
        {
            StartCoroutine(BallFall());
        }
    }

    private IEnumerator BallFall()
    {
        float fallSpeed = 0;
        nowFall = true;

        while(!onGround)
        {
            if (onGround)
            { 
                
                break; 
            }

            // óéâ∫ÇÃÇ›
            rb.velocity = new Vector3(rb.velocity.x, -fallSpeed, 0);

            if (fallSpeed < maxFallSpeed) fallSpeed++;

            StopCoroutine(cor_Destroy);

            // ÇPÉtÉåÅ[ÉÄíxâÑÇ≥ÇπÇÈ
            yield return null;
        }

        nowFall = false;

        if (!nowDelay)
        {

            StartCoroutine(DestroyStart());

        }

        cor_Destroy = DestroyDelay();
        StartCoroutine(cor_Destroy);
    }

    private IEnumerator DestroyDelay()
    {
        nowDelay = true;
        //mainBoss.vfxManager = underRayHit.transform.GetChild(0).GetComponent<vfxManager>();
        //mainBoss.WaveCreate(transform.position.x, underRayHit.transform.position.y);

        for (int i = 0; i < destroyDelayFrame; i++)
        {
            yield return null;

            if (nowFall) yield break;
        }

        nowDelay = false;
        Destroy(this.gameObject);
    }

    private IEnumerator DestroyStart()
    {
        mainBoss.vfxManager = underRayHit.transform.GetChild(0).GetComponent<vfxManager>();
        

        StartCoroutine(WaveWideSpawn());

        mainBoss.audio.BossStompBallSound();
        for (int j = 0; j < destroyDelayFrame * 2; j++)
        {
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private IEnumerator WaveWideSpawn()
    {
        if (underRayHit.transform != null)
        {
            mainBoss.waveAngle = -1;
            mainBoss.WaveCreate(transform.position.x - 0.05f, underRayHit.transform);
            for (byte i = 0; i < 3; i++)
            {
                if(underRayHit.transform==null)
                {
                    yield break;
                }
                yield return null;
            }
            mainBoss.waveAngle = 1;
            mainBoss.WaveCreate(transform.position.x + 0.05f, underRayHit.transform);
        }
    }
}
