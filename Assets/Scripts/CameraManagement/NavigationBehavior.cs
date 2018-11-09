using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attaché à l'objet "UserController" dans la scene "Creation_architecture".
/// Permet à l'utilisateur, lorsque la camera 1ere personne est active, de se déplacer dans les maisons en vue à la 
/// 1ere personne en utilisant uniquement sa souris.
/// Un clique gauche sur le sol lui permet de se déplacer.
/// Un clique droit maintenu permet de faire tourner la camera.
/// La touche F permet de simuler une chute.
/// La touche R permet ensuite de se redresser.
/// </summary>
public class NavigationBehavior : MonoBehaviour {

	//référence à lui-même
	static NavigationBehavior instance;

	//cible au sol
	public GameObject horizontalTargetPrefab, verticalTargetPrefab;
	GameObject horizontalTarget, verticalTarget;
	Transform t_horizontalTarget;
	MeshRenderer m_horizontalTarget, m_verticalTarget;

	//position et rotation
	public Camera cameraPlayer, renderCameraPlayer;
	private Transform Tcamera;
	public NavMeshAgent agent;
	private Transform target;
	private float yaw;
	private float pitch;

	//texte d'utilisation d'objets connectés
	Device watchedSensor;
	public Text data;
	public DescriptionOnMouseOver description;
	bool clicked;

	//chute
	bool falling, recovering;
	float fallTimer;
	public HealthParameter fallPrm;
	int fallHour;

	/// <summary>
	/// Crée une référence unique à la classe dans le projet.
	/// </summary>
	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Etablit les references et instancie les cibles.
	/// </summary>
	void Start()
	{
		horizontalTarget = Instantiate(horizontalTargetPrefab);
		verticalTarget = Instantiate(verticalTargetPrefab);
		t_horizontalTarget = horizontalTarget.transform;
		m_horizontalTarget = horizontalTarget.GetComponent<MeshRenderer>();
		m_verticalTarget = verticalTarget.GetComponent<MeshRenderer>();
		Tcamera = cameraPlayer.GetComponent<Transform>();
		target = GetComponent<Transform>();
		pitch = Tcamera.eulerAngles.x;
		if (180f >= pitch && pitch > 0f)
		{
			pitch = Mathf.Clamp(pitch, 0f, 50f);
		}
		if (360f >= pitch && pitch > 180f)
		{
			pitch = Mathf.Clamp(pitch, 320f, 360f);
		}
		yaw = Tcamera.eulerAngles.y;
	}

