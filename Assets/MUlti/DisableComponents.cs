using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableComponents : NetworkBehaviour {
    [SerializeField]
    Behaviour[] componentsToDisable;

	// Use this for initialization
	void Start ()
    {
		if(!isLocalPlayer)
        {
            //boucle de désactivation des scripts ne concernant pas notre joueur
            for(int i=0; i< componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
	}
}
