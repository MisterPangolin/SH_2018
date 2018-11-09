using UnityEngine;

//Les différentes tailles de features possibles. Chaque taille correspond à une classe héritant de celle-ci.
public enum Bloc { bloc1x1, bloc2x2, bloc3x3, bloc1x2, bloc1x3, bloc2x3, bloc1x4, bloc2x4}

//Les différents types de features. Pour les features s'appliquant sur les murs, de nouvelles classes sont créées.
public enum FeatureType { furniture, celling, wall, carpet, stairs }

//les différentes catégories dans lesquelles peuvent se classer les items.
public enum Category { lounge, kitchen, bathroom, bedroom, light, health, other, stair}

/// <summary>
/// Classe permettant de diriger l'édition des meubles et des objets placés sur les murs. Ce script ne doit pas être 
/// placé en composant, seules les classes qui héritent de celle-ci doivent l'être.
/// </summary>
public class Feature : PersistableObject {

	//la taille de l'objet si son pivot n'est pas situé à sa base.
	public float height;

	//Décalage de l'objet selon x et z si nécessaire
	public float xOffset, zOffset;

	//type de l'objet
	public FeatureType featureType;
	//la direction vers laquelle est tournée l'objet. On choisit Sud par défaut: l'objet nous fait face.
	//Il faut s'assurer en créant un nouvel asset d'objet qu'il fait bien face au Sud.
	public SquareDirection orientation = SquareDirection.S;

	//paramètres de l'objet
	Collider[] colliders, childColliders;
	MeshRenderer[] renderers, childRenderers;
	int level = -1;
	AvailableSurface[] surfaces;
	cakeslice.Outline[] outlines;

	//Rotation initiale
	[HideInInspector]
	public Vector3 baseRotation;

	//taille du bloc
	[HideInInspector]
	public Bloc blocType;

	//image de l'objet
	public Sprite sprite;

	//Connectivité
	[HideInInspector]
	public bool hasDevice;
	[HideInInspector]
	public Device device;

	//utilisé pour les murs
	[HideInInspector]
	public int side;

	//catégorie de l'objet
	public Category category;

	/// <summary>
	/// Etablit les références aux différents colliders et renderers de l'objet.
	/// </summary>
	protected virtual void Awake()
	{
		childColliders = GetComponentsInChildren<Collider>();
		childRenderers = GetComponentsInChildren<MeshRenderer>();
		colliders = GetComponents<Collider>();
		renderers = GetComponents<MeshRenderer>();
		baseRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		hasDevice |= GetComponent<Device>();
		if (hasDevice)
		{
			device = GetComponent<Device>();
		}
		surfaces = GetComponentsInChildren<AvailableSurface>();
		if (featureType == FeatureType.wall)
		{
			outlines = AddOutline();
		}
	}

