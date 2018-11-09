using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// Classe principale de chaque étage de la maison.
/// Elle permet de créer les éléments qui constituent l'étage, puis de manipuler ces éléments.
/// </summary>
public class FloorGrid : MonoBehaviour {

	//constantes de construction
	public int cellCountX = 8, cellCountZ = 6;
	int chunkCountX, chunkCountZ;
	int dotsSizeX, dotsSizeZ;
	int defaultColor;

	//prefabs des éléments de la grille
	public Cell cellPrefab;
	public Dot dotPrefab;
	public Wall wallPrefab;
	public GridChunk chunkPrefab;
	public Image cellImagePrefab;

	//éléments de la grille
	Cell[] cells;
	Dot[] dots;
	Wall[] walls;
	GridChunk[] chunks;
	CombineMesh terrainMesh, cellingMesh, wallsMesh;
	public wallFeaturesStorage wallStorage;

	//propriétés de l'étage
	[HideInInspector]
	public float elevation; 
	public int level;
	bool hidden;
	public Canvas canvas;

	//références à d'autres scripts
	[HideInInspector]
	public NavigationBaker baker;

	/// <summary>
	/// Etablit les références.
	/// </summary>
	void Awake()
	{
		elevation = Metrics.wallsElevation;
		baker = GameObject.Find("NavigationBaker").GetComponent<NavigationBaker>();
		terrainMesh = transform.Find("TerrainCombineMesh").GetComponent<CombineMesh>();
		cellingMesh = transform.Find("CellingCombineMesh").GetComponent<CombineMesh>();
		wallsMesh = transform.Find("WallsCombineMesh").GetComponent<CombineMesh>();
		canvas = GetComponentInChildren<Canvas>();
	}

	/// <summary>
	/// Crée les chunks, points, cellules et murs lors de l'activation du script "editor" et de la création d'un nouvel
	/// étage. 
	/// </summary>
	public void Build(int x, int z)
	{
		if (
			x <= 0 || x % Metrics.chunkSizeX != 0 || z <= 0 || z % Metrics.chunkSizeZ != 0)
		{
			Debug.LogError("Unsupported map size.");
			return;
		}
		cellCountX = x;
		cellCountZ = z;
		chunkCountX = cellCountX / Metrics.chunkSizeX;
		chunkCountZ = cellCountZ / Metrics.chunkSizeZ;
		dotsSizeX = Metrics.chunkSizeX * 2 + 1;
		dotsSizeZ = Metrics.chunkSizeZ * 2 + 1;
		dots = new Dot[(cellCountX * 2 + 1) * (cellCountZ * 2 + 1)];
		walls = new Wall[dots.Length * 4];

		if (level > 0)
		{
			defaultColor = 0;
		}
		else
		{
			baker.SetSurface(GetComponent<NavMeshSurface>());
			defaultColor = 2;
		}
		CreateChunks();
		CreateCells();
		CreateWalls();
		AddDotsToCell();
	}

	/// <summary>
	/// Crée les chunks.
	/// Attribue à chaque chunk un tableau de points.
	/// </summary>
	void CreateChunks()
	{
		chunks = new GridChunk[chunkCountX * chunkCountZ];
		dots = new Dot[(cellCountX * 2 + 1) * (cellCountZ * 2 + 1)];
		for (int z = 0, i = 0; z < chunkCountZ; z++)
		{
			for (int x = 0; x < chunkCountX; x++)
			{
				GridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.floor = this;
				chunk.transform.SetParent(transform);
				CreateDots(chunk, x, z);
			}
		}
	}

