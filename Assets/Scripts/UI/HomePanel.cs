using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Composant de "HomePanel", affichant l'adresse de l'habitation, le liste des profils, et les paramètres de maison à
/// suivre.
/// Profil et paramètre de maison sont accessibles depuis ce panneau, en cliquant sur les boutons à leurs noms.
/// </summary>
public class HomePanel : MonoBehaviour {

	//titre
	public GameObject homeDescription;

	//profils
	public Button characterItem;
	List<Button> buttons;
	public RectTransform charactersList;
	public CharacterPanel characterPanel;
	public CharacterEdition edition;

	//paramètres de maison
	public GameObject healthButton, blueHealthButton, redHealthButton;
	public Transform parametersLayout;
	public HealthParameterPanel parametersPanel;

	//adresse
	public Text Adress;

	/// <summary>
	/// Active le panneau et nettoie les champs.
	/// Change l'adresse pour celle de l'habitation, si un profil existe.
	/// Construit les listes de boutons des profils et des paramètres de maison.
	/// Désactive les caméras et le défilement du temps.
	/// </summary>
	public void Open()
	{
		gameObject.SetActive(true);
		homeDescription.SetActive(true);
		FillCharactersList();
		ClearParametersField();
		SetParametersField();
		if (CharacterStorage.Get(0))
		{
			Adress.text = CharacterStorage.Get(0).adress;
		}
		else
		{
			Adress.text = "Pas d'adresse enregistrée";
		}
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Détruit les boutons donnant les noms des profils, puis les instancie pour prendre en compte les modifications
	/// apportées à la liste des profils.
	/// </summary>
	void FillCharactersList()
	{
		for (int i = 0; i < charactersList.childCount; i++)
		{
			Destroy(charactersList.GetChild(i).gameObject);
		}
		buttons = new List<Button>();
		foreach (Character character in CharacterStorage.GetAll())
		{
			Button b = Instantiate(characterItem);
			b.GetComponentsInChildren<Text>()[0].text = character.name + " " + character.surname;
			b.GetComponentsInChildren<Text>()[1].text = character.birthday;
			b.transform.SetParent(charactersList, false);
			b.onClick.AddListener(delegate { OpenCharacterDescriptionOnClick(character); });
			buttons.Add(b);
		}
		Button bAdd = Instantiate(characterItem);
		bAdd.GetComponentsInChildren<Text>()[0].text = "Ajouter un nouveau profil";
		bAdd.GetComponentsInChildren<Text>()[0].fontStyle = FontStyle.Italic;
		bAdd.GetComponentsInChildren<Text>()[1].text = "";
		bAdd.transform.SetParent(charactersList, false);
		bAdd.onClick.AddListener(delegate { AddCharacterOnClick(); });
		buttons.Add(bAdd);
	}

	/// <summary>
	/// Fonction donnée aux boutons des noms de profils lors de leur création.
	/// L'appui sur ce bouton permet d'ouvrir le panneau de description pour lire le profil indiqué.
	/// </summary>
	void OpenCharacterDescriptionOnClick(Character character)
	{
		characterPanel.Open(character);
		homeDescription.SetActive(false);
		EventsStorage.Push(new PanelEvent());
	}

	/// <summary>
	/// Désactive ce panneau, et ouvre le panneau d'édition des profils pour créer un nouveau profil.
	/// </summary>
	void AddCharacterOnClick()
	{
		homeDescription.SetActive(false);
		edition.Open();
	}

	/// <summary>
	/// Ferme le panneau, réactive les caméras et le défilement du temps.
	/// </summary>
	public void Close()
	{
		gameObject.SetActive(false);
		homeDescription.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Détruit les boutons des paramètres de maison.
	/// </summary>
	void ClearParametersField()
	{
		int count = parametersLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(parametersLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Construit la liste déroulante des paramètres de maison à suivre.
	/// Instancie un bouton pour chaque paramètre. Si le paramètre doit être suivi et qu'il l'est, le bouton est vert,
	/// rouge sinon. Si le paramètre n'est pas indiqué comme devant être suivi mais qu'il l'est tout de même, le bouton
	/// est bleu.
	/// </summary>
	void SetParametersField()
	{
		GameObject b;
		HomeParameterFactory homeF = DeviceStorage.GetHomeFactory();
		for (int i = 0; i < homeF.GetSize(); i++)
		{
			HomeParameter homeP = homeF.Get(i);
			if (homeP.IsFollowed())
			{
				if (homeP.MustBeFollowed)
				{
					b = homeP.InstantiateHomeButton(healthButton);
				}
				else
				{
					b = homeP.InstantiateHomeButton(blueHealthButton);
				}
			}
			else if (homeP.MustBeFollowed)
			{
				b = homeP.InstantiateHomeButton(redHealthButton);
			}
			else
			{
				b = null;
			}
			if (b != null)
			{
				b.GetComponentInChildren<Text>().text = homeP.name;
				b.transform.SetParent(parametersLayout, true);
				b.GetComponent<Button>().onClick.AddListener(delegate { OpenParameterDescriptionOnClick(homeP); });
			}
		}
	}

	/// <summary>
	/// Ferme le panneau.
	/// Ouvre le panneau de description des paramètres pour afficher les informations de "homeP".
	/// </summary>
	void OpenParameterDescriptionOnClick(HomeParameter homeP)
	{
		Close();
		parametersPanel.Open(homeP);
		EventsStorage.Push(new PanelEvent());
	}
}
