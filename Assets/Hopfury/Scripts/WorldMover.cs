using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldMover : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}
