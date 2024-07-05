using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    //�V�K�֐��E�X�N���v�g
    //======================================
    //�쐬�ҁF����
    //�쐬���F05/20
    //======================================

    // Start is called before the first frame update
    //UI��audioSource��ێ�����ϐ�
    public AudioSource UISource;

    //UI���쎞��SE��ϐ��Ɋi�[���Ă���
    [Header("�J�[�\���ړ��̃T�E���h")]
    [SerializeField] private AudioClip UIMoveCursorSound;
    [Header("����̃T�E���h")]
    [SerializeField] private AudioClip UIDecisionSound;
    [Header("�L�����Z���̃T�E���h")]
    [SerializeField] private AudioClip UICancelSound;
    [Header("�|�[�Y��ʂ̃T�E���h")]
    [SerializeField] private AudioClip UIPauseSound;
    [Header("�Q�[���J�n���̃T�E���h")]
    [SerializeField] private AudioClip UIStartSound;
    [Header("�V�[���؂�ւ��̃T�E���h")]
    [SerializeField] private AudioClip UIChangeSceneSound;
    [Header("���ɕ������̃T�E���h")]
    [SerializeField] private AudioClip UIWindSound;
    [Header("������̃T�E���h")]
    [SerializeField] private AudioClip UIBookMarkSound;
    [Header("�n�}���͂܂鉹")]
    [SerializeField] private AudioClip UIMapSetSound;

    //SE�̉��ʂ𒲐�����p�̕ϐ������
    [Header("�J�[�\���ړ����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIMoveCursorVolume;
    [Header("���莞�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIDecisionVolume;
    [Header("�L�����Z�����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UICancelVolume;
    [Header("�|�[�Y��ʂ̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIPauseVolume;
    [Header("�Q�[���J�n���̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIStartVolume;
    [Header("�V�[���؂�ւ��̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIChangeSceneVolume;
    [Header("���ɕ������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIWindVolume;
    [Header("������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float UIBookMarkVolume;
    [Header("�n�}���͂܂鉹")]
    [SerializeField, Range(0f, 1f)] float UIMapSetVolume;
    void Start()
    {
        //UI��AudioSource�R���|�[�l���g���擾����
        UISource = GetComponent<AudioSource>();
    }

    public void MoveCursorSound()
    {
        UISource.volume = UIMoveCursorVolume;
        UISource.PlayOneShot(UIMoveCursorSound);
    }
    public void DecisionSound()
    {
        UISource.volume = UIDecisionVolume;
        UISource.PlayOneShot(UIDecisionSound);
    }
    public void CancelSound()
    {
        UISource.volume = UICancelVolume;
        UISource.PlayOneShot(UICancelSound);
    }
    public void PauseSound()
    {
        UISource.volume = UIPauseVolume;
        UISource.PlayOneShot(UIPauseSound);
    }
    public void StartSound()
    {
        UISource.volume = UIStartVolume;
        UISource.PlayOneShot(UIStartSound);
    }
    public void ChangeSceneSound()
    {
        UISource.volume = UIChangeSceneVolume;
        UISource.PlayOneShot(UIChangeSceneSound);
    }
    public void WindSound()
    {
        UISource.volume = UIWindVolume;
        UISource.PlayOneShot(UIWindSound);
    }
    public void BookMarkSound()
    {
        UISource.volume = UIBookMarkVolume;
        UISource.PlayOneShot(UIBookMarkSound);
    }
    public void MapSetSound()
    {
        UISource.volume = UIMapSetVolume;
        UISource.PlayOneShot(UIMapSetSound);
    }




    // Update is called once per frame
    //void Update()
    //{

    //}
}
