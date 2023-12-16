using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

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
    //int totalHuman = 0; // 정원 6명
    public int totalWeight = 0; // 제한 250kg
    public int maxWeight = 0;
    bool isSave; // 저장된 진행 상황이 있는지 확인

    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // 개별 학생 스폰, 관리용

    [SerializeField] GameObject stage; // 현재 스테이지

    public enum placing // 위치 좌표 설정을 위한것들
    {
        left, mid, right, leftUp, midUp, rightUp, leftDown, midDown, rightDown, MaxCount
    }
    public bool[] check_Place; // 엘리베이터 내부에 쓰임
    public bool[,] check_Out; // 외부의 층마다 쓰임
    public Vector3[] place; // 좌표

    GameObject door_Left; // 엘리베이터 문
    GameObject door_Right;

    GameObject[] existenceMarks;
    #endregion

    void Start()
    {
        isSave = PlayerPrefs.HasKey("savedStage"); // 스테이지 프리팹
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

        check_Place = new bool[(int)placing.MaxCount]; // 학생의 처음 생성 위치 및 엘리베이터 위치에 학생이 있는지 여부
        check_Out = new bool[11,3];                    // 엘리베이터 외부 위치에 학생이 있는지 여부
        place = new Vector3[(int)placing.MaxCount];    // 좌표 설정

        door_Left = GameObject.Find("Door_Left"); // 엘리베이터 문 설정
        door_Right = GameObject.Find("Door_Right");

        for (int i = 0; i < (int)placing.MaxCount; i++) // false로 초기화
        {
            check_Place[i] = false;
        }

        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                check_Out[i, j] = false;
            }
        }

        for (int i = 0; i < (int)placing.MaxCount; i++) // 좌표 설정
        {
            int index = i;
            float x = (index % 3) * 1.15f;
            float y = index / 3 * 2.5f;
            place[i] = new Vector3(-0.75f + x, 1.1f - y, 10);
        }

        existenceMarks = new GameObject[11]; // 층의 학생 존재 유무 느낌표, 비활성으로 초기화
        for (int i = 0; i < 11; i++)
        {
            existenceMarks[i] = GameObject.Find($"Mark_{i}");
            existenceMarks[i].SetActive(false);
        }

        stage = Resources.Load<GameObject>($"Stage{stageNum}"); // 스테이지 불러오기
        Instantiate(stage);
    }

    public bool checkGoal() // 목표 인원에 도달했으면 true
    {
        return goal == attend;
    } 

    //public void stageClear() // checkGoal 수행 후 true이면 반환, stageNum을 +1, 다음 스테이지, 게임 종료 버튼 출현
    //{
    //    Instantiate(Resources.Load($"stageClear"), GameObject.Find("Info").transform);
    //    Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
    //    UIManager.Instance.disableElv();

    //    if (stageNum == 9)
    //        stageNum = 0;
    //    stageNum++;
    //    PlayerPrefs.SetInt("savedStage", stageNum);
    //    PlayerPrefs.Save();
    //}

    //public void stageFailed() // checkGoal 수행 후 true이면 반환, 스테이지 재시도 , 게임 종료 버튼 출현
    //{
    //    Instantiate(Resources.Load($"stageFailed"), GameObject.Find("Info").transform);
    //    Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
    //    UIManager.Instance.disableElv();
    //}

    #region 엘리베이터 문 여닫기
    public void DoorClose()
    {
        door_Left.transform.DOMoveX(-0.458f, 1);
        door_Right.transform.DOMoveX(1.297f, 1);
    }

    public void DoorOpen()
    {
        door_Left.transform.DOMoveX(-2.278f, 1);
        door_Right.transform.DOMoveX(3.13f, 1);
    }
    #endregion

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
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("doorclose");
        foreach (Tuple<GameObject, Student> student in students)
        {
            if(student.Item2.nowFloor!=-1)
                student.Item1.SetActive(false);
        }
        for (int i = floor; i < f; i++)
        {
                yield return StartCoroutine(UpFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if(student.Item2.nowFloor==floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(eSpeed);
        ActiveStudents(f);
        DoorOpen();
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("dooropen");
        UIManager.Instance.enableElv(f);
    }

    public IEnumerator DownElevator(int f)
    {
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("doorclose");
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
                student.Item1.SetActive(false);
        }
        for (int i = floor; i > f; i--)
        {
            yield return StartCoroutine(DownFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(eSpeed);
        ActiveStudents(f);
        DoorOpen();
        UIManager.Instance.addStageTime(eSpeed);
        Debug.Log("dooropen");
        UIManager.Instance.enableElv(f);
    }
    #endregion

    #region 학생 관리
    public Tuple<GameObject,Student> Spawn(int s, int w, int nf, int gf, int op) // 학생 소환
    {
        GameObject studentObject = Instantiate(Resources.Load("Student")) as GameObject;
        Student studentComponent = studentObject.AddComponent<Student>();
        studentComponent.sex = s;
        studentComponent.weight = w;
        studentComponent.nowFloor = nf;
        studentComponent.goalFloor = gf;
        studentComponent.orderPlace = op;
        //studentComponent.activeTime = at;
        studentObject.transform.position = place[op];
        studentObject.SetActive(false);
        check_Out[nf, op] = true;
        return Tuple.Create(studentObject,studentComponent);
    }

    void ActiveStudents(int f) // 학생 활성화, 엘리베이터가 층 이동을 완료하면 실행
    {
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == f)
                student.Item1.SetActive(true);
        }
    }
    #endregion

    public void checkExistenceAll() // 전체 층 대상으로 학생 유무 판단하여 느낌표 활성화
    {
        for(int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i,1] || check_Out[i,2])
                existenceMarks[i].SetActive(true);
        }
    }

    public void checkExistence(int f) // 단일 층 대상 버전
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2])
            existenceMarks[f].SetActive(false);
    }
}