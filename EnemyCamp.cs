using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnValue
{
    public int lowerLimit;
    public int upperLimit;

    public SpawnValue(int lowerLimit, int upperLimit)
    {
        this.lowerLimit = lowerLimit;
        this.upperLimit = upperLimit;
    }
}

public class EnemyCamp : MonoBehaviour
{   
    public string campName;
    public List<GameObject> enemyPrefabs;
    public List<int> enemySpawnChance;
    public int maxEnemyAtOnce;
    public int spawnDelaySeconds;
    public float regionRadius;

    private Vector3 startingPoint;
    private List<SpawnValue> enemySpawnValues = new List<SpawnValue>();

    private int aliveEnemyCount;
    private float lastSpawnTime;

    // Start is called before the first frame update
    public void ConsumeObjectDestroyed()
    {
        aliveEnemyCount--;
        Debug.Log("A member of enemy camp " + campName + " has been killed! Alive enemy count: " + aliveEnemyCount);
    }

    public Vector3 GetPos()
    {
        return startingPoint;
    }

    Vector3 GetRandomPosition()
    {
        float startingX = startingPoint.x;
        float startingY = startingPoint.y;
        float startingZ = startingPoint.z;

        float randomX = Random.Range(startingX - regionRadius, startingX + regionRadius);
        float randomZ = Random.Range(startingZ - regionRadius, startingZ + regionRadius);
        float safeY = startingY + 2;

        return new Vector3(randomX, safeY, randomZ);
    }

    GameObject GetRandomObject()
    {
        int totalSpawnChance = 0;
        foreach(int i in enemySpawnChance){
            enemySpawnValues.Add(new SpawnValue(totalSpawnChance, totalSpawnChance + i - 1));
            totalSpawnChance += i;
        }

        int randomInt = Random.Range(0, totalSpawnChance-1);

        foreach(SpawnValue sp in enemySpawnValues){
            if(randomInt >= sp.lowerLimit && randomInt <= sp.upperLimit){
                return enemyPrefabs[enemySpawnValues.IndexOf(sp)]; 
            }
        }
        return null;
    }

    void Spawn()
    {
        GameObject spawned = GetRandomObject();
        spawned.GetComponent<Enemy>().SetCamp(gameObject.GetComponent<EnemyCamp>());
        spawned.GetComponent<Enemy>().lookAt = GameObject.Find("Main Camera").transform;

        Instantiate(spawned, GetRandomPosition(), Quaternion.Euler(transform.rotation.x, transform.rotation.y + Random.Range(0, 360), transform.rotation.z));
        
        aliveEnemyCount++;
        lastSpawnTime = Time.time;
    }

    void Start()
    {
        startingPoint = transform.position;
        aliveEnemyCount = 0;
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSpawnTime >= spawnDelaySeconds)
        {
            if(aliveEnemyCount < maxEnemyAtOnce)
            {
                Spawn();
                Debug.Log("Enemy camp " + campName + " has spawned a new enemy! Alive enemy count: " + aliveEnemyCount);
            }
        }
    }

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, regionRadius);
	}
}
