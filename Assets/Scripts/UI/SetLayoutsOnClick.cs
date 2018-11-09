using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Change la catégorie affichée dans le menu d'aide.
/// </summary>

public class SetLayoutsOnClick : MonoBehaviour {

	//tableau des pages correspondant à la catégorie
	public GameObject[] layouts;

	/// <summary>
	/// Ajoute la fonction d'ouverture aux boutons
	/// </summary>
	void Awake()
	{
		GetComponent<Button>().onClick.AddListener(delegate { Open(); });
	}

	/// <summary>
	/// Met à jour le menu d'aide pour afficher la catégorie.
	/// </summary>
	public void Open()
	{
		HelpCanvas.Open(layouts);
	}
}
