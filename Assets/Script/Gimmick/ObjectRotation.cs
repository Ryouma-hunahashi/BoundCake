using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    enum Rotate_Shaft
    {
        X,
        Y,
        Z,
    }

    [Header("回転軸の設定")]
    [SerializeField] private Rotate_Shaft m_Rotate = Rotate_Shaft.Z;
    [Header("回転の中心")]
    [SerializeField] private Vector3 shaftPos = Vector3.zero;
    private Vector3 worldShaftPos;  // 中心のワールド座標
   
    [Header("回転角")]
    [SerializeField] private float rotateAngle = 180;
    [Header("回転速度")]
    [SerializeField] private float rotationSpeed = 3;
    private float defaultAngle = 0; // 回転開始時の角度
    private float nowRotateAngle = 0;   // 現在の回転角
    [SerializeField] private bool nowRotate = false;
    private bool setUpFg = false;
    private Quaternion quaterVelocity;
    private Vector3 positionLog;

    // Start is called before the first frame update
    void Start()
    {
        worldShaftPos = transform.position+shaftPos;
        positionLog = transform.position;
        if(transform.parent != null)
        {
            //worldShaftPos += transform.parent.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position!=positionLog)
        {
            worldShaftPos=transform.position+shaftPos;
            positionLog=transform.position;
        }
        if(nowRotate)
        {
            if(!setUpFg)
            {
                switch(m_Rotate)
                {
                    case Rotate_Shaft.X:
                        defaultAngle = transform.eulerAngles.x;
                        break;
                    case Rotate_Shaft.Y:
                        defaultAngle = transform.eulerAngles.y;
                        break;
                    case Rotate_Shaft.Z:
                        defaultAngle = transform.eulerAngles.z;
                        break;
                }

                
                
                setUpFg = true;
            }
            
            switch (m_Rotate)
            {
                case Rotate_Shaft.X:
                    quaterVelocity = Quaternion.AngleAxis(rotationSpeed, Vector3.right);
                    
                    
                    break;
                case Rotate_Shaft.Y:
                    quaterVelocity = Quaternion.AngleAxis(rotationSpeed, Vector3.down);
                    
                    break;
                case Rotate_Shaft.Z:
                    quaterVelocity = Quaternion.AngleAxis(rotationSpeed, Vector3.forward);
                    
                    break;
            }
            transform.position -= worldShaftPos;
            transform.position = quaterVelocity * transform.position;
            transform.position += worldShaftPos;
            transform.rotation *= quaterVelocity;
            if(transform.childCount>0)
            {
                for(byte i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position -= transform.position;
                    transform.GetChild(i).position = Quaternion.Inverse(quaterVelocity)*transform.GetChild(i).position;
                    transform.GetChild(i).position += transform.position;
                    transform.GetChild(i).rotation *= Quaternion.Inverse(quaterVelocity);
                }
            }
            nowRotateAngle += rotationSpeed;
            if (rotationSpeed > 0)
            {
                if (defaultAngle + nowRotateAngle >= defaultAngle + rotateAngle)
                {
                    switch (m_Rotate)
                    {
                        case Rotate_Shaft.X:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.right);


                            break;
                        case Rotate_Shaft.Y:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.down);

                            break;
                        case Rotate_Shaft.Z:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.forward);

                            break;
                    }
                    transform.position -= worldShaftPos;
                    transform.position = quaterVelocity * transform.position;
                    transform.position += worldShaftPos;
                    transform.rotation *= quaterVelocity;

                    nowRotateAngle = 0;
                    setUpFg = false;
                    nowRotate = false;
                }
            }
            else
            {
                if (defaultAngle + nowRotateAngle <= defaultAngle + rotateAngle)
                {
                    switch (m_Rotate)
                    {
                        case Rotate_Shaft.X:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.right);


                            break;
                        case Rotate_Shaft.Y:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.down);

                            break;
                        case Rotate_Shaft.Z:
                            quaterVelocity = Quaternion.AngleAxis(rotateAngle - nowRotateAngle, Vector3.forward);

                            break;
                    }
                    transform.position -= worldShaftPos;
                    transform.position = quaterVelocity * transform.position;
                    transform.position += worldShaftPos;
                    transform.rotation *= quaterVelocity;

                    nowRotateAngle = 0;
                    setUpFg = false;
                    nowRotate = false;
                }
            }
        }
    }

    //========================================
    // 回転を開始する。
    // 戻り値無し
    // 第一引数：回転角度(オイラー角)
    // 第二引数：回転速度(フレーム)
    //========================================
    public void RotateStart(float _rotateAngle,float _rotateSpeed)
    {
        //if(!nowRotate)
        {
            if(_rotateSpeed<=0)
            {
                _rotateSpeed = 60;
            }
            rotateAngle = _rotateAngle;
            rotationSpeed = (rotateAngle/_rotateSpeed);

            nowRotateAngle = 0;
            setUpFg = false;
            nowRotate = true;
        }
    }
}
