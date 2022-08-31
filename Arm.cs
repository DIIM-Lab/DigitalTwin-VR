using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase.Firestore;
using FirebaseWebGL.Scripts.FirebaseBridge;


public class Arm : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject arm;
    public GameObject rotator;
    public GameObject bicep;
    public GameObject wrist;
    public GameObject forearm;
    public GameObject hand;
    public GameObject finger1;
    public GameObject finger2;
    public GameObject floor;
    public GameObject pusher;

    public GameObject h;
    public GameObject h1;
    public GameObject h2;
    public GameObject crossbeam;


    public GameObject plate;
    public GameObject clone;

    bool begin = true;
    bool rot1 = false;
    bool rot2 = false;
    bool rot3 = false;
    bool rot4 = false;
    bool rot5 = false;
    bool moving = false;
    float sb, sw, sf;
    public bool test = true;
    public bool pistonAssembly = false;
    public bool position1 = false;
    public bool startOperation = false;
    public bool rawMaterialA = false;
    public bool rawMaterialB = false;

    Vector3 rot;
    
    // Structure to store all the flags from the Firebase database
    [System.Serializable]
    public struct Fields
    {
        public bool pistonAssembly;
        public bool position1;
        public bool rawMaterialA;
        public bool rawMaterialB;
        public bool startOperation;
    }
    private Fields fields;
    
    // Error function for firebase
    void errorFunction(string error)
    {
        Debug.Log(error);
    }

    // Sets local simulation variables according to the values of the Firebase flags.
    void documentData(string data)
    {
        //txt.text = data;
        Debug.Log(data);
        fields = JsonUtility.FromJson<Fields>(data);
        bool state = bool.Parse(fields.pistonAssembly.ToString().Trim());
        if (state == true)
        {
            Debug.Log("pistonAssembly: true");
            pistonAssembly = true;

        }
        else
        {
            Debug.Log("pistonAssembly: false");
            pistonAssembly = false;

        }
        state = bool.Parse(fields.position1.ToString().Trim());
        if (state == true)
        {
            Debug.Log("position1: true");
            position1 = true;

        }
        else
        {
            Debug.Log("position1: false");
            position1 = false;

        }
        state = bool.Parse(fields.rawMaterialA.ToString().Trim());
        if (state == true)
        {
            Debug.Log("rawMaterialA: true");
            rawMaterialA = true;

        }
        else
        {
            Debug.Log("rawMaterialA: false");
            rawMaterialA = false;
        }
        state = bool.Parse(fields.rawMaterialB.ToString().Trim());
        if (state == true)
        {
            Debug.Log("rawMaterialB: true");
            rawMaterialB = true;

        }
        else
        {
            Debug.Log("rawMaterialB: false");
            rawMaterialB = false;
        }
        state = bool.Parse(fields.startOperation.ToString().Trim());
        if (state == true)
        {
            Debug.Log("startOperation: true");
            startOperation = true;

        }
        else
        {
            Debug.Log("startOperation: false");
            startOperation = false;
        }
    }

    // Tracks changes of a specified Firestore database Document
    void onColChange(string output)
    {
        FirebaseFirestore.GetDocument("Scenario-2", "9GOwnU3Heub3NZL1eTNd", gameObject.name, "documentData", "errorFunction");
    }

    // This function runs onces at the beginning of the simulation and is responsible for setting all the required GameObject varibles
    void Start()
    {
        FirebaseFirestore.ListenForCollectionChange("Scenario-2", false, gameObject.name, "onColChange", "errorFunction");

        // Define the gameObjects for later use
        arm = gameObject;
        rotator = GameObject.Find("/Arm/Fillet5/Bottom Joint");
        bicep = GameObject.Find("/Arm/Fillet5/Bottom Joint/Robot Arm");
        forearm = GameObject.Find("Robot Forearm Base");
        wrist = GameObject.Find("Wrist");
        hand = GameObject.Find("Wrist Block");
        finger1 = GameObject.Find("Gripper1");
        finger2 = GameObject.Find("Gripper2");
        floor = GameObject.Find("Shape_IndexedFaceSet.072");
        pusher = GameObject.Find("Shape_IndexedFaceSet.1965");

        h = GameObject.Find("Holder Assembly");
        h1 = GameObject.Find("Holder Assembly 1");
        h2 = GameObject.Find("Holder Assembly 2");

        plate = GameObject.Find("plate");


        crossbeam = GameObject.Find("Crossbeam Assembly");

        rot = new Vector3(0, 0, -45);
        
    }
        
    // Update is called once per frame
    void Update()
    {
        if (startOperation)
        {
            GameObject.Find("snap-pos").GetComponent<BoxCollider>().enabled = false;
        } else
        if (!startOperation)
        {
            GameObject.Find("snap-pos").GetComponent<BoxCollider>().enabled = true;
        }
        // Begin the scenario
        if (begin)
        {
            begin = false;
            simp1();
        }
    }
    
    // Function to activate the raw material A pusher
    // Pusher will move forwards then return back to the starting position
    IEnumerator push()
    {
        float t = 0.0f;
        float d = 1;
        while (t < d)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(0, -10, t / d);
            float z = Mathf.Lerp(0, 10, t / d);
            pusher.transform.localPosition = new Vector3(x, pusher.transform.localPosition.y, z);
            yield return null;
        }
        t = 0.0f;
        plate.SetActive(true);
        yield return new WaitForSecondsRealtime(1);

        while (t < d)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(-10, 0, t / d);
            float z = Mathf.Lerp(10, 0, t / d);
            pusher.transform.localPosition = new Vector3(x, pusher.transform.localPosition.y, z);
            yield return null;
        }
    }

    // Function to grip or ungrip depending on the input parameter
    // @Param bool state: Either true to grip or false to ungrip
    IEnumerator grip(bool state)
    {
        float t = 0.0f;
        float d = 1;
        float z1 = finger1.transform.localPosition.z;
        float z2 = finger2.transform.localPosition.z;
        if (state)
        {
            
            float zf1 = -0.4515891f;
            float zf2 = -0.2588217f;
            while (t < d)
            {
                t += Time.deltaTime;
                float pos1 = Mathf.Lerp(z1, zf1, t / d);
                float pos2 = Mathf.Lerp(z2, zf2, t / d);
                finger1.transform.localPosition = new Vector3(finger1.transform.localPosition.x, finger1.transform.localPosition.y, pos1);
                finger2.transform.localPosition = new Vector3(finger2.transform.localPosition.x, finger2.transform.localPosition.y, pos2);
                yield return null;
            }
            
        }
        else
        {
            float zf1 = finger1.transform.localPosition.z - 0.5f;
            float zf2 = finger2.transform.localPosition.z + 0.5f;
            while (t < 1)
            {
                t += Time.deltaTime;
                float pos1 = Mathf.Lerp(z1, zf1, t / d);
                float pos2 = Mathf.Lerp(z2, zf2, t / d);
                finger1.transform.localPosition = new Vector3(finger1.transform.localPosition.x, finger1.transform.localPosition.y, pos1);
                finger2.transform.localPosition = new Vector3(finger2.transform.localPosition.x, finger2.transform.localPosition.y, pos2);
                yield return null;
            }
        }
    }

    // Function that rotates a given given arm component over a specified period of time
    // @Param GameObject obj: GameObject to be rotated
    // @Param float startRot: Starting rotation angle
    // @Param float endRot: Target rotation angle
    // @Param float duration: Length of time for the rotation (in seconds)
    // @Param char axis: axis to rotate around (x, y, or z)
    // @Param bool rotating: an unused bool variable to track when rotation is completed
    IEnumerator rotateArm(GameObject obj , float startRot, float endRot, float duration, char axis, bool rotating)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float axisRotation = Mathf.Lerp(startRot, endRot, t / duration);
            if (axis.Equals('x')) {
                obj.transform.localEulerAngles= new Vector3(axisRotation, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
            }
            else if (axis.Equals('y'))
            {
                obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, axisRotation, obj.transform.eulerAngles.z);
            }
            else
            {
                obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, obj.transform.eulerAngles.y, axisRotation);
            }

            yield return null;
        }
        rotating = false;
    }

    // Function that moves the arm over a given duration
    // @Param Vector3 startPos: beginning position represented by 3D vector
    // @Param Vector3 endPos: target position represented by 3D vector
    // @Param float duration: length of time for the movement (in seconds)
    IEnumerator moveArm(Vector3 startPos, Vector3 endPos, float duration)
    {
        if (moving)
        {
            yield break;
        }
        moving = true;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 movement = Vector3.Lerp(startPos, endPos, t / duration);
            arm.transform.position = movement;
            yield return null;
        }
        moving = false;
    }

    // Begins the Scenario
    void simp1()
    {
        hand.transform.localEulerAngles = new Vector3(0, 0, 0);
        StartCoroutine(state1());
    }
    // The beginning state
    IEnumerator state1()
    {
        // Set the starting position for the arm and gantry system
        arm.transform.position = new Vector3(-80.3f, arm.transform.position.y, 7.7f);
        h.transform.localPosition = new Vector3(h.transform.localPosition.x, h.transform.localPosition.y, h.transform.localPosition.z + 19.25f);
        h1.transform.localPosition = new Vector3(h1.transform.localPosition.x, h1.transform.localPosition.y, h1.transform.localPosition.z + 19.25f);
        h2.transform.localPosition = new Vector3(h2.transform.localPosition.x - 10, h2.transform.localPosition.y, h2.transform.localPosition.z + 19.25f);
        crossbeam.transform.localPosition = new Vector3(crossbeam.transform.localPosition.x, crossbeam.transform.localPosition.y, crossbeam.transform.localPosition.z + 19.25f);

        // Save the initial angles for the arm's sub-components
        bicep.transform.localEulerAngles = new Vector3(0, 90, 90);
        sb = bicep.transform.eulerAngles.z;
        wrist.transform.localEulerAngles = new Vector3(0, 0, -20);
        sw = wrist.transform.eulerAngles.z;
        forearm.transform.localEulerAngles = new Vector3(0, 0, 110);
        sf = forearm.transform.eulerAngles.z;

        // Wait for startOperation to become true, and then activate the next state
        yield return new WaitUntil(() => (startOperation == true));
        StartCoroutine(state2());
    }
    // The second state
    IEnumerator state2()
    {
        // Set duration for the next movement
        float time = 10.5f; //time = 2;

        // The first movement of the arm. Moves towards the raw material A feeder
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, 315, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, -5, time, 'z', rot2));
        StartCoroutine(rotateArm(hand, hand.transform.eulerAngles.x, hand.transform.eulerAngles.x - 90, time, 'x', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, hand.transform.eulerAngles.z + 1, time, 'z', rot4));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);

        // Set duration for the next movement
        time = 5; //time = 2;

        // The second movement of the arm. Lowers down to the correct level of the raw material A
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, 260, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 20, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));


        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);
        // Open the grippers
        StartCoroutine(grip(false));
        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(0.5f);


        //yield return new WaitUntil(() => (rawMaterialA== true));

        time = 4; //time = 2;
        
        // The third movement of the arm. Moves grippers forward to be in range of the raw material A
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, 275, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 1, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);

        // Activate the next state
        StartCoroutine(state3());
    }
    // The third state
    IEnumerator state3()
    {

        // Close the grippers to grab the plate
        StartCoroutine(grip(true));

        // Duplicate the plate object and hide the original.
        clone = Instantiate(plate);
        plate.SetActive(false);
        // Make the new plate a child of the wrist so that they move together.
        clone.transform.parent = wrist.transform;

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(2);

        
        float time = 4; //time = 2;

        // The fourth movement of the arm. The arm moves upward away from the feeder
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 25, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 15, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time);
        // Activate the push function for the feeder's pusher
        StartCoroutine(push());

        // The fifth movement of the arm. The arm swings back to where it will drop the plate.
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y - 45, 1, 'y', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 12, 1, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, 1, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(1);

        // Retract the grippers to drop the plate.
        StartCoroutine(grip(false));
        clone.GetComponent<Rigidbody>().useGravity = true;
        finger1.GetComponent<BoxCollider>().enabled = true;
        clone.GetComponent <Rigidbody>().constraints = RigidbodyConstraints.None;
        // Sets the plate's parent to the floor so that it no longer moves with the wrist.
        clone.transform.parent = floor.transform;

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(3f);

        // Activate the next state
        StartCoroutine(returnState());

    }
    // The final state
    IEnumerator returnState()
    {
        float time = 2;

        // The sixth and final movement of the arm. Moves every sub-component back to its original ready position.
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, sb, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, sf, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, 0, time, 'z', rot3));
        StartCoroutine(rotateArm(hand, hand.transform.eulerAngles.x, hand.transform.eulerAngles.x + 90, time, 'x', rot4));
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, 180, time, 'y', rot5));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);

        // Reset the hand's angle to prevent misalignment
        hand.transform.localEulerAngles = new Vector3(0, 0, 0);

        // Wait for rawMaterialA to be true and then move back to the second state.
        yield return new WaitUntil(() => (rawMaterialA == true));
        yield return new WaitUntil(() => (startOperation == true));
        //StartCoroutine(push());
        finger1.GetComponent<BoxCollider>().enabled = false;
        // Activate the second state
        StartCoroutine(state2());
    }
}
