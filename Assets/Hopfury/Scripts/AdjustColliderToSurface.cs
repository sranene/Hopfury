using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AdjustColliderToSurface : MonoBehaviour
{
    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Transform surfaceTransform = transform.Find("Surface");

        if (surfaceTransform != null)
        {
            SpriteRenderer sr = surfaceTransform.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                float fixedHeight = 0.6f;
                collider.size = new Vector2(sr.size.x, fixedHeight);
                collider.offset = new Vector2(surfaceTransform.localPosition.x, 0f);
            }

        }
        else
        {
            Debug.LogWarning("surfaceTransform não encontrado");
        }
    }
}
