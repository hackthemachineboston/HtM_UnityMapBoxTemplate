using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeRotate : MonoBehaviour
{
    public Transform world;

    public float zMin = -80.0f;
    public float zMax = -300.0f;

    public Control control;

    private Vector3 startPoint = Vector3.zero;
    private Quaternion startRotation;
    bool moving = false;

    Vector3 baseLeftControllerPosition;
    Vector3 baseRightControllerPosition;

    double zoom = 0.0;
    bool zooming = false;

    void Update()
    {
        try
        {
            var z = Camera.main.transform.position.z;
            var d = -Input.GetAxis("Mouse ScrollWheel") * (z - zMin);
            var distanceMod = ((z - zMin) / zMax);

            bool vrEnabled = UnityEngine.VR.VRSettings.enabled;

            if (vrEnabled)
            {
                if (control.vrLeftControllerEvents.triggerPressed && control.vrRightControllerEvents.triggerPressed)
                {
                    if (!zooming)
                    {
                        baseLeftControllerPosition = control.vrLeftControllerEvents.transform.position;
                        baseRightControllerPosition = control.vrRightControllerEvents.transform.position;
                        zooming = true;
                    }

                    double baseDistance = (baseLeftControllerPosition - baseRightControllerPosition).magnitude;
                    double distance = (control.vrLeftControllerEvents.transform.position - control.vrRightControllerEvents.transform.position).magnitude;

                    double dd = (1.0 - (baseDistance / distance));

                    if (dd > 0.0)
                        dd = dd * 2.0;
                    if (dd < 0.0)
                    {
                        dd /= 2.0;
                        dd = -(dd * dd * dd * dd);
                    }
                    if (dd < -1.0)
                        dd = -1.0;

                    zoom = dd;
                }
                else
                    zooming = false;

                if (zooming)
                {
                    float zoomMod = (float)zoom * distanceMod * 10.0f;
                    d += zoomMod;
                }
            }

            if (vrEnabled)
                control.CameraRig.transform.localPosition += new Vector3(0.0f, 0.0f, d);
            else
                Camera.main.transform.localPosition += new Vector3(0.0f, 0.0f, d);

            if (vrEnabled)
            {
                if (control.vrRightControllerEvents != null && control.vrRightControllerEvents.triggerPressed)
                {
                    control.pointerRenderer.tracerVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
                    control.pointerRenderer.cursorVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
                }
                else
                {
                    control.pointerRenderer.tracerVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
                    control.pointerRenderer.cursorVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
                }
            }

            if (!zooming)
            {
                float spinFactor = 25.0f * distanceMod;

                RaycastHit hit;
                if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    startPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                    startRotation = world.localRotation;
                    moving = true;
                }
                else if (vrEnabled && !moving && control.vrRightControllerEvents != null && control.vrRightControllerEvents.triggerPressed)
                {
                    Vector3 h = control.pointer.transform.position + control.pointer.transform.forward * 100.0f;
                    startPoint = new Vector3(h.x, h.y, 100.0f);
                    startRotation = world.localRotation;
                    moving = true;
                }

                if (moving && Input.GetMouseButton(0))
                {
                    float v = Input.GetAxis("Mouse Y") * distanceMod * 0.75f * 5.0f;
                    transform.Rotate(v, 0, 0);

                    Vector3 pt = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                    pt = Camera.main.ScreenToWorldPoint(pt);
                    world.localRotation = Quaternion.AngleAxis((startPoint.x - pt.x) * spinFactor, Vector3.up) * startRotation;
                }
                else if (vrEnabled && moving && control.vrRightControllerEvents != null && control.vrRightControllerEvents.triggerPressed)
                {
                    Vector3 h = control.pointer.transform.position + control.pointer.transform.forward * 100.0f;
                    Vector3 pt = new Vector3(h.x, h.y, 100.0f);

                    float v = (startPoint.y - pt.y) * spinFactor / -15.0f * Mathf.Max(0.5f, distanceMod) * 0.75f * 2.5f;
                    transform.Rotate(v, 0, 0);

                    world.localRotation = Quaternion.AngleAxis((startPoint.x - pt.x) * spinFactor / 15.0f, Vector3.up) * startRotation;
                    startPoint.y = pt.y;
                }

                if (Input.GetMouseButtonUp(0) || (vrEnabled && control.vrRightControllerEvents != null && !control.vrRightControllerEvents.triggerPressed))
                    //if (Input.GetMouseButtonUp(0))
                    moving = false;
            }
        }
        catch (System.Exception e)
        {
        }
    }
}
