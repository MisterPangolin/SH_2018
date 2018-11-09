using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;

public enum ObjectType { feature, door, window, wallFeature, placeableObject }

/// <summary>
/// Stocke les références à toutes les listes d'objets pour la création de 
/// maison, et permet d'instancier ces objets.
/// Permet le chargement et la sauvegarde de la maison en faisant le lien avec la classe "PersistentStorage".
/// Dirige le menu de sauvegarde.
/// </summary>

public class EditorManagement : PersistableObject {

	//éditeur
	public HomeEditor editor;

	//listes des prefabs d'objets puvant être instanciés
	public FeatureFactory featureFactory;
	public FeatureFactory wallFactory;
	public DoorFactory doorFactory;
	public WindowFactory windowFactory;
	public ObjectFactory objectFactory;

	//mode d'ouverture du panneau de sauvegarde/chargement
	bool saveMode;
	public Text actionButtonLabel;

	//recherche et sélection de fichiers
	public InputField nameInput;
	public InputField searchField;

	//noms des fichiers
	public RectTransform listContent;
	public SaveLoadItem itemPrefab, protectedItemPrefab;
	List<GameObject> buttons = new List<GameObject>();

	/// <summary>
	/// Sauvegarde la maison.
	/// </summary>
	public void Save()
	{
		PersistentStorage.Save(this);
	}

	/// <summary>
	/// Charge une maison.
	/// </summary>
	public void Load()
	{
		PersistentStorage.Load(this);
	}

	/// <summary>
	/// Appelle la méthode de sauvegarde de l'éditeur.
	/// </summary>
	public override void Save(HomeDataWriter writer)
	{
		TimeClock.Save(writer);
		CharacterStorage.Save(writer);
		editor.Save(writer);
	}

	/// <summary>
	/// Appelle la méthode de chargement de l'éditeur.
	/// </summary>
	public override void Load(HomeDataReader reader)
	{
		TimeClock.Load(reader);
		DeviceStorage.Clear();
		CharacterStorage.Load(reader);
		CameraSwitch.Load();
		editor.Load(reader);
	}

	/// <summary>
	/// Instancie un objet dans une liste donnée selon un Id donné. Les listes d'objets sont des paramètres de ce 
	/// script.
	/// </summary>
	public GameObject InstantiateObject(int objectId, ObjectType type)
	{
		GameObject o;
		switch (type)
		{
			case ObjectType.feature:
				o = featureFactory.Get(objectId).gameObject;
				break;
			case ObjectType.wallFeature:
				o = wallFactory.Get(objectId).gameObject;
				break;
			case ObjectType.door:
				o = doorFactory.Get(objectId).gameObject;
				break;
			case ObjectType.window:
				o = windowFactory.Get(objectId).gameObject;
				break;
			case ObjectType.placeableObject:
				o = objectFactory.Get(objectId).gameObject;
				break;
			default:
				o = null;
				break;
		}
		return o;
	}

	/// <summary>
	/// Ouvre le menu de sauvegarde selon le mode choisi.
	/// saveMode = true pour sauvegarde et false pour chargement.
	/// Désactive les caméras.
	/// </summary>
	public void Open(bool saveMode)
	{
		this.saveMode = saveMode;
		if (saveMode)
		{
			actionButtonLabel.text = "Sauvegarder";
		}
		else
		{
			actionButtonLabel.text = "Charger";
		}
		FillList();
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Ferme le menu.
	/// Active les caméras.
	/// </summary>
	public void Close()
	{
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Renvoie la chaîne de caractères entrée par l'utilisateur dans le champ du menu de sauvegarde.
	/// Si la chaîne est non vide, change le fichier de sauvegarde à utiliser.
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
	/// Appelée lors de l'appui sur les boutons Sauver ou charger pour déterminer les actions à effectuer.
	/// </summary>
	public void Action()
	{
		GetSelectedPath();
		if (saveMode)
		{
			Save();
		}
		else
		{
			Load();
		}
		Close();
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
		buttons.Clear();
		for (int i = 0; i < listContent.childCount; i++)
		{
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.home");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++)
		{
			SaveLoadItem item = Instantiate(itemPrefab);
			item.editorMenu = this;
			item.HomeName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
			buttons.Add(item.gameObject);
		}

		paths = Directory.GetFiles(Application.streamingAssetsPath, "*.home");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++)
		{
			SaveLoadItem item = Instantiate(protectedItemPrefab);
			item.editorMenu = this;
			PersistentStorage.AddExemple(Path.GetFileName(paths[i]));
			item.HomeName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
			buttons.Add(item.gameObject);
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
			foreach (GameObject b in buttons)
			{
				b.SetActive(true);
			}
		}
		else
		{
			string input = searchField.text.ToLower();
			foreach (GameObject b in buttons)
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
