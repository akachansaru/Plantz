//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// From Spherical Gravity (Walk, Jump on Sphere/Planet) in Unity 3D by Learn Everything Fast on Youtube
//[RequireComponent(typeof(Rigidbody))]
//public class GravityBody : MonoBehaviour
//{

//    PlanetGravity planet;
//    Rigidbody objectRigidbody;

//    void Awake()
//    {
//        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
//        objectRigidbody = GetComponent<Rigidbody>();

//        // Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
//        objectRigidbody.useGravity = false;
//        objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
//    }

//    void FixedUpdate()
//    {
//        // Allow this body to be influenced by planet's gravity
//        planet.Attract(objectRigidbody);
//    }
//}
