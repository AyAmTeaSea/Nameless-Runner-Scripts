using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionForcePower;

    // On collision casts overlapsphere, if the player is in the overlapsphere adds an explosion force to the player
    private void OnCollisionEnter(Collision other) 
    {   
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody hitRigidbody = collider.GetComponent<Rigidbody>();
            if (hitRigidbody != null)
            {
                hitRigidbody.AddExplosionForce(explosionForcePower, transform.position, explosionRadius, 0, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
