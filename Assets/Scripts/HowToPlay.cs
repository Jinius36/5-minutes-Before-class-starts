using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    #region 싱글톤
    private static HowToPlay _Instance;
    public static HowToPlay Instance {  get { return _Instance; } }
    #endregion

    public GameObject images_HowToPlay; // 인스펙터 조작
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
        images_HowToPlay.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_PrevPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        tuto.gameObject.SetActive(true);
        PlayHow(audioClip2);
    } 

    public void CloseHowToPlay() // 게임 방법 창 닫기
    {
        PlayHow(audioClip2);
        images_HowToPlay.SetActive(false);
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        btn_CloseHow.SetActive(false);
        tuto.gameObject.SetActive(false);
    }

    public void NextPage()
    {
        if(nowPage != 2)
        {
            PlayHow(audioClip);
            nowPage++;
            tuto.sprite = tuto_Images[nowPage];
        }
    }

    public void PrevPage()
    {
        if (nowPage != 0)
        {
            PlayHow(audioClip);
            nowPage--;
            tuto.sprite = tuto_Images[nowPage];
        }
    }

    void PlayHow(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioClip = Resources.Load<AudioClip>("Sound/SlideHow");
        audioClip2 = Resources.Load<AudioClip>("Sound/Button");
        tuto_Images[0] = Resources.Load<Sprite>("Sprites/Tut1");
        tuto_Images[1] = Resources.Load<Sprite>("Sprites/Tut2");
        tuto_Images[2] = Resources.Load<Sprite>("Sprites/Tut3");
    }
}
