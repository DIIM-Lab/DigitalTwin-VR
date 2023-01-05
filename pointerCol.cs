using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WEBXR_INPUT_PROFILES
using WebXRInputProfile;
#endif
using UnityEngine.XR;
using WebXR;
using System;




public class pointerCol : MonoBehaviour
{
    //private WebXR.WebXRController controller;
    //WebXRController controller;
    //private WebXRController controller;
    //private WebXRController controller;
    
    bool click = false;
    public GameObject player;
    public GameObject menu;
    public GameObject arm;
    public Arm armScript;
    public Material hover;
    public Material normal;

    //public GameObject hand;
    public WebXRController controller;

    public GameObject cylMarker;
    public GameObject plateMarker;
    public GameObject assemMarker;


    Vector3 homeRot = new Vector3(0, 180f, 0);
    Vector3 homePos = new Vector3(-55.2f, -50.3f, 66.3f);
    Vector3 cylRot = new Vector3(0, 90f, 0);
    //Vector3 cylPos = new Vector3(56.4f, -50.3f, 63.5f);
    Vector3 cylPos;
    Vector3 plateRot = new Vector3(0, 270, 0);
    //Vector3 platePos = new Vector3(-147.2f, -50.3f, 43.4f);
    Vector3 platePos;
    Vector3 assemblyRot = new Vector3(0, 180f, 0);
    //Vector3 assemblyPos = new Vector3(10.1f, -50.3f, -14f);
    Vector3 assemblyPos;

    Vector3 menuHomeRot = new Vector3(0, 45f, 0);
    Vector3 menuHomePos = new Vector3(-127.3f, 63.53f, 58.4F);
    Vector3 menuCylRot = new Vector3(0, 270f, 0);
    Vector3 menuCylPos = new Vector3(88.4f, 63.5f, 60f);
    Vector3 menuPlateRot = new Vector3(0, 90f, 0);
    Vector3 menuPlatePos = new Vector3(-192.1f, 51f, 34.7f);
    Vector3 menuAssemblyRot = new Vector3(0, 0, 0);
    Vector3 menuAssemblyPos = new Vector3(26f, 63.5f, -35.1f);




    // Start is called before the first frame update
    void Start()
    {
        cylPos = cylMarker.transform.position;
        platePos = plateMarker.transform.position;
        assemblyPos = assemMarker.transform.position;

        armScript = arm.GetComponent<Arm>();
        //controller = hand.GetComponent<WebXRController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("down");
            click = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("up");
            click = false;
        }


        //if (controller.GetButtonDown("Trigger"))
        //    print(hand + " controller Trigger is down!");

        //if (controller.GetButtonUp("Trigger"))
        //    print(hand + " controller Trigger is up!");

        try
        {
            if (controller != null)
            {
                if (controller.GetButtonDown(WebXRController.ButtonTypes.ButtonA))
                {
                    click = true;
                    //Debug.Log("Pressed!!!");
                }
            }
        } catch(NullReferenceException err)
        {

        }

        //if (controller.GetButtonUp(WebXRController.ButtonTypes.Trigger)
        //  || controller.GetButtonUp(WebXRController.ButtonTypes.Grip)
        //  || controller.GetButtonUp(WebXRController.ButtonTypes.ButtonA))
        //{
        //    click = false;
        //}
    }


    private IEnumerator OnTriggerStay(Collider other)
    {
        if (other.name == "CylButton" && !armScript.liveSimulation)
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = hover;
            if (click)
            {
                //Debug.Log("click");
                click = false;

                player.transform.position = cylPos;
                player.transform.eulerAngles = cylRot;

                menu.transform.position = menuCylPos;
                menu.transform.eulerAngles = menuCylRot;

                armScript.platePlacing = true;
                armScript.beginTrainingScenario = false;
                armScript.trainingScenario = false;


                yield return new WaitForSecondsRealtime(1.5f);

            }
        }
        else if (other.name == "HomeButton" && !armScript.liveSimulation)
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = hover;
            if (click)
            {
                //Debug.Log("click");
                click = false;

                player.transform.position = homePos;
                player.transform.eulerAngles = homeRot;

                menu.transform.position = menuHomePos;
                menu.transform.eulerAngles = menuHomeRot;

                armScript.platePlacing = false;
                armScript.beginTrainingScenario = false;
                armScript.trainingScenario = false;


                yield return new WaitForSecondsRealtime(1.5f);

            }
        }
        else if (other.name == "PlateButton" && !armScript.liveSimulation)
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = hover;
            if (click)
            {
                //Debug.Log("click");
                click = false;

                player.transform.position = platePos;
                player.transform.eulerAngles = plateRot;

                menu.transform.position = menuPlatePos;
                menu.transform.eulerAngles = menuPlateRot;

                armScript.platePlacing = true;
                armScript.beginTrainingScenario = false;
                armScript.trainingScenario = false;

                yield return new WaitForSecondsRealtime(1.5f);

            }
        }
        else if (other.name == "AsButton" && !armScript.liveSimulation)
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = hover;
            if (click)
            {
                Debug.Log("click");
                click = false;

                player.transform.position = assemblyPos;
                player.transform.eulerAngles = assemblyRot;

                menu.transform.position = menuAssemblyPos;
                menu.transform.eulerAngles = menuAssemblyRot;

                armScript.platePlacing = false;
                armScript.beginTrainingScenario = true;

                yield return new WaitForSecondsRealtime(1.5f);

            }
        }
        else if (other.name == "ConnectionMode")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = hover;
            if (click)
            {
                //Debug.Log("click");
                click = false;

                //player.transform.position = homePos;
                //player.transform.eulerAngles = homeRot;

                //menu.transform.position = menuHomePos;
                //menu.transform.eulerAngles = menuHomeRot;

                player.transform.position = homePos;
                player.transform.eulerAngles = homeRot;

                menu.transform.position = menuHomePos;
                menu.transform.eulerAngles = menuHomeRot;

                armScript.platePlacing = false;
                armScript.beginTrainingScenario = false;
                armScript.trainingScenario = false;

                if (armScript.liveSimulation)
                {
                    armScript.liveSimulation = false;
                    menu.transform.Find("ConnectionMode").GetComponent<TextMesh>().text = "Online Mode";
                } 
                else
                {
                    armScript.liveSimulation = true;
                    menu.transform.Find("ConnectionMode").GetComponent<TextMesh>().text = "Offline Mode";

                }



                yield return new WaitForSecondsRealtime(1.5f);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "CylButton")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = normal;
            
        }
        else if (other.name == "HomeButton")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = normal;

        }
        else if (other.name == "PlateButton")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = normal;

        }
        else if (other.name == "AsButton")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = normal;

        }
        else if (other.name == "ConnectionMode")
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().material = normal;

        }
    }
}
