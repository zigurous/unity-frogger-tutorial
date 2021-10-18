using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Frogger frogger;
    private Home[] homes;

    public GameObject gameOverMenu;
    public Text timeText;
    public Text livesText;
    public Text scoreText;

    private int lives;
    private int score;
    private int time;

    private Vector3 spawnPosition;
    private float farthestRow;
    private float respawnTime;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        homes = FindObjectsOfType<Home>();
        frogger = FindObjectOfType<Frogger>();

        spawnPosition = frogger.transform.position;
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        gameOverMenu.SetActive(false);
        frogger.gameObject.SetActive(true);

        SetScore(0);
        SetLives(3);
        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < homes.Length; i++) {
            homes[i].enabled = false;
        }

        NewRound();
    }

    public void NewRound()
    {
        farthestRow = spawnPosition.y;

        Respawn();
    }

    private void Respawn()
    {
        frogger.Respawn(spawnPosition);

        StopAllCoroutines();
        StartCoroutine(Timer(30));
    }

    private IEnumerator Timer(int duration)
    {
        time = duration;
        timeText.text = time.ToString();

        while (time > 0)
        {
            yield return new WaitForSeconds(1);

            time--;
            timeText.text = time.ToString();
        }

        frogger.Death();
    }

    public void Died()
    {
        SetLives(lives - 1);

        if (lives > 0) {
            Invoke(nameof(Respawn), 1f);
        } else {
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    }

    private IEnumerator CheckForPlayAgain()
    {
        bool playAgain = false;

        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return)) {
                playAgain = true;
            }

            yield return null;
        }

        NewGame();
    }

    public void FroggerMoved(Vector3 destination)
    {
        if (destination.y > farthestRow)
        {
            farthestRow = destination.y;
            SetScore(score + 10);
        }
    }

    public void HomeReached()
    {
        frogger.gameObject.SetActive(false);

        int bonusPoints = time * 20;
        SetScore(score + bonusPoints + 50);

        if (Cleared())
        {
            SetLives(lives + 1);
            SetScore(score + 1000);
            Invoke(nameof(NewLevel), 1f);
        }
        else {
            Invoke(nameof(NewRound), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < homes.Length; i++)
        {
            if (!homes[i].enabled) {
                return false;
            }
        }

        return true;
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = this.score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = this.lives.ToString();
    }

}
