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
    HapticFeedback hapFeed;

    Vector3 collisionSphereSize = new Vector3(0.02f, 0.02f, 0.25f);
    Collider coll;

    bool hitIndicators;

    void Start()
    {
        GameObject obj = GameObject.Find("GrabSphere");
        hapFeed = obj.GetComponent<HapticFeedback>();
    }

    void OnCollisionEnter(Collision other)
    {
        hitIndicators = hapFeed.showHitIndicators; // not done in Start() to avoid race condition with HapticFeedback Start()/getIntent()

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
