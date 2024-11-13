using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    bool isGo;  //ノーツ始動フラグ
    Vector3 firstPos;   //ノーツの初期位置

    //ノーツ生成時に取得
    string Type;    //ノーツのタイプ
    float Timing;   //ノーツの出現タイミング

    //ノーツ始動時に取得
    float DistanceX; //ノーツの初期位置から叩く位置までのX軸の距離
    float DistanceY; //ノーツの初期位置から叩く位置までのY軸の距離
    float During;   //ノーツの初期位置から叩く位置までの時間

    float GoTime;   //

    GameObject BeatPoint;   //押下位置
    GameObject HidePoint;
    bool isBeatPoint;
    float afterBeatPointDuration;

    GameObject SEController;    //ノーツ始動時のSE再生で利用

    // Start is called before the first frame update
    void Start()
    {
        isGo = false;
        firstPos = this.transform.position;

        BeatPoint = GameObject.Find("BeatPoint");
        HidePoint = GameObject.Find("HidePoint");
        isBeatPoint = false;

        SEController = GameObject.Find("SoundEffectController");
    }

    // Update is called once per frame
    void Update()
    {
        //if(isGo)
        //{
        //    //ノーツの位置を計算して移動させる
        //    this.gameObject.transform.position = new Vector3(firstPos.x - DistanceX * (Time.time * 1000 - GoTime) / During, firstPos.y + DistanceY * (Time.time * 1000 - GoTime) / During, firstPos.z);
        //}

        //押下位置に到達した場合、フラグを真に変更して移動開始時間を記録する
        if(this.gameObject.transform.position == BeatPoint.transform.position
            && isBeatPoint == false)
        {
            isBeatPoint = true;
            GoTime = Time.time * 1000;
        }

        //フラグが真の場合、非表示位置に向けて移動を行う
        if (isBeatPoint)
        {
            this.gameObject.transform.position = new Vector3(BeatPoint.transform.position.x + HidePoint.transform.position.x * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.y + HidePoint.transform.position.y * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.z);

        }
    }

    //各ノーツ作成時にGameManagerから呼び出される
    public void setParameter(string type,float timing)
    {
        //ノーツのタイプ・始動タイミングを設定する
        Type = type;
        Timing = timing;

        //ノーツのタイプに応じて押下位置以降の移動時間を設定する
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

    //各ノーツの始動時にGameManagerから呼び出される
    public void go(float distanceX, float distanceY, float during)
    {
        DistanceX = distanceX;
        DistanceY = distanceY;
        During = during;
        GoTime = Time.time * 1000;
        isGo = true;    //始動フラグをTrueに変更してノーツを動かす
        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
    }

    public void StartThrow(float height, Vector3 start, Vector3 end, float duration)
    {
        // 中点を求める
        Vector3 half = end - start * 0.50f + start;
        half.y += Vector3.up.y + height;

        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
        StartCoroutine(LerpThrow(start, half, end, duration));
    }

    IEnumerator LerpThrow(Vector3 start, Vector3 half, Vector3 end, float duration)
    {
        float startTime = Time.timeSinceLevelLoad;  //シーンをロードしてからの経過時間　関数の開始時間
        float rate = 0f;

        //Debug.Log("startTime:" + startTime);

        while (true)
        {
            if (rate >= 1.0f)
            {
                yield break;
            }


            float diff = Time.timeSinceLevelLoad - startTime;   //シーンをロードしてからの経過時間　関数の開始時間
            rate = diff / (duration / 1000f);

            this.gameObject.transform.position = CalcLerpPoint(start, half, end, rate);
            yield return null;
        }
    }

    Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t);
        var b = Vector3.Lerp(p1, p2, t);

        //Debug.Log("a:" + a);
        //Debug.Log("b:" + b);

        return Vector3.Lerp(a, b, t);
    }
}
