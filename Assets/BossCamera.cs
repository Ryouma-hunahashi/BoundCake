using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("切り替え後のカメラ")]
    private CinemachineVirtualCamera virtualCamera;

    // 切り替え後のカメラの元々のPriorityを保持しておく
    private int defaultPriority;

    // コルーチンを変数で格納する
    public IEnumerator sleep;

    private bool playercheck;
    // Start is called before the first frame update
    void Start()
    {
        // 元々の優先度を保存しておく
        defaultPriority = virtualCamera.Priority;
        // コルーチンを格納する
        sleep = BossScene();
    }

    // Update is called once per frame
    void Update()
    {
        if(playercheck)
        {
            Debug.Log("きてるよ～");
            Invoke("changebosscamera", 3);

            Invoke("changeplayercamera", 3);
            //StartCoroutine(sleep);

           // virtualCamera.Priority = 200;

            //StartCoroutine(sleep);
            // 元のpriorityに戻す
            virtualCamera.Priority = defaultPriority;

            playercheck = false;
        }
    }

    private void changebosscamera()
    {
        Debug.Log("3秒立ったよ～");
        virtualCamera.Priority = 200;

    }
    private void changeplayercamera()
    {
        // 元のpriorityに戻す
        virtualCamera.Priority = defaultPriority;
    }
    public IEnumerator BossScene()
    {
        Debug.Log("当たってるよー");
        yield return new WaitForSeconds(1);
        Debug.Log("コルーチン通ってるよ～");
        virtualCamera.Priority = 200;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 当たった相手に"Player"タグが付いていた場合
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("当たってるよー");

            playercheck = true;

            //StartCoroutine(sleep);
            //Debug.Log("当たってるよー");
            //// 他のvirtualCameraよりも高い優先度にすることで切り替わる
         

            //StartCoroutine(sleep);
            //// 元のpriorityに戻す
            //virtualCamera.Priority = defaultPriority;
        }
    }
   
}
