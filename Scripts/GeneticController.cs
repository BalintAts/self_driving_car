using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using UnityEditor;

public class GeneticController : MonoBehaviour
{
    [Header("References")]
    public CarController carController;
    [Header("Controls")]
    public int populationSize = 200;
    [Range(0f, 1f)]
    public float mutationRate = 0.05f;
    [Header("Crossover Controls")]
    public int numberOfBestAgentToSelect = 8;
    public int numberOfWorstAgentToSelect = 3;
    public int numberToCrossover;

    private List<int> genePool = new List<int>();  //indices of networks to crossover

    private int naturallySelectedIndex;

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
        population = new NeuralNet[populationSize];
        //FillPartOfPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome()
    {
        carController.ResetWithNetwork(population[currentGenomeIndex]);
    }

    //public void FillPartOfPopulationWithRandomValues(NeuralNet[] newPopulation, int startingIndex) //startingindex: we don't have to reinilitialize all nets, becouse most of them coming from crossover
    //{
    //    for (int i = startingIndex; i < initialPopulationSize; i++)
    //    {
    //        // TODO: refactor 3 2
    //        newPopulation[i] = new NeuralNet(3, 2, carController.HIDDENLAYERS, carController.HIDDENLAYERSIZE);
    //    }

    //}
    
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
            Repopulate();
        }
    }

    public void Repopulate()
    {
        genePool.Clear();
        currentGenerationIndex++;
        naturallySelectedIndex = 0;
        population = population.OrderBy(o => o.fitness).ToArray();
        NeuralNet[] newPopulation = PickBestsAndWorstsFromPopulation();

        //
        Crossover(newPopulation);
        //Mutate(something);
    
    }

    /// <summary>
    /// dfsdfsdfsdfsdfsdfsdfsdfsdfds
    /// </summary>
    /// <returns></returns>
    private NeuralNet[] PickBestsAndWorstsFromPopulation()
    {
        //NeuralNet a = new NeuralNet(3, 2, 5, 5);
        //NeuralNet b = a.DeepClone();

        NeuralNet[] newPopulation = new NeuralNet[populationSize];


        //select the best ones, and add them some times, depending on how good it is     ..add teh best's indices to genepool
        for (int i = 0; i < numberOfBestAgentToSelect; i++)
        {
            newPopulation[naturallySelectedIndex] = population[i].DeepClone();
            newPopulation[naturallySelectedIndex].fitness = 0;
            naturallySelectedIndex++;
            int f = Mathf.RoundToInt(population[i].fitness * 10);   //this is a factor which depends how good is hte network. If it is better, it will be given to the genepool more times
            for (int c = 0; c < f; c++)
            {
                genePool.Add(i);
            }        
        }

        for (int i = 0; i < numberOfWorstAgentToSelect; i++)
        {
            int last = population.Length - i;
            int f = Mathf.RoundToInt(population[last].fitness * 10);   //this is a factor which depends how good is hte network. If it is better, it will be given to the genepool more times
            for (int c = 0; c < f; c++)
            {
                genePool.Add(last);
            }

        }
        return newPopulation;
    }


    private void Crossover(NeuralNet[] newPopulation)
    {
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNet Child = new NeuralNet(3, 2, carController.HIDDENLAYERS, carController.HIDDENLAYERSIZE); //TODO create fields for iput, output vector size

            ////if the ganepool is empty:
            //int fatherIndex = i;
            //int motherIndex = i + 1;

            ////iterating trough all the weight matrices, and choose one of the parents's randomly
            //for (int w = 0; w < Child1.weightsMatrixList.Count; w++)
            //{
            //    if (genePool.Count >= 1)
            //    {
            //        fatherIndex = genePool[Random.Range(0, genePool.Count)];
            //        motherIndex = genePool[Random.Range(0, genePool.Count)];                   
            //    }

            //    //swap layers instead of individual weights for now
            //    if (Random.Range(.0f, 1f) < .5f)
            //    {
            //        Child1.weightsMatrixList[w] = population[fatherIndex].weightsMatrixList[w];
            //        Child2.weightsMatrixList[w] = population[motherIndex].weightsMatrixList[w];
            //    }
            //    else
            //    {
            //        Child1.weightsMatrixList[w] = population[motherIndex].weightsMatrixList[w];
            //        Child2.weightsMatrixList[w] = population[fatherIndex].weightsMatrixList[w];
            //    }
            //}
            for (int w = 0; w < Child.weightsMatrixList.Count; w++)
            {
                if (genePool.Count >= 1)
                {
                    int choosedWeightMatrixAndBiasIndex = genePool[Random.Range(0, genePool.Count)];
                    Child.weightsMatrixList[w] = population[choosedWeightMatrixAndBiasIndex].weightsMatrixList[w];
                    
                    //we can use the same index for bias becouse one of the 2 layers of the matrices have that bias
                    //but we need one less bias
                    if (w < Child.biases.Count)
                    {
                        Child.biases[w] = population[choosedWeightMatrixAndBiasIndex].biases[w];
                    }
                }
            }

            population[naturallySelectedIndex] = Child;
            naturallySelectedIndex++;

        }
    }

    private void Mutate()
    {

    }
}
