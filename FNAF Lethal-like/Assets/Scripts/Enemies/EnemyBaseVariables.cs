using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    [SerializeField]
    private float base_health;
    public float BaseHealth { get { return base_health; } }

    [SerializeField]
    private float base_speed;
    public float BaseSpeed { get { return base_speed; } }

}
