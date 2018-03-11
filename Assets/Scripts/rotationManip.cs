using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationManip : MonoBehaviour {

    public Camera cam;
    public GameObject Target;

    enum RotationMode {None, Free, X, Y, Z};

    private RotationMode mode = RotationMode.None;
    private Vector3 initialSpherePoint;
    private GameObject sphere = null;

    // Use this for initialization
	void Start () {
		
	}



    // Update is called once per frame
    void Update() {
        RaycastHit hit = new RaycastHit();
        Vector3 center = Target.transform.position;
        float radius = .2f * Mathf.Sin(cam.fieldOfView) / 4 * (center - cam.transform.position).magnitude;

        if (Input.GetMouseButtonDown(0)) {
            if (!sphereRayIntersect(cam.ScreenPointToRay(Input.mousePosition), center, radius, hit))
                return;
            initialSpherePoint = hit.point;
            
            return;
        }
        


    }

    /// <summary>
    /// Return intersection point between ray and sphere.
    /// 
    /// In case you have to re-derive, use inner product multi-linearity
    /// ||p-td||^2 = dot(p,p) + 2t dot(p,d) + t^2 dot(p,d) = r^2
    /// </summary>
    /// <param name="ray">The ray</param>
    /// <param name="origin">Sphere origin</param>
    /// <param name="r">Sphere radius</param>
    /// <returns></returns>
    bool sphereRayIntersect(Ray ray, Vector3 origin, float r, RaycastHit hit) {
        Vector3 p = ray.origin - origin;
        float b = 2*Vector3.Dot(p, ray.direction);
        float a = ray.direction.sqrMagnitude;
        float c = p.sqrMagnitude - r*r;
        float desc = Mathf.Sqrt(b*b - 4*a*c);
        if (desc < 0)
            return false;
        float t1 = (-b + desc) / (2 * a);
        float t2 = (-b - desc) / (2 * a);
        if (t1 < t2 || t2 < 0)
            hit.distance = t1;
        else
            hit.distance = t2;
        hit.point = ray.origin + ray.direction*hit.distance;
        return true;
	}

   
}
