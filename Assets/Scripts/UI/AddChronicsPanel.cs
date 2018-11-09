using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Permet de définir les informations de santé d'un profil ou de les modifier.
/// Il existe une base de problèmes de santé que l'utilisateur peut choisir.
/// L'utilisateur peut également ajouter des informations manuellement dans un champ d'écriture.
/// </summary>
public class AddChronicsPanel : MonoBehaviour {

	//profil ouvert et panneau d'édition
	public CharacterEdition characterEdition;
	Character currentCharacter;

	//informations de santé
	public HealthInformationsList informationsList;
	public GameObject healthButton;
	public Sprite greenSprite, blueSprite;
	public Transform informationLayout;
	List<GameObject> Buttons = new List<GameObject>();
	bool[] informationsActive;

	//informations complémentaires
	public InputField healthField;
	string newHealthField;
	public Text healthText;

	//champ de recherche
	public InputField searchField;

	/// <summary>
	/// Active le panneau pour afficher les informations de santé du profil en cours d'édition.
	/// Nettoie l'objet "diseaseLayout" puis instancie les boutons des troubles de santé, en les placant comme enfant de
	/// "diseaseLayout".
	/// Affiche dans le champ "healthField" les informations de santé déjà existantes du profil. Le champ sera vide si 
	/// de telles informations n'existent pas encore.
	/// </summary>
	public void Open()
	{
		characterEdition.gameObject.SetActive(false);
		gameObject.SetActive(true);
		currentCharacter = characterEdition.character;
		informationsActive = new bool[informationsList.GetSize()];
		foreach (HealthInformation information in currentCharacter.chronics)
		{
			int i = informationsList.GetRef(information);
			informationsActive[i] = true;
		}
		foreach (Transform child in informationLayout)
		{
			Destroy(child.gameObject);
		}
		for (int i = 0; i < informationsActive.Length; i++)
		{
			InstantiateButton(i,informationsActive[i]);
		}
		healthField.text = currentCharacter.health;
	}

	/// <summary>
	/// Instancie les boutons des troubles de santé et les place en enfant de "diseaseLayout".
	/// Si un bouton correspond à un trouble déjà actif, le bouton sera vert, sinon il sera bleu.
	/// </summary>
	void InstantiateButton(int i, bool active)
	{
		HealthInformation condition = informationsList.Get(i);
		GameObject button = condition.InstantiateInformationButton(healthButton);
		if (active)
		{
			button.GetComponent<Image>().sprite = greenSprite;
		}
		else
		{
			button.GetComponent<Image>().sprite = blueSprite;
		}
		button.GetComponent<Button>().onClick.
		      AddListener(delegate { SetConditionOnClick(i, button.GetComponent<Image>()); });
		button.transform.SetParent(informationLayout, false);
		Buttons.Add(button);
	}

	/// <summary>
	/// Enregistre dans "newHealthField" la valeur du champ des informations de santé.
	/// Appelée lors de la complétion du champ.
	/// </summary>
	public void SetHealthField()
	{
		newHealthField = healthField.text;
	}

	/// <summary>
	/// Fonction ajoutée à chaque bouton de santé lors de leur création pour afficher si le problème de santé associé
	/// au bouton est selectionné ou non.
	/// Le bouton est vert si oui, bleu si non.
	/// Le tableau de booleens "diseasesActive" garde ces informations et est modifié lors de l'appui sur un de ces 
	/// boutons.
	/// </summary>
	void SetConditionOnClick(int i, Image image)
	{
		informationsActive[i] = !informationsActive[i];
		if (informationsActive[i])
		{
			image.sprite = greenSprite;
		}
		else
		{
			image.sprite = blueSprite;
		}
	}

	/// <summary>
	/// Ferme le panneau d'informations de santé sans modifier le profil en cours de création.
	/// </summary>
	public void Close()
	{
		characterEdition.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Ferme le panneau d'informations de santé en modifiant le profil en cours de création avec les nouvelles
	/// informations du panneau.
	/// </summary>
	public void AddChronics()
	{
		var newConditions = new List<HealthInformation>();
		for (int i = 0; i < informationsActive.Length; i++)
		{
			if (informationsActive[i])
			{
				newConditions.Add(informationsList.Get(i));
			}
		}
		currentCharacter.SetChronics(newConditions.ToArray());
		currentCharacter.health = newHealthField;
		healthText.text = newHealthField;
		Close();
	}

	/// <summary>
	/// Appelée lorsque la valeur du champ de recherche change.
	/// Active ou désactive les boutons de santé selon la présence ou non dans le nom du problème qui leur est associé
	/// du texte de la barre de recherche.
	/// Un texte vide affichera tous les boutons.
	/// </summary>
	public void Search()
	{
		if (searchField.text == "")
		{
			foreach (GameObject b in Buttons)
			{
				b.SetActive(true);
			}
		}
		else
		{
			string input = searchField.text.ToLower();
			foreach (GameObject b in Buttons)
			{
				if (!b.GetComponentInChildren<Text>().text.ToLower().Contains(input))
				{
					b.SetActive(false);
				}
			}
		}
	}

	/// <summary>
	/// Réinitialise la barre de recherche.
	/// </summary>
	public void ClearSearch()
	{
		searchField.text = "";
	}
}
