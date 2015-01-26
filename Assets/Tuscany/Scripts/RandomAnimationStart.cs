using UnityEngine;
using System.Collections;

public class RandomAnimationStart : MonoBehaviour
{
	void Awake()
	{
		Animation anim = GetComponent<Animation>();
		anim[anim.clip.name].normalizedTime = Random.Range(0.0f, 1.0f);
	}
}
