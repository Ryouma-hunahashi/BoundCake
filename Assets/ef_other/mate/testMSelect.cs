using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testMSelect : MonoBehaviour
{

    private Image canvasRenderer;
    private int StartSortNum;
    private Material mat;
    //透明色（非選択時）
    Color color_1;
    //色付き（選択時）
    Color color_2;
    // Start is called before the first frame update
    void Start()
    {
        canvasRenderer = GetComponent<Image>();
        if(canvasRenderer == null)
        {
            Debug.LogError("CanvasRendererがねえ");
        }
        StartSortNum = this.transform.GetSiblingIndex();
        mat = canvasRenderer.material;
        if(mat == null)
        {
            Debug.LogError("Materialなんてねえぞ");
        }
        color_1 = mat.color;
        color_2 = new Color(color_1.r, color_1.g, color_1.b, 1.0f);
        color_1 = new Color(color_1.r, color_1.g, color_1.b, 0.0f);
        mat.SetColor("_Color", color_1);
        mat.SetFloat("_UVScale", 100.0f);
    }

    public void Enter()
    {
        //光の影響を受けるようにする
        mat.SetFloat("_UVScale", 1.0f);
        mat.SetColor("_Color", color_2);
        //描画順を最前列に持ってくる
        transform.SetAsLastSibling();
    }

    public void Exit()
    {
        //光の影響を0にする
        mat.SetFloat("_UVScale", 100.0f);
        mat.SetColor("_Color", color_1);
        //描画順を開始時と同じ位置に戻す
        transform.SetSiblingIndex(StartSortNum);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
