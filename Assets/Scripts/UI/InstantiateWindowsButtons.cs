using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Permet, lorsque l'utilisateur ouvre pour la première fois le panneau d'ajout de fenêtres, de créer les boutons
/// associés aux fenêtres.
/// L'appui sur un bouton de fenêtre permet de sélectionner le mode d'édition de "editor" et de dénifir la référence
/// de la fenêtre à instancier.
/// </summary>

public class InstantiateWindowsButtons : MonoBehaviour {

	//mode d'édition
	public int editionMode;
	public HomeEditor editor;

	//prefab du bouton
	public GameObject prefabButton;

	//tableau des fenêtres utilisées
	public WindowFactory windows;

	/// <summary>
	/// Appelée lors de la première activation de l'objet ayant pour composant cette classe pour créer les boutons.
	/// Un bouton est créé seulement si le meuble a un sprite en paramètre de sa classe "window".
	/// </summary>
	void Awake()
	{
		int size = windows.GetSize();
		for (int i = 0; i < size; i++)
		{
			Window window = windows.Get(i);
			if (window.sprite)
			{
				InstantiateButton(window, i);
			}
			window.DestroyWindow();
		}
	}

	/// <summary>
	/// Instancie un bouton selon les paramètres de "window". "i" est l'indice dans "windows" de "window".
	/// </summary>
	void InstantiateButton(Window window, int i)
	{
		GameObject button = Instantiate(prefabButton);
		button.GetComponent<Image>().sprite = window.sprite;
		button.GetComponent<Button>().onClick.AddListener(delegate { SetWindowOnClick(i); });
		button.transform.SetParent(transform, false);
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton créé pour sélectionner le bon mode d'édition dans "editor" ainsi que la bonne
	/// référence de fenêtre.
	/// </summary>
	void SetWindowOnClick(int i)
	{
		editor.SetMode(editionMode);
		editor.SelectWindow(i);
	}
}
