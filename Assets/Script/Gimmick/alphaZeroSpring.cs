using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class alphaZeroSpring : MonoBehaviour
{
    private VisualEffect springEffect;
    private float alphaTime;


    [SerializeField] private VariousSwitches_2 alphaSwitch;

    private bool oldSwitchStatus;
    private float elapsedTime = 0;
    private bool lookFg = false;

    // Start is called before the first frame update
    void Start()
    {
        springEffect = transform.GetChild(0).GetComponent<VisualEffect>();
        alphaTime = springEffect.GetFloat("AlphaTime");
        springEffect.SetBool("AlphaFg",true);
        gameObject.SetActive(false);
        if(alphaSwitch == null)
        {
            Debug.LogError("スイッチを設定してください");
        }

        oldSwitchStatus = alphaSwitch.switchStatus;
    }

    // Update is called once per frame
    void Update()
    {
        if (oldSwitchStatus != alphaSwitch.switchStatus)
        {
            springEffect.SendEvent("Start");
            oldSwitchStatus = alphaSwitch.switchStatus;
            elapsedTime = 0.0f;
            if(!lookFg)
            {
                lookFg = true;
                gameObject.SetActive(true);
            }
            
        }
        if(lookFg)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= alphaTime)
            {
                gameObject.SetActive(false);
                lookFg = false;
            }
        }
    }
}
