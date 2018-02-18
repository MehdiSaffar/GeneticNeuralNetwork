using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class NeuralNetworkUI : MonoBehaviour{

	private NeuralNetworkScript nn;

	public NeuronUI neuronPrefab;
	public SynapseUI synapsePrefab;
	public List<NeuronUI> neurons = new List<NeuronUI>();

	public List<SynapseUI> synapses = new List<SynapseUI>();

	public float xNeuronDistance;
	public float yNeuronDistance;
	public float xNeuronOffset;
	public float yNeuronOffset;

	private RectTransform rt;
	public LineGraphics lg;
	public float maxThickness;

	public Text currentCar;
	public Transform carsList;

	public int carCount;
	public int carIndex;

	public EvolutionManager evolutionManager;

	private void Awake(){
		rt = GetComponent<RectTransform>();
	}

	private void Start(){
		evolutionManager.onNewGeneration += onNewGeneration;
	}

	private void OnDestroy(){
		evolutionManager.onNewGeneration -= onNewGeneration;
	}

	private void FixedUpdate(){
		var cars = GetAllCars().ToList();
		carCount = cars.Count;
		if (Input.GetKeyDown(KeyCode.UpArrow) && carIndex > 0) {
			carIndex--;
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) && carIndex < carCount - 1) {
			carIndex++;
		}

		var highestDistance = cars.Max(x => x.distanceTraveled);
		var carToShow = cars.FindIndex(car => Math.Abs(car.distanceTraveled - highestDistance) < 0.01);
		carIndex = Math.Min(carToShow, carCount - 1);
		UpdateUI();
	}

	private void UpdateUI(){
		DisplayCar(carIndex);
	}

	private void DisplayCar(int carIndex){
		nn = GetAllCars()[carIndex];
		ResetUI();
		DrawUI();
	}

	private void onNewGeneration(){
		carCount = GetAllCars().Length;
		DisplayCar(0);
	}

	private NeuralNetworkScript[] GetAllCars() => carsList.GetComponentsInChildren<NeuralNetworkScript>();

	public void DrawUI(){
		currentCar.text = nn.name;
		for (var layerIndex = 0; layerIndex < nn.agent.network.Layers.Count; layerIndex++) {
			var layer = nn.agent.network.Layers[layerIndex];
			float x = layerIndex * xNeuronDistance;

			for (var neuronIndex = 0; neuronIndex < layer.Count; neuronIndex++) {
				var neuron = layer[neuronIndex];
				float y = neuronIndex * yNeuronDistance;
				var neuronUi =
					Instantiate(neuronPrefab.gameObject,
							new Vector3(xNeuronOffset + x, Camera.main.pixelHeight - (yNeuronOffset + y), 0), Quaternion.identity, transform)
						.GetComponent<NeuronUI>();
				neuronUi.GetComponent<Image>().color = Color.Lerp(Color.white, Color.black, (float) neuron.Output);
				neurons.Add(neuronUi);

				for (var synapseIndex = 0; synapseIndex < neuron.InputSynapses.Count; synapseIndex++) {
					var synapse = neuron.InputSynapses[synapseIndex];
					float xStart = (layerIndex - 1) * xNeuronDistance;
					float yStart = synapseIndex * yNeuronDistance;
					float xEnd = layerIndex * xNeuronDistance;
					float yEnd = neuronIndex * yNeuronDistance;
					var line = new LineSegment{
						start = new Vector2(xNeuronOffset + xStart - rt.rect.width / 2f, -(yNeuronOffset + yStart - rt.rect.height / 2f)),
						end = new Vector2(xNeuronOffset + xEnd - rt.rect.width / 2f, -(yNeuronOffset + yEnd - rt.rect.height / 2f)),
						color = Color.red,
						thickness = Mathf.Clamp(Mathf.Abs((float) synapse.Weight), 0.2f,1f) * maxThickness,
					};
					if (synapse.Weight <= 0f) {
						line.color = Color.blue;
					}
					lg.segments.Add(line);
				}

			}
		}
	}

	private void ResetUI(){
		lg.segments.Clear();
		foreach (var neuronUi in neurons) {
			if(neuronUi != null)
			Destroy(neuronUi.gameObject);
		}

		neurons.Clear();
	}
}