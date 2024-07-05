using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // プレイヤーオブジェクト格納用
    private GameObject playerObject;

    // カメラのTransform格納用
    private Transform cameraTrans;

    //カメラの位置格納用
    private Vector3 cameraPosition;

    [Header("プレイヤー名")]
    [SerializeField] private string playerName = "Player";

    [Header("カメラの注視点")]
    [SerializeField] private float cameraTargetPosX = 0.0f;
    [SerializeField] private float cameraTargetPosY = 3.0f;

    [Header("カメラとの距離")]
    [SerializeField] private float toPlayerDistance = 5.0f;



    // Start is called before the first frame update
    void Start()
    {
        // playerNameのオブジェクトを見つけてプレイヤーオブジェクトとして格納
        playerObject = GameObject.Find(playerName);
        if(playerObject == null)
        {//----- if_start -----

            Debug.LogError("プレイヤーオブジェクトが見つかりません");
            return;

        }//----- if_stop -----

        // カメラのTransformを格納
        cameraTrans = this.gameObject.transform;
        if(cameraTrans == null)
        {//----- if_start -----

            Debug.LogError("カメラのトランスフォームが見つかりません");
            return;

        }//----- if_stop -----
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    private void LateUpdate()
    {
        // カメラポジションに注視点とプレイヤーからの距離を代入
        cameraPosition = new Vector3(cameraTargetPosX, cameraTargetPosY, toPlayerDistance);

        // カメラの位置を決定
        // プレイヤーオブジェクトの位置からカメラポジション分ずらす。
        cameraTrans.position = playerObject.transform.position + cameraPosition;
    }
}
