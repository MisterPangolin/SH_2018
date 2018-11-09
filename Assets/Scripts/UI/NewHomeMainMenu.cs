using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Associée au panneau de création de nouvelles maisons dans le menu d'accueil.
/// Permet de choisir le nom de la sauvegarde à utiliser, ainsi que la taille de la grille qui sera créée.
/// Un message d'erreur apparaît si les dimensions de la maison ne sont pas possibles, ou si un nom n'est pas entré.
/// Une fois les dimensions et le nom validé, le panneau d'édition des profils s'ouvre.
/// </summary>

public class NewHomeMainMenu : MonoBehaviour
{
	//dimensions
	int customX, customZ;
	public Slider sliderX, sliderZ;

	//nom de sauvegarde
	public InputField nameInput;
	string savedPath = PersistentStorage.savePathName;

	//édition des profils
	public CharacterEdition edition;

	//message d'erreur
	public ErrorPanel errorPanel;
	public OverwriteFilePanel overwriteFilePanel;

	/// <summary>
	/// Crée une nouvelle maison avec les dimensions choisies. Réinitialise les caméras et ouvre le panneau d'édition
	/// de profils.
	/// </summary>
	void CreateHome(int x, int z)
	{
		string homeName = nameInput.text;
		if (homeName.Length == 0)
		{
			errorPanel.Open("Veuillez entrer un nom de sauvegarde avant de créer la maison.", gameObject);
			return;
		}
		else if (PersistentStorage.ContainsExemple(homeName + ".home"))
		{
			errorPanel.Open("Le nom de fichier choisi est protégé." + "\n\nVeuillez choisir un autre nom.", gameObject);
			return;
		}
		else if (x == 0 || z == 0)
		{
			errorPanel.Open("Les dimensions entrées ne sont pas valides." + "\n\nVeuillez choisir d'autres dimensions.", 
			                gameObject);
			return;
		}
		if (PersistentStorage.CheckifExists(homeName + ".home"))
		{
			PersistentStorage.NewHome(homeName + ".home", x, z);
			edition.Open(true);
		}
		else
		{
			overwriteFilePanel.Open(edition, homeName + ".home", x, z, gameObject);
			return;
		}
	}

	/// <summary>
	/// Fonction du bouton "Retour" permettant de revenir au menu.
	/// Modifie le nom du fichier de sauvegarde à utiliser pour le nom précédent.
	/// </summary>
	public void Return()
	{
		PersistentStorage.ChangePathName(savedPath);
		gameObject.SetActive(false);
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
		sliderX.gameObject.GetComponentsInChildren<Text>()[0].text = "X : " + value * 2 * 4 + "m";
	}

	/// <summary>
	/// Permet de choisir la dimension z de la maison à créer.
	/// </summary>
	public void CustomZ()
	{
		int value = (int)sliderZ.value;
		customZ = value * 2;
		sliderZ.gameObject.GetComponentsInChildren<Text>()[0].text = "Z : " + value * 2 * 4 + "m";
	}

	/// <summary>
	/// Appelle la fonction "CreateHome" en définisant à customX x customZ les dimensions de la maison à créer.
	/// </summary>
	public void CreateCustomSizeHome()
	{
		CreateHome(customX, customZ);
	}
}
