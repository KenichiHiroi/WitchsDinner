using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] string ChartPath;  //���ʃt�@�C���̃p�X
    [SerializeField] string MusicPath;  //�y�ȃt�@�C���̃p�X

    [SerializeField] GameObject Vegetables; //�ʏ�m�[�c�̃v���t�@�u
    [SerializeField] GameObject Meat;       //����m�[�c�̃v���t�@�u

    [SerializeField] Transform SpawnPoint;  //�m�[�c�̏o���ʒu
    [SerializeField] Transform BeatPoint;   //�m�[�c�̉����ʒu

    //�m�[�c�Ǘ��p
    string Title;           //�y�ȃ^�C�g��
    int BPM;                //BPM
    List<GameObject> Notes; //�m�[�c�p���X�g
    List<float> NoteTiming; //�m�[�c�̏o���^�C�~���O�p���X�g

    AudioSource Music;      //�y�Ȑ���p�R���|�[�l���g  Pop swing   DOVA-SYNDROME�l�@(https://dova-s.jp/bgm/play9758.html)

    float PlayTime;     //�v���C�J�n�̎���
    float DistanceX;    //�m�[�c�̏����ʒu����@���ʒu�܂ł�X���̋���
    float DistanceY;    //����@Y���̋���
    float During;       //�m�[�c�̏����ʒu����@���ʒu�܂ł̎���
    float DuringVege;   
    float DuringMeat;
    bool isPlaying;     //�v���C������t���O
    int GoIndex;        //�n���Ώۂ̃m�[�c�̔ԍ�

    float CheckRange;   //�������肪�o��͈�
    float BeatRange;    //�ǔ��肪�o��͈�

    //���_�v�Z�p�ϐ�
    float AddPoint;     //���Z���链�_
    float TotalScore;   //���v���_

    //���ʉ��Đ��p�ϐ�
    GameObject SEController;    //�{�^����������SE�Đ��ŗ��p

    //�y�ȏI������p
    float SongLength;


    //�b���\��UI�p
    public GameObject TimeText;
    Text NowTimeText;
    float NowTime;

    // Start is called before the first frame update
    void Start()
    {
        DistanceX = Mathf.Abs(BeatPoint.position.x-SpawnPoint.position.x);
        DistanceY = Mathf.Abs(BeatPoint.position.y-SpawnPoint.position.y);
        During = 600;
        DuringVege = 500;
        DuringMeat = 1000;
        isPlaying = false;
        GoIndex = 0;

        CheckRange = 120;
        BeatRange = 80;

        TotalScore = 0;

        SEController = GameObject.Find("SoundEffectController");


        loadChart();
        loadMusic();

        NowTimeText = TimeText.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        NowTime = Time.time * 1000 - PlayTime;
        NowTimeText.text = NowTime.ToString();



        if (isPlaying
            && Input.GetMouseButtonDown(0))
        {
            beat(Time.time * 1000 - PlayTime);
        }

        //���ɋN������m�[�c�̃^�C�v���Q�Ƃ��A�����܂ł̎��Ԃ�ݒ肷��
        if(isPlaying
            & Notes.Count > GoIndex)
        {
            if(Notes[GoIndex].GetComponent<NoteController>().getType() == "Meat")
            {
                During = DuringMeat;
            }
            else
            {
                During = DuringVege;
            }

        }
        

        if(isPlaying
            && Notes.Count > GoIndex
            && Notes[GoIndex].GetComponent<NoteController>().getTiming() <= (Time.time * 1000 - PlayTime) + During) //���̃m�[�c�̎n���^�C�~���O�ƌ��݂̐i�s�
        {
            //Debug.Log("�m�[�c�̃^�C�v���擾�F" + Notes[GoIndex].GetComponent<NoteController>().getType());
            Debug.Log("Time.time:" + Time.time);
            Debug.Log("PlayTime:" + PlayTime);

            Debug.Log("(Time.time * 1000 - PlayTime) + During:" + ((Time.time * 1000 - PlayTime) + During));
            Debug.Log("Nowtime:" + NowTime);

            Notes[GoIndex].GetComponent<NoteController>().go(DistanceX,DistanceY, During); //go�֐����Ăяo��
            GoIndex++;
        }

        if(Time.time - PlayTime > SongLength)
        {
            SceneManager.LoadScene("Result");
        }
    }

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
            if(type == "Vegetables")
            {
                Note = Instantiate(Vegetables, SpawnPoint.position, Quaternion.identity);   //Instantiate(��������I�u�W�F�N�g, �ʒu, ��]) Quaternion.identity �� ��]�����Ȃ�
            }
            else if(type == "Meat")
            {
                Note = Instantiate(Meat, SpawnPoint.position, Quaternion.identity);
            }
            else
            {
                Note = Instantiate(Vegetables, SpawnPoint.position, Quaternion.identity);       //��O�͒ʏ�m�[�c
            }

            //setParameter�֐������s
            Note.GetComponent<NoteController>().setParameter(type, timing);

            Notes.Add(Note);
            NoteTiming.Add(timing);
        }

        //�擾�����m�[�c�̐������Ƀm�[�c���Ƃ̓��_���v�Z����
        AddPoint = 100 / Notes.Count;

        //Debug.Log("�擾�����m�[�c���F" + Notes.Count);
        //Debug.Log("1�m�[�c������̓��_�F" + AddPoint);

        Debug.Log("Finish loadChart()");
    }

    void loadMusic()
    {
        Music = this.GetComponent<AudioSource>();   //AudioSource�R���|�[�l���g���擾
        Music.clip = (AudioClip)Resources.Load(MusicPath);

        Music.Stop();
        Music.Play();

        PlayTime = Time.time * 1000;
        isPlaying = true;
        Debug.Log("PlayTime:" + PlayTime);

        Debug.Log("Finish loadMusic()");
    }

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

        //Debug.Log("�Ώۃm�[�c�̔ԍ��F" + minDiffIndex + " �Ώۃm�[�c�̉����^�C�~���O:" + NoteTiming[minDiffIndex]
        //    + "\n   timing(�^�b�v�����^�C�~���O):" + timing + " minDiff(�Ώۃm�[�c�̉����^�C�~���O�Ƃ̍�):" + minDiff);

        //�Ώۃm�[�c�̉����^�C�~���O�Ƃ̍��ɉ����ĕ]�����s��
        if (minDiff != -1 & minDiff < CheckRange)    //���ȏ�̕]���̏ꍇ�͎��ɐi��
        {
            if (minDiff < BeatRange)    //�Ǖ]���̏���
            {
                SEController.GetComponent<SoundEffectController>().PlaySE_BeatGood();


                NoteTiming[minDiffIndex] = -1;          //������s�����m�[�c�̉����^�C�~���O�l��-1�ɏ���������
                Notes[minDiffIndex].SetActive(false);   //�Ώۂ̃m�[�c���\���ɂ���
                TotalScore += AddPoint;
                //Debug.Log("�����^�C�~���O�̕]���F��");
                //Debug.Log("���݂̓��_�F" + TotalScore);
                
            }
            else //���]���̏���
            {
                SEController.GetComponent<SoundEffectController>().PlaySE_BeatGood();

                NoteTiming[minDiffIndex] = -1;          //������s�����m�[�c�̉����^�C�~���O�l��-1�ɏ���������
                Notes[minDiffIndex].SetActive(false);   //�Ώۂ̃m�[�c���\���ɂ���
                TotalScore += AddPoint / 2;

                //Debug.Log("�����^�C�~���O�̕]���F��");
                //Debug.Log("���݂̓��_�F" + TotalScore);

            }
        }
        else //�s�]���̏���
        {
            SEController.GetComponent<SoundEffectController>().PlaySE_BeatMiss();

            //Debug.Log("�����^�C�~���O�̕]���F�s��");
        }

    }
}
