using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Classe qui permet la modification ou la création de profils.
/// Elle est en composant des panneaux nommés "NewCharacter".
/// </summary>

public class CharacterEdition : MonoBehaviour
{
	//modification d'un profil existant
	public bool changing;
	public Character savedCharacter;

	//prefab de profil et profils en cours d'édition
	public GameObject characterObject;
	public Character character;
	[HideInInspector]
	public Character[] characters = new Character[0];

	//sexe
	public Toggle sir, madam;

	//informations
	public new InputField name;
	public InputField surname, birthday, adress;
	public Text health;
	string previousBirthday = "";
	string savedAdress = "";

	//panneau pour demander confirmation des actions de l'utilisateur
	public ContinueCharacterEdition continuePanel;

	//références à d'autres panneaux
	public GameObject homePanel;
	public CharacterPanel characterPanel;

	//nom de la scène
	public string sceneName;

	//liste des informations de santé du simulateur
	public HealthInformationsList chronics;

	/// <summary>
	/// Active l'objet ayant pour composant cette classe.
	/// Initialise les champs d'édition et crée un nouveau profil à éditer.
	/// Bloque les caméras et le défilement du temps.
	/// </summary>
	public void Open(bool fromMainMenu = false)
	{
		gameObject.SetActive(true);
		characters = new Character[0];
		previousBirthday = "XX/XX/XXXX";

		if (characterPanel)
		{
			Character firstCharacter = CharacterStorage.Get(0);
			if (firstCharacter)
			{
				savedAdress = firstCharacter.adress;
			}
		}
		CreateNewCharacter();
		if (!fromMainMenu)
		{
			MapCamera.Locked = true;
			OverviewCamera.Locked = true;
			NavigationBehavior.Locked = true;
			TimeClock.Locked = true;
		}
	}

	/// <summary>
	/// Active le panneau pour la modification d'un profil déjà existant.
	/// Crée un objet temporaire étant une copie du profil et change les champs d'édition pour correspondre au profil
	/// à modifier.
	/// </summary>
	public void Open(Character oldCharacter)
	{
		changing = true;
		gameObject.SetActive(true);
		savedCharacter = oldCharacter;
		character = Instantiate(characterObject).GetComponent<Character>();
		characters = new Character[0];
		oldCharacter.Copy(character);
		previousBirthday = character.birthday;
		birthday.text = previousBirthday;
		sir.isOn = (character.gender ==0);
		madam.isOn = (character.gender == 1);
		name.text = character.name;
		surname.text = character.surname;
		adress.text = character.adress;
		health.text = character.health;
	}

	/// <summary>
	/// Fonction de test permettant de créer rapidement un profil.
	/// </summary>
	public void Generate()
	{
		sir.isOn = true;
		madam.isOn = false;
		name.text = "A";
		surname.text = "B";
		birthday.text = "01/01/1900";
		previousBirthday = birthday.text;
		adress.text = savedAdress;
		health.text = "h";
		character.health = "h";
	}

	/// <summary>
	/// Ouvre le panneau "Continue" pour demander à l'utilisateur s'il veut vraiment arrêter l'édition de profils.
	/// </summary>
	public void Quit()
	{
		continuePanel.Open(continueMode.close);
	}

	/// <summary>
	/// Désactive le panneau en détruisant tous les profils venant d'être créés.
	/// Ouvre le panneau de création de maison si l'utilisateur est dans le menu d'accueil, ouvre le panneau de
	/// description des profils sinon.
	/// Si aucun profil n'existe dans ce second cas, le panneau d'édition va être appelé par le panneau de description
	/// pour être de nouveau ouvert, jusqu'à ce qu'un profil soit créé.
	/// </summary>
	public void Close()
	{
		Destroy(character.gameObject);
		foreach (Character newCharacter in characters)
		{
			Destroy(newCharacter.gameObject);
		}
		gameObject.SetActive(false);
		if (homePanel)
		{
			homePanel.SetActive(true);
		}
		else
		{
			characterPanel.Open();
		}
	}

