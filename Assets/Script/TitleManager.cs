using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public AudioClip TitleJingle;       //Springin様　ジングル21(https://www.springin.org/sound-stock/category/bgm-short/)
    public AudioClip TransitionSE;      //Springin様　正解5(https://www.springin.org/sound-stock/category/system/)

    AudioSource audioSource;    //コンポーネント

    float LoadTime; //画面ロード時の時間

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(TitleJingle);

        LoadTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Time.time * 1000 >= 3000)
        //{
        //    SceneManager.LoadScene("MainScene");
        //}

        if(Time.time > LoadTime + 3
           && Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(TransitionSE);
            Invoke(nameof(LoadMainScene), 1.0f);
        }
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GetPlayButtonDown()
    {
        SceneManager.LoadScene("MainScene");
    }
}
