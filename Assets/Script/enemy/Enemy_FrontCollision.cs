using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FrontCollision : MonoBehaviour
{
    private Vector3 par_Pos;
    private Enemy_KB par_KB;
    

    private void Start()
    {
        par_Pos = this.transform.parent.transform.position;
        par_Pos.z += 1.5f;
        this.transform.parent.transform.position = par_Pos;

        par_KB = transform.parent.GetComponent<Enemy_KB>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (par_KB != null)
        {
            if (other.transform.CompareTag("Wave") && !par_KB.hitPlayerWave)
            {
                var waveScript = other.gameObject.GetComponent<waveCollition>();
                par_KB.SetHitWave(waveScript);
                if (!waveScript.CheckType(waveCollition.WAVE_TYPE.ENEMY) &&
                    (waveScript.CheckVelocity(waveCollition.WAVE_VELOCITY.RIGHT) ||
                    waveScript.CheckVelocity( waveCollition.WAVE_VELOCITY.LEFT)))
                {
                    Debug.Log("ノックバック発生");
                    par_KB.hitPlayerWave = true;
                    par_KB.hitWaveSpeed = waveScript.vfxManager.waveSpeedArray[waveScript.GetVFXNum()];
                    par_KB.hitWaveHight = waveScript.vfxManager.waveHeightArray[waveScript.GetVFXNum()];
                    switch (waveScript.GetVelocity())
                    {
                        case waveCollition.WAVE_VELOCITY.RIGHT:
                            par_KB.knockBackVelocity = 1;
                            break;
                        case waveCollition.WAVE_VELOCITY.LEFT:
                            par_KB.knockBackVelocity = -1;
                            break;
                    }


                    // 速度の判定
                    if (par_KB.hitWaveSpeed >= par_KB.judgeSpeed)
                    {
                        par_KB.speedLevel = 1;

                    }//----- if_stop -----
                    else
                    {
                        par_KB.speedLevel = 0;

                    }//----- else_stop -----

                    // 高さの判定
                    if (WaveManager.instance.CheckStrongWave(par_KB.hitWaveHight))
                    {
                        par_KB.hightLevel = 1;

                    }//----- if_stop -----
                    else
                    {
                        par_KB.hightLevel = 0;

                    }//----- else_stop -----

                    par_KB.fallPow = par_KB.blowOfHight[par_KB.hightLevel];

                    par_KB.waveTest = false;
                }
            }
        }
        //if(other.CompareTag("VGround")&&par_KB.hitPlayerWave)
        //{

        //}
    }
}