	/// <summary>
	/// Ferme le panneau d'édition et ajoute les profils créés à la liste existante.
	/// Détruit le profil en cours s'il est incomplet.
	/// </summary>
	public void CloseAndAdd()
	{
		
		if (!IsValid())
		{
			Destroy(character.gameObject);
		}
		ValidateSingleCharacter();
		foreach (Character newCharacter in characters)
		{
			CharacterStorage.Add(newCharacter);
		}
		gameObject.SetActive(false);
		if (savedCharacter)
		{
			characterPanel.Open(savedCharacter);
		}
		else if (character)
		{
			characterPanel.Open(character);
		}
		else
		{
			characterPanel.Open();
		}
	}

	/// <summary>
	/// Appelée lors de la création d'une nouvelle maison.
	/// Détruit les profils de la maison pour en charger une nouvelle avec les profils venant d'être créés.
	/// </summary>
	public void CloseAndChange()
	{
		CharacterStorage.Clear();
		if (!IsValid())
		{
			Destroy(character.gameObject);
		}
		ValidateSingleCharacter();
		foreach (Character newCharacter in characters)
		{
			CharacterStorage.Add(newCharacter);
		}

		gameObject.SetActive(false);
		LoadScene();
	}

	/// <summary>
	/// Instancie un nouveau profil et réinitialise les champs d'édition.
	/// </summary>
	public void CreateNewCharacter()
	{
		changing = false;
		savedCharacter = null;
		sir.isOn = true;
		madam.isOn = false;
		name.text = "";
		surname.text = "";
		birthday.text = "XX/XX/XXXX";
		previousBirthday = birthday.text;
		adress.text = savedAdress;
		health.text = "";
		character = Instantiate(characterObject).GetComponent<Character>();
	}

	/// <summary>
	/// Fonction permettant de choisir le genre du profil. Appelée par les boutons "Sir" et "Madam".
	/// </summary>
	public void SetGender(int g)
	{
		character.gender = g;
	}

