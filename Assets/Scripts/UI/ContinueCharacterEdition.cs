using UnityEngine;
using UnityEngine.UI;

//mode d'ouverture du panneau
public enum continueMode { addNew, close, validate, validateAndReturn, validateAndNew};

/// <summary>
/// Classe associée au panneau "Continue" dans les interfaces relatives aux profils afin d'indiquer à l'utilisateur
/// les actions qui seront effectuées après validation si le profil qu'il était en train de créer/modifier n'est pas
/// complet.
/// </summary>
/// 
public class ContinueCharacterEdition : MonoBehaviour {

	//référence au panneau d'édition
	public CharacterEdition edition;

	//mode d'affichage du panneau
	continueMode mode;

	//message du panneau 
	public Text text;

	/// <summary>
	/// Ouvre le panneau "Continue" en affichant un message déterminé par le paramètre "newMode".
	/// Le paramètre "message" est donné par la classe CharacterEdition selon les champs qu'ils restent à remplir.
	/// </summary>
	public void Open(continueMode newMode, string message = "")
	{
		mode = newMode;
		switch (mode)
		{
			//L'utilisateur veut ajouter un profil sans avoir complété le profil en cours
			case continueMode.addNew:
				text.text = "\nLe profil en cours est incomplet. En continuant, vous allez supprimer ce profil.\n" +
					message + "\n\n" +
					"Voulez-vous continuer?";
				break;
			//L'utilisateur veut fermer la fenêtre d'édition sans avoir complété le profil en cours
			case continueMode.close:
				text.text = "\nEn quittant, vous allez supprimer les profils créés.\n\n" +
					"Voulez-vous continuer?";
				break;
			//L'utilisateur veut valider des modifications de profils sans avoir complété le profil en cours
			case continueMode.validateAndReturn:
			//L'utilisateur veut créer une nouvelle maison, à partir d'une maison déjà existante, sans avoir complété le 
			//profil en cours
			case continueMode.validateAndNew:
				text.text = "\nLe profil en cours est incomplet. En continuant, vous allez supprimer ce profil et " +
					"accéder à la maison.\n" +
					message + "\n\n" + 
					"Voulez-vous continuer?";
				break;
				//L'utilisateur veut créer une nouvelle maison, à partir du menu d'accueil, sans avoir complété le
				//profil en cours
			default:
				text.text = "\nLe profil en cours est incomplet. En continuant, vous allez supprimer ce profil et accéder" +
					" à la maison. Vous pourrez modifier ou ajouter des profils plus tard.\n" + 
					message + "\n\n" +
					"Voulez-vous continuer?";
				break;
		}
		gameObject.SetActive(true);
	}

	/// <summary>
	/// A ajouter aux boutons affichant "Oui" et "Non" du panneau, afin de fermer le panneau.
	/// "continueToggle" est négatif pour le bouton "Non". Cliquer dessus permet de revenir au panneau d'édition.
	/// "continueToggle" est positif pour le bouton "Oui". CLiquer dessus permet de quitter le mode d'édition. Ce qui
	/// s'effectue ensuite depend de "mode".
	/// </summary>
	public void Close(bool continueToggle)
	{
		if (continueToggle)
		{
			edition.EraseCurrentCharacter();
			if (mode == continueMode.validate)
			{
				edition.LoadScene();
			}
			else if (mode == continueMode.close)
			{
				edition.Close();
			}
			else if (mode == continueMode.validateAndReturn)
			{
				edition.CloseAndAdd();
			}
			else if (mode == continueMode.validateAndNew)
			{
				edition.CloseAndChange();
			}
		}
		gameObject.SetActive(false);
	}
}
