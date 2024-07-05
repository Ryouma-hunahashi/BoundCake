using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

//[CustomEditor(typeof(MapEdit))]
//public class MapEditEditor : Editor
//{
//    private bool nowDestroy;

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        if(GUILayout.Button("Reset"))
//        {
//            Debug.Log("�������I");
//        }

//        if (GUILayout.Button("Save"))
//        {
//            Debug.Log("�ۑ����I");
//        }
//    }
//}

//public static class OnPlayState
//{
//    [InitializeOnLoadMethod]
//    static void Initalize()
//    {
//        EditorApplication.playModeStateChanged -= OnChangedPlayMode;

//        EditorApplication.playModeStateChanged += OnChangedPlayMode;
//    }

//    static void OnChangedPlayMode(PlayModeStateChange state)
//    {
//        if(state == PlayModeStateChange.ExitingEditMode)
//        {
//            EditorApplication.SaveScene();
//        }
//    }
//}

//======================================================
//      �}�b�v�G�f�B�^
// ��CSV�t�@�C�����g�p���ă}�b�v���̃I�u�W�F�N�g��z�u���܂�
//======================================================
// �����2023/04/04
// �{��
public class MapEdit : MonoBehaviour
{
    // CSV�t�@�C����ǂݍ���
    [Header("----- CSV�t�@�C�� -----"),Space(5)]
    [SerializeField] private TextAsset csvFile;

    // �ǂݍ���CSV�t�@�C����ێ�
    private List<string[]> csvData = new List<string[]>();

    // �I�u�W�F�N�g�̐������J�n����
    [Header("----- �������J�n -----"),Space(5)]
    [SerializeField] private bool goEdit;

    // ���ݔj�󒆂̏��
    private bool nowDestroy;

    // Prefab�i�[�p�̃��X�g
    [Header("----- ���̂̊i�[ -----"),Space(5)]
    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

    private GameObject putPrefab;

    private void Update()
    {
        // �������J�n���ꂽ�Ȃ�
        if (goEdit)
        {//----- if_start -----

            // �j����J�n����
            nowDestroy = true;

        }//----- if_stop -----
        else
        {//----- else_start -----

            // CSV�p�ێ��f�[�^���폜����
            csvData.Clear();

        }//----- else_stop -----

        // �������̂Ƃ� &
        // �j�󒆂̏�ԂȂ�
        if (goEdit && nowDestroy)
        {//----- if_start -----

            // ���̃V�[�����ɂ������̖��O�̃I�u�W�F�N�g��j�󂷂�
            GameObject objA = GameObject.Find("Coin");
            GameObject objB = GameObject.Find("BreakBlock");
            Destroy(objA);
            Destroy(objB);
            
            // �j�󂵐s�������Ȃ�
            if((objA == null) && (objB == null))
            {//----- if_start -----

                // �j���Ԃ��X�V����
                nowDestroy = false;

            }//----- if_stop -----

        }//----- if_stop -----

        // �������̂Ƃ� &
        // �j�󒆂̏�ԂłȂ��Ȃ�����
        if (goEdit && !nowDestroy)
        {//----- if_start -----

            // Resources�t�H���_����CSV�t�@�C�����擾
            csvFile = Resources.Load("stage1") as TextAsset;

            // CSV�t�@�C�����̃e�L�X�g��ǂݍ���
            StringReader reader = new StringReader(csvFile.text);

            // �������m�F����Ă���Ԃ̂݌J��Ԃ�
            while (reader.Peek() != -1)
            {//----- while_start -----

                // �w�肳�ꂽ�e�L�X�g�t�@�C�����P�s���ǂݍ���
                string line = reader.ReadLine();

                // �Z���̋�؂���m�F ----- ?
                csvData.Add(line.Split(','));

            }//----- while_stop -----

            for (int i = 0; i < csvData.Count; i++)
            {//----- for_start -----

                for (int j = 0; j < csvData[i].Length; j++)
                {//----- for_start -----

                    // CSV�t�@�C�����̈ȉ����W�ԍ������w�肳�ꂽ�ԍ��Ȃ�
                    if (csvData[i][j] == "1")
                    {//----- if_start -----

                        // �w����W��[Prefab]��ݒu����
                        putPrefab = Instantiate(prefabList[0]) as GameObject;
                        putPrefab.transform.position = new Vector3(j, -i, 0);

                    }//----- if_stop -----

                    if (csvData[i][j] == "2")
                    {//----- if_start -----

                        // �w����W��[Prefab]��ݒu����
                        putPrefab = Instantiate(prefabList[1]) as GameObject;
                        putPrefab.transform.position = new Vector3(j, -i, 0);

                    }//----- if_stop -----

                }//----- for_stop -----

            }//----- for_stop -----

            // �������ł͂Ȃ��Ȃ�
            goEdit = false;

        }
    }

}
