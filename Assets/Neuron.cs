using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Neuron{
	/// <summary>
	/// Input synapses
	/// </summary>
	public readonly List<Synapse> InputSynapses;

	/// <summary>
	/// Bias
	/// </summary>
	public double Bias;

	/// <summary>
	/// Output of the neuron
	/// </summary>
	public double Output;

	/// <summary>
	/// Builds a neuron a connects it with synapse to the previous neurons. Provide null to <paramref name="previousNeurons"/>
	/// in case the neuron has no connection to previous neurons (such as a neuron in the input layer of a feed forward neural network)
	/// </summary>
	/// <param name="previousNeurons">List of the previous neurons to connect to</param>
	public Neuron(IEnumerable<Neuron> previousNeurons, double bias = NeuralNetwork.DefaultNeuronBias){
//		Debug.Log("Created neuron");
		InputSynapses = new List<Synapse>();
		Bias = bias;
		if (previousNeurons == null) {
//			Debug.Log("The current neuron does not have any incoming synapses.");
		}
		else {
			int i = 0;
			foreach (var previousNeuron in previousNeurons) {
				i++;
//				Debug.Log($"Created synapse #{i}");
				InputSynapses.Add(new Synapse(previousNeuron, this, NeuralNetwork.DefaultSynapseWeight));
			}
		}
//		Debug.Log("Finished creating neuron.");
	}
}
