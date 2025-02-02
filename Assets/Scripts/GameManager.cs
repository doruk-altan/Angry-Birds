using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private IconHandler iconHandler;

    [SerializeField] private float timeBeforeDeathCheck = 3f;
    [SerializeField] private GameObject WinRestartScreenObj;
    [SerializeField] private GameObject LoseRestartScreenObj;
    [SerializeField] private SlingShotHandler slingShotHandler;

    private List<Piggy> piggies = new List<Piggy>();

    public int maxNumberOfShots = 3;

    private int usedNumberOfShots;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindAnyObjectByType<IconHandler>();

        Piggy[] piggyArr = FindObjectsOfType<Piggy>();

        for(int i = 0; i < piggyArr.Length; i++)
        {
            piggies.Add(piggyArr[i]);
        }
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
        
    }

    public bool HasEnoughShots()
    {
        if (usedNumberOfShots < maxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    {
        if(usedNumberOfShots == maxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(timeBeforeDeathCheck);

        //Have all piggies been destroyed
        if(piggies.Count == 0)
        {
            //win
            WinGame();
        }
        else
        {
            //lose
            LoseGame();
        }
    }

    public void RemovePiggy(Piggy piggy)
    {
        piggies.Remove(piggy);
        CheckForAllDeadPiggies();
    }

    private void CheckForAllDeadPiggies()
    {
        if (piggies.Count == 0)
        {
            //win
            WinGame();
        }
    }

    private void WinGame()
    {
        WinRestartScreenObj.SetActive(true);
        slingShotHandler.enabled = false;
    }

    private void LoseGame()
    {
        LoseRestartScreenObj.SetActive(true);
        slingShotHandler.enabled = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
