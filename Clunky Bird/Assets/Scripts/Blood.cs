using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Random.insideUnitSphere * 90f;
        rigidbody.velocity = rigidbody.velocity + Random.insideUnitSphere * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
