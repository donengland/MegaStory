using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Animator))]
public class Boss : MonoBehaviour, IHitable  {
	
	public AudioClip audio_hit;
	public AudioClip audio_fire;

	public Rigidbody2D enemy_bullet;
	private Rigidbody2D rb2d;
	public float bullet_force;
	
	public float health;
	public float collision_damage = 5f;
	public float vulnerable_timer = 0f;
	public float vulnerable_cooldown = 0.5f;
	// TODO(don): hit_timer should inhibit enemy movement?
	public float hit_timer = 0f;
	
	public bool fire_weapon = true;
	public bool left = true;
	public float fire_rate = 2f;
	private float fire_count = 0f;

	public float jump_force = 600f;
	public float jump_timer = 0f;
	
	public float velocity_x = -2f;
	public float velocity_y_max = 1.5f;
	public float velocity_y_current = 1.5f;
	public float velocity_y_accel = 1f;
	public bool velocity_y_pos = false;
	
	private Transform player_transform;
	
	private Animator anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		player_transform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	public bool Hit(Vector2 hit_location, float hit_power, float hit_duration, float damage)
	{
		bool result = false;
		if (vulnerable_timer <= 0){
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
		if (health <= 0){// || (Mathf.Abs(transform.position.x - player_transform.position.x) > 10)) {			
			Instantiate(Resources.Load("Explode_Collection", typeof(GameObject)), transform.position, transform.rotation);
			Destroy(gameObject);
		}
		if (fire_count < fire_rate) {
			fire_count += Time.deltaTime;
		}
		if (fire_count >= fire_rate && fire_weapon) {
			fire_count = Random.Range (-2f, 0f);
			ShootPlayer();
		}

		if (jump_timer <= 0) {
			rb2d.AddForce(new Vector2(0f, jump_force));
			jump_timer = Random.Range (1.0f, 3.0f);
		} else {
			jump_timer -= Time.deltaTime;
		}
	}
	
	void FixedUpdate() {
		rb2d.velocity = new Vector2(velocity_x, rb2d.velocity.y);
	}

	public void ShootPlayer() {
		AudioSource.PlayClipAtPoint(audio_fire, transform.position);
		Rigidbody2D bullet_instance = Instantiate(enemy_bullet, transform.position, transform.rotation) as Rigidbody2D;
		Vector2 vector_to_player = new Vector2((player_transform.position.x - transform.position.x),
		                                       ((player_transform.position.y + 0.5f)- transform.position.y)).normalized * bullet_force;
		bullet_instance.AddForce(vector_to_player);
	}

	public void ChooseAction() {
		int choice = Random.Range(0,3);
		switch(choice) {
		case 0:
			anim.SetTrigger("Idle");
			break;
		case 1:
			anim.SetTrigger("Jump");
			break;
		case 2:
			left = !left;
			anim.SetBool("Left", left);
			anim.SetTrigger("SwitchSides");
			break;
		}
	}

	public void Idle() {
		anim.SetTrigger("Idle");
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
}
