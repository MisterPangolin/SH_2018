using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utilisée dans le menu d'aide pour créer des catégories.
/// Si une catégorie a des sous-catégories, un triangle doit être ajouté à côté du texte du bouton.
/// Ce script s'ajoute à ce triangle comme composant.
/// Les boutons des sous-catégories doivent être placés dans la hiérarchie juste en dessous de celui de la catégorie.
/// </summary>
/// 
public class ExpandOnClick : MonoBehaviour {

	//transform du bouton
	RectTransform rT;

	//tableaux des sous-catégories
	public GameObject[] listItems;

	//affichage des sous-catégories
	bool listEnabled;

	/// <summary>
	/// Etablit les références, et ajoute la fonction d'expansion au bouton.
	/// </summary>
	void Awake()
	{
		rT = GetComponent<RectTransform>();
		GetComponent<Button>().onClick.AddListener(delegate { SetListActive(); });
	}

	/// <summary>
	/// Appelée lorsque l'utilisateur clique sur le bouton.
	/// Affiche les boutons des sous-catégories ou les cache.
	/// </summary>
	void SetListActive()
	{
		listEnabled = !listEnabled;
		foreach (GameObject item in listItems)
		{
			item.SetActive(listEnabled);
		}
		if (listEnabled)
		{
			rT.eulerAngles = new Vector3(0f, 0f, -90f);
		}
		else
		{
			rT.eulerAngles = new Vector3(0f, 0f, 0f);
		}
	}


}
