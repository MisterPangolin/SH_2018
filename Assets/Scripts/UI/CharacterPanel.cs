using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Classe qui permet d'afficher la description des profils d'une maison et l'évolution des paramètres de santé au cours
/// du temps sur un graphique.
/// </summary>
public class CharacterPanel : MonoBehaviour
{
	//champs de texte
	public Text health, birthday, gender;

	//profil en cours
	Character currentCharacter;

	//édition
	public CharacterEdition edition;

	//panneau de description des profils
	public GameObject characterDescription;

	//noms des profils
	public Transform characterNamePanel;
	public Button characterName;
	public Sprite blueCharacterSprite, whiteCharacterSprite;
	List<Button> buttons;

	//informations de santé
	public GameObject healthButton, redHealthButton;
	public Transform informationsLayout;
	public ChronicsPanel informationsPanel;

	//demande de confirmation de suppression de profil
	public GameObject confirmePanel;

	//graphique
	public LineGraphManager graph;
	public Dropdown prmDropdown;
	public GameObject buttonsLayout, prmInformationsButton;
	public HealthParameterPanel prmPanel;
	public ChooseGraphTimeSet graphTimeSet;
	public GameObject UICamera;

	/// <summary>
	/// Ouvre le panneau de description pour afficher les informations du profil donné.
	/// Bloque les caméras et le déroulement du temps.
	/// </summary>
	public void Open(Character character = null)
	{
		if (character == null)
		{
			if (CharacterStorage.Get(0))
			{
				Open(CharacterStorage.Get(0));
			}
			else
			{
				characterDescription.SetActive(false);
				UICamera.SetActive(false);
				edition.Open();
			}
		}
		else
		{
			graphTimeSet.SetTime();
			gameObject.SetActive(true);
			InstantiateNameButtons();
			ChangeButtonsColor(character.index);
			currentCharacter = character;
			gameObject.SetActive(true);
			characterDescription.SetActive(true);
			UICamera.SetActive(true);
			health.text = "";
			birthday.text = character.birthday;
			if (character.gender == 0)
			{
				gender.text = "Homme";
			}
			else
			{
				gender.text = "Femme";
			}
			prmDropdown.ClearOptions();
			var options = new List<string>();
			List<HealthParameter> measuredParameters = new List<HealthParameter>();

			for (int i = 0; i < character.healthPrms.Count; i++)
			{
				HealthParameter prm = character.healthPrms[i];
				if (prm.IsMeasured())
				{
					string measure = prm.measure;
					if (measure != "")
					{
						options.Add(prm.name + " (" + prm.measure + ")");
					}
					else
					{
						options.Add(prm.name);
					}
					measuredParameters.Add(prm);
				}
				else
				{
					health.text += "<color=#F59E92>" + prm.name + " - suivi non actif" + "</color>" + "\n";
				}
			}
			for (int i = 0; i < character.homePrms.Count; i++)
			{
				HomeParameter prm = character.homePrms[i];
				if (!prm.IsFollowed())
				{
					health.text += "<color=#F59E92>" + prm.name + " - suivi non actif" + "</color>" + "\n";
				}
			}

			if (options.Count == 0)
			{
				options.Add("Pas de paramètre à afficher");
				prmDropdown.AddOptions(options);
				graph.ClearGraph();
				buttonsLayout.SetActive(false);
				prmInformationsButton.SetActive(false);
			}
			else
			{
				prmDropdown.AddOptions(options);
				prmDropdown.onValueChanged.RemoveAllListeners();
				prmDropdown.onValueChanged.AddListener(delegate
				{
					graph.Open(24, character.GetData(prmDropdown.value));
				});
				graph.Open(24, character.GetData());
				prmDropdown.value = 0;
				buttonsLayout.SetActive(true);
				prmInformationsButton.SetActive(true);
			}
			health.text += character.health;

			ClearChronicsField();
			SetInformationsField(character);
			MapCamera.Locked = true;
			OverviewCamera.Locked = true;
			NavigationBehavior.Locked = true;
			TimeClock.Locked = true;
		}
	}

	/// <summary>
	/// Attribuée au bouton se situant à côté des noms des paramètres dans le menu déroulant.
	/// Permet d'afficher les informations de ce paramètre dans un panneau, et d'enregistrer cette transition dans
	/// "EventsStorage".
	/// Ferme le panneau des profils.
	/// </summary>
	public void OpenDescriptionOnClick()
	{
		EventsStorage.Push(new PanelEvent(currentCharacter));
		Close();
		prmPanel.Open(currentCharacter.healthPrms[prmDropdown.value]);
	}

