using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightFlicker : MonoBehaviour
{
	public float min = 0.5f;
	public float max = 0.5f;
	public bool useSmooth = false;
	public float smoothTime = 10.0f;
	public float intervalTime = 0.2f;

	private Light cachedLight;

	void Start()
	{
		cachedLight = GetComponent<Light>();

		if (!useSmooth && cachedLight != null)
		{
			StartCoroutine(FlickerCoroutine());
		}
	}

	private void Update()
	{
		if (useSmooth && cachedLight != null)
		{
			cachedLight.intensity = Mathf.Lerp(cachedLight.intensity, GetRandomInRange(min, max), Time.deltaTime * smoothTime);
		}
	}

	private IEnumerator FlickerCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(intervalTime);

			if (cachedLight != null)
			{
				cachedLight.intensity = GetRandomInRange(min, max);
			}
		}
	}

	private static float GetRandomInRange(float min, float max)
	{
		rand = rand ?? new System.Random();

		return (float)(rand.NextDouble()) * (max - min) + min;
	}
	private static System.Random rand;
}
