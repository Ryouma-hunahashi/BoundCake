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
//            Debug.Log("消去っ！");
//        }

//        if (GUILayout.Button("Save"))
//        {
//            Debug.Log("保存っ！");
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
//      マップエディタ
// ※CSVファイルを使用してマップ内のオブジェクトを配置します
//======================================================
// 制作日2023/04/04
// 宮﨑
public class MapEdit : MonoBehaviour
{
    // CSVファイルを読み込む
    [Header("----- CSVファイル -----"),Space(5)]
    [SerializeField] private TextAsset csvFile;

    // 読み込んだCSVファイルを保持
    private List<string[]> csvData = new List<string[]>();

    // オブジェクトの生成を開始する
    [Header("----- 生成を開始 -----"),Space(5)]
    [SerializeField] private bool goEdit;

    // 現在破壊中の状態
    private bool nowDestroy;

    // Prefab格納用のリスト
    [Header("----- 物体の格納 -----"),Space(5)]
    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

    private GameObject putPrefab;

    private void Update()
    {
        // 生成が開始されたなら
        if (goEdit)
        {//----- if_start -----

            // 破壊を開始する
            nowDestroy = true;

        }//----- if_stop -----
        else
        {//----- else_start -----

            // CSV用保持データを削除する
            csvData.Clear();

        }//----- else_stop -----

        // 生成中のとき &
        // 破壊中の状態なら
        if (goEdit && nowDestroy)
        {//----- if_start -----

            // このシーン内にある特定の名前のオブジェクトを破壊する
            GameObject objA = GameObject.Find("Coin");
            GameObject objB = GameObject.Find("BreakBlock");
            Destroy(objA);
            Destroy(objB);
            
            // 破壊し尽くしたなら
            if((objA == null) && (objB == null))
            {//----- if_start -----

                // 破壊状態を更新する
                nowDestroy = false;

            }//----- if_stop -----

        }//----- if_stop -----

        // 生成中のとき &
        // 破壊中の状態でなくなったら
        if (goEdit && !nowDestroy)
        {//----- if_start -----

            // Resourcesフォルダ内のCSVファイルを取得
            csvFile = Resources.Load("stage1") as TextAsset;

            // CSVファイル内のテキストを読み込む
            StringReader reader = new StringReader(csvFile.text);

            // 文字が確認されている間のみ繰り返す
            while (reader.Peek() != -1)
            {//----- while_start -----

                // 指定されたテキストファイルを１行ずつ読み込む
                string line = reader.ReadLine();

                // セルの区切りを確認 ----- ?
                csvData.Add(line.Split(','));

            }//----- while_stop -----

            for (int i = 0; i < csvData.Count; i++)
            {//----- for_start -----

                for (int j = 0; j < csvData[i].Length; j++)
                {//----- for_start -----

                    // CSVファイル内の以下座標番号内が指定された番号なら
                    if (csvData[i][j] == "1")
                    {//----- if_start -----

                        // 指定座標に[Prefab]を設置する
                        putPrefab = Instantiate(prefabList[0]) as GameObject;
                        putPrefab.transform.position = new Vector3(j, -i, 0);

                    }//----- if_stop -----

                    if (csvData[i][j] == "2")
                    {//----- if_start -----

                        // 指定座標に[Prefab]を設置する
                        putPrefab = Instantiate(prefabList[1]) as GameObject;
                        putPrefab.transform.position = new Vector3(j, -i, 0);

                    }//----- if_stop -----

                }//----- for_stop -----

            }//----- for_stop -----

            // 生成中ではなくなる
            goEdit = false;

        }
    }

}
