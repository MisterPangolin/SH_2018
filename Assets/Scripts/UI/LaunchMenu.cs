using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// Associée au panneau de chargementdu menu principal.
/// Permet de choisir la sauvegarde à lancer.
/// </summary>
public class LaunchMenu : MonoBehaviour {

	//listes des fichiers
	public InputField nameInput;
	public RectTransform listContent;
	public SaveLoadItem itemPrefab, protectedItemPrefab;
	List<GameObject> Buttons = new List<GameObject>();

	//zone de recherche
	public InputField searchField;

	/// <summary>
	/// Active le panneau et affiche la liste des fichiers de sauvegarde.
	/// </summary>
	public void Open()
	{
		gameObject.SetActive(true);
		FillList();
	}
	/// <summary>
	/// Renvoie la chaîne de caractères entrée par l'utilisateur dans le champ du menu de sauvegarde.
	/// </summary>
	string GetSelectedPath()
	{
		string homeName = nameInput.text;
		if (homeName.Length == 0)
		{
			Debug.LogError("pas de nom entré");
			return null;
		}
		else
		{
			PersistentStorage.ChangePathName(homeName + ".home");
			if (PersistentStorage.ContainsExemple(homeName + ".home"))
			{
				return Path.Combine(Application.streamingAssetsPath, homeName + ".home");
			}
			else
			{
				return Path.Combine(Application.persistentDataPath, homeName + ".home");
			}
		}
	}

	/// <summary>
	/// Définit le fichier à lancer puis charge la scène des maisons.
	/// </summary>
	public void Load()
	{
		GetSelectedPath();
		SceneManager.LoadScene("Creation_architecture");
	}

	/// <summary>
	/// Change la chaîne de caractères affichée dans le champ du menu de sauvegarde lorsque l'utilisateur clique sur un
	/// nom dans la liste.
	/// </summary>
	public void SelectItem(string name)
	{
		nameInput.text = name;
	}

	/// <summary>
	/// Remplit la liste des fichiers de sauvegarde à afficher dans le menu de sauvegarde.
	/// </summary>
	void FillList()
	{
		Buttons.Clear();
		for (int i = 0; i < listContent.childCount; i++)
		{
			Destroy(listContent.GetChild(i).gameObject);
		}

		string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.home");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++)
		{
			SaveLoadItem item = Instantiate(itemPrefab);
			item.mainMenu = this;
			item.HomeName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
			Buttons.Add(item.gameObject);
		}

		paths = Directory.GetFiles(Application.streamingAssetsPath, "*.home");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++)
		{
			SaveLoadItem item = Instantiate(protectedItemPrefab);
			item.mainMenu = this;
			PersistentStorage.AddExemple(Path.GetFileName(paths[i]));
			item.HomeName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
			Buttons.Add(item.gameObject);
		}
	}

	/// <summary>
	/// Supprime le fichier de sauvegarde choisi.
	/// </summary>
	public void Delete()
	{
		string path = GetSelectedPath();
		if (path == null || PersistentStorage.ContainsExemple(Path.GetFileName(path)))
		{
			return;
		}
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		nameInput.text = "";
		FillList();
	}

	/// <summary>
	/// Active ou désactive les boutons des fichiers de sauvegarde selon la présence ou non du texte de la barre de
	/// recherche dans le nom des fichiers de sauvegarde associés à ces boutons.
	/// Une chaîne de caractères vide active tous les boutons.
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
	/// Réinitialise le texte de la barre de recherche.
	/// </summary>
	public void ClearSearch()
	{
		searchField.text = "";
	}
}