	bool colliderActive = true;
	/// <summary>
	/// Active ou désactive les colliders de l'objet ou de ses enfants.
	/// Utilisé seulement en mode 1erePersonne pour pouvoir interagir avec les objets.
	/// </summary>
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
			foreach (Collider c in childColliders)
			{
				c.enabled = value;
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

	int basePart;
	/// <summary>
	/// Renvoie ou modifie le quart de cellule ou de mur servant d'origine à l'objet.
	/// </summary>
	public int BasePart
	{
		get
		{
			return basePart;
		}
		set
		{
			basePart = value;
		}
	}

	Cell baseCell;
	/// <summary>
	/// Renvoie ou modifie la cellule sur laquelle l'objet a son origine.
	/// </summary>
	public Cell BaseCell
	{
		get
		{
			return baseCell;
		}
		set
		{
			baseCell = value;
		}
	}

	Wall baseWall;
	/// <summary>
	/// Renvoie ou modifie le mur sur laquelle l'objet a son origine.
	/// </summary>
	public Wall BaseWall
	{
		get
		{
			return baseWall;
		}
		set
		{
			baseWall = value;
		}
	}

	/// <summary>
	/// Ajoute l'objet au 16eme de cellule donné.
	/// </summary>
	public virtual void AddFeature(Cell cell, int part, int side = 0)
	{
		Cell[] neighbors= GetNeighbors(cell, part);
		int[] neighborsParts = GetNeighborsParts(part);
		CalculatePosition(part);
		CalculateRotation();
		if (featureType == FeatureType.furniture || featureType == FeatureType.stairs)
		{
			for (int i = 0; i < neighbors.Length; i++)
			{
				DestroyFeature(neighbors[i].features[neighborsParts[i]]);
			}
			for (int i = 0; i < neighbors.Length; i++)
			{
				neighbors[i].features[neighborsParts[i]] = gameObject;
			}
		}
		else if (featureType == FeatureType.carpet)
		{
			for (int i = 0; i < neighbors.Length; i++)
			{
				DestroyFeature(neighbors[i].floorFeatures[neighborsParts[i]]);
			}
			for (int i = 0; i < neighbors.Length; i++)
			{
				neighbors[i].floorFeatures[neighborsParts[i]] = gameObject;
			}
		}
		else
		{
			DestroyFeature(cell.cellingFeatures[part]);
			cell.cellingFeatures[part] = gameObject;
			transform.position += Metrics.offset;
		}
		gameObject.transform.SetParent(cell.transform, false);
		ColliderActive = false;
		BaseCell = cell;
		BasePart = part;
		AddReferenceToWall();
		if (hasDevice)
		{
			DeviceStorage.Add(device, cell.level);
			level = cell.level;
		}
	}

	/// <summary>
	/// Ajoute l'objet au 16eme du côté donné du mur donné.
	/// Le numéro de l'étage est donné pour le positionnement.
	/// </summary>
	public virtual void AddFeatureToWall(Wall wall, int side, int part)
	{
		Wall[] neighbors = GetWallNeighbors(wall, part, side);
		int[] neighborsParts = GetWallNeighborsParts(part);
		CalculatePosition(wall.direction, part, side);
		AdjustPosition(wall, side);
		transform.position += new Vector3(wall.Position.x,0f,wall.Position.z);
		
		Destroy((side == 1) ? wall.featuresRight[part] : wall.featuresLeft[part]);
		if (side == 1)
		{
			wall.featuresRight[part] = gameObject;
			if (blocType != Bloc.bloc1x1)
			{
				for (int i = 0; i < neighbors.Length; i++)
				{
					Destroy(neighbors[i].featuresRight[neighborsParts[i]]);
					neighbors[i].featuresRight[neighborsParts[i]] = gameObject;
				}
			}
		}
		else
		{
			wall.featuresLeft[part] = gameObject;
			if (blocType != Bloc.bloc1x1)
			{
				for (int i = 0; i < neighbors.Length; i++)
				{
					Destroy(neighbors[i].featuresLeft[neighborsParts[i]]);
					neighbors[i].featuresLeft[neighborsParts[i]] = gameObject;
				}
			}
		}
		gameObject.transform.SetParent(wall.storage.transform, false);
		ColliderActive = false;
		BaseWall = wall;
		BasePart = part;
		if (hasDevice)
		{
			DeviceStorage.Add(device, wall.level);
			level = wall.level;
		}
		EnableOutline(false);
	}

	/// <summary>
	/// Calcule la position de l'objet s'il doit être placé sur une cellule.
	/// </summary>
	public virtual void CalculatePosition(int part)
	{
	}

	/// <summary>
	/// Calcule la position de l'objet s'il doit être placé sur un mur.
	/// </summary>
	public virtual void CalculatePosition(CompassDirection wallDirection, int part, int side)
	{
	}

	/// <summary>
	/// Ajuste la position de l'objet selon ses particularités.
	/// Les offsets sont à déterminer lorsque l'objet est orienté vers le sud.
	/// </summary>
	public virtual void AdjustPosition(Wall wall = null, int side = 0)
	{
		if (xOffset != 0f || zOffset != 0f)
		{
			switch (orientation)
			{
				case SquareDirection.N:
					transform.position += new Vector3(-xOffset, 0f, -zOffset);
					break;
				case SquareDirection.W:
					transform.position += new Vector3(-xOffset, 0f, zOffset);
					break;
				case SquareDirection.S:
					transform.position += new Vector3(xOffset, 0f, zOffset);
					break;
				default:
					transform.position += new Vector3(xOffset, 0f, -zOffset);
					break;
			}
		}
	}

	/// <summary>
	/// Calcule la rotation de l'objet.
	/// Non utilisé pour les objets se placant sur les murs.
	/// </summary>
	public void CalculateRotation()
	{
		Vector3 rotation = baseRotation;
		switch (orientation)
		{
			case SquareDirection.N:
				rotation += new Vector3(0f, 180f, 0f);
				break;
			case SquareDirection.W:
				rotation += new Vector3(0f, 90f, 0f);
				break;
			case SquareDirection.S:
				rotation += new Vector3(0f, 0f, 0f);
				break;
			default:
				rotation += new Vector3(0f, -90f, 0f);
				break;
		}
		transform.eulerAngles = rotation;
	}

	/// <summary>
	/// Vérifie si l'objet peut se placer sur le 16eme de cellule donnée.
	/// </summary>
	public virtual bool CheckFeatureAvailability(Cell cell, int part)
	{
		bool available = true;
		if (featureType == FeatureType.stairs && cell.level == Metrics.houseLevels - 1)
		{
			Debug.Log("1");
			available = false;
		}
		else
		{
			Cell[] neighbors = GetNeighbors(cell, part);
			foreach (Cell neighbor in neighbors)
			{
				if (neighbor == null)
				{
					available = false;
				}
			}
		}
		if (available)
		{
			available = CheckWallsRaised(GetNeighbors(cell, part), GetNeighborsParts(part));
		}
		return available;
	}

	/// <summary>
	/// Vérifie si l'objet peut se placer sur le 16eme du côté donné du mur donné.
	/// </summary>
	public virtual bool CheckWallFeatureAvailability(Wall wall, int part, int side)
	{
		bool available = true;
		int[] neighborsParts = GetWallNeighborsParts(part);
		if (wall.Door >= 0)
		{
			Door door = wall.o_Door.GetComponent<Door>();
			foreach (int i in door.busyParts)
			{
				if (part == i)
				{
					available = false;
				}
				foreach (int p in neighborsParts)
				{
					if (p == i)
					{
						available = false;
					}
				}
			}
		}
		else if (wall.Window >= 0)
		{
			Window window = wall.o_Window.GetComponent<Window>();
			var busyParts = window.busyParts;
			if (side == -1)
			{
				busyParts = Metrics.OppositeWallParts(busyParts);
			}
			foreach (int i in busyParts)
			{
				if (part == i)
				{
					available = false;
				}
				foreach (int p in neighborsParts)
				{
					if (p == i)
					{
						available = false;
					}
				}
			}
		}
		else if (wall.HasIncomingWindow)
		{
			Window window = wall.dot.GetWall(wall.direction.Opposite()).o_Window.GetComponent<Window>();
			var busyParts = Metrics.OppositeWallParts(window.busyParts);
			if (side == -1)
			{
				busyParts = Metrics.OppositeWallParts(busyParts);
			}
			foreach (int i in busyParts)
			{
				if (part == i)
				{
					available = false;
				}
				foreach (int p in neighborsParts)
				{
					if (p == i)
					{
						available = false;
					}
				}
			}
		}
		else if(wall.HasIncomingDoor)
		{
			Door door = wall.dot.GetWall(wall.direction.Opposite()).o_Door.GetComponent<Door>();
			var busyParts = Metrics.OppositeWallParts(door.busyParts);
			if (side == -1)
			{
				busyParts = Metrics.OppositeWallParts(busyParts);
			}
			foreach (int i in busyParts)
			{
				if (part == i)
				{
					available = false;
				}
				foreach (int p in neighborsParts)
				{
					if (p == i)
					{
						available = false;
					}
				}
			}
		}
		Wall[] neighbors = GetWallNeighbors(wall, part, side);
		foreach (Wall neighbor in neighbors)
		{
			available &= (neighbor != null && neighbor.Raised);
		}
		return available;
	}

	/// <summary>
	/// Détruit l'objet.
	/// </summary>
	public virtual void DestroyFeature()
	{
		if (hasDevice && device.index != -1)
		{
			DeviceStorage.Remove(device, level);
		}
		foreach (AvailableSurface surface in surfaces)
		{
			surface.DestroyObjects();
		}
		Cell[] neighbors = GetNeighbors(baseCell, basePart);
		int[] neighborsParts = GetNeighborsParts(basePart);
		for (int i = 0; i < neighbors.Length; i++)
		{
			if (neighbors[i])
			{
				neighbors[i].features[neighborsParts[i]] = null;
			}
		}
		Destroy(gameObject);
	}

	/// <summary>
	/// Détruit l'objet donné.
	/// Est appelé par le script pour détruit un autre objet.
	/// </summary>
	public virtual void DestroyFeature(GameObject feature)
	{
		if (feature)
		{
			feature.GetComponent<Feature>().DestroyFeature();
		}
	}

	/// <summary>
	/// Renvoie les cellules voisines sur lesquelles l'objet va se placer.
	/// </summary>
	public virtual Cell[] GetNeighbors(Cell cell, int part)
	{
		var neighbors = new Cell[1];
		neighbors[0] = cell;
		return neighbors;
	}

	/// <summary>
	/// Renvoie les autres 16eme de cellules sur lesquels l'objet va se placer.
	/// </summary>
	public virtual int[] GetNeighborsParts(int part)
	{
		var neighborsParts = new int[1];
		neighborsParts[0] = part;
		return neighborsParts;
	}

	/// <summary>
	/// Renvoie les murs voisins sur lesquels l'objet va se placer.
	/// </summary>
	public virtual Wall[] GetWallNeighbors(Wall wall, int part, int side)
	{
		var neighbors = new Wall[1];
		neighbors[0] = wall;
		return neighbors;
	}

	/// <summary>
	/// Renvoie les autres 16eme de murs sur lesquels l'objet va se placer.
	/// </summary>
	public virtual int[] GetWallNeighborsParts(int part)
	{
		var neighborsParts = new int[1];
		neighborsParts[0] = part;
		return neighborsParts;
	}

	/// <summary>
	/// Renvoie l'objet et le détache de son parent s'il se place sur une cellule.
	/// </summary>
	public virtual void GetFeature()
	{
		BaseCell.features[BasePart] = null;

		Cell[] neighbors = GetNeighbors(BaseCell, BasePart);
		int[] neighborsParts =
			GetNeighborsParts(BasePart);
		for (int i = 0; i < neighbors.Length; i++)
		{
			neighbors[i].features[neighborsParts[i]] = null;
		}
		transform.parent = null;
	}

	/// <summary>
	/// Renvoie l'objet et le détache de son parent s'il se place sur un mur.
	/// </summary>
	public virtual void GetFeature(int side)
	{
		if (side == 1)
		{
			BaseWall.featuresRight[BasePart] = null;
		}
		else
		{
			BaseWall.featuresLeft[BasePart] = null;
		}
		transform.parent = null;
	}

	/// <summary>
	/// Vérifie que les murs que va traverser l'objet n'existent pas.
	/// Renvoie true si l'emplacement est disponible, false sinon.
	/// </summary>
	public virtual bool CheckWallsRaised(Cell[] cells, int[] parts)
	{
		bool available = true;
		int i = 0;
		available = !Metrics.CheckWallRaisedBetween(cells[0], parts[0], parts[parts.Length - 1]);
		while (available && i < cells.Length)
		{
			Cell cell = cells[i];
			int part = parts[i];
			available = !Metrics.CheckWallRaised(cell, part);
			if (i > 0 && available)
			{
				available = !Metrics.CheckWallRaisedBetween(cell, part, parts[i - 1]);
			}
			i += 1;
		}
		return available;
	}

	/// <summary>
	/// Ajoute aux murs traversés par l'objet une référence à l'objet, ce qui permettra de détruire efficacement l'objet
	/// si le mur venait à être construit.
	/// </summary>
	public virtual void AddReferenceToWall()
	{
		Cell[] cells = GetNeighbors(baseCell, basePart);
		int[] parts = GetNeighborsParts(basePart);
		Wall wall = Metrics.GetWallBetween(cells[0], parts[0], parts[parts.Length - 1]);
		for (int i = 0; i < cells.Length; i++)
		{
			Cell cell = cells[i];
			int part = parts[i];
			wall = Metrics.GetWall(cell, part);
			if (featureType == FeatureType.furniture)
			{
				wall.AddCellFeature(this);
			}
			else if(featureType == FeatureType.carpet)
			{
				wall.AddCarpetFeature(this);
			}
			if (i > 0)
			{
				wall = Metrics.GetWallBetween(cell, part, parts[i - 1]);
				if (wall)
				{
					if (featureType == FeatureType.furniture)
					{
						wall.AddCellFeature(this);
					}
					else if (featureType == FeatureType.carpet)
					{
						wall.AddCarpetFeature(this);
					}
				}
			}
		}
	}

	/// <summary>
	/// Ajoute des composants "Outline" à l'objet s'il a un mesh renderer et à tous ces enfants.
	/// </summary>
	/// <returns>The outline.</returns>
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

	/// <summary>
	/// Active ou désactive les composants "Outline" référencés dans "outlines".
	/// </summary>
	public void EnableOutline(bool enabled)
	{
		for (int i = 0; i < outlines.Length; i++)
		{
 			outlines[i].enabled = enabled;
		}
	}

	/// <summary>
	/// Définit la couleur des composants "Outline" de l'objet et de ses enfants.
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
