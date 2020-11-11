﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector3 input;
    private Vector3 startPosition, startRotation;

    [Range(-1f, 1f)]
    public float acceleration, turning;
    private float tweekMaxTrunging = 90;   //in degrees
    public float timerSinceStart;

    //score based on distance travelled, and avaragy speed
    [Header("Fitness")]
    public float overAllFitness;
    public float distanceMultiplier = 1.4f;   //these values influences which is more important
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    public float targetFitness = 1000;
    public float dumbTime = 20;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor; //raycast distance values.

    float tweekingA = 11.4f;
    float tweekingB = 0.02f;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    public void Reset()
    {
        timerSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overAllFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    private void OnCollisionEnter(Collision collision)
     
    {
        Reset();
    }

    private void CalculateFintess()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timerSinceStart;

        overAllFitness = totalDistanceTravelled * distanceMultiplier
            + avgSpeed * avgSpeedMultiplier
            + (aSensor + bSensor + cSensor) / 3 * sensorMultiplier;
    
        //Check if network is bad, and reset
        if (timerSinceStart > dumbTime && overAllFitness < 40)
        {
            Reset();
        }

        if (overAllFitness >= targetFitness)
        {
            //save network to json
            Reset();
        }
    }

    private void SetInputSensorsBySensing()
    {
        Vector3 a = (transform.forward + transform.right); //diagonal right. using this vector's direction, to create ray for raycast.
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            float reduction = 20f; //makes sure that raycast distances are low numbers, where the sigmoid function is the most teresting.
            aSensor = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);
            Debug.Log("A hit");

        }

        r.direction = b;
        if (Physics.Raycast(r, out hit))
        {
            float reduction = 20f; 
            bSensor = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);
            Debug.Log("B hit");

        }

        r.direction = c;
        if (Physics.Raycast(r, out hit))
        {
            float reduction = 20f; 
            cSensor = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);
            Debug.Log("C hit");
        }

        //List<Ray> rays = new List<Ray>()
        //{
        //    new Ray(transform.position, transform.forward + transform.right ),
        //    new Ray(transform.position, transform.forward),
        //    new Ray(transform.position, transform.forward - transform.right )
        //};

        //List<float> sensors = List<float>(){ }
    }

    public void MoveCar (float throttle, float steering)
    {
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, throttle * tweekingA), tweekingB);
        //converting input to local space
        input = transform.TransformDirection(input);
        transform.position += input;
        transform.eulerAngles += new Vector3(0, steering * tweekMaxTrunging * 0.02f, 0);

    }

    void FixedUpdate()
    {
        SetInputSensorsBySensing();
        lastPosition = transform.position;
        //neural net sets acceleration and turn

        MoveCar(acceleration, turning);
        timerSinceStart += Time.deltaTime;
        CalculateFintess();
        acceleration = 1;
        turning = .2f;
        //Debug.Log(aSensor);
    }
}
