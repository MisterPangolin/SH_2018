using UnityEngine;

/// <summary>
/// Classe permettant de diriger l'édition des petits objets se placant sur les surfaces des meubles.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlaceableObject : PersistableObject {

	//paramètres et composants de l'objet
	public BoxCollider surface;
	[HideInInspector]
	public PlaceableObjectCollision collision;
	[HideInInspector]
	public MeshRenderer[] renderers, childRenderers;
	cakeslice.Outline[] outlines;
	Collider[] colliders;
	bool colliderActive = true;
	[HideInInspector]
	public int level = -1;

	//placement
	[HideInInspector]
	public Vector3[] vertex;

	//image de l'objet dans les listes d'objets
	public Sprite sprite;

	//objet connecté
	[HideInInspector]
	public bool hasDevice;
	public Device device;

	//catégorie de l'objet
	public Category category;


	/// <summary>
	/// Etablit les références aux différents colliders et renderers de l'objet.
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
		vertex = Metrics.GetColliderVertexPositions(surface);
		collision = gameObject.AddComponent<PlaceableObjectCollision>();
		var r = GetComponent<Rigidbody>();
		r.constraints = RigidbodyConstraints.FreezeAll;
		childRenderers = GetComponentsInChildren<MeshRenderer>();
		renderers = GetComponents<MeshRenderer>();
		outlines = AddOutline();
		hasDevice |= GetComponent<Device>();
		if (hasDevice)
		{
			device = GetComponent<Device>();
		}
	}

	/// <summary>
	/// A chaque frame, enregistre les positions des coins du collider servant de base à l'objet.
	/// Pour être placé sur une surface, il faut que le quadrilatère formé par les 4 coins enregistrés se place dans
	/// le quadrilatère de la surface choisie.
	/// </summary>
	void Update()
	{
		vertex = Metrics.GetColliderVertexPositions(surface);
	}

	/// <summary>
	/// Ajoute des composants "Outline" à l'objet (s'il a un renderer ) et à ses enfants.
	/// </summary>
	cakeslice.Outline[] AddOutline()
	{
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		int count = meshRenderers.Length;
		cakeslice.Outline[] newOutlines = new cakeslice.Outline[count + 1];
		if (GetComponent<MeshRenderer>())
		{
			newOutlines[0] = gameObject.AddComponent<cakeslice.Outline>();
			for (int i = 0; i < count; i++)
			{
				newOutlines[i + 1] = meshRenderers[i].transform.gameObject.AddComponent<cakeslice.Outline>();
			}
		}
		else
		{
			newOutlines = new cakeslice.Outline[count];
			for (int i = 0; i < count; i++)
			{
				newOutlines[i] = meshRenderers[i].transform.gameObject.AddComponent<cakeslice.Outline>();
			}
		}
		return newOutlines;
	}

	public bool ColliderActive
	{
		get
		{
			return colliderActive;
		}
		set
		{
			colliderActive = value;
			foreach (Collider c in colliders)
			{
				c.enabled = value;
			}
		}
	}

	/// <summary>
	/// Sauvegarde la position, rotation et taille de l'objet.
	/// </summary>
	public override void Save(HomeDataWriter writer)
	{
		writer.Write(Id);
		writer.Write(transform.position);
		writer.Write(transform.rotation);
		writer.Write(level);
	}

	/// <summary>
	/// Charge la position, rotation et taille de l'objet.
	/// </summary>
	public override void Load(HomeDataReader reader)
	{
		transform.position = reader.ReadVector3();
		transform.rotation = reader.ReadQuaternion();
		level = reader.ReadInt();
		Placed = true;
	}

	/// <summary>
	/// Détruit l'objet.
	/// </summary>
	public void DestroyObject()
	{
		if (hasDevice && level != -1)
		{
			DeviceStorage.Remove(device, level);
		}
		Destroy(gameObject);
	}

	bool placed;
	/// <summary>
	/// Retourne "placed". True si l'objet est placé, false s'il est en train d'être placé.
	/// Définit "placed". Si "placed" passe à true, détruit les composants de collision et "Outline", puis ajoute 
	/// l'objet à la liste des capteurs si c'en est un.
	/// Sinon, enlève l'objet à la liste des capteurs si c'en est un, puis lui ajoute les composants de collision et
	/// "Outline".
	/// </summary>
	public bool Placed
	{
		get
		{
			return placed;
		}
		set
		{
			placed = value;
			if (placed)
			{
				Destroy(collision);
				for (int i = 0; i < outlines.Length; i++)
				{
					Destroy(outlines[i]);
				}
				if (hasDevice && level != -1)
				{
					DeviceStorage.Add(device, level);
				}
			}
			else
			{
				if (hasDevice && level != -1)
				{
					DeviceStorage.Remove(device, level);
				}
				collision = gameObject.AddComponent<PlaceableObjectCollision>();
				outlines = AddOutline();
			}
		}
	}

	/// <summary>
	/// Si l'objet a un composant "PlaceableObjetCollision", renvoie la valeur du paramètre "contact" du composant.
	/// Renvoie false sinon.
	/// </summary>
	public bool Contact
	{
		get
		{
			if (collision)
			{
				return collision.contact;
			}
			else
			{
				return false;
			}
		}
	}

	bool rendererActive = true;
	/// <summary>
	/// Active ou désactive les MeshRenderers de l'objet ou de ses enfants.
	/// Utilisé seulement en mode édition pour cacher les objets à ajouter/déplacer.
	/// </summary>
	public bool RendererActive
	{
		get
		{
			return rendererActive;
		}
		set
		{
			rendererActive = false;
			foreach (MeshRenderer r in renderers)
			{
				r.enabled = value;
			}
			foreach (MeshRenderer r in childRenderers)
			{
				r.enabled = value;
			}
		}
	}

	/// <summary>
	/// Efface le lien entre l'objet et sa surface parente, puis lui ajoute grâce à "Placed" les composants pour 
	/// permettre son déplacement.
	/// </summary>
	public void GetObject()
	{
		transform.parent = null;
		Placed = false;
	}

	/// <summary>
	/// Active ou désactive les composants "Outline" de l'objet.
	/// </summary>
	public void EnableOutline(bool enabled)
	{
		for (int i = 0; i < outlines.Length; i++)
		{
			outlines[i].enabled = enabled;
		}
	}

	/// <summary>
	/// Définit la couleur des composants "Outline" de l'objet.
	/// </summary>
	public void SetOutline(bool placeable)
	{
		if (placeable)
		{
			for (int i = 0; i < outlines.Length; i++)
			{
				outlines[i].color = 1;
			}
		}
		else
		{
			for (int i = 0; i < outlines.Length; i++)
			{
				outlines[i].color = 0;
			}
		}
	}
}
