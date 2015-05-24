using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float shake_distance = 0.01f;
	public float jump_drop_amount = 0.08f;
	public float jump_return_speed = 0.5f;

	private float current_y;
	private float change_x;
	private float change_y;

	// Use this for initialization
	void Start () {
	
	}

	public void Shake() {
		change_x = Random.Range(-shake_distance, shake_distance);		
		change_y = Random.Range(-shake_distance, shake_distance);
		transform.localPosition = new Vector3 ( change_x, (current_y + change_y), 0f);
	}
	public void Landed() {
		current_y = jump_drop_amount;
		Shake();
	}
	
	// Update is called once per frame
	void Update () {
		if (current_y > 0f) {
			current_y -= jump_return_speed * Time.deltaTime;
		} else {
			current_y = 0f;
		}
		transform.localPosition = new Vector3 (change_x, (change_y + current_y), 0f);
	}
}
