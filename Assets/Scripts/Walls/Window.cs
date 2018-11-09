using UnityEngine;

/// <summary>
/// Paramètres du prefab de type fenêtre.
/// Il peut être placé sur un seul ("normal") ou sur deux murs adjacents ("through2").
/// width, heightw et heightw2 sont à paramétrer à la main en placant la fenêtre sur un mur.
/// Le tableau "busyParts" est l'ensemble des 16eme de murs occupés par la fenêtre. Il est à paramétrer en entrant les
/// numéros des 16eme occupés sur le côté droit d'un mur dirigé vers l'est.
/// Si la fenêtre traverse deux murs, seuls les 16eme du murs de gauche dans la configuration précedemment donnée sont
/// à rentrer.
/// </summary>
public enum WindowType { normal, through2 }

public class Window : PersistableObject {

	//paramètres de la fenêtre
	public float width;
	public float heightw;
	public float heightw2;
	public int[] busyParts;

	//type de fenêtre
	public WindowType windowType;

	//image de la fenêtre dans les listes d'objets
	public Sprite sprite;

	/// <summary>
	/// Détruit la fenêtre.
	/// </summary>
	public void DestroyWindow()
	{
		Destroy(gameObject);
	}
}
