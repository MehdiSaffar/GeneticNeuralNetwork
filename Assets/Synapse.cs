using System;

using JetBrains.Annotations;

[Serializable]
public class Synapse{

    /// <summary>
    /// Neuron the synapse is attached from
    /// </summary>
    public Neuron Start{ get; set; }
    /// <summary>
    /// Neuron the synapse is attached to
    /// </summary>
    public Neuron End{ get; set; }

    /// <summary>
    /// Weight of the connection
    /// </summary>
    public double Weight;
    
    public Synapse([NotNull] Neuron startNeuron,[NotNull] Neuron endNeuron, double weight){
        Start = startNeuron;
        End = endNeuron;
        Weight = weight;
    }
}
