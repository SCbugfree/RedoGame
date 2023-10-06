using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGameManager : MonoBehaviour
{
    public GameObject cloudPrefab;

    public int maxClouds;
    public int minClouds;

    public float timeToCloud;
    float timeToCLoudCounter;

    BoxCollider2D worldBounds;

    Vector3 minBounds;
    Vector3 maxBounds;

    List<GameObject> allClouds = new List<GameObject>(); //keep all clouds in a list

    void Start()
    {
        worldBounds = GetComponent<BoxCollider2D>();
        minBounds = worldBounds.bounds.min; //min and max are built-in variables
        maxBounds = worldBounds.bounds.max;
    }

    void Update()
    {
        timeToCLoudCounter += Time.deltaTime;

        if (timeToCLoudCounter > timeToCloud || allClouds.Count < minClouds)
        {
            if (allClouds.Count < maxClouds)
            {
                MakeACloud();
                timeToCLoudCounter = 0;
            }
        }
    }

    void MakeACloud() //create a new cloud prehab
    {
        GameObject newCloud = Instantiate(cloudPrefab);

        Vector3 newPos = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            0f
            );

        newCloud.transform.position = newPos;
        allClouds.Add(newCloud);
    }
}
