using UnityEngine;
using System.Collections;

public class BirdsBehavior : MonoBehaviour
{
	public Transform birdsPrefab;
	private float birdTimer;
	
	void Start()
	{
		birdTimer = GetRandomInRange(2, 5);
	}
	
	void Update()
	{
		if (birdTimer < Time.time)
		{
			EmitBirds();

			birdTimer = Time.time + GetRandomInRange(5, 20);
		}
	}
	
	void EmitBirds()
	{
		Transform birds = Instantiate(birdsPrefab, transform.position, transform.rotation) as Transform;

		ParticleAnimator animator = birds.GetComponentInChildren<ParticleAnimator>();
		animator.force = new Vector3(0, GetRandomInRange(-0.3f, 0.3f), 0);

		ParticleEmitter emitter = birds.GetComponentInChildren<ParticleEmitter>();
		emitter.emit = true;
	}

	private static float GetRandomInRange(float min, float max)
	{
		rand = rand ?? new System.Random();

		return (float)(rand.NextDouble()) * (max - min) + min;
	}
	private static System.Random rand;
}
