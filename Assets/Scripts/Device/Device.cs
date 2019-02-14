using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Classe définissant le fonctionnement des objets connectés, et permettant d'en définir les informations.
/// Pour créer un nouvel objet connecté, il suffit d'ajouter ce composant au prefab d'objet souhaité.
/// </summary>
[DisallowMultipleComponent]
public class Device : MonoBehaviour
{
	// Indice de l'objet dans la liste de "DeviceStorage.cs"
	[HideInInspector]
	public int index;

	// Informations de l'objet
	[TextArea(4,6)] 
	public string objectName;
	[TextArea(4, 6)]
	public string objectDescription;
	[TextArea(4, 6)]
	public string objectIndications;

	// interactions avec l'utilisateur
	[HideInInspector]
	public Collider[] colliders;
	List<cakeslice.Outline> outlines = new List<cakeslice.Outline>();
	[HideInInspector]
	public bool isDestroyed;

	// references aux cameras
	[HideInInspector]
	public Camera cameraPlayer;
	[HideInInspector]
	public Camera cameraOverview;

    // paramètre de position du panneau d'information
    public float Y;


	// paramètres suivis par l'objet
	public HomeParameter[] homeParameters;
	[HideInInspector]
	public int[] homePIndex;
	public HealthParameter[] healthParameters;
	[HideInInspector]
	public int[] healthPIndex;

	/// <summary>
	/// Initialise les paramètres et établit les références.
	/// </summary>
	void Awake()
	{
		int childCount = GetComponentsInChildren<Collider>().Length;
		int parentCount = GetComponents<Collider>().Length;
		colliders = new Collider[childCount + parentCount];
		for (int i = 0; i < parentCount; i++)
		{
			colliders[i] = GetComponents<Collider>()[i];
		}
		for (int i = 0; i < childCount; i++)
		{
			colliders[i + parentCount] = GetComponentsInChildren<Collider>()[i];
		}
		AddOutline();
		EnableOutline(false);
		cameraPlayer = GameObject.Find("CameraPlayer").GetComponent<Camera>();
		cameraOverview = GameObject.Find("OverviewCamera").GetComponentInChildren<Camera>();
		index = -1;
		homePIndex = new int[homeParameters.Length];
		healthPIndex = new int[healthParameters.Length];
	}

	/// <summary>
	/// Ajoute des composants "Outline" à l'objet portant le capteur, s'il a un renderer, et à tous ses enfants.
	/// </summary>
	List<cakeslice.Outline> AddOutline()
	{
		outlines.Clear();
		if (GetComponent<MeshRenderer>())
		{
			cakeslice.Outline outline = gameObject.AddComponent<cakeslice.Outline>();
			outline.color = 1;
			outlines.Add(outline);
		}
		int count = transform.childCount;
		for (int i = 0; i < count; i++)
		{
			if (transform.GetChild(i).GetComponent<MeshRenderer>())
			{
				cakeslice.Outline outline = transform.GetChild(i).gameObject.AddComponent<cakeslice.Outline>();
				outline.color = 1;
				outlines.Add(outline);
			}
		}
		return outlines;
	}

	/// <summary>
	/// Définit la couleur des composants "Outline" de l'objet.
	/// </summary>
	public void SetOutlineColor(int c)
	{
		foreach (cakeslice.Outline outline in outlines)
		{
			outline.color = c;
		}
	}

	/// <summary>
	/// Active ou désactive les composants "Outline" de l'objet.
	/// </summary>
	public void EnableOutline(bool enabled, int c = 0)
	{
		foreach(cakeslice.Outline outline in outlines)
		{
			outline.enabled = enabled;
		}
		if (enabled)
		{
			SetOutlineColor(c);
		}
	}

	/// <summary>
	/// Renvoie une chaîne de caractère décrivant les mesures effectuées par le capteur.
	/// </summary>
	public string Data()
	{
		string data = "";
		foreach (HealthParameter parameter in healthParameters)
		{
			if (!parameter.isABool)
			{
				data += "Mesure de " + parameter.name + " effectuée";
				data += "\n";
				CharacterStorage.RefreshCharactersData(parameter, TimeClock.GetInstant(), 0f);
			}
		}
		foreach (HomeParameter function in homeParameters)
		{
			data += function.name + " : " + function.measure;
			data += "\n";
		}
		if (data != "")
		{
			data += "le " + TimeClock.GetTime();
		}
		return data;
	}

	/// <summary>
	/// Active ou désactive les colliders de l'objet.
	/// </summary>
	public void EnableColliders(bool enabled)
	{
		foreach (Collider c in colliders)
		{
			c.enabled = enabled;
		}
	}

	/// <summary>
	/// Instancie le bouton "button" en chnageant son texte pour le nom de l'objet.
	/// </summary>
	public GameObject InstantiateDeviceButton(GameObject button)
	{
		GameObject b = Instantiate(button);
		b.GetComponentInChildren<Text>().text = objectName;
		return b;
	}

	/// <summary>
	/// Renvoie la référence du paramètre "prm" dans la liste des paramètres de maison de l'objet.
	/// </summary>
	public int HomeReference(HomeParameter prm)
	{
		int reference = -1;
		for (int i = 0; i < homeParameters.Length; i++)
		{
			if (homeParameters[i].name == prm.name)
			{
				reference = i;
			}
		}
		return reference;
	}

	/// <summary>
	/// Renvoie la référence du paramètre "prm" dans la liste des paramètres de santé de l'objet.
	/// </summary>
	public int HealthReference(HealthParameter prm)
	{
		int reference = -1;
		for (int i = 0; i < healthParameters.Length; i++)
		{
			if (healthParameters[i].name == prm.name)
			{
				reference = i;
			}
		}
		return reference;
	}
}
