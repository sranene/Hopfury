using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TittleAnimation : MonoBehaviour
{
    public float speed = 4f; // Velocidade do pulo
    public float height = 8f; // Altura do pulo
    private RectTransform rectTransform;
    private float originalY;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalY = rectTransform.localPosition.y;
    }

    void Update()
    {
        float newY = originalY + Mathf.Sin(Time.time * speed) * height;
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, newY, rectTransform.localPosition.z);
    }
}