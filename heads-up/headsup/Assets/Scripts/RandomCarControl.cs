using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCarControl : MonoBehaviour
{
    public float speed;
    private bool seen = false;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnBecameVisible()
    {
        seen = true;
    }

    void OnBecameInvisible()
    {
        if (seen)
        {
            Destroy(gameObject);
        }
    }
}
