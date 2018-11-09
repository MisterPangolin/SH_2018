using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "placeableObject" dans un seul asset.
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset ObjectFactory
[CreateAssetMenu]
public class ObjectFactory : ScriptableObject {

	[SerializeField]
	PlaceableObject[] prefabs;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public PlaceableObject Get(int id = 0)
	{
		PlaceableObject instance = Instantiate(prefabs[id]);
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
