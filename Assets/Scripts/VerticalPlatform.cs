using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    IEnumerator DropPlatform()
    {
        effector.rotationalOffset = 180f;
        yield return new WaitForSeconds(.3f);
        effector.rotationalOffset = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(DropPlatform());
        }
    }
}
