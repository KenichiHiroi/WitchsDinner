using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    //�Q�[���I�u�W�F�N�g
    [SerializeField] GameObject GoodRatingPicture;
    [SerializeField] GameObject NomalRatingPicture;
    [SerializeField] GameObject RatingText;
    [SerializeField] GameObject InfomationText;

    //�I�[�f�B�I�\�[�X
    AudioSource audioSource;

    //���ʉ�
    public AudioClip GoodRatingSE;
    public AudioClip NormalRatingSE;

    //���ԊǗ�
    float LoadTime;     //�V�[���ǂݍ��ݎ���

    // Start is called before the first frame update
    void Start()
    {
        //�V�[���ǂݍ��ݎ��Ԃ�ݒ�
        LoadTime = Time.time;

        //�摜�E�e�L�X�g�̏�����
        GoodRatingPicture.SetActive(false);
        NomalRatingPicture.SetActive(false);
        RatingText.GetComponent<Text>().text = "�@";

        //�I�[�f�B�I�\�[�X��ݒ�
        audioSource = GetComponent<AudioSource>();

        //���v���_�ɉ����āA�摜�E�e�L�X�g��\������
        if(GameManager.TotalScore >= 80)
        {
            GoodRatingPicture.SetActive(true);
            RatingText.GetComponent<Text>().text = "�喞���̔��������J���[�ɂȂ�܂����I";
            audioSource.PlayOneShot(GoodRatingSE);
        }
        else
        {
            NomalRatingPicture.SetActive(true);
            RatingText.GetComponent<Text>().text = "�f�ނ̖��������ĂāA������܂��ꋻ�c�c�����c�c�I";
            audioSource.PlayOneShot(NormalRatingSE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�V�[���ǂݍ��݂�3�b��Ƀ^�C�g����ʂւ̈ē��e�L�X�g��\������
        if(InfomationText.activeSelf == false
            && (Time.time - LoadTime) > 3)
        {
            InfomationText.SetActive(true);
        }

        //�ē��e�L�X�g�̕\����A���N���b�N�Ń^�C�g���ɖ߂�
        if (InfomationText.activeSelf
            && Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
