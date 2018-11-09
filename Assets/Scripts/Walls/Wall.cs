using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Elément de la grille.
/// Classe qui dirige le fonctionnement des murs.
/// Les murs sont tous instanciés lors de la création de la grille, mais ils n'existent pas encore.
/// L'utilisateur doit les construire grâce à sa souris, ce qui affichera le mesh du mur.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Wall : MonoBehaviour {

	//parents et composants
	[HideInInspector]
	public Dot dot, neighbor;
	[HideInInspector]
	public GridChunk chunk;
	[HideInInspector]
	public BoxCollider[] boxes;
	[HideInInspector]
	public CompassDirection direction;
	bool boxActive;

	//paramètres
	public int level;
	Color colorLeft, colorRight;

	//taille
	float elevation;
	bool raised;

	//fenêtre
	int window = -1;
	GameObject o_window;
	[HideInInspector]
	public bool hasIncomingOpening, hasOutgoingOpening;
	bool hasIncomingDoor, hasIncomingWindow;
	[HideInInspector]
	public bool hideOpening;

	//porte
	int door = -1;
	GameObject o_door;

	//features
	[HideInInspector]
	public Feature[] cellFeature, carpetFeature;
	[HideInInspector]
	public GameObject[] featuresLeft = new GameObject[16];
	[HideInInspector]
	public GameObject[] featuresRight = new GameObject[16];
	bool[] wasChecked = new bool[16];

	//références à d'autres scripts
	EditorManagement manager;
	public wallFeaturesStorage storage;

	/// <summary>
	/// Initialise les paramètres et établit les références.
	/// </summary>
	void Awake()
	{
		cellFeature = new Feature[2];
		carpetFeature = new Feature[2];
		manager = GameObject.Find("EditorManagement").GetComponent<EditorManagement>();
		elevation = Metrics.wallsElevation;
		boxes = new BoxCollider[3];
		boxes[0] = GetComponent<BoxCollider>();
		boxes[0].enabled = false;
		raised = false;
	}

	/// <summary>
	/// Ajoute au tableau "cellFeature" une référence à "feature".
	/// Appelée seulement si le mur n'existe pas.
	/// Un mur ne peut être traversé que par deux meubles différents au plus.
	/// </summary>
	public void AddCellFeature(Feature feature)
	{
		if (cellFeature[0] == null)
		{
			cellFeature[0] = feature;
		}
		else
		{
			cellFeature[1] = feature;
		}
	}

	/// <summary>
	/// Ajoute au tableau "carpetFeature" une référence à "feature".
	/// Appelée seulement si le mur n'existe pas.
	/// Un mur ne peut être traversé que par deux tapis différents au plus.
	/// </summary>
	public void AddCarpetFeature(Feature feature)
	{
		if (carpetFeature[0] == null)
		{
			carpetFeature[0] = feature;
		}
		else
		{
			carpetFeature[1] = feature;
		}
	}

	/// <summary>
	/// Renvoie ou modifie l'entier "window".
	/// Sa valeur est égale à la référence du prefab dans la liste "WindowFactory".
	/// Sa valeur est négative égale à -1 si le mur n'a pas de fenêtre.
	/// </summary>
	public int Window
	{
		get
		{
			return window;
		}
		set
		{
			window = value;
		}
	}

	/// <summary>
	/// Renvoie ou modifie l'objet fenêtre attaché au mur.
	/// Un mur ne peut avoir fenêtre et porte en même temps.
	/// </summary>
	public GameObject o_Window
	{
		get
		{
			return o_window;
		}
		set
		{
			if (o_Window != null)
			{
				Destroy(o_window);
			}
			if (value != null && !HasIncomingOpening)
			{
				o_window = value;
				door = -1;
				o_Door = null;
			}
			else
			{
				o_window = null;
				Window = -1;
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie le booleen "hasIncomingOpening"
	/// Le booleen vaut true si le mur à une fenêtre ou une porte traversant 2 murs et si cette ouverture ne se base pas
	/// sur ce mur.
	/// Le mur n'a pas d'objet fenêtre ou porte attaché à lui dans ce cas.
	/// </summary>
	public bool HasIncomingOpening
	{
		get
		{
			return hasIncomingOpening;
		}
		set
		{
			if (hasIncomingOpening != value)
			{
				hasIncomingOpening = value;
				if (hasIncomingOpening)
				{
					o_Door = o_Window = null;
					HasOutgoingOpening = false;
				}
				else
				{
					HasIncomingDoor = false;
					HasIncomingWindow = false;
				}
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie le booleen "hasIncomingDoor"
	/// Le booleen vaut true si le mur à une porte traversant 2 murs et si cette ouverture ne se base pas
	/// sur ce mur.
	/// Le mur n'a pas d'objet porte attaché à lui dans ce cas.
	/// </summary>
	public bool HasIncomingDoor
	{
		get
		{
			return hasIncomingDoor;
		}
		set
		{
			hasIncomingDoor = value;
			if (HasIncomingDoor)
			{
				HasIncomingOpening = true;
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie le booleen "hasIncomingWindow"
	/// Le booleen vaut true si le mur à une fenêtre traversant 2 murs et si cette ouverture ne se base pas
	/// sur ce mur.
	/// Le mur n'a pas d'objet fenêtre attaché à lui dans ce cas.
	/// </summary>
	public bool HasIncomingWindow
	{
		get
		{
			return hasIncomingWindow;
		}
		set
		{
			hasIncomingWindow = value;
			if (HasIncomingWindow)
			{
				HasIncomingOpening = true;
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie le booleen "hasIncomingOpening"
	/// Le booleen vaut true si le mur à une fenêtre ou une porte traversant 2 murs et si cette ouverture se base sur ce
	/// mur.
	/// Le mur à un objet fenêtre ou porte attaché à lui dans ce cas.
	/// </summary>
	public bool HasOutgoingOpening
	{
		get
		{
			return hasOutgoingOpening;
		}
		set
		{
			hasOutgoingOpening = value;
		}
	}

	/// <summary>
	/// Renvoie ou modifie l'entier "door".
	/// Sa valeur est égale à la référence du prefab dans la liste "DoorFactory".
	/// Sa valeur est négative égale à -1 si le mur n'a pas de porte.
	/// </summary>$
	public int Door
	{
		get
		{
			return door;
		}
		set
		{
			door = value;
		}
	}

	/// <summary>
	/// Renvoie ou modifie l'objet porte attaché au mur.
	/// Un mur ne peut avoir fenêtre et porte en même temps.
	/// </summary>
	public GameObject o_Door
	{
		get
		{
			return o_door;
		}
		set
		{
			if (o_door != null)
			{
				Destroy(o_door);
			}
			if (value != null)
			{
				o_door = value;
				window = -1;
				o_Window = null;
			}
			else
			{
				o_door = null;
				Door = -1;
				ResetCollider();
			}
		}
	}

	/// <summary>
	/// Renvoie la position absolue du mur.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return transform.position;
		}
	}

	/// <summary>
	/// Renvoie ou modifie l'elevation du mur.
	/// Rafraîchit le chunk si la valeur change.
	/// </summary>
	public float Elevation
	{
		get
		{
			return elevation;
		}
		set
		{
			if (Mathf.Abs(elevation - value) > 0.0001)
			{
				if (!hasIncomingOpening)
				{
					elevation = value;
					if (Mathf.Abs(1f - value) > 0.0001)
					{
						hideOpening = false;
						if (hasOutgoingOpening)
						{
							neighbor.GetWall(direction).Elevation = value;
						}
					}
					else
					{
						hideOpening = true;
						if (hasOutgoingOpening)
						{
							neighbor.GetWall(direction).Elevation = value;
						}
					}
					Vector3 scale = transform.localScale;
					scale.y = elevation * 10f;
					transform.localScale = scale;
					Refresh();
				}
				else
				{
					elevation = dot.GetWall(direction.Opposite()).Elevation;
					if (Mathf.Abs(1f - elevation) > 0.0001)
					{
						hideOpening = false;
					}
					else
					{
						hideOpening = true;
					}
					Vector3 scale = transform.localScale;
					scale.y = elevation * 10f;
					transform.localScale = scale;
					Refresh();
				}
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie la couleur du côté gauche du mur.
	/// Rafraîchit le chunk si la valeur change.
	/// </summary>
	public Color ColorLeft
	{
		get
		{
			return colorLeft;
		}
		set
		{
			if (colorLeft == value)
			{
				return;
			}
			colorLeft = value;
			Refresh();
		}
	}

	/// <summary>
	/// Renvoie ou modifie la couleur du côté droit du mur.
	/// Rafraîchit le chunk si la valeur change.
	/// </summary>
	/// <value>The color right.</value>
	public Color ColorRight
	{
		get
		{
			return colorRight;
		}
		set
		{
			if (colorRight == value)
			{
				return;
			}
			colorRight = value;
			Refresh();
		}
	}

	/// <summary>
	/// Cache le mur.
	/// </summary>
	public void Hide()
	{
		Elevation = 1f;
	}

	/// <summary>
	/// Affiche le mur.
	/// </summary>
	public void Show()
	{
		Elevation = 7f;
	}

	/// <summary>
	/// Rafraîchit le chunk et le chunk voisin si le mur se situe entre deux chunks.
	/// </summary>
	void Refresh()
	{
		if (chunk)
		{
			chunk.Refresh();
			if (neighbor.chunk != chunk)
			{
				neighbor.chunk.Refresh();
			}
		}
	}


	/// <summary>
	/// Renvoie ou modifie la valeur du booleen boxActive.
	/// </summary>
	public bool BoxActive
	{
		get
		{
			return boxActive;
		}
		set
		{
			if (boxActive != value && raised)
			{
				boxActive = value;
				{
					if (boxes[0] != null)
					{
						boxes[0].enabled = boxActive;
					}
				}
			}
			else if (!raised)
			{
				boxActive = false;
				if (boxes[0] != null)
				{
					boxes[0].enabled = boxActive;
				}
			}
		}
	}

	/// <summary>
	/// Ajoute des colliders aux murs si on lui ajoute une porte.
	/// Un collider est ajouté de chaque côté de la porte pour mieux prendre en compte la nouvelle forme du mur.
	/// Le collider instantié avec le mur existe toujours.
	/// </summary>
	public void AddCollider(Vector3 size, Vector3 center)
	{
		if (boxes[1] == null)
		{
			boxes[1] = gameObject.AddComponent<BoxCollider>();
		}
		BoxCollider box1 = boxes[1];
		box1.size = size;
		box1.center = new Vector3(center.x,center.y,-center.z);
		box1.enabled = false;

		if (boxes[2] == null)
		{
			boxes[2] = gameObject.AddComponent<BoxCollider>();
		}
		BoxCollider box2 = boxes[2];
		box2.size = size;
		box2.center = center;
		box2.enabled = false;
	}

	/// <summary>
	/// Détruit les colliders ajoutés aux murs si on détruit l'objet porte qui lui était attaché.
	/// </summary>
	public void ResetCollider()
	{
		Destroy(boxes[1]);
		Destroy(boxes[2]);
	}

	/// <summary>
	/// Échange les colliders actifs si le mur à une porte.
	/// Si le mode 1ere personne est actif, les colliders actifs sont ceux sur les côtés de la porte.
	/// Sinon, le collider actif est le collider instantié avec le mur.
	/// </summary>
	public void SwitchColliders(bool playerActive)
	{
		if (boxes[1])
		{
			boxes[1].enabled = playerActive;
			boxes[2].enabled = playerActive;
			boxes[0].enabled = !playerActive;
			if (door >= 0)
			{
				o_door.GetComponent<Door>().BoxActive(playerActive);
			}
		}
	}

	/// <summary>
	/// Renvoie ou modifie la valeur du booleen "raised".
	/// True si le mur a été construit (s'il est visible), false sinon.
	/// Désactive le collider du mur s'il n'existe pas.
	/// </summary>
	public bool Raised
	{
		get
		{
			return raised;
		}
		set
		{
			if (raised != value)
			{
				raised = value;
				if (!raised)
				{
					BoxActive = false;
				}
			}
		}
	}

	/// <summary>
	/// Renvoie true si le mur peut accueillir l'objet fenêtre donné.
	/// False sinon.
	/// </summary>
	public bool CheckWindowAvaibility(int applyWindow, GameObject windowPrefab)
	{
		bool available = true;
		WindowType type = windowPrefab.GetComponent<Window>().windowType;

		if (direction == CompassDirection.NE)
		{
			Dot northDot = dot.GetNeighbor(CompassDirection.N);
			if (northDot.isWalled(CompassDirection.SE))
			{
				available = false;
			}

		}
		else if (direction == CompassDirection.SE)
		{
			Dot southDot = dot.GetNeighbor(CompassDirection.S);
			if (southDot.isWalled(CompassDirection.NE))
			{
				available = false;
			}
		}

		if (available)
		{
			if (type == WindowType.through2)
			{
				if (!HasIncomingOpening)
				{
					if ((neighbor.isWalled(direction) || HasOutgoingOpening) 
					    && !neighbor.GetWall(direction).HasOutgoingOpening)
					{
						Wall nextWall = neighbor.GetWall(direction);
						HasOutgoingOpening = true;
						HasIncomingOpening = false;
						o_Door = null;
						Window = applyWindow;
						o_Window = windowPrefab;
						neighbor.HasAWindow = true;
						neighbor.HasADoor = false;
						nextWall.hasIncomingOpening = true;
						nextWall.HasIncomingDoor = false;
						nextWall.HasIncomingWindow = true;
						nextWall.o_Window = null;
						nextWall.o_Door = null;
						dot.Refresh();
					}
					else
					{
						available = false;
					}
				}
				else
				{
					available = false;
				}
			}
			else
			{
				if (HasIncomingOpening)
				{
					dot.GetWall(direction.Opposite()).HasOutgoingOpening = false;
					dot.GetWall(direction.Opposite()).o_Window = null;
					dot.GetWall(direction.Opposite()).o_Door = null;
					HasIncomingOpening = false;
					dot.HasAWindow = false;
					dot.HasADoor = false;
				}
				else if (HasOutgoingOpening)
				{
					neighbor.GetWall(direction).HasIncomingOpening = false;
					neighbor.HasAWindow = false;
					neighbor.HasADoor = false;
					HasOutgoingOpening = false;
				}
				o_Door = null;
				Window = applyWindow;
				o_Window = windowPrefab;
				dot.Refresh();
			}
		}
		return available;
	}

	/// <summary>
	/// Renvoie true si le mur peut accueillir l'objet "porte" donné.
	/// False sinon.
	/// </summary>
	public bool CheckDoorAvaibility(int applyDoor, GameObject doorPrefab)
	{
		bool available = true;
		DoorType type = doorPrefab.GetComponent<Door>().doorType;

		if (direction == CompassDirection.NE)
		{
			Dot northDot = dot.GetNeighbor(CompassDirection.N);
			if (northDot.isWalled(CompassDirection.SE))
			{
				available = false;
			}

		}
		else if (direction == CompassDirection.SE)
		{
			Dot southDot = dot.GetNeighbor(CompassDirection.S);
			if (southDot.isWalled(CompassDirection.NE))
			{
				available = false;
			}
		}

		if (available)
		{
			if (type == DoorType.through2)
			{
				if (!HasIncomingOpening)
				{
					if ((neighbor.isWalled(direction) || HasOutgoingOpening ) 
					    && !neighbor.GetWall(direction).HasOutgoingOpening)
					{
						Wall nextWall = neighbor.GetWall(direction);
						HasOutgoingOpening = true;
						HasIncomingOpening = false;
						o_Window = null;
						Door = applyDoor;
						o_Door = doorPrefab;
						neighbor.HasAWindow = false;
						neighbor.HasADoor = true;
						nextWall.hasIncomingOpening = true;
						nextWall.hasIncomingWindow = false;
						nextWall.hasIncomingDoor = true;
						nextWall.o_Window = null;
						nextWall.o_Door = null;
						dot.Refresh();
					}
					else
					{
						available = false;
					}
				}
				else
				{
					available = false;
				}
			}
			else
			{
				if (HasIncomingOpening)
				{
					dot.GetWall(direction.Opposite()).HasOutgoingOpening = false;
					dot.GetWall(direction.Opposite()).o_Window = null;
					dot.GetWall(direction.Opposite()).o_Door = null;
					HasIncomingOpening = false;
					dot.HasAWindow = false;
					dot.HasADoor = false;
				}
				else if (HasOutgoingOpening)
				{
					neighbor.GetWall(direction).HasIncomingOpening = false;
					neighbor.HasAWindow = false;
					neighbor.HasADoor = false;
					HasOutgoingOpening = false;
				}
				o_Window = null;
				Door = applyDoor;
				o_Door = doorPrefab;
				dot.Refresh();
			}
		}
		return available;
	}

	/// <summary>
	/// Appelée lors de la triangulation pour vérifier que le mur peut accueillir une porte.
	/// </summary>
	public void CheckDoorAvaibility()
	{
		bool available = true;

		if (dot.isWalled(direction.GetPrevious()) || dot.isWalled(direction.GetNext())
			|| neighbor.isWalled(direction.Opposite().GetPrevious()) || neighbor.isWalled(direction.Opposite().GetNext()))
		{
			available = false;
		}
		else
		{
			if (direction == CompassDirection.NE)
			{
				Dot northDot = dot.GetNeighbor(CompassDirection.N);
				if (northDot.isWalled(CompassDirection.SE))
				{
					available = false;
				}

			}
			else if (direction == CompassDirection.SE)
			{
				Dot southDot = dot.GetNeighbor(CompassDirection.S);
				if (southDot.isWalled(CompassDirection.NE))
				{
					available = false;
				}
			}
			if (HasIncomingOpening)
			{
				for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
				{
					if (dot.isWalled(d) && ((d != direction) && d != direction.Opposite()))
					{
						available = false;
					}
				}
			}
			else if (HasOutgoingOpening)
			{
				for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
				{
					if (neighbor.isWalled(d) && ((d != direction) && d != direction.Opposite()))
					{
						available = false;
					}
				}
			}
		}
		if (!available)
		{
			if (HasIncomingDoor)
			{
				dot.GetWall(direction.Opposite()).HasOutgoingOpening = false;
				dot.GetWall(direction.Opposite()).o_Door = null;
				HasIncomingOpening = false;
				dot.HasAWindow = false;
				dot.HasADoor = false;
				dot.Refresh();
			}
			else if (HasOutgoingOpening && Door >= 0)
			{
				o_Door = null;
				neighbor.GetWall(direction).HasIncomingOpening = false;
				HasOutgoingOpening = false;
				neighbor.HasAWindow = false;
				neighbor.HasADoor = false;
				dot.Refresh();
				neighbor.GetWall(direction).Refresh();
			}
		}
	}

	/// <summary>
	/// Appelé lors de la triangulation pour vérifier que le mur peut accueillir une fenêtre.
	/// </summary>
	public void CheckWindowAvaibiliy()
	{
		bool available = true;
		if (direction == CompassDirection.NE)
		{
			Dot northDot = dot.GetNeighbor(CompassDirection.N);
			if (northDot.isWalled(CompassDirection.SE))
			{
				available = false;
			}

		}
		else if (direction == CompassDirection.SE)
		{
			Dot southDot = dot.GetNeighbor(CompassDirection.S);
			if (southDot.isWalled(CompassDirection.NE))
			{
				available = false;
			}
		}
		if (HasIncomingOpening)
		{
			for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
			{
				if (dot.isWalled(d) && ((d != direction) && d != direction.Opposite()))
				{
					available = false;
				}
			}
		}
		else if (HasOutgoingOpening)
		{
			for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
			{
				if (neighbor.isWalled(d) && ((d != direction) && d != direction.Opposite()))
				{
					available = false;
				}
			}
		}
		if (!available)
		{
			
			if (HasIncomingWindow)
			{
				dot.GetWall(direction.Opposite()).HasOutgoingOpening = false;
				dot.GetWall(direction.Opposite()).o_Window = null;
				HasIncomingOpening = false;
				dot.HasAWindow = false;
				dot.HasADoor = false;
				dot.Refresh();
			}
			else if (HasOutgoingOpening && Window >= 0)
			{
				o_Window = null;
				neighbor.GetWall(direction).HasIncomingOpening = false;
				HasOutgoingOpening = false;
				neighbor.HasAWindow = false;
				neighbor.HasADoor = false;
				dot.Refresh();
			}
		}
	}

	/// <summary>
	/// Sauvegarde les données du mur.
	/// </summary>
	public void Save(HomeDataWriter writer)
	{
		writer.Write(colorLeft);
		writer.Write(colorRight);
		if (Door >= 0)
		{
			if (HasOutgoingOpening)
			{
				writer.Write((byte)100);
				writer.Write(Door);
			}
			else
			{
				writer.Write((byte)101);
				writer.Write(Door);
			}
		}
		else if (Window >= 0)
		{
			if (HasOutgoingOpening)
			{
				writer.Write((byte)102);
				writer.Write(Window);
			}
			else
			{
				writer.Write((byte)103);
				writer.Write(Window);
			}
		}
		else if (HasIncomingDoor)
		{
			writer.Write((byte)104);
		}
		else if (HasIncomingWindow)
		{
			writer.Write((byte)105);
		}
		else
		{
			writer.Write((byte)0);
		}
		var toSave = SelectFeaturesToSave(featuresLeft,-1);
		writer.Write(toSave.Count);
		foreach (GameObject item in toSave)
		{
			Feature feature = item.GetComponent<Feature>();
			writer.Write(feature.Id);
			writer.Write(feature.BasePart);
		}
		toSave = SelectFeaturesToSave(featuresRight,1);
		writer.Write(toSave.Count);
		foreach (GameObject item in toSave)
		{
			Feature feature = item.GetComponent<Feature>();
			writer.Write(feature.Id);
			writer.Write(feature.BasePart);
		}
	}

	/// <summary>
	/// Charge les données du mur.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		ColorLeft = reader.ReadColor();
		ColorRight = reader.ReadColor();
		var wallData = (int)reader.ReadByte();
		if (wallData == 100)
		{
			HasOutgoingOpening = true;
			Door = reader.ReadInt();
			CheckDoorAvaibility(Door, manager.InstantiateObject(Door, ObjectType.door));
		}
		else if (wallData == 101)
		{
			Door = reader.ReadInt();
			CheckDoorAvaibility(Door, manager.InstantiateObject(Door, ObjectType.door));
		}
		else if (wallData == 102)
		{
			HasOutgoingOpening = true;
			Window = reader.ReadInt();
			CheckWindowAvaibility(Window, manager.InstantiateObject(Window, ObjectType.window));
		}
		else if (wallData == 103)
		{
			Window = reader.ReadInt();
			CheckWindowAvaibility(Window, manager.InstantiateObject(Window, ObjectType.window));
		}
		else if (wallData == 104)
		{
			HasIncomingDoor = true;
		}
		else if (wallData == 105)
		{
			HasIncomingWindow = true;
		}
		int leftCount = reader.ReadInt();
		for (int i = 0; i < leftCount; i++)
		{
			GameObject item = manager.InstantiateObject(reader.ReadInt(), ObjectType.wallFeature);
			Feature feature = item.GetComponent<Feature>();
			feature.BasePart = reader.ReadInt();
			feature.AddFeatureToWall(this, -1, feature.BasePart);
		}
		int rightCount = reader.ReadInt();
		for (int i = 0; i < rightCount; i++)
		{
			GameObject item = manager.InstantiateObject(reader.ReadInt(), ObjectType.wallFeature);
			Feature feature = item.GetComponent<Feature>();
			feature.BasePart = reader.ReadInt();
			feature.AddFeatureToWall(this, 1, feature.BasePart);
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features attachées au mur.
	/// </summary>
	public bool FeaturesActive
	{
		set
		{
			foreach (GameObject feature in featuresLeft)
			{
				if (feature)
				{
					feature.GetComponent<Feature>().ColliderActive = value;
				}
			}
			foreach (GameObject feature in featuresRight)
			{
				if (feature)
				{
					feature.GetComponent<Feature>().ColliderActive = value;
				}
			}
		}
	}

	/// <summary>
	/// Renvoie les éléments de la liste donnée qui n'ont pas encore été sauvegardés.
	/// </summary>
	List<GameObject> SelectFeaturesToSave(GameObject[] list, int side)
	{
		var toSave = new List<GameObject>();
		for (int i = 0; i < list.Length; i++)
		{
			if (!wasChecked[i] && list[i])
			{
				wasChecked[i] = true;
				Feature feature = list[i].GetComponent<Feature>();
				if (feature.BaseWall == this)
				{
					toSave.Add(list[i]);
					Wall[] featureNeighbors = feature.GetWallNeighbors(this, feature.BasePart, side);
					int[] featureParts = feature.GetWallNeighborsParts(feature.BasePart);
					wasChecked[feature.BasePart] = true;
					for (int u = 0; u < featureParts.Length; u++)
					{
						if (featureNeighbors[u].transform == transform)
						{
							wasChecked[featureParts[u]] = true;
						}
					}
				}
			}
		}
		wasChecked = new bool[16];
		return toSave;
	}

	/// <summary>
	/// Détruit toutes les features du murs.
	/// </summary>
	public void EraseFeatures()
	{
		foreach (GameObject feature in featuresLeft)
		{
			if (feature)
			{
				feature.GetComponent<Feature>().DestroyFeature();
			}
		}
		foreach (GameObject feature in featuresRight)
		{
			if (feature)
			{
				feature.GetComponent<Feature>().DestroyFeature();
			}
		}
	}

	/// <summary>
	/// Détruit les features de références données.
	/// </summary>
	public void EraseFeatures(int[] parts)
	{
		if (!hasIncomingOpening)
		{
			foreach (int i in parts)
			{
				if (featuresRight[i])
				{
					featuresRight[i].GetComponent<Feature>().DestroyFeature();
				}
			}
			foreach (int i in Metrics.OppositeWallParts(parts))
			{
				if (featuresLeft[i])
				{
					featuresLeft[i].GetComponent<Feature>().DestroyFeature();
				}
			}
		}
		else
		{
			foreach (int i in Metrics.OppositeWallParts(parts))
			{
				if (featuresRight[i])
				{
					featuresRight[i].GetComponent<Feature>().DestroyFeature();
				}
			}
			foreach (int i in parts)
			{
				if (featuresLeft[i])
				{
					featuresLeft[i].GetComponent<Feature>().DestroyFeature();
				}
			}
		}
	}

	/// <summary>
	/// Active ou désactive les lumières des features attachées aux murs.
	/// Elles doivent être actives seulement en mode 1ere personne.
	/// </summary>
	public void SwitchLights(bool enabled)
	{
		foreach (GameObject feature in featuresRight)
		{
			if (feature)
			{
				Light[] lights = feature.GetComponentsInChildren<Light>();
				foreach (Light lightItem in lights)
				{
					lightItem.enabled = enabled;
				}
			}
		}
		foreach (GameObject feature in featuresLeft)
		{
			if (feature)
			{
				Light[] lights = feature.GetComponentsInChildren<Light>();
				foreach (Light lightItem in lights)
				{
					lightItem.enabled = enabled;
				}
			}
		}
	}

	//Partie pouvant être améliorée, permet en mode vision d'ensemble de baisser les murs lorsqu'ils sont trop proches
	//de la caméra

	//attente entre deux états du mur
	int waitTime;
	//état du mur
	public bool hidden, increases, decreases;

	/// <summary>
	/// Attends "count" secondes avant de quitter l'état "decreases".
	/// </summary>
	IEnumerator Down(float count)
	{
		yield return new WaitForSeconds(count);
		decreases = false;
	}

	/// <summary>
	/// Attends "count" secondes avant de quitter l'état "increases".
	/// </summary>
	IEnumerator Up(float count)
	{
		yield return new WaitForSeconds(count);
		                            
		increases = false;
	}

	/// <summary>
	/// True si le mur est proche de la caméra, false sinon.
	/// </summary>
	public bool isTriggered;

	/// <summary>
	/// Détermine à chaque frame si le mur doit être baissé ou relevé.
	/// Utilisée en mode vision d'ensemble.
	/// </summary>
	void Update()
	{
		if (isTriggered)
		{
			if (hidden || increases)
			{
				return;
			}
			else
			{
				hidden = true;
				decreases = true;
				Elevation = 1f;
				StartCoroutine(Down(0.25f));
			}
		}
		else
		{
			if (!hidden || decreases)
			{
				return;
			}
			else
			{
				hidden = false;
				increases = true;
				Elevation = 7f;
				StartCoroutine(Up(0.25f));
			}
		}
	}
}
