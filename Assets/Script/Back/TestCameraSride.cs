using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraSride : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var trans = transform.position;
        trans.x += moveSpeed * Time.deltaTime;
        transform.position = trans;
    }
}
