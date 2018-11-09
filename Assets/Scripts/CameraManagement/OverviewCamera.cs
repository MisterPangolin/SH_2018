using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attachée à l'objet "OverviewCamera" dans la scene "Creation_architecture".
/// Permet à l'utilisateur d'avoir une vue d'ensemble de la maison observée grace à une camera dirigée par cette classe 
/// qui tourne autour.
/// </summary>
public class OverviewCamera : MonoBehaviour 
{
	//référence à lui-même
	static OverviewCamera instance;

	//éditeur et étages
	HomeEditor editor;
	FloorGrid currentFloor;
	FloorGrid[] floors;

	//déplacement et rotation
	float direction = 1f;
	float zoom = -25f;
	float elevation = 40f;
	public float sensitivity = 4f;
	public float MinZoom, MaxZoom;
	public float MinAngle, MaxAngle;
	public float MaxElevation, MinElevation;
	public float speedAuto, speedManual;
	public new Camera camera;
	Vector3 pivot;
	bool manual;

	//description des objets visés
	Device watchedDevice;
	public DevicePanel descriptivePanel;

	/// <summary>
	/// Crée une reference unique à la classe dans le projet.
	/// </summary>
	void Awake()
	{
		instance = this;
		editor = GameObject.Find("Editor").GetComponent<HomeEditor>();
		manual = false;
	}

	/// <summary>
	/// Initialise la camera.
	/// </summary>
	void Start() //se déclenche après les awakes
	{
		camera = GetComponentInChildren<Camera>();
		camera.transform.eulerAngles = new Vector3(25f, 45f, 0f);
		camera.transform.position = new Vector3(zoom, elevation, zoom);
		ResetCamera();
	}

