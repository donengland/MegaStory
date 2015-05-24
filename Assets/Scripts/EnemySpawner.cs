using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	public Rigidbody2D enemy_to_spawn;
	public Transform spawn_location;

	public int number_of_enemies = 1;

	public float spawn_timer = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(spawn_timer > 0f) {
			spawn_timer -= Time.deltaTime;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		PlayerController player = (PlayerController)other.GetComponent(typeof(PlayerController));
		if (player != null && (number_of_enemies > 0)) {
			if (spawn_timer <= 0f) {
				spawn_timer = 2f;
				Instantiate(enemy_to_spawn, spawn_location.position, spawn_location.rotation);
				number_of_enemies--;
			}
		}
	}
}
