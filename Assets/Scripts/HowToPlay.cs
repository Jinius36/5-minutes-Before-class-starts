using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    #region �̱���
    private static HowToPlay _Instance;
    public static HowToPlay Instance {  get { return _Instance; } }
    #endregion

    public GameObject images_HowToPlay; // �ν����� ����
    public GameObject btn_NextPage;
    public GameObject btn_PrevPage;
    public GameObject btn_CloseHow;
    public GameObject popup_HowToPlay;
    int nowPage;
    AudioSource audioSource;
    AudioClip audioClip;
    AudioClip audioClip2;

    public void OpenHowToPlay() // ���� ��� â ����, ���� ȭ�� HowToPlayButton�� ����
    {
        images_HowToPlay.SetActive(true);
        btn_NextPage.SetActive(true);
        btn_PrevPage.SetActive(true);
        btn_CloseHow.SetActive(true);
        PlayHow(audioClip2);
    } 

    public void CloseHowToPlay() // ���� ��� â �ݱ�
    {
        PlayHow(audioClip2);
        images_HowToPlay.SetActive(false);
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        btn_CloseHow.SetActive(false);
    }

    public void NextPage()
    {
        PlayHow(audioClip);
        //if(nowPage<images_HowToPlay.Length - 1) 
        //{
        //    images_HowToPlay[nowPage].SetActive(false);
        //    nowPage++;
        //    images_HowToPlay[nowPage].SetActive(true);
        //}
    }

    public void PrevPage()
    {
        PlayHow(audioClip);
        //if (nowPage > 0)
        //{
        //    images_HowToPlay[nowPage].SetActive(false);
        //    nowPage--;
        //    images_HowToPlay[nowPage].SetActive(true);
        //}
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
    }
}