	/// <summary>
	/// Crée le tableau "cells" et appelle la méthode "CreateCell" pour créer chaque cellule. 
	/// </summary>
	void CreateCells()
	{
		cells = new Cell[cellCountZ * cellCountX];
		for (int z = 0, i = 0; z < cellCountZ; z++)
		{
			for (int x = 0; x < cellCountX; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	/// <summary>
	/// Crée la cellule de coordonnées (x,z). Le paramètre "i" est l'index de la cellule dans le tableau "cells".
	/// </summary>
	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = x * 10f;
		position.y = 0f;
		position.z = z * 10f;

		Cell cell = cells[i] = Instantiate(cellPrefab);
		cell.level = level;
		cell.transform.localPosition = position;
		cell.coordinates = Coordinates.FromOffsetCoordinates(x, z);
		int[] colors = new int[16];
		for (int c = 0; c < colors.Length; c++)
		{
			colors[c] = defaultColor;
		}
		cell.Colors = colors;

		if (x > 0)
		{
			cell.SetNeighbors(SquareDirection.W, cells[i - 1]);
		}
		if (z > 0)
		{
			cell.SetNeighbors(SquareDirection.S, cells[i - cellCountX]);
		}
		Image image = Instantiate(cellImagePrefab);
		image.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		image.rectTransform.sizeDelta = new Vector2(Metrics.innerRadius * 2f, Metrics.innerRadius * 2f);
		cell.uiRectImage = image.rectTransform;
		cell.uiRectImage.SetParent(canvas.transform, false);
		cell.pivot = GetDot(new Coordinates(cell.coordinates.X * 2 + 1, cell.coordinates.Z * 2 + 1));
		AddCellToChunk(x, z, cell);
	}

	/// <summary>
	/// Attribue la cellule de coordonnées(x,z) à son chunk.
	/// </summary>
	void AddCellToChunk(int x, int z, Cell cell)
	{
		int chunkX = x / Metrics.chunkSizeX;
		int chunkZ = z / Metrics.chunkSizeZ;
		GridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * Metrics.chunkSizeX;
		int localZ = z - chunkZ * Metrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * Metrics.chunkSizeX, cell);
	}

	/// <summary>
	/// Retourne la cellule située à une position donnée.
	/// </summary>
	public Cell GetCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		if ((coordinates.X < 0 || coordinates.X >= cellCountX)
		    || (coordinates.Z < 0 || coordinates.Z >= cellCountZ))
		{
			return null;
		}
		int index = coordinates.X + coordinates.Z * cellCountX;
		return cells[index];
	}

