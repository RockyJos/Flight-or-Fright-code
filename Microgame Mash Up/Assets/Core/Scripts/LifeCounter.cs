using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{
    private int lifeCount = 0;
    [SerializeField] private List<Animator> lifeHearts;


    public int GetLivesRemaining()
    {
        return lifeCount;
    }

    public void SetLives(int lives)
    {
        lifeCount = Mathf.Clamp(lives, 0, 10);
        UpdateLifeHearts();
    }

    public void AddLife()
    {
        lifeCount = Mathf.Min(10, lifeCount + 1);
        UpdateLifeHearts();
    }

    public void RemoveLife()
    {
        lifeCount = Mathf.Max(0, lifeCount - 1);
        UpdateLifeHearts();
    }

    private void UpdateLifeHearts()
    {
        StartCoroutine(UpdateLifeHeartsRoutine());
    }

    IEnumerator UpdateLifeHeartsRoutine()
    {
        for (int i = 0; i < lifeHearts.Count; i++)
        {
            if (lifeHearts[i].GetBool("Alive") != i < lifeCount)
            {
                lifeHearts[i].SetBool("Alive",i<lifeCount);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
