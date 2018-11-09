using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Permet de changer les paramètres du simulateur.
/// </summary>
public class ParametersPanel : MonoBehaviour {

	//affichage des fonctions des boutons
	bool descriptionOn;
	public Toggle toggleDescription;

	//vitesse de la caméra à la première personne
	int speedX, speedY;
	public Slider sliderX, sliderY;
	public Text textX, textY;

	//affichage des statistiques
	bool debugModeOn;
	public Toggle toggleDebugMode;
	public GameObject debugModePanel;

	//résolution mode première personne
	bool halfResolutionOn;
	public Toggle toggleResolution;

	/// <summary>
	/// Demande l'initialisation du panneau.
	/// </summary>
	void Awake()
	{
		SetPanel();
	}

	/// <summary>
	/// Attribue aux paramètres leurs valeurs, données par "ApplicationParameters".
	/// </summary>
	void SetPanel()
	{
		descriptionOn = ApplicationParameters.descriptionOn;
		debugModeOn = ApplicationParameters.debugModeOn;
		halfResolutionOn = ApplicationParameters.halfResolutionOn;
		toggleDescription.isOn = descriptionOn;
		toggleDebugMode.isOn = debugModeOn;
		toggleResolution.isOn = halfResolutionOn;
		sliderX.value = ApplicationParameters.speedX;
		sliderY.value = ApplicationParameters.speedY;
	}

	/// <summary>
	/// Active ou désactive l'aide aux boutons.
	/// Appelée lors du changement de valeur de "toggleDescription".
	/// </summary>
	public void ChangeDescription()
	{
		descriptionOn = !descriptionOn;
	}

	public void ChangeDebugMode()
	{
		debugModeOn = !debugModeOn;
	}

	public void ChangeResolution()
	{
		halfResolutionOn = !halfResolutionOn;
	}

	/// <summary>
	/// Change la vitesse selon X de la caméra 1ere personne.
	/// Appelée lors du changement de valeur du slider "sliderX".
	/// </summary>
	public void ChangeSpeedX()
	{
		speedX = (int)sliderX.value;
		textX.text = speedX.ToString();
	}

	/// <summary>
	/// Change la vitesse selon Y de la caméra 1ere personne.
	/// Appelée lors du changement de valeur du slider "sliderY".
	/// </summary>
	public void ChangeSpeedY()
	{
		speedY = (int)sliderY.value;
		textY.text = speedY.ToString();

	}

	/// <summary>
	/// Active le panneau et attribue aux paramètres leurs valeurs.
	/// </summary>
	public void Open()
	{
		gameObject.SetActive(true);
		SetPanel();
	}

	/// <summary>
	/// Sauvegarde les changements effectués sur les paramètres.
	/// </summary>
	public void Save()
	{
		ApplicationParameters.descriptionOn = descriptionOn;
		ApplicationParameters.debugModeOn = debugModeOn;
		ApplicationParameters.halfResolutionOn = halfResolutionOn;
		if (SceneManager.GetActiveScene().name == "Creation_architecture")
		{
			debugModePanel.SetActive(debugModeOn);
		}
		ApplicationParameters.speedX = speedX;
		ApplicationParameters.speedY = speedY;
	}

	/// <summary>
	/// Réinitialise les paramètres.
	/// </summary>
	public void Reset()
	{
		ApplicationParameters.Reset();
		SetPanel();
	}
}