using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour {
	
	//private Rigidbody2D rb2d;
	public float TTL;
	
	// Use this for initialization
	void Start () {
		//rb2d = GetComponent<Rigidbody2D>();
		TTL = 5f;
	}
	
	public void SetTTL(float ttl) {
		TTL = ttl;
	}
	
	// Update is called once per frame
	void Update () {
		TTL -= Time.deltaTime;
		if (TTL < 0){
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		PlayerController player = (PlayerController)other.GetComponent(typeof(PlayerController));
		if (player != null) {
			bool result = player.Hit (transform.position, 50f, 0.1f, 5f);
			// TODO(don): add feedback for end of bullet life
			TTL = 0;
		}
	}
}