	/// <summary>
	/// Appelée lors de la complétion du champ "birthday" afin de compléter automatiquement le champ, pour toujours
	/// avoir une date donnée selon le format souhaitée, c'est-à-dire "jour/mois/année" ("XX/XX/XXXX" où X un chiffre).
	/// Corrige la date pour qu'elle soit possible.
	/// </summary>
	public void AutoCompleteBirthday()
	{
		string birthdayText = birthday.text;
		if (previousBirthday != birthdayText)
		{
			if (previousBirthday.Length == birthdayText.Length)
			{
				int index = 0;
				for (int i = 0; i < previousBirthday.Length; i++)
				{
					if (previousBirthday[i] != birthdayText[i])
					{
						index = i;
					}
				}
				if (ValidCharacter(birthdayText[index]) == -1 && birthdayText[index].ToString() != "X")
				{
					birthdayText = birthdayText.Remove(index, 1);
					birthdayText = birthdayText.Insert(index, previousBirthday[index].ToString());
				}
			}
			else if (previousBirthday.Length > birthdayText.Length)
			{
				int startIndex = 0;
				int index = 0;
				bool equal = true;
				while (index < previousBirthday.Length && index < birthdayText.Length && equal)
				{
					if (previousBirthday[index] == birthdayText[index])
					{
						index++;
					}
					else
					{
						equal = false;
					}
				}
				startIndex = index;
				int endIndex = startIndex + (previousBirthday.Length - birthdayText.Length);
				for (int i = startIndex; i < endIndex; i++)
				{
					if (i == 2 || i == 5)
					{
						birthdayText = birthdayText.Insert(i, "/");
					}
					else
					{
						birthdayText = birthdayText.Insert(i, "X");
					}
				}

			}
			else
			{
				int index = -1;
				int i = 0;
				while (index < 0 && i < previousBirthday.Length)
				{
					if (previousBirthday[i] != birthdayText[i])
					{
						index = i;
					}
					i++;
				}
				if (index == -1)
				{
					index = i;
				}
				if (ValidCharacter(birthdayText[index]) == -1)
				{
					birthdayText = birthdayText.Remove(index, 1);
				}
				else
				{
					if (index == (birthdayText.Length - 1) || birthdayText[index + 1].ToString() == "/")
					{
						birthdayText = birthdayText.Remove(index, 1);
					}
					else
					{
						birthdayText = birthdayText.Remove(index + 1, 1);
					}
				}
			}
		}
		int day = ValidString(birthdayText.Substring(0, 2));
		if (day != -1)
		{
			day = Mathf.Clamp(day, 01, 31);
			birthdayText = birthdayText.Remove(0, 2);
			if (day >= 10)
			{
				birthdayText = birthdayText.Insert(0, day.ToString());
			}
			else
			{
				birthdayText = birthdayText.Insert(0, "0" + day.ToString());
			}
		}
		int month = ValidString(birthdayText.Substring(3, 2));
		if (month != -1)
		{
			month = Mathf.Clamp(month, 01, 12);
			birthdayText = birthdayText.Remove(3, 2);
			if (month >= 10)
			{
				birthdayText = birthdayText.Insert(3, month.ToString());
			}
			else
			{
				birthdayText = birthdayText.Insert(3, "0" + month.ToString());
			}
		}
		int year = ValidString(birthdayText.Substring(6, 4));
		if (year != -1)
		{
			year = Mathf.Clamp(year, 1900, 2018);
			birthdayText = birthdayText.Remove(6, 4);
			birthdayText = birthdayText.Insert(6, year.ToString());
		}
		previousBirthday = birthdayText;
		birthday.text = birthdayText;
	}

	/// <summary>
	/// Appelée par la fonction "AutoCompleteBirthday" et "ValidString" pour vérifier qu'un caractère entré soit 
	/// acceptable.
	/// </summary>
	int ValidCharacter(char c)
	{
		int number;
		if (int.TryParse(c.ToString(), out number))
		{
			return number;
		}
		else
		{
			return -1;
		}
	}

	/// <summary>
	/// Appeléé par la fonction "AutoCompleteBirthday" pour vérifier qu'une chaîne de caractères entrée soit acceptable.
	/// </summary>
	int ValidString(string s)
	{
		string number = "";
		int i = 0;
		while (i < s.Length)
		{
			if (ValidCharacter(s[i]) != -1)
			{
				number += s[i].ToString();
				i++;
			}
			else
			{
				return -1;
			}
		}
		return int.Parse(number);
	}

	/// <summary>
	/// Si le profil est valide, il est ajouté à la liste des profils en création.
	/// Si un profil existant était modifié, effectue les changements et détruit l'objet temporaire utilisé pour cet
	/// effet.
	/// </summary>
	public void ValidateSingleCharacter()
	{
		if (character)
		{
			character.name = name.text;
			character.surname = surname.text;
			character.birthday = birthday.text;
			character.adress = adress.text;
			savedAdress = adress.text;
			character.health = health.text;
		}
		foreach (Character previousCharacter in characters)
		{
			previousCharacter.adress = savedAdress;
		}
		if (!changing)
		{
			var newCharacters = new Character[characters.Length + 1];
			for (int i = 0; i < characters.Length; i++)
			{
				newCharacters[i] = characters[i];
			}
			newCharacters[characters.Length] = character;
			characters = newCharacters;
		}
		else
		{
			CharacterStorage.RemoveHomeParametersToFollow(savedCharacter);
			CharacterStorage.SetHomeParametersToFollow(character);
			character.Copy(savedCharacter);
			Destroy(character.gameObject);
			changing = false;
		}
	}

