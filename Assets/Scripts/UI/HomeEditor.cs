using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Classe permettant la création et la modification des maisons en instanciant les objets qui la constituent, et en
/// traitant les inputs de l'utilisateur pour appliquer les fonctions d'édition.
/// </summary>
public class HomeEditor : MonoBehaviour
{
	//mode d'édition
	OptionalToggle buildMode;

	//caméra d'édition
	public Camera cameraEditor;

	//suivi des inputs de l'utilisateur
	bool isDrag;
	bool isDropped;
	CompassDirection dragDirection;
	Dot previousDot;
	float t0;
	bool firstClick, leftClick, rightClick;

	//couleurs et texture
	public Color[] colors;
	private Color activeColor;
	int defaultColorGroundFloor = 2;
	int brushSize;
	bool applyColor;
	[HideInInspector]
	public int activeType;

	//références des prefabs pouvant être instanciés
	public EditorManagement manager;

	//placement de nouveaux éléments
	int applyWindow = -1;
	int applyDoor = -1;
	int featureRef;
	int objectRef;
	GameObject featurePrefab, objectPrefab;
	int keepOrientation = 2;

	//indications de couleur pour le placement
	public GameObject[] ObjectHighlightPrefabs;
	GameObject ObjectHighlight;
	[HideInInspector]
	public Color availableColor, unavailableColor;

	//déplacement d'objets
	GameObject movedFeature, movedObject;

	//rotation d'objets
	public GameObject featureRotationPanel, objectRotationPanel;
	GameObject rotatedFeature, rotatedObject;

	//fonctions de l'interface
	public GameObject wallsUpEditor, wallsUpOverview, wallsDownEditor, wallsDownOverview;
	[HideInInspector]
	public bool hidden;

	//grilles
	int cellCountX = 8, cellCountZ = 6;
	[HideInInspector]
	public FloorGrid squareGridPrefab;
	[HideInInspector]
	public FloorGrid currentFloor;
	[HideInInspector]
	public FloorGrid[] floors;

	/// <summary>
	/// Crée le chemin de sauvegarde grâce à "PersistentStorage".
	/// Crée une grille par défaut.
	/// Initialise les paramètres d'édition.
	/// La grille créée ne sera pas forcèment utilisée mais permet d'établir dans d'autres classes des références.
	/// </summary>
	void Awake()
	{
		PersistentStorage.Awake();
		t0 = 0f;
		SelectColor(0);
		floors = new FloorGrid[Metrics.houseLevels];
		floors[0] = Instantiate(squareGridPrefab);
		currentFloor = floors[0];
		floors[0].Build(cellCountX, cellCountZ);
		ObjectHighlight = Instantiate(ObjectHighlightPrefabs[0]);
		ObjectHighlight.SetActive(false);
	}

	/// <summary>
	/// Appelée automatique après "Awake".
	/// Si la maison n'est pas une nouvelle maison, la charge.
	/// Sinon, crée une nouvelle maison selon les dimensions voulues, puis crée une première sauvegarde de cette maison.
	/// </summary>
	void Start()
	{
		if (!PersistentStorage.newHome)
		{
			manager.Load();
		}
		else
		{
			int x = PersistentStorage.X;
			int z = PersistentStorage.Z;
			CreateHome(x, z);
			manager.Save();
		}
		ChangeMode();
	}

	/// <summary>
	/// Appelée lors du chargement de la scène si la maison à éditer/observer est une nouvelle maison.
	/// Détruit l'ancienne grille pour la remplacer par une nouvelle grille de dimension x*z.
	/// Réinitialise les caméras et le mode d'édition.
	/// </summary>
	public void CreateHome(int x, int z)
	{
		foreach (FloorGrid floor in floors)
		{
			if (floor)
			{
				Destroy(floor.gameObject);
			}
		}
		cellCountX = x;
		cellCountZ = z;
		floors = new FloorGrid[Metrics.houseLevels];
		floors[0] = Instantiate(squareGridPrefab);
		currentFloor = floors[0];
		floors[0].Build(x, z);
		MapCamera.ValidatePosition();
		OverviewCamera.ResetCamera();
	}

