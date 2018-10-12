using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// The number of objects in the pool currently
    /// </summary>
    public int PoolCount { get { return Pool.Count; } }
    Queue<GameObject> Pool = new Queue<GameObject>();


    #region Inspector Variables
    public bool PoolOpened = true;

    [Tooltip("Actually represents period (1/rate)")]
    [SerializeField]
    /// <summary>
    /// Actually represents period (1/rate)
    /// </summary>
    float Rate;
    [SerializeField]
    float RandomizingFactor;

    [Tooltip("The transform parent containing the initial Pool")]
    [SerializeField]
    Transform StartPoolParent;



    [Tooltip("If not null, every time an object exits SoundEffect.Play() is called.")]
    [SerializeField]
    AudioSource SoundEffect;

    [Tooltip("Should the Pool ignore are and spawn at the right side of the screen instead?")]
    [SerializeField]
    bool ScreenRelative;

    [Tooltip("Spawn area")]
    [SerializeField]
    Rect Area;

    //For debug purposes, if not checked a warning will come if the pool gets empty
#if DEBUG
    [Tooltip("For debug purposes, if not checked a warning will come if the pool gets empty")]
    [SerializeField]
    bool PoolGetsEmpty;
#endif 
    #endregion

    /// <summary>
    /// The time at which next object shall leave the pool
    /// </summary>
    float NextSpawnTime;


    /// <summary>
    /// Manually add an object back to the pool
    /// </summary>
    /// <param name="poolObject">The object to add</param>
    public void Repool(GameObject poolObject)
    {
        if (poolObject.activeSelf) poolObject.SetActive(false);
        if (!Pool.Contains(poolObject)) Pool.Enqueue(poolObject);
    }

    void Awake()
    {
        //adds the children of StartPoolParent to the pool, if it isn't null
        if (StartPoolParent != null)
            foreach (Transform obj in StartPoolParent)
            {
                if (obj != StartPoolParent) Pool.Enqueue(obj.gameObject);
            }
    }

    /// <summary>
    /// Manually places an object as if it had left the pool
    /// </summary>
    /// <param name="pooling">The object to act on</param>
    public void PoolObject(GameObject pooling)
    {
        if (ScreenRelative)
        {
            //Gets a position at the right border of the screen
            Vector3 position = GameManager.camera.ScreenToWorldPoint(new Vector3(GameManager.camera.pixelWidth, Random.Range(0, GameManager.camera.pixelHeight)));
            pooling.transform.position = new Vector3(position.x, position.y);

            if (!pooling.activeSelf) pooling.SetActive(true);

            NextSpawnTime = Time.time + Rate + (Rate * Random.Range(-RandomizingFactor, RandomizingFactor));
        }
        else
        {
            pooling.transform.position = transform.position + new Vector3(Random.Range(Area.xMin, Area.xMax), Random.Range(Area.yMin, Area.yMax));
            if (!pooling.activeSelf) pooling.SetActive(true);
            NextSpawnTime = Time.time + Rate + (Rate * Random.Range(-RandomizingFactor, RandomizingFactor));
        }
        //Plays sound if SoundEffect isn't null
        if (SoundEffect != null)
        {
            SoundEffect.Play();
        }
    }




    // Update is called once per frame
    void Update()
    {
        if (PoolOpened && Time.time >= NextSpawnTime)
        {
            if (Pool.Count > 0)
            {
                PoolObject(Pool.Dequeue());

            }
            //Warns user about pool getting empty
#if DEBUG
            else
            {
                if (!PoolGetsEmpty) Debug.LogWarning("Pool is empty, please check the PoolGetsEmpty if it is intended. Object:" + gameObject.name);
            }
#endif
        }
    }
}
