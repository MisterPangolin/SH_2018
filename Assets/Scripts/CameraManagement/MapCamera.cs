using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attachée à la camera du mode Edition.
/// Dirige la camera selon les inputs de l'utilisateur.
/// </summary>
public class MapCamera : MonoBehaviour {

	//référence à lui-même
	static MapCamera instance;

	//état de la caméra
	bool active;

	//éditeur et étages
	HomeEditor editor;
	FloorGrid grid;

	//rotation
	Transform swivel, stick;
	float zoom = 1f;
	public float stickMinZoom, stickMaxZoom;
	public float swivelMinZoom, swivelmaxZoom;
	public float moveSpeedMinZoom, moveSpeedMaxZoom;
	public float rotationSpeed, speedManual;

	/// <summary>
	/// Etablit les references aux objets "swivel" et "stick".
	/// Crée une référence unique à la classe dans le projet.
	/// </summary>
	void Awake()
	{
		editor = GameObject.Find("Editor").GetComponent<HomeEditor>();
		instance = this;
		active = true;
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}

	/// <summary>
	/// Change la position de la caméra en traitant les Input donnés par l'utilisateur.
	/// </summary>
	void Update()
	{
		if (active)
		{
			grid = editor.CurrentFloor;
			transform.position = new Vector3(transform.position.x, grid.level * 5f, transform.position.z);
			float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
			if (zoomDelta != 0f && !EventSystem.current.IsPointerOverGameObject())
			{
				AdjustZoom(zoomDelta);
			}

			float rotationDelta = Input.GetAxis("Rotation");
			if (rotationDelta != 0f)
			{
				AdjustRotation(rotationDelta);
			}

			float xDelta = Input.GetAxis("Horizontal");
			float zDelta = Input.GetAxis("Vertical");
			if (xDelta != 0f || zDelta != 0f)
			{
				AdjustPosition(xDelta, zDelta);
			}
			if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
			{
				float delta = Input.GetAxis("Mouse X") * speedManual * Time.deltaTime;
				AdjustRotation(delta);
			}
		}
	}

	/// <summary>
	/// Ajuste le zoom de la caméra selon un delta donné.
	/// </summary>
	void AdjustZoom(float delta)
	{
		zoom = Mathf.Clamp01(zoom + delta);
		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelmaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

	/// <summary>
	/// Ajuste la position de la caméra selon des deltas donnés.
	/// </summary>
	void AdjustPosition(float xDelta, float zDelta)
	{
		Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = Mathf.Lerp(moveSpeedMinZoom,moveSpeedMaxZoom,zoom) * damping * Time.deltaTime;
		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = ClampPosition(position);
	}

	/// <summary>
	/// Méthode utilisée pour s'assurer que la position de la caméra reste dans un intervale défini.
	/// </summary>
	Vector3 ClampPosition(Vector3 position)
	{
		float xMax =( grid.cellCountX -0.5f) * (2f * Metrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f - 2f, xMax + 2f);
		float zMax = (grid.cellCountZ -0.5f) * (2f * Metrics.innerRadius);
		position.z = Mathf.Clamp(position.z, 0f - 2f, zMax + 2f);
		return position;
	}

	/// <summary>
	/// Ajuste la rotation de la caméra selon un delta donné.
	/// </summary>
	float rotationAngle;
	void AdjustRotation(float delta)
	{
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f)
		{
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f)
		{
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}

	/// <summary>
	/// Passe d'un mode de rotation à un autre.
	/// </summary>
	public void SwitchMotion()
	{
		active = !active;
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
	/// Réinitialise la position de la caméra. Appelée lors de la création d'une nouvelle maison.
	/// </summary>
	public static void ValidatePosition()
	{
		instance.grid = instance.editor.CurrentFloor;
		instance.AdjustPosition(0f, 0f);
	}
}