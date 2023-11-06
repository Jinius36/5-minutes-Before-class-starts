using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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
    int totalHuman = 0; // ���� 6��
    int totalWeight = 0; // ���� 250kg
    bool isSave; // ����� ���� ��Ȳ�� �ִ��� Ȯ��

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
            Debug.Log($"���� ��������: {stageNum}");
        }
        else
        {
            stageNum = PlayerPrefs.GetInt("savedStage");
            Debug.Log($"���� ��������: {stageNum}");
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

    public bool checkGoal() // ��ǥ �ο��� ���������� true
    {
        return goal == attend;
    } 

    public void stageClear() // checkGoal ���� �� true�̸� ��ȯ, stageNum�� +1, ���� ��������, ���� ���� ��ư ����
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

    public void stageFailed() // checkGoal ���� �� true�̸� ��ȯ, �������� ��õ� , ���� ���� ��ư ����
    {
        Instantiate(Resources.Load($"stageFailed"), GameObject.Find("Info").transform);
        Destroy(GameObject.Find($"Stage{stageNum}(Clone)"));
        UIManager.Instance.disableElv();
    }

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