using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnchor : MonoBehaviour
{
    
    private Vector3 changeAngleValue;
    // Start is called before the first frame update
    void Start()
    {
        
        changeAngleValue = new Vector3(0,0,-transform.parent.gameObject.GetComponent<Rotation>().angularVelocity);
            
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += changeAngleValue;
    }
}
