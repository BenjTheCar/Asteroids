using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Asteroid asteroidPrefab;
    [SerializeField] private Text scoreText;

    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioSource audiosource;

    public int asteroidCount = 0;

    private int level = 0;
    private int score = 0;

    private void Update()
    {
        if (asteroidCount == 0)
        {
            level++;

            int numAsteroids = 2 + (2 * level);
            for (int i=0; i < numAsteroids; i++)
            {
                SpawnAsteroid();
            }
        }

        scoreText.text = score.ToString();
    }

    private void SpawnAsteroid()
    {
        float offset = Random.Range(0f, 1f);
        Vector2 viewportSpawnPosition = Vector2.zero;

        int edge = Random.Range(0, 4);
        if (edge == 0)
        {
            viewportSpawnPosition = new Vector2(offset, 0);
        } else if (edge == 1)
        {
            viewportSpawnPosition = new Vector2(offset, 1);
        } else if (edge == 2)
        {
            viewportSpawnPosition = new Vector2(offset, 2);
        } else if (edge == 3)
        {
            viewportSpawnPosition = new Vector2(offset, 3);
        }

        Vector2 worldSpawnPosition = Camera.main.ViewportToWorldPoint(viewportSpawnPosition);
        Asteroid asteroid = Instantiate(asteroidPrefab, worldSpawnPosition, Quaternion.identity);
        asteroid.gameManager = this;
    }

    public void scoreUpdate()
    {
        score++;
    }

    public void asteroidBoom()
    {
        audiosource.PlayOneShot(boom);
    }

    public void GameOver()
    {
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        Debug.Log("Game Over!");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }
}
