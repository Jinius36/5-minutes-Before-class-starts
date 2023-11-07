using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    #region 싱글톤
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
    } // 다음 스테이지

    public void RetryStage() 
    {
        Destroy(GameObject.Find("stageFailed(Clone)"));
        UIManager.Instance.enableElv();
        Instantiate(Resources.Load($"Stage{GameManager.Instance.stageNum}"));
    } // 스테이지 재시도

    

    
}
