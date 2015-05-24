using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

	[HideInInspector] public bool facing_right = true;
	public bool jump = false;

	public AudioClip audio_jump;
	public AudioClip audio_fire;
	public AudioClip audio_land;
	public AudioClip audio_hit;
	public AudioClip audio_death;

	public Canvas game_canvas;
	private Animator canvas_anim;

	public float total_health = 100f;
	public float current_health = 100f;
	private bool dead = false;
	public float vulnerable_timer = 0f;
	public float vulnerable_cooldown = 1.5f;
	public float hit_timer = 0f;

	public float move_force = 200f;
	public float max_speed = 3.5f;
	public float jump_force = 600f;
	public float look_ahead_target = 2.5f;
	public float camera_speed = 0.02f;
	public float bullet_force = 400f;
	public float bullet_cone_range = 30f;
	public float fire_rate = 0.07f;
	public float fire_kickback = 20f;
	public Transform ground_check_front;
	public Transform ground_check_back;
	public Transform player_camera_root;
	public Transform player_camera;
	public Transform player_rifle_exit;
	public Rigidbody2D player_bullet;

	private bool grounded = false;
	private bool previously_grounded = true;
	private float look_ahead_current = 0f;
	private float fire_count = 0f;
	private Animator anim;
	private Rigidbody2D rb2d;
	private CameraController player_camera_controller;

	void Awake() {
		anim = GetComponent<Animator>();
		canvas_anim = game_canvas.GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		rb2d.velocity = new Vector2(0,0);
		look_ahead_current = look_ahead_target;
	}
	void Start () {
		player_camera_controller = (CameraController)player_camera.GetComponent<CameraController>();
	}
	void Update () {
		if (current_health > 0) {
			if (vulnerable_timer > 0) {
				vulnerable_timer -= Time.deltaTime;
			}
			if (hit_timer > 0 ) {
				hit_timer -= Time.deltaTime;
			}
			if (fire_count < fire_rate) {
				fire_count += Time.deltaTime;
			}
			grounded = Physics2D.Linecast (transform.position, ground_check_back.position, 1 << LayerMask.NameToLayer("Ground"));
			if (!grounded) {
				grounded = Physics2D.Linecast (transform.position, ground_check_front.position, 1 << LayerMask.NameToLayer("Ground"));
			}

			if (!grounded) {
				previously_grounded = false;
			}

			if (grounded && (previously_grounded == false)) {
				previously_grounded = true;
				if (player_camera_controller != null) {
					player_camera_controller.Landed();				
					AudioSource.PlayClipAtPoint(audio_land, transform.position);
				}
			}

			if (Input.GetButtonDown("Jump") && grounded) {
				jump = true;
			}
			float camera_distance = Mathf.Abs(look_ahead_current - look_ahead_target);
			float camera_adjusted_speed = (camera_distance/Mathf.Abs(look_ahead_target)) * camera_speed;
			if (camera_distance > camera_speed) {
				if (look_ahead_target > look_ahead_current) {
					look_ahead_current += camera_adjusted_speed;
				} else {			
					look_ahead_current -= camera_adjusted_speed;
				}
			}
			float cam_root_y = player_camera_root.transform.position.y;
			if (transform.position.y < (cam_root_y - 6)) {
				cam_root_y -= 6;
			}
			player_camera_root.transform.position = new Vector3(transform.position.x + look_ahead_current,
			                                                    cam_root_y,
			                                                    player_camera_root.transform.position.z);;

			if (Input.GetButton ("Fire1")) {
				if (fire_count >= fire_rate) {
					if (player_camera_controller != null) {
						player_camera_controller.Shake();
					}
					AudioSource.PlayClipAtPoint(audio_fire, transform.position);
					rb2d.AddForce(new Vector2((facing_right ? -fire_kickback : fire_kickback), 0f));
					fire_count = 0f;
					Rigidbody2D bullet_instance = Instantiate(player_bullet, player_rifle_exit.position, player_rifle_exit.rotation) as Rigidbody2D;
					float bullet_y_force = Random.Range(-bullet_cone_range,bullet_cone_range);
					bullet_instance.AddForce(new Vector2((facing_right ? bullet_force: -bullet_force), bullet_y_force));
				}
				//float bullet_x = (facing_right) ? bullet_force: -bullet_force;
				//bullet_instance.velocity = new Vector2(bullet_x, 0);
				//PlayerBullet current_shot = bullet_instance.GetComponent<PlayerBullet>();
				//if (current_shot != null) {
					//current_shot.SetTTL(5f);
					//float bullet_x = (facing_right) ? bullet_force: -bullet_force;
					//current_shot.SetVelocity(new Vector2(bullet_x, 0));
				//}
			}
		}
	}
	void FixedUpdate() {
		if (current_health > 0 ) {
			if (hit_timer <= 0) {
				float horizontal = Input.GetAxis("Horizontal");
				anim.SetFloat("Speed", Mathf.Abs(horizontal));
				if (Mathf.Abs(horizontal) < 0.1) {
					rb2d.velocity = new Vector2(0, rb2d.velocity.y);
				} else {
					rb2d.velocity = new Vector2(max_speed * horizontal, rb2d.velocity.y);

					if (horizontal > 0 && !facing_right)
						Flip ();
					if (horizontal < 0 && facing_right)
						Flip ();
				}
				if (jump) {
					anim.SetTrigger("Jump");
					AudioSource.PlayClipAtPoint(audio_jump, transform.position);
					rb2d.AddForce(new Vector2(0f, jump_force));
					jump = false;
				}
			}
		}
	}
	void Flip() {
		// Flip Direction
		// TODO(don): add directional feedback in animation. MECANIM
		look_ahead_target = -look_ahead_target;
		// TODO(don): remove rotation when bullet flipping is solved
		//player_rifle_exit.Rotate(180f, 0f, 0f);
		facing_right = !facing_right;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	public bool Hit(Vector2 hit_location, float hit_power, float hit_duration, float damage) {
		bool result = false;
		if (vulnerable_timer <= 0){
			rb2d.AddForce(new Vector2(((hit_location.x < transform.position.x) ? hit_power : -hit_power), 300f));
			current_health -= damage;
			canvas_anim.SetFloat("Health", (current_health / total_health));
			if (current_health <= 0 ) {
				Die();
			} else {
				hit_timer = hit_duration;
				vulnerable_timer = vulnerable_cooldown;
				AudioSource.PlayClipAtPoint(audio_hit, transform.position);
				anim.SetTrigger("Hit");
			}
			result = true;
		}
		return result;
	}

	public void Die() {
		if (dead == false) {
			current_health = 0;
			canvas_anim.SetFloat("Health", 0f);
			anim.SetTrigger("Death");
			rb2d.velocity = new Vector2(0f,0f);
			rb2d.isKinematic = true;
			vulnerable_timer = 100f;
			AudioSource.PlayClipAtPoint(audio_death, transform.position);
			Instantiate(Resources.Load("Explode_Collection", typeof(GameObject)), transform.position, transform.rotation);
			Invoke ("Restart", 3);
			dead = true;
		}
	}

	private void Restart() {
		Application.LoadLevel("Main");
	}
}
