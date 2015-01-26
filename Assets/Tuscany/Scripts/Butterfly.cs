using UnityEngine;
using System.Collections;

public class Butterfly : MonoBehaviour
{
	public float updateRate = 0.35f;

	private Transform cachedTransform;
	private float updateHeadingTime;
	private Vector3 heading;
	private Vector3 lookAtPos;
	private Vector3 prevPos;

	void Start()
	{
		cachedTransform = transform;
		prevPos = cachedTransform.position;
		updateHeadingTime = Time.time;
	}
	
	void Update()
	{
		if (Time.time > updateHeadingTime)
		{
			updateHeadingTime = Time.time + updateRate;

			heading = cachedTransform.position - prevPos;
			prevPos = cachedTransform.position;
		}

		lookAtPos = Vector3.Lerp(lookAtPos, cachedTransform.position + (heading * 10), Time.deltaTime * 5);
	
		cachedTransform.LookAt(lookAtPos);
	}
}
