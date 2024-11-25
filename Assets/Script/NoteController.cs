using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    //�m�[�c�������Ɏ擾
    string Type;    //�m�[�c�̃^�C�v
    float Timing;   //�m�[�c�̏o���^�C�~���O

    //�m�[�c�n�����Ɏ擾
    float DistanceX; //�m�[�c�̏����ʒu����@���ʒu�܂ł�X���̋���
    float DistanceY; //�m�[�c�̏����ʒu����@���ʒu�܂ł�Y���̋���
    float During;   //�m�[�c�̏����ʒu����@���ʒu�܂ł̎���

    float GoTime;   //

    GameObject BeatPoint;   //�����ʒu
    GameObject HidePoint;
    bool isBeatPoint;
    float afterBeatPointDuration;

    GameObject SEController;    //�m�[�c�n������SE�Đ��ŗ��p

    // Start is called before the first frame update
    void Start()
    {
        BeatPoint = GameObject.Find("BeatPoint");
        HidePoint = GameObject.Find("HidePoint");
        isBeatPoint = false;

        SEController = GameObject.Find("SoundEffectController");
    }

    // Update is called once per frame
    void Update()
    {
        //�����ʒu�̓��B�𔻒�
        if (this.gameObject.transform.position == BeatPoint.transform.position
            && isBeatPoint == false)
        {
            isBeatPoint = true;
            GoTime = Time.time * 1000;
        }

        //�����ʒu���B��̏���
        if (isBeatPoint)
        {
            //��\���ʒu�Ɍ����Ĉړ����s��
            this.gameObject.transform.position = new Vector3(BeatPoint.transform.position.x + HidePoint.transform.position.x * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.y + HidePoint.transform.position.y * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.z);
        }

        //��\���ʒu�̒ʉ߂𔻒�
        if(this.gameObject.transform.position.x < HidePoint.transform.position.x
            && this.gameObject.transform.position.y < HidePoint.transform.position.y)
        {
            //�m�[�c���\���ɂ���
            this.gameObject.SetActive(false);
        }
    }

    //�e�m�[�c�쐬����GameManager����Ăяo�����
    public void setParameter(string type,float timing)
    {
        //�m�[�c�̃^�C�v�E�n���^�C�~���O��ݒ肷��
        Type = type;
        Timing = timing;

        //�m�[�c�̃^�C�v�ɉ����ĉ����ʒu�ȍ~�̈ړ����Ԃ�ݒ肷��
        if(Type == "Potate"
            || Type == "Carrot")
        {
            afterBeatPointDuration = 200;
        }
        else
        {
            afterBeatPointDuration = 300;
        }
    }

    public string getType()
    {
        return Type;
    }

    public float getTiming()
    {
        return Timing;
    }

    //�m�[�c�̎n������GameManager����Ăяo�����
    public void StartThrow(float parabolaHaight, Vector3 spawnPosition, Vector3 beatPosition, float duration)
    {
        // ���Ԓn�_�����߂�
        Vector3 half = beatPosition - spawnPosition * 0.50f + spawnPosition;
        half.y += Vector3.up.y + parabolaHaight;

        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
        StartCoroutine(LerpThrow(spawnPosition, half, beatPosition, duration));
    }

    IEnumerator LerpThrow(Vector3 start, Vector3 half, Vector3 end, float duration)
    {
        float startTime = Time.timeSinceLevelLoad;  //�V�[�������[�h���Ă���̌o�ߎ��ԁ@�֐��̊J�n����
        float rate = 0f;

        //Debug.Log("startTime:" + startTime);

        while (true)
        {
            if (rate >= 1.0f)
            {
                yield break;
            }


            float diff = Time.timeSinceLevelLoad - startTime;   //�V�[�������[�h���Ă���̌o�ߎ��ԁ@�֐��̊J�n����
            rate = diff / (duration / 1000f);

            this.gameObject.transform.position = CalcLerpPoint(start, half, end, rate);
            yield return null;
        }
    }

    Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t);
        var b = Vector3.Lerp(p1, p2, t);

        return Vector3.Lerp(a, b, t);
    }
}