	/// <summary>
	/// Réinitialise les variables utilisées par les différents modes, puis traite les inputs de l'utilisateur selon le 
	/// mode d'édition selectioné.
	/// </summary>
	void Update()
	{
		if (cameraEditor.enabled)
		{
			if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() 
			    && buildMode != OptionalToggle.Ignore)
			{
				if (buildMode == OptionalToggle.Destruction || buildMode == OptionalToggle.ItemDestruction)
				{
					if (firstClick)
					{
						t0 = Time.time;
					}
					if (((Time.time - t0) > 0.2) || firstClick)
					{
						firstClick = false;
						HandleInput();
					}
				}
				else
				{
					if (buildMode == OptionalToggle.AddFeature || buildMode == OptionalToggle.AddWallFeature
					   || buildMode == OptionalToggle.AddPlaceableObject)
					{
						isDropped = true;
					}
					HandleInput();
				}
			}
			else {
				t0 = 0f;
				previousDot = null;
				firstClick = true;
				leftClick = false;
				if (movedFeature)
				{
					ReleaseFeature();
				}
				if (movedObject)
				{
					int surfaceLayerIndex = LayerMask.NameToLayer("AvailableSurface");
					int layerMask = 1 << surfaceLayerIndex;
					Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					PlaceableObject o = movedObject.GetComponent<PlaceableObject>();
					if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, layerMask))
					{
						AvailableSurface surface = hit.transform.GetComponent<AvailableSurface>();
						if (surface)
						{
							ReleaseObject(o, surface);
							currentFloor.EnableFeaturesCollider(false);
						}
						else
						{
							o.DestroyObject();
						}
					}
					else
					{
						o.DestroyObject();
					}
				}
				if (ObjectHighlight)
				{
					ObjectHighlight.SetActive(false);
				}
				movedFeature = null;
				movedObject = null;
			}
			if (buildMode == OptionalToggle.AddFeature || buildMode == OptionalToggle.AddWallFeature
			    || buildMode == OptionalToggle.AddPlaceableObject)
			{
				HandleInput();
				isDropped = false;
			}
			if (Input.GetMouseButton(1))
			{
				if (!rightClick)
				{
					if (buildMode == OptionalToggle.AddFeature && featurePrefab != null)
					{
						Feature featureProperties = featurePrefab.GetComponent<Feature>();
						RotateFeature((int)featureProperties.orientation.GetNext());
					}
					else if (buildMode == OptionalToggle.MoveItem && movedFeature != null)
					{
						Feature featureProperties = movedFeature.GetComponent<Feature>();
						RotateFeature((int)featureProperties.orientation.GetNext());
					}
					else if (rotatedObject == null && ( objectPrefab != null || movedObject != null))
					{
						RotateObject();
					}
				}
				rightClick = true;
			}
			else
			{
				rightClick = false;
			}
			if ( rotatedObject != null)
			{
				PlaceableObject o = rotatedObject.GetComponent<PlaceableObject>();
				AvailableSurface surface = rotatedObject.GetComponentInParent<AvailableSurface>();
				CheckObjectAvailability(o, surface);
			}
		}
	}

	/// <summary>
	/// Change de mode d'édition lorsque l'utilisateur clique sur un bouton attribué à un mode d'édition.
	/// Réinitialise les paramètres d'édition, active les colliders des différents objets selon le mode choisi.
	/// </summary>
	public void ChangeMode()
	{
		if (cameraEditor.enabled)
		{
			Clear();
			currentFloor.EnableFeaturesCollider(false);
			currentFloor.EnableObjectsCollider(false);
			activeColor = Color.white;
			activeType = 0;
			featureRef = 0;
			switch (buildMode)
			{
				case OptionalToggle.Destruction:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = true;
					currentFloor.EnableCellsCollider(true);
					currentFloor.EnableFeaturesCollider(true);
					currentFloor.EnableObjectsCollider(true);
					break;
				case OptionalToggle.BuildWall:
					currentFloor.InteractableUnder(true);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(false);
					break;
				case OptionalToggle.ColorWall:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = true;
					currentFloor.EnableCellsCollider(false);
					break;
				case OptionalToggle.ColorFloor:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(true);
					break;
				case OptionalToggle.Window:
				case OptionalToggle.Door:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = true;
					currentFloor.EnableCellsCollider(false);
					break;
				case OptionalToggle.AddFeature:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(true);
					SetFeaturePrefab();
					break;
				case OptionalToggle.RotateItem:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(true);
					currentFloor.EnableObjectsCollider(true);
					break;
				case OptionalToggle.MoveItem:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = true;
					currentFloor.EnableCellsCollider(true);
					currentFloor.EnableObjectsCollider(true);
					break;
				case OptionalToggle.ItemDestruction:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(false);
					currentFloor.EnableFeaturesCollider(true);
					currentFloor.EnableObjectsCollider(true);
					break;
				case OptionalToggle.AddWallFeature:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = true;
					currentFloor.EnableCellsCollider(false);
					SetFeaturePrefab();
					break;
				case OptionalToggle.AddPlaceableObject:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(false);
					currentFloor.EnableFeaturesCollider(true);
					currentFloor.EnableObjectsCollider(true);
					SetObjectPrefab();
					break;
				default:
					currentFloor.InteractableUnder(false);
					currentFloor.WallsCollide = false;
					currentFloor.EnableCellsCollider(false);
					break;
			}
		}
	}

	/// <summary>
	/// Appelée lors d'un changement de caméra pour supprimer les objets en cours de placement.
	/// </summary>
	public void Clear()
	{
		featureRotationPanel.SetActive(false);
		objectRotationPanel.SetActive(false);
		Destroy(featurePrefab);
		if (rotatedFeature)
		{
			ValidateItemRotation();
		}
		Destroy(objectPrefab);
		if (rotatedObject)
		{
			ValidateItemRotation();
		}
	}

	/// <summary>
	/// Traite le clique gauche effectué par l'utilisateur et détermine l'action à effectuer.
	/// Utilisé également sans clique gauche lors de la création de nouveaux objets.
	/// </summary>
	void HandleInput()
	{

		int surfaceLayerIndex = LayerMask.NameToLayer("AvailableSurface");
		int layerMask = 1 << surfaceLayerIndex;

		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit) && !EventSystem.current.IsPointerOverGameObject())
		{
			if (buildMode == OptionalToggle.Destruction)
			{
				if (hit.transform.name == "Wall(Clone)")
				{
					Wall wall = hit.transform.gameObject.GetComponent<Wall>();
					EraseWall(wall);
				}
				else if (hit.transform.name == "Cell(Clone)")
				{
					EditCell(currentFloor.GetCell(hit.point), hit.point);

				}
				else
				{
					DestroyItem(hit.transform.gameObject);
				}
			}
			else if (buildMode == OptionalToggle.BuildWall)
			{
				Dot currentDot = currentFloor.GetDot(hit.point);

				if (previousDot && previousDot != currentDot)
				{
					ValidateDrag(currentDot);
				}
				else {
					isDrag = false;
				}
				AddWall(currentDot);
				previousDot = currentDot;
			}
			else if (buildMode == OptionalToggle.ColorWall || buildMode == OptionalToggle.AddWallFeature)
			{
				Wall wall = hit.transform.gameObject.GetComponent<Wall>();
				int side = Metrics.GetWallSide(wall, hit.point);
				EditWall(wall, side);
				if (buildMode == OptionalToggle.AddWallFeature)
				{
					featurePrefab.GetComponent<Feature>().RendererActive = true;
					featurePrefab.GetComponent<Feature>().EnableOutline(true);
					if (isDropped && !leftClick)
					{
						AddFeatureToWall(wall, side, hit.point);
						SetFeaturePrefab();
						isDropped = false;
						leftClick = true;
					}
					else
					{
						MoveWallFeature(featurePrefab, hit.point, side);
						//UpdateHighlight(featurePrefab, hit.point);
					}
				}
			}
			else if (buildMode == OptionalToggle.ColorFloor)
			{
				EditCells(hit.point);
			}
			else if (buildMode == OptionalToggle.AddFeature)
			{
				if (hit.transform.name == "Cell(Clone)")
				{
					featurePrefab.GetComponent<Feature>().RendererActive = true;
					ObjectHighlight.SetActive(true);
					if (isDropped && !leftClick)
					{
						if (featurePrefab.GetComponent<Feature>().CheckFeatureAvailability(
							currentFloor.GetCell(hit.point),
							Metrics.GetCellPart(currentFloor.GetCell(hit.point), hit.point)))
						{
							AddFeatureToCell(currentFloor.GetCell(hit.point), hit.point);
							SetFeaturePrefab();
							isDropped = false;
							leftClick = true;
						}
						else
						{
							isDropped = false;
							UpdateHighlight(featurePrefab, hit.point);
						}
					}
					else
					{
						MoveCellFeature(featurePrefab, hit.point);
						UpdateHighlight(featurePrefab, hit.point);
					}
				}
				else
				{
					featurePrefab.GetComponent<Feature>().RendererActive = false;
					ObjectHighlight.SetActive(false);
				}
			}
			else if (buildMode == OptionalToggle.AddPlaceableObject)
			{
				PlaceableObject o = objectPrefab.GetComponent<PlaceableObject>();
				if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, layerMask))
				{
					AvailableSurface surface = hit.transform.GetComponent<AvailableSurface>();
					if (surface)
					{
						o.RendererActive = true;
						o.EnableOutline(true);
						if (isDropped && !leftClick)
						{
							o.level = currentFloor.level;
							ReleaseObject(o, surface);
							SetObjectPrefab();
							isDropped = false;
							leftClick = true;
						}
						else
						{
							MoveObject(o, hit.point, surface);
						}
					}
					else
					{
						o.RendererActive = false;
						o.EnableOutline(false);
					}
				}
				else
				{
					o.RendererActive = false;
					o.EnableOutline(false);
				}
			}
			else if (buildMode == OptionalToggle.ItemDestruction)
			{
				DestroyItem(hit.transform.gameObject);
			}
			else if (buildMode == OptionalToggle.MoveItem)
			{
				if (movedFeature == null && movedObject == null)
				{
					if (hit.transform.name == "Wall(Clone)")
					{
						Wall wall = hit.transform.gameObject.GetComponent<Wall>();
						movedFeature = GetFeature(wall, hit.point);
					}
					else if (hit.transform.name == "Cell(Clone)")
					{
						movedFeature = GetFeature(currentFloor.GetCell(hit.point), hit.point);
						if (movedFeature)
						{
							ChangeHighlight(movedFeature);
							ObjectHighlight.SetActive(true);
							UpdateHighlight(movedFeature, hit.point);
						}
					}
					else if (hit.transform.GetComponent<PlaceableObject>())
					{
						currentFloor.EnableFeaturesCollider(true);
						movedObject = hit.transform.gameObject;
						movedObject.GetComponent<PlaceableObject>().GetObject();
					}
				}
				else
				{
					if (movedFeature)
					{
						Feature featureProperties = movedFeature.GetComponent<Feature>();
						if (featureProperties.featureType == FeatureType.wall)
						{
							if (hit.transform.name == "Wall(Clone)")
							{
								Wall wall = hit.transform.gameObject.GetComponent<Wall>();
								MoveWallFeature(movedFeature, hit.point, Metrics.GetWallSide(wall, hit.point));
							}
						}
						else
						{
							if (hit.transform.name == "Cell(Clone)")
							{
								MoveCellFeature(movedFeature, hit.point);
								UpdateHighlight(movedFeature, hit.point);
							}
						}
					}
					else
					{
						PlaceableObject o = movedObject.GetComponent<PlaceableObject>();
						if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, layerMask))
						{
							AvailableSurface surface = hit.transform.GetComponent<AvailableSurface>();
							if (surface)
							{
								o.EnableOutline(true);
								o.RendererActive = true;
								MoveObject(o, hit.point, surface);
							}
						}
						else
						{
							o.RendererActive = false;
							o.EnableOutline(false);
						}
					}
				}
			}
			else if (buildMode == OptionalToggle.RotateItem)
			{
				if (rotatedFeature == null && rotatedObject == null)
				{
					if (hit.transform.GetComponent<PlaceableObject>())
					{
						rotatedObject = hit.transform.gameObject;
						rotatedObject.GetComponent<PlaceableObject>().Placed = false;
						objectRotationPanel.SetActive(true);
					}
					else
					{
						rotatedFeature = GetFeature(currentFloor.GetCell(hit.point), hit.point);
						if (rotatedFeature)
						{
							featureRotationPanel.SetActive(true);
						}
					}
				}
			}
			else if (buildMode == OptionalToggle.Window && !hidden)
			{
				Wall wall = hit.transform.gameObject.GetComponent<Wall>();
				EditWall(wall);
			}
			else if (buildMode == OptionalToggle.Door && !hidden)
			{
				Wall wall = hit.transform.gameObject.GetComponent<Wall>();
				EditWall(wall);
			}

		}
		else
		{
			if (buildMode == OptionalToggle.AddFeature)
			{
				featurePrefab.GetComponent<Feature>().RendererActive = false;
				ObjectHighlight.SetActive(false);
			}
			else if (buildMode == OptionalToggle.AddWallFeature)
			{
				featurePrefab.GetComponent<Feature>().RendererActive = false;
				featurePrefab.GetComponent<Feature>().EnableOutline(false);
			}
			else if (buildMode == OptionalToggle.MoveItem)
			{
				UnvailableHiglight();
			}
			else if (buildMode == OptionalToggle.AddPlaceableObject)
			{
				objectPrefab.GetComponent<PlaceableObject>().RendererActive = false;
				objectPrefab.GetComponent<PlaceableObject>().EnableOutline(false);
			}
		}
	}

	/// <summary>
	/// Détermine si l'utilisateur a glissé sa souris en maintenant le clique gauche d'un point donnée à un autre.
	/// Renvoie true si oui, false si non.
	/// </summary>
	void ValidateDrag(Dot currentDot)
	{
		for (dragDirection = CompassDirection.N; dragDirection <= CompassDirection.NW; dragDirection++)
		{
			if (previousDot.GetNeighbor(dragDirection) == currentDot)
			{
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	/// <summary>
	/// Sélectionne la couleur que l'utilisateur souhaite appliquer.
	/// </summary>
	public void SelectColor(int index)
	{
		applyColor = index >= 0;
		if (applyColor)
		{
			activeColor = colors[index];
		}
	}

	/// <summary>
	/// Selectionne le type de sol que l'utilisateur souhaite appliquer.
	/// </summary>
	public void SetType(int index)
	{
		applyColor = index >= 0;
		if (applyColor)
		{
			activeType = index;
		}
	}

	/// <summary>
	/// Sélectionne le prefab de la fenêtre que l'utilisateur souhaite instantier.
	/// </summary>
	public void SelectWindow(int index)
	{
		applyWindow = index;
		applyDoor = -1;
	}

	/// <summary>
	/// Sélectionne le prefab de la porte que l'utilisateur souhaite instantier.
	/// </summary>
	public void SelectDoor(int index)
	{
		applyDoor = index;
		applyWindow = -1;
	}

	/// <summary>
	/// Edite la cellule située à la position de la souris lors du clique gauche, et les cellules situées autour d'elle,
	/// en fonction de la taille du pinceau actif.
	/// La texture est appliquée à tous les triangles composant le quart de cellule touché.
	/// </summary>
	void EditCells(Vector3 hitPoint)
	{
		if (currentFloor.GetCell(hitPoint) == null)
		{
			return;
		}
		Vector3 newHitPoint;
		float cellRadius = Metrics.innerRadius;

		for (int z = -brushSize; z <= 0; z++)
		{
			for (int x = -brushSize; x <= brushSize; x++)
			{
				newHitPoint = hitPoint + new Vector3(x * cellRadius, 0f, z*cellRadius);
				EditCell(currentFloor.GetCell(newHitPoint),newHitPoint);
			}
		}
		for (int z = brushSize; z > 0; z--)
		{
			for (int x = - brushSize; x <= brushSize; x++)
			{
				newHitPoint = hitPoint + new Vector3(x * cellRadius, 0f, z * cellRadius);
				EditCell(currentFloor.GetCell(newHitPoint), newHitPoint);
			}
		}
	}

	/// <summary>
	/// Détermine le quart de cellule touché, et le triangle touché dans ce quart, pour appliquer la texture.
	/// </summary>
	void EditCell(Cell cell, Vector3 hitPoint)
	{
		if (cell != null)
		{
			Dot dot;
			CompassDirection direction = Metrics.GetCellQuarter(cell,hitPoint);
			Dot pivot = cell.pivot;

			dot = pivot.GetNeighbor(direction);
			Vector3 centerSquare = Metrics.GetMiddle(pivot.transform.position, dot.transform.position);
			float dX = hitPoint.x - centerSquare.x;
			float dZ = hitPoint.z - centerSquare.z;
			int triangle;
			if (Mathf.Abs(dZ) >= Mathf.Abs(dX))
			{
				if (dZ >= 0)
				{
					switch (direction)
					{
						case CompassDirection.NW:
							pivot = pivot.GetNeighbor(CompassDirection.NW);
							direction = CompassDirection.E;
							triangle = 2;
							break;
						case CompassDirection.NE:
							pivot = pivot.GetNeighbor(CompassDirection.N);
							direction = CompassDirection.E;
							triangle = 5;
							break;
						case CompassDirection.SE:
							direction = CompassDirection.E;
							triangle = 8;
							break;
						default:
							pivot = pivot.GetNeighbor(CompassDirection.W);
							direction = CompassDirection.E;
							triangle = 15;
							break;
					}
				}
				else {
					switch (direction)
					{
						case CompassDirection.NW:
							direction = CompassDirection.W;
							triangle = 0;
							break;
						case CompassDirection.NE:
							pivot = pivot.GetNeighbor(CompassDirection.E);
							direction = CompassDirection.W;
							triangle = 7;
							break;
						case CompassDirection.SE:
							pivot = pivot.GetNeighbor(CompassDirection.SE);
							direction = CompassDirection.W;
							triangle = 10;
							break;
						default:
							pivot = pivot.GetNeighbor(CompassDirection.S);
							direction = CompassDirection.W;
							triangle = 13;
							break;
					}
				}
			}
			else {
				if (dX >= 0)
				{
					switch (direction)
					{
						case CompassDirection.NW:
							pivot = pivot.GetNeighbor(CompassDirection.N);
							direction = CompassDirection.S;
							triangle = 3;
							break;
						case CompassDirection.NE:
							pivot = pivot.GetNeighbor(CompassDirection.NE);
							direction = CompassDirection.S;
							triangle = 6;
							break;
						case CompassDirection.SE:
							pivot = pivot.GetNeighbor(CompassDirection.E);
							direction = CompassDirection.S;
							triangle = 9;
							break;
						default:
							direction = CompassDirection.S;
							triangle = 12;
							break;
					}
				}
				else {
					switch (direction)
					{
						case CompassDirection.NW:
							pivot = pivot.GetNeighbor(CompassDirection.W);
							direction = CompassDirection.N;
							triangle = 1;
							break;
						case CompassDirection.NE:
							direction = CompassDirection.N;
							triangle = 4;
							break;
						case CompassDirection.SE:
							pivot = pivot.GetNeighbor(CompassDirection.S);
							direction = CompassDirection.N;
							triangle = 11;
							break;
						default:
							pivot = pivot.GetNeighbor(CompassDirection.SW);
							direction = CompassDirection.N;
							triangle = 14;
							break;
					}
				}
			}
			EditTriangles(triangle, cell, pivot, direction);
		}
	}

	/// <summary>
	/// Ajoute l'objet selectionnée à une cellule donnée, en lui donnant pour base l'endroit visé.
	/// </summary>
	void AddFeatureToCell(Cell cell, Vector3 hitPoint)
	{
		if (cell != null)
		{
			Feature featureProperties = featurePrefab.GetComponent<Feature>();
			int part = Metrics.GetCellPart(cell, hitPoint);
			if (featureProperties.CheckFeatureAvailability(cell, part))
			{
				featureProperties.AddFeature(cell, part);
				featurePrefab = null;
			}
		}
	}

	/// <summary>
	/// Ajoute l'objet selectionné à une face donée d'un mur donné, en lui donnant pour base l'endroit visé.
	/// </summary>
	void AddFeatureToWall(Wall wall, int side, Vector3 hitPoint)
	{
		if (wall != null)
		{
			Feature featureProperties = featurePrefab.GetComponent<Feature>();
			int part = Metrics.GetWallPart(wall, hitPoint, side);
			if (featureProperties.CheckWallFeatureAvailability(wall, part, side))
			{
				featureProperties.AddFeatureToWall(wall, side, part);
				featurePrefab = null;
			}
		}
	}

	/// <summary>
	/// Selectionne la référence de l'objet choisi et le crée.
	/// </summary>
	public void SetFeatureRef(int value)
	{
		featureRef = value;
		SetFeaturePrefab();
	}

	/// <summary>
	/// Selectionne la référence de l'objet choisi et le crée.
	/// </summary>
	public void SetObjectRef(int value)
	{
		objectRef = value;
		SetObjectPrefab();
	}

	/// <summary>
	/// Crée le meuble choisi, sans le placer sur la grille.
	/// </summary>
	void SetFeaturePrefab()
	{
		Destroy(featurePrefab);
		if (buildMode == OptionalToggle.AddFeature)
		{
			featurePrefab = manager.InstantiateObject(featureRef, ObjectType.feature);
			RotateFeature(keepOrientation);
		}
		else
		{
			featurePrefab = manager.InstantiateObject(featureRef, ObjectType.wallFeature);
			RotateFeature(keepOrientation);
		}
		featurePrefab.GetComponent<Feature>().ColliderActive = false;
		ChangeHighlight(featurePrefab);
	}

	/// <summary>
	/// Crée l'objet choisi, sans le placer sur la grille.	
	/// </summary>
	void SetObjectPrefab()
	{
		Destroy(objectPrefab);
		objectPrefab = manager.InstantiateObject(objectRef, ObjectType.placeableObject);
		objectPrefab.GetComponent<PlaceableObject>().RendererActive = false;
	}

	/// <summary>
	/// Selectionne le rectangle à placer sous l'objet pour les modes "AddFeature" et "Movefeature" en fonction de la
	/// taille de l'objet selectionné.
	/// </summary>
	void ChangeHighlight(GameObject feature)
	{
		Destroy(ObjectHighlight);
		Feature featureProperties = feature.GetComponent<Feature>();
		Bloc featurePosition = featureProperties.blocType;
		switch (featurePosition)
		{
			case Bloc.bloc1x1:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[0]);
				break;
			case Bloc.bloc1x2:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[1]);
				break;
			case Bloc.bloc1x3:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[2]);
				break;
			case Bloc.bloc1x4:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[3]);
				break;
			case Bloc.bloc2x2:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[4]);
				break;
			case Bloc.bloc2x3:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[5]);
				break;
			default:
				ObjectHighlight = Instantiate(ObjectHighlightPrefabs[6]);
				break;
		}
		ObjectHighlight.transform.SetParent(currentFloor.canvas.transform, false);
		ObjectHighlight.SetActive(false);
	}

	/// <summary>
	/// Met à jour la position, la rotation et la couleur du rectangle sous l'objet à placer.
	/// </summary>
	void UpdateHighlight(GameObject feature, Vector3 hitPoint)
	{
		if (feature)
		{
			Feature featureProperties = feature.GetComponent<Feature>();
			SquareDirection orientation = featureProperties.orientation;
			ObjectHighlight.transform.position =
				new Vector3(feature.transform.position.x, hitPoint.y, feature.transform.position.z);
			if (orientation == SquareDirection.N || orientation == SquareDirection.S)
			{
				ObjectHighlight.GetComponent<RectTransform>().eulerAngles =
					new Vector3(90f, 90f, 90f);
			}
			else
			{
				ObjectHighlight.GetComponent<RectTransform>().eulerAngles =
					new Vector3(90f, 0f, 90f);
			}

			Cell cell = currentFloor.GetCell(hitPoint);
			int part = Metrics.GetCellPart(cell, hitPoint);

			if (featureProperties.CheckFeatureAvailability(cell, part))
			{
				ObjectHighlight.GetComponentInChildren<Image>().color = availableColor;
			}
			else
			{
				UnvailableHiglight();
			}
		}
	}

	/// <summary>
	/// Change la couleur du rectangle sous l'objet à placer par du rouge.
	/// </summary>
	void UnvailableHiglight()
	{
		ObjectHighlight.GetComponentInChildren<Image>().color = unavailableColor;
	}

	/// <summary>
	/// Détruit l'objet pointé.
	/// </summary>
	void DestroyItem(GameObject hit)
	{
		if (hit.GetComponent<Feature>())
		{
			hit.GetComponent<Feature>().DestroyFeature();
		}
		else if (hit.GetComponent<PlaceableObject>())
		{
			hit.GetComponent<PlaceableObject>().DestroyObject();
		}
		else
		{
			hit.GetComponentInParent<Feature>().DestroyFeature();
		}
	}

	/// <summary>
	/// Renvoie l'objet placé sur la cellule donnée au point donné.
	/// </summary>
	GameObject GetFeature(Cell cell, Vector3 hitPoint)
	{
		if (cell != null)
		{
			int part = Metrics.GetCellPart(cell, hitPoint);
			GameObject feature = GetFeature(cell, part);
			return feature;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Renvoie l'objet placé sur un mur donné au point donné.
	/// </summary>
	GameObject GetFeature(Wall wall, Vector3 hitPoint)
	{
		if (wall != null)
		{
			int side = Metrics.GetWallSide(wall, hitPoint);
			int part = Metrics.GetWallPart(wall, hitPoint, side);
			GameObject feature = GetFeature(wall, part, side);
			return feature;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Déplace le meuble donné au point donné sans le placer sur la grille.
	/// L'utilisateur doit effectuer une autre action pour cela.
	/// </summary>
	void MoveCellFeature(GameObject feature, Vector3 hitPoint)
	{
		Feature featureProperties = feature.GetComponent<Feature>();
		int part = Metrics.GetCellPart(currentFloor.GetCell(hitPoint), hitPoint);
		featureProperties.CalculatePosition(part);
		float elevation = Metrics.wallsElevation * currentFloor.level;
		feature.transform.position += currentFloor.GetCell(hitPoint).Position + 
			new Vector3(0f,featureProperties.height/2f + elevation,0f);
	}

	/// <summary>
	/// Déplace l'objet donné au point donné sans le placer sur la grille.
	/// L'utilisateur doit effectuer une autre action pour cela.
	/// Appelle "CheckObjectAvaibility" pour changer la couleur de l'objet.
	/// </summary>
	void MoveObject(PlaceableObject o, Vector3 hitPoint, AvailableSurface surface)
	{
		o.transform.position = hitPoint - surface.yOffset;
		CheckObjectAvailability(o, surface);
	}

	/// <summary>
	/// Vérifie si l'objet peut être placé à l'endroit visé et change sa couleur en conséquence.
	/// Vert s'il peut être placé, rouge sinon.
	/// </summary>
	void CheckObjectAvailability(PlaceableObject o, AvailableSurface surface)
	{
		if (surface.CheckAvailability(o) && !o.Contact)
		{
			o.SetOutline(true);
		}
		else
		{
			o.SetOutline(false);
		}
	}

	/// <summary>
	/// Lâche l'objet "o" sur la surface visée.
	/// Si l'objet peut être placé, le place sur la surface en enfant de celle-ci.
	/// Sinon, le détruit.
	/// </summary>
	void ReleaseObject(PlaceableObject o, AvailableSurface surface)
	{
		if (surface.CheckAvailability(o) && !o.Contact)
		{
			o.transform.SetParent(surface.transform, true);
			o.Placed = true;
		}
		else
		{
			Destroy(o.gameObject);
		}
		objectPrefab = null;
		movedObject = null;
		rotatedObject = null;
	}

	/// <summary>
	/// Déplace l'objet donné au point donné sans le placer sur le côté de mur choisi.
	/// L'utilisateur doit effectuer une autre action pour cela.
	/// </summary>
	void MoveWallFeature(GameObject feature, Vector3 hitPoint, int side)
	{
		Feature featureProperties = feature.GetComponent<Feature>();
		Wall wall;
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit) && !EventSystem.current.IsPointerOverGameObject())
		{
			if (hit.transform.name == "Wall(Clone)")
			{
				featureProperties.EnableOutline(true);
				wall = hit.transform.gameObject.GetComponent<Wall>();
				int part = Metrics.GetWallPart(wall, hitPoint, Metrics.GetWallSide(wall, hit.point));
				if (part > -1)
				{
					featureProperties.CalculatePosition(wall.direction, part, side);
					featureProperties.AdjustPosition(wall, side);
					float elevation = Metrics.wallsElevation * currentFloor.level;
					feature.transform.position += new Vector3(wall.Position.x, elevation, wall.Position.z);
				}
				if (featureProperties.CheckWallFeatureAvailability(wall, part, Metrics.GetWallSide(wall, hit.point)))
				{
					featureProperties.SetOutline(true);
				}
				else
				{
					featureProperties.SetOutline(false);
				}
			}
			else
			{
				featureProperties.EnableOutline(false);
			}
		}
		else
		{
			featureProperties.EnableOutline(false);
		}
	}

	/// <summary>
	/// Tourne selon l'orientation donnée le meuble correspondant au mode actuel d'édition.
	/// </summary>
	public void RotateFeature(int orientation)
	{
		GameObject feature;
		if (buildMode == OptionalToggle.RotateItem)
		{
			feature = rotatedFeature;
		}
		else if (buildMode == OptionalToggle.AddFeature || buildMode == OptionalToggle.AddWallFeature)
		{
			feature = featurePrefab;
		}
		else
		{
			feature = movedFeature;
		}
		Feature featureProperties = feature.GetComponent<Feature>();
		Vector3 rotation = featureProperties.baseRotation;
		SquareDirection previousOrientation = featureProperties.orientation;
		switch (orientation)
		{
			case 0:
				rotation += new Vector3(0f, 180f, 0f);
				featureProperties.orientation = SquareDirection.N;
				break;
			case 1:
				rotation += new Vector3(0f, 90f, 0f);
				featureProperties.orientation = SquareDirection.W;
				break;
			case 2:
				rotation += new Vector3(0f, 0f, 0f);
				featureProperties.orientation = SquareDirection.S;
				break;
			default:
				rotation += new Vector3(0f, -90f, 0f);
				featureProperties.orientation = SquareDirection.E;
				break;
		}
		if (buildMode == OptionalToggle.RotateItem)
		{
			if (featureProperties.CheckFeatureAvailability(
				featureProperties.BaseCell, featureProperties.BasePart))
			{
				feature.transform.eulerAngles = rotation;
				featureProperties.CalculatePosition(featureProperties.BasePart);
				feature.transform.position += featureProperties.BaseCell.Position;
				keepOrientation = orientation;
			}
			else
			{
				featureProperties.orientation = previousOrientation;
			}
		}
		else
		{
			feature.transform.eulerAngles = rotation;
			keepOrientation = orientation;
		}
	}

	/// <summary>
	/// Tourne selon l'orientation donnée l'objet correspondant au mode actuel d'édition.
	/// </summary>
	public void RotateObject()
	{
		GameObject placeableObject;
		if (buildMode == OptionalToggle.RotateItem)
		{
			placeableObject = rotatedObject;
		}
		else if (buildMode == OptionalToggle.AddPlaceableObject)
		{
			placeableObject = objectPrefab;
		}
		else
		{
			placeableObject = movedObject;
		}
		Vector3 rotation = placeableObject.transform.eulerAngles;
		rotation += new Vector3(0f, 45f, 0f);
		placeableObject.transform.eulerAngles = rotation;
	}

	/// <summary>
	/// Place l'objet à faire tourner sur la grille et ferme la fenêtre de rotation.
	/// </summary>
	public void ValidateItemRotation()
	{
		if (rotatedFeature)
		{
			Feature featureProperties = rotatedFeature.GetComponent<Feature>();
			featureProperties.AddFeature(featureProperties.BaseCell, featureProperties.BasePart);
			rotatedFeature = null;
		}
		else if (rotatedObject)
		{
			PlaceableObject o = rotatedObject.GetComponent<PlaceableObject>();
			AvailableSurface surface = rotatedObject.GetComponentInParent<AvailableSurface>();
			ReleaseObject(o, surface);

		}
		featureRotationPanel.SetActive(false);
		objectRotationPanel.SetActive(false);
	}

	/// <summary>
	/// Lâche l'objet "movedFeature" sur la grille, et le place si il est lâché dans un emplacement valide, sinon le
	/// supprime.
	/// </summary>
	void ReleaseFeature()
	{
		Feature featureProperties = movedFeature.GetComponent<Feature>();
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			if (hit.transform.name == "Cell(Clone)" && featureProperties.featureType != FeatureType.wall)
			{
				Cell cell = currentFloor.GetCell(hit.point);
				if (cell)
				{
					int part = Metrics.GetCellPart(cell, hit.point);
					if (featureProperties.CheckFeatureAvailability(cell, part))
					{
						featureProperties.AddFeature(cell, part);
						return;
					}
				}
			}
			else if (hit.transform.name == "Wall(Clone)" && featureProperties.featureType == FeatureType.wall)
			{
				Wall wall = hit.transform.gameObject.GetComponent<Wall>();
				int side = Metrics.GetWallSide(wall, hit.point);
				if (wall)
				{
					int part = Metrics.GetWallPart(wall, hit.point, side);
					if (featureProperties.CheckWallFeatureAvailability(wall, part, side))
					{
						featureProperties.AddFeatureToWall(wall, side, part);
						return;
					}
				}
			}
		}
		Destroy(movedFeature);
	}

	/// <summary>
	/// Selectionne la taille du pinceau.
	/// </summary>
	public void SetBrushSize(float size)
	{
		brushSize = (int)size;
	}

	/// <summary>
	/// Ajoute un mur au point donné.
	/// </summary>
	void AddWall(Dot dot)
	{
		if (dot && isDrag)
		{
			dot.SetWall(dragDirection.Opposite(), true);
		}
	}

	/// <summary>
	/// Détruit le mur donné. L'objet "wall" existe toujours, mais n'est plus actif, et ses paramètres sont remis à 0.
	/// </summary>
	void EraseWall(Wall wall)
	{
		Dot dot = wall.dot;
		wall.ResetCollider();
		if (wall.HasOutgoingOpening)
		{
			wall.neighbor.GetWall(wall.direction).HasIncomingOpening = false;
			wall.neighbor.HasAWindow = false;
			wall.neighbor.HasADoor = false;
		}
		if (wall.HasIncomingOpening)
		{
			wall.dot.GetWall(wall.direction.Opposite()).HasOutgoingOpening = false;
			wall.dot.GetWall(wall.direction.Opposite()).o_Window = null;
			wall.dot.HasAWindow = false;
			wall.dot.HasADoor = false;
		}
		wall.HasIncomingOpening = wall.HasOutgoingOpening = false;
		wall.o_Window = null;
		wall.o_Door = null;
		wall.Raised = false;
		dot.SetWall(wall.direction, false);
		wall.ColorLeft = wall.ColorRight = Color.white;
		wall.EraseFeatures();
	}

	/// <summary>
	/// Applique aux triangles donnés de la cellule donnée la texture active.
	/// </summary>
	void EditTriangles(Cell cell, bool[] triangles)
	{
		int[] applyColors = new int[16];
		int appliedColor = activeType;
		if (buildMode == OptionalToggle.Destruction)
		{
			if (currentFloor.level > 0)
			{
				appliedColor = 0;
			}
			else
			{
				appliedColor = defaultColorGroundFloor;
			}
		}
		for (int i = 0; i < triangles.Length; i++)
		{
			applyColors[i] = cell.Colors[i];
			if (triangles[i])
			{
				applyColors[i] = appliedColor;
			}
		}
		cell.Colors = applyColors;
	}

	/// <summary>
	/// À partir du point pivot, détermine les triangles du quart de cellule donnée à éditer. Cette sélection s'effectue 
	/// en fonction du positionnement des murs.
	/// </summary>
	void EditTriangles(int triangle, Cell cell, Dot pivot, CompassDirection direction)
	{
		bool[] triangles = new bool[16];
		triangles[triangle] = true;

		Dot nextDot = pivot.GetNeighbor(direction);

		bool walledNextToCenter, walledPivotToCenter;
		walledNextToCenter = nextDot.isWalled(direction.Opposite().GetPrevious());
		walledPivotToCenter = pivot.isWalled(direction.GetNext());

		bool pivotTriangle, nextTriangle, secondNextTriangle, thirdNextTriangle;
		if (brushSize < 1)
		{
			pivotTriangle = nextTriangle = secondNextTriangle = thirdNextTriangle = false;
			if (!walledNextToCenter)
			{
				if (!walledPivotToCenter)
				{
					secondNextTriangle = thirdNextTriangle = pivotTriangle = nextTriangle = true;
				}
				else {
					pivotTriangle = nextTriangle = true;
				}
			}
			else
			{
				if (!walledPivotToCenter)
				{
					thirdNextTriangle = pivotTriangle = true;
				}
				else {
					pivotTriangle = true;
				}
			}
		}
		else
		{
			pivotTriangle = nextTriangle = secondNextTriangle = thirdNextTriangle = true;
		}
		triangles[triangle] = pivotTriangle;
		triangles[Metrics.GetNextTriangle(triangle)] = nextTriangle;
		triangles[Metrics.GetSecondNextTriangle(triangle)] = secondNextTriangle;
		triangles[Metrics.GetThirdNextTriangle(triangle)] = thirdNextTriangle;
		EditTriangles(cell, triangles);
	}

	/// <summary>
	/// L'enumération des différents modes d'édition.
	/// </summary>
	enum OptionalToggle
	{
		Ignore, //Aucune fonction d'édition ne peut être appliquée. - - mode 0 - -

		Destruction, //Détruit tout objet, meuble, sol ou mur sur lequel clique l'utilisateur.- - mode 1 - -

		BuildWall, //Permet de construire un mur entre deux points voisins si l'utilisateur glisse sa souris de l'un à
				   // l'autre en maintenant un clique gauche.- - mode 2 - -

		ColorWall, //Colorie avec la couleur enregistrée le mur sur lequel l'utilisateur clique.- - mode 3 - -

		ColorFloor, //Applique la texture enregistrée au sol sur lequel l'utilisateur clique.- - mode 4 - -

		Window, //Applique le type de fenêtre enregistré au mur sur lequel l'utilisateur clique.- - mode 5 - -

		Door, //Applique le type de porte enregistré au mur sur lequel l'utilisateur clique.- - mode 6 - -

		AddFeature, //Permet d'ajouter des meubles sur la grille lorsque l'utilisateur clique sur une cellule.- - mode 7

		ItemDestruction, //Détruit tout objet ou meuble sur lequel clique l'utilisateur.- - mode 8 - -

		MoveItem, //Permet de déplacer tout objet ou meuble sur lequel clique l'utilisateur.- - mode 9 - -

		RotateItem, //Permet de faire tourner tout objet ou meuble sur lequel clique l'utilisateur.- - mode 10 - -

		AddWallFeature, //Permet d'ajouter des objets aux murs lorsque l'utilisateur clique dessus.- - mode 11 - -

		AddPlaceableObject //Permet d'ajouter des objets sur toutes les surfaces de meubles possibles.- - mode 12 - -
	}

	/// <summary>
	/// Selectionne le mode d'édition, et remet à 0 les variables des autres modes.
	/// </summary>
	public void SetMode(int mode)
	{
		if (buildMode != (OptionalToggle)mode)
		{
			ValidateItemRotation();
			buildMode = (OptionalToggle)mode;
			ChangeMode();
		}
	}


	/// <summary>
	/// Cache les murs.
	/// </summary>
	public void HideWalls()
	{
		if (!hidden)
		{
			hidden = true;
			wallsUpEditor.SetActive(true);
			wallsUpOverview.SetActive(true);
			wallsDownEditor.SetActive(false);
			wallsDownOverview.SetActive(false);
			currentFloor.HideWalls();
		}
	}

	/// <summary>
	/// Affiche les murs.
	/// </summary>
	public void ShowWalls()
	{
		hidden = false;
		wallsUpEditor.SetActive(false);
		wallsUpOverview.SetActive(false);
		wallsDownEditor.SetActive(true);
		wallsDownOverview.SetActive(true);
		currentFloor.ShowWalls();
	}

	/// <summary>
	/// Abaisse les murs si ils doivent l'être.
	/// </summary>
	public void CheckWallsHidden()
	{
		if (hidden)
		{
			currentFloor.HideWalls();
		}
	}

	/// <summary>
	/// Edite le mur donné, en lui ajoutant porte ou fenêtre, si l'emplacement est disponible.
	/// </summary>
	void EditWall(Wall wall)
	{
		if (wall)
		{
			if (applyWindow >= 0)
			{
				GameObject window = manager.InstantiateObject(applyWindow, ObjectType.window);
				if (!wall.CheckWindowAvaibility(applyWindow, window))
				{
					Destroy(window);
				}
			}
			else if (applyDoor >= 0)
			{
				GameObject door = manager.InstantiateObject(applyDoor, ObjectType.door);
				if (!wall.CheckDoorAvaibility(applyDoor, door))
				{
					Destroy(door);
				}
			}
		}
	}

	/// <summary>
	/// Applique la texture active à la face donnée du mur donné.
	/// </summary>
	void EditWall(Wall wall, int side)
	{
		if (wall)
		{
			if (buildMode == OptionalToggle.ColorWall)
			{
				Color appliedColor = activeColor;
				if (side == -1)
				{
					wall.ColorLeft = appliedColor;
				}
				else if (side == 1)
				{
					wall.ColorRight = appliedColor;
				}
				else {
					return;
				}
			}
		}
	}

	/// <summary>
	/// Ajoute un nouvel étage d'indice i.
	/// </summary>
	public void AddNewFloor(int i)
	{
		floors[i] = Instantiate(squareGridPrefab);
		floors[i].level = i;
		floors[i].Build(cellCountX, cellCountZ);
		floors[i].transform.position = new Vector3(0f, floors[i - 1].elevation, 0f);
		floors[i].elevation += floors[i].transform.position.y;
	}

	/// <summary>
	/// Monte la caméra d'un étage pour l'édition ou la vue d'ensemble et désactive les fonctionnalités de l'étage
	/// inférieur.
	/// </summary>
	public void Up()
	{
		int index = currentFloor.level;
		if (index < 2)
		{
			if (floors[index + 1] == null)
			{
				AddNewFloor(index + 1);
				floors[index].InteractableUnder(false);
				floors[index].HideCanvas(true);
			}
			else {
				floors[index + 1].InteractableUpper(true);
				floors[index].InteractableUnder(false);
				floors[index].HideCanvas(true);
			}
			ShowWalls();
			CurrentFloor = floors[index + 1];
			if (cameraEditor.enabled)
			{
				floors[index + 1].HideCanvas(false);
				floors[index + 1].Refresh(); //utile si on supprime des éléments en modifiant l'étage inférieur.
			}
		}
		if (cameraEditor.enabled)
		{
			ChangeMode();
		}
	}

	/// <summary>
	/// Monte la caméra d'un étage pour l'édition ou la vue d'ensemble et désactive l'étage supérieur.
	/// </summary>
	public void Down()
	{
		int index = currentFloor.level;
		if (index > 0)
		{
			ShowWalls();
			CurrentFloor = floors[index - 1];
			floors[index].InteractableUpper(false);
			floors[index-1].InteractableUnder(true);
			if (cameraEditor.enabled)
			{
				floors[index - 1].HideCanvas(false);
			}
		}
		if (cameraEditor.enabled)
		{
			ChangeMode();
		}
	}

	/// <summary>
	/// Affiche les étages s'ils existent.
	/// </summary>
	public void ShowLevels()
	{
		foreach (FloorGrid floor in floors)
		{
			if (floor)
			{
				floor.InteractableUpper(true);
				floor.HideCanvas(true);
			}
		}
	}

	/// <summary>
	/// Cache les étages s'ils existent.
	/// </summary>
	public void HideLevels()
	{
		int index = currentFloor.level;
		foreach (FloorGrid floor in floors)
		{
			if (floor && floor.level > index)
			{
				floor.InteractableUpper(false);
			}
		}
	}

	/// <summary>
	/// Efface le niveau actuel, sauf s'il s'agit du RDC.
	/// </summary>
	public void EraseLevel()
	{
		int index = currentFloor.level;
		if (index != 0 && ((index == floors.Length-1) || floors[index + 1] == null))
		{
			Down();
			Destroy(floors[index].transform.gameObject);
			floors[index] = null;
		}
	}

	/// <summary>
	/// Renvoie l'étage en cours d'édition, ou sélectionne un nouvel étage à éditer.
	/// </summary>
	public FloorGrid CurrentFloor
	{
		get
		{
			return currentFloor;
		}
		set
		{
			if (currentFloor != value)
			{
				currentFloor = value;
			}
		}
	}

	/// <summary>
	/// Sauvegarde les étages de la maison.
	/// </summary>
	public void Save(HomeDataWriter writer)
	{
		writer.Write(cellCountX);
		writer.Write(cellCountZ);

		floors[0].Save(writer);
		for (int i = 1; i < floors.Length; i++)
		{
			if (floors[i])
			{
				writer.Write((byte)123);
				floors[i].Save(writer);
			}
			else
			{
				writer.Write((byte)0);
			}
		}
	}

	/// <summary>
	/// Charge les étages de la maison.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		CreateHome(reader.ReadInt(), reader.ReadInt());
		floors[0].Load(reader);
		for (int i = 1; i < floors.Length; i++)
		{
			byte floorData = reader.ReadByte();
			if (floorData > 0)
			{
				AddNewFloor(i);
				floors[i].Load(reader);
				floors[i].InteractableUpper(false);
			}
			else
			{
				floors[i] = null;
			}
		}
		MapCamera.ValidatePosition();
		OverviewCamera.ResetCamera();
		SetMode(0);
	}

	/// <summary>
	/// Récupère l'objet situé dans le 16eme de cellule donné.
	/// </summary>
	GameObject GetFeature(Cell cell, int part)
	{
		GameObject feature = cell.features[part];
		Feature featureProperties;
		if (feature)
		{
			featureProperties = feature.GetComponent<Feature>();
			featureProperties.GetFeature();
		}
		else
		{
			return null;
		}
		return feature;
	}

	/// <summary>
	/// Récupère l'objet situé dans le 16eme du côté donné du mur donné.
	/// </summary>
	GameObject GetFeature(Wall wall, int part, int side)
	{
		GameObject feature;
		if (side == 1)
		{
			feature = wall.featuresRight[part];
		}
		else
		{
			feature = wall.featuresLeft[part];
		}
		Feature featureProperties;
		if (feature)
		{
			featureProperties = feature.GetComponent<Feature>();
			featureProperties.GetFeature(side);
		}
		else
		{
			return null;
		}
		return feature;
	}
}