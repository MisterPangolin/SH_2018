using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attachée à l'objet "NavigationBaker" dans la scene Creation_architecture.
/// Permet de generer les surfaces navigables, c'est à dire celles ou l'utilisateur pourra se déplacer.
/// Les surfaces naviguables sont définies grâce aux meshs. Toute partie de mesh qui est horizontale et plane devient
/// une surface naviguable.
/// Il faut s'assurer que certains objets, comme les lits, ne générent pas des surfaces en ajoutant à leurs prefabs des
/// composants "NavMeshObstacles" si besoin.
/// Tout mesh qui entre en contact avec une surface devient un obstacle, créant un trou dans la surface naviguable. 
/// </summary>
public class NavigationBaker : MonoBehaviour {

	//surface naviguable, générée par la classe
	NavMeshSurface surface;

	/// <summary>
	/// Ajoute une surface à la liste des surfaces naviguables à générer.
	/// </summary>
	public void SetSurface( NavMeshSurface s)
	{
		surface = s;
	}

	/// <summary>
	/// Génère les surfaces naviguables, en utilisant tous les meshs horizontaux enregistrés.
	/// </summary>
	public void Bake()
	{
		if (surface)
		{
			surface.BuildNavMesh();
		}
	}
}
