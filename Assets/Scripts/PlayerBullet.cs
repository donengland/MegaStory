using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour {
	
	//private Rigidbody2D rb2d;
	public float TTL;

	// Use this for initialization
	void Start () {
		//rb2d = GetComponent<Rigidbody2D>();
		TTL = 1.5f;
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
		IHitable enemy = (IHitable)other.GetComponent(typeof(IHitable));
		if (enemy != null) {
			bool result = enemy.Hit(transform.position, 50f, 0.1f, 1f);
			// TODO(don): add feedback for end of bullet life
			TTL = 0;
		}
	}
}
