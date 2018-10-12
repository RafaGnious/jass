using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class ShipController : MonoBehaviour
{
    /// <summary>
    /// The player instance on the Scene
    /// </summary>
    public static ShipController Instance;

    public const float Damage = 10f;

    #region Inspector Variables
    public float Speed;

    [SerializeField]
    RectTransform HealthBar;

    [SerializeField]
    float HealthRestoreRate;
    [SerializeField]
    float HealthRestoreCooldown;

    [SerializeField]
    float MaxHealth;

    [Tooltip("Sprite when moving up")]
    [SerializeField]
    Sprite Side;

    [Tooltip("Sprite when stationary")]
    [SerializeField]
    Sprite Normal;

    [SerializeField]
    ObjectPool FrontCannon;

    [Tooltip("Half height of the sprite, for movement limitation purposes")]
    [SerializeField]
    float halfHeight;
    #endregion

    /// <summary>
    /// Use TakeHealth(float damage) if you want to take health
    /// </summary>
    public float Health { get; private set; }

    bool movingToSide;

    new SpriteRenderer renderer;


    /// <summary>
    /// Time when health restore can resume
    /// </summary>
    float timeForNextRestore;

    /// <summary>
    /// Decreases Health by damage
    /// </summary>
    /// <param name="damage">The amount of health to take</param>
    public void TakeHealth(float damage)
    {
        timeForNextRestore = Time.time + HealthRestoreCooldown;
        Health -= damage;
        if (Health <= 0)
        {
            GameManager.GameOver();
        }
    }

    //Detects Collision with Enemy shots
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyFire"))
        {
            //Places the bullets back in its pool
            collider.gameObject.GetComponent<ConstantSpeed>().SelfRepool();
            TakeHealth(10f);
        }
    }

    //Initializes
    void Awake()
    {
        Instance = this;
        renderer = GetComponent<SpriteRenderer>();
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        #region Movement
        float verticalInput = Input.GetAxis("Vertical");
        //Checks if the game is running before setting input
        if (Time.timeScale != 0 && verticalInput != 0)
        {
            //Declares movement state
            if (!movingToSide)
            {
                renderer.sprite = Side;
                movingToSide = true;
            }
            Vector3 nextPosition = transform.position + new Vector3(0, verticalInput * Speed * Time.deltaTime, 0);
            //checks if NextPosition is inside the screen before moving
            Vector3 screenPosition = GameManager.camera.WorldToScreenPoint(nextPosition);
            if (0 <= screenPosition.y - halfHeight && GameManager.camera.pixelHeight >= screenPosition.y + halfHeight) transform.position = nextPosition;
            //Flips the sprite case the ship is moving down
            renderer.flipX = verticalInput < 0;
        }
        else if (movingToSide)
        {
            //Resets the sprite to normal
            renderer.sprite = Normal;
            movingToSide = false;
        }
        #endregion
        #region Shooting
        if (Time.timeScale > 0) FrontCannon.PoolOpened = Input.GetButton("Fire1");
        #endregion
        #region HealthRestore
        {
            if (Time.time >= timeForNextRestore && Health < MaxHealth)
            {
                //Calculates the health restore for this frame
                float nextHealth = Health + HealthRestoreRate * Time.deltaTime;
                if (nextHealth < MaxHealth) Health = nextHealth;
                else Health = MaxHealth;

                //Sets the HealthBar value
                HealthBar.localScale = new Vector3(Health / MaxHealth, 1, 1);
            }
            
        }
        #endregion

    }
}
