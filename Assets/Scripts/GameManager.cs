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
            _Instance = this;
    }
    public static GameManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    #endregion

    public int stageNum; // �÷��̾ ���� �����ؾ� �ϴ� ��������
    public int stageTime; // ��� �ð�
    public int limitTime; // ���� �ð�
    public int goal = 0; // �������� ��ǥ �ο�
    public int attend = 0; // �ش� ������������ ��ǥ���� ������ �ο�
    public int floor = 0; // ���� ��
    bool isSave; // ����� ���� ��Ȳ�� �ִ��� Ȯ��
    [SerializeField] GameObject stage; // ���� ��������

    public enum placing // ��ġ ��ǥ ������ ���Ѱ͵�
    {
        left, mid, right, leftUp, midUp, rightUp, leftDown, midDown, rightDown, MaxCount
    }
    public bool[] check_Place; // ���������� ���ο� ����
    public bool[,] check_Out; // �ܺ��� ������ ����
    public Vector3[] place; // ��ǥ

    #region ����
    public enum soundList
    {
        StudentMove, ElevatorMove, Arrive, Button, ElvButton, Clear, Fail, MaxCount
    }
    public AudioClip[] sounds;
    AudioSource effectSound;
    AudioSource buttonSound;
    public void PlaySound(AudioClip clip)
    {
        effectSound.clip = clip;
        effectSound.Play();
    }
    public void PlaySound2(AudioClip clip)
    {
        buttonSound.clip = clip;
        buttonSound.Play();
    }
    #endregion

    #region ����,����
    public Button retry_BTN;
    public Button next_BTN;
    public Button exit_BTN_Clear;
    public Button exit_BTN_Failed;
    public GameObject clearScreen;
    public GameObject failedScreen;
    public TextMeshProUGUI clearText;
    public TextMeshProUGUI failedText;
    public bool checkGoal() // ��ǥ �ο��� ���������� true
    {
        return goal == attend;
    }
    public void CallClear()
    {
        StageClear();
    }
    void StageClear() // checkGoal ���� �� true�̸� ��ȯ, stageNum�� +1, ���� ��������, ���� ���� ��ư ����
    {
        PlaySound(sounds[(int)soundList.Clear]);
        if (stageNum == 9)
            stageNum = 1;
        else
            stageNum++;
        PlayerPrefs.SetInt("savedStage", stageNum);
        PlayerPrefs.Save();
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
            Destroy(student.Item1);
        clearScreen.SetActive(true);
        next_BTN.gameObject.SetActive(true);
        exit_BTN_Clear.gameObject.SetActive(true);
        clearText.text = $"��ǥ: {goal}\n�޼�: {attend}";
    }
    public void CallFailed()
    {
        StageFailed();
    }
    void StageFailed() // ���ѽð� ��� �� ����, �������� ��õ� �� ���� ���� ��ư ����
    {
        PlaySound(sounds[(int)soundList.Fail]);
        UIManager.Instance.disableElv();
        foreach (Tuple<GameObject, Student> student in this.students)
            Destroy(student.Item1);
        failedScreen.SetActive(true);
        retry_BTN.gameObject.SetActive(true);
        exit_BTN_Failed.gameObject.SetActive(true);
        failedText.text = $"��ǥ: {goal}\n�޼�: {attend}";
    }
    #endregion

    #region ����������
    GameObject door_Left; // ���������� ��
    GameObject door_Right;
    SpriteRenderer background; // ���
    Sprite[] backgrounds;
    Image arrow;
    Sprite[] arrowMotion = new Sprite[8];
    int arrowNum = 1;
    int eSpeed = 1;
    public void DoorClose()
    {
        door_Left.transform.DOMoveX(-0.458f, 0.5f);
        door_Right.transform.DOMoveX(1.297f, 0.5f);
    }
    public void DoorOpen()
    {
        door_Left.transform.DOMoveX(-2.278f, 0.5f);
        door_Right.transform.DOMoveX(3.13f, 0.5f);
    }
    IEnumerator ArrowUping(int f)
    {
        arrowNum = 0;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[0];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 3)
                arrowNum = 0;
            yield return StartCoroutine(ArrowUp());
        }
        arrowNum = 0;
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowUp()
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum];
    }
    IEnumerator ArrowDowning(int f)
    {
        arrowNum = 0;
        arrow.gameObject.SetActive(true);
        arrow.sprite = arrowMotion[4];
        while (floor != f)
        {
            arrowNum++;
            if (arrowNum > 3)
                arrowNum = 0;
            yield return StartCoroutine(ArrowDown());
        }
        arrowNum = 0;
        arrow.gameObject.SetActive(false);
    }
    IEnumerator ArrowDown()
    {
        yield return new WaitForSeconds(0.3f);
        arrow.sprite = arrowMotion[arrowNum + 4];
    }
    IEnumerator UpFloor()
    {
        yield return new WaitForSeconds(0.5f);
        floor++;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    IEnumerator DownFloor()
    {
        yield return new WaitForSeconds(0.5f);
        floor--;
        UIManager.Instance.changeFloor();
        UIManager.Instance.addStageTime(eSpeed);
    }
    public IEnumerator UpElevator(int f)
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
                student.Item1.SetActive(false);
        }
        StartCoroutine(ArrowUping(f));
        for (int i = floor; i < f; i++)
        {
            yield return StartCoroutine(UpFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        PlaySound(sounds[(int)soundList.Arrive]);
        ActiveStudents(f);
        ChangeBack(f);
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
            CallFailed();
    }
    public IEnumerator DownElevator(int f)
    {
        Student.isElvMoving = true;
        DoorClose();
        yield return new WaitForSeconds(eSpeed);
        //PlaySound(sounds[(int)soundList.ElevatorMove]);
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor != -1)
                student.Item1.SetActive(false);
        }
        StartCoroutine(ArrowDowning(f));
        for (int i = floor; i > f; i--)
        { 
            yield return StartCoroutine(DownFloor());
        }
        foreach (Tuple<GameObject, Student> student in students)
        {
            if (student.Item2.nowFloor == floor)
                student.Item1.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        PlaySound(sounds[(int)soundList.Arrive]);
        ActiveStudents(f);
        ChangeBack(f);
        DoorOpen();
        UIManager.Instance.enableElv(f);
        Student.isElvMoving = false;
        if (stageTime >= limitTime)
            CallFailed();
    }
    void ChangeBack(int f)
    {
        background.sprite = backgrounds[f];
    }
    #endregion

    #region �л� ����
    public List<Tuple<GameObject, Student>> students = new List<Tuple<GameObject, Student>>(); // ���� �л� ����, ������
    public Tuple<GameObject, Student> Spawn(int sex, int spawnFloor, int goalFloor, int orderPlace) // �л� ��ȯ
    {
        GameObject studentObject = Instantiate(Resources.Load("Student")) as GameObject;
        Student studentComponent = studentObject.AddComponent<Student>();
        studentComponent.sex = sex;
        studentComponent.nowFloor = spawnFloor;
        studentComponent.goalFloor = goalFloor;
        studentComponent.orderPlace = orderPlace;
        studentObject.transform.position = place[orderPlace];
        studentObject.SetActive(false);
        return Tuple.Create(studentObject, studentComponent);
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

    #region ����ǥ ����
    GameObject[] existenceMarks;
    public void checkExistenceAll() // ��ü �� ������� �л� ���� �Ǵ��Ͽ� ����ǥ Ȱ��ȭ
    {
        for (int i = 0; i < 11; i++)
        {
            if (check_Out[i, 0] || check_Out[i, 1] || check_Out[i, 2])
                existenceMarks[i].SetActive(true);
        }
    }
    public void checkExistence(int f) // ���� �� ��� ����
    {
        if (!check_Out[f, 0] && !check_Out[f, 1] && !check_Out[f, 2])
            existenceMarks[f].SetActive(false);
    }
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

        arrow = GameObject.Find("Arrow").GetComponent<Image>();
        arrow.gameObject.SetActive(false);
        for( int i = 0; i < 4; i++)
        {
            arrowMotion[i] = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_Up_{i}");
        }
        for (int i = 4; i < 8; i++)
        {
            arrowMotion[i] = Resources.Load<Sprite>($"Sprites/Pannel_Images/Pannel_Down_{i-4}");
        }

        effectSound = gameObject.AddComponent<AudioSource>();
        buttonSound = gameObject.AddComponent<AudioSource>();
        sounds = new AudioClip[(int)soundList.MaxCount];
        for (int i = 0; i < (int)soundList.MaxCount; i++)
        {
            soundList soundName = (soundList)i;
            sounds[i] = Resources.Load<AudioClip>($"Sound/{soundName.ToString()}");
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
            place[i] = new Vector3(-0.75f + x, 1.5f - y, 10);
        }

        existenceMarks = new GameObject[11]; // ���� �л� ���� ���� ����ǥ, ��Ȱ������ �ʱ�ȭ
        for (int i = 0; i < 11; i++)
        {
            existenceMarks[i] = GameObject.Find($"Mark_{i}");
            existenceMarks[i].SetActive(false);
        }

        background=GameObject.Find("Background").GetComponent<SpriteRenderer>();
        backgrounds=new Sprite[11];
        for (int i = 0;i < 11; i++)
        {
            backgrounds[i] = Resources.Load<Sprite>($"Sprites/Background_Images/Back{i}");
        }

        stage = Resources.Load<GameObject>($"Stage"); // �������� �ҷ�����
        Instantiate(stage);
    }
}