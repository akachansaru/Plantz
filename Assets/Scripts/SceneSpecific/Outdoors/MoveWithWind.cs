using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithWind : MonoBehaviour
{
    //private Rigidbody rb;
    //private bool inWindArea = false;
    //private WindArea windArea;

    private void Start()
    {
        if (GetComponent<Collider>().isTrigger)
        {
            //Debug.LogWarning(gameObject.name + " is a trigger. Won't move with wind.");
        }
      //  rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //if (inWindArea)
        //{
        // PERFORMANCE: try adding in an if statement to check if it's in the camera view. Might not lag out so badly
        //    rb.AddForce(windArea.Direction * windArea.Strength);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstantValues.Tags.WindArea))
        {
            //windArea = other.GetComponent<WindArea>();
            //inWindArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ConstantValues.Tags.WindArea))
        {
            //inWindArea = false;
        }
    }
}
