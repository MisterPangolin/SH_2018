using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panneau d'erreur pouvant s'afficher lors de la création d'une nouvelle maison pour expliquer pourquoi la maison ne
/// peut pas être créées à partir des informations données.
/// </summary>

public class ErrorPanel : MonoBehaviour {

	//texte d'erreur
	public Text text;

	//objet qui a permis d'accéder à ce script
	GameObject otherPanel;

	/// <summary>
	/// Ouvre à partir du panneau donné le panneau d'erreur, en affichant le message d'erreur donné.
	/// </summary>
	public void Open(string error, GameObject panel)
	{
		otherPanel = panel;
		otherPanel.SetActive(false);
		gameObject.SetActive(true);
		text.text = error;
	}

	/// <summary>
	/// Ferme le panneau et affiche le panneau "otherPanel".
	/// </summary>
	public void Close()
	{
		gameObject.SetActive(false);
		otherPanel.SetActive(true);
		otherPanel = null;
	}
}
