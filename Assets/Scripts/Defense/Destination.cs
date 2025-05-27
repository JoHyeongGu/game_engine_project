using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            SceneControl scene = GameObject.FindWithTag("Root").GetComponent<SceneControl>();
            if (scene.stepTimer >= 0.1f) scene.hp--;
        }
    }
}
