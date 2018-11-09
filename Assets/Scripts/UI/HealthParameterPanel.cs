using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Permet d'afficher les informations des paramètres de santé dans un panneau.
/// Mis en composant de l'objet "HealthParameterPanel" du hub.
/// </summary>
public class HealthParameterPanel : MonoBehaviour {

	//champs de text
	public Text parameterName;
	public Text parameterDescription;
	public Text parameterIndications;

	//objets connectés
	public Transform deviceLayout;
	public DevicePanel descriptivePanel;
	public GameObject healthButton;

	/// <summary>
	/// Active le panneau et change les valeurs des champs pour afficher les informations du paramètre de santé choisi.
	/// Bloque les caméras et le défilement du temps.
	/// </summary>
	public void Open(HealthParameter prm)
	{
		gameObject.SetActive(true);
		parameterName.text = prm.name;
		parameterDescription.text = prm.description;
		SetHealthDeviceField(prm);
		parameterIndications.text = prm.indications;
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Active le panneau et change les valeurs des champs pour afficher les informations du paramètre de maison choisi.
	/// Bloque les caméras et le défilement du temps.
	/// </summary>
	public void Open(HomeParameter prm)
	{
		gameObject.SetActive(true);
		parameterName.text = prm.name;
		parameterDescription.text = prm.description;
		SetHomeDeviceField(prm);
		parameterIndications.text = prm.indications;
		MapCamera.Locked = true;
		OverviewCamera.Locked = true;
		NavigationBehavior.Locked = true;
		TimeClock.Locked = true;
	}

	/// <summary>
	/// Désactive le panneau et active les caméras.
	/// </summary>
	public void Close()
	{
		ClearSensorField();
		gameObject.SetActive(false);
		MapCamera.Locked = false;
		OverviewCamera.Locked = false;
		NavigationBehavior.Locked = false;
		TimeClock.Locked = false;
	}

	/// <summary>
	/// Instancie les boutons des capteurs suivant le paramètre donné, puis les place en enfant de l'objet 
	/// "deviceLayout".
	/// </summary>
	void SetHealthDeviceField(HealthParameter prm)
	{
		Device[] devices = DeviceStorage.GetList(prm);
		for (int i = 0; i < devices.Length; i++)
		{
			var i2 = i;
			GameObject b = devices[i2].InstantiateDeviceButton(healthButton);
			b.transform.SetParent(deviceLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenDescriptionOnClick(devices[i2]); 
				EventsStorage.Push(new PanelEvent(prm)); });
		}
	}

	/// <summary>
	/// Instancie les boutons des capteurs suivant le paramètre donné, puis les place en enfant de l'objet 
	/// "deviceLayout".
	/// </summary>
	void SetHomeDeviceField(HomeParameter prm)
	{
		Device[] devices = DeviceStorage.GetList(prm);
		for (int i = 0; i < devices.Length; i++)
		{
			var i2 = i;
			GameObject b = devices[i2].InstantiateDeviceButton(healthButton);
			b.transform.SetParent(deviceLayout, true);
			b.GetComponent<Button>().onClick.AddListener(delegate { OpenDescriptionOnClick(devices[i2]); 
				EventsStorage.Push(new PanelEvent(prm));});
		}
	}

	/// <summary>
	/// Détruit les boutons de l'objet "deviceLayout".
	/// </summary>
	void ClearSensorField()
	{
		int count = deviceLayout.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			Destroy(deviceLayout.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// Fonction attribuée aux boutons des capteurs lors de leur création.
	/// Ferme le panneau et en ouvre un nouveau pour afficher les informations du capteur choisi.
	/// </summary>
	void OpenDescriptionOnClick(Device device)
	{
		Close();
		descriptivePanel.Open(device);
	}
}
