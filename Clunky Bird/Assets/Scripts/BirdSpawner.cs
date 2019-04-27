using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public Bird BirdPrefab;
    public float SpawnRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextBird(1f / SpawnRate));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected IEnumerator NextBird(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        var bird = Instantiate(BirdPrefab);
        bird.StartPoint = new Vector3(1 + Random.value * 4f, Random.value * 3f - 1f, Random.value > 0.5f ? 4f : -4f);
        bird.EndPoint = new Vector3(1 + Random.value * 4f, Random.value * 3f - 1f, -bird.StartPoint.z);
        bird.transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, bird.EndPoint - bird.StartPoint, Vector3.up), 0);
        bird.Duration = 3f + Random.value * 3f;
        bird.Height = Random.value * 2f;
        bird.transform.parent = transform;

        StartCoroutine(NextBird(1f / SpawnRate));
    }
}
