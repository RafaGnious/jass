using UnityEngine;

///<summary>
///Even though there is only one Enemy, the base class exists to make it easier in case of future enemy addition
///</summary>
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class EnemyBaseClass : MonoBehaviour
{
    protected abstract int ScoreOnKill { get; }

    public float Health = 50f;

    protected abstract float DamageOnCollision { get; }

    public float Speed;


    ///<summary>
    ///Destroys the Enemy from the top of the hierarchy
    ///</summary> 
    protected virtual void SelfDestroy()
    {
        Destroy(transform.GetMaxParent().gameObject);
    }



    void OnTriggerEnter2D(Collider2D collider)
    {
        //Detects Collision with Player's shots
        if (collider.CompareTag("FriendlyFire"))
        {
            Health -= ShipController.Damage;
            //Sends the bullet that collided back to the pool
            collider.gameObject.GetComponent<ConstantSpeed>().SelfRepool();
            //Handles Death
            if (Health <= 0)
            {
                GameManager.instance.Score += ScoreOnKill;
                SelfDestroy();
            }
        }
        //Detects Collision with Player
        else if (collider.CompareTag("Player"))
        {
            GameManager.instance.Score += ScoreOnKill;
            ShipController.Instance.TakeHealth(DamageOnCollision);
            SelfDestroy(); //Collision with player results on enemy's instant death, regardless of its health
        }
    }

    //General actions on Awake for all enemies
    protected void Awake()
    {
        SubclassAwake();
    }

    ///<summary>
    ///Use this instead of Awake on Subclasses
    ///</summary> 
    protected virtual void SubclassAwake() { }

    //General actions on Start for all enemies
    protected void Start()
    {
        if (!Spawner.HasSpawnedYet)
        {
            SelfDestroy();
        }
        SubclassStart();
    }

    ///<summary>
    ///Use this instead of Start on Subclasses
    ///</summary> 
    protected virtual void SubclassStart() { }

    //General actions on Update for all enemies
    protected void Update()
    {

        SubclassUpdate();
    }

    ///<summary>
    ///Use this instead of Update on Subclasses
    ///</summary> 
    protected virtual void SubclassUpdate() { }
}
