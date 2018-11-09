using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "disease" dans un seul asset.
/// Une nouvelle liste peut être créée grâce au sous-menu "Assets/Create/Health Informations List".
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset DiseaseFactory
[CreateAssetMenu]
public class HealthInformationsList : ScriptableObject {

	//Tableau des informations de santé à utiliser dans le simulateur
	[SerializeField]
	HealthInformation[] informations;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public HealthInformation Get(int shapeId = 0)
	{
		HealthInformation information = informations[shapeId];
		return information;
	}

	/// <summary>
	/// Renvoie la taille du tableau "diseases".
	/// </summary>
	public int GetSize()
	{
		return informations.Length;
	}

	/// <summary>
	/// Renvoie l'indice de "disease" dans "diseases".
	/// Chaque Disease doit être placé une unique fois dans le tableau.
	/// </summary>
	public int GetRef(HealthInformation information)
	{
		int reference = System.Array.IndexOf(informations, information);
		return reference;
	}
}
