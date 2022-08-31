using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class plateController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject trigCollider;
    private GameObject plate;
    public Canvas warn;
    public GameObject lHand;
    public GameObject rHand;
    Vector3 homePos;
    Vector3 homeForward;
    Quaternion homeRot;
    float rotX, rotY;

    // Set all necessasry game objects to their variables
    void Start()
    {
        warn.enabled = false;
        plate = gameObject;
        homePos = plate.transform.position;
        homeRot = plate.transform.rotation;
        homeForward = plate.transform.right;
        rotX = plate.transform.localEulerAngles.x;
        rotY = plate.transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the plate's velocity to zero, meaning it will not continute in direction of travel when released
        plate.GetComponent<Rigidbody>().velocity = Vector3.zero;
        plate.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // For testing only
        Text ang = warn.transform.GetChild(1).GetComponent<Text>();
        ang.text = Vector3.Dot(plate.transform.right, (homeForward - plate.transform.right).normalized).ToString();
        //warn.enabled = true;

    }

    // When the plate enters the trigger area of the feeder, it will check to see if it is oriented correctly.
    // If it is oriented correctly, then it will snap to the ready position and no warnings will display.
    // If it is oriented incorrectly, then it will snap to the closed incorrect angle and display a warning to the user.
    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.name == "snap-pos")
        {
            if (other.gameObject.tag == "Hand")
            {
                Physics.IgnoreCollision(other.gameObject.GetComponent<SphereCollider>(), GetComponent<Collider>());
            }
            plate.GetComponent<Rigidbody>().velocity = Vector3.zero;
            plate.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //Debug.Log("Hit");
            //Debug.Log(plate.transform.localEulerAngles.z);
            float dotProd = Vector3.Dot(plate.transform.right, (homeForward - plate.transform.right).normalized);
            Debug.Log(dotProd);

            //if (plate.transform.localEulerAngles.z > 90 && plate.transform.localEulerAngles.z < 270)
            if (dotProd > -0.707)
            {
                yield return new WaitForSecondsRealtime(3);
                plate.transform.position = homePos;
                plate.transform.rotation = homeRot;
                warn.enabled = false;
                yield return new WaitForSecondsRealtime(1.5f);
            }
            else
            {
                yield return new WaitForSecondsRealtime(3);
                plate.transform.position = homePos;
                plate.transform.localEulerAngles = new Vector3(rotX, rotY, plate.transform.localEulerAngles.z);
                warn.enabled = true;
                yield return new WaitForSecondsRealtime(1.5f);

            }

        }
    }

    // When the plate hits the ground, it will stop its movement so that it does not clip through.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            plate.GetComponent<Rigidbody>().velocity = Vector3.zero;
            plate.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            plate.GetComponent<Rigidbody>().useGravity = false;

        }
    }




}
