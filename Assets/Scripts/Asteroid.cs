using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyedParticles;

    [SerializeField] private AudioClip boom;

    public int size = 3;

    public GameManager gameManager;

    public void Start()
    {
        transform.localScale = 0.5f * size * Vector3.one;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(Random.value, Random.value).normalized;
        float spawnSpeed = Random.Range(4f - size, 5f - size);
        rb.AddForce(direction * spawnSpeed, ForceMode2D.Impulse);


        gameManager.asteroidCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            gameManager.asteroidCount--;
            gameManager.scoreUpdate();

            AudioSource ass = GetComponent<AudioSource>();


            Destroy(collision.gameObject);

            if (size > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Asteroid newAsteroid = Instantiate(this, transform.position, Quaternion.identity);
                    newAsteroid.size = size - 1;
                    newAsteroid.gameManager = gameManager;
                }
            }

            Instantiate(destroyedParticles, transform.position, Quaternion.identity);
            gameManager.asteroidBoom();

            Destroy(gameObject);
        }
    }
}
