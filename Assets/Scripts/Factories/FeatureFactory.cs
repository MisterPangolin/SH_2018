using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "feature" dans un seul asset.
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset FeatureFactory
[CreateAssetMenu]
public class FeatureFactory : ScriptableObject {

	[SerializeField]
	Feature[] prefabs;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public Feature Get(int id = 0)
	{
		Feature instance = Instantiate(prefabs[id]);
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
