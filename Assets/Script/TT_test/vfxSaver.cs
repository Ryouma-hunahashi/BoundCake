using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vfxSaver : MonoBehaviour
{
    public vfxManager vfxManager;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
        {
            vfxManager = gameObject.transform.GetChild(0).GetComponent<vfxManager>();
            if (vfxManager == null)
            {
                Debug.LogError("vfxが子オブジェの1番目に存在しないか、vfxManagerがコンポーネントされていません");
            }
        }
        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
