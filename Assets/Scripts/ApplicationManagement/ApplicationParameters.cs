using UnityEngine;

/// <summary>
/// Utilisée dans les deux scènes du simulateur.
/// Elle permet de choisir les paramètres de l'application. Certains doivent rester constant comme le frame rate,
/// d'autres puevent être modifiés par l'utilisateur.
/// Les paramètres peuvent être changés dans le menu.
/// </summary>
public class ApplicationParameters : MonoBehaviour {

	//options
	public static bool descriptionOn, debugModeOn, halfResolutionOn;
	public static int speedX, speedY;

	//référence à lui-même
	public static ApplicationParameters instance;
	static bool created;

	//date de début
	public static int startHour, startDay, startMonth, startYear, startDaysLength;

	/// <summary>
	/// Fixe les paramètres.
	/// </summary>
	void Awake()
	{
		if (!created)
		{
			DontDestroyOnLoad(gameObject);
			created = true;
			instance = this;
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			descriptionOn = true;
			debugModeOn = false;
			halfResolutionOn = true;
			speedX = 5;
			speedY = 3;
			startHour = 9;
			startDay = 24;
			startMonth = 7;
			startYear = 2018;
			startDaysLength = 731;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Réinitialise les paramètres.
	/// </summary>
	public static void Reset()
	{
		descriptionOn = true;
		debugModeOn = false;
		halfResolutionOn = true;
		speedX = 5;
		speedY = 3;
	}
}
