using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Elément de la grille, permettant de représenter le sol. Chaque chunk est divisé en un certain nombre de cellules,
/// donné par les dimensions du chunk dans "FloorGrid".
/// Diviser en cellules le sol permet de gérer plus efficacement l'aplat des textures et le positionnement des objets.
/// Chaque tableau de features est de taille 16, chaque 16eme de la cellule pouvant contenir une fois chaque type de
/// feature.
/// </summary>
public class Cell : MonoBehaviour {

	//propriétés et composants de la cellule
	public Coordinates coordinates;
	public Dot pivot;
	BoxCollider box;
	public GridChunk chunk;
	public RectTransform uiRectImage;
	public int level;

	//éléments de la cellule
	[SerializeField]
	Dot[] dots = new Dot[9];
	[SerializeField]
	Wall[] walls = new Wall[20];

	//voisins de la cellule
	[SerializeField]
	Cell[] neighbors;

	//features
	[HideInInspector]
	public GameObject[] features = new GameObject[16];
	[HideInInspector]
	public GameObject[] cellingFeatures = new GameObject[16];
	[HideInInspector]
	public GameObject[] floorFeatures = new GameObject[16];
	bool[] wasChecked = new bool[16];

	//réfrence à un autre script
	EditorManagement manager;

	/// <summary>
	/// Etablit la référence à "manganer" et au collider de la cellule.
	/// </summary>
	void Awake()
	{
		manager = GameObject.Find("EditorManagement").GetComponent<EditorManagement>();
		box = GetComponent<BoxCollider>();
	}

	/// <summary>
	/// Tableau d'entiers de taille 16, chaque entier correspond à une couleur, elle-même associé à un type de texture.
	/// "Colors" est utilisé pour la triangulation ou lors de l'édition pour changer les textures de la cellule.
	/// </summary>
	int[] colors;
	public int[] Colors
	{
		get
		{
			return colors;
		}
		set
		{
			if (colors == value)
			{
				return;
			}
			colors = value;
			Refresh();
		}
	}

	/// <summary>
	/// Réinitialise les couleurs de références données.
	/// </summary>
	public void EraseColors(int[] erasedColors)
	{
		int[] newColors = Colors;
		foreach (int i in erasedColors)
		{
			if (i < 16)
			{
				newColors[i] = 0;
			}
		}
		Colors = newColors;
	}

	/// <summary>
	/// Renvoie le voisin dans une direction donnée.
	/// </summary>
	public Cell GetNeighbor(SquareDirection direction)
	{
		return neighbors[(int)direction];
	}

	/// <summary>
	/// Renvoie la cellule se situant au-dessus de la cellule.
	/// </summary>
	public Cell GetUpNeighbor()
	{
		HomeEditor editor = chunk.editor;
		if (level < (Metrics.houseLevels - 1) && editor.floors[level + 1])
		{
			return editor.floors[level + 1].GetCell(coordinates);
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Attribue un voisin dans un direction donnée.
	/// </summary>
	public void SetNeighbors(SquareDirection direction, Cell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	/// <summary>
	/// Renvoie la position du centre de la cellule.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return transform.localPosition;
		}
	}

	/// <summary>
	/// Rafraichit le chunk contenant la cellule, et ses voisins si la cellule est en frontière d'un chunk.
	/// </summary>
	void Refresh()
	{
		if (chunk)
		{
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++)
			{
				Cell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk)
				{
					neighbor.chunk.Refresh();
				}
			}
		}
	}

	/// <summary>
	/// Active ou désactive le collider de la cellule, le collider doit être actif seulement si le mode colorFloor est
	/// actif ou si le mode destruction est enclenché.
	/// </summary>
	bool boxActive = true;
	public bool BoxActive
	{
		get
		{
			return boxActive;
		}
		set
		{
			if (boxActive != value)
			{
				boxActive = value;
				box.enabled = value;
			}
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features.
	/// </summary>
	public bool FeaturesActive
	{
		set
		{
			foreach (GameObject feature in features)
			{
				if (feature)
				{
					feature.GetComponent<Feature>().ColliderActive = value;
				}
			}
			foreach (GameObject feature in cellingFeatures)
			{
				if (feature)
				{
					feature.GetComponent<Feature>().ColliderActive = value;
				}
			}
			foreach (GameObject feature in floorFeatures)
			{
				if (feature)
				{
					feature.GetComponent<Feature>().ColliderActive = value;
				}
			}
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features et placeableObjects, les colliders doivent être actifs seulement 
	/// si le mode vue 1erePersonne est actif afin de pouvoir interagir avec les objets.
	/// </summary>
	public bool ObjectsColliderActive
	{
		set
		{
			foreach (GameObject feature in features)
			{
				if (feature)
				{
					AvailableSurface[] surfaces = feature.GetComponentsInChildren<AvailableSurface>();
					foreach (AvailableSurface surface in surfaces)
					{
						PlaceableObject[] objects = surface.GetComponentsInChildren<PlaceableObject>();
						foreach (PlaceableObject o in objects)
						{
							o.ColliderActive = value;
						}
					}
				}
			}
			foreach (GameObject feature in cellingFeatures)
			{
				if (feature)
				{
					AvailableSurface[] surfaces = feature.GetComponentsInChildren<AvailableSurface>();
					foreach (AvailableSurface surface in surfaces)
					{
						PlaceableObject[] objects = surface.GetComponentsInChildren<PlaceableObject>();
						foreach (PlaceableObject o in objects)
						{
							o.ColliderActive = value;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Sauvegarde les données de la cellule.
	/// </summary>
	public void Save(HomeDataWriter writer)
	{
		for (int i = 0; i < 16; i++)
		{
			writer.Write(colors[i]);
		}
		var toSave = SelectFeaturesToSave(features);
		writer.Write(toSave.Count);
		foreach (GameObject item in toSave)
		{
			Feature feature = item.GetComponent<Feature>();
			writer.Write(feature.Id);
			writer.Write((int)feature.orientation);
			writer.Write(feature.BasePart);
			AvailableSurface[] surfaces = feature.GetComponentsInChildren<AvailableSurface>();
			foreach (AvailableSurface surface in surfaces)
			{
				var objToSave = GetObjectsToSave(surface);
				writer.Write(objToSave.Count);
				foreach (GameObject o in objToSave)
				{
					o.GetComponent<PlaceableObject>().Save(writer);
				}
			}
		}
		toSave = SelectFeaturesToSave(floorFeatures);
		writer.Write(toSave.Count);
		foreach (GameObject item in toSave)
		{
			Feature feature = item.GetComponent<Feature>();
			writer.Write(feature.Id);
			writer.Write((int)feature.orientation);
			writer.Write(feature.BasePart);
		}
		toSave = SelectFeaturesToSave(cellingFeatures);
		writer.Write(toSave.Count);
		foreach (GameObject item in toSave)
		{
			Feature feature = item.GetComponent<Feature>();
			writer.Write(feature.Id);
			writer.Write((int)feature.orientation);
			writer.Write(feature.BasePart);
		}
	}

	/// <summary>
	/// Charge les données de la cellule.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		for (int i = 0; i < 16; i++)
		{
			colors[i] = reader.ReadInt();
		}
		int featuresCount = reader.ReadInt();
		for (int i = 0; i < featuresCount; i++)
		{
			GameObject item = manager.InstantiateObject(reader.ReadInt(), ObjectType.feature);
			Feature feature = item.GetComponent<Feature>();
			feature.orientation = (SquareDirection)reader.ReadInt();
			feature.BasePart = reader.ReadInt();
			feature.AddFeature(this, feature.BasePart);
			AvailableSurface[] surfaces = feature.GetComponentsInChildren<AvailableSurface>();
			foreach (AvailableSurface surface in surfaces)
			{
				int objectsCount = reader.ReadInt();
				for (int u = 0; u < objectsCount; u++)
				{
					GameObject o = manager.InstantiateObject(reader.ReadInt(), ObjectType.placeableObject);
					o.GetComponent<PlaceableObject>().Load(reader);
					o.transform.SetParent(surface.transform, true);
				}
			}
		}
		int floorCount = reader.ReadInt();
		for (int i = 0; i < floorCount; i++)
		{
			GameObject item = manager.InstantiateObject(reader.ReadInt(), ObjectType.feature);
			Feature feature = item.GetComponent<Feature>();
			feature.orientation = (SquareDirection)reader.ReadInt();
			feature.BasePart = reader.ReadInt();
			feature.AddFeature(this, feature.BasePart);
			AvailableSurface[] surfaces = feature.GetComponentsInChildren<AvailableSurface>();
			foreach (AvailableSurface surface in surfaces)
			{
				int objectsCount = reader.ReadInt();
				for (int u = 0; u < objectsCount; u++)
				{
					GameObject o = manager.InstantiateObject(reader.ReadInt(), ObjectType.placeableObject);
					o.GetComponent<PlaceableObject>().Load(reader);
					o.transform.SetParent(surface.transform, true);
				}
			}
		}
		int cellingCount = reader.ReadInt();
		for (int i = 0; i < cellingCount; i++)
		{
			GameObject item = manager.InstantiateObject(reader.ReadInt(), ObjectType.feature);
			Feature feature = item.GetComponent<Feature>();
			feature.orientation = (SquareDirection)reader.ReadInt();
			feature.BasePart = reader.ReadInt();
			feature.AddFeature(this, feature.BasePart);
		}
	}

	/// <summary>
	/// Renvoie dans la liste donnée les objets qui ne sont pas encore sauvegardés.
	/// </summary>
	List<GameObject> SelectFeaturesToSave(GameObject[] list)
	{
		var toSave = new List<GameObject>();
		for (int i = 0; i < list.Length; i++)
		{
			if (!wasChecked[i] && list[i])
			{
				wasChecked[i] = true;
				Feature feature = list[i].GetComponent<Feature>();
				if (feature.BaseCell == this)
				{
					Cell[] featureNeighbors = feature.GetNeighbors(feature.BaseCell, feature.BasePart);
					int[] featureNeighborsParts = feature.GetNeighborsParts(feature.BasePart);
					for (int n = 0; n < featureNeighbors.Length; n++)
					{
						featureNeighbors[n].wasChecked[featureNeighborsParts[n]] = true;
					}
					toSave.Add(list[i]);
				}
			}
		}
		wasChecked = new bool[16];
		return toSave;
	}

	/// <summary>
	/// Active ou désactive les lumières des features de la cellule.
	/// </summary>
	public void SwitchLights(bool enabled)
	{
		foreach (GameObject feature in features)
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
		foreach (GameObject feature in cellingFeatures)
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

	/// <summary>
	/// Renvoie les objets placés sur la surface "surface" pour les sauvegarder.
	/// </summary>
	List<GameObject> GetObjectsToSave(AvailableSurface surface)
	{
		var toSave = new List<GameObject>();
		int count = surface.transform.childCount;
		for (int i = 0; i < count; i++)
		{
			toSave.Add(surface.transform.GetChild(i).gameObject);
		}
		return toSave;
	}

	/// <summary>
	/// Ajoute un point à la cellule
	/// </summary>
	public void AddDot(Dot dot, int index)
	{
		dots[index] = dot;
	}

	/// <summary>
	/// Ajoute un mur à la cellule.
	/// </summary>
	public void AddWall()
	{
		for (int i = 0; i < dots.Length; i++)
		{
			Dot dot = dots[i];
			switch (i)
			{
				case 0:
				case 1:
					AddWall(dot.GetWall(CompassDirection.E));
					AddWall(dot.GetWall(CompassDirection.SE));
					break;
				case 2:
					break;
				case 3:
				case 4:
					AddWall(dot.GetWall(CompassDirection.N));
					break;
				case 5:
				case 6:
					AddWall(dot.GetWall(CompassDirection.N));
					AddWall(dot.GetWall(CompassDirection.NE));
					AddWall(dot.GetWall(CompassDirection.E));
					break;
				default:
					AddWall(dot.GetWall(CompassDirection.N));
					AddWall(dot.GetWall(CompassDirection.NE));
					AddWall(dot.GetWall(CompassDirection.E));
					AddWall(dot.GetWall(CompassDirection.SE));
					break;
			}
		}
	}

	//Incrémenté à chaque ajout de mur pour avoir la référence du prochain mur à ajouter dans le tableau "walls"
	int wallIndex;
	/// <summary>
	/// Ajoute le mur "wall" à la cellule.
	/// </summary>
	public void AddWall(Wall wall)
	{
		walls[wallIndex] = wall;
		wallIndex += 1;
	}

	/// <summary>
	/// Renvoie le mur d'indice "i".
	/// </summary>
	public Wall GetWall(int i)
	{
		return walls[i];
	}
}
