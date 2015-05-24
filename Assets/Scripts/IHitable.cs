using UnityEngine;
using System.Collections;

public interface IHitable {
	bool Hit(Vector2 hit_location, float hit_power, float hit_duration, float damage);
}
