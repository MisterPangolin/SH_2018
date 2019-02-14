using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ControllerInput : MonoBehaviour {
/*
    public Hand _hand; // The hand object

    private GrabObject interactable;
    private bool pickedUp;



    // Use this for initialization
    void Start () {
        pickedUp = false;
	
	}

	// Update is called once per frame
	void Update () {
        if (_hand.grabGripAction.GetStateUP(_hand.handType) && pickedUp)
        {   
            Debug.Log("I'll call release!! ");
            //Release object
            interactable.Release(_hand);
            pickedUp = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        Debug.Log("OnTriggerStay");

        //If object is an interactable item, i.e. it has a GrabObject script attached
        interactable = collider.GetComponent<GrabObject>();
        if (interactable != null)
        {
            Debug.Log("not null");

            //If trigger button is down
            if (_hand.grabGripAction.GetStateDown(_hand.handType)) //|| _hand.otherHand.grabGripAction.GetStateDown(_hand.otherHand.handType))
            {
                Debug.Log("Trigger to grab");

                //Pick up object
                interactable.Pickup(_hand);
                pickedUp = true;
            }
        }
     
    }
    */
}
