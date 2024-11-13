using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public AudioClip TitleJingle;       //Springin様　ジングル21(https://www.springin.org/sound-stock/category/bgm-short/)
    public AudioClip TransitionSE;      //Springin様　正解5(https://www.springin.org/sound-stock/category/system/)

    AudioSource audioSource;    //コンポーネント

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(TitleJingle);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Time.time * 1000 >= 3000)
        //{
        //    SceneManager.LoadScene("MainScene");
        //}

        if(Input.GetMouseButtonDown(0)
            || Input.GetMouseButtonDown(1))
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
