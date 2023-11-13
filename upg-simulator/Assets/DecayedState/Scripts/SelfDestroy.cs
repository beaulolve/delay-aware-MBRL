using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
    private AudioSource aSource;
    public AudioClip[] crashsound;

	// Use this for initialization
	void Start () {
        aSource = this.GetComponent<AudioSource>();
        int n = Random.Range(1, crashsound.Length);
        aSource.clip = crashsound[n];
        aSource.PlayOneShot(aSource.clip);
        Destroy(gameObject, 10f);

	}
}
