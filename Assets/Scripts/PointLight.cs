using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{
    public float orbitHeight;
    public Color color;

    // set radius of orbit
    void Start()
    {
        this.transform.position = new Vector3(0, orbitHeight, 0);
    }

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }
}
