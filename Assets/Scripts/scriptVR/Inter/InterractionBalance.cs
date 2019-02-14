using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterractionBalance : MonoBehaviour {
    public Text poid;
    private float valeurPoid;
    private bool resetAffichage = false;
    private float timer = 0f;

    /// <summary>
    /// si le joueur monte sur la balance, affiche le poids
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            poid.text = valeurPoid.ToString();
            resetAffichage = false;
            timer = 0;
        }
    }

    /// <summary>
    /// si le joueur decend de la balance, affiche 00:00
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            poid.text = "00,00";
            resetAffichage = true;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (resetAffichage)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 20)
        {
            poid.text = "";
            resetAffichage = false;
            timer = 0;
        }
    }

    public void setValeurPoid(float _Poids)
    {
        valeurPoid = _Poids;
    }
}