	/// <summary>
	/// Appelée à chaque frame.
	/// Si la camera 1ere personne est active, gere les inputs de l'utilisateur.
	/// </summary>
	void Update()
	{
		m_horizontalTarget.enabled = false;
		m_verticalTarget.enabled = false;

		if (cameraPlayer.enabled)
		{
			if (!falling && !recovering)
			{
				agent = GetComponent<NavMeshAgent>();
				agent.updateRotation = true;
				pitch = Tcamera.eulerAngles.x;
				if (180f >= pitch && pitch > 0f)
				{
					pitch = Mathf.Clamp(pitch, 0f, 50f);
				}
				if (360f >= pitch && pitch > 180f)
				{
					pitch = Mathf.Clamp(pitch, 320f, 360f);
				}
				yaw = Tcamera.eulerAngles.y;
				Tcamera.position = target.position;
				Ray ray = renderCameraPlayer.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				// trace un rayon dans l'axe de la souris qui renvoie dans la variable hit le premier collider touché par le
				// rayon
				if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
				{
					// si l'objet touché est le sol, place une cible sous la souris et détermine la destination de
					// l'utilisateur s'il clique avec le bouton gauche de la souris
					if (hit.collider.gameObject.tag == "Floor")
					{
						m_horizontalTarget.enabled = true;
						t_horizontalTarget.position = hit.point;
						if (Input.GetMouseButtonDown(0))
						{
							agent.ResetPath();
							agent.destination = hit.point;
						}
					}
					// si l'objet touché est un capteur, active ses composants outline et affiche la mesure effectuée 
					// par le capteur
					if (hit.collider.GetComponent<Device>() || hit.collider.GetComponentInParent<Device>())
					{
						Device newSensor;
						if (hit.collider.GetComponent<Device>())
						{
							newSensor = hit.collider.GetComponent<Device>();
						}
						else
						{
							newSensor = hit.collider.GetComponentInParent<Device>();
						}
						if (watchedSensor)
						{
							if (watchedSensor.transform != newSensor.transform)
							{
								watchedSensor.EnableOutline(false);
								watchedSensor = newSensor;
								if (watchedSensor.Data() == "")
								{
									watchedSensor.EnableOutline(true, 2);
									description.Disactive();
								}
								else
								{
									watchedSensor.EnableOutline(true, 1);
									description.Active("Clique gauche pour utiliser");
								}
							}
						}
						else
						{
							watchedSensor = newSensor;
							watchedSensor.EnableOutline(true);
							if (watchedSensor.Data() == "")
							{
								watchedSensor.SetOutlineColor(2);
							}
							else
							{
								watchedSensor.SetOutlineColor(1);
								description.Active("Clique gauche pour utiliser");
							}
						}
					}
					else
					{
						Clear();
						description.Disactive();
					}
				}
				else
				{
					Clear();
				}

				// si l'utilisateur maintient le bouton droit de la souris tout en bougeant la souris, fais tourner la
				// camera
				if (Input.GetMouseButton(1))
				{

					m_horizontalTarget.enabled = false;
					m_verticalTarget.enabled = false;
					agent.updateRotation = false;
					yaw -= ApplicationParameters.speedX * Input.GetAxis("Mouse X");
					pitch += ApplicationParameters.speedY * Input.GetAxis("Mouse Y");
					if (180f >= pitch && pitch > 0f)
					{
						pitch = Mathf.Clamp(pitch, 0f, 50f);
					}
					if (360f >= pitch && pitch > 180f)
					{
						pitch = Mathf.Clamp(pitch, 320f, 360f);
					}
					Tcamera.eulerAngles = new Vector3(pitch, yaw, 0.0f);
					target.rotation = 
						Quaternion.Euler(target.rotation.eulerAngles.x, Tcamera.rotation.eulerAngles.y, 0);
				}
				if (Input.GetMouseButton(0) && watchedSensor && !EventSystem.current.IsPointerOverGameObject())
				{
					if (!clicked)
					{
						SetSensorText();
						clicked = true;
					}
				}
				else
				{
					clicked = false;
				}
				//l'appui sur la touche f provoque la chute de l'utilisateur
				if (Input.GetKeyDown("f"))
				{
					falling = true;
					m_horizontalTarget.enabled = false;
					m_verticalTarget.enabled = false;
					agent.updateRotation = false;
					agent.isStopped = true;
					agent.ResetPath();
					Clear();
					description.Disactive();
					data.text = "Appuyez sur 'R' pour vous relever";
					if (fallPrm.IsMeasured())
					{
						CharacterStorage.RefreshCharactersData(fallPrm, TimeClock.GetInstant(), 1);
					}
					fallHour = TimeClock.Hour;
				}
			}
			//l'avatar chute, ce qui bloque tout déplacement. La caméra simule un mouvement de chute
			if (falling)
			{
				fallTimer += Time.deltaTime * 2f;
				float angle = Mathf.LerpAngle(0f, -90.0f, fallTimer);
				float fallHeight = Mathf.Lerp(target.position.y, 0.50f + target.position.y - 4f, fallTimer);
				Tcamera.rotation = 
					Quaternion.Euler(Tcamera.rotation.eulerAngles.x, Tcamera.rotation.eulerAngles.y, angle);
				Tcamera.position = new Vector3(Tcamera.position.x, fallHeight, Tcamera.position.z);
				target.position = new Vector3(Tcamera.position.x, target.position.y, Tcamera.position.z);
				if (fallHour != TimeClock.Hour)
				{
					CharacterStorage.RefreshCharactersData(fallPrm, TimeClock.GetInstant(), 1);
				}
				if (Input.GetKeyDown("r") && fallTimer > 1)
				{
					data.text = "";
					fallTimer = 0;
					falling = false;
					recovering = true;
				}
			}
			//l'avatar se redresse, ce qui bloque tout déplacement. La caméra simule le redressement de l'avatar
			else if (recovering)
			{
				fallTimer += Time.deltaTime * 2f;
				float angle = Mathf.LerpAngle(-90f, -0f, fallTimer);
				float fallHeight = Mathf.Lerp(0.50f + target.position.y - 4f, target.position.y, fallTimer);
				Tcamera.rotation = 
					Quaternion.Euler(Tcamera.rotation.eulerAngles.x, Tcamera.rotation.eulerAngles.y, angle);
				Tcamera.position = new Vector3(Tcamera.position.x, fallHeight, Tcamera.position.z);
				target.position = new Vector3(Tcamera.position.x, target.position.y, Tcamera.position.z);
				if (fallTimer > 1)
				{
					fallTimer = 0;
					recovering = false;
				}
			}
		}
	}

	/// <summary>
	/// Affiche les données prelevées par le capteur visé par l'utilisateur lorsque l'utilisateur clique dessus.
	/// </summary>
	void SetSensorText()
	{
		data.text = watchedSensor.Data();
		description.Disactive();
	}

	/// <summary>
	/// Efface le texte decrivant les mesures effectuées.
	/// Desactive le outline du capteur puis annule la reference au capteur.
	/// </summary>
	public static void Clear()
	{
		if (instance.watchedSensor)
		{
			instance.watchedSensor.EnableOutline(false);
			instance.watchedSensor = null;
			instance.data.text = "";
		}
	}

	/// <summary>
	/// Ajoute le composant agent à l'objet si celui-ci n'existe pas. Appelée à chaque fois que l'utilisateur passe en
	/// vision à la 1ere personne.
	/// </summary>
	public void AddAgentComponent()
	{
		if (GetComponent<NavMeshAgent>() == null)
		{
			agent = gameObject.AddComponent<NavMeshAgent>() as NavMeshAgent;
			agent.speed = 8f;
			agent.baseOffset = 4f;
			agent.stoppingDistance = 1f;
			agent.acceleration = 15f;
		}
	}

	/// <summary>
	/// Supprime le composant agent de l'objet. Appelée lors du chargement d'une maison.
	/// </summary>
	public static void RemoveAgentComponent()
	{
		if (instance.agent)
		{
			Destroy(instance.GetComponent<NavMeshAgent>());
			instance.agent = null;
		}
	}

	/// <summary>
	/// Active ou désactive ce script. Appelée lors de l'ouverture ou de la fermeture du menu.
	/// </summary>
	public static bool Locked
	{
		set
		{
			if (instance.agent)
			{
				instance.agent.enabled = !value;
			}
			instance.enabled = !value;
			instance.transform.position = instance.Tcamera.position;
		}
	}
}
