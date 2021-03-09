using UnityEditor;
using UnityEngine;
/// 始终面向摄像机
/// </summary>
public class LookAt : MonoBehaviour
{
    public Transform Target;
 
    void Update()
    {
          if(Target)
          {
              Rot(Target);
          }
    }
 
    void Rot(Transform target)
    {
        Plane plane = new Plane(target.forward, target.position);
        float dis;
        Vector3 tar = target.position;
        if (plane.Raycast(new Ray(this.transform.position, -target.forward),out dis) == true)
        {
            tar = this.transform.position + (-target.forward * dis);
        }
        this.transform.LookAt(tar, Vector3.up);
    }
}