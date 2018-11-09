using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permet d'afficher une information de santé dans un panneau.
/// Mis en composant de l'objet "ChronicsPanel" du hub.
/// </summary>
/// 
public class ChronicsPanel : MonoBehaviour {

	//champs de texte
	public Text conditionName;
	public Text conditionDescription;
	public Text management;

	//affichage des paramètres
	public Transform healthPLayout, homePLayout;
	public GameObject healthButton, redHealthButton;

	//référence au panneau descriptif des paramètres
	public HealthParameterPanel healthPanel;


	/// <summary>
	/// Active le panneau et change les valeurs des champs pour afficher les informations de "chronic".
	/// Bloque les cameras et le défilement du temps.
	/// </summary>
	public void Open(HealthInformation chronic)
	{
		gameObject.SetActive(true);
		conditionName.text = chronic.name;
		conditionDescription.text = chronic.description;
		ClearHealthParameterField();
		ClearHomeParameterField();
		SetHealthParameterField(chronic);
		SetHomeParameterField(chronic);
		management.text = chronic.management;
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Désactive le panneau et active les caméras et le défilement du temps.
	/// </summary>
	public void Close()
	{
		ClearHealthParameterField();
		ClearHomeParameterField();
		gameObject.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Instancie les boutons des paramètres de santé concernant "chronic", puis les place en enfant de l'objet 
	/// "healthPLayout".
	/// </summary>
	void SetHealthParameterField(HealthInformation condition)
	{
		HealthParameter[] parameters = condition.healthP;
		for (int i = 0; i < parameters.Length; i++)
		{
			var i2 = i;
			GameObject b;
			if (parameters[i2].IsMeasured())
			{
				b = parameters[i2].InstantiateHealthButton(healthButton);
			}
			else
			{
				b = parameters[i2].InstantiateHealthButton(redHealthButton);
			}
			b.transform.SetParent(healthPLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenDescriptionOnClick(parameters[i2]); 
				EventsStorage.Push(new PanelEvent(condition)); });
		}
	}

	/// <summary>
	/// Détruit les boutons de l'objet "healthPLayout".
	/// </summary>
	void ClearHealthParameterField()
	{
		int count = healthPLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(healthPLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Instancie les boutons des paramètres de maison concernant "chronic", puis les place en enfant de l'objet 
	/// "homePLayout".
	/// </summary>
	void SetHomeParameterField(HealthInformation chronic)
	{
		HomeParameter[] parameters = chronic.homeP;
		for (int i = 0; i < parameters.Length; i++)
		{
			var i2 = i;
			GameObject b;
			if (parameters[i2].IsFollowed())
			{
				b = parameters[i2].InstantiateHomeButton(healthButton);
			}
			else
			{
				b = parameters[i2].InstantiateHomeButton(redHealthButton);
			}
			b.transform.SetParent(homePLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate{OpenDescriptionOnClick(parameters[i2]);
				EventsStorage.Push(new PanelEvent(chronic));});
		}
	}

	/// <summary>
	/// Détruit les boutons de l'objet "homePLayout".
	/// </summary>
	void ClearHomeParameterField()
	{
		int count = homePLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(homePLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Fonction attribuée aux boutons des paramètres de santé lors de leur création.
	/// Ferme le panneau et en ouvre un nouveau pour afficher les informations du paramètre choisi.
	/// </summary>
	void OpenDescriptionOnClick(HealthParameter parameter)
	{
		Close();
		healthPanel.Open(parameter);
	}

	/// <summary>
	/// Fonction attribuée aux boutons des paramètres de maison lors de leur création.
	/// Ferme le panneau et en ouvre un nouveau pour afficher les informations du paramètre choisi.
	/// </summary>
	void OpenDescriptionOnClick(HomeParameter parameter)
	{
		Close();
		healthPanel.Open(parameter);
	}
}
