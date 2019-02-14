using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Valve.VR.InteractionSystem;

public class DisplayInfo : MonoBehaviour {

    public Hand _hand; // The hand object
    //public Hand _rightHand; // The hand object

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);
    private bool timerRetirer = false;
    private float timer;
    protected GameObject hitObject;
    protected RaycastHit rayHit;
    protected Device device;
    protected GameObject infos;
    private GameObject ObjetTampon;
    public GameObject Player;
    private bool gripPressed = false;

    // Use this for initialization
    void Start () {
        infos = GameObject.Find("Infos");
        Player = GameObject.Find("Player");
    }

    protected string formatString(string str)
    {
        StringBuilder sb = new StringBuilder(str);
        int count = 0;
        int maxCharInLine = 25;
        int lastSpaceIndex = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if(str[i]==' ')
            {
                lastSpaceIndex = i;
            }
            if (count > maxCharInLine)
            {
                sb[lastSpaceIndex] = '\n';
                count = 0;
            }
            count++;
        }
        //string result = Regex.Replace(str, ".{25}", "$0\n-");
        return sb.ToString();
    }

    protected void RayEnter(RaycastHit hit)
    {
        timer = 0;
        ObjetTampon = hitObject;
        //Asign to class variable so it can be used in RayStay
        device = hitObject.GetComponent<Device>();
        //Object has a device script (informations to be displayed)
        if (device != null)
        {
            Debug.Log("Object hitted has a Device Script!");
            string objectName = device.objectName;
            string objectDescription = device.objectDescription;
            //string objectIndications = device.objectIndications;
            infos.transform.GetChild(1).GetComponent<Text>().text = formatString(objectName) + "\n" + formatString(objectDescription);// + "\n" + formatString(objectIndications);

            // show infos
            infos.transform.SetParent(device.transform);
            infos.transform.localPosition = new Vector3 (0f, device.Y, 0f);
            Vector3 VectAngle = Player.transform.position - infos.transform.position;
            float angle;
            angle = VectAngle.x * VectAngle.x + VectAngle.z * VectAngle.z;
            angle = Mathf.Sqrt(angle);
            angle = 1 / angle;
            angle = VectAngle.x * angle;
            angle = Mathf.Acos(angle);
            angle = angle * 2 * Mathf.PI / 360;
            angle = angle % 360;
            infos.transform.eulerAngles = new Vector3(infos.transform.rotation.eulerAngles.x, angle, infos.transform.rotation.eulerAngles.z);
            infos.GetComponent<Animation>().Play("Afficher Infos");
        } else
        {
            print("Object hitted doesn't have a Device Script!");

        }
    }

    protected void RayStay(RaycastHit hit)
    {
        if (device != null)
        {
            print("I am in RayStay!");
        }
    }

    protected void RayExit()
    {
        if (device != null)
        {
            timerRetirer = true;
            timer = 0;
            //Clear class variables
            device = null;
            hitObject = null;
        }
    }

// Update is called once per frame
void Update ()
    {
        if ((_hand.grabGripAction.GetStateDown(_hand.handType) || _hand.otherHand.grabGripAction.GetStateDown(_hand.otherHand.handType)))
        {
            gripPressed = true;
        }
        if ((_hand.grabGripAction.GetStateUp(_hand.handType) || _hand.otherHand.grabGripAction.GetStateUp(_hand.otherHand.handType)))
        {
            gripPressed = false;
        }
        //Check if raycast hits anything
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit))
        {
            //If the object is the same as the one we hit last frame
            if (hitObject != null && hitObject == rayHit.transform.gameObject && (timerRetirer == false || hitObject == ObjetTampon))
            {
                //Trigger "Stay" method on Interactable every frame we hit
                RayStay(rayHit);
            }
            //We're still hitting something, but it's a new object
            else
            {
                print("We found this object for the first time!");

                //Trigger the ray "Exit" method on Interactable/ the previous object that was hit
                RayExit();

                //Keep track of new object that we're hitting, and trigger the ray "Enter" method on Interactable
                hitObject = rayHit.transform.gameObject;
                RayEnter(rayHit);
            }
        }
        //If we’re not hitting anything, we can call RayExit on the hitObject
        else
        {
            //We aren't hitting anything. Trigger ray "Exit" on Interactable
            RayExit();
        }
        if(timerRetirer)
        {
            timer += Time.deltaTime;
            if(timer> 10)
            {
                // hide infos
                infos.GetComponent<Animation>().Play("Retirer Infos");
                timerRetirer = false;
            }
        }
    }
}
