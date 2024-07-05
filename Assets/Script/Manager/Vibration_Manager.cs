using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//==================================================
//          �R���g���[���[�U���̃}�l�[�W���[
// ���ʃX�N���v�g�̏���������������ɐU�������܂�
//==================================================
// �����2023/05/22    �X�V��2023/05/26
// �{��
public class Vibration_Manager : MonoBehaviour
{
    // �V���O���g���̍쐬
    public static Vibration_Manager instanse;

    private void Awake()
    {
        // ���g�����݂��Ă��Ȃ��Ȃ�
        if (instanse == null)
        {
            // ���g���C���X�^���X��
            instanse = this;

            // �V�[���ύX���ɔj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {
            // ���łɎ��g�����݂��Ă���Ȃ�j��
            Destroy(this.gameObject);

        }//----- else_stop -----
    }

    // �e�R���g���[���[�U���̐ݒ�p�\����
    [System.Serializable]
    private struct BothVibrationValue
    {
        public string vibName;    // �U���̖��O

        // �U���p���̐ݒ�
        [Header("----- �p���̐ݒ� -----")]
        public int loopFrame;       // �U����������

        // �R���g���[���[�U���̐ݒ�
        [Header("----- �U���̐ݒ� -----")]
        
        [Range(0f, 1f)] public float leftValue;     // �����̐U��
        [Range(0f, 1f)] public float rightValue;    // �E���̐U��
    }

    // �U���̏��
    public List<bool> nowVibration;
    public bool stopVibration;      // �U���������I�Ɏ~�߂�

    // �e�R���g���[���[�U���̐ݒ�
    [SerializeField] private List<BothVibrationValue> vibValues = new List<BothVibrationValue>();

    // �ȉ��ҏW����� -----
    [SerializeField] private bool test;
    [SerializeField] private string testName;

    private void Start()
    {
        // �R���g���[���[�U���̐ݒ肪���镪�J��Ԃ�
        for(int i = 0; i < vibValues.Count; i++)
        {
            // �R���g���[���[�U���󋵂���ޕʂŎ擾
            nowVibration.Add(false);

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        if(stopVibration)
        {
            for(int i = 0; i < nowVibration.Count; i++)
            {
                nowVibration[i] = false;
            }

            // �U�����I��������
            Gamepad.current.SetMotorSpeeds(0, 0);
            stopVibration = false;
        }
    }

    //==================================================
    //          �R���g���[���[�U���ݒ�̑I��
    // ���ǂ̃R�����g�[���[�U���ݒ���g�p���邩��
    // �@�����Ă����������g�p���Ē��ׂ܂�
    // �߂�l �F�Ȃ�
    //�@�����@�F(_VibName)�ݒ�̖��O
    //==================================================
    // �����2023/05/22
    // �{��
    public void VibrationSelect(string _VibName)
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ��Ȃ珈���𔲂���
        if (Gamepad.current == null) return;

        // �ݒ肳��Ă���R���g���[���[�U���̐��J��Ԃ�
        for(int i = 0; i < vibValues.Count; i++)
        {
            // �w�肳�ꂽ�ݒ肪���s���Ȃ�X�L�b�v
            if (nowVibration[i]) continue;

            // ����ꂽ���O����v���Ă���Ȃ�
            if (vibValues[i].vibName == _VibName)
            {
                // �R���g���[���[�U�����X�^�[�g������
                nowVibration[i] = true;
                StartCoroutine(VibrationUpdate(i, vibValues[i].leftValue, vibValues[i].rightValue, vibValues[i].loopFrame));

            }//----- if_stop -----

        }//----- for_stop -----
    }

    //==================================================
    //          �R���g���[���[�U���̎��s
    // ���I�����ꂽ�ݒ���g�p���ăR���g���[���[��U��������
    // �߂�l �F�Ȃ�
    //�@�����@�F(_num)�ݒ�ԍ�,
    //�@        (_left)���U������, (_right)�E�U������
    //          (_count)�U�����p��������t���[��
    //==================================================
    // �����2023/05/22
    // �{��
    public IEnumerator VibrationUpdate(int _num, float _left, float _right, int _count)
    {
        // �R���g���[���[���w��̋����ŐU��������
        Gamepad.current.SetMotorSpeeds(_left, _right);

        for(int i = 0; i < _count; i++)
        {
            // �r���ŐU�����~�߂�ꂽ�Ȃ珈���𔲂���
            if (!nowVibration[_num]) break;

            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // ���݂̐U�����I��������
        nowVibration[_num] = false;

        // �e�ݒ�ŐU�����Ă��Ȃ�
        bool allFalse = true;

        // �e�ݒ�̐U����Ԃ𒲂ׂ�
        for (int i = 0; i < nowVibration.Count; i++)
        {
            // �P�ł��U�����Ă����Ȃ�
            if(nowVibration[i])
            {
                // �Ȃɂ��͐U�����Ă���
                allFalse = false;

            }//----- if_stop -----

        }//----- for_stop -----

        // �e�ݒ�ŐU�����Ă��Ȃ���ԂȂ�
        if(allFalse)
        {
            // �U�����I��������
            Gamepad.current.SetMotorSpeeds(0, 0);

        }//----- if_stop -----
    }

    private void OnApplicationQuit()
    {
        // �U�����I��������
        Gamepad.current.SetMotorSpeeds(0, 0);

        // �U�����̏����J��
        nowVibration.Clear();
        vibValues.Clear();
    }
}