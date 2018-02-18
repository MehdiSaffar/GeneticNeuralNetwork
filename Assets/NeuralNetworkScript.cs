using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class NeuralNetworkScript : MonoBehaviour{
	[Header("Settings")]
	public float sensorLength;

	public Vector3[] sensors;
	public float maxTurnSpeed;
	public float maxForwardSpeed;

	public bool drawTrail;
	public int trailCount;
	public float trailPointRadius;
	public float trailInterval;

	public Rigidbody2D rb;
	private readonly LinkedList<Vector3> oldPositions = new LinkedList<Vector3>();

	[Header("Simulation")]
	public double fitness;

	[HideInInspector] public Agent agent;
	private double placeTrailCountdown;
	private double[] _distances;

	public float distanceTraveled;
	public Vector3 oldPosition;
	public event Action onCarDead;

	private void Awake(){
		rb = GetComponent<Rigidbody2D>();
		sensors = sensors.Select(sensor => sensor.normalized).ToArray();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag("Wall")) {
//			Debug.Log($"Car {name} hit {other.name}");
			agent.isAlive = false;
			rb.simulated = false;
			onCarDead?.Invoke();
		}
	}

	private void FixedUpdate(){
		if (agent == null) {
			Debug.LogWarning("No agent is provided, will not perform FixedUpdate");
			return;
		}
		HandleTrail();

		if (agent.isAlive) {
			_distances = GetDistancesFromSensors().ToArray();
			var distancesNormalized = _distances.Select(distance => distance / sensorLength).ToArray();
			var output = agent.ProcessInput(distancesNormalized);
//			Debug.LogError($"Output: {output.ToPrettyString()}");
			var forwardSpeed = (float) output[0] * maxForwardSpeed;
			var turnSpeed = (float) (output[1] * 2.0 - 1.0) * maxTurnSpeed;
			rb.velocity = transform.right * forwardSpeed;
			rb.angularVelocity = turnSpeed;
			distanceTraveled += (oldPosition - transform.position).magnitude;
		}
	}

	/// <summary>
	/// Handles the placement of the trail spheres
	/// </summary>
	private void HandleTrail(){
		placeTrailCountdown -= Time.fixedDeltaTime;
		if (!(placeTrailCountdown <= 0f)) return;

		placeTrailCountdown = trailInterval;
		oldPositions.AddLast(transform.position);
		if (oldPositions.Count > trailCount) {
			oldPositions.RemoveFirst();
		}
	}

	/// <summary>
	/// Returns the distances of each sensor to the wall.
	/// If the ray does not hit a wall then it returns the maximum distance
	/// </summary>
	private IEnumerable<double> GetDistancesFromSensors() {
		return sensors.Select(sensor => {
			var hit = Physics2D.Raycast(transform.position, transform.TransformVector(sensor), sensorLength);
			return (double)(hit.collider == null ? sensorLength : hit.distance );
			});
	}

	private void OnDrawGizmos() {
		if (_distances != null) {
			for (var index = 0; index < _distances.Length; index++) {
				var distance = _distances[index];
				Gizmos.color = Color.red;
				var rotatedSensor = transform.TransformVector(sensors[index]);
				Gizmos.DrawLine(transform.position, transform.position + rotatedSensor * (float) distance);
			}
		}

		if (drawTrail) {
			foreach (var oldPosition in oldPositions) {
				Gizmos.DrawSphere(oldPosition, trailPointRadius);
			}
		}
	}
}