using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utilisée pour la modification des paramètres du simulateur afin d'expliquer à l'utilisateur les changements qu'il
/// peut effectuer.
/// Ajoutée en composant aux boutons en forme de points d'interrogation.
/// </summary>

public class ParameterDescription : MonoBehaviour {

	//gameObject du message
	public GameObject descriptionPrefab;
	GameObject description;
	Image image;
	Text text;

	//transform du panneau des paramètres, servant de parent au message
	public Transform canvas;

	//message descriptif
	[TextArea(4, 6)]
	public string message;

	//coroutine
	IEnumerator coroutine;
	float i = 0f;

	/// <summary>
	/// Stoppe la coroutine si elle existait et détruit le message précédent.
	/// Instancie un nouveau message correspondant à la fonction visée.
	/// </summary>
	public void Active()
	{
		if (description)
		{
			StopCoroutine(coroutine);
			Destroy(description);
		}
		description = Instantiate(descriptionPrefab);
		text = description.GetComponentInChildren<Text>();
		text.text = message;
		image = description.GetComponent<Image>();
		description.transform.SetParent(canvas, false);
		description.transform.position = transform.position;
		coroutine = FadeImage(false);
		StartCoroutine(coroutine);
	}

	/// <summary>
	/// Lance la coroutine pour faire disparaître le message lentement.
	/// </summary>
	public void Disactive()
	{
		StartCoroutine(FadeImage(true));
	}

	/// <summary>
	/// Coroutine permettant d'afficher ou de faire disparaître le message. "fadeAway" donne le mode d'affichage.
	/// Si true, le message disparaît, sinon, il apparaît.
	/// </summary>
	IEnumerator FadeImage(bool fadeAway)
	{
		if (fadeAway)
		{
			for (float u = i; u >= 0; u -= 2 * Time.deltaTime)
			{
				i = u;
				if (image && text)
				{
					image.color = new Color(1, 1, 1, u);
					text.color = new Color(0, 0, 0, u);
					if (u < 0.1)
					{
						Destroy(description);
					}
				}
				yield return null;
			}
		}
		else
		{
			for (float u = i; u <= 1; u += 2 * Time.deltaTime)
			{
				i = u;
				if (image && text)
				{
					image.color = new Color(1, 1, 1, u);
					text.color = new Color(0, 0, 0, u);
				}
				yield return null;
			}
		}
	}
}
