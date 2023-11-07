using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    #region �̱���
    private static HowToPlay _Instance;
    public static HowToPlay Instance {  get { return _Instance; } }
    #endregion

    public GameObject[] images_HowToPlay; // �ν����� ����
    public GameObject btn_NextPage;
    public GameObject btn_PrevPage;
    public GameObject popup_HowToPlay;
    int nowPage;

    private void Awake()
    {
        popup_HowToPlay.SetActive(false);
        for(int i = 0; i < images_HowToPlay.Length; i++) // ó���� �����־����
        {
            images_HowToPlay[i].SetActive(false);
        }
        btn_NextPage.SetActive(false);
        btn_PrevPage.SetActive(false);
        nowPage = 0;
        _Instance = this; // NULL ����
    }

    public void OpenHowToPlay() // ���� ��� â ����, ���� ȭ�� HowToPlayButton�� ����
    {
        popup_HowToPlay.SetActive(true);
        images_HowToPlay[0].SetActive(true);
        btn_NextPage.SetActive(true);
        btn_PrevPage.SetActive(true);
    } 

    public void CloseHowToPlay() // ���� ��� â �ݱ�
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
