using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private MeshFilter filt;
    private MeshRenderer ren;
    private SphereCollider sph;

    Vector3 collisionPoint;
    GameObject collisionMarker;

    Vector3 collisionSphereSize = new Vector3(0.02f, 0.02f, 0.25f);
    Collider collider;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.name == "GrabSphere")
        {
            Destroy(collisionMarker);
            collisionPoint = other.GetContact(0).point;
            collisionMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            collider = collisionMarker.GetComponent(typeof(Collider)) as Collider;
            collider.enabled = false;
            collisionMarker.transform.position = collisionPoint;
            collisionMarker.transform.localScale = collisionSphereSize;
        }
    }
}
