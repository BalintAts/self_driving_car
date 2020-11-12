using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class NeuralNet : MonoBehaviour
{
    #region properties
    public Matrix<float> inputlayer; 
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer;
    public List<Matrix<float>> weightsMatrixList = new List<Matrix<float>>();
    public List<float> biases = new List<float>();  //one bias applies all the neurons in a layer
    public float fitness;
    #endregion

    public NeuralNet(int inputSize, int outputSize, int hiddenLayerCount, int hiddenLayerSize)
    {
        inputlayer = Matrix<float>.Build.Dense(1, inputSize);
        outputLayer = Matrix<float>.Build.Dense(1, outputSize);
        Initialize(inputSize, outputSize, hiddenLayerCount, hiddenLayerSize);
    }

    //initializes weights and biases
    public void Initialize(int inputSize, int outputSize,  int hiddenLayerCount, int hiddenLayerSize)
    {
        inputlayer.Clear();
        outputLayer.Clear();
        weightsMatrixList.Clear();
        hiddenLayers.Clear();
        biases.Clear();

        //adding the weight matriy between the input and first hidden layer
        weightsMatrixList.Add(Matrix<float>.Build.Dense(inputSize, hiddenLayerSize));
        for ( int i= 1; i <= hiddenLayerSize; i++)
        {
            //Layers
            hiddenLayers.Add(Matrix<float>.Build.Dense(1, hiddenLayerSize));
            biases.Add(Random.Range(-1f, 1f));

            //Adding the weight matrices between hidden layers
            weightsMatrixList.Add(Matrix<float>.Build.Dense(hiddenLayerSize, hiddenLayerSize));
        }
        weightsMatrixList.Add(Matrix<float>.Build.Dense(hiddenLayerSize, outputSize));
        biases.Add(Random.Range(-1f, 1f));

        RandomizeWeights();
        
    }

    private void RandomizeWeights()
    {
        for (int i = 0; i< weightsMatrixList.Count; i++)
        {
            for (int j = 0; j < weightsMatrixList[i].RowCount; j++)
            {
                for (int k = 0; k < weightsMatrixList[i].ColumnCount ; k++)
                {
                    weightsMatrixList[i][j, k] = Random.Range(-1f,1f);
                }
            }
        }
    }

    public (float,float) RunNetwork (float a, float b, float c)
    {
        //need to generalize for any input size
        inputlayer[0, 0] = a;
        inputlayer[0, 1] = b;
        inputlayer[0, 2] = c;

        inputlayer.PointwiseTanh(); //similar like the sigmoid function, but the output range is -1, 1,  not 0, 1 .

        //propagate
        hiddenLayers[0] = ((inputlayer * weightsMatrixList[0]) + biases[0]).PointwiseTanh();
        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i-1] * weightsMatrixList[i]) + biases[i]).PointwiseTanh();
        }
        outputLayer = ((hiddenLayers[hiddenLayers.Count- 1] * weightsMatrixList[weightsMatrixList.Count-1]) + biases[biases.Count - 1]).PointwiseTanh();

        return ((float)Math.Tanh(outputLayer[0, 0]) / 2 + 0.5f,  (float)Math.Tanh(outputLayer[0,1])); //acceleration must be between 0,1 , sterring between -1, 1
    }
}