	/// <summary>
	/// Appelée à chaque frame.
	/// Dirige la camera selon le mode de rotation choisi (automatique ou manuel).
	/// </summary>
	void Update()
	{
		if (camera.enabled)
		{
			if (manual)
			{
				ManualRotation();
			}
			else {
				AutomaticRotation();
			}
			float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
			if (zoomDelta != 0f)
			{
				AdjustZoom(zoomDelta);
			}
			int wallLayerIndex = LayerMask.NameToLayer("Walls");
			int layerMask = 1 << wallLayerIndex;
			layerMask = ~layerMask;
			Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, layerMask) && !EventSystem.current.IsPointerOverGameObject())
			{
				Device newSensor = null;
				if (hit.transform.GetComponent<Device>())
				{
					newSensor = hit.transform.GetComponent<Device>();
				}
				else if (hit.transform.GetComponentInParent<Device>())
				{
					newSensor = hit.transform.GetComponentInParent<Device>();
				}
				if (watchedDevice && newSensor && newSensor.transform != watchedDevice.transform)
				{
					watchedDevice.SetOutlineColor(1);
				}
				watchedDevice = newSensor;
				if (watchedDevice)
				{
					watchedDevice.SetOutlineColor(2);
				}
			}
			else
			{
				Clear();
			}
			if (watchedDevice && Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				descriptivePanel.Open(watchedDevice);
			}
		}
	}

	/// <summary>
	/// Desactive les composants outline de l'objet observé, puis efface la reference à cet objet.
	/// </summary>
	public static void Clear()
	{
		if (instance.watchedDevice)
		{
			instance.watchedDevice.SetOutlineColor(1);
			instance.watchedDevice = null;
		}
	}

	/// <summary>
	/// Automatise la rotation.
	/// </summary>
	void AutomaticRotation()
	{
		currentFloor = editor.currentFloor;
		transform.position = new Vector3(transform.position.x, currentFloor.level * 5f, transform.position.z);

		if (Input.GetKey("left"))
		{
			direction = -1;
		}
		if (Input.GetKey("right"))
		{
			direction = 1;
		}
		if (Input.GetKey("space"))
		{
			direction = 0;
		}
		transform.RotateAround(pivot, Vector3.up,
		                       direction * speedAuto * Time.deltaTime);
	}

	/// <summary>
	/// Change la rotation selon les inputs de l'utilisateur.
	/// </summary>
	void ManualRotation()
	{
		if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
		{
			transform.RotateAround(pivot, Vector3.up, Input.GetAxis("Mouse X") * speedManual * Time.deltaTime);
		}
	}

	/// <summary>
	/// Change le mode de rotation de la caméra.
	/// </summary>
	public void SwitchManual()
	{
		manual = !manual;
	}

	/// <summary>
	/// Ajuste le zoom de la caméra selon les inputs de l'utilisateur.
	/// </summary>
	void AdjustZoom(float delta)
	{
		zoom = Mathf.Clamp01(zoom + delta * sensitivity);
		float distance = Mathf.Lerp(MinZoom, MaxZoom, zoom);
		elevation = Mathf.Lerp(MaxElevation, MinElevation, zoom);
		camera.transform.localPosition = new Vector3(distance, elevation, distance);

		float angle = Mathf.Lerp(MinAngle, MaxAngle, zoom);
		camera.transform.localEulerAngles = new Vector3(angle, 45f, 0f);
	}

	/// <summary>
	/// Monte la caméra d'un étage.
	/// </summary>
	public void Up()
	{
		int index = editor.currentFloor.level;
		if (index < 2)
		{
			if (floors[index + 1] != null)
			{
				Debug.Log("up camera");
				editor.Up();
				editor.currentFloor.WallsCollide = true;
				DeviceStorage.SetOutlines(false, index);
				DeviceStorage.SetOutlines(true, index + 1);
				DeviceStorage.EnableColliders(false, index);
				DeviceStorage.EnableColliders(true, index + 1);
			}
		}
	}

	/// <summary>
	/// Descends la caméra d'un étage.
	/// </summary>
	public void Down()
	{
		int index = editor.currentFloor.level;
		if (index > 0)
		{
			Debug.Log("down camera");
			editor.Down();
			editor.currentFloor.WallsCollide = true;
			DeviceStorage.SetOutlines(false, index);
			DeviceStorage.SetOutlines(true, index - 1);
			DeviceStorage.EnableColliders(false, index);
			DeviceStorage.EnableColliders(true, index - 1);
		}
	}

	/// <summary>
	/// Active ou désactive ce script. Appelée lors de l'ouverture et de la fermeture du menu.
	/// </summary>
	public static bool Locked
	{
		set
		{
			instance.enabled = !value;
		}
	}

	/// <summary>
	/// Réinitialise la caméra. Appelée lors de la création d'une nouvelle maison.
	/// </summary>
	public static void ResetCamera()
	{
		instance.currentFloor = instance.editor.currentFloor;
		instance.floors = instance.editor.floors;

		instance.transform.eulerAngles = new Vector3(0, 0, 0);
		instance.transform.position = new Vector3(0, 0, 0);
		instance.pivot.x = Metrics.innerRadius * (instance.currentFloor.cellCountX - 1f);
		instance.pivot.z = Metrics.innerRadius * (instance.currentFloor.cellCountZ - 1f);
	}

	//Tout ce qui suit peut être amélioré, un collider a été ajouté à la caméra afin de permettre de baisser les murs
	// tous les murs qui rencontrent ce collider sont baissés.

	/// <summary>
	/// Détecte les murs qui entrent en collision avec la caméra/
	/// </summary>
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Wall>())
		{
			other.gameObject.GetComponent<Wall>().isTriggered = true;
		}
	}

	/// <summary>
	/// Détecte les murs en collision avec la caméra.
	/// </summary>
	public void OnTriggerStay(Collider other)
	{
		if (other.gameObject.GetComponent<Wall>())
		{
			other.gameObject.GetComponent<Wall>().isTriggered = true;
		}
	}

	/// <summary>
	/// Détecte les murs ne sont plus en collision avec la caméra.
	/// </summary>
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<Wall>())
		{
			other.gameObject.GetComponent<Wall>().isTriggered = false;
		}
	}
}