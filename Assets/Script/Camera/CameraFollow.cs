using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform cameraTrans;
    private string cameraTag = "MainCamera";
    private float followPointY;

    // Start is called before the first frame update
    void Start()
    {
        cameraTrans = GameObject.FindWithTag(cameraTag).transform;
        followPointY = transform.position.y-cameraTrans.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        pos.y = cameraTrans.position.y+followPointY;
        transform.position = pos;
    }
}
