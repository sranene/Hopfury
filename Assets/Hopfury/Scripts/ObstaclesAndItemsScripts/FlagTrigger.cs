using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FlagTrigger : MonoBehaviour
{
    public Sprite flagDown;
    public Sprite flagUp1;
    public Sprite flagUp2;

    private SpriteRenderer spriteRenderer;
    private bool animateFlag = false;
    private float timer = 0f;
    public float switchInterval = 0.2f; // time between sprite switches

    void Start()
    {
        spriteRenderer = transform.Find("FlagSprite").GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = flagDown;
    }

    void Update()
    {
        if (animateFlag)
        {
            timer += Time.deltaTime;

            if (timer >= switchInterval)
            {
                timer = 0f;
                // Alternate between the two "up" sprites
                if (spriteRenderer.sprite == flagUp1)
                {
                    spriteRenderer.sprite = flagUp2;
                }
                else
                {
                    spriteRenderer.sprite = flagUp1;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            animateFlag = true;
            spriteRenderer.sprite = flagUp1;
        }
    }
}

