using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Associée au panneau de création de nouvelles maisons lorsqu'une maison est déjà créée.
/// Permet de choisir le nom de la sauvegarde à utiliser, ainsi que la taille de la grille qui sera créée.
/// </summary>

public class NewHome : MonoBehaviour
{
	//éditeur
	public HomeEditor editor;

	//références au menu
	public GameObject menuCanvas;
	public GameObject menuPanel;

	//dimensions
	int customX, customZ;
	public Slider sliderX, sliderZ;

	/// <summary>
	/// Ouvre le menu de création de nouvelle maison. Désactive les caméras et le défilement du temps.
	/// </summary>
	public void Open()
	{
		menuCanvas.SetActive(true);
		menuPanel.SetActive(false);
		gameObject.SetActive(true);
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Ferme le menu de création de nouvelle maison. Active les caméras et le défilement du temps.
	/// </summary>
	public void Close()
	{
		menuPanel.SetActive(true);
		menuCanvas.SetActive(false);
		gameObject.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Crée une nouvelle maison avec les dimensions choisies. Réinitialise les caméras et ouvre le panneau d'édition
	/// de profils.
	/// </summary>
	void CreateHome(int x, int z)
	{
		DeviceStorage.Clear();
		editor.CreateHome(x, z);
		CameraSwitch.Load();
		Close();
	}

	/// <summary>
	/// Définit à 6x4 les dimensions de la maison à créer, et appelle la fonction "CreateHome".
	/// </summary>
	public void CreateSmallHome()
	{
		CreateHome(6, 4);
	}

	/// <summary>
	/// Définit à 8x6 les dimensions de la maison à créer, et appelle la fonction "CreateHome".
	/// </summary>
	public void CreateMediumHome()
	{
		CreateHome(8, 6);
	}

	/// <summary>
	/// Définit à 10x8 les dimensions de la maison à créer, et appelle la fonction "CreateHome".
	/// </summary>
	public void CreateLargeHome()
	{
		CreateHome(10, 8);
	}

	/// <summary>
	/// Permet de choisir la dimension x de la maison à créer.
	/// </summary>
	public void CustomX()
	{
		int value = (int)sliderX.value;
		customX = value * 2;
		sliderX.gameObject.GetComponentsInChildren<Text>()[0].text = "X : " + value * 2;
	}

	/// <summary>
	/// Permet de choisir la dimension z de la maison à créer.
	/// </summary>
	public void CustomZ()
	{
		int value = (int)sliderZ.value;
		customZ = value * 2;
		sliderZ.gameObject.GetComponentsInChildren<Text>()[0].text = "Z : " + value * 2;
	}

	/// <summary>
	/// Appelle la fonction "CreateHome" en définisant à customX x customZ les dimensions de la maison à créer.
	/// </summary>
	public void CreateCustomSizeHome()
	{
		CreateHome(customX, customZ);
	}
}
