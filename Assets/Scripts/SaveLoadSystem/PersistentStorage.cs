using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// Classe unique qui ne peut être instanciée. N'importe quelle partie du projet peut y accéder.
/// Conserve le nom du fichier de sauvegarde à utiliser.
/// Permet le chargement et la sauvegarde de la maison.
/// </summary>
public static class PersistentStorage 
{
	//fichier de sauvegarde
	static string savePath;
	public static string savePathName = "quickSave.home";
	static List<string> exemplesPaths = new List<string>();

	//booléen indiquant si la maison vient d'être crée
	public static bool newHome;

	//dimensions de la maison
	public static int X, Z;

	/// <summary>
	/// Initialise le chemin de sauvegarde, en donnant pour nom au fichier de sauvegarde "quickSave.home"
	/// </summary>
	public static void Awake()
	{
		if (ContainsExemple(savePathName))
		{
			savePath = Path.Combine(Application.streamingAssetsPath, savePathName);
		}
		else
		{
			savePath = Path.Combine(Application.persistentDataPath, savePathName);
		}
	}

	/// <summary>
	/// Sauvegarde l'objet o.
	/// </summary>
	public static void Save(PersistableObject o)
	{
		if (Application.isEditor || !ContainsExemple(savePathName))
		{
			using (
				var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
			)
			{
				o.Save(new HomeDataWriter(writer));
			}
		}
		else
		{
			Debug.Log("ceci est un fichier protégé " + savePathName);
		}
	}

	/// <summary>
	/// Charge l'objet o.
	/// Renvoie un message d'erreur si le fichier de sauvegarde n'existe pas.
	/// </summary>
	public static void Load(PersistableObject o)
	{
        if (!File.Exists(savePath))
		{
			Debug.LogError("le fichier n'existe pas " + savePath);
		}
		else
		{
			using (
				var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
			)
			{
				o.Load(new HomeDataReader(reader));
            }
		}
	}

	/// <summary>
	/// Change le nom du fichier de sauvegarde.
	/// Ce nom peut être changé dans le champ du menu de sauvegarde.
	/// </summary>
	public static void ChangePathName(string name)
	{
		savePathName = name;
		if (ContainsExemple(savePathName))
		{
			savePath = Path.Combine(Application.streamingAssetsPath, savePathName);
		}
		else
		{
			savePath = Path.Combine(Application.persistentDataPath, savePathName);
		}
	}

	/// <summary>
	/// Change le fichier de sauvegarde et enregistre les dimensions x et z qui seront utilisés par la classe HomeEditor
	/// lors de son chargement.
	/// </summary>
	public static void NewHome(string name, int x, int z)
	{
		ChangePathName(name);
		X = x;
		Z = z;
		newHome = true;
	}

	/// <summary>
	/// Vérifie l'existence du fichier, afin de prévenir lorsqu'il souhaite créer une nouvelle maison qu'il risque
	/// d'écraser un fichier.
	/// </summary>
	public static bool CheckifExists(string name)
	{
		if (ContainsExemple(savePathName))
		{
			savePath = Path.Combine(Application.streamingAssetsPath, name);
		}
		else
		{
			savePath = Path.Combine(Application.persistentDataPath, name);
		}
		bool exists = false;
		if (!File.Exists(savePath))
		{
			exists = true;
		}
		return exists;
	}

	/// <summary>
	/// Permet d'ajouter un exemple dans la liste des exempls.
	/// </summary>
	public static void AddExemple(string name)
	{
		if (exemplesPaths.Count > 0)
		{
			if (!exemplesPaths.Contains(name))
			{
				exemplesPaths.Add(name);
			}
		}
		else
		{
			exemplesPaths.Add(name);
		}
	}

	/// <summary>
	/// Vérifie le nom du fichier avant de l'ajouter aux exemples.
	/// </summary>
	public static bool ContainsExemple(string name)
	{
		if (exemplesPaths.Count > 0)
		{
			return exemplesPaths.Contains(name);
		}
		else
		{
			return false;
		}
	}
}