using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_StageCreate : MonoBehaviour
{
    enum PhaseLevel
    {
        Nomal,
        Last,
    }
    [SerializeField] private GameObject switchObj;
    private VariousSwitches_2 switchScript;
    private List<GameObject> l_childrenObj = new List<GameObject>();
    private bool stageSetFg = false;

    [SerializeField] private PhaseLevel level = PhaseLevel.Nomal;

    // Start is called before the first frame update
    void Start()
    {
        switchScript = switchObj.GetComponent<VariousSwitches_2>();
        switch (level)
        {
            case PhaseLevel.Nomal:
                for (byte i = 0; i < transform.childCount; i++)
                {
                    l_childrenObj.Add(transform.GetChild(i).gameObject);
                    l_childrenObj[i].SetActive(true);
                }
                break;
            case PhaseLevel.Last:
                for (byte i = 0; i < transform.childCount; i++)
                {
                    l_childrenObj.Add(transform.GetChild(i).gameObject);
                    l_childrenObj[i].SetActive(false);
                }
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!stageSetFg)
        {
            if (!switchScript.switchStatus)
            {
                StartCoroutine(stageCreate());
                stageSetFg = true;
            }
        }
        
    }

    private IEnumerator stageCreate()
    {
        for(byte i =0;i<30;i++)
        {
            yield return null;
        }
        for(byte i=0;i<l_childrenObj.Count;i++)
        {
            switch(level)
            {
                case PhaseLevel.Nomal:
                    l_childrenObj[i].SetActive(false);
                    break;
                case PhaseLevel.Last:
                    l_childrenObj[i].SetActive(true);
                    break;
            }
            
        }
        
    }
    private void OnApplicationQuit()
    {
        l_childrenObj.Clear();
    }
}
