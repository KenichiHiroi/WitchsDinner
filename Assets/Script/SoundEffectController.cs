using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    //�����t�@�C���ϊ�  XRECORD3
    //�����t�@�C���ҏW  SoundEngine Free

    public AudioClip ThrowNotes;    //���̑f�U��R�@���ʉ����{�l(https://soundeffect-lab.info/sound/battle/)
    public AudioClip BeatGood;      //�p�b          ���ʉ����{�l(https://soundeffect-lab.info/sound/anime/) 
    public AudioClip BeatMiss;      //�p���`�f�U��@���ʉ����{�l(https://soundeffect-lab.info/sound/battle/)

    AudioSource audioSource;    //�R���|�[�l���g

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
