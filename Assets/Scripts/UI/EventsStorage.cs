using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enregistre l'ordre d'ouverture des panneaux, et leurs contenus, afin de pouvoir revenir en arrière sans avoir à
/// fermer les panneaux à chaque fois, certains panneaux ne pouvant pas être atteints une fois quittés sans ce script.
/// Si l'utilisateur ferme un panneau grâce aux boutons en forme de croix situés en bas à droite, les évènements sont
/// supprimés et la pile réinitialisée.
/// </summary>
/// 
public class EventsStorage : MonoBehaviour {

	//référence à lui-même
	static EventsStorage instance;

	//pile d'évènements
	Stack<PanelEvent> events = new Stack<PanelEvent>();

	//références aux panneaux
	public CharacterPanel characterPanel;
	public HealthParameterPanel parametersPanel;
	public ChronicsPanel informationPanel;
	public DevicePanel devicePanel;
	public HomePanel homePanel;

	//liste des boutons permettant le retour arrière
	public List<GameObject> buttons;

	/// <summary>
	/// Désactive les boutons de retour arrière puisqu'aucun panneau ne fut ouvert.
	/// </summary>
	void Awake()
	{
		instance = this;
		foreach (GameObject b in buttons)
		{
			b.SetActive(false);
		}
	}

	/// <summary>
	/// Ajoute un évènement dans la pile.
	/// Active les boutons pour permettre le retour arrière.
	/// </summary>
	public static void Push(PanelEvent e)
	{
		instance.events.Push(e);
		foreach (GameObject b in instance.buttons)
		{
			b.SetActive(true);
		}
	}

	/// <summary>
	/// Interprète le dernier évènement, en ouvrant le dernier panneau fermé avec les informations qui y étaient
	/// affichées.
	/// </summary>
	static void Pop()
	{
		PanelEvent e = instance.events.Pop();
		switch (e.Type)
		{
			case eventType.character:
				instance.characterPanel.Open(e.Character);
				break;
			case eventType.healthP:
				instance.parametersPanel.Open(e.HealthP);
				break;
			case eventType.homeP:
				instance.parametersPanel.Open(e.HomeP);
				break;
			case eventType.healthI:
				instance.informationPanel.Open(e.HealthI);
				break;
			case eventType.device:
				instance.devicePanel.Open(e.Device);
				break;
			default:
				instance.events = new Stack<PanelEvent>();
				instance.homePanel.Open();
				break;
		}
		if (instance.events.Count == 0)
		{
			foreach (GameObject b in instance.buttons)
			{
				b.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Fonction donnée aux boutons pour permettre le retour arrière.
	/// </summary>
	public void GetPrevEvent()
	{
		Pop();
	}
}
