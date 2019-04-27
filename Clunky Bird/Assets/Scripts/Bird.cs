using DitzeGames.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    /// <summary>
    /// from 0 to 1 => highpoint 1
    /// </summary>
    public float Trajectory(float x) => -4f * Mathf.Pow(x - 0.5f, 2f) + 1f;

    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public float Height;
    public float Duration;
    protected float CurrentDuration;
    protected SkinnedMeshRenderer Mesh;
    protected Collider Collider;
    protected ParticleSystem Partical;
    protected Blood[] Bloods;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        Collider = GetComponentInChildren<Collider>();
        Partical = GetComponentInChildren<ParticleSystem>();
        Bloods = GetComponentsInChildren<Blood>();
        for (int i = 0; i < Bloods.Length; i++)
            Bloods[i].gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentDuration += Time.deltaTime;
        transform.position = Vector3.Lerp(StartPoint, EndPoint, CurrentDuration / Duration) + Height * Vector3.up * Trajectory(CurrentDuration / Duration);
        if (CurrentDuration > Duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider.enabled = false;
            Mesh.enabled = false;
            Partical.Play();
            for (int i = 0; i < Bloods.Length; i++)
                Bloods[i].gameObject.SetActive(true);
            CameraEffects.ShakeOnce(0.5f);
        }
    }
}
