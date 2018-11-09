using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Paramètre de la maison.
/// Il est possible de créer de nouveaux paramètres dans les assets de Unity et d'en choisir les informations.
/// Pour créer un nouveau paramètre, il suffit d'aller dans le sous-menu asset puis "Create/Home Parameter".
/// </summary>
[CreateAssetMenu]
public class HomeParameter : ScriptableObject {

	//champs de texte
	public new string name;
	[TextArea(4, 6)]
	public string description;
	[TextArea(4, 6)]
	public string indications;
	public string measure;

	//référence du paramètre dans le tableau des paramètres utilisés dans le simulateur, donné par HomeParameterFactory
	[HideInInspector]
	public int reference;

	//indiquent si le paramètre doit être suivi ou non
	[Header("Paramètre nécessaire de la maison")]
	public bool majorParameter;
	bool mustBeFollowed;


	/// <summary>
	/// Instancie le bouton "button" et change son texte pour le nom de la fonction.
	/// </summary>
	public GameObject InstantiateHomeButton(GameObject button)
	{
		GameObject b = Instantiate(button);
		b.GetComponentInChildren<Text>().text = name;
		return b;
	}

	/// <summary>
	/// Vérifie que le paramètre est suivi grâce à la liste des capteurs présents dans la maison. Si la liste de 
	/// capteurs pouvant relever ce paramètre à une taille supérieure ou égale à 1, le paramètre est considéré comme
	/// suivi.
	/// Renvoie true si le paramètre est suivi, false sinon.
	/// </summary>
	public bool IsFollowed()
	{
		if (DeviceStorage.GetList(this).Length > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// True si le paramètre est indiqué comme paramètre important pour le fonctionnement de la maison, ou qu'un
	/// des profils est lié à ce paramètre.
	/// False sinon.
	/// </summary>
	public bool MustBeFollowed
	{
		get
		{
			if (majorParameter)
			{
				return true;
			}
			return mustBeFollowed;
		}
		set
		{
			mustBeFollowed = value;
		}
	}
}
