using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    //音声ファイル変換  XRECORD3
    //音声ファイル編集  SoundEngine Free

    public AudioClip ThrowNotes;    //剣の素振り３　効果音ラボ様(https://soundeffect-lab.info/sound/battle/)
    public AudioClip BeatGood;      //パッ          効果音ラボ様(https://soundeffect-lab.info/sound/anime/) 
    public AudioClip BeatMiss;      //パンチ素振り　効果音ラボ様(https://soundeffect-lab.info/sound/battle/)

    AudioSource audioSource;    //コンポーネント

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySE_ThrowNotes()
    {
        audioSource.PlayOneShot(ThrowNotes);
    }

    public void PlaySE_BeatGood()
    {
        audioSource.PlayOneShot(BeatGood);
    }

    public void PlaySE_BeatMiss()
    {
        audioSource.PlayOneShot(BeatMiss);
    }
}
