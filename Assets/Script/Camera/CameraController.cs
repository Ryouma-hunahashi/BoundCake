using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // プレイヤーオブジェクト格納
    private GameObject playerObject;

    // カメラオブジェクト格納
    private GameObject cameraObject;

    private Camera camera;


    [Header("プレイヤーのタグ")]
    [SerializeField] private string playerTagName = "Player";
    
    [Header("カメラの表示する縦横")]
    [SerializeField] private int cameraDefaultHight = 9;
    [SerializeField] private int cameraDefaultWidth = 16;
    private float cameraDefaultAspect;  // 設定したいアスペクト比  縦/横で計算

    [Header("カメラが注視するY座標")]
    public float cameraTargetPosY = 3.0f;

    [Header("カメラが次の区画への移動にかかる秒数")]
    [SerializeField] private float cameraMoveSecond = 0.45f;
    // カメラの移動速度。三角関数に放り込むので角速度にしている。
    private float moveAnglarAccelaration;
    private float moveAnglarVelocity = 0;

    
    private enum ACCEL_TYPE
    {
        ACCELERATION,
        DECELERATION,
        P_ACCELERATION,
        P_DECELERATION,
    }
    private enum SLIDE_TYPE
    {
        ONE,
        HALF,
        QUARTER
    }
    [Header("カメラの移動タイプ")]
    [Tooltip("徐々に加速か減速か")]
    [SerializeField] private ACCEL_TYPE accelType = ACCEL_TYPE.DECELERATION;
    [Tooltip("円周期から見た変位")]
    [SerializeField] private SLIDE_TYPE slideType = SLIDE_TYPE.QUARTER;

    private enum FOLLOW_TYPE
    {
        DEFAULT,
        LINER,
        SECTION_FOLLOW
    }
    [Header("カメラの追従タイプ")]
    [SerializeField] private FOLLOW_TYPE followType = FOLLOW_TYPE.DEFAULT;
    [Header("プレイヤーを追いかける最低速度")]
    [SerializeField] private float minFollowSpeed = 3;
    private float maxFollowSpeed = 13;
    private float followSpeed = 5.0f;
    private float nowfollowSpeedIndex;

    [Header("プレイヤーを落ち着ける地点")]
    [Tooltip("左端からの距離")]
    [SerializeField] private float followPosition = 8.0f;
    [Header("プレイヤー追従を開始する距離")]
    [Tooltip("落ち着ける地点からの幅")]
    [SerializeField] private float followLenge = 2.0f;
    [Header("プレイヤーが追従中カメラのどの距離まで行けるか")]
    [SerializeField] private float maxFollowPosition = 20.0f;
    private float anglarIndex;

    

    [Header("現在カメラが注視している区画番号(0スタート)")]
    [SerializeField] private int cameraTargetNumber = 0;

    [Header("カメラを移動させたい区画の数")]
    [SerializeField] private int stageSectionNumber = 3;

    [Header("区画ごとの表示範囲")]
    [SerializeField] private List<float> l_stageLength = new List<float>();

    [Header("デバッグ用。区画が切り替わる毎にカメラが注視するX座標のリスト")]
    [SerializeField] private List<float> l_cameraTargetPosX = new List<float>();

    

    private float cameraDistance = 15;


    private float cameraSize;

    

    // カメラが各区画でプレイヤーを追従する範囲を格納するリスト
    [SerializeField] private List<float> l_cameraMoveLength = new List<float>();

    

    // カメラが次に注視する区画番号
    [SerializeField] private int cameraTargetNextNumber;

    // カメラの移動可能範囲を合計して、
    private float cameraAddDistance = 0;

    // プレイヤーのポジション格納
    Vector3 playerPosition;
    // カメラのポジション格納
    Vector3 cameraPosition;
    
    
    // カメラが表示するX範囲の半分。画面外に出たかの判定、カメラのターゲット間の距離計算に使用
    private float cameraTargetToEdgeDistance;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤータグからプレイヤーを探索
        playerObject = GameObject.FindWithTag(playerTagName);
        if (playerObject == null)
        {
            Debug.LogError("プレイヤーオブジェクトが見つかりません");
        }

        // カメラオブジェクト格納
        cameraObject = this.gameObject;
        if (cameraObject == null)
        {
            Debug.LogError("カメラオブジェクトが見つかりません");
        }
        camera = this.GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("カメラがコンポーネントされていません");
        }

        // カメラの設定サイズを取得
        cameraSize = camera.orthographicSize;

        Debug.Log(cameraSize);

        
        cameraDefaultAspect =(float) cameraDefaultHight / (float)cameraDefaultWidth;
        
        // カメラが表示している範囲を計算
        cameraTargetToEdgeDistance = cameraSize / cameraDefaultAspect;
        Debug.Log(cameraTargetToEdgeDistance);
        // 各ステージ区画の範囲を指定しているかチェックする。
        if (l_stageLength.Count < stageSectionNumber)
        {
            for (int i = l_stageLength.Count; i < stageSectionNumber; i++)
            {
                l_stageLength.Add(cameraTargetToEdgeDistance * 2);
            }
           
        }
        // 区画範囲が表示範囲以下であれば、表示範囲に固定
        for (int i = 0; i < stageSectionNumber; i++)
        {
            if(l_stageLength[i] <cameraTargetToEdgeDistance*2)
            {
                l_stageLength[i] = cameraTargetToEdgeDistance * 2;
            }
        }


        // カメラの区画毎の基準地点を計算
        for (int i = 0; i < stageSectionNumber; i++)
        {
            // カメラがプレイヤーを追従する範囲を計算
            l_cameraMoveLength.Add(l_stageLength[i] - cameraTargetToEdgeDistance * 2);
            if (i == 0)
            {
                // 最初の区画を0に指定
                l_cameraTargetPosX.Add(0);
            }
            else
            {
                // 移動範囲の合計を計算
                cameraAddDistance += l_cameraMoveLength[i - 1];
                // 各区画のカメラ固定位置を計算。ひとつ前までの移動区画を補正値として加算
                l_cameraTargetPosX.Add(i * cameraTargetToEdgeDistance * 2 + cameraAddDistance);

            }

        }

        cameraTargetNextNumber = cameraTargetNumber;
        switch(slideType)
        {
            case SLIDE_TYPE.ONE:
                anglarIndex = 0.5f;
                break;
            case SLIDE_TYPE.HALF:
                anglarIndex = 1;
                break;
            case SLIDE_TYPE.QUARTER:
                anglarIndex = 2;
                break;
        }

        //maxFollowSpeed = playerObject.GetComponent<MovePlayer3_3>().moveSetting.defaultMoveSpeed;
        nowfollowSpeedIndex = maxFollowSpeed / (maxFollowPosition - followPosition);
        

        moveAnglarAccelaration = Mathf.PI / 2 / (cameraMoveSecond*60*anglarIndex);

        // カメラの位置を初期の区画に移動
        cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNumber], cameraTargetPosY, -cameraDistance);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        playerPosition = playerObject.transform.position;
        cameraPosition = cameraObject.transform.position;
        if (cameraTargetNumber == cameraTargetNextNumber)
        {
            followSpeed = Mathf.Abs(((playerPosition.x+cameraTargetToEdgeDistance-followPosition) - camera.transform.position.x)) * nowfollowSpeedIndex;
            if (followSpeed < minFollowSpeed)
            {
                followSpeed = minFollowSpeed;
            }

            switch (followType)
            {
                case FOLLOW_TYPE.DEFAULT:
                    // 各区画のカメラの左端注視点から、右端までの間にプレイヤーを追従するポイントが存在すればカメラに追従させる。
                    if (l_cameraTargetPosX[cameraTargetNumber] < playerPosition.x + cameraTargetToEdgeDistance / 2 &&
                        l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] > playerPosition.x + cameraTargetToEdgeDistance / 2)
                    {
                        cameraObject.transform.position = new Vector3(playerPosition.x + cameraTargetToEdgeDistance / 2, cameraTargetPosY, -cameraDistance);
                    }
                    break;
                case FOLLOW_TYPE.LINER:
                    // 各区画のカメラの左端注視点から、右端までの間にプレイヤーを追従するポイントが存在すればカメラに追従させる。
                    if (l_cameraTargetPosX[cameraTargetNumber]-cameraTargetToEdgeDistance+followPosition+followLenge < playerPosition.x&&
                        l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] - cameraTargetToEdgeDistance + followPosition - followLenge > playerPosition.x )
                    {
                        Debug.Log("カメラ：等速移動。範囲内の時ね");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(playerPosition.x + cameraTargetToEdgeDistance - followPosition, cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    else if (camera.transform.position.x != l_cameraTargetPosX[cameraTargetNumber] && playerPosition.x + cameraTargetToEdgeDistance - followPosition <= l_cameraTargetPosX[cameraTargetNumber])
                    {
                        Debug.Log("カメラ：等速移動。左端なのに到達してない時ね");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNumber], cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    else if (camera.transform.position.x != l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] &&
                            playerPosition.x + cameraTargetToEdgeDistance - followPosition >= l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber])
                    {
                        Debug.Log("カメラ：等速移動。右端なのに到達していない時ね");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber], cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    break;
                case FOLLOW_TYPE.SECTION_FOLLOW:
                    break;
            }

            // カメラの表示限界の右端にプレイヤーが到達したとき
            if (l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] + cameraTargetToEdgeDistance
                < playerPosition.x)
            {
                // 次の区画が設定した区画の限界でなければ右側に設定
                if (cameraTargetNumber + 1 < stageSectionNumber)
                {

                    cameraTargetNextNumber = cameraTargetNumber + 1;

                }

            }
            // 左端に到達したとき
            else if (l_cameraTargetPosX[cameraTargetNumber] - cameraTargetToEdgeDistance
                    > playerPosition.x)
            {
                // 次の区画が0以下でなければ左側に設定
                if (cameraTargetNumber - 1 >= 0)
                {
                    cameraTargetNextNumber = cameraTargetNumber - 1;
                }
            }
        }
        // カメラ注視点番号が更新された時
        // 次の番号が右側であれば
        if (cameraTargetNextNumber > cameraTargetNumber)
        {
            // 現在のカメラの座標が次の注視点でない間カメラを等速直線運動させる
            if (cameraObject.transform.position.x != l_cameraTargetPosX[cameraTargetNextNumber])
            {
                // 角速度を更新
                moveAnglarVelocity -= moveAnglarAccelaration;
                switch(accelType)
                {
                    case ACCEL_TYPE.DECELERATION:
                        // カメラを指定秒数かけて移動させる。
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.ACCELERATION:
                        // カメラを指定秒数かけて移動させる。
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_DECELERATION:
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_ACCELERATION:
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                }
                // カメラを指定秒数かけて移動させる。
                //cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance*2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                //cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNextNumber], cameraTargetPosY, -cameraDistance), cameraMoveSecond * Time.deltaTime);

                // カメラが目標地点を超えた場合、目標地点に固定する
                if (cameraObject.transform.position.x > l_cameraTargetPosX[cameraTargetNextNumber])
                {
                    Debug.Log("カメラ到達しました！");
                    cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNextNumber], cameraTargetPosY, -cameraDistance);
                    // 速度を初期化
                    moveAnglarVelocity = 0;
                    // 現在の番号を更新
                    cameraTargetNumber = cameraTargetNextNumber;
                }
            }
            else
            {
                
            }
        }
        // 次の番号が左端であれば
        else if (cameraTargetNextNumber < cameraTargetNumber)
        {
            // 現在のカメラの座標が (次の注視点 + プレイヤー追従範囲) 出ない間、カメラを等速直線運動させる
            if (cameraObject.transform.position.x != l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber])
            {
                // 角速度を更新
                moveAnglarVelocity -= moveAnglarAccelaration;
                switch (accelType)
                {
                    case ACCEL_TYPE.DECELERATION:
                        // カメラを指定秒数かけて移動させる。
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.ACCELERATION:
                        // カメラを指定秒数かけて移動させる。
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_DECELERATION:
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_ACCELERATION:
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                }

                //cameraObject.transform.position = Vector3.MoveTowards(
                //    cameraObject.transform.position,
                //    new Vector3(l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber],
                //    cameraTargetPosY, -cameraDistance), cameraMoveSecond * Time.deltaTime);
                
                // カメラが目標地点を超えた場合、目標地点に固定する
                if (cameraObject.transform.position.x < l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber]+0.1f)
                {
                    cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber],
                                                                  cameraTargetPosY, -cameraDistance);
                    Debug.Log("カメラ到達しました！");
                    // 速度を初期化
                    moveAnglarVelocity = 0;
                    // 現在の番号を更新
                    cameraTargetNumber = cameraTargetNextNumber;
                }
            }
            else
            {
                
            }
        }

    }

    // ゲーム終了時にリストを破棄する。
    private void OnApplicationQuit()
    {

        l_cameraMoveLength.Clear();
        l_cameraTargetPosX.Clear();
        l_stageLength.Clear();
        
    }
}
