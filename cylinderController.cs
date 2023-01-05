using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class cylinderController : MonoBehaviour
{
    public GameObject trigCollider;

    private GameObject cyl;
    public Canvas warn;
    public GameObject lHand;
    public GameObject rHand;
    Vector3 homePos;
    Vector3 homeForward;
    Quaternion homeRot;
    float rotX, rotY, rotZ;

    // Start is called before the first frame update
    void Start()
    {
        warn.enabled = false;
        cyl = gameObject;
        homePos = cyl.transform.position;
        homeRot = cyl.transform.rotation;
        homeForward = cyl.transform.right;
        rotX = cyl.transform.localEulerAngles.x;
        rotY = cyl.transform.localEulerAngles.y;
        rotZ = cyl.transform.localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (cyl.GetComponent<Rigidbody>() != null)
        {
            cyl.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cyl.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        Text ang = warn.transform.GetChild(1).GetComponent<Text>();
        ang.text = Vector3.Dot(cyl.transform.right, (homeForward - cyl.transform.right).normalized).ToString();
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.GetComponent<BoxCollider>().ToString());
        if (other.name == "snap-pos-cyl")
        {
            if (other.gameObject.tag == "Hand")
            {
                Physics.IgnoreCollision(other.gameObject.GetComponent<SphereCollider>(), GetComponent<Collider>());
            }
            cyl.GetComponent<Rigidbody>().velocity = Vector3.zero;
            cyl.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //Debug.Log("Hit");
            //Debug.Log(plate.transform.localEulerAngles.z);
            float dotProd = Vector3.Dot(cyl.transform.right, (homeForward - cyl.transform.right).normalized);
            Debug.Log(dotProd);

            //if (plate.transform.localEulerAngles.z > 90 && plate.transform.localEulerAngles.z < 270)
            if (dotProd > -0.707)
            {

                yield return new WaitForSecondsRealtime(3);
                cyl.transform.position = homePos;
                cyl.transform.rotation = homeRot;
                warn.enabled = false;
                yield return new WaitForSecondsRealtime(1.5f);

            }
            else
            {

                yield return new WaitForSecondsRealtime(3);
                cyl.transform.position = homePos;
                cyl.transform.localEulerAngles = new Vector3(rotX, cyl.transform.localEulerAngles.y, rotZ);
                warn.enabled = true;
                yield return new WaitForSecondsRealtime(1.5f);

            }

        }
    }


}
