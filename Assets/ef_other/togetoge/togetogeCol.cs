using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider))]
//[RequireComponent(typeof(VisualEffect))]

public class togetogeCol : MonoBehaviour
{
    

    // Start is called before the first frame update
    [SerializeField] VisualEffect effect;
    [SerializeField] Transform togeColliderTrans;
    [SerializeField]float Frequency;
    [SerializeField] float Speed;
    [SerializeField] float SizeLimitMax;
    [SerializeField] float SizeLimitMin;
    [SerializeField] bool ChangeDirectionX;
    [SerializeField]bool ChangeDirectionY;
    [SerializeField] private Vector3 startPos;

    private float ScaleY;
    
    private float PosY;
    private void Reset()
    {

        effect = GetComponentInParent<VisualEffect>();
        
        togeColliderTrans = this.transform;
        Frequency = effect.GetFloat("Frequency");
        Speed = effect.GetFloat("Speed");
        SizeLimitMax = effect.GetFloat("SizeLimitMax");
        SizeLimitMin = effect.GetFloat("SizeLimitMin");
        ChangeDirectionX = effect.GetBool("ChangeDirectionX");
        ChangeDirectionY = effect.GetBool("changeDirectionY");
        startPos = transform.position;
    }
    void Start()
    {
        effect = GetComponentInParent<VisualEffect>();
        togeColliderTrans = this.transform;
        Frequency = effect.GetFloat("Frequency");
        Speed = effect.GetFloat("Speed");
        SizeLimitMax = effect.GetFloat("SizeLimitMax");
        SizeLimitMin = effect.GetFloat("SizeLimitMin");
        ChangeDirectionX = effect.GetBool("ChangeDirectionX");
        ChangeDirectionY = effect.GetBool("changeDirectionY");
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        //ìñÇΩÇËîªíËÇÃçÇÇ≥ÇÃåvéZ
        ScaleY=Mathf.Sin(Time.time * Frequency*Mathf.PI*2)*Speed;

        if(ScaleY<=SizeLimitMin)
        {
            ScaleY = SizeLimitMin;
        }
        else if(ScaleY>=SizeLimitMax)
        {
            ScaleY=SizeLimitMax;
        }

        if(!ChangeDirectionY)
        {
            ScaleY *= -1;
        }
        //ScaleY /= SizeLimitMax;

        PosY = (ScaleY - 1);
        if (ChangeDirectionX)
        {
            togeColliderTrans.localScale = new Vector3(ScaleY, 1, 1);
            togeColliderTrans.position = new Vector3(startPos.x+PosY, startPos.y , startPos.z);
        }
        else
        {
            togeColliderTrans.localScale = new Vector3(1, ScaleY, 1);
            togeColliderTrans.position = new Vector3(startPos.x, startPos.y + PosY, startPos.z);
        }
    }

}
