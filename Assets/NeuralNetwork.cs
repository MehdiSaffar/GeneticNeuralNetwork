using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using JetBrains.Annotations;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[Serializable]
public class NeuralNetwork{
    /// <summary>
    /// Default neuron bias
    /// </summary>
    public const double DefaultNeuronBias = 0;

    /// <summary>
    /// Default synapse weight
    /// </summary>
    public const double DefaultSynapseWeight = 1;

    /// <summary>
    /// Do not use the setter
    /// </summary>
    public readonly List<List<Neuron>> Layers = new List<List<Neuron>>();

    /// <summary>
    /// Creates a neural network with the structure provided in <paramref name="layerNeuronCount"/> with random weights
    /// </summary>
    public static NeuralNetwork Network(params int[] layerNeuronCount){
        var neuralNetwork = new NeuralNetwork();
        int layerCount = layerNeuronCount.Length;
        for (int i = 0; i < layerCount; i++) {
            neuralNetwork.Layers.Add(new List<Neuron>());
            var currentLayer = neuralNetwork.Layers.Last();
            var previousLayer = i == 0 ? null : neuralNetwork.Layers[i - 1];
            for (int j = 0; j < layerNeuronCount[i]; j++) {
                currentLayer.Add(new Neuron(previousLayer));
            }
        }
        return neuralNetwork;
    }

    public override string ToString(){
        var strBuilder = new StringBuilder();
        for (var layerIndex= 0; layerIndex < Layers.Count; layerIndex++) {
            var neurons = Layers[layerIndex];
            for (var neuronIndex = 0; neuronIndex < neurons.Count; neuronIndex++) {
                var neuron = neurons[neuronIndex];
                var weights = neuron.InputSynapses.Select(x => x.Weight).ToArray();
                if(weights.Any())
                    strBuilder.Append($"L({layerIndex}) N({neuronIndex}) : {weights.ToPrettyString()}\n");
            }
        }

        return strBuilder.ToString();
    }

    /// <summary>
    /// Creates a neural network with the structure provided in <paramref name="layerNeuronCount"/> with weights decoded from the chromosome
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the number of genes does not match the count of
    ///     synapses and biases in the neural network to be creates
    /// </exception>
    public static NeuralNetwork FromChromosome(Chromosome chromosome, params int[] layerNeuronCount){
        int synapseCount = layerNeuronCount.Aggregate(1, (current, t) => current * t);
        int biasCount = layerNeuronCount.Sum();
        if (chromosome.genes.Count != synapseCount + biasCount) {
            throw new InvalidOperationException("The count of genes does not match the count of synapses and biases!");
        }
        var neuralNetwork = Network(layerNeuronCount);
        int geneIndex = 0;
        for (var layerIndex = 1; layerIndex < neuralNetwork.Layers.Count; layerIndex++) {
            var layer = neuralNetwork.Layers[layerIndex];
            foreach (var neuron in layer) {
                foreach (var synapse in neuron.InputSynapses) {
                    synapse.Weight = chromosome.genes[geneIndex];
                    geneIndex++;
                }

                neuron.Bias = chromosome.genes[geneIndex];
                geneIndex++;
            }
        }

        return neuralNetwork;
    }

    /// <summary>
    /// Randomizes all the weights in the neural network
    /// </summary>
    public void RandomizeWeightsAndBiases(){
        foreach (var layer in Layers) {
            foreach (var neuron in layer) {
                foreach (var synapse in neuron.InputSynapses) {
                    synapse.Weight = Random.Range(-1.0f, 1.0f);
                }

                neuron.Bias = Random.Range(-1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// Feeds forward the values in <paramref name="inputs"/>
    /// </summary>
    /// <param name="inputs">Inputs to the neural network</param>
    public double[] FeedForward(params double[] inputs){
        if (inputs.Length != Layers.First().Count) {
            throw new InvalidOperationException($"The number of inputs provided `{inputs.Length}` does not match the number of input neurons `{Layers.First().Count}`");
        }
        var inputLayer = Layers[0];
        for (var neuronIndex = 0; neuronIndex < inputLayer.Count; neuronIndex++) {
            var neuron = inputLayer[neuronIndex];
            neuron.Output = inputs[neuronIndex];
            neuron.Bias = 1.0;
        }

        FeedForward(1);
        return GetOutput();
    }

    /// <summary>
    /// Returns the output values of the neural network
    /// </summary>
    private double[] GetOutput(){
        var output = Layers.Last().Select(neuron => neuron.Output).ToArray();
        return output;
    }

    private void FeedForward(int layerIndex){
        var layer = Layers[layerIndex];
        var input = Layers[layerIndex - 1].Select(neuron => neuron.Output).ToArray();
        foreach (var neuron in layer) {
            var weightedSum = neuron.InputSynapses
                .Select((synapse, synapseIndex) => synapse.Weight * input[synapseIndex])
                .Sum();
            neuron.Output = NeuralNetworkHelper.Sigmoid(weightedSum + neuron.Bias);
        }
        if (layerIndex < Layers.Count - 1) {
            FeedForward(layerIndex + 1);
        }
    }
}
