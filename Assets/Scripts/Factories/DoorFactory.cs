using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "door" dans un seul asset.
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset DoorFactory
[CreateAssetMenu]
public class DoorFactory : ScriptableObject {

	[SerializeField]
	Door[] prefabs;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public Door Get(int id = 0)
	{
		Door instance = Instantiate(prefabs[id]);
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
