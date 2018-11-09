using UnityEngine;

/// <summary>
/// Classe permettant de stocker les références aux assets "homeParameter" dans un seul asset.
/// Un nouveau tableau peut être créé grâce au sous-menu "Assets/Create/Home Parameter Factory".
/// </summary>

[CreateAssetMenu]
public class HomeParameterFactory : ScriptableObject {

	[SerializeField]
	HomeParameter[] functions;

	/// <summary>
	/// Renvoie l'élément d'indice id de la liste.
	/// </summary>
	public HomeParameter Get(int id = 0)
	{
		HomeParameter function = functions[id];
		return function;
	}

	/// <summary>
	/// Renvoie la taille du tableau "parameters".
	/// </summary>
	public int GetSize()
	{
		return functions.Length;
	}
}
