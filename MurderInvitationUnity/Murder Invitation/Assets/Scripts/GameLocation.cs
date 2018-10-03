using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLocation : MonoBehaviour
{
    public Sprite nextObjectiveSprite;
    public SpriteRenderer objective;
    public Transform[] playerLocations;

    public void ChangeObjective()
    {
        objective.sprite = nextObjectiveSprite;
    }

    public void ClearObjective()
    {
        objective.enabled = false;
    }
}
