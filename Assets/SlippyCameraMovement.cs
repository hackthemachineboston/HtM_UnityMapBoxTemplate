namespace Mapbox.Examples
{
    using UnityEngine;

    public class SlippyCameraMovement : MonoBehaviour
    {
        [SerializeField]
        float _panSpeed = 20f;

        [SerializeField]
        float _zoomSpeed = 50f;

        [SerializeField]
        Camera _referenceCamera;

        Quaternion _originalRotation;
        Vector3 _origin;
        Vector3 _delta;
        bool _shouldDrag;

        public Control control;

        public GameObject CameraRig;

        private Vector3 vrStartPoint = Vector3.zero;
        bool vrDragging = false;
        bool vrRotating = false;

        Vector3 baseLeftControllerPosition;
        Vector3 baseRightControllerPosition;

        double zoom = 0.0;
        bool zooming = false;

        void Awake()
        {
            _originalRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            if (_referenceCamera == null)
            {
                _referenceCamera = GetComponent<Camera>();
                if (_referenceCamera == null)
                {
                    throw new System.Exception("You must have a reference camera assigned!");
                }
            }
        }

        void Update()
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
            {
                zooming = false;
            }

            var x = 0f;
            var y = 0f;
            var z = 0f;

            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = _referenceCamera.transform.localPosition.y;
                _delta = _referenceCamera.ScreenToWorldPoint(mousePosition) - _referenceCamera.transform.localPosition;
                _delta.y = 0f;
                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosition);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var offset = _origin - _delta;
                offset.y = transform.localPosition.y;
                transform.localPosition = offset;
            }
            else
            {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");
                y = -Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
                if (zooming)
                {
                    float zoomMod = (float)zoom * 10.0f;
                    y += zoomMod;
                }
                transform.localPosition += transform.forward * y + (_originalRotation * new Vector3(x * _panSpeed, 0, z * _panSpeed));
            }

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

            if (!zooming)
            {
                if (!vrDragging && control.vrRightControllerEvents != null && control.vrRightControllerEvents.triggerPressed)
                {
                    Vector3 h = control.vrRightControllerEvents.transform.position + control.vrRightControllerEvents.transform.forward * 100.0f;
                    vrStartPoint = new Vector3(h.x, h.y, 100.0f);
                    vrDragging = true;
                    //control.pointerRenderer.tracerVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
                }

                if (vrDragging && control.vrRightControllerEvents != null && control.vrRightControllerEvents.triggerPressed)
                {
                    Vector3 h = control.vrRightControllerEvents.transform.position + control.vrRightControllerEvents.transform.forward * 100.0f;
                    Vector3 pt = new Vector3(h.x, h.y, 100.0f);
                    float vrMod = 10.0f;

                    y = 0.0f;
                    var delta = transform.forward * y + (_originalRotation * new Vector3((vrStartPoint.x - pt.x) * _panSpeed / vrMod, 0, (vrStartPoint.y - pt.y) * _panSpeed / vrMod));
                    transform.localPosition += delta;

                    vrStartPoint = pt + delta;
                }

                if (control.vrRightControllerEvents != null && !control.vrRightControllerEvents.triggerPressed)
                {
                    vrDragging = false;
                    //control.pointerRenderer.gameObject.SetActive(true);
                    //control.pointerRenderer.tracerVisibility = VRTK.VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
                }

                //if (!vrRotating && control.vrRightControllerEvents != null && control.vrRightControllerEvents.gripPressed)
                //{
                //    Vector3 h = control.vrRightControllerEvents.transform.position + control.vrRightControllerEvents.transform.forward * 100.0f;
                //    vrStartPoint = new Vector3(h.x, h.y, 100.0f);
                //    vrRotating = true;
                //}

                //if (vrRotating && control.vrRightControllerEvents != null && control.vrRightControllerEvents.gripPressed)
                //{
                //    Vector3 h = control.vrRightControllerEvents.transform.position + control.vrRightControllerEvents.transform.forward * 100.0f;
                //    Vector3 pt = new Vector3(h.x, h.y, 100.0f);

                //    float vrMod = 0.05f;
                //    control.CameraRig.transform.localEulerAngles += new Vector3((vrStartPoint.y - pt.y) * vrMod, (vrStartPoint.x - pt.x) * vrMod * 0.0f, 0.0f);

                //    vrStartPoint = pt;
                //}

                //if (control.vrRightControllerEvents != null && !control.vrRightControllerEvents.gripPressed)
                //    vrRotating = false;
            }

            CameraRig.transform.localPosition = transform.localPosition;
        }
    }
}