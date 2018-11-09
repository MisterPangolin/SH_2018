using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permet de changer le texte de l'image située tout en haut de certains menus, permettant de les décrire.
/// </summary>
public class SetMenuTitle : MonoBehaviour {

	//texte du titre
	public Text title;

	/// <summary>
	/// Fonction attribuée à certains boutons permettant d'aller d'un menu à un autre, afin de changer le nom du menu
	/// en même temps.
	/// </summary>
	public void SetTitle(string newTitle)
	{
		title.text = newTitle;
	}
}
