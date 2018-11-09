using UnityEngine;

/// <summary>
/// Panneau d'erreur pouvant s'afficher lors de la création d'une nouvelle maison si le nouveau nom de fichier existe
/// déjà, afin de demander à l'utilisateur de confirmer. Si l'utilisateur confirme, le fichier sera écrasé.
/// </summary>

public class OverwriteFilePanel : MonoBehaviour {

	//paramètres de la maison en création
	new string name;
	int x;
	int z;
	CharacterEdition edition;

	//objet qui a permis d'accéder à ce script
	GameObject otherPanel;

	/// <summary>
	/// Ouvre le panneau en sauvegardant les paramètres de la maison.
	/// </summary>
	public void Open(CharacterEdition newEdition, string newName, int newX, int newZ, GameObject panel)
	{
		otherPanel = panel;
		otherPanel.SetActive(false);
		gameObject.SetActive(true);
		edition = newEdition;
		name = newName;
		x = newX;
		z = newZ;
	}

	/// <summary>
	/// Accède au panneau d'édition des profil.
	/// </summary>
	public void Confirm()
	{
		PersistentStorage.NewHome(name, x, z);
		edition.Open(true);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Ferme ce panneau pour pouvoir changer le nom.
	/// </summary>
	public void Close()
	{
		gameObject.SetActive(false);
		otherPanel.SetActive(true);
	}
}
