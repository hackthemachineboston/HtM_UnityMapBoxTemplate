using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;

public class Control : MonoBehaviour
{
    public bool vrEnabled = true;

    public GameObject Globe;
    public GameObject Slippy;

    public List<GameObject> Globes;
    public List<GameObject> SlippyMaps;

    public Camera GlobeCamera;
    public Camera SlippyMapCamera;
    public GameObject CameraRig;

    public Dropdown styleDropdown;
    public Dropdown vrStyleDropdown;
    public Button vrGlobeButton;

    public VRTK.VRTK_Pointer pointer;
    public VRTK.VRTK_StraightPointerRenderer pointerRenderer;
    public VRTK.VRTK_ControllerEvents vrLeftControllerEvents;
    public VRTK.VRTK_ControllerEvents vrRightControllerEvents;

    private bool showingGlobe = true;
    private Vector3 previousGlobePosition;
    private Vector3 previousGlobeAngles;

    private Vector2d previousTeleportLocation = Vector2d.zero;

    private int activeStyle = 0;

    void Start()
    {
        styleDropdown.onValueChanged.AddListener(delegate { OnStyleValueChange(styleDropdown); });
        vrStyleDropdown.onValueChanged.AddListener(delegate { OnStyleValueChange(vrStyleDropdown); });
        vrGlobeButton.onClick.AddListener(delegate { TeleportToGlobe(); });
    }

    void Awake()
    {
        //Slippy.SetActive(false);

        //bool vrEnabled = true;  //(VRTK.VRTK_DeviceFinder.HeadsetCamera() != null);

        //if (!vrEnabled)
        //{
        //    GlobeCamera.gameObject.SetActive(true);
        //    SlippyMapCamera.GetComponent<Camera>().enabled = true;
        //}
    }

    void OnStyleValueChange(Dropdown dd)
    {
        SetActiveStyle(dd.value);
    }

    public void SetActiveStyle(int newStyle)
    {
        styleDropdown.value = newStyle;
        vrStyleDropdown.value = newStyle;
        Globes[activeStyle].SetActive(false);
        SlippyMaps[activeStyle].SetActive(false);
        activeStyle = newStyle;
        Globes[activeStyle].SetActive(true);
        SlippyMaps[activeStyle].SetActive(true);

        RefreshMap(previousTeleportLocation);
    }

    public void Teleport(Vector2d teleportLocation)
    {
        Globe.SetActive(false);
        Slippy.SetActive(true);

        showingGlobe = false;

        SlippyMapCamera.transform.localPosition = new Vector3(0.0f, 300.0f, 0.0f);
        CameraRig.transform.localPosition = new Vector3(0.0f, 300.0f, 0.0f);
        CameraRig.transform.localEulerAngles = new Vector3(45.0f, 0.0f, 0.0f);

        RefreshMap(teleportLocation);

        previousTeleportLocation = teleportLocation;
    }

    public void RefreshMap(Vector2d teleportLocation)
    {
        var map = SlippyMaps[activeStyle];
        var all = map.GetComponentsInChildren<Transform>();
        foreach (var a in all)
            if (map.gameObject.transform != a)
                Destroy(a.gameObject);

        var am = map.GetComponent<AbstractMap>();
        am.RestartAtLocation(teleportLocation);
    }

    public void TeleportToGlobe()
    {
        showingGlobe = true;
        Slippy.SetActive(false);
        CameraRig.transform.localPosition = previousGlobePosition;
        CameraRig.transform.localEulerAngles = previousGlobeAngles;
        Globe.SetActive(true);
    }

    void Update()
    {
        if (!UnityEngine.VR.VRSettings.enabled)
        {
            GlobeCamera.gameObject.SetActive(true);
            SlippyMapCamera.GetComponent<Camera>().enabled = true;
        }
        else
        {
            GlobeCamera.gameObject.SetActive(false);
            SlippyMapCamera.GetComponent<Camera>().enabled = false;
        }

        if (showingGlobe)
        {
            previousGlobePosition = CameraRig.transform.localPosition;
            previousGlobeAngles = CameraRig.transform.localEulerAngles;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TeleportToGlobe();
        }

        //if (Input.GetKeyDown(KeyCode.F1)) SetActiveStyle(0);
        //if (Input.GetKeyDown(KeyCode.F2)) SetActiveStyle(1);
        //if (Input.GetKeyDown(KeyCode.F3)) SetActiveStyle(2);
        //if (Input.GetKeyDown(KeyCode.F4)) SetActiveStyle(3);

        //if (Input.GetKeyDown(KeyCode.Home))
        //{
        //    SlippyMap.Restart("36.058781, -112.109295");
        //}

        //if (Input.GetKeyDown(KeyCode.End))
        //{
        //    SlippyMap.Restart("35.658611111111, 139.74555555556");
        //}
    }
}