	/// <summary>
	/// Renvoie true si le profil est complet, false sinon.
	/// </summary>
	public bool IsValid()
	{
		bool valid = true;
		if (name.text == "" || surname.text == "" || adress.text == "")
		{
			valid = false;
		}
		if (valid && (ValidString(birthday.text.Substring(0, 2)) == -1 ||
					 ValidString(birthday.text.Substring(3, 2)) == -1 ||
					 ValidString(birthday.text.Substring(6, 4)) == -1))
		{
			valid = false;
		}
		return valid;
	}

	/// <summary>
	/// Appelée lors de l'appui sur le bouton "Creer un autre profil".
	/// Réinitialise le panneau pour la création d'un autre profil ou ouvre le panneau "Continue" si le profil en cours
	/// n'est pas complet.
	/// </summary>
	public void AddCharacter()
	{
		if (IsValid())
		{
			ValidateSingleCharacter();
			CreateNewCharacter();
		}
		else
		{
			continuePanel.Open(continueMode.addNew, GenerateContinueMessage());
		}
	}

	/// <summary>
	/// Détruit le profil en cours d'édition pour en créer un nouveau.
	/// </summary>
	public void EraseCurrentCharacter()
	{
		Destroy(character.gameObject);
		CreateNewCharacter();
	}

	/// <summary>
	/// Appelée lorsque l'utilisateur souhaite créer une nouvelle maison à partir du menu d'accueil.
	/// Ouvre la scène de la maison si le profil en cours est complet, ouvre le panneau "Continue" sinon.
	/// </summary>
	public void ValidateCharacters()
	{
		if (IsValid())
		{
			ValidateSingleCharacter();
			LoadScene();
		}
		else
		{
			continuePanel.Open(continueMode.validate, GenerateContinueMessage());
		}
	}

	/// <summary>
	/// Charge la scène de la maison.
	/// </summary>
	public void LoadScene()
	{
		SceneManager.LoadScene(sceneName);
	}

	/// <summary>
	/// Appelée lorsque l'utilisateur souhaite valider les profils créés pour les ajouter à la liste existante.
	/// Appelle la fonction "CloseAndAdd" si le profil en cours est complet, ouvre le panneau "Continue" sinon.
	/// </summary>
	public void AddCharacters()
	{
		if (IsValid())
		{
			CloseAndAdd();
		}
		else
		{
			continuePanel.Open(continueMode.validateAndReturn, GenerateContinueMessage());
		}
	}

	/// <summary>
	/// Appelée lorsque l'utilisateur souhaite créer une nouvelle maison à partir d'une autre maison.
	/// Ouvre la scène de la maison si le profil en cours est complet, ouvre le panneau "Continue" sinon.
	/// </summary>
	public void NewHomeAndCharacters()
	{
		if (IsValid())
		{
			CloseAndChange();
		}
		else
		{
			continuePanel.Open(continueMode.validateAndNew, GenerateContinueMessage());
		}
	}

	/// <summary>
	/// Genère une partie du message à afficher sur le panneau "Continue", pour indiquer les champs du profil encore
	/// vides.
	/// </summary>
	string GenerateContinueMessage()
	{
		string message = "Champ(s) à compléter :" + "\n";
		bool previous = false;
		if (name.text == "")
		{
			message += "prénom ";
			previous = true;
		}
		if (surname.text == "")
		{
			if (previous)
			{
				message += ", ";
			}
			message += "nom ";
			previous = true;
		}
		if (adress.text == "")
		{
			if (previous)
			{
				message += ", ";
			}
			message += "adresse ";
			previous = true;
		}
		if ((ValidString(birthday.text.Substring(0, 2)) == -1 ||
					 ValidString(birthday.text.Substring(3, 2)) == -1 ||
					 ValidString(birthday.text.Substring(6, 4)) == -1))
		{
			if (previous)
			{
				message += ", ";
			}
			message += "anniversaire ";
		}
		return message;
	}
}