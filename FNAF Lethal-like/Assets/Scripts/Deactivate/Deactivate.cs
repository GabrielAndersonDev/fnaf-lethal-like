using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Deactivate : MonoBehaviour
{
    public bool isDeactivated;

    public Enemy enemy;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        isDeactivated = !isDeactivated;
    }
}
