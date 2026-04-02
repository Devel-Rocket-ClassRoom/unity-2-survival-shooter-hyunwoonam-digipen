using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameManager gameManager;

    public Zombie[] prefabs;

    public Transform[] spawnPoints;

    //public UIManager uIManager;

    private List<Zombie> zombies = new List<Zombie>();

    private int wave = 0;

    private float lastWaveTime;
    public float timeBetWave = 30.0f;

    private int activeSpawnCoroutines = 0;

    private void SpawnWave()
    {
        //wave++;
        //int count = Mathf.RoundToInt(wave * 1.5f);
        //for (int i = 0; i < count; i++)
        //{
        //    CreateZombunny();
        //    CreateZombear();
        //    CreateHellephant();
        //}

        wave++;

        int bunnyCount = Mathf.RoundToInt(wave * 2.0f);      
        int bearCount = Mathf.RoundToInt(wave * 1.0f);       
        int hellephantCount = Mathf.RoundToInt(wave * 0.5f); 

        activeSpawnCoroutines = 0;

        if (bunnyCount > 0)
        {
            StartCoroutine(SpawnZombieRoutine(0, bunnyCount, timeBetWave));
            activeSpawnCoroutines++;
        }

        if (bearCount > 0)
        {
            StartCoroutine(SpawnZombieRoutine(1, bearCount, timeBetWave));
            activeSpawnCoroutines++;
        }

        if (hellephantCount > 0)
        {
            StartCoroutine(SpawnZombieRoutine(2, hellephantCount, timeBetWave));
            activeSpawnCoroutines++;
        }
    }

    private IEnumerator SpawnZombieRoutine(int typeIndex, int spawnCount, float waveDuration)
    {
        float spawnInterval = waveDuration / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie(typeIndex);

            yield return new WaitForSeconds(spawnInterval);
        }

        activeSpawnCoroutines--;
    }

    private void CreateZombie(int index)
    {
        var point = spawnPoints[index];
        var zombie = Instantiate(prefabs[index], point.position, point.rotation);

        zombie.Setup();
        zombies.Add(zombie);
        zombie.gameObject.SetActive(true);

        zombie.OnDead.AddListener(() => gameManager.AddScore(zombie.Score));
        zombie.OnDead.AddListener(() => zombies.Remove(zombie));
        //zombie.OnDead.AddListener(() => uIManager.SetWaveInfo(wave, zombies.Count));
        zombie.OnDead.AddListener(() => Destroy(zombie.gameObject, 5f));
    }

    //private void CreateZombunny()
    //{
    //    var point = spawnPoints[0];
    //    var zombie = Instantiate(prefabs[0], point.position, point.rotation);
    //    zombie.Setup();
    //    zombies.Add(zombie);
    //
    //    zombie.gameObject.SetActive(true);
    //
    //    zombie.OnDead.AddListener(() => zombies.Remove(zombie));
    //    //zombie.OnDead.AddListener(() => gameManager.AddScore(100));
    //    //zombie.OnDead.AddListener(() => uIManager.SetWaveInfo(wave, zombies.Count));
    //    zombie.OnDead.AddListener(() => Destroy(zombie.gameObject, 5f));
    //}
    //
    //private void CreateZombear()
    //{
    //    var point = spawnPoints[1];
    //    var zombie = Instantiate(prefabs[1], point.position, point.rotation);
    //    zombie.Setup();
    //    zombies.Add(zombie);
    //
    //    zombie.gameObject.SetActive(true);
    //
    //    zombie.OnDead.AddListener(() => zombies.Remove(zombie));
    //    //zombie.OnDead.AddListener(() => gameManager.AddScore(100));
    //    //zombie.OnDead.AddListener(() => uIManager.SetWaveInfo(wave, zombies.Count));
    //    zombie.OnDead.AddListener(() => Destroy(zombie.gameObject, 5f));
    //}
    //
    //
    //private void CreateHellephant()
    //{
    //    var point = spawnPoints[2];
    //    var zombie = Instantiate(prefabs[2], point.position, point.rotation);
    //    zombie.Setup();
    //    zombies.Add(zombie);
    //
    //    zombie.gameObject.SetActive(true);
    //
    //    zombie.OnDead.AddListener(() => zombies.Remove(zombie));
    //    //zombie.OnDead.AddListener(() => gameManager.AddScore(100));
    //    //zombie.OnDead.AddListener(() => uIManager.SetWaveInfo(wave, zombies.Count));
    //    zombie.OnDead.AddListener(() => Destroy(zombie.gameObject, 5f));
    //}


    // Update is called once per frame
    void Update()
    {
        //uIManager.SetWaveInfo(wave, zombies.Count);
        if (activeSpawnCoroutines > 0) 
            return;

        if (Time.time > lastWaveTime + timeBetWave || zombies.Count == 0)
        {
            lastWaveTime = Time.time;

            SpawnWave();
        }
    }

}
