using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarController : MonoBehaviour
{
    #region  //properties
    public NeuralNet net;
    private GeneticController geneticController;

    [Header("Neural network options")]
    public static int NUMBEROFHIDDENLAYERS = 1;
    public static int HIDDENLAYERSIZE = 10;

    private Vector3 input;
    private Vector3 startPosition, startRotation;

    [Range(-1f, 1f)]
    public float acceleration, turning;
    private float tweekMaxTrunging = 90;   //in degrees
    public float timerSinceStart;

    //score based on distance travelled, and avarage speed
    [Header("Fitness")]
    public float overAllFitness;
    public float distanceMultiplier = 1.4f;   //these values influences which is more important
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    public float targetFitness = 1000;
    public float dumbTime = 10;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor; //raycast distance values.
    //TOdo: create a list, instead of multiple different sensors
    public static int numberOfSensos = 3;
    public float[] sensors;

    public static int numberOfOutput = 2;
    private float[] outPut; //Todo : Use this

    float tweekingA = 11.4f;
    float tweekingB = 0.02f;
    #endregion

    public int id;

    public GameObject explosionEffect;
    
    public CarController()
    {
        Debug.Log("carcontroller constructor");
    }

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        geneticController = GameObject.FindObjectOfType<GeneticController>();
        sensors = new float[3];

    }

    private void OnCollisionEnter(Collision collision)     
    {
        Death();
    }

    private void CalculateFintess()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timerSinceStart;

        overAllFitness = totalDistanceTravelled * distanceMultiplier
            + avgSpeed * avgSpeedMultiplier
            + sensors.Sum() / 3 * sensorMultiplier;
    
        //Check if network is bad, and reset
        if (timerSinceStart > dumbTime && overAllFitness < 40)
        {
            Death();  //kill stupid but ambitious cars
        }
        if (overAllFitness >= targetFitness)
        {
            //save network to json
            Death(); //car is too good
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
            //aSensor = hit.distance / reduction;
            sensors[0] = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }
        r.direction = b;
        if (Physics.Raycast(r, out hit))
        {
            float reduction = 20f;
            sensors[1] = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);

        }
        r.direction = c;
        if (Physics.Raycast(r, out hit))
        {
            float reduction = 20f;
            sensors[2] = hit.distance / reduction;
            Debug.DrawLine(r.origin, hit.point, Color.red);
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

    private void Death()
    {
        Explode();
        geneticController.Death(overAllFitness, id);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        SetInputSensorsBySensing();
        lastPosition = transform.position;
        //neural net sets acceleration and turn

        if (net != null)
        {
            (acceleration, turning) = net.RunNetwork(sensors);  //need to generalize the arguments for any size of input 
        }
        MoveCar(acceleration, turning);
        timerSinceStart += Time.deltaTime;
        CalculateFintess();
        //if (timerSinceStart < 3 && overAllFitness < 100)
        //{
        //    Death();
        //}
        Debug.Log("overallFitness " + overAllFitness);
        
    }

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
    }
}
