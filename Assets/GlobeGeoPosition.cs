using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;

public class GlobeGeoPosition : MonoBehaviour
{
    public float radius = 80.0f;

    [Geocode]
    [SerializeField]
    string _latitudeLongitudeString;

    public Vector2d location;

    public Vector3 eulerAngles;

    public float elevation = 0.0f;

    void Start()
    {
    }

    void Update()
    {
        var latLonSplit = _latitudeLongitudeString.Split(',');
        location = new Vector2d(double.Parse(latLonSplit[0]), double.Parse(latLonSplit[1]));

        float lat = (float)location.x * Mathf.Deg2Rad;
        float lng = (float)location.y * Mathf.Deg2Rad;

        float co = Mathf.Cos(lng);
        float so = Mathf.Sin(lng);
        float ca = Mathf.Cos(lat);
        float sa = Mathf.Sin(lat);

        float x = co * ca;
        float z = so * ca;
        float y = sa;

        Vector3 py = new Vector3(-co * sa, -so * sa, ca);
        Vector3 pz = new Vector3(x, y, z);

        transform.localRotation = Quaternion.LookRotation(pz, py) * Quaternion.Euler(eulerAngles);
        transform.localPosition = pz * (radius + elevation);
    }
}
