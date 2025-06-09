using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private SceneControl scene;
    private AudioSource audioSource;

    void Start()
    {
        scene = GameObject.FindWithTag("Root").GetComponent<SceneControl>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            audioSource.Play();
            scene.hp--;
        }
    }
}
