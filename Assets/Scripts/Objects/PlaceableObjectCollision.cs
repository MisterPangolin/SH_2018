using UnityEngine;

/// <summary>
/// Composant s'ajoutant à un "placeableObject" lors de sa création ou lorsque l'utilisateur souhaite le changer de
/// place.
/// Il est utile pour le positionnement de l'objet afin de vérifier que l'objet ne rentre pas en collision avec d'autres
/// objets ou features.
/// Les méthodes utilisées sont natives à Unity et très lourdes, le composant est donc supprimé dès que l'objet est
/// placé.
/// </summary>
public class PlaceableObjectCollision : MonoBehaviour {

	//Si true, l'objet est en contact avec autre chose et ne peut être placé.
	public bool contact;

	/// <summary>
	/// Vérifie que l'objet ne rentre pas en collision avec autre chose que la surface sur laquelle il doit se placer.
	/// </summary>
	public void OnCollisionEnter(Collision other)
	{
		if (!other.gameObject.GetComponent<AvailableSurface>())
		{
			contact = true;
		}
	}

	/// <summary>
	/// Vérifie que l'objet n'est pas en contact avec autre chose que la surface sur laquelle il doit se placer.
	/// </summary>
	public void OnCollisionStay(Collision other)
	{
		if (!other.gameObject.GetComponent<AvailableSurface>())
		{
			contact = true;
		}
	}

	/// <summary>
	/// Passe contact à false si l'objet n'est plus en contact avec un objet.
	/// Cependant, si l'objet est en contact avec d'autres objets, contact sera toujours égal à true.
	/// </summary>
	public void OnCollisionExit(Collision other)
	{
		if (!other.gameObject.GetComponent<AvailableSurface>())
		{
			contact = false;
		}
	}
}
