using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GoalCamera : MonoBehaviour
{
	[SerializeField]
	[Tooltip("�؂�ւ���̃J����")]
	private CinemachineVirtualCamera virtualCamera;

	// �؂�ւ���̃J�����̌��X��Priority��ێ����Ă���
	private int defaultPriority;
	public bool BossStart = false;

	public bool test;
	public bool BossEnd;
	[SerializeField] private float weightflame = 2.0f;

	[System.Serializable]
	public enum GoalType
	{
		result1, // �X�e�[�W�P�̃��U���g
		result2, // �X�e�[�W�Q�̃��U���g
		result3, // �X�e�[�W�R�̃��U���g
		result4, // �X�e�[�W�S�̃��U���g�@
		result5, // �X�e�[�W�T�̃��U���g
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
    /// Collider�͈̔͂ɓ��葱���Ă���Ԏ��s���ꑱ����
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
	{
		// �������������"Player"�^�O���t���Ă����ꍇ
		if (other.gameObject.tag == "Player")
		{
			if(!BossStart)
            {
				BossStart = true;
            }
			// ����virtualCamera���������D��x�ɂ��邱�ƂŐ؂�ւ��
			virtualCamera.Priority = 100;

			if(BossEnd)
            {
				StartCoroutine("BossEndWeight");

				SceneChange();
            }

		}
	}

	/// <summary>
	/// Collider����o���Ƃ��Ɏ��s�����
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerExit(Collider other)
	{
		// �������������"Player"�^�O���t���Ă����ꍇ
		if (other.gameObject.tag == "Player")
		{
			// ����priority�ɖ߂�
			virtualCamera.Priority = defaultPriority;
		}
	}

	IEnumerable BossEndWeight()
    {
		for(int i=0;i<weightflame;i++)
        {
			yield return null;
        }
		// �w�莞�ԏ�����҂�
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