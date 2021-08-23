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
    Collider coll;

    bool hitIndicators;

    void Start()
    {
        GameObject obj = GameObject.Find("GrabSphere");
        var component = obj.GetComponent<HapticFeedback>();
        hitIndicators = component.showHitIndicators;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.name == "GrabSphere" && hitIndicators)
        {
            Destroy(collisionMarker);
            collisionPoint = other.GetContact(0).point;
            collisionMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coll = collisionMarker.GetComponent(typeof(Collider)) as Collider;
            coll.enabled = false;
            collisionMarker.transform.position = collisionPoint;
            collisionMarker.transform.localScale = collisionSphereSize;
        }
    }
}
