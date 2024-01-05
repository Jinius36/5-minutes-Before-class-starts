using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Rand = UnityEngine.Random;

public class Stage : MonoBehaviour
{
    int[] howManyStudent = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int spawnFloor;
    int sex;
    int goalFloor;
    int orderPlace;

    void Start()
    {
        switch (GameManager.Instance.stageNum)
        {
            case 1:
                GameManager.Instance.goal = 8;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 8; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 1);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                } 
                break;

            case 2:
                GameManager.Instance.goal = 9;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 9; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 1);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 3:
                GameManager.Instance.goal = 10;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 10; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 4:
                GameManager.Instance.goal = 11;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 11; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 5:
                GameManager.Instance.goal = 12;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 9; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 6:
                GameManager.Instance.goal = 13;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 10; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 7:
                GameManager.Instance.goal = 14;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 11; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 8:
                GameManager.Instance.goal = 15;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 12; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;

            case 9:
                GameManager.Instance.goal = 15;
                GameManager.Instance.limitTime = 30;
                for (int i = 0; i < 13; i++)
                {
                    do
                    {
                        spawnFloor = Rand.Range(0, 11);
                    }
                    while (howManyStudent[spawnFloor] > 2);
                    howManyStudent[spawnFloor] += 1;

                    sex = Rand.Range(0, 2);

                    do
                    {
                        goalFloor = Rand.Range(0, 11);
                    }
                    while (goalFloor == spawnFloor);

                    do
                    {
                        orderPlace = Rand.Range(0, 3);
                    }
                    while (GameManager.Instance.check_Out[spawnFloor, orderPlace]);
                    GameManager.Instance.check_Out[spawnFloor, orderPlace] = true;

                    GameManager.Instance.students.Add(GameManager.Instance.Spawn(sex, spawnFloor, goalFloor, orderPlace));
                }
                break;
        }
        
        foreach (Tuple<GameObject, Student> student in GameManager.Instance.students)
        {
            if (student.Item2.nowFloor == 0)
            {
                student.Item1.SetActive(true);
            }
        }
        UIManager.Instance.SetGoalUI();
        GameManager.Instance.DoorOpen();
        GameManager.Instance.checkExistenceAll();
    }
}