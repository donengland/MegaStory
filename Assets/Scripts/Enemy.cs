using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IHitable  {

	public AudioClip audio_hit;
	public AudioClip audio_fire;

	private Rigidbody2D rb2d;
	public Rigidbody2D enemy_bullet;
	public float bullet_x_force;
	public float bullet_y_force;

	public float health;
	public float collision_damage = 5f;
	public float vulnerable_timer = 0f;
	public float vulnerable_cooldown = 0.5f;
	// TODO(don): hit_timer should inhibit enemy movement?
	public float hit_timer = 0f;

	public bool fire_weapon = true;
	public float fire_rate = 2f;
	private float fire_count = 2f;

	public float velocity_x = -1.5f;
	public float velocity_y_max = 1.5f;
	public float velocity_y_current = 1.5f;
	public float velocity_y_accel = 1f;
	public bool velocity_y_pos = false;

	public enum EnemyType {Ground, Flyer};
	public EnemyType my_type = EnemyType.Ground;

	private Transform player_transform;

	private Animator anim;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		player_transform = GameObject.FindGameObjectWithTag("Player").transform;
	}

	public bool Hit(Vector2 hit_location, float hit_power, float hit_duration, float damage)
	{
		bool result = false;
		if (vulnerable_timer <= 0){
			if (my_type != EnemyType.Flyer) {
				rb2d.AddForce(new Vector2(((hit_location.x < transform.position.x) ? hit_power : -hit_power), 50f));
			}
			health -= damage;
			hit_timer = hit_duration;
			vulnerable_timer = vulnerable_cooldown;
			AudioSource.PlayClipAtPoint(audio_hit, transform.position);
			anim.SetBool ("Hit", true);
			result = true;
		}
		return result;
	}
	
	// Update is called once per frame
	void Update () {
		if (vulnerable_timer >= 0) {
			vulnerable_timer -= Time.deltaTime;
			if (vulnerable_timer <= 0) {
				anim.SetBool ("Hit", false);
			}
		}
		if (hit_timer > 0) {
			hit_timer -= Time.deltaTime;
		}
		if (health <= 0 || (Mathf.Abs(transform.position.x - player_transform.position.x) > 10)) {
			Destroy(gameObject);
		}
		if (velocity_y_pos) {
			velocity_y_current += Time.deltaTime * velocity_y_accel;
			if (velocity_y_current >= velocity_y_max) {
				velocity_y_pos = false;
			}
		} else {
			velocity_y_current -= Time.deltaTime * velocity_y_accel;
			if (velocity_y_current <= -velocity_y_max) {
				velocity_y_pos = true;
			}
		}
		if (fire_count < fire_rate) {
			fire_count += Time.deltaTime;
		}
		if (fire_count >= fire_rate && fire_weapon) {			
			AudioSource.PlayClipAtPoint(audio_fire, transform.position);
			fire_count = 0f;
			Rigidbody2D bullet_instance = Instantiate(enemy_bullet, transform.position, transform.rotation) as Rigidbody2D;
			bullet_instance.AddForce(new Vector2(((velocity_x > 0) ? bullet_x_force: -bullet_x_force), bullet_y_force));
		}
	}

	void FixedUpdate() {
		switch(my_type) {
			case EnemyType.Ground: {
				if (hit_timer <= 0) {
					rb2d.velocity = new Vector2(velocity_x, rb2d.velocity.y);
				}
				break;
			}
			case EnemyType.Flyer: {
				rb2d.velocity = new Vector2(velocity_x, velocity_y_current);
				break;
			}
		}
	}

	void OnCollisionStay2D(Collision2D other) {
		PlayerController pc = (PlayerController)other.gameObject.GetComponent(typeof(PlayerController));
		if (pc != null) {
			pc.Hit(transform.position, 600f, 0.1f, collision_damage);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.transform.position.y >= (transform.position.y -0.1f)){
			if (other.collider.transform.position.x > transform.position.x) {
				if (velocity_x > 0){ velocity_x = -velocity_x; }
			} else if (other.collider.transform.position.x <= transform.position.x) {
				if (velocity_x < 0){ velocity_x = -velocity_x; }
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		PlayerController pc = (PlayerController)other.gameObject.GetComponent(typeof(PlayerController));
		if (pc != null) {
			pc.Hit(transform.position, 600f, 0.1f, collision_damage);
		}
	}
}
