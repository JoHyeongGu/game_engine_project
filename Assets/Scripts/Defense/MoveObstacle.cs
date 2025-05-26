using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    public Vector3 anotherPos;
    private Vector3 currentPos;
    private bool isInAnother = false;

    void Start()
    {
        currentPos = this.transform.localPosition;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            this.transform.localPosition = isInAnother ? currentPos : anotherPos;
            isInAnother = !isInAnother;
        }
    }
}