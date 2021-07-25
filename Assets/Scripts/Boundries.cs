using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundries : MonoBehaviour
{
    public static Boundries instance;
    public BoxCollider2D WorldBoundries;
    public float maxStep = 5.0f;

    public Vector3 worldBoundsMin;
    public Vector3 worldBoundsMax;
    public Vector3 viewchange;

    float max_x, max_z, min_x, min_z;

    
    public GameObject terrain_;
    Vector3 ClampVector3(Vector3 value, Vector3 mins, Vector3 maxs)
     {
         value.x = Mathf.Clamp(value.x, mins.x, maxs.x);
         value.z = Mathf.Clamp(value.z, mins.z, maxs.z);
 
         return value;
     }
    // Start is called before the first frame update
    public void GetBounds()
    {
        terrain_.AddComponent<BoxCollider>();
        terrain_.GetComponent<BoxCollider>().isTrigger = true;

        max_x = terrain_.GetComponent<BoxCollider>().bounds.max.x;
        min_x = terrain_.GetComponent<BoxCollider>().bounds.min.x;

        max_z = terrain_.GetComponent<BoxCollider>().bounds.max.z;
        min_z = terrain_.GetComponent<BoxCollider>().bounds.min.z;
    }
    
     void Awake()
    {   
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        viewchange = transform.position;
        viewchange.x = Mathf.Clamp(viewchange.x, min_x, max_x);
        viewchange.z = Mathf.Clamp(viewchange.z, min_z, max_z);
        transform.position = viewchange;       
        
    }
}
