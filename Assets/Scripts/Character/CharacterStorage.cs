using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Classe qui permet de stocker les références à chaque composant "Character" dans la scène et de manipuler ces 
/// composants.
/// Elle permet qu'une seule instance du composant associé dans chaque scène.
/// </summary>
public class CharacterStorage : MonoBehaviour {

	//référence à lui-même
	static CharacterStorage instance;

	//Liste des profils et référence au prefab des profils
	[HideInInspector]
	public List<Character> characters;
	public GameObject characterObject;

	/// <summary>
	/// Crée grâce à "instance" une référence unique à la classe CharacterStorage dans la scène.
	/// Trouve tous les objets ayant Character comme composant pour les placer dans une liste.
	/// </summary>
	void Awake()
	{
		instance = this;
		Character[] newCharacters = FindObjectsOfType<Character>();
		int i = 0;
		foreach (Character character in newCharacters)
		{
			characters.Add(character);
			SetHomeParametersToFollow(character);
			character.index = i;
			i++;
		}
	}

	/// <summary>
	/// Enregistre les profils de la liste "characters".
	/// </summary>
	public static void Save(HomeDataWriter writer)
	{
		writer.Write(instance.characters.Count);
		foreach (Character character in instance.characters)
		{
			character.Save(writer);
		}
	}

	/// <summary>
	/// Charge les profils du fichier de sauvegarde et instancier de nouveaux objets pour leur associer ces profils.
	/// </summary>
	public static void Load(HomeDataReader reader)
	{
		for (int i = 0; i < instance.characters.Count; i++)
		{
			Destroy(instance.characters[i].gameObject);
		}
		instance.characters.Clear();
		int count = reader.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Character character = Instantiate(instance.characterObject).GetComponent<Character>();
			character.Load(reader);
			instance.characters.Add(character);
			SetHomeParametersToFollow(character);
			character.index = i;
		}
	}

	/// <summary>
	/// Renvoie le profil d'indice "index" dans la liste "characters".
	/// </summary>
	public static Character Get(int index)
	{
		if (instance.characters.Count > index && index >= 0)
		{
			return instance.characters[index];
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Renvoie sous forme de tablea la liste "characters".
	/// </summary>
	public static Character[] GetAll()
	{
		return instance.characters.ToArray();
	}

	/// <summary>
	/// Retire de la liste "characters" l'objet ayant pour composant "Character" puis détruit cet objet.
	///	L'indice du profil est un paramètre de la classe Character ce qui permet de retrouver rapidement l'objet dans
	/// la liste.
	/// </summary>
	public static void Remove(Character character)
	{
		int index = character.index;
		instance.characters.RemoveAt(index);
		RemoveHomeParametersToFollow(character);
		Destroy(character.gameObject);
		for (int i = index; i < instance.characters.Count; i++)
		{
			Get(i).index -= 1;
		}
	}

	/// <summary>
	/// Ajoute un profil dans la liste des profils.
	/// </summary>
	public static void Add(Character character)
	{
		instance.characters.Add(character);
		SetHomeParametersToFollow(character);
		character.index = instance.characters.Count - 1;
	}

	/// <summary>
	/// Réinitialise la liste characters en détruisant tous les objets qui la composent.
	/// </summary>
	public static void Clear()
	{
		foreach (Character character in instance.characters)
		{
			Destroy(character.gameObject);
		}
	}

	/// <summary>
	/// Sélectionne les paramètres de la maison à suivre, en vérifiant pour chaque profil si le paramètre est renseigné
	/// ou non.
	/// </summary>
	public static void SetHomeParametersToFollow(Character character)
	{
		foreach (HealthInformation healthI in character.chronics)
		{
			foreach (HomeParameter homeP in healthI.homeP)
			{
				homeP.MustBeFollowed = true;
			}
		}
	}

	/// <summary>
	/// Enlève des paramètres de la liste des paramètres de la maison à suivre.
	/// </summary>
	public static void RemoveHomeParametersToFollow(Character character)
	{
		foreach (HealthInformation healthI in character.chronics)
		{
			foreach (HomeParameter homeP in healthI.homeP)
			{
				homeP.MustBeFollowed = false;
			}
		}
	}

	/// <summary>
	/// Ajoute pour chaque paramètre de santé de chaque profil une nouvelle donnée correspondant à l'instant "t".
	/// </summary>
	public static void AddSubDataValue(Instant t)
	{
		foreach (Character character in instance.characters)
		{
			character.AddSubDataValue(t);
		}
	}

	/// <summary>
	/// Modifie la valeur de la donnée du paramètre de santé "prm" correspondant à l'instant "t" pour chaque profil, si
	/// le paramètre concerne le profil, par la nouvelle valeur "f".
	/// </summary>
	public static void RefreshCharactersData(HealthParameter prm, Instant t, float f)
	{
		foreach (Character c in instance.characters)
		{
			c.RefreshData(prm, t, f);
		}
	}
}