	/// <summary>
	/// Instancie les boutons correspondant aux problèmes médicaux du profil et les place en enfant de l'objet
	/// "diseaseLayout".
	/// Si le problème est suivi dans la maison, le bouton sera vert.
	/// Sinon, il sera rouge.
	/// </summary>
	public void SetInformationsField(Character character)
	{
		
		GameObject b;
		foreach (HealthInformation info in character.chronics)
		{
			if (info.isFollowed())
			{
				b = info.InstantiateInformationButton(healthButton);
			}
			else
			{
				b = info.InstantiateInformationButton(redHealthButton);
			}
			b.transform.SetParent(informationsLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenChronicDescriptionOnClick(info); });
		}
	}

	/// <summary>
	/// Détruit tous les boutons de l'objet "diseaseLayout" avant d'afficher les nouveaux.
	/// </summary>
	public void ClearChronicsField()
	{
		int count = informationsLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(informationsLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Change la couleaur des boutons des noms des profils.
	/// Le profil en cours de lecture a un bouton bleu. Les autres sont blancs, comme le bouton permettant d'ajouter des
	/// nouveaux profils.
	/// </summary>
	public void ChangeButtonsColor(int i)
	{
		foreach (Button b in buttons)
		{
			b.image.sprite = whiteCharacterSprite;
		}
		buttons[i].image.sprite = blueCharacterSprite;
	}

	/// <summary>
	/// Ouvre le panneau d'édition pour modifier le profil en cours de lecture.
	/// </summary>
	public void Modify()
	{
		characterDescription.SetActive(false);
		UICamera.SetActive(false);
		edition.Open(currentCharacter);
	}

	/// <summary>
	/// Détruit les boutons donnant les noms des profils, puis les instancie pour prendre en compte les modifications
	/// apportées à la liste des profils.
	/// </summary>
	void InstantiateNameButtons()
	{
		foreach (Transform child in characterNamePanel)
		{
			Destroy(child.gameObject);
		}
		buttons = new List<Button>();
		foreach (Character character in CharacterStorage.GetAll())
		{
			Button b = Instantiate(characterName);
			b.GetComponentInChildren<Text>().text = character.name + " " + character.surname;
			b.transform.SetParent(characterNamePanel, false);
			b.onClick.AddListener(delegate { OpenCharacterDescriptionOnClick(character);});
			buttons.Add(b);
		}
	}

	/// <summary>
	/// Fonction donnée aux boutons des noms de profils lors de leur création.
	/// L'appui sur ce bouton permet d'ouvrir le panneau de description pour lire le profil indiqué.
	/// </summary>
	void OpenCharacterDescriptionOnClick(Character character)
	{
		EventsStorage.Push(new PanelEvent(currentCharacter));
		Open(character);
	}

	/// <summary>
	/// Fonction donnée aux boutons des problèmes médicaux lors de leur création.
	/// L'appui sur ce bouton permet d'ouvrir un panneau décrivant le problème.
	/// </summary>
	void OpenChronicDescriptionOnClick(HealthInformation condition)
	{
		Close();
		informationsPanel.Open(condition);
		EventsStorage.Push(new PanelEvent(currentCharacter));
	}

	/// <summary>
	/// Ferme le panneau de description et active les caméras.
	/// </summary>
	public void Close()
	{
		characterDescription.SetActive(false);
		UICamera.SetActive(false);
		gameObject.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Affiche un message demandant à l'utilisateur de confirmer la suppression du profil en cours de lecture.
	/// </summary>
	public void ConfirmeErase()
	{
		confirmePanel.SetActive(true);
	}

	/// <summary>
	/// Appelée après confirmation de la suppression du profil pour le retirer de la liste.
	/// </summary>
	public void EraseCharacter()
	{
		int index = currentCharacter.index;
		CharacterStorage.Remove(currentCharacter);
		if (index > 0)
		{
			Open(CharacterStorage.Get(index - 1));
		}
		else
		{
			Open();
		}
	}

	/// <summary>
	/// Ouvre le panneau d'édition pour la création de nouveaux profils.
	/// </summary>
	public void AddCharacter()
	{
		characterDescription.SetActive(false);
		UICamera.SetActive(false);
		edition.Open();
	}
}
