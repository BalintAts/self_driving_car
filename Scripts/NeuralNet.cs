using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

public class NeuralNet : MonoBehaviour
{
    #region
    public Matrix<float> inputlayer; 
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer;
    public List<Matrix<float>> weightsList = new List<Matrix<float>>();
    public List<float> biases = new List<float>();
    public float fitness;
    #endregion

    public NeuralNet(int inputSize, int outputSize)
    {
        inputlayer = Matrix<float>.Build.Dense(1, inputSize);
        outputLayer = Matrix<float>.Build.Dense(1, outputSize);
    }
}
