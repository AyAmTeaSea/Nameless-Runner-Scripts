using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectHit : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] [Range(0,5f)] float translateSpeed = 3f;
    [SerializeField] Transform objectToBeMoved;
    float translateProcess;
    Vector3 endPosition;

    // When the projectile pickup raycast hit an object it calls the HitByRay function
    // This function moves an object by the desired amount
    public void HitByRay()
    {
        translateProcess = 0f;
        endPosition = new Vector3(objectToBeMoved.position.x + movementVector.x, 
                                objectToBeMoved.position.y + movementVector.y, 
                                objectToBeMoved.position.z + movementVector.z);
        StartCoroutine(MoveObject());
    }

    // Lerp to create a more realistic movement on the object
    IEnumerator MoveObject()
    {
        while (translateProcess < 1)
        {
            translateProcess +=  translateSpeed * Time.deltaTime;
            objectToBeMoved.position = Vector3.Lerp(objectToBeMoved.position, endPosition, translateProcess);
            yield return new WaitForEndOfFrame();
        }
    }
}
