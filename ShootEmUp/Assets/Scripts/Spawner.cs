using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /// <summary>
    /// Due to the fact of the enemies not getting destroyed on scene load, this will tell them to destroy themselves if no spawner has spawned yet
    /// </summary>
    public static bool HasSpawnedYet { get; private set; }

    /// <summary>
    /// The object that this spawner spawns
    /// </summary>
    [Tooltip("The object that this spawner spawns")]
    public GameObject spawning;
    [Tooltip("The Equation that defines frequency in function of level, read about equations for more info")]
    [SerializeField]
    Equation Frequency;
    [Tooltip("A randomizing factor for the frequency. 1 will represent no randomization")]
    [SerializeField]
    float RandomFactor;
    [Tooltip("To prevent Spawning ships half outside the screen")]
    [SerializeField]
    float halfHeight;

    [Tooltip("To prevent ships from spawning all in the same zone of the screen the Spawner will divide the screen in zones and count how many spawn in one zone to spawn them elsewhere")]
    [SerializeField]
    int ZoneAmount;
    [Tooltip("The max spawns in a zone that can be")]
    [SerializeField]
    int MaxSpawnsPerZone;

    float TimeForNextSpawn;
    int SpawningThereFor;
    int LastSpawnedZone;

    void Awake()
    {
        //Sets the timer for next spawn
        float f = Frequency.GetValue(GameManager.instance.Level);
        TimeForNextSpawn = f + Random.Range(-RandomFactor * f, RandomFactor * f);
        //Resets HasSpawnedYet so enemies destroy themselves
        HasSpawnedYet = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.timeSinceLevelLoad >= TimeForNextSpawn)
        {

            #region Zone Picker
            if (!HasSpawnedYet) HasSpawnedYet = true;
            int Zone = Random.Range(1, ZoneAmount + 1);
            if (Zone == LastSpawnedZone)
            {
                if (SpawningThereFor >= MaxSpawnsPerZone)
                {
                    if (Zone > ZoneAmount / 2f) Zone = 1;
                    else Zone = ZoneAmount;
                }
                else SpawningThereFor++;
            }
            else
            {
                LastSpawnedZone = Zone;
                SpawningThereFor = 0;
            } 
            #endregion

            //Values that represent the respective zone of the screen
            float secondFactor = Zone / (float)ZoneAmount;
            float firstFactor = secondFactor - (1f / ZoneAmount);

            Vector3 SpawnPosition = GameManager.camera.ScreenToWorldPoint(new Vector3(GameManager.camera.pixelWidth, Random.Range(GameManager.camera.pixelHeight * firstFactor, GameManager.camera.pixelHeight * secondFactor)));
            SpawnPosition.y -= halfHeight * Mathf.Sign(SpawnPosition.y);
            Instantiate(spawning, SpawnPosition, Quaternion.Euler(0, 0, 0));

            //Resets time for next spawn
            float f = Frequency.GetValue(GameManager.instance.Level);
            TimeForNextSpawn += f + Random.Range(-RandomFactor * f, RandomFactor * f);
        }
    }
}
