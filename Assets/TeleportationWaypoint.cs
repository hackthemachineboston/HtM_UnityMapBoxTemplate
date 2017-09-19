using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationWaypoint : MonoBehaviour
{
    public Control control;

    public float zMin = -80.0f;
    public float zMax = -300.0f;

    bool highlighted = false;
    float size = 1.0f;

    GlobeGeoPosition globeGeoPosition;

    ParticleSystem[] particleSystems;
    SphereCollider collider;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        globeGeoPosition = GetComponent<GlobeGeoPosition>();
        collider = gameObject.AddComponent<SphereCollider>();
        Set();
    }

    void Set()
    {
        for (int i = 0; i < particleSystems.Length; ++i)
        {
            var p = particleSystems[i];
            p.transform.localScale = new Vector3(size, size, 1.0f);
            var m = p.GetComponent<Renderer>().material;
            m.SetColor("_Color", highlighted ? new Color(0.4f, 1.0f, 0.4f, 0.4f) : new Color(0.8f, 0.8f, 1.0f, 0.4f));
            m.SetFloat("_Scale", 1.0f);
        }

        collider.radius = size * 1.0f;
    }

    void Update()
    {
        try
        {
            int layerMask = (1 << 8);

            RaycastHit hitObject;
            bool rayHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitObject, Mathf.Infinity, layerMask);

            if (!rayHit && control.pointer != null)
            {
                Ray ray = new Ray(control.pointer.transform.position, control.pointer.transform.forward);
                rayHit = Physics.Raycast(ray, out hitObject, Mathf.Infinity, layerMask);
            }

            highlighted = rayHit && (hitObject.collider == collider);

            if (highlighted && (Input.GetMouseButtonDown(0) || (control.vrRightControllerEvents != null && control.vrRightControllerEvents.touchpadPressed)))
                control.Teleport(globeGeoPosition.location);

            //Ray raycast = new Ray(laserPointer.transform.position, laserPointer.transform.forward);

            //RaycastHit hitObject;
            //bool rayHit = Physics.Raycast(raycast, out hitObject);

            //highlighted = rayHit && (hitObject.collider == collider);
            //if (!teleportedTo && highlighted && rightController.padPressed)
            //    planetController.Teleport(teleportLocation);

            var distanceMod = ((Camera.main.transform.position.z - zMin) / zMax);
            size = Mathf.Max(0.75f, Mathf.Min(distanceMod * 3.0f, 4.0f)) * 2.0f;

            Set();
        }
        catch (System.Exception e)
        {
        }
    }
}
