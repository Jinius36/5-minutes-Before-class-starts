using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Rand = UnityEngine.Random;

public class Stage1 : MonoBehaviour
{
    int[] howManyStudent = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    void Start()
    {
        GameManager.Instance.goal = 10;
        GameManager.Instance.limitTime = 60;
        for(int i = 0; i < 5; i++)
        {
            int s1 = Rand.Range(0, 2);
            int s2 = Rand.Range(0, 2);
            int nf = Rand.Range(0, 11);
            while(howManyStudent[nf]!=0)
                nf = Rand.Range(0, 11);
            howManyStudent[nf] += 2;
            int gf1 = Rand.Range(0, 11);
            int gf2 = Rand.Range(0, 11);
            while (gf1==nf)
                gf1 = Rand.Range(0, 11);
            while (gf2 == nf)
                gf2 = Rand.Range(0, 11);
            int op1 = Rand.Range(0, 3); int op2 = Rand.Range(0, 3);
            while (op1 == op2)
                op2 = Rand.Range(0, 3);
            GameManager.Instance.students.Add(GameManager.Instance.Spawn(s1, nf, gf1, op1));
            GameManager.Instance.students.Add(GameManager.Instance.Spawn(s2, nf, gf2, op2));
        }
        foreach(Tuple<GameObject, Student> student in GameManager.Instance.students)
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
