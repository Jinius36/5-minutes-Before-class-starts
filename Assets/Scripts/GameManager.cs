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
    #region �̱���
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

    #region ������
    public int stageNum; // �÷��̾ ���� �����ؾ� �ϴ� ��������
    public int stageTime = 0; // ��� �ð�
    public int goal = 0; // �������� ��ǥ �ο�
    public int attend = 0; // �ش� ������������ ��ǥ���� ������ �ο�
    public int floor = 0; // ���� ��
    //int totalHuman = 0; // ���� 6��
    public int totalWeight = 0; // ���� 250kg
    public int maxWeight = 0;
    bool isSave; // ����� ���� ��Ȳ�� �ִ��� Ȯ��

    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // ���� �л� ����, ������

    [SerializeField] GameObject stage; // ���� ��������

    public enum placing // ��ġ ��ǥ ������ ���Ѱ͵�
    {
        left, mid, right, leftUp, midUp, rightUp, leftDown, midDown, rightDown, MaxCount
    }
    public bool[] check_Place; // ���������� ���ο� ����
    public bool[,] check_Out; // �ܺ��� ������ ����
    public Vector3[] place; // ��ǥ

    GameObject door_Left; // ���������� ��
    GameObject door_Right;

    GameObject[] existenceMarks;
    #endregion

    void Start()
    {
        isSave = PlayerPrefs.HasKey("savedStage"); // �������� ������
        if (!isSave)
        {
            stageNum = 1;
            Debug.Log($"���� ��������: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"���� ��������: {stageNum}");
        }

        check_Place = new bool[(int)placing.MaxCount]; // �л��� ó�� ���� ��ġ �� ���������� ��ġ�� �л��� �ִ��� ����
        check_Out = new bool[11,3];                    // ���������� �ܺ� ��ġ�� �л��� �ִ��� ����
        place = new Vector3[(int)placing.MaxCount];    // ��ǥ ����

        door_Left = GameObject.Find("Door_Left"); // ���������� �� ����
        door_Right = GameObject.Find("Door_Right");

        for (int i = 0; i < (int)placing.MaxCount; i++) // false�� �ʱ�ȭ
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

        for (int i = 0; i < (int)placing.MaxCount; i++) // ��ǥ ����
        {
            int index = i;
            float x = (index % 3) * 1.15f;
            float y = index / 3 * 2.5f;
            place[i] = new Vector3(-0.75f + x, 1.1f - y, 10);
        }

        existenceMarks = new GameObject[11]; // ���� �л� ���� ���� ����ǥ, ��Ȱ������ �ʱ�ȭ
        for (int i = 0; i < 11; i++)
        {
            existenceMarks[i] = GameObject.Find($"Mark_{i}");
            existenceMarks[i].SetActive(false);
        }

        stage = Resources.Load<GameObject>($"Stage{stageNum}"); // �������� �ҷ�����
        Instantiate(stage);
    }

    public bool checkGoal() // ��ǥ �ο��� ���������� true
    {
        return goal == attend;
    } 

    //public void stageClear() // checkGoal ���� �� true�̸� ��ȯ, stageNum�� +1, ���� ��������, ���� ���� ��ư ����
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

    //public void stageFailed() // checkGoal ���� �� true�̸� ��ȯ, �������� ��õ� , ���� ���� ��ư ����
    //{
    //    Instantiate(Resources.Load($"stageFailed"), GameObject.Find("Info").transform);
    //    Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
    //    UIManager.Instance.disableElv();
    //}

    #region ���������� �� ���ݱ�
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

    #region ���������� �ڷ�ƾ
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

    #region �л� ����
    public Tuple<GameObject,Student> Spawn(int s, int w, int nf, int gf, int op) // �л� ��ȯ
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

    void ActiveStudents(int f) // �л� Ȱ��ȭ, ���������Ͱ� �� �̵��� �Ϸ��ϸ� ����
    {
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == f)
                student.Item1.SetActive(true);
        }
    }
    #endregion

    public void checkExistenceAll() // ��ü �� ������� �л� ���� �Ǵ��Ͽ� ����ǥ Ȱ��ȭ
    {
        for(int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i,1] || check_Out[i,2])
                existenceMarks[i].SetActive(true);
        }
    }

    public void checkExistence(int f) // ���� �� ��� ����
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2])
            existenceMarks[f].SetActive(false);
    }
}