using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHitParticle : MonoBehaviour
{
    //This script is attached to the hit particle game object that is located in Resources/HitParticle.
    //When the ball hits left or right barrier of the platform the hit particle will be instantiated on the scene
    void Start()
    {
        Destroy(this.gameObject, 0.2f);//Destroy the particle from the scene in 0.2 seconds
    }
}
