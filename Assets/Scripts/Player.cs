
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 moveinput;
    Vector2 screenBoundery;
    [SerializeField] int playerHealth = 4;
    [SerializeField] float invinsibleTime = 3f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotationSpeed = 700f;
    [SerializeField] float bulletSpeed = 7f;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject gun;

    bool invinsible;

    float targetAngle;
    float distanceToPlane;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        distanceToPlane = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        screenBoundery = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distanceToPlane));
    }

    void OnMove(InputValue value)
    {
        moveinput = value.Get<Vector2>();
    }

    void OnAttack()
    {
        Rigidbody2D playerBullet = Instantiate(bullet, gun.transform.position, transform.rotation).GetComponent<Rigidbody2D>();
        playerBullet.AddForce(transform.up * bulletSpeed, ForceMode2D.Impulse);
        Destroy(playerBullet.gameObject, 3f);
    }
    
    void Update()
    {
        rb.linearVelocity = moveinput * moveSpeed;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceToPlane));
        Vector2 aimDirection = (Vector2)mouseWorldPos - (Vector2)transform.position;
        targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -screenBoundery.x, screenBoundery.x),
            Mathf.Clamp(transform.position.y, -screenBoundery.y, screenBoundery.y)
        );
    }

    void FixedUpdate()
    {
        float rotation = Mathf.MoveTowardsAngle(rb.rotation, targetAngle - 90, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rotation);
    }

    void ResetInvinsibility()
    {
        invinsible = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies") && !invinsible)
        {
            if (playerHealth <= 1)
            {
                Destroy(gameObject);
            }
            else
            {
                playerHealth--;
                invinsible = true;
                Invoke("ResetInvinsibility", invinsibleTime);
                Debug.Log("Player Health: " + playerHealth);
            }
        }
    }
}