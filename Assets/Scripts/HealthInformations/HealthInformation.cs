 using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe donnant les attributs d'une information médicale, et permettant de la créer.
/// Une nouvelle information peut être ajoutée aux assets grâce au sous-menu "Assets/Create/Health information".
/// </summary>

//Crée une nouvelle option dans le sous-menu asset pour créer une nouvelle information médicale.
[CreateAssetMenu]
public class HealthInformation : ScriptableObject {

	//champs de texte
	public new string name;
	[TextArea(4, 6)]
	public string description;
	[TextArea(4, 6)]
	public string management;

	//paramètres à suivre
	public HealthParameter[] healthP;
	public HomeParameter[] homeP;

	/// <summary>
	/// Crée le bouton "button" et change son texte pour correspondre à ce problème médical.
	/// </summary>
	public GameObject InstantiateInformationButton(GameObject button)
	{
		GameObject b = Instantiate(button);
		b.GetComponentInChildren<Text>().text = name;
		return b;
	}

	/// <summary>
	/// Vérifie que le problème est suivi en vérifiant si tous les paramètres importants concernant ce problème sont
	/// suivis.
	/// Renvoie true si le problème est suivi, false sinon.
	/// </summary>
	public bool isFollowed()
	{
		foreach (HealthParameter parameter in healthP)
		{
			if (!parameter.IsMeasured())
			{
				return false;
			}
		}
		return true;
	}
}
