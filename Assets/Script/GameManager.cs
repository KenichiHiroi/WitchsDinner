using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //楽曲の再生判定
    bool isPlaying;

    //時間関連
    float LoadTime;                 //シーン読み込み時の時間
    float PlayTime;                 //楽曲再生時の時間

    float TimeElapsed;              //ゲーム開始からの経過時間
    float TimeElapsed_Load;         //シーン読み込みからの経過時間
    float TimeElapsed_PlayMusic;    //楽曲開始からの経過時間

    //譜面・楽曲ファイルの読み込み用
    [SerializeField] string ChartPath;  //譜面ファイルのパス
    [SerializeField] string MusicPath;  //楽曲ファイルのパス

    string Title;           //楽曲タイトル
    int BPM;                //BPM
    List<GameObject> Notes; //ノーツ用リスト
    List<float> NoteTiming; //ノーツの出現タイミング用リスト

    //楽曲ファイルの制御用
    AudioSource Music;      //Pop swing   DOVA-SYNDROME様　(https://dova-s.jp/bgm/play9758.html)

    //ノーツ関連
    //ゲームオブジェクト
    [SerializeField] GameObject Vegetables;     //通常ノーツ
    [SerializeField] GameObject Potate;         //通常ノーツ
    [SerializeField] GameObject Carrot;         //通常ノーツ

    [SerializeField] GameObject Meat;           //特殊ノーツ

    [SerializeField] GameObject SpawnPoint;     //出現位置
    Vector3 SpawnPosition;

    [SerializeField] GameObject BeatPoint;      //押下位置
    Vector3 BeatPosition;
    
    //[SerializeField] Transform SpawnPointTransform;  
    //[SerializeField] Transform BeatPointTransform;  

    //放物線の高さ
    float ParabolaHeight;           //格納用
    float ParabolaHeightVegetables; //通常ノーツ
    float ParabolaHeightMeat;       //特殊ノーツ

    //始動から押下までの時間
    float Duration;       //格納用
    float DurationVege;   //通常ノーツ
    float DurationMeat;   //特殊ノーツ

    int GoIndex;        //始動対象のノーツの番号

    //押下時の判定
    float CheckRange;   //押下判定が出る範囲
    float BeatRange;    //良判定が出る範囲

    //得点
    float AddPoint;     //加算する得点
    public static float TotalScore;   //合計得点


    //猫アニメーション
    GameObject CatController;   //ノーツ始動時のアニメーションで使用
    bool isCrouch;

    //効果音再生用
    GameObject SEController;    //ボタン押下時のSE再生で利用

    //エフェクト再生用
    GameObject VEController;

    //楽曲終了判定用
    float SongLength;

    //秒数表示UI用
    public GameObject TimeText;
    Text NowTimeText;


    // Start is called before the first frame update
    void Start()
    {
        //各開始時間の設定
        LoadTime = Time.time * 1000;
        PlayTime = 0;

        //楽曲の再生判定の設定
        isPlaying = false;


        //ノーツ関連の設定
        //出現位置の座標
        SpawnPosition = new Vector3(SpawnPoint.transform.position.x, SpawnPoint.transform.position.y, 0);

        //押下位置の座標
        BeatPosition = new Vector3(BeatPoint.transform.position.x, BeatPoint.transform.position.y, 0);

        //放物線の高さ
        ParabolaHeight = 0;
        ParabolaHeightVegetables = 4;
        ParabolaHeightMeat = 7;

        //始動から押下までの時間
        Duration = 0;
        DurationVege = 500;
        DurationMeat = 1000;

        //始動ノーツの番号
        GoIndex = 0;


        //押下時の判定の設定
        CheckRange = 120;
        BeatRange = 80;

        //スコア初期値の設定
        TotalScore = 0;

        //各種制御用オブジェクトの設定
        CatController = GameObject.Find("Cat");
        isCrouch = false;
        SEController = GameObject.Find("SoundEffectController");
        VEController = GameObject.Find("VisualEffectController");


        //譜面ファイルの読み込み
        loadChart();

        //楽曲ファイルの読み込み
        loadMusic();

        //楽曲再生からの経過時間 表示用
        NowTimeText = TimeText.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //経過時間の変数を更新
        TimeElapsed = Time.time * 1000;                         //全体の経過時間
        TimeElapsed_Load = TimeElapsed - LoadTime;              //シーン読み込みからの経過時間
        TimeElapsed_PlayMusic = TimeElapsed - PlayTime;         //楽曲再生からの経過時間

        //楽曲再生からの経過時間 表示用
        NowTimeText.text = TimeElapsed_PlayMusic.ToString();

        //シーン読み込みの2秒後に楽曲を再生する
        if (isPlaying == false
            && TimeElapsed_Load >= 2000)
        {
            Music.Play();
            PlayTime = TimeElapsed;                            //楽曲再生時の時間
            TimeElapsed_PlayMusic = TimeElapsed - PlayTime;    //楽曲再生からの経過時間を設定（現在フレームの間にリザルト画面への遷移で参照するため）
            isPlaying = true;                                  //楽曲再生のフラグを真に設定
        }

        //楽曲再生後の処理
        if(isPlaying)
        {
            //ボタン押下時の処理
            if(Input.GetMouseButtonDown(0))
            {
                beat(TimeElapsed_PlayMusic);
            }

            //次に起動するノーツのタイプによって、始動から押下までの時間と軌道の高さを設定する
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

            //ノーツ始動0.3秒前にノーツの出現と猫の状態を「かがみ」に変更する
            if (Notes.Count > GoIndex
                && (Notes[GoIndex].GetComponent<NoteController>().getTiming() - 300) <= TimeElapsed_PlayMusic + Duration
                && isCrouch == false) 
            {
                Notes[GoIndex].SetActive(true);
                CatController.GetComponent<CatController>().transitionCrouch();
                isCrouch = true;
            }

            //ノーツ始動処理　次のノーツの押下タイミングと（現在の時間+始動から押下までの時間）の比較
            if (Notes.Count > GoIndex
                && Notes[GoIndex].GetComponent<NoteController>().getTiming() <= TimeElapsed_PlayMusic + Duration)
            {
                //猫　アニメーション制御
                CatController.GetComponent<CatController>().transitionThrow();
                isCrouch = false;

                //ノーツ始動処理　呼び出し
                Notes[GoIndex].GetComponent<NoteController>().StartThrow(ParabolaHeight, SpawnPosition, BeatPosition, Duration); 
                GoIndex++;
            }

            //楽曲終了後の処理
            if (TimeElapsed_PlayMusic > SongLength)
            {
                //リザルト画面へ遷移
                SceneManager.LoadScene("Result");
            }
        }
    }

    //譜面ファイルの読み込み
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
                Note = Instantiate(Potate, SpawnPosition, Quaternion.identity);   //Instantiate(生成するオブジェクト, 位置, 回転) Quaternion.identity → 回転させない
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
                Note = Instantiate(Potate, SpawnPosition, Quaternion.identity);       //例外は通常ノーツ
            }

            //setParameter関数を実行
            Note.GetComponent<NoteController>().setParameter(type, timing);

            Notes.Add(Note);
            NoteTiming.Add(timing);

            Note.SetActive(false);
        }

        //取得したノーツの数を元にノーツごとの得点を計算する
        AddPoint = 100 / Notes.Count;
    }

    //楽曲ファイルの読み込み
    void loadMusic()
    {
        Music = this.GetComponent<AudioSource>();
        Music.clip = (AudioClip)Resources.Load(MusicPath);

        Music.Stop();
    }

    //ボタン押下時の処理
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
