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

    [Header("‰ñ“]Ž²‚ÌÝ’è")]
    [SerializeField] private Rotate_Shaft m_Rotate = Rotate_Shaft.Z;
    [Header("‰ñ“]‚Ì’†S")]
    [SerializeField] private Vector3 shaftPos = Vector3.zero;
    private Vector3 worldShaftPos;  // ’†S‚Ìƒ[ƒ‹ƒhÀ•W
   
    [Header("‰ñ“]Šp")]
    [SerializeField] private float rotateAngle = 180;
    [Header("‰ñ“]‘¬“x")]
    [SerializeField] private float rotationSpeed = 3;
    private float defaultAngle = 0; // ‰ñ“]ŠJŽnŽž‚ÌŠp“x
    private float nowRotateAngle = 0;   // Œ»Ý‚Ì‰ñ“]Šp
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
    // ‰ñ“]‚ðŠJŽn‚·‚éB
    // –ß‚è’l–³‚µ
    // ‘æˆêˆø”F‰ñ“]Šp“x(ƒIƒCƒ‰[Šp)
    // ‘æ“ñˆø”F‰ñ“]‘¬“x(ƒtƒŒ[ƒ€)
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
