using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roadRayCast : MonoBehaviour
{
    private RaycastHit hit;
    public Quaternion targetRotation;
    public bool roadDetectFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int Rmask = LayerMask.GetMask("GroundLayer");
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10.0f, Rmask)) //&& (baseFunction.angleOffset(angle.x) > -5 || baseFunction.angleOffset(angle.x) < -35))
        {
            roadDetectFlag = true;
            Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);
            targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 200f);

        }
        else
            roadDetectFlag = false;
    }
}
