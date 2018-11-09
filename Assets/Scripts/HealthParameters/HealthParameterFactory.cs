using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "healthParameter" dans un seul asset.
/// Un nouveau tableau peut être créé grâce au sous-menu "Assets/Create/Health Parameter Factory".
/// </summary>

//Crée une option dans le sous-menu asset pour créer un nouvel asset HealthParameterFactory
[CreateAssetMenu]
public class HealthParameterFactory : ScriptableObject
{
	//tableau des paramètres de santé à utiliser dans le simulateur
	[SerializeField]
	HealthParameter[] parameters;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public HealthParameter Get(int id = 0)
	{
		HealthParameter parameter = parameters[id];
		return parameter;
	}

	/// <summary>
	/// Renvoie la taille du tableau "parameters".
	/// </summary>
	public int GetSize()
	{
		return parameters.Length;
	}
}
