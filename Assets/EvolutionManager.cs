using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using UnityEngine;

using Random = UnityEngine.Random;

public class EvolutionManager : MonoBehaviour{
    public GameObject carPrefab;
    public List<NeuralNetworkScript> cars;
    public Transform spawnPoint;
    public NeuralNetworkUI ui;

    private float elapsedSinceGeneration;
    private int generationIndex;

    [Header("Evolution settings")]
    public int populationSize;
    public int[] neuronLayerCount;
    public double mutationRate;
    public float generationTime;
    public event Action onNewGeneration;

    public int deadCarCount;

    private void Start(){
        GenerateInitialPopulation();
    }

    private  void Update(){
        elapsedSinceGeneration += Time.deltaTime;
        if (elapsedSinceGeneration >= generationTime || deadCarCount == cars.Count) {
            deadCarCount = 0;
            elapsedSinceGeneration = 0f;
             GenerateNextGeneration();
        }
    }

    private void OnGUI(){
        if(elapsedSinceGeneration < generationTime / 4f)
            GUI.Label(new Rect(10,10,200,200), "Generated!");
    }

    private void GenerateNextGeneration(){
        elapsedSinceGeneration = 0f;
        generationIndex++;

        Debug.Log("--- Generating new population");
        Debug.Log("Ordering by fitness ..");
        OrderByFitnessDescending();
        Debug.Log("Removing unfit ...");
        RemoveUnfit();
        Debug.Log("Creating offspring...");
        cars.AddRange(GetOffspring());
        Debug.Log("Resetted the car's properties");
        ResetCars();
        Debug.Log($"--- Generated new population: {generationIndex}");

        onNewGeneration?.Invoke();
    }

    private void ResetCars(){
        foreach (var car in cars) {
            car.transform.position = spawnPoint.position; //+ new Vector3(Random.value, Random.value, 0f) * 4;
            car.transform.rotation = Quaternion.identity;
            car.distanceTraveled = 0f;
            car.agent.isAlive = true;
            car.rb.simulated = true;
            car.onCarDead += onCarDead;
        }
    }

    private void RemoveUnfit(){
        foreach (var car in cars) {
            car.onCarDead -= onCarDead;
        }
        int start = populationSize / 2;
        int end = cars.Count;
        foreach (var car in cars.Skip(populationSize / 2).ToArray()) {
            if (car != null) {
                Destroy(car.gameObject);
            }
        }

        cars.RemoveRange(start, end - start);
    }

    private void OrderByFitnessDescending(){
        cars = cars.OrderByDescending(car => car.distanceTraveled).ToList();
    }

    private IEnumerable<NeuralNetworkScript> GetOffspring(){
        var chromosomes = new List<Chromosome>();
//        for (int i = 0; i < best.Count - 1; i += 2) {
//            var first = best[i];
//            var second = best[i + 1];
//            chromosomes.Add(Chromosome.Crossover(first.agent.chromosome, second.agent.chromosome));
//            chromosomes.Add(Chromosome.Crossover(first.agent.chromosome, second.agent.chromosome));
//        }
        var parentChromosomes = cars.Select(x => x.agent.chromosome).ToArray();
        while (chromosomes.Count < populationSize / 2) {
            var firstParentIndex = Random.Range(0, parentChromosomes.Length);
            var secondParentIndex = Random.Range(0, parentChromosomes.Length);
            chromosomes.Add(Chromosome.Crossover(parentChromosomes[firstParentIndex], parentChromosomes[secondParentIndex]));
        }
//        foreach (var chromosome in chromsomes) {
//            chromosome.Mutate();
//        }
        var newCars = new List<NeuralNetworkScript>();
        for (int i = 0; i < chromosomes.Count; i++) {
            var car = Instantiate(carPrefab, spawnPoint.position , Quaternion.identity, transform)
                .GetComponent<NeuralNetworkScript>();
            car.agent = Agent.Create(chromosomes[i], NeuralNetwork.FromChromosome(chromosomes[i], neuronLayerCount));
//            car.agent = Agent.CreateRandom(neuronLayerCount);
            car.name = $"Car G: {generationIndex} | {i}";

            newCars.Add(car);
        }
        return newCars;
    }

    /// <summary>
    /// Generates the initial random population
    /// </summary>
    public void GenerateInitialPopulation(){
        for (int i = 0; i < populationSize; i++) {
            var car = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity, transform)
                .GetComponent<NeuralNetworkScript>();
            car.agent = Agent.CreateRandom(neuronLayerCount);
            car.name = $"Car {generationIndex} | {i}";
            car.onCarDead += onCarDead;
            cars.Add(car);
        }

        onNewGeneration?.Invoke();
        Debug.Log($"Generated initial population, size: {populationSize}");
    }

    private void onCarDead(){
        deadCarCount++;
    }
}