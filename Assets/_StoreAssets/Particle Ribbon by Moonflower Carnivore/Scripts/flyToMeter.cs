using UnityEngine;
using System.Collections;

public class flyToMeter : MonoBehaviour
{

    public GameObject meter;

    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, meter.transform.position, 1.5f * Time.deltaTime);

    }
}