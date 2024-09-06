using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    bool isGo;  //�m�[�c�n���t���O
    Vector3 firstPos;   //�m�[�c�̏����ʒu

    //�m�[�c�������Ɏ擾
    string Type;    //�m�[�c�̃^�C�v
    float Timing;   //�m�[�c�̏o���^�C�~���O

    //�m�[�c�n�����Ɏ擾
    float DistanceX; //�m�[�c�̏����ʒu����@���ʒu�܂ł�X���̋���
    float DistanceY; //�m�[�c�̏����ʒu����@���ʒu�܂ł�Y���̋���
    float During;   //�m�[�c�̏����ʒu����@���ʒu�܂ł̎���

    float GoTime;   //

    GameObject SEController;    //�m�[�c�n������SE�Đ��ŗ��p

    // Start is called before the first frame update
    void Start()
    {
        isGo = false;
        firstPos = this.transform.position;

        SEController = GameObject.Find("SoundEffectController");
    }

    // Update is called once per frame
    void Update()
    {
        if(isGo)
        {
            //�m�[�c�̈ʒu���v�Z���Ĉړ�������
            this.gameObject.transform.position = new Vector3(firstPos.x - DistanceX * (Time.time * 1000 - GoTime) / During, firstPos.y + DistanceY * (Time.time * 1000 - GoTime) / During, firstPos.z);
        }
    }

    //�e�m�[�c�쐬����GameManager����Ăяo�����
    public void setParameter(string type,float timing)
    {
        //�m�[�c�̃^�C�v�E�n���^�C�~���O��ݒ肷��
        Type = type;
        Timing = timing;
    }

    public string getType()
    {
        return Type;
    }

    public float getTiming()
    {
        return Timing;
    }

    //�e�m�[�c�̎n������GameManager����Ăяo�����
    public void go(float distanceX, float distanceY, float during)
    {
        DistanceX = distanceX;
        DistanceY = distanceY;
        During = during;
        GoTime = Time.time * 1000;
        isGo = true;    //�n���t���O��True�ɕύX���ăm�[�c�𓮂���
        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
    }

    public void StartThrow(float height, Vector3 start, Vector3 end, float duration)
    {
        // ���_�����߂�
        Vector3 half = end - start * 0.50f + start;
        half.y += Vector3.up.y + height;

        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
        StartCoroutine(LerpThrow(start, half, end, duration));
    }

    IEnumerator LerpThrow(Vector3 start, Vector3 half, Vector3 end, float duration)
    {
        float startTime = Time.timeSinceLevelLoad;  //�V�[�������[�h���Ă���̌o�ߎ��ԁ@�֐��̊J�n����
        float rate = 0f;

        Debug.Log("startTime:" + startTime);

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

        Debug.Log("a:" + a);
        Debug.Log("b:" + b);

        return Vector3.Lerp(a, b, t);
    }
}
