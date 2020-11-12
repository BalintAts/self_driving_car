using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class GeneticController : MonoBehaviour
{
    [Header("References")]
    public CarController carController;
    [Header("Controls")]
    public int initialPopulationSize = 85;
    [Range(0f, 1f)]
    public float mutationRate = 0.05f;
    [Header("Crossover Controls")]
    public int numberOfBestAgentToSelect = 8;
    public int numberOfWorstAgentToSelect = 3;
    public int numberToCrossover;

    private List<int> genePool = new List<int>();

    private int naturallySelected;

    //array of neural nets
    private NeuralNet[] population;

    [Header("Public View")]
    public int currentGenerationIndex;
    public int currentGenomeIndex;

    private void Start()
    {
        CreatePopulation();
    }
    private void CreatePopulation()
    {
        population = new NeuralNet[initialPopulationSize];
        FillPartOfPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome()
    {
        carController.ResetWithNetwork(population[currentGenomeIndex]);
    }

    public void FillPartOfPopulationWithRandomValues(NeuralNet[] newPopulation, int startingIndex) //startingindex: we don't have to reinilitialize all nets, becouse most of them coming from crossover
    {
        for (int i = startingIndex; i < initialPopulationSize; i++)
        {
            // TODO: refactor 3 2
            newPopulation[i] = new NeuralNet(3, 2, carController.HIDDENLAYERS, carController.HIDDENLAYERSIZE);
        }

    }
    
    public void Death(float fitness, NeuralNet net)
    {
        //this should be currentGenomeIndex
        if (currentGenerationIndex < population.Length - 1)
        {
            population[currentGenomeIndex].fitness = fitness;
            currentGenomeIndex++;
            ResetToCurrentGenome();
        }
        else
        {
            //repopulate
        }
    }
}
