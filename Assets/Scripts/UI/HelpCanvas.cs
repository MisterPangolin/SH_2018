using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panneau d'aide. Permet d'ouvrir le panneau de façon à afficher les pages de l'article choisi, et de naviguer entre
/// les pages d'un article.
/// </summary>
public class HelpCanvas : MonoBehaviour {

	//référence à lui-même
	static HelpCanvas instance;

	//zone d'affichage
	public Transform helpPanel;

	//catégorie actuelle
	GameObject[] sheets;
	int index, length;

	//navigation entre les pages
	public GameObject layoutNav;
	public Text navText;
	public Button navLeftArrow, navRightArrow;

	/// <summary>
	/// Etablit une référence unique dans la scène à ce script, pouvant ainsi être appelé par tous les autres scripts.
	/// </summary>
	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Nettoie la zone d'affichage.
	/// Met en place la première page de la nouvelle catégorie.
	/// Actualise la zone de navigation entre les pages.
	/// </summary>
	public static void Open(GameObject[] newLayouts)
	{
		if (instance.length > 0)
		{
			foreach (GameObject layout in instance.sheets)
			{
				Destroy(layout);
			}
		}
		instance.sheets = (GameObject[])newLayouts.Clone();
		for (int i = 0; i < newLayouts.Length; i++)
		{
			instance.sheets[i] = Instantiate(instance.sheets[i]);
		}
		instance.length = instance.sheets.Length;
		instance.sheets[0].SetActive(true);
		instance.sheets[0].transform.SetParent(instance.helpPanel, false);
		if (instance.length > 1)
		{
			for (int i = 1; i < instance.length; i++)
			{
				instance.sheets[i].SetActive(false);
				instance.sheets[i].transform.SetParent(instance.helpPanel, false);
			}
			instance.layoutNav.SetActive(true);
			instance.navLeftArrow.gameObject.SetActive(false);
			instance.navRightArrow.gameObject.SetActive(true);
			instance.navText.text = 1 + " / " + instance.length;
		}
		else
		{
			instance.layoutNav.SetActive(false);
		}
		instance.index = 0;
	}

	/// <summary>
	/// Passe à la page de gauche.
	/// Met à jour la zone de navigation.
	/// </summary>
	public void ReadLeftSheet()
	{
		sheets[index].SetActive(false);
		index -= 1;
		sheets[index].SetActive(true);
		if (index == 0)
		{
			navLeftArrow.gameObject.SetActive(false);
		}
		navRightArrow.gameObject.SetActive(true);
		navText.text = (index + 1) + " / " + length;
	}

	/// <summary>
	/// Passe à la page de droit.
	/// Met à jour la zone de navigation.
	/// </summary>
	public void ReadRightSheet()
	{
		sheets[index].SetActive(false);
		index += 1;
		sheets[index].SetActive(true);
		if (index == length - 1)
		{
			navRightArrow.gameObject.SetActive(false);
		}
		navLeftArrow.gameObject.SetActive(true);
		navText.text = (index + 1) + " / " + length;
	}
}
