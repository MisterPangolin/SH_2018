public enum eventType
{
	character, healthP, homeP, healthI, device, homePanel
}

/// <summary>
/// Créé lors du passage d'un panneau à un autre, pour avoir une trace du parcours de l'utilisateur et lui permettre
/// de revenir en arrière.
/// </summary>
public class PanelEvent
{
	/// <summary>
	/// type de panneau fermé.
	/// </summary>
	eventType type;
	public eventType Type
	{
		get
		{
			return type;
		}
	}

	/// <summary>
	/// profil fermé.
	/// </summary>
	Character character;
	public Character Character
	{
		get
		{
			return character;
		}
	}

	/// <summary>
	/// paramètre de santé fermé.
	/// </summary>
	HealthParameter healthP;
	public HealthParameter HealthP
	{
		get
		{
			return healthP;
		}
	}

	/// <summary>
	/// paramètre de maison fermé.
	/// </summary>
	HomeParameter homeP;
	public HomeParameter HomeP
	{
		get
		{
			return homeP;
		}
	}

	/// <summary>
	/// information de santé fermée.
	/// </summary>
	HealthInformation healthI;
	public HealthInformation HealthI
	{
		get
		{
			return healthI;
		}
	}

	/// <summary>
	/// objet connecté fermé.
	/// </summary>
	Device device;
	public Device Device
	{
		get
		{
			return device;
		}
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau des profils est fermé.
	/// </summary>
	public PanelEvent(Character c)
	{
		type = eventType.character;
		character = c;
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau des paramètres vitaux est fermé.
	/// </summary>
	public PanelEvent(HealthParameter parameter)
	{
		type = eventType.healthP;
		healthP = parameter;
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau des paramètres de maison est fermé.
	/// </summary>
	public PanelEvent(HomeParameter parameter)
	{
		type = eventType.homeP;
		homeP = parameter;
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau des informations médicales est fermé.
	/// </summary>
	public PanelEvent(HealthInformation information)
	{
		type = eventType.healthI;
		healthI = information;
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau des objets connectés est fermé.
	/// </summary>
	public PanelEvent(Device d)
	{
		type = eventType.device;
		device = d;
	}

	/// <summary>
	/// Initialise une nouvelle instance de cette classe lorsque le panneau de la maison est fermé.
	/// </summary>
	public PanelEvent()
	{
		type = eventType.homePanel;
	}
}
