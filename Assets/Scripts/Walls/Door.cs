using UnityEngine;

/// <summary>
/// Paramètres du prefab de type porte.
/// Il peut être placé sur un seul ("normal") ou sur deux murs adjacents ("through2").
/// width, heightw et rotation sont à paramétrer à la main en placant la fenêtre sur un mur.
/// Le tableau "busyParts" est l'ensemble des 16eme de murs occupés par la porte. Il est à paramétrer en entrant les
/// numéros des 16eme occupés sur le côté droit d'un mur dirigé vers l'est.
/// Si la porte traverse deux murs, seuls les 16eme du murs de gauche dans la configuration précedemment donnée sont
/// à rentrer.
/// </summary>

public enum DoorType { normal, through2 }

public class Door : PersistableObject
{

	//paramètres et composants
	public float width;
	public float height;
	public float rotation;
	BoxCollider[] colliders;
	public int[] busyParts;

	//type de porte
	public DoorType doorType;

	//image de la porte dans les listes d'objets
	public Sprite sprite;

	/// <summary>
	/// Etablit la référence aux colliders de l'objet puis les désactive.
	/// </summary>
	void Awake()
	{
		colliders = GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider c in colliders)
		{
			c.enabled = false;
		}
	}

	/// <summary>
	/// Active ou désactive les colliders de la porte.
	/// Ils doivent être actifs en mode 1ere personne et inactifs sinon.
	/// </summary>
	public void BoxActive(bool playerActive)
	{
		foreach (BoxCollider c in colliders)
		{
			c.enabled = playerActive;
		}
	}

	/// <summary>
	/// Détruit la porte.
	/// </summary>
	public void DestroyDoor()
	{
		Destroy(gameObject);
	}
}
