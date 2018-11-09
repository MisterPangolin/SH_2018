using UnityEngine;

/// <summary>
/// A appliquer aux boutons exit pour pouvoir fermer l'application.
/// </summary>
public class ExitApplication : MonoBehaviour {

	/// <summary>
	/// Provoque la fermeture de l'application lors d'un clique gauche sur le bouton Exit.
	/// </summary>
	public void TaskOnClickExit()
	{
		Application.Quit();
	}
}
