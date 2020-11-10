using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;

    [Range(-1f, 1f)]
    public float acceleration, turning;
    public float timerSinceStart;

    //score based on distance travelled, and avaragy speed
    [Header("Fitness")]
    public float overAllFitness;
    public float distanceMultiplier = 1.4f;   //these values influences which is more important
    public float avgSpeedMultiplier = 0.2f;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor; //raycast distance values.


    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    void Update()
    {

    }
}
