using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    #region 싱글톤
    private static HowToPlay _Instance;
    public static HowToPlay Instance {  get { return _Instance; } }
    #endregion

    public GameObject[] images_HowToPlay; // 인스펙터 조작
    public GameObject btn_NextPage;
    public GameObject btn_PrevPage;
    public GameObject popup_HowToPlay;
    int nowPage;

    private void Awake()
    {
        popup_HowToPlay.SetActive(false);
        for(int i = 0; i < images_HowToPlay.Length; i++) // 처음엔 꺼져있어야함
        {
            images_HowToPlay[i].SetActive(false);
        }
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        nowPage = 0;
        _Instance = this; // NULL 방지
    }

    public void OpenHowToPlay() // 게임 방법 창 열기, 시작 화면 HowToPlayButton이 보유
    {
        popup_HowToPlay.SetActive(true);
        images_HowToPlay[0].SetActive(true);
        btn_NextPage.SetActive(true);
        btn_PrevPage.SetActive(true);
    } 

    public void CloseHowToPlay() // 게임 방법 창 닫기
    {
        popup_HowToPlay.SetActive(false);
    }

    public void NextPage()
    {
        if(nowPage<images_HowToPlay.Length - 1) 
        {
            images_HowToPlay[nowPage].SetActive(false);
            nowPage++;
            images_HowToPlay[nowPage].SetActive(true);
        }
    }

    public void PrevPage()
    {
        if (nowPage > 0)
        {
            images_HowToPlay[nowPage].SetActive(false);
            nowPage--;
            images_HowToPlay[nowPage].SetActive(true);
        }
    }
}
