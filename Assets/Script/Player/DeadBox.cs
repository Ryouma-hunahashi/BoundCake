using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBox : MonoBehaviour
{

    private CheckPointManager m_CheckPointManager;  // ‚ ‚³‚í
    //// Start is called before the first frame update
    void Start()
    {
        m_CheckPointManager = new CheckPointManager();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!m_CheckPointManager.GetIsNowRespawning())  // ‚ ‚³‚í
            {

                StartCoroutine(other.GetComponent<Player_Main>().Respawn());

            }
        }
    }
}
