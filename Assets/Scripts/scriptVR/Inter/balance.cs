using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class balance : MonoBehaviour {

    public Text Affichage;
    public float poids;
    private float timer;
    private bool afficher = false;

    private void Start()
    {
        transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<BoxCollider>().isTrigger = true;
        

    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("detection player");
        if(other.tag == "Player")
        {
            Affichage.text = "" + poids;
            afficher = false;
            timer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Affichage.text = "00.00";
        }
        afficher = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(GetComponent<MeshRenderer>().enabled == true)
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        if (transform.GetChild(1).GetComponent<MeshRenderer>().enabled)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
        }
		if (afficher)
        {
            timer += Time.deltaTime;
            if(timer > 10)
            {
                afficher = false;
                timer = 0;
                Affichage.text = "";
            }
        }
	}
}
