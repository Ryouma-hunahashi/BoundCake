using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testMSelect : MonoBehaviour
{

    private Image canvasRenderer;
    private int StartSortNum;
    private Material mat;
    //�����F�i��I�����j
    Color color_1;
    //�F�t���i�I�����j
    Color color_2;
    // Start is called before the first frame update
    void Start()
    {
        canvasRenderer = GetComponent<Image>();
        if(canvasRenderer == null)
        {
            Debug.LogError("CanvasRenderer���˂�");
        }
        StartSortNum = this.transform.GetSiblingIndex();
        mat = canvasRenderer.material;
        if(mat == null)
        {
            Debug.LogError("Material�Ȃ�Ă˂���");
        }
        color_1 = mat.color;
        color_2 = new Color(color_1.r, color_1.g, color_1.b, 1.0f);
        color_1 = new Color(color_1.r, color_1.g, color_1.b, 0.0f);
        mat.SetColor("_Color", color_1);
        mat.SetFloat("_UVScale", 100.0f);
    }

    public void Enter()
    {
        //���̉e�����󂯂�悤�ɂ���
        mat.SetFloat("_UVScale", 1.0f);
        mat.SetColor("_Color", color_2);
        //�`�揇���őO��Ɏ����Ă���
        transform.SetAsLastSibling();
    }

    public void Exit()
    {
        //���̉e����0�ɂ���
        mat.SetFloat("_UVScale", 100.0f);
        mat.SetColor("_Color", color_1);
        //�`�揇���J�n���Ɠ����ʒu�ɖ߂�
        transform.SetSiblingIndex(StartSortNum);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
