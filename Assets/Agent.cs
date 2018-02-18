using System;
using System.Linq;

//[Serializable]
public class Agent{
    /// <summary>
    /// A neural network as the brain of the agent
    /// </summary>
    public NeuralNetwork network;
    /// <summary>
    /// The chromosome of the agent
    /// </summary>
    public Chromosome chromosome;

    public bool isAlive;

    /// <summary>
    /// Creates an agent formed of a certain chromosome and a neural network
    /// </summary>
    public static Agent Create(Chromosome _chromosome, NeuralNetwork _network){
        var agent = new Agent{
            chromosome = _chromosome,
            network = _network,
            isAlive = true
        };
        return agent;
    }

    /// <summary>
    /// Processes the input and returns the decision of the agent
    /// </summary>
    /// <returns>Output of the agent</returns>
    public double[] ProcessInput(params double[] inputs){
        return network.FeedForward(inputs);
    }

    /// <summary>
    /// Creates a random a chromosome then creates a neural network with the given structure with weights decoded from the chromosome
    /// </summary>
    /// <param name="neuronLayerCount">An array whose each item contains the count of neurons on the i'th layer</param>
    public static Agent CreateRandom(int[] neuronLayerCount){
        var geneCount = neuronLayerCount.Sum() + neuronLayerCount.Aggregate(1, (current, t) => current * t);
        var chromosome = Chromosome.RandomChromosome(geneCount);
        var agent = Create(chromosome, NeuralNetwork.FromChromosome(chromosome, neuronLayerCount));
        return agent;
    }
}