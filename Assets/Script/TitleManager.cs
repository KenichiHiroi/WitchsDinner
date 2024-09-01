using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time * 1000 >= 3000)
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void GetPlayButtonDown()
    {
        SceneManager.LoadScene("MainScene");
    }
}
