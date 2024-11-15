using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] string ChartPath;  //譜面ファイルのパス
    [SerializeField] string MusicPath;  //楽曲ファイルのパス
    public AudioClip BGM;

    [SerializeField] GameObject Vegetables; //通常ノーツのプレファブ
    [SerializeField] GameObject Potate; //通常ノーツのプレファブ
    [SerializeField] GameObject Carrot; //通常ノーツのプレファブ

    [SerializeField] GameObject Meat;       //特殊ノーツのプレファブ

    [SerializeField] Transform SpawnPointTransform;  //ノーツの出現位置
    [SerializeField] Transform BeatPointTransform;   //ノーツの押下位置
    
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] GameObject BeatPoint;
    Vector3 SpawnPosition;
    Vector3 BeatPosition;
    float ParabolaHeight;
    float ParabolaHeightMeat;   //放物線の高さ
    float ParabolaHeightVegetables;   //放物線の高さ




    //ノーツ管理用
    string Title;           //楽曲タイトル
    int BPM;                //BPM
    List<GameObject> Notes; //ノーツ用リスト
    List<float> NoteTiming; //ノーツの出現タイミング用リスト

    AudioSource Music;      //楽曲制御用コンポーネント  Pop swing   DOVA-SYNDROME様　(https://dova-s.jp/bgm/play9758.html)

    float PlayTime;     //プレイ開始の時間
    float DistanceX;    //ノーツの初期位置から叩く位置までのX軸の距離
    float DistanceY;    //同上　Y軸の距離
    float During;       //ノーツの初期位置から叩く位置までの時間
    float DuringVege;   
    float DuringMeat;
    bool isPlaying;     //プレイ中判定フラグ
    int GoIndex;        //始動対象のノーツの番号

    float CheckRange;   //押下判定が出る範囲
    float BeatRange;    //良判定が出る範囲

    //得点計算用変数
    float AddPoint;     //加算する得点
    public static float TotalScore;   //合計得点

    //猫アニメーション用変数
    GameObject CatController;   //ノーツ始動時のアニメーションで使用
    bool isCrouch;

    //効果音再生用変数
    GameObject SEController;    //ボタン押下時のSE再生で利用

    //エフェクト再生用変数
    GameObject VEController;

    //楽曲終了判定用
    float SongLength;


    //秒数表示UI用
    public GameObject TimeText;
    Text NowTimeText;
    float NowTime;

    //問題調査用
    float LoadTime;

    float ingameTime;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Time" + Time.time * 1000);

        DistanceX = Mathf.Abs(BeatPointTransform.position.x-SpawnPointTransform.position.x);
        DistanceY = Mathf.Abs(BeatPointTransform.position.y-SpawnPointTransform.position.y);
        SpawnPosition = new Vector3(SpawnPoint.transform.position.x, SpawnPoint.transform.position.y, 0);
        BeatPosition = new Vector3(BeatPoint.transform.position.x, BeatPoint.transform.position.y, 0);

        ParabolaHeightVegetables = 4;
        ParabolaHeightMeat = 7;


        During = 600;
        DuringVege = 500;
        DuringMeat = 1000;
        isPlaying = false;
        GoIndex = 0;

        CheckRange = 120;
        BeatRange = 80;

        TotalScore = 0;

        CatController = GameObject.Find("Cat");
        SEController = GameObject.Find("SoundEffectController");
        VEController = GameObject.Find("VisualEffectController");
        LoadTime = Time.time*1000;

        isCrouch = false;

        loadChart();
        loadMusic();

        NowTimeText = TimeText.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        NowTime = Time.time * 1000 - PlayTime;
        NowTimeText.text = NowTime.ToString();


        if (isPlaying == false
            && (Time.time * 1000 - LoadTime) >= 2000)
        {
            Debug.Log("音楽開始：" + Time.time * 1000);
            Music.Play();
            PlayTime = Time.time * 1000;
            isPlaying = true;
        }


        if (isPlaying
            && Input.GetMouseButtonDown(0))
        {
            beat(Time.time * 1000 - PlayTime);
        }

        //次に起動するノーツのタイプを参照し、押下までの時間を設定する
        if(isPlaying
            & Notes.Count > GoIndex)
        {
            if(Notes[GoIndex].GetComponent<NoteController>().getType() == "Meat")
            {
                During = DuringMeat;
                ParabolaHeight = ParabolaHeightMeat;
            }
            else
            {
                During = DuringVege;
                ParabolaHeight = ParabolaHeightVegetables;
            }

        }

        //ノーツ始動0.5秒前にノーツの出現と猫の状態を「かがみ」に変更する
        if (isPlaying
            && Notes.Count > GoIndex
            && (Notes[GoIndex].GetComponent<NoteController>().getTiming() - 300) <= (Time.time * 1000 - PlayTime) + During
            && isCrouch == false) //次のノーツの押下タイミングと（現在の時間+飛翔時間）を比較し、始動タイミングか判定する
        {
            CatController.GetComponent<CatController>().transitionCrouch();
            isCrouch = true;
        }

        if (isPlaying
            && Notes.Count > GoIndex
            && Notes[GoIndex].GetComponent<NoteController>().getTiming() <= (Time.time * 1000 - PlayTime) + During) //次のノーツの押下タイミングと（現在の時間+飛翔時間）を比較し、始動タイミングか判定する
        {
            CatController.GetComponent<CatController>().transitionThrow();
            isCrouch = false;

            //Notes[GoIndex].GetComponent<NoteController>().go(DistanceX,DistanceY, During); //go関数を呼び出す
            Notes[GoIndex].GetComponent<NoteController>().StartThrow(ParabolaHeight, SpawnPosition, BeatPosition, During); //対象ノーツの始動関数を呼び出す
            //Debug.Log("go関数：" + Time.time * 1000);
            GoIndex++;

        }

        if(Time.time * 1000 - PlayTime > SongLength)
        {
            SceneManager.LoadScene("Result");
        }
    }

    void loadChart()
    {
        //ノーツ用リストを作成
        Notes = new List<GameObject>(); //各ノーツのオブジェクトを格納した配列
        NoteTiming = new List<float>(); //各ノーツの押下タイミングを格納した配列

        string jsonText = Resources.Load<TextAsset>(ChartPath).ToString();  //文字列として譜面データを読み込み

        JsonNode json = JsonNode.Parse(jsonText);               //文字列からjson形式に変換
        BPM = int.Parse(json["bpm"].Get<string>());             //BPM値を取得
        SongLength = int.Parse(json["length"].Get<string>());   //曲の長さを取得

        foreach (var note in json["notes"])
        {
            string type = note["type"].Get<string>();                   //ノーツのタイプを取得
            float timing = float.Parse(note["timing"].Get<string>());   //ノーツの出現タイミングを取得し、数値に変換
            
            GameObject Note;
            if(type == "Potate")
            {
                Note = Instantiate(Potate, SpawnPointTransform.position, Quaternion.identity);   //Instantiate(生成するオブジェクト, 位置, 回転) Quaternion.identity → 回転させない
            }
            else if(type == "Carrot")
            {
                Note = Instantiate(Carrot, SpawnPointTransform.position, Quaternion.identity);
            }
            else if (type == "Meat")
            {
                Note = Instantiate(Meat, SpawnPointTransform.position, Quaternion.identity);
            }
            else
            {
                Note = Instantiate(Potate, SpawnPointTransform.position, Quaternion.identity);       //例外は通常ノーツ
            }
            Debug.Log("setParameter 前");
            //setParameter関数を実行
            Note.GetComponent<NoteController>().setParameter(type, timing);

            Notes.Add(Note);
            NoteTiming.Add(timing);
        }

        //取得したノーツの数を元にノーツごとの得点を計算する
        AddPoint = 100 / Notes.Count;

        Debug.Log("Finish loadChart()");
    }

    void loadMusic()
    {
        Music = this.GetComponent<AudioSource>();   //AudioSourceコンポーネントを取得
        Music.clip = (AudioClip)Resources.Load(MusicPath);

        Music.Stop();

        Debug.Log("Finish loadMusic()");
    }

    void beat(float timing)
    {


        float minDiff = -1;
        int minDiffIndex = -1;

        //タップしたタイミングを用いて判定を行うノーツを決定する（最もタップしたタイミングに近いノーツを対象とする）
        for (int i = 0;i < NoteTiming.Count;i++)
        {
            //判定終了済みのノーツは押下タイミング値を-1とし、2回目以降はスキップする
            if(NoteTiming[i] > 0)
            {
                float diff = Mathf.Abs(NoteTiming[i] - timing); //ノーツの押下タイミング[i] - beat()呼び出し時点の楽曲内時間 →　すべてのノーツの押下タイミングと押下時の楽曲内時間の差を格納する

                if (minDiff == -1 || minDiff > diff)    //初回と差の数値がより少ない時に差の数値をminDiffに格納し、ノーツの番号をminDiffIndexに格納する
                {
                    minDiff = diff;
                    minDiffIndex = i;
                }
            }

        }

        //対象ノーツの押下タイミングとの差に応じて評価を行う
        if (minDiff != -1 & minDiff < CheckRange)    //並以上の評価の場合は次に進む
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
                Debug.Log("ボタン押下タイミング　ノーツタイプの設定ミス");
            }

            NoteTiming[minDiffIndex] = -1;          //判定を行ったノーツの押下タイミング値を-1に書き換える
            Notes[minDiffIndex].SetActive(false);   //対象のノーツを非表示にする

            if (minDiff < BeatRange)    //良評価の処理
            {
                TotalScore += AddPoint;                
            }
            else //並評価の処理
            {
                TotalScore += AddPoint / 2;             //加点は半分
            }
        }
        else //不可評価の処理
        {
            SEController.GetComponent<SoundEffectController>().PlaySE_BeatMiss();
        }

    }
}
