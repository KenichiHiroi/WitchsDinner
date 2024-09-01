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

    GameObject SEController;    //ノーツ始動時のSE再生で利用

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
            //ノーツの位置を計算して移動させる
            this.gameObject.transform.position = new Vector3(firstPos.x - DistanceX * (Time.time * 1000 - GoTime) / During, firstPos.y + DistanceY * (Time.time * 1000 - GoTime) / During, firstPos.z);
        }
    }

    //各ノーツ作成時にGameManagerから呼び出される
    public void setParameter(string type,float timing)
    {
        //ノーツのタイプ・始動タイミングを設定する
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
}
