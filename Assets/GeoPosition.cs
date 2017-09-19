using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;

public class GeoPosition : MonoBehaviour
{
    public AbstractMap map;

    [Geocode]
    [SerializeField]
    string _latitudeLongitudeString;

    Vector2d location;
    public float elevation = 0.0f;

    public bool rotateZ = false;

    public float heading = 0.0f;

    void Start()
    {
    }

    void Update()
    {
        var latLonSplit = _latitudeLongitudeString.Split(',');
        location = new Vector2d(double.Parse(latLonSplit[0]), double.Parse(latLonSplit[1]));
        transform.position = Conversions.GeoToWorldPosition(location, map.CenterMercator, map.WorldRelativeScale).ToVector3xz() + new Vector3(0.0f, elevation, 0.0f);

        var euler = Mapbox.Unity.Constants.Math.Vector3Zero;

        if (rotateZ)
            euler.z = -heading;
        else
            euler.y = heading;

        transform.localRotation = Quaternion.Euler(euler);
    }
}