	/// <summary>
	/// Retourne la cellule correspondant à des coordonnées données.
	/// </summary>
	public Cell GetCell(Coordinates coordinates)
	{
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ)
		{
			return null;
		}
		int x = coordinates.X;
		if (x < 0 || x >= cellCountX)
		{
			return null;
		}
		return cells[x + z * cellCountX];
	}

	/// <summary>
	/// Crée pour un chunk donnée son tableau "dots" et son tableau "walls".
	/// Appelle la méthode "CreateDot" pour créer les points du chunk.
	/// </summary>
	void CreateDots(GridChunk chunk, int chunkX, int chunkZ)
	{
		int sizeX = dotsSizeX, sizeZ = dotsSizeZ;
		if (chunkX > 0)
		{
			sizeX -= 1;
		}
		if (chunkZ > 0)
		{
			sizeZ -= 1;
		}
		chunk.dots = new Dot[sizeX * sizeZ];
		chunk.Walls = new Wall[sizeX * sizeZ * 4];

		for (int z = (dotsSizeZ - sizeZ), i = 0; z < dotsSizeZ; z++)
		{
			for (int x = (dotsSizeX - sizeX); x < dotsSizeX; x++)
			{
				CreateDot(x, z, i++, chunk, chunkX, chunkZ);
			}
		}
	}

	/// <summary>
	/// Crée le point de coordonnées données correspondant au chunk donné.
	/// </summary>
	void CreateDot(int x, int z, int i, GridChunk chunk, int chunkX, int chunkZ)
	{
		Vector3 position;
		position.x = x * 5f - 5f;
		position.x += chunkX * Metrics.chunkSizeX * Metrics.innerRadius * 2;
		position.y = 0f;
		position.z = z * 5f - 5f;
		position.z += chunkZ * Metrics.chunkSizeZ * Metrics.innerRadius * 2;

		Dot dot = Instantiate(dotPrefab);
		dot.transform.localPosition = position;
		dot.transform.localScale = new Vector3(Metrics.dotRadius*2, 0.1f, Metrics.dotRadius * 2);
		dot.coordinates = Coordinates.FromOffsetCoordinates(chunkX * (dotsSizeX- 1) + x, chunkZ* (dotsSizeZ-1) +z);

		int index = dot.coordinates.Z * (cellCountX * 2 + 1) + dot.coordinates.X;
		dots[index] = dot;
		int iX = dot.coordinates.X, iZ = dot.coordinates.Z;
		if (iX > 0)
		{
			dot.SetNeighbors(CompassDirection.W, dots[index - 1]);
			if (iZ > 0)
			{
				dot.SetNeighbors(CompassDirection.SW, dots[index - (cellCountX * 2 + 1) - 1]);
			}
			if (x == 1 && chunkX > 0 && z < (dotsSizeZ - 1))
			{
				dot.SetNeighbors(CompassDirection.NW, dots[index + (cellCountX * 2 + 1) - 1]);
			}
			else if (chunkZ > 0 && x == (dotsSizeX - 1) && z == 1)
			{
				dot.SetNeighbors(CompassDirection.SE, dots[index - (cellCountX * 2 + 1) + 1]);
			}
		}
		if (iZ > 0)
		{
			dot.SetNeighbors(CompassDirection.S, dots[index - (cellCountX * 2 + 1)]);
			if (iX < (cellCountX * 2) && x != 4)
			{
				dot.SetNeighbors(CompassDirection.SE, dots[index - (cellCountX * 2 + 1) + 1]);
			}
		}
		chunk.AddDot(i, dot);
	}

	/// <summary>
	/// Retourne le point situé à une position donnée.
	/// </summary>
	public Dot GetDot(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromDotPosition(position);

		int index = coordinates.Z * (cellCountX * 2 + 1) + coordinates.X;
		return dots[index];
	}

	/// <summary>
	/// Retourne le point correspondant à des coordonnées données.
	/// </summary>
	public Dot GetDot(Coordinates coordinates)
	{
		int z = coordinates.Z;
		if (z < 0 || z >= (cellCountZ*2 + 1))
		{
			return null;
		}
		int x = coordinates.X;
		if (x < 0 || x >= (cellCountX*2 + 1))
		{
			return null;
		}
		return dots[x + z * (cellCountX * 2 + 1)];
	}

	/// <summary>
	/// Créer pour chaque point quatre murs. Les quatre murs restants sont créés avec les voisins de ce point.
	/// </summary>
	void CreateWalls()
	{
		for (int i = 0; i < dots.Length; i++)
		{
			for (CompassDirection d = CompassDirection.N; d <= CompassDirection.SE; d++)
			{
				CreateWall(i, d);
			}
		}
	}

	/// <summary>
	/// Crée un mur selon une direction donnée, et l'attribue au point du tableau "dots" d'indice "i".
	/// Le mur n'est créé que s'il existe un voisin dans la direction donnée.
	/// </summary>
	void CreateWall(int i, CompassDirection direction)
	{
		Dot dot = dots[i];
		Dot neighbor = dot.GetNeighbor(direction);
		if (neighbor)
		{
			Vector3 position = Vector3.Lerp(dot.Position, neighbor.Position, 0.5f);
			Wall wall = walls[(int)direction + i*4] = Instantiate(wallPrefab);
			wall.transform.position = position;
			wall.ColorLeft = wall.ColorRight = Color.white;
			wall.direction = direction;
			//Ajuste la direction selon le vecteur entre le point et son voisin.
			Vector3 alignTo = neighbor.Position - dot.Position;
			wall.transform.rotation = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), alignTo);
			float dist = Vector3.Distance(dot.Position, neighbor.Position);
			wall.transform.localScale = new Vector3(0.1f, wall.Elevation, dist);
			dot.AddWall(direction, wall,level);
			wall.chunk = dot.chunk;
			wall.storage = wall.chunk.floor.wallStorage;
		}
	}

	/// <summary>
	/// Ajoute à chaque cellule les points qui sont placés sur elle.
	/// </summary>
	void AddDotsToCell()
	{
		for (int i = 0; i < cells.Length; i++)
		{
			Cell cell = cells[i];
			cell.AddDot(cell.pivot, 8);
			for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
			{
				Dot neighbor = cell.pivot.GetNeighbor(d);
				if (d != CompassDirection.NW)
				{
					cell.AddDot(neighbor, (int)d + 1);
				}
				else
				{
					cell.AddDot(neighbor, 0);
				}
			}
			cell.AddWall();
		}
	}

	/// <summary>
	/// Cache les murs de la grille.
	/// </summary>
	public void HideWalls()
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i])
			{
				walls[i].Hide();
			}
		}
	}

	/// <summary>
	/// Affiche les murs de la grille.
	/// </summary>
	public void ShowWalls()
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i])
			{
				walls[i].Show();
			}
		}
		ForcedRefresh();
	}

	/// <summary>
	/// Active ou désactive les colliders des points de la grille.
	/// </summary>
	bool wallsCollide;
	public bool WallsCollide
	{
		get
		{
			return wallsCollide;
		}
		set
		{
			if (wallsCollide != value)
			{
				wallsCollide = value;
				foreach (Dot dot in dots)
				{
					dot.BoxWallActive = wallsCollide;
				}
			}
		}
	}

	/// <summary>
	/// Change les colliders des points de la grille selon le mode activé.
	/// </summary>
	public void SwitchColliders(bool playerActive)
	{
		foreach (Dot dot in dots)
		{
			dot.SwitchColliders(playerActive);
		}
	}

	/// <summary>
	/// Désactive la grille.
	/// Méthode appelée en modes vue d'ensemble et Edition pour cacher les étages supérieurs. 
	/// </summary>
	public void InteractableUpper(bool interactable)
	{
		gameObject.SetActive(interactable);
	}

	/// <summary>
	/// Active ou désactive les interactions avec la grille.
	/// </summary>
	public void InteractableUnder(bool interactable)
	{
		foreach (Dot dot in dots)
		{
			dot.BoxActive = interactable;
			dot.BoxWallActive = false;
			wallsCollide = false;
		}
		EnableCellsCollider(false);
	}

	/// <summary>
	/// Active ou désactive les colliders de cellules.
	/// </summary>
	public void EnableCellsCollider(bool enabled)
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.EnableCellsCollider(enabled);
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features.
	/// </summary>
	public void EnableFeaturesCollider(bool enabled)
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.EnableFeaturesCollider(enabled);
		}
		foreach (Wall wall in walls)
		{
			if (wall)
			{
				wall.FeaturesActive = enabled;
			}
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features et placeableObjects.
	/// </summary>
	public void EnableObjectsCollider(bool enabled)
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.EnableObjectsCollider(enabled);
		}
	}

	/// <summary>
	/// Active ou désactive le meshCollider du mesh Floor.
	/// Ce meshCollider n'est actif qu'en mode 1erePersonne pour permettre à l'utilisateur de se déplacer en cliquant
	/// sur le sol.
	/// </summary>
	public void EnableFloorCollider(bool enabled)
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.EnableFloorCollider(enabled);
		}
	}

	/// <summary>
	/// Cache ou affiche le canvas de la grille, comprenant les images des cellules et des points.
	/// </summary>
	public void HideCanvas(bool hide)
	{
		if (hidden != hide)
		{
			hidden = hide;
			canvas.gameObject.SetActive(!hide);
		}
	}

	/// <summary>
	/// Active ou désactive les lumières des features dans la scène.
	/// Ces lumières sont actives lors de l'utilisation de la 1ere personne et inactives sinon.
	/// </summary>
	public void SwitchLights(bool enabled)
	{
		foreach (Cell cell in cells)
		{
			cell.SwitchLights(enabled);
		}
		foreach (Wall wall in walls)
		{
			if (wall)
			{
				wall.SwitchLights(enabled);
			}
		}
	}

	/// <summary>
	/// Provoque la triangulation de la grille.
	/// </summary>
	public void Refresh()
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.Refresh();
		}
	}

	public void ForcedRefresh()
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.ForcedRefresh();
		}
	}

	/// <summary>
	/// Provoque la triangulation des cellules seulement.
	/// </summary>
	public void RefreshCellsOnly()
	{
		foreach (GridChunk chunk in chunks)
		{
			chunk.RefreshCellsOnly();
		}
	}

	/// <summary>
	/// Sauvegarde les points et les cellules.
	/// </summary>
	public void Save(HomeDataWriter writer)
	{
		for (int i = 0; i < dots.Length; i++)
		{
			dots[i].Save(writer);
		}
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].Save(writer);
		}
	}

	/// <summary>
	/// Charge les points et les cellules et provoque un rafraichissement des chunks.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		wallStorage.Load(reader);
		for (int i = 0; i < dots.Length; i++)
		{
			dots[i].Load(reader);
		}
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].Load(reader);
		}
		ForcedRefresh();
	}

	/// <summary>
	/// Combine les meshs de la grille.
	/// </summary>
	public void CombineMesh()
	{
		terrainMesh.Combine(chunks, "terrain");
		cellingMesh.Combine(chunks, "celling");
		wallsMesh.Combine(chunks, "walls");
	}

	/// <summary>
	/// Désunit les meshs de la grille pour les réattribuer aux chunks.
	/// </summary>
	public void DesuniteMesh()
	{
		terrainMesh.Desunite(chunks, "terrain");
		cellingMesh.Desunite(chunks, "celling");
		wallsMesh.Desunite(chunks, "walls");
	}
}
