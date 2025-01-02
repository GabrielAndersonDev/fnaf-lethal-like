using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class Entity : MonoBehaviour
{
    //[HideInInspector]
    //public UnityEvent<float, float, float> OnHealthChange = new();

   public virtual void DestroyEntity()
    {
        GameObject.Destroy(gameObject);
    }
}
