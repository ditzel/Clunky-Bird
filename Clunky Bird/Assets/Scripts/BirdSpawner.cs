using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public Bird BirdPrefab;
    protected GameManager GameManager;
    protected float SpawnRate;
    [HideInInspector]
    public int BirdsLeftToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawning(int birdCount, float birdsPerSecound)
    {
        BirdsLeftToSpawn = birdCount;
        SpawnRate = birdsPerSecound / 2f;
        StartCoroutine(NextBird(1f / SpawnRate));
    }

    protected IEnumerator NextBird(float seconds)
    {
        if (GameManager.InMenu)
            yield break;

        yield return new WaitForSeconds(seconds);

        BirdsLeftToSpawn--;
        var bird = Instantiate(BirdPrefab);
        bird.StartPoint = new Vector3(1 + Random.value * 4f, Random.value * 3f, Random.value > 0.5f ? 4f : -4f);
        bird.EndPoint = new Vector3(1 + Random.value * 4f, Random.value * 3f, -bird.StartPoint.z);
        bird.transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, bird.EndPoint - bird.StartPoint, Vector3.up), 0);
        bird.Duration = 3f + Random.value * 3f;
        bird.Height = Random.value * 1f;
        bird.transform.parent = transform;

        if (BirdsLeftToSpawn > 0)
            StartCoroutine(NextBird(Random.value * 1f / SpawnRate));
    }
}
