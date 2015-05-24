using UnityEngine;
using System.Collections;

public class Death_Floor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		PlayerController player = (PlayerController)other.GetComponent(typeof(PlayerController));
		if (player != null) {
			player.Die();
		}
	}
}
