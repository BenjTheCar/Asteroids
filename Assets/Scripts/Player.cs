using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Ship Parameters")]
    [SerializeField] private float shipAcceleration = 10f;
    [SerializeField] private float shipMaxVelocity = 10f;
    [SerializeField] private float shipRotationSpeed = 180f;
    [SerializeField] private float bulletSpeed = 8f;

    [Header("Object References")]
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private ParticleSystem destroyedParticles;
    [SerializeField] public ParticleSystem thrustParticles;
    [SerializeField] private AudioClip pew;
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioSource audiosource;

    private Rigidbody2D shipRigidbody;
    private bool isAlive = true;
    private bool isAccelerating = false;

    public InputActionReference forward;
    public InputActionReference left;
    public InputActionReference right;
    public InputActionReference shoot;

    public int shootdbTime = 0;
    public bool shootDB = false;
    public int editorshootdbTime;

    private void Start() {
        shipRigidbody = GetComponent<Rigidbody2D>();
        thrustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Update()
    {
        if (isAlive)
        {
            HandleShipAcceleration();
            HandleShipRotation();
            HandleShooting();
        }

        if (!isAccelerating)
        {
            thrustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void FixedUpdate() {
        if (isAlive && isAccelerating) {
            shipRigidbody.AddForce(shipAcceleration * transform.up);
            shipRigidbody.linearVelocity = Vector2.ClampMagnitude(shipRigidbody.linearVelocity, shipMaxVelocity);
            thrustParticles.Play();
        }

        if (shootDB == true)
        {
            if (shootdbTime >= editorshootdbTime)
            {
                shootDB = false;
            }
            shootdbTime++;
        }
    }
    private void HandleShipAcceleration() {
        isAccelerating = forward.action.IsPressed();
    }

    private void HandleShipRotation() {
        if (right.action.IsPressed()) {
            transform.Rotate(-shipRotationSpeed * Time.deltaTime * transform.forward);
        } else if (left.action.IsPressed()) {
            transform.Rotate(shipRotationSpeed * Time.deltaTime * transform.forward);
        }
    }

    private void HandleShooting()
    {
        if (shoot.action.IsPressed())
        {
            if (shootDB == false)
            {
                Rigidbody2D bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
                audiosource.clip = pew;
                audiosource.PlayOneShot(pew);

                Vector2 shipVelocity = shipRigidbody.linearVelocity;
                Vector2 shipDirection = transform.up;
                float shipForwardSpeed = Vector2.Dot(shipVelocity, shipDirection);

                if (shipForwardSpeed < 0)
                {
                    shipForwardSpeed = 0;
                }

                bullet.linearVelocity = shipDirection * shipForwardSpeed;

                bullet.AddForce(bulletSpeed * transform.up, ForceMode2D.Impulse);

                shootDB = true;
                shootdbTime = 0;
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            isAlive = false;

            GameManager gameManager = FindAnyObjectByType<GameManager>();

            gameManager.GameOver();

            Instantiate(destroyedParticles, transform.position, Quaternion.identity);
            audiosource.PlayOneShot(boom);

            Destroy(gameObject);
        }
    }
}
