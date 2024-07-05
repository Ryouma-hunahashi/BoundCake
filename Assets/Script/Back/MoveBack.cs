using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBack : MonoBehaviour
{
    // Transform
    private Transform trans;

    // カメラのTransform
    private Transform cameraTrans;

    // カメラ座標計算用
    private Vector3 oldCameraPos;
    private Vector3 nowCameraPos;
    private float cameraMoveDisX;

    [Header("カメラの移動度に対する追従指数")]
    [SerializeField,Range(0.0f,1.0f)] private float chaseIndex = 1.0f;
    


    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        cameraTrans = GameObject.FindWithTag("MainCamera").transform;
        if(cameraTrans == null)
        {
            Debug.LogError("カメラが見つかりません");
        }
        oldCameraPos = cameraTrans.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 現在のカメラの位置を保存
        nowCameraPos = cameraTrans.position;

        // 1フレーム間でのカメラの移動値を計算
        cameraMoveDisX = nowCameraPos.x - oldCameraPos.x;

        // ポジションを移動値に指数を掛けた文だけ移動
        var pos = trans.position;
        pos.x += (cameraMoveDisX*chaseIndex);
        trans.position = pos;

        // 1フレーム前の位置として保存
        oldCameraPos = nowCameraPos;
    }
}
