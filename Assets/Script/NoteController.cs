using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
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
        BeatPoint = GameObject.Find("BeatPoint");
        HidePoint = GameObject.Find("HidePoint");
        isBeatPoint = false;

        SEController = GameObject.Find("SoundEffectController");
    }

    // Update is called once per frame
    void Update()
    {
        //押下位置の到達を判定
        if (this.gameObject.transform.position == BeatPoint.transform.position
            && isBeatPoint == false)
        {
            isBeatPoint = true;
            GoTime = Time.time * 1000;
        }

        //押下位置到達後の処理
        if (isBeatPoint)
        {
            //非表示位置に向けて移動を行う
            this.gameObject.transform.position = new Vector3(BeatPoint.transform.position.x + HidePoint.transform.position.x * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.y + HidePoint.transform.position.y * (Time.time * 1000 - GoTime) / afterBeatPointDuration, 
                BeatPoint.transform.position.z);
        }

        //非表示位置の通過を判定
        if(this.gameObject.transform.position.x < HidePoint.transform.position.x
            && this.gameObject.transform.position.y < HidePoint.transform.position.y)
        {
            //ノーツを非表示にする
            this.gameObject.SetActive(false);
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

    //ノーツの始動時にGameManagerから呼び出される
    public void StartThrow(float parabolaHaight, Vector3 spawnPosition, Vector3 beatPosition, float duration)
    {
        // 中間地点を求める
        Vector3 half = beatPosition - spawnPosition * 0.50f + spawnPosition;
        half.y += Vector3.up.y + parabolaHaight;

        SEController.GetComponent<SoundEffectController>().PlaySE_ThrowNotes();
        StartCoroutine(LerpThrow(spawnPosition, half, beatPosition, duration));
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

        return Vector3.Lerp(a, b, t);
    }
}
