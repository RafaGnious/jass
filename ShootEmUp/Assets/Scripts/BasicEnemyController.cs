using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
public class BasicEnemyController : EnemyBaseClass
{
    #region Inspector Variables
    [Tooltip("The Object Pool that will handle this enemy's shooting")]
    [SerializeField]
    ObjectPool Cannon;

    [Tooltip("The delta time between the sprite changes on dead")]
    [SerializeField]
    float DeadAnimationDeltaTime;
    [Tooltip("The dead animation sprites, ordered")]
    [SerializeField]
    Sprite[] DeadAnimation;
    #endregion

    #region Overridden Properties
    protected override int ScoreOnKill
    {
        get
        {
            return 15;
        }
    }

    protected override float DamageOnCollision
    {
        get
        {
            return 30f;
        }
    } 
    #endregion

    /// <summary>
    /// The current frame's index on dead animation
    /// </summary>
    int DeadAnimationIndex;
    /// <summary>
    /// Time at which next sprite change should take place. If 0, means dead animation isn't running
    /// </summary>
    float TimeForNextAnimationFrame;

    new SpriteRenderer renderer;

    //Initializes
    protected override void SubclassAwake()
    {
        renderer = GetComponent<SpriteRenderer>();
        Health = 30;
    }

    /// <summary>
    /// Is the object waiting for the bullets to exit screen before destroying it self?
    /// </summary>
    bool isWaitingForPool = false;


    //Overridden to handle the Waiting For Pool and trigger the dead animation
    protected override void SelfDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        TimeForNextAnimationFrame = Time.time + DeadAnimationDeltaTime;
        renderer.sprite = DeadAnimation[0];
        Cannon.PoolOpened = false;
        GetComponent<AudioSource>().Play();
    }

    //Destroys when exiting the screen
    private void OnBecameInvisible()
    {
        if (!isWaitingForPool) base.SelfDestroy();
    }

    //Enables firing only when get's on screen
    private void OnBecameVisible()
    {
        Cannon.PoolOpened = true;
    }

    // Subclass Update is called once per frame
    protected override void SubclassUpdate()
    {
        //Handles the dead animation
        if (TimeForNextAnimationFrame > 0)
        {
            if (TimeForNextAnimationFrame <= Time.time)
            {
                //Ends dead animation case there are no frames left and starts Waiting For Pool
                if (DeadAnimationIndex == DeadAnimation.Length - 1)
                {
                    isWaitingForPool = true;
                    renderer.enabled = false;
                    TimeForNextAnimationFrame = 0;
                }
                //Switches to next frame
                else
                {
                    renderer.sprite = DeadAnimation[++DeadAnimationIndex];
                    TimeForNextAnimationFrame = Time.time + DeadAnimationDeltaTime;
                }
            }
        }
        else if (isWaitingForPool)
        {
            //Destroys this enemy case there are no bullets left on screen
            if (Cannon.PoolCount == 17) base.SelfDestroy();
        }
        else
        {
            //Works as a Constant Speed
            transform.position += new Vector3(Speed * Time.deltaTime, 0);
        }
    }
}
