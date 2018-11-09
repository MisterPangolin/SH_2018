using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permet d'afficher une description des boutons visés par la souris de l'utilisateur, afin de comprendre ce qui sera
/// executé lors du clique.
/// C'est une option que l'utilisateur peut désactiver.
/// </summary>
public class DescriptionOnMouseOver : MonoBehaviour
{
	//état de l'objet
	public bool active;

	//transform de l'objet
	public RectTransform t;

	//texte
	public Text text;

	/// <summary>
	/// Active le texte de description et change son texte pour correspondre au bouton visé.
	/// Met à jour sa position.
	/// </summary>
	public void Active(string newText)
	{
		if (ApplicationParameters.descriptionOn)
		{
			t.gameObject.SetActive(true);
			active = true;
			text.text = newText;
			t.position = Input.mousePosition + new Vector3(t.sizeDelta.x, t.sizeDelta.y, 0f);
			SetPosition();
		}
	}

	/// <summary>
	/// Désactive le texte.
	/// Appelé lorsque l'utilisateur ne vise plus un bouton.
	/// </summary>
	public void Disactive()
	{
		active = false;
		text.text = "";
		t.gameObject.SetActive(false);
	}

	/// <summary>
	/// Si le texte est en cours d'affichage, met à jour sa position pour correspondre à la position de la souris.
	/// </summary>
	void Update()
	{
		if (active && ApplicationParameters.descriptionOn)
		{
			t.position = Input.mousePosition + new Vector3(t.sizeDelta.x,t.sizeDelta.y,0f);
			SetPosition();
		}
	}

	/// <summary>
	/// Change la position du texte. Le texte apparaît en haut à droite du curseur.
	/// Si le curseur de la souris se situe en haut et/ou à droite de l'écran, le texte apparaît alors en bas à gauche
	/// du curseur.
	/// </summary>
	void SetPosition()
	{
		Vector3[] v = new Vector3[4];
		t.GetWorldCorners(v);
		float maxX = Mathf.Max(v[0].x, v[1].x, v[2].x, v[3].x);
		float maxY = Mathf.Max(v[0].y, v[1].y, v[2].y,v[3].y);
		if (maxY > Screen.height || maxX > Screen.width)
		{
			t.position = Input.mousePosition + new Vector3(-t.sizeDelta.x, -t.sizeDelta.y, 0f);
		}
	}
}
