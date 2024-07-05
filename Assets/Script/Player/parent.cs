using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parent : MonoBehaviour
{

   

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Floor")
            transform.SetParent(other.transform.parent/*parentObj.transform*/);
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Floor")
            transform.SetParent(null);
    }
}
