using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Composant du bouton des fichiers de sauvegarde.
/// </summary>
public class SaveLoadItem : NetworkBehaviour {

	//nom du fichier de sauvegarde
	string homeName;

	//références à d'autres scripts
	public EditorManagement editorMenu;
	public LaunchMenu mainMenu;

	/// <summary>
	/// Renvoie le nom du fichier de sauvegarde ou le modifie.
	/// </summary>
	public string HomeName
	{
		get
		{
			return homeName;
		}
		set
		{
			homeName = value;
			transform.GetChild(0).GetComponent<Text>().text = value;
		}
	}

	/// <summary>
	/// Selectionne le fichier de sauvegarde.
	/// </summary>
	public void Select()
	{
		if (editorMenu)
		{
			editorMenu.SelectItem(homeName);
		}
		else
		{
			mainMenu.SelectItem(homeName);
		}
	}
}
