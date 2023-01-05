using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using Firebase.Firestore;
//using FirebaseWebGL.Examples.Utils;
//using FirebaseWebGL.Scripts.FirebaseBridge;
using TMPro;
//using FirebaseWebGL.Scripts.Objects;
using System.Collections.Generic;



public class Piston : MonoBehaviour
{

    [System.Serializable]
    public struct Fields
    {
        public string location;
        public string lastUpdate;
        public int seconds;
        public int nanoseconds;
        public string state;
        public string id;
    }
    private Fields fields;

    Animator animator;
    float delay = 5;
    InputField delField;
    Button pButton;
    public Text txt;
    public bool pistonActivation = false;

    IEnumerator waitDelay()
    {
        float time = gameObject.GetComponent<Piston>().delay - 1f;
        yield return new WaitForSecondsRealtime(time);
        animator.SetBool("Rev", true);
        animator.SetBool("Start", false);
        //animator.SetBool("Stop", true);
    }

    private void changeValue(string arg0)
    {
        float time = float.Parse(arg0);
        if (time < 1.5f)
        {
            time = 1.5f; // Temp fix to solve animation speedup
        } 
        gameObject.GetComponent<Piston>().delay = float.Parse(arg0);
        delField.SetTextWithoutNotify(arg0);
        delField.ForceLabelUpdate();
    }

    void onButtonClick()
    {
        if (animator.GetBool("Start") == false)
        {
            animator.SetBool("Rev", false);

            animator.SetBool("Start", true);


            StartCoroutine(gameObject.GetComponent<Piston>().waitDelay());
        }
    }

    void errorFunction(string error)
    {
        Debug.Log(error);
    }

    void documentData(string data)
    {
        //txt.text = data;
        Debug.Log(data);
        fields = JsonUtility.FromJson<Fields>(data);
        string state = fields.state.ToString().Trim();
        if (state == "down")
        {
            Debug.Log("Going down");

            animator.SetBool("Rev", false);

            animator.SetBool("Start", true);

        }
        else if (state.ToString() == "up")
        {
            Debug.Log("Going up");

            animator.SetBool("Rev", true);

            animator.SetBool("Start", false);
        }
    }

    void onColChange(string output)
    {
        //FirebaseFirestore.GetDocument("pistons", "IDBGYt7Ctv7qPG5YbBCm", gameObject.name, "documentData", "errorFunction");

    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //animator.SetBool("Start", false);
        //gameObject.GetComponent<Piston>().delay = 5.0f;
        //delField = GameObject.Find("delayInput").GetComponent<InputField>();
        //pButton = GameObject.Find("pistonButton").GetComponent<Button>();
        //pButton.onClick.AddListener(onButtonClick);
        var se = new InputField.SubmitEvent();
        //delField.onEndEdit.AddListener(changeValue);
        //delField.SetTextWithoutNotify(delay.ToString());

        //FirebaseFirestore.ListenForCollectionChange("pistons", false, gameObject.name, "onColChange", "errorFunction");




        /**FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference doc = db.Collection("pistons").Document("IDBGYt7Ctv7qPG5YbBCm");

        ListenerRegistration listener = doc.Listen(task =>
        {
            Dictionary<string, object> data = task.ToDictionary();
            string state = data["state"].ToString().Trim();
            Debug.Log(state);

            if (state == "down")
            {
                Debug.Log("Going down");

                animator.SetBool("Rev", false);

                animator.SetBool("Start", true);

            }
            else if (state.ToString() == "up")
            {
                Debug.Log("Going up");

                animator.SetBool("Rev", true);

                animator.SetBool("Start", false);
            }

        });**/

    }

    // Update is called once per frame
    void Update()
    {
        //delField.ForceLabelUpdate();

        if (pistonActivation)
        {
            //Debug.Log("Going down");

            animator.SetBool("Rev", false);

            animator.SetBool("Start", true);

        } else if (!pistonActivation)
        {
            //Debug.Log("Going up");

            animator.SetBool("Rev", true);

            animator.SetBool("Start", false);
        }

    }


}
