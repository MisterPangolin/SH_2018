using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "window" dans un seul asset.
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset WindowFactory
[CreateAssetMenu]
public class WindowFactory : ScriptableObject {

	[SerializeField]
	Window[] prefabs;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public Window Get(int id = 0)
	{
		Window instance = Instantiate(prefabs[id]);
		instance.Id = id;
		return instance;
	}

	/// <summary>
	/// Renvoie la taille du tableau "prefabs".
	/// </summary>
	public int GetSize()
	{
		return prefabs.Length;
	}
}
