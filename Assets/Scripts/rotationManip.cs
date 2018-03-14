using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationManip : MonoBehaviour {

    public Camera cam;
    public GameObject target;
    public GameObject xHandle;
    public GameObject yHandle;
    public GameObject zHandle;
    public GameObject wHandle;

    enum RotationMode {None, Free, X, Y, Z};

    private RotationMode mode = RotationMode.None;
    private Vector3 lastSpherePoint;
    private GameObject sphere = null;
    private GameObject[] handles;
    private Color[] handleColors;
    private LineRenderer[] renderers;
    
    // Use this for initialization
    void Start () {
        handles = new GameObject[] { xHandle, yHandle, zHandle, wHandle};
        handleColors = new Color[] { Color.red, Color.green, Color.blue, Color.white };
        Quaternion[] rotations = new Quaternion[] { Quaternion.AngleAxis(90, new Vector3(0, 1, 0)), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), Quaternion.identity, Quaternion.AngleAxis(45, new Vector3(0, 1, 0)) };
        renderers = new LineRenderer[4];
        Vector3[] circlePositions = new Vector3[36];
        


        float incr = (2 * 3.14159f) / (circlePositions.Length);
        for (int i = 0; i < handles.Length; i++)
        {
            renderers[i] = handles[i].AddComponent<LineRenderer>();
       
            for (int j = 0; j < circlePositions.Length; j++)
                circlePositions[j] = Matrix4x4.Rotate(rotations[i]).MultiplyPoint3x4(new Vector3(Mathf.Sin(j * incr), Mathf.Cos(j * incr), 0));
       
            renderers[i].startWidth = 1f; renderers[i].endWidth = 1f;
            renderers[i].positionCount = circlePositions.Length;
            renderers[i].useWorldSpace = false;
            renderers[i].SetPositions(circlePositions);
            renderers[i].startColor = handleColors[i]; renderers[i].endColor = handleColors[i];
            renderers[i].loop = true;
            renderers[i].material = new Material(Shader.Find("Particles/Additive"));
            renderers[i].widthMultiplier = 0.01f;
        }
    }



    // Update is called once per frame
    void Update() {
        if (target == null)
            target = GameController.Instance.targetObject;
        
        transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        RaycastHit hit = new RaycastHit();
        Vector3 center = target.transform.position;
        float radius = 4f * Mathf.Sin(cam.fieldOfView) / 4 * (center - cam.transform.position).magnitude;
        this.transform.localScale = new Vector3(radius, radius, radius);
        if (Input.GetMouseButtonDown(0)) {
            if (!sphereRayIntersect(cam.ScreenPointToRay(Input.mousePosition), center, radius, ref hit))
                return;

            lastSpherePoint = hit.point;
            Debug.Log(hit.point);
            mode = RotationMode.Free;
            return;
        }

        if (Input.GetMouseButtonUp(0))
            mode = RotationMode.None;
        
        
        switch (mode)
        {
            case RotationMode.Free:
                if (sphereRayIntersect(cam.ScreenPointToRay(Input.mousePosition), center, radius, ref hit))
                {
                    target.transform.Rotate(Vector3.Cross((lastSpherePoint - center).normalized, (hit.point - center).normalized), Vector3.Dot((lastSpherePoint - center).normalized, (hit.point - center).normalized), Space.World);
                    //Debug.Log("La");
                    lastSpherePoint = hit.point;
                } else
                {

                }
                break;
        }

       
        


    }

    /// <summary>
    /// Return intersection point between ray and sphere. [Unoptimized]
    /// 
    /// In case you have to re-derive, use inner product multi-linearity
    /// ||p-td||^2 = dot(p,p) + 2t dot(p,d) + t^2 dot(p,d) = r^2
    /// </summary>
    /// <param name="ray">The ray</param>
    /// <param name="origin">Sphere origin</param>
    /// <param name="r">Sphere radius</param>
    /// <returns></returns>
    bool sphereRayIntersect(Ray ray, Vector3 origin, float r, ref RaycastHit hit) {
        Vector3 p = ray.origin - origin;
        float b = 2*Vector3.Dot(p, ray.direction);
        float a = ray.direction.sqrMagnitude;
        float c = p.sqrMagnitude - r*r;
        float desc = b*b - 4*a*c;
        if (desc < 0)
            return false;
        desc = Mathf.Sqrt(desc);
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
