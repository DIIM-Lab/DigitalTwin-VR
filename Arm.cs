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
    public GameObject cyl;
    public GameObject cylClone;

    public GameObject piston;
    public GameObject err;

    bool begin = true;
    int cap = 0;
    int parts = 0;
    int loops = 0;
    bool readCap = false;
    bool rot1 = false;
    bool rot2 = false;
    bool rot3 = false;
    bool rot4 = false;
    bool rot5 = false;
    bool moving = false;
    //bool cont = true;
    float sb, sw, sf;
    float fsb, fsf;
    public bool test = true;
    public bool startOperation = false;
    public bool rawMaterialA = false;
    public bool rawMaterialB = false;
    public bool pistonAssembly = false;
    public bool position1 = false;

    public bool platePlacing = true;
    public bool trainingScenario = false;
    public bool buttonPressed = false;
    public bool beginTrainingScenario = false;
    public bool liveSimulation = false;
    

    Vector3 rot;
    Vector3 prev_loc;

    //DocumentReference pistonDoc;

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
    [System.Serializable]
    public struct FeederFields
    {
        public int Capacity;
        public string time;
        public string location;
        public string name;
        public int partsRemaining;
        public bool startOperation;
    }
    private FeederFields feederFields;

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

    // Sets local simulation variables according to the values of the Firebase flags.
    void documentDataFeeder(string data)
    {
        //txt.text = data;
        Debug.Log(data);
        feederFields = JsonUtility.FromJson<FeederFields>(data);
        string state = feederFields.Capacity.ToString().Trim();
        cap = int.Parse(state);
        Debug.Log(string.Format("Capacity: {0}", cap));
        state = feederFields.partsRemaining.ToString().Trim();
        parts = int.Parse(state);
        Debug.Log(string.Format("Parts Remaining: {0}", parts));
    }

        // Tracks changes of a specified Firestore database Document
        void onColChange(string output)
    {
        FirebaseFirestore.GetDocument("Scenario-2", "9GOwnU3Heub3NZL1eTNd", gameObject.name, "documentData", "errorFunction");
    }
    void onColChangePiston(string output)
    {
        FirebaseFirestore.GetDocument("pistons", "IDBGYt7Ctv7qPG5YbBCm", gameObject.name, "documentDataPiston", "errorFunction");
    }
    void onColChangeFeeder(string output)
    {
        FirebaseFirestore.GetDocument("feeders", "e3rSFWNgwhJX4FRW3HiL", gameObject.name, "documentDataFeeder", "errorFunction");
    }

    // This function runs onces at the beginning of the simulation and is responsible for setting all the required GameObject varibles
    void Start()
    {
        FirebaseFirestore.ListenForCollectionChange("Scenario-2", false, gameObject.name, "onColChange", "errorFunction");
        FirebaseFirestore.ListenForCollectionChange("pistons", false, gameObject.name, "onColChangePiston", "errorFunction");
        FirebaseFirestore.ListenForCollectionChange("feeders", false, gameObject.name, "onColChangeFeeder", "errorFunction");

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
        crossbeam = GameObject.Find("Crossbeam Assembly");

        piston = GameObject.Find("Shaded_Piston");
        err = GameObject.Find("Error Text");

        plate = GameObject.Find("plate");
        cyl = GameObject.Find("Cylinder");

        rot = new Vector3(0, 0, -45);

    }
        
    // Update is called once per frame
    void Update()
    {
        if (!platePlacing || startOperation)
        {
            GameObject.Find("snap-pos").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("snap-pos-cyl").GetComponent<BoxCollider>().enabled = false;
        } else
        if (platePlacing)
        {
            GameObject.Find("snap-pos").GetComponent<BoxCollider>().enabled = true;
            GameObject.Find("snap-pos-cyl").GetComponent<BoxCollider>().enabled = true;

        }
        if (beginTrainingScenario)
        {
            if (!trainingScenario)
            {
                trainingScenario = true;
                hand.transform.localEulerAngles = new Vector3(0, 180, 0);
                beginTrainingScenario = false;
                StartCoroutine(full_state1());
            }
        }
        // Begin the scenario
        if (begin)
        {
            begin = false;
            full_scen2();
        }
        // Reads the capacity in the database and changes a local variable to match
        // Only reads when readCap is true
        if (readCap)
        {
            loops = cap;
            Debug.Log(string.Format("Capacity set to: {0}", loops));
            readCap = false;
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
            float zf1 = 0.4515891f;
            float zf2 = 0.2588217f;
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
            float zf1 = finger1.transform.localPosition.z + 0.5f;
            float zf2 = finger2.transform.localPosition.z - 0.5f;
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
    IEnumerator rotateArm(GameObject obj, float startRot, float endRot, float duration, char axis, bool rotating)
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
            if (axis.Equals('x'))
            {
                obj.transform.localEulerAngles = new Vector3(axisRotation, obj.transform.localEulerAngles.y, obj.transform.localEulerAngles.z);
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
        //crossbeam.transform.parent = null;
        moving = true;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 cborigin = crossbeam.transform.position;

            Vector3 movement = Vector3.Lerp(startPos, endPos, t / duration);
            arm.transform.position = movement;

            crossbeam.transform.position = new Vector3(crossbeam.transform.position.x, cborigin.y, cborigin.z);
            yield return null;
        }
        moving = false;
    }

    IEnumerator activatePiston()
    {
        //pistonDoc.UpdateAsync("state", "down");
        yield return new WaitForSecondsRealtime(1);
        piston.GetComponent<Piston>().pistonActivation = true;
        yield return new WaitForSecondsRealtime(11);
        piston.GetComponent<Piston>().pistonActivation = false;
        //pistonDoc.UpdateAsync("state", "up");
        yield return new WaitForSecondsRealtime(7);
    }

    // Begins the Scenario
    void full_scen2()
    {
        hand.transform.localEulerAngles = new Vector3(0, 180, 0);
        StartCoroutine(full_state1());
    }

    // The initial state
    IEnumerator full_state1()
    {
        // Set the starting position for the arm and gantry system
        arm.transform.position = new Vector3(-80.3f, arm.transform.position.y, 7.7f);
        
        if (!trainingScenario)
        {
            h.transform.localPosition = new Vector3(h.transform.localPosition.x, h.transform.localPosition.y, h.transform.localPosition.z + 19.25f);
            h1.transform.localPosition = new Vector3(h1.transform.localPosition.x, h1.transform.localPosition.y, h1.transform.localPosition.z + 19.25f);
            h2.transform.localPosition = new Vector3(h2.transform.localPosition.x - 10, h2.transform.localPosition.y, h2.transform.localPosition.z + 19.25f);
            crossbeam.transform.localPosition = new Vector3(crossbeam.transform.localPosition.x, crossbeam.transform.localPosition.y, crossbeam.transform.localPosition.z + 19.25f);
        }

        crossbeam.transform.parent = arm.transform;
        h.transform.parent = crossbeam.transform;
        h1.transform.parent = crossbeam.transform;
        h2.transform.parent = arm.transform;

        // Save the initial angles for the arm's sub-components
        bicep.transform.localEulerAngles = new Vector3(0, 90, 90);
        sb = bicep.transform.eulerAngles.z;
        //wrist.transform.localEulerAngles = new Vector3(0, 0, -20);
        wrist.transform.localEulerAngles = new Vector3(0, 0, 0);
        sw = wrist.transform.eulerAngles.z;
        //forearm.transform.localEulerAngles = new Vector3(0, 0, 110);
        forearm.transform.localEulerAngles = new Vector3(0, 0, 90);
        sf = forearm.transform.eulerAngles.z;

        StartCoroutine(grip(false));


        if (!trainingScenario)
        {
            // Wait for startOperation to become true, and then activate the next state
            yield return new WaitUntil(() => (liveSimulation == true));
            yield return new WaitUntil(() => (startOperation == true));
            yield return new WaitUntil(() => (rawMaterialA == true));
            yield return new WaitUntil(() => (parts > 0));


        }

        readCap = true;

        // Begin the next state
        StartCoroutine(full_state2());
    }

    // The second state
    IEnumerator full_state2()
    {
        // For syncing with the video
        //yield return new WaitForSecondsRealtime(3.8f);

        // Set duration for the next movement
        float time = 2;
        // The first movement of the arm. Moves towards the raw material A feeder
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, 315, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, -5, time, 'z', rot2));
        StartCoroutine(rotateArm(hand, hand.transform.localEulerAngles.x, hand.transform.localEulerAngles.x - 90, time, 'x', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z + 1, time, 'z', rot4));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);

        // Set duration for the next movement
        time = 4;
        // The second movement of the arm. Lowers down to the correct level of the raw material A
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, 260, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 20, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));


        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);

        time = 4;
        // The third movement of the arm. Moves grippers forward to be in range of the raw material A
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, 275, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 1, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 0.5f);

        // Activate the next state
        StartCoroutine(full_state3());
    }

    // The third state
    IEnumerator full_state3()
    {
        // Close the grippers to grab the plate
        StartCoroutine(grip(true));

        // Duplicate the plate object and hide the original.
        clone = Instantiate(plate);
        plate.SetActive(false);
        // Make the new plate a child of the wrist so that they move together.
        clone.transform.parent = wrist.transform;

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(1.5f);


        float time = 4;
        // The fourth movement of the arm. The arm moves upward away from the feeder
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 25, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 15, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(time + 1);
        // Activate the push function for the feeder's pusher
        StartCoroutine(push());

        // The fifth movement of the arm. The arm swings back to where it will drop the plate.
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y - 45, 1, 'y', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 12, 1, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, 1, 'z', rot3));

        // Wait for the operation to complete
        yield return new WaitForSecondsRealtime(3.5f);

        // Save the initial angles for the arm's sub-components
        fsb = bicep.transform.eulerAngles.z;
        fsf = forearm.transform.eulerAngles.z;

        // Activate the next state
        StartCoroutine(full_state4());
    }

    // The fourth state
    IEnumerator full_state4()
    {
        float time = 5.7f;
        StartCoroutine(moveArm(arm.transform.position, new Vector3(0.6f, arm.transform.position.y, 44.5f), time));
        yield return new WaitForSecondsRealtime(time + 3);

        // The sixth movement of the arm.
        time = 8.5f;
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y - 90, time, 'y', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 16, time, 'z', rot2));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 20, time, 'z', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot4));

        yield return new WaitForSecondsRealtime(time + 1);

        // The 7th movement of the arm.
        time = 9;
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 15, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 18, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 1);

        clone.transform.parent = floor.transform;
        clone.transform.parent = null;

        StartCoroutine(grip(false));
        yield return new WaitForSecondsRealtime(1);

        // The 8th movement of the arm.
        time = 9;
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 25, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 2);

        // Close grippers
        StartCoroutine(grip(true));
        yield return new WaitForSecondsRealtime(2.8f);

        // Activate the next state
        StartCoroutine(full_state5());
    }

    // The fifth state
    IEnumerator full_state5()
    {

        float time = 8;
        // The 9th movement of the arm.
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y - 90, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 10, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 3);

        time = 1;
        // The 10th movement of the arm.
        prev_loc = arm.transform.position;
        StartCoroutine(moveArm(arm.transform.position, new Vector3(11f, arm.transform.position.y, 33f), time));
        yield return new WaitForSecondsRealtime(time + 1);

        // The 11th movement of the arm.
        time = 6;
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y - 45, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 8, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 1);

        // Open grippers fully
        StartCoroutine(grip(false));
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(grip(false));
        yield return new WaitForSecondsRealtime(0.5f);

        // The 12th movement of the arm.
        time = 4;
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 13, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 1);

        // Close grippers fully and grab a cylinder
        StartCoroutine(grip(true));
        yield return new WaitForSecondsRealtime(1);

        // Duplicate the plate object and hide the original.
        cylClone = Instantiate(cyl);
        cyl.SetActive(false);
        // Make the new plate a child of the wrist so that they move together.
        cylClone.transform.parent = hand.transform;

        // Activate the next state
        StartCoroutine(full_state6());

    }

    // The sixth state
    IEnumerator full_state6()
    {

        float time = 4;
        // The 13th movement of the arm.
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 15, time, 'z', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 15, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 2);

        // The 14th movement of the arm.
        time = 5;
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y + 45, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 5, time, 'z', rot2));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 2, time, 'z', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot4));
        yield return new WaitForSecondsRealtime(time + 3.2f);

        // The 15th movement of the arm.
        time = 1;
        StartCoroutine(moveArm(arm.transform.position, new Vector3(prev_loc.x, prev_loc.y, prev_loc.z), time));
        yield return new WaitForSecondsRealtime(time + 2.5f);

        // The 16th movement of the arm.
        time = 8.5f;
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y + 90, time, 'y', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 11, time, 'z', rot2));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 5, time, 'z', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot4));
        yield return new WaitForSecondsRealtime(time + 1);

        // The 17th movement of the arm.
        time = 4;
        cylClone.GetComponent<Rigidbody>().freezeRotation = false;
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 8, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z + 2, time, 'z', rot3));
        StartCoroutine(rotateArm(hand, hand.transform.localEulerAngles.x, hand.transform.localEulerAngles.x + 90, time, 'x', rot4));
        yield return new WaitForSecondsRealtime(time + 1);

        cyl.SetActive(true);

        // Activate the next state
        StartCoroutine(full_state7());
    }

    // The seventh state
    IEnumerator full_state7()
    {
        // The 18th movement of the arm.
        float time = 5;
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 16, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 1);


        // Training Scenario code
        if (trainingScenario)
        {
            // Open the grippers and drop the cylinder into place. The cylinder is programmed to snap down into the correct position everytime.
            StartCoroutine(grip(false));
            cylClone.transform.parent = null;
            cylClone.transform.eulerAngles = new Vector3(70f, 90f, 0);
            cylClone.GetComponent<Rigidbody>().freezeRotation = true;
            cylClone.transform.parent = clone.transform;
            cylClone.tag = "Untagged";
            Destroy(cylClone.GetComponent<WebXRS.Interactions.MouseDragObject>());
            Destroy(cylClone.GetComponent<Rigidbody>());
            cylClone.transform.localPosition = new Vector3(cylClone.transform.localPosition.x, cylClone.transform.localPosition.y - 1.9f, 1f);
        }
        else
        {
            // Open the grippers and drop the cylinder into place. The cylinder is programmed to snap down into the correct position everytime.
            StartCoroutine(grip(false));
            cylClone.transform.parent = null;
            cylClone.transform.eulerAngles = new Vector3(90f, 0f, 0);
            cylClone.GetComponent<Rigidbody>().freezeRotation = true;
            cylClone.transform.parent = clone.transform;
            cylClone.transform.localPosition = new Vector3(cylClone.transform.localPosition.x - 0.1f, cylClone.transform.localPosition.y - 1.9f, 0);
        }
        yield return new WaitForSecondsRealtime(1);

        // The 19th movement of the arm.
        time = 5;
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 16, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 3, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 1);

        // The 20th movement of the arm.
        time = 5;
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 14, time, 'z', rot1));
        StartCoroutine(rotateArm(hand, hand.transform.localEulerAngles.x, hand.transform.localEulerAngles.x - 90, time, 'x', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        yield return new WaitForSecondsRealtime(time + 8);

        
        // Activate the piston routine to depress and raise the piston
        StartCoroutine(activatePiston());
        yield return new WaitForSecondsRealtime(18);

        // Training Scenario code
        if (trainingScenario)
        {
            err.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitUntil(() => (buttonPressed == true));
            err.GetComponent<MeshRenderer>().enabled = false;
            buttonPressed = false;
            StartCoroutine(returnToReady());

        }

        if (!trainingScenario)
        {
            // The 21st movement of the arm.
            time = 8;
            StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 27, time, 'z', rot1));
            StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 3, time, 'z', rot2));
            StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
            yield return new WaitForSecondsRealtime(time + 1);

            // Grip the plate assembly
            StartCoroutine(grip(true));
            clone.transform.parent = wrist.transform;
            yield return new WaitForSecondsRealtime(0.5f);

            // The 22nd movement of the arm.
            time = 11;
            StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z - 27, time, 'z', rot1));
            StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z - 3, time, 'z', rot2));
            StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
            yield return new WaitForSecondsRealtime(time + 1);

            // Activate the next state
            StartCoroutine(fullReturnState());
        }
    }

    // The final state
    IEnumerator fullReturnState()
    {
        // The 23rd movement of the arm.
        float time = 10;
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.y + 90, time, 'y', rot1));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 27, time, 'z', rot2));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 31, time, 'z', rot3));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot4));
        yield return new WaitForSecondsRealtime(time + 1);

        // The 24th movement of the arm.
        time = 6;
        StartCoroutine(moveArm(arm.transform.position, new Vector3(-80.3f, arm.transform.position.y, 7.7f), time));
        yield return new WaitForSecondsRealtime(time + 2);

        // Open the gripper to drop the assembly on the ground
        StartCoroutine(grip(false));
        clone.transform.parent = floor.transform;
        clone.GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSecondsRealtime(1);

        cyl.SetActive(true);

        // The 25th movement of the arm.
        time = 10.7f;
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, sb, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, sf, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        StartCoroutine(rotateArm(hand, hand.transform.eulerAngles.x, hand.transform.eulerAngles.x + 90, time, 'x', rot4));
        StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, 180, time, 'y', rot5));
        yield return new WaitForSecondsRealtime(time);
        bicep.transform.localEulerAngles = new Vector3(0, 90, 90);
        wrist.transform.localEulerAngles = new Vector3(0, 0, 0);
        forearm.transform.localEulerAngles = new Vector3(0, 0, 90);
        hand.transform.localEulerAngles = new Vector3(0, 180, 0);

        // This section of code tracks the amount of loops the simulation has made and compares it to the database's parts reminaing to make 
        // sure that both are still in sync with eachother
        loops--;
        if (loops == 0)
        {
            yield return new WaitUntil(() => (startOperation == true));
            readCap = true;
        }
        if (parts != 0)
        {
            // Return to the second state
            StartCoroutine(full_state2());
        }
        else
        {
            yield return new WaitUntil(() => (parts > 0));
            // Return to the second state
            StartCoroutine(full_state2());

        }
    }
    IEnumerator returnToReady()
    {
        //bicep.transform.localEulerAngles = new Vector3(0, 90, 90);
        //wrist.transform.localEulerAngles = new Vector3(0, 0, 0);
        //forearm.transform.localEulerAngles = new Vector3(0, 0, 90);

        float time = 2f;
        StartCoroutine(moveArm(arm.transform.position, new Vector3(-80.3f, arm.transform.position.y, 7.7f), time));
        StartCoroutine(rotateArm(bicep, bicep.transform.eulerAngles.z, bicep.transform.eulerAngles.z + 26, time, 'z', rot1));
        StartCoroutine(rotateArm(forearm, forearm.transform.eulerAngles.z, forearm.transform.eulerAngles.z + 22, time, 'z', rot2));
        StartCoroutine(rotateArm(wrist, wrist.transform.eulerAngles.z, wrist.transform.eulerAngles.z, time, 'z', rot3));
        StartCoroutine(rotateArm(hand, hand.transform.eulerAngles.x, hand.transform.eulerAngles.x + 90, time, 'x', rot4));
        //StartCoroutine(rotateArm(rotator, rotator.transform.eulerAngles.y, 180, time, 'y', rot5));
        yield return new WaitForSecondsRealtime(time);
        bicep.transform.localEulerAngles = new Vector3(0, 90, 90);
        wrist.transform.localEulerAngles = new Vector3(0, 0, 0);
        forearm.transform.localEulerAngles = new Vector3(0, 0, 90);
        hand.transform.localEulerAngles = new Vector3(0, 180, 0);
        Destroy(clone);
        trainingScenario = false;
        beginTrainingScenario = false;
        //StartCoroutine(full_state2());

    }
}
