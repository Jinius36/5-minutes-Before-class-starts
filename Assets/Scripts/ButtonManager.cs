using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    #region �̱���
    static ButtonManager instance;
    ButtonManager() { }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static ButtonManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    public void NextStage() 
    {
        Destroy(GameObject.Find("stageClear(Clone)"));
        UIManager.Instance.enableElv();
        Instantiate(Resources.Load($"Stage{GameManager.Instance.stageNum}"));
    } // ���� ��������

    public void RetryStage() 
    {
        Destroy(GameObject.Find("stageFailed(Clone)"));
        UIManager.Instance.enableElv();
        Instantiate(Resources.Load($"Stage{GameManager.Instance.stageNum}"));
    } // �������� ��õ�

    

    
}
