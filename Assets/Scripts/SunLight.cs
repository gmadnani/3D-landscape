using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;
// attach this script to the an object at the centre of the terrain and the script will create a sun orbitting arount that point
public class SunLight : MonoBehaviour {
    //change the speed of rotation of the sun here
    public float spinSpeed;
    // Update is called once per frame
    void Start()
    {
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ////change position of sun here
        //sphere.transform.position = new Vector3(0, 10f, 0);
        //Light bli = sphere.gameObject.AddComponent<Light>();
        //bli.type = UnityEngine.LightType.Directional;
        //sphere.transform.eulerAngles = new UnityEngine.Vector3(90, 0, 0);
        //sphere.transform.parent = this.transform;
    }
    void Update () {
		this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, Vector3.right);
	}
}
