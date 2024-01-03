using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public GameObject teduri;
    public GameObject btn_NextPage;
    public GameObject btn_PrevPage;
    public GameObject btn_CloseHow;
    public Image tuto;
    Sprite[] tuto_Images=new Sprite[3];
    int nowPage = 0;
    AudioSource audioSource;
    AudioClip audioClip;
    AudioClip audioClip2;
    public void OpenHowToPlay() // 게임 방법 창 열기, 시작 화면 HowToPlayButton이 보유
    {
        teduri.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        tuto.gameObject.SetActive(true);
        PlayHow(audioClip2);
    } 
    public void CloseHowToPlay() // 게임 방법 창 닫기
    {
        PlayHow(audioClip2);
        teduri.SetActive(false);
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        btn_CloseHow.SetActive(false);
        tuto.gameObject.SetActive(false);
    }
    public void NextPage()
    {
        btn_PrevPage.SetActive(true);
        if(nowPage != 2)
        {
            PlayHow(audioClip);
            nowPage++;
            tuto.sprite = tuto_Images[nowPage];
        }
        if (nowPage == 2)
            btn_NextPage.SetActive(false);
    }
    public void PrevPage()
    {
        btn_NextPage.SetActive(true);
        if (nowPage != 0)
        {
            PlayHow(audioClip);
            nowPage--;
            tuto.sprite = tuto_Images[nowPage];
        }
        if (nowPage == 0)
            btn_PrevPage.SetActive(false);
    }
    void PlayHow(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioClip = Resources.Load<AudioClip>("Sound/SlideHow");
        audioClip2 = Resources.Load<AudioClip>("Sound/Button");
        tuto_Images[0] = Resources.Load<Sprite>("Sprites/Tut1");
        tuto_Images[1] = Resources.Load<Sprite>("Sprites/Tut2");
        tuto_Images[2] = Resources.Load<Sprite>("Sprites/Tut3");
    }
}