using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    static GameManager _Instance;
    GameManager() { }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    public static GameManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    #endregion

    #region 변수들
    public int stageNum; // 플레이어가 현재 도전해야 하는 스테이지
    public int stageTime = 0; // 경과 시간
    public int goal = 0; // 스테이지 목표 인원
    public int attend = 0; // 해당 스테이지에서 목표층에 도달한 인원
    public int floor = 0; // 현재 층
    int totalHuman = 0; // 정원 6명
    int totalWeight = 0; // 제한 250kg
    bool isSave; // 저장된 진행 상황이 있는지 확인

    public bool[] check_Place = new bool[(int)placing.MaxCount];
    public Vector3[] place = new Vector3[(int)placing.MaxCount];

    [SerializeField] GameObject stage;

    #endregion

    public enum placing
    {
        left, mid, right, leftUp, midUp, rightUp, leftDown, midDown, rightDown, MaxCount
    }

    void Start()
    {
        isSave = PlayerPrefs.HasKey("savedStage");
        if (!isSave)
        {
            stageNum = 1;
            Debug.Log($"현재 스테이지: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"현재 스테이지: {stageNum}");
        }

        for (int i = 0; i < (int)placing.MaxCount; i++)
        {
            check_Place[i] = false;
        }

        for (int i = 0; i < (int)placing.MaxCount; i++)
        {
            int index = i;
            float x = (index / 3 + 1) * 1.3f;
            float y = index / 3 * 2;
            place[i] = new Vector3(-0.8f + x, 2 - y, 10);
        }

        stage = Resources.Load<GameObject>($"Stage{stageNum}");
        Instantiate(stage);
    }

    public bool checkGoal() // 목표 인원에 도달했으면 true
    {
        return goal == attend;
    } 

    public void stageClear() // checkGoal 수행 후 true이면 반환, stageNum을 +1, 다음 스테이지, 게임 종료 버튼 출현
    {
        Instantiate(Resources.Load($"stageClear"), GameObject.Find("Info").transform);
        Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
        UIManager.Instance.disableElv();

        if (stageNum == 9)
            stageNum = 0;
        stageNum++;
        PlayerPrefs.SetInt("savedStage", stageNum);
        PlayerPrefs.Save();
    }

    public void stageFailed() // checkGoal 수행 후 true이면 반환, 스테이지 재시도 , 게임 종료 버튼 출현
    {
        Instantiate(Resources.Load($"stageFailed"), GameObject.Find("Info").transform);
        Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
        UIManager.Instance.disableElv();
    }

    #region 엘리베이터 코루틴
    int eSpeed = 1;
    IEnumerator UpFloor()
    {
        if (stageNum > 5)
            eSpeed = 2;

        yield return new WaitForSeconds(eSpeed);

        floor++;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }

    IEnumerator DownFloor()
    {
        int eSpeed = 1;
        if (stageNum > 5)
            eSpeed = 2;

        yield return new WaitForSeconds(eSpeed);

        floor--;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }

    public IEnumerator UpElevator(int f)
    {
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("doorclose");
        for (int i = floor; i < f; i++)
        {
                yield return StartCoroutine(UpFloor());
        }
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("dooropen");
        UIManager.Instance.enableElv();
    }

    public IEnumerator DownElevator(int f)
    {
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("doorclose");
        for (int i = floor; i > f; i--)
        {
            yield return StartCoroutine(DownFloor());
        }
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("dooropen");
        UIManager.Instance.enableElv();
    }
    #endregion
}