using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToken : MonoBehaviour
{
    public Sprite deathSprite;
    public SpriteRenderer foregroundRenderer;
    public Vector3 TargetPosition { get; set; }
    const float Speed = 2;

    void Awake()
    {
        TargetPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, Speed * Time.deltaTime);
    }

    public void UpdateHp(int value)
    {
        if (value <= 0)
        {
            foregroundRenderer.sprite = deathSprite;
        }
    }
}
