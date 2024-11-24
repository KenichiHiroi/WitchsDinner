using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    //ゲームオブジェクト
    [SerializeField] GameObject GoodRatingPicture;
    [SerializeField] GameObject NomalRatingPicture;
    [SerializeField] GameObject RatingText;
    [SerializeField] GameObject InfomationText;

    //オーディオソース
    AudioSource audioSource;

    //効果音
    public AudioClip GoodRatingSE;
    public AudioClip NormalRatingSE;

    //時間管理
    float LoadTime;     //シーン読み込み時間

    // Start is called before the first frame update
    void Start()
    {
        //シーン読み込み時間を設定
        LoadTime = Time.time;

        //画像・テキストの初期化
        GoodRatingPicture.SetActive(false);
        NomalRatingPicture.SetActive(false);
        RatingText.GetComponent<Text>().text = "　";

        //オーディオソースを設定
        audioSource = GetComponent<AudioSource>();

        //合計得点に応じて、画像・テキストを表示する
        if(GameManager.TotalScore >= 80)
        {
            GoodRatingPicture.SetActive(true);
            RatingText.GetComponent<Text>().text = "大満足の美味しいカレーになりました！";
            audioSource.PlayOneShot(GoodRatingSE);
        }
        else
        {
            NomalRatingPicture.SetActive(true);
            RatingText.GetComponent<Text>().text = "素材の味が生きてて、これもまた一興……かも……！";
            audioSource.PlayOneShot(NormalRatingSE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //シーン読み込みの3秒後にタイトル画面への案内テキストを表示する
        if(InfomationText.activeSelf == false
            && (Time.time - LoadTime) > 3)
        {
            InfomationText.SetActive(true);
        }

        //案内テキストの表示後、左クリックでタイトルに戻る
        if (InfomationText.activeSelf
            && Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
