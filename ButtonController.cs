using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject arm;
    // Start is called before the first frame update
    void Start()
    {
        arm = GameObject.Find("Arm");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            arm.GetComponent<Arm>().buttonPressed = true;
            //Debug.Log("pressed");
            yield return new WaitForSecondsRealtime(2f);
            arm.GetComponent<Arm>().buttonPressed = false;

        }
    }

}
