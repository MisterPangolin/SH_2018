using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Permet, lorsque l'utilisateur ouvre pour la première fois le panneau d'ajout de portes, de créer les boutons
/// associés aux portes.
/// L'appui sur un bouton de porte permet de sélectionner le mode d'édition de "editor" et de dénifir la référence
/// de la porte à instancier.
/// </summary>
public class InstantiateDoorsButtons : MonoBehaviour {

	//mode d'édition
	public HomeEditor editor;
	public int editionMode;

	//liste des portes et bouton
	public DoorFactory doors;
	public GameObject prefabButton;


	/// <summary>
	/// Appelée lors de la première activation de l'objet ayant pour composant cette classe pour créer les boutons.
	/// Un bouton est créé seulement si la porte a un sprite en paramètre de sa classe "door".
	/// </summary>
	void Awake()
	{
		int size = doors.GetSize();
		for (int i = 0; i < size; i++)
		{
			Door door = doors.Get(i);
			if (door.sprite)
			{
				InstantiateButton(door, i);
			}
			door.DestroyDoor();
		}
	}

	/// <summary>
	/// Instancie un bouton selon les paramètres de "door". "i" est l'indice dans "doors" de "door".
	/// </summary>
	void InstantiateButton(Door door, int i)
	{
		GameObject button = Instantiate(prefabButton);
		button.GetComponent<Image>().sprite = door.sprite;
		button.GetComponent<Button>().onClick.AddListener(delegate { SetDoorOnClick(i); });
		button.transform.SetParent(transform, false);
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton créé pour sélectionner le bon mode d'édition dans "editor" ainsi que la bonne
	/// référence de porte.
	/// </summary>
	void SetDoorOnClick(int i)
	{
		editor.SetMode(editionMode);
		editor.SelectDoor(i);
	}
}
