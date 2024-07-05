using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GoalCamera : MonoBehaviour
{
	[SerializeField]
	[Tooltip("切り替え後のカメラ")]
	private CinemachineVirtualCamera virtualCamera;

	// 切り替え後のカメラの元々のPriorityを保持しておく
	private int defaultPriority;
	public bool BossStart = false;

	public bool test;
	public bool BossEnd;
	[SerializeField] private float weightflame = 2.0f;

	[System.Serializable]
	public enum GoalType
	{
		result1, // ステージ１のリザルト
		result2, // ステージ２のリザルト
		result3, // ステージ３のリザルト
		result4, // ステージ４のリザルト　
		result5, // ステージ５のリザルト
		ending,
	}

	public GoalType resultkinds = GoalType.result1;
	// Start is called before the first frame update
	void Start()
	{
		defaultPriority = virtualCamera.Priority;
		BossEnd = false;
		
	}
    private void Update()
    {
        if(test)
        {
			BossEnd = true;
        }
    }

    /// <summary>
    /// Colliderの範囲に入り続けている間実行され続ける
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
	{
		// 当たった相手に"Player"タグが付いていた場合
		if (other.gameObject.tag == "Player")
		{
			if(!BossStart)
            {
				BossStart = true;
            }
			// 他のvirtualCameraよりも高い優先度にすることで切り替わる
			virtualCamera.Priority = 100;

			if(BossEnd)
            {
				StartCoroutine("BossEndWeight");

				SceneChange();
            }

		}
	}

	/// <summary>
	/// Colliderから出たときに実行される
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerExit(Collider other)
	{
		// 当たった相手に"Player"タグが付いていた場合
		if (other.gameObject.tag == "Player")
		{
			// 元のpriorityに戻す
			virtualCamera.Priority = defaultPriority;
		}
	}

	IEnumerable BossEndWeight()
    {
		for(int i=0;i<weightflame;i++)
        {
			yield return null;
        }
		// 指定時間処理を待つ
		// return new WaitForSeconds(weightflame);
    }

	void SceneChange()
    {
		switch (resultkinds)
		{
			case GoalType.result1:
				SceneManager.LoadScene("result1");
				break;
			case GoalType.result2:
				SceneManager.LoadScene("result2");
				break;
			case GoalType.result3:
				SceneManager.LoadScene("result3");
				break;
			case GoalType.result4:
				SceneManager.LoadScene("result4");
				break;
			case GoalType.result5:
				SceneManager.LoadScene("result5");
				break;
			case GoalType.ending:
				SceneManager.LoadScene("ending");
				break;
		}
	}
}