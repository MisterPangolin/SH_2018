using UnityEngine;

/// <summary>
/// Elément de la grille instancié lors de la création d'un étage.
/// Chaque cellule a 9 dots ( ou points), qui sont placés en enfants.
/// Le point sert de base pour les murs, un mur se construisant lorsque l'utilisateur glisse sa souris d'un point à un
/// autre.
/// Chaque point a 4 murs en enfants, selon les directions N, N-E, E, S-E. Le reste des murs étant donnés par les points
/// voisins.
/// </summary>
public class Dot : MonoBehaviour {

	//propriétés et composant du points
	public Coordinates coordinates;
	BoxCollider box;
	public RectTransform uiRect;
	public GridChunk chunk;

	//voisins du point
	[SerializeField]
	Dot[] neighbors;

	//murs
	[SerializeField]
	bool[] wallsActive;
	public Wall wallPrefab;
	[SerializeField]
	Wall[] walls;



	/// <summary>
	/// Initialise le tableau des murs et établit la référence au collider.
	/// </summary>
	void Awake()
	{
		walls = new Wall[8];
		box = GetComponent<BoxCollider>();
	}


	/// <summary>
	/// Retourne le point voisin dans une direction donnée.
	/// </summary>
	public Dot GetNeighbor(CompassDirection direction)
	{
		return neighbors[(int)direction];
	}

	/// <summary>
	/// Attribue ses voisins au point.
	/// Un voisin par direction.
	/// </summary>
	public void SetNeighbors(CompassDirection direction, Dot dot)
	{
		neighbors[(int)direction] = dot;
		dot.neighbors[(int)direction.Opposite()] = this;
	}

	/// <summary>
	/// Renvoie la position locale du point.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return transform.localPosition;
		}
	}

	/// <summary>
	/// Appelé pour trianguler le chunk du point et les chunks de ses voisins s'ils appartiennent à des chunks
	/// différents.
	/// </summary>
	public void Refresh()
	{
		if (chunk)
		{
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++)
			{
				Dot neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk)
				{
					neighbor.chunk.Refresh();
				}
			}
		}
	}

	/// <summary>
	/// Active ou désactive un mur.
	/// </summary>
	public void SetWall(CompassDirection direction, bool walled)
	{
		wallsActive[(int)direction] = walled;
		Dot neighbor = GetNeighbor(direction);
		neighbor.wallsActive[(int)direction.Opposite()] = walled;
		if (walled)
		{
			foreach (Feature feature in walls[(int)direction].cellFeature)
			{
				if (feature)
				{
					feature.DestroyFeature();
				}
			}
			foreach (Feature feature in walls[(int)direction].carpetFeature)
			{
				if (feature)
				{
					feature.DestroyFeature();
				}
			}
		}
		Refresh();
	}

	/// <summary>
	/// Renvoie true si un mur existe dans une direction donnée, false sinon.
	/// </summary>
	public bool isWalled(CompassDirection direction)
	{
		return wallsActive[(int)direction];
	}

	/// <summary>
	/// Renvoie true si le point est traversé par une fenêtre, false sinon.
	/// </summary>
	bool hasAWindow;
	public bool HasAWindow
	{
		get
		{
			return hasAWindow;
		}
		set
		{
			hasAWindow = value;
		}
	}

	bool hasADoor;
	public bool HasADoor
	{
		get
		{
			return hasADoor;
		}
		set
		{
			hasADoor = value;
		}
	}

	/// <summary>
	/// Méthode appelée lors de la création des grilles pour créer le tableau walls.
	/// </summary>
	public void AddWall(CompassDirection direction, Wall wall, int level)
	{
		walls[(int)direction] = wall;
		wall.transform.SetParent(transform, true);
		wall.dot = this;
		wall.neighbor = GetNeighbor(direction);
		wall.neighbor.walls[(int)direction.Opposite()] = wall;
		wall.level = level;
	}

	/// <summary>
	/// Renvoie le mur existant dans une direction donnée.
	/// </summary>
	public Wall GetWall(CompassDirection direction)
	{
		return walls[(int)direction];
	}

	/// <summary>
	/// Active ou désactive les colliders des murs traversant le point.
	/// Les colliders sont actifs en mode 1erePersonne et en mode Edition pour colorier ou détruire les murs, ou leur
	/// ajouter portes et fenêtres.
	/// </summary>
	bool boxWallActive;
	public bool BoxWallActive
	{
		get
		{
			return boxWallActive;
		}
		set
		{
			if (boxWallActive != value)
			{
				boxWallActive = value;
				foreach (Wall wall in walls)
				{
					if (wall)
					{
						wall.BoxActive = boxWallActive;
					}
				}
			}
		}
	}

	/// <summary>
	/// Active ou désactive le collider du point.
	/// Le collider est actif seulement lorsqu'on veut ajouter un mur en mode Edition.
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
	/// Méthode utilisée si le mur possède une porte lorsqu'on utilise le mode 1erePersonne.
	/// Lorsque ce mode est actif, le collider du mur bloquerait la porte et ne permettrait pas le déplacement au
	/// travers. On échange alors ce collider contre deux colliders plus petit prenant la forme du mur de chaque côté
	/// de la porte.
	/// </summary>
	public void SwitchColliders(bool playerActive)
	{
		for (int i = 0; i < 4 ;i++)
		{
			if (wallsActive[i])
			{
				walls[i].SwitchColliders(playerActive);
			}
		}
	}

	/// <summary>
	/// Sauvegarde les données du point.
	/// </summary>
	public void Save(HomeDataWriter writer)
	{
		for (int i = 0; i < 4; i++)
		{
			if (wallsActive[i])
			{
				writer.Write((byte)111);
				walls[i].Save(writer);
			}
			else
			{
				writer.Write((byte)0);
			}
		}
	}

	/// <summary>
	/// Charge les données du point.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		for (int i = 0; i < 4; i++)
		{
			byte wallData = reader.ReadByte();
			if ((int)wallData ==111 )
			{
				wallsActive[i] = true;
				walls[i].Raised = true;
				walls[i].Load(reader);
				CompassDirection direction = walls[i].direction;
				GetNeighbor(direction).wallsActive[(int)direction.Opposite()] = true;
			}
		}
	}
}
