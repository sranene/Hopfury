using UnityEngine;

public class Lava : MonoBehaviour
{
    private int layer = 2;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = layer;
    }
}
