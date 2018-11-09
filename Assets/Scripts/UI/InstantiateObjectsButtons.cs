using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Permet, lorsque l'utilisateur ouvre pour la première fois le panneau d'ajout de meubles, de créer les boutons
/// associés aux objets.
/// L'appui sur un bouton de porte permet de sélectionner le mode d'édition de "editor" et de dénifir la référence
/// de l'objet à instancier.
/// </summary>
public class InstantiateObjectsButtons : MonoBehaviour {

	//catégorie
	public Category category;

	//mode d'édition
	public HomeEditor editor;

	//listes des objets
	public FeatureFactory cellFeatures;
	public FeatureFactory wallFeatures;
	public ObjectFactory objects;

	//bouton
	public GameObject prefabButton;

	//panneau de description des objets connectés
	public DevicePanel devicePanel;

	/// <summary>
	/// Appelée lors de la première activation de l'objet ayant pour composant cette classe pour créer les boutons.
	/// Un bouton est créé seulement si le meuble a un sprite en paramètre de sa classe "feature".
	/// </summary>
	void Awake()
	{
		int size = cellFeatures.GetSize();
		for (int i = 0; i < size; i++)
		{
			Feature feature = cellFeatures.Get(i);
			if (category == feature.category && feature.sprite)
			{
				InstantiateCellButton(feature, i);
			}
			feature.DestroyFeature();
		}
		size = wallFeatures.GetSize();
		for (int i = 0; i < size; i++)
		{
			Feature feature = wallFeatures.Get(i);
			if (category == feature.category && feature.sprite)
			{
				InstantiateWallButton(feature, i);
			}
			feature.DestroyFeature();
		}
		size = objects.GetSize();
		for (int i = 0; i < size; i++)
		{
			PlaceableObject o = objects.Get(i);
			if (category == o.category && o.sprite)
			{
				InstantiateObjectButton(o, i);
			}
			o.DestroyObject();
		}
	}

	/// <summary>
	/// Instancie un bouton selon les paramètres de "feature". "i" est l'indice dans "features" de "feature".
	/// </summary>
	void InstantiateCellButton(Feature feature, int i)
	{
		GameObject button = Instantiate(prefabButton);
		button.GetComponent<Image>().sprite = feature.sprite;
		button.GetComponent<Button>().onClick.AddListener(delegate { SetFeatureOnClick(i, 7); });
		button.transform.SetParent(transform, false);
		if (feature.hasDevice)
		{
			Device device = feature.gameObject.GetComponent<Device>();
			EnableDeviceButton(button, device);
		}
	}

	/// <summary>
	/// Instancie un bouton selon les paramètres de "feature". "i" est l'indice dans "features" de "feature".
	/// </summary>
	void InstantiateWallButton(Feature feature, int i)
	{
		GameObject button = Instantiate(prefabButton);
		button.GetComponent<Image>().sprite = feature.sprite;
		button.GetComponent<Button>().onClick.AddListener(delegate { SetFeatureOnClick(i, 11); });
		button.transform.SetParent(transform, false);
		if (feature.hasDevice)
		{
			Device device = feature.gameObject.GetComponent<Device>();
			EnableDeviceButton(button, device);
		}
	}

	/// <summary>
	/// Instancie un bouton selon les paramètres de "o". "i" est l'indice dans "objects" de "o".
	/// </summary>
	void InstantiateObjectButton(PlaceableObject o, int i)
	{
		GameObject button = Instantiate(prefabButton);
		button.GetComponent<Image>().sprite = o.sprite;
		button.GetComponent<Button>().onClick.AddListener(delegate { SetObjectOnClick(i, 12); });
		button.transform.SetParent(transform, false);
		if (o.hasDevice)
		{
			Device sensor = o.gameObject.GetComponent<Device>();
			EnableDeviceButton(button, sensor);
		}
	}

	/// <summary>
	/// Si le meuble dont on instancie le bouton à un capteur, permet d'ajouter un autre bouton par dessus le premier
	/// bouton, indiquant que l'objet a un capteur, et permettant en cliquant dessus d'afficher sa description.
	/// </summary>
	void EnableDeviceButton(GameObject button, Device device)
	{
		GameObject deviceButton = button.transform.GetChild(0).gameObject;
		deviceButton.SetActive(true);
		deviceButton.GetComponent<Button>().onClick.
						AddListener(delegate { OpenDescriptionOnClick(device); });
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton créé pour sélectionner le bon mode d'édition dans "editor" ainsi que la bonne
	/// référence de meuble.
	/// </summary>
	void SetFeatureOnClick(int i, int editionMode)
	{
		editor.SetMode(editionMode);
		editor.SetFeatureRef(i);
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton créé pour sélectionner le bon mode d'édition dans "editor" ainsi que la bonne
	/// référence d'objet.
	/// </summary>
	void SetObjectOnClick(int i, int editionMode)
	{
		editor.SetMode(editionMode);
		editor.SetObjectRef(i);
	}

	/// <summary>
	/// Fonction attribué au deuxième bouton, si le meuble a un capteur, pour afficher sa description.
	/// </summary>
	void OpenDescriptionOnClick(Device device)
	{
		devicePanel.Open(device);
	}
}