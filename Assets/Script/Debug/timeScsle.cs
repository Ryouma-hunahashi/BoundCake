using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeScsle : MonoBehaviour
{
    [SerializeField] float timeScale = 1;
    [SerializeField] bool enterFlg = false;
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void FixedUpdate()
    {
        if(enterFlg)
        {
            enterFlg = false;
            Time.timeScale = timeScale;
        }
    }
}
