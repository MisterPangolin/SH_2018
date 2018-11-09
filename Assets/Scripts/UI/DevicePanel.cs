using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Associée au panneau de description des capteurs.
/// Définit les champs du panneau selon le capteur selectionné.
/// </summary>
public class DevicePanel : MonoBehaviour
{
	
	//champs de texte
	public Text layoutTitle;
	public Text objectName;
	public Text objectDescription;
	public Text objectIndications;

	//paramètres
	public Transform parameterLayout;
	public GameObject healthButton, redHealthButton;

	//panneau des paramètres
	public HealthParameterPanel healthParameterPanel;

	//liste des profils
	public CharacterStorage characterStorage;

	/// <summary>
	/// Active le panneau.
	/// Change les champs du panneau pour afficher les informations du capteur "sensor".
	/// Bloque les caméras.
	/// </summary>
	public void Open(Device device)
	{
		layoutTitle.text = "Paramètres suivis";
		SetCommonFields(device);
		SetParametersField(device);
	}

	/// <summary>
	/// Change les textes en fonction de l'objet connecté selectionné.
	/// Désactive les caméras et le déroulement du temps.
	/// </summary>
	void SetCommonFields(Device device)
	{
		gameObject.SetActive(true);
		objectName.text = device.objectName;
		objectDescription.text = device.objectDescription;
		objectIndications.text = device.objectIndications;
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Réinitialise le champ des paramètres suivis.
	/// Désactive le panneau.
	/// Active les caméras et le déroulement du temps.
	/// </summary>
	public void Close()
	{
		ClearParametersField();
		gameObject.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Instancie pour chaque paramètre suivi par le capteur un bouton dans le champ "healthLayout".
	/// Le bouton est vert si le paramètre est suivi par les capteurs présents dans la maison, rouge sinon.
	/// </summary>
	void SetParametersField(Device device)
	{
		HealthParameter[] healthP = device.healthParameters;
		for (int i = 0; i < healthP.Length; i++)
		{
			var i2 = i;
			GameObject b;
			if (healthP[i2].IsMeasured())
			{
				b = healthP[i2].InstantiateHealthButton(healthButton);
			}
			else
			{
				b = healthP[i2].InstantiateHealthButton(redHealthButton);
			}
			b.transform.SetParent(parameterLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenHealthDescriptionOnClick(healthP[i2]); 
				EventsStorage.Push(new PanelEvent(device)); });
		}
		HomeParameter[] homeP = device.homeParameters;
		for (int i = 0; i < homeP.Length; i++)
		{
			var i2 = i;
			GameObject b;
			if (homeP[i2].IsFollowed())
			{
				b = homeP[i2].InstantiateHomeButton(healthButton);
			}
			else
			{
				b = homeP[i2].InstantiateHomeButton(redHealthButton);
			}
			b.transform.SetParent(parameterLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenFunctionDescriptionOnClick(homeP[i2]); 
				EventsStorage.Push(new PanelEvent(device)); });
		}
	}

	/// <summary>
	/// Détruit les boutons du champ "healthLayout".
	/// </summary>
	void ClearParametersField()
	{
		int count = parameterLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(parameterLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton de paramètre de santé pour ouvrir le panneau décrivant le paramètre 
	/// selectionné.
	/// </summary>
	void OpenHealthDescriptionOnClick(HealthParameter prm)
	{
		Close();
		healthParameterPanel.Open(prm);
	}

	/// <summary>
	/// Fonction attribuée à chaque bouton de paramètre de maison pour ouvrir le panneau décrivant le paramètre 
	/// selectionné.
	/// </summary>
	void OpenFunctionDescriptionOnClick(HomeParameter prm)
	{
		Close();
		healthParameterPanel.Open(prm);
	}
}
