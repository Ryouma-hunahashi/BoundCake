using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loccy : MonoBehaviour
{
    private Vector3 defaultScale = Vector3.one;
    private int position = 1;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.lossyScale;
        if(transform.parent != null)
        {
            if (transform.position.x < transform.parent.position.x)
            {
                position = -1;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
                transform.localScale.x / transform.lossyScale.x  * defaultScale.x,
                transform.localScale.y / transform.lossyScale.y * defaultScale.y,
                transform.localScale.z / transform.lossyScale.z * defaultScale.z);
        //if(transform.parent != null)
        //{
        //    this.transform.position = new Vector3(
        //    transform.parent.position.x + position * (transform.parent.localScale.x / 2 + transform.lossyScale.x / 2),
        //    transform.parent.position.y,
        //    transform.parent.position.z);
        //}
        
    }
}
