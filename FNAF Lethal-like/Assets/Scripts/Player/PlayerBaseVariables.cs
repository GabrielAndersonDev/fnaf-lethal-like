using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Player : MonoBehaviour {

    [SerializeField]
    private float base_health;
    public float BaseHealth { get { return base_health; } }

    [SerializeField]
    private float base_movement_speed;
    public float BaseMovementSpeed { get { return base_movement_speed; } }

    [SerializeField]
    private float base_stamina;
    public float BaseStamina { get { return base_stamina; } }
}
