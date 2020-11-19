using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using UnityEditor;
using System;
public class GeneticController : MonoBehaviour
{
    public GameObject car;
    [Header("References")]
    public GameObject spawner;
    //[Header("References")]
    //public CarController carController;
    [Header("Controls")]
    public int populationSize = 10;
    public int numberOfRetards = 1;  //instead of mutation, we add some retard among the parents
    [Range(0f, 1f)]
    public float mutationRate = 0.05f;
    [Header("Crossover Controls")]
    public int numberOfBestAgentToSelect = 2;
    public CarController carController; //class reference


    private NeuralNet[] population;

    [Header("Public View")]
    public int currentGenerationIndex;
    public int numberofMatricesToSwap = 1;
    private int currentGenomeIndex  = 0;

    private void Start()
    {
        //carController = car.GetComponent<CarController>();
        Debug.Log(carController);
        CreatePopulation();
    }
    private void CreatePopulation()
    {
        population = new NeuralNet[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNet newNet = new NeuralNet(CarController.numberOfSensos, CarController.numberOfOutput, CarController.NUMBEROFHIDDENLAYERS, CarController.HIDDENLAYERSIZE);
            population[i] = newNet;
            GameObject newCar = Instantiate(car, spawner.transform.position , spawner.transform.rotation);
            newCar.GetComponent<CarController>().net = newNet;
            newCar.GetComponent<CarController>().id = i;
        }
    }
    
    public void Death(float fitness,  int index)
    {

        if (currentGenomeIndex < population.Length - 1)
        {
            population[index].fitness = fitness;
            currentGenomeIndex++;
            
        }
        else
        {
            currentGenomeIndex = 0;
            Repopulate();
        }
    }

    private void Repopulate()
    {
        Debug.Log("repopulate");
        currentGenerationIndex++;
        population = population.OrderBy(o => o.fitness).ToArray();
        Crossover();
        //Mutate(something); instead of mutate, we give some retard among the parents
        for (int i = 0; i < numberOfRetards; i++)
        {
            population[populationSize - i - 1] = new NeuralNet(CarController.numberOfSensos, CarController.numberOfOutput, CarController.NUMBEROFHIDDENLAYERS, CarController.HIDDENLAYERSIZE);   // add retards
        }
        for (int i = 0; i < populationSize; i++)
        {
            GameObject newCar = Instantiate(car, spawner.transform.position, spawner.transform.rotation);
            newCar.GetComponent<CarController>().net = population[i];
            newCar.GetComponent<CarController>().id = i;
        }
    }

    private void Crossover()
    {
        NeuralNet[] newPopulation = new NeuralNet[populationSize];
        for (int i = 0; i < populationSize - numberOfRetards; i++)
        {
            NeuralNet Child = new NeuralNet(CarController.numberOfSensos, CarController.numberOfOutput, CarController.NUMBEROFHIDDENLAYERS, CarController.HIDDENLAYERSIZE); 
            for (int w = numberofMatricesToSwap; w < Child.weightsMatrixList.Count; w++)
            {
                int choosedWeightMatrixAndBiasIndex = populationSize - UnityEngine.Random.Range(0, numberOfBestAgentToSelect) - 1; //choosing the best of the ordered population array
                NeuralNet chosedNetToSwapAMatrix = population[choosedWeightMatrixAndBiasIndex];
                Child.weightsMatrixList[w] = chosedNetToSwapAMatrix.weightsMatrixList[w];
                //we can use the same index for bias becouse one of the 2 layers of the matrices have that bias
                //but we need one less bias
                if (w < Child.biases.Count)
                {
                    Child.biases[w] = population[choosedWeightMatrixAndBiasIndex].biases[w];
                }
            }
            newPopulation[i] = Child;
            

        }
        population = newPopulation;
    }
}
