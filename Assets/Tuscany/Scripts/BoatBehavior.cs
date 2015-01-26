using UnityEngine;
using System.Collections;

public class BoatBehavior : MonoBehaviour
{
	public Transform[] waypoints;
	public float speed;
	public float turnSpeed;

	private float arrivalSqrDistance = 25.0f;
	private int currentWaypointIndex;
	private Transform cachedTransform;

	void Start()
	{
		currentWaypointIndex = 0;
		cachedTransform = transform;
	}
	
	void FixedUpdate()
	{
		Vector3 lookDir = waypoints[currentWaypointIndex].position - cachedTransform.position;
		Quaternion lookRot = Quaternion.LookRotation(lookDir);
		float alpha = Time.time * (turnSpeed / 1000.0f);

		cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, lookRot, alpha);
		cachedTransform.localEulerAngles = new Vector3(270.0f, cachedTransform.localEulerAngles.y, 0.0f);
		cachedTransform.Translate(-Vector3.up * Time.deltaTime * speed, Space.Self);

		if (lookDir.sqrMagnitude < arrivalSqrDistance)
		{
			currentWaypointIndex = (++currentWaypointIndex) % waypoints.Length;
		}
	}
}
