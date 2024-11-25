using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�y�Ȃ̍Đ�����
    bool isPlaying;

    //���Ԋ֘A
    float LoadTime;                 //�V�[���ǂݍ��ݎ��̎���
    float PlayTime;                 //�y�ȍĐ����̎���

    float TimeElapsed;              //�Q�[���J�n����̌o�ߎ���
    float TimeElapsed_Load;         //�V�[���ǂݍ��݂���̌o�ߎ���
    float TimeElapsed_PlayMusic;    //�y�ȊJ�n����̌o�ߎ���

    //���ʁE�y�ȃt�@�C���̓ǂݍ��ݗp
    [SerializeField] string ChartPath;  //���ʃt�@�C���̃p�X
    [SerializeField] string MusicPath;  //�y�ȃt�@�C���̃p�X

    string Title;           //�y�ȃ^�C�g��
    int BPM;                //BPM
    List<GameObject> Notes; //�m�[�c�p���X�g
    List<float> NoteTiming; //�m�[�c�̏o���^�C�~���O�p���X�g

    //�y�ȃt�@�C���̐���p
    AudioSource Music;      //Pop swing   DOVA-SYNDROME�l�@(https://dova-s.jp/bgm/play9758.html)

    //�m�[�c�֘A
    //�Q�[���I�u�W�F�N�g
    [SerializeField] GameObject Vegetables;     //�ʏ�m�[�c
    [SerializeField] GameObject Potate;         //�ʏ�m�[�c
    [SerializeField] GameObject Carrot;         //�ʏ�m�[�c

    [SerializeField] GameObject Meat;           //����m�[�c

    [SerializeField] GameObject SpawnPoint;     //�o���ʒu
    Vector3 SpawnPosition;

    [SerializeField] GameObject BeatPoint;      //�����ʒu
    Vector3 BeatPosition;
    
    //[SerializeField] Transform SpawnPointTransform;  
    //[SerializeField] Transform BeatPointTransform;  

    //�������̍���
    float ParabolaHeight;           //�i�[�p
    float ParabolaHeightVegetables; //�ʏ�m�[�c
    float ParabolaHeightMeat;       //����m�[�c

    //�n�����牟���܂ł̎���
    float Duration;       //�i�[�p
    float DurationVege;   //�ʏ�m�[�c
    float DurationMeat;   //����m�[�c

    int GoIndex;        //�n���Ώۂ̃m�[�c�̔ԍ�

    //�������̔���
    float CheckRange;   //�������肪�o��͈�
    float BeatRange;    //�ǔ��肪�o��͈�

    //���_
    float AddPoint;     //���Z���链�_
    public static float TotalScore;   //���v���_


    //�L�A�j���[�V����
    GameObject CatController;   //�m�[�c�n�����̃A�j���[�V�����Ŏg�p
    bool isCrouch;

    //���ʉ��Đ��p
    GameObject SEController;    //�{�^����������SE�Đ��ŗ��p

    //�G�t�F�N�g�Đ��p
    GameObject VEController;

    //�y�ȏI������p
    float SongLength;

    //�b���\��UI�p
    public GameObject TimeText;
    Text NowTimeText;


    // Start is called before the first frame update
    void Start()
    {
        //�e�J�n���Ԃ̐ݒ�
        LoadTime = Time.time * 1000;
        PlayTime = 0;

        //�y�Ȃ̍Đ�����̐ݒ�
        isPlaying = false;


        //�m�[�c�֘A�̐ݒ�
        //�o���ʒu�̍��W
        SpawnPosition = new Vector3(SpawnPoint.transform.position.x, SpawnPoint.transform.position.y, 0);

        //�����ʒu�̍��W
        BeatPosition = new Vector3(BeatPoint.transform.position.x, BeatPoint.transform.position.y, 0);

        //�������̍���
        ParabolaHeight = 0;
        ParabolaHeightVegetables = 4;
        ParabolaHeightMeat = 7;

        //�n�����牟���܂ł̎���
        Duration = 0;
        DurationVege = 500;
        DurationMeat = 1000;

        //�n���m�[�c�̔ԍ�
        GoIndex = 0;


        //�������̔���̐ݒ�
        CheckRange = 120;
        BeatRange = 80;

        //�X�R�A�����l�̐ݒ�
        TotalScore = 0;

        //�e�퐧��p�I�u�W�F�N�g�̐ݒ�
        CatController = GameObject.Find("Cat");
        isCrouch = false;
        SEController = GameObject.Find("SoundEffectController");
        VEController = GameObject.Find("VisualEffectController");


        //���ʃt�@�C���̓ǂݍ���
        loadChart();

        //�y�ȃt�@�C���̓ǂݍ���
        loadMusic();

        //�y�ȍĐ�����̌o�ߎ��� �\���p
        NowTimeText = TimeText.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //�o�ߎ��Ԃ̕ϐ����X�V
        TimeElapsed = Time.time * 1000;                         //�S�̂̌o�ߎ���
        TimeElapsed_Load = TimeElapsed - LoadTime;              //�V�[���ǂݍ��݂���̌o�ߎ���
        TimeElapsed_PlayMusic = TimeElapsed - PlayTime;         //�y�ȍĐ�����̌o�ߎ���

        //�y�ȍĐ�����̌o�ߎ��� �\���p
        NowTimeText.text = TimeElapsed_PlayMusic.ToString();

        //�V�[���ǂݍ��݂�2�b��Ɋy�Ȃ��Đ�����
        if (isPlaying == false
            && TimeElapsed_Load >= 2000)
        {
            Music.Play();
            PlayTime = TimeElapsed;                            //�y�ȍĐ����̎���
            TimeElapsed_PlayMusic = TimeElapsed - PlayTime;    //�y�ȍĐ�����̌o�ߎ��Ԃ�ݒ�i���݃t���[���̊ԂɃ��U���g��ʂւ̑J�ڂŎQ�Ƃ��邽�߁j
            isPlaying = true;                                  //�y�ȍĐ��̃t���O��^�ɐݒ�
        }

        //�y�ȍĐ���̏���
        if(isPlaying)
        {
            //�{�^���������̏���
            if(Input.GetMouseButtonDown(0))
            {
                beat(TimeElapsed_PlayMusic);
            }

            //���ɋN������m�[�c�̃^�C�v�ɂ���āA�n�����牟���܂ł̎��ԂƋO���̍�����ݒ肷��
            if (Notes.Count > GoIndex)
            {
                if (Notes[GoIndex].GetComponent<NoteController>().getType() == "Meat")
                {
                    Duration = DurationMeat;
                    ParabolaHeight = ParabolaHeightMeat;
                }
                else
                {
                    Duration = DurationVege;
                    ParabolaHeight = ParabolaHeightVegetables;
                }
            }

            //�m�[�c�n��0.3�b�O�Ƀm�[�c�̏o���ƔL�̏�Ԃ��u�����݁v�ɕύX����
            if (Notes.Count > GoIndex
                && (Notes[GoIndex].GetComponent<NoteController>().getTiming() - 300) <= TimeElapsed_PlayMusic + Duration
                && isCrouch == false) 
            {
                Notes[GoIndex].SetActive(true);
                CatController.GetComponent<CatController>().transitionCrouch();
                isCrouch = true;
            }

            //�m�[�c�n�������@���̃m�[�c�̉����^�C�~���O�Ɓi���݂̎���+�n�����牟���܂ł̎��ԁj�̔�r
            if (Notes.Count > GoIndex
                && Notes[GoIndex].GetComponent<NoteController>().getTiming() <= TimeElapsed_PlayMusic + Duration)
            {
                //�L�@�A�j���[�V��������
                CatController.GetComponent<CatController>().transitionThrow();
                isCrouch = false;

                //�m�[�c�n�������@�Ăяo��
                Notes[GoIndex].GetComponent<NoteController>().StartThrow(ParabolaHeight, SpawnPosition, BeatPosition, Duration); 
                GoIndex++;
            }

            //�y�ȏI����̏���
            if (TimeElapsed_PlayMusic > SongLength)
            {
                //���U���g��ʂ֑J��
                SceneManager.LoadScene("Result");
            }
        }
    }

    //���ʃt�@�C���̓ǂݍ���
    void loadChart()
    {
        //�m�[�c�p���X�g���쐬
        Notes = new List<GameObject>(); //�e�m�[�c�̃I�u�W�F�N�g���i�[�����z��
        NoteTiming = new List<float>(); //�e�m�[�c�̉����^�C�~���O���i�[�����z��

        string jsonText = Resources.Load<TextAsset>(ChartPath).ToString();  //������Ƃ��ĕ��ʃf�[�^��ǂݍ���

        JsonNode json = JsonNode.Parse(jsonText);               //�����񂩂�json�`���ɕϊ�
        BPM = int.Parse(json["bpm"].Get<string>());             //BPM�l���擾
        SongLength = int.Parse(json["length"].Get<string>());   //�Ȃ̒������擾

        foreach (var note in json["notes"])
        {
            string type = note["type"].Get<string>();                   //�m�[�c�̃^�C�v���擾
            float timing = float.Parse(note["timing"].Get<string>());   //�m�[�c�̏o���^�C�~���O���擾���A���l�ɕϊ�
            
            GameObject Note;
            if(type == "Potate")
            {
                Note = Instantiate(Potate, SpawnPosition, Quaternion.identity);   //Instantiate(��������I�u�W�F�N�g, �ʒu, ��]) Quaternion.identity �� ��]�����Ȃ�
            }
            else if(type == "Carrot")
            {
                Note = Instantiate(Carrot, SpawnPosition, Quaternion.identity);
            }
            else if (type == "Meat")
            {
                Note = Instantiate(Meat, SpawnPosition, Quaternion.identity);
            }
            else
            {
                Note = Instantiate(Potate, SpawnPosition, Quaternion.identity);       //��O�͒ʏ�m�[�c
            }

            //setParameter�֐������s
            Note.GetComponent<NoteController>().setParameter(type, timing);

            Notes.Add(Note);
            NoteTiming.Add(timing);

            Note.SetActive(false);
        }

        //�擾�����m�[�c�̐������Ƀm�[�c���Ƃ̓��_���v�Z����
        AddPoint = 100 / Notes.Count;
    }

    //�y�ȃt�@�C���̓ǂݍ���
    void loadMusic()
    {
        Music = this.GetComponent<AudioSource>();
        Music.clip = (AudioClip)Resources.Load(MusicPath);

        Music.Stop();
    }

    //�{�^���������̏���
    void beat(float timing)
    {
        float minDiff = -1;
        int minDiffIndex = -1;

        //�^�b�v�����^�C�~���O��p���Ĕ�����s���m�[�c�����肷��i�ł��^�b�v�����^�C�~���O�ɋ߂��m�[�c��ΏۂƂ���j
        for (int i = 0;i < NoteTiming.Count;i++)
        {
            //����I���ς݂̃m�[�c�͉����^�C�~���O�l��-1�Ƃ��A2��ڈȍ~�̓X�L�b�v����
            if(NoteTiming[i] > 0)
            {
                float diff = Mathf.Abs(NoteTiming[i] - timing); //�m�[�c�̉����^�C�~���O[i] - beat()�Ăяo�����_�̊y�ȓ����� ���@���ׂẴm�[�c�̉����^�C�~���O�Ɖ������̊y�ȓ����Ԃ̍����i�[����

                if (minDiff == -1 || minDiff > diff)    //����ƍ��̐��l����菭�Ȃ����ɍ��̐��l��minDiff�Ɋi�[���A�m�[�c�̔ԍ���minDiffIndex�Ɋi�[����
                {
                    minDiff = diff;
                    minDiffIndex = i;
                }
            }

        }

        //�Ώۃm�[�c�̉����^�C�~���O�Ƃ̍��ɉ����ĕ]�����s��
        if (minDiff != -1 & minDiff < CheckRange)    //���ȏ�̕]���̏ꍇ�͎��ɐi��
        {
            SEController.GetComponent<SoundEffectController>().PlaySE_BeatGood();
            VEController.GetComponent<VisualEffectController>().PlayVE_Smoke(Notes[minDiffIndex].transform.position.x, Notes[minDiffIndex].transform.position.y);

            if (Notes[minDiffIndex].GetComponent<NoteController>().getType() == "Carrot")
            {
                VEController.GetComponent<VisualEffectController>().PlayVE_Carrot(Notes[minDiffIndex].transform.position.x, Notes[minDiffIndex].transform.position.y);
            }
            else if (Notes[minDiffIndex].GetComponent<NoteController>().getType() == "Potate")
            {
                VEController.GetComponent<VisualEffectController>().PlayVE_Potate(Notes[minDiffIndex].transform.position.x, Notes[minDiffIndex].transform.position.y);
            }
            else if (Notes[minDiffIndex].GetComponent<NoteController>().getType() == "Meat")
            {
                VEController.GetComponent<VisualEffectController>().PlayVE_Meat(Notes[minDiffIndex].transform.position.x, Notes[minDiffIndex].transform.position.y);
            }
            else
            {
                Debug.Log("�{�^�������^�C�~���O�@�m�[�c�^�C�v�̐ݒ�~�X");
            }

            NoteTiming[minDiffIndex] = -1;          //������s�����m�[�c�̉����^�C�~���O�l��-1�ɏ���������
            Notes[minDiffIndex].SetActive(false);   //�Ώۂ̃m�[�c���\���ɂ���

            if (minDiff < BeatRange)    //�Ǖ]���̏���
            {
                TotalScore += AddPoint;                
            }
            else //���]���̏���
            {
                TotalScore += AddPoint / 2;             //���_�͔���
            }
        }
        else //�s�]���̏���
        {
            SEController.GetComponent<SoundEffectController>().PlaySE_BeatMiss();
        }
    }
}
