using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permet de stocker sous forme de listes les références aux capteurs présents dans la maison.
/// Il existe des listes pour chaque étage, et des listes pour chaque paramètre de santé et de maison.
/// </summary>
public class DeviceStorage : MonoBehaviour
{
	//référence à lui-même
	static DeviceStorage instance;

	//Listes des objets étage par étage
	[HideInInspector]
	public List<Device> devices0 = new List<Device>();
	[HideInInspector]
	public List<Device> devices1 = new List<Device>();
	[HideInInspector]
	public List<Device> devices2 = new List<Device>();

	//Listes de tous les objets suivant des paramètres de santé, et de tous les objets suivant des paramètres de maison.
	//Chaque paramètre correspond à une sous-liste.
	public List<List<Device>> healthDevices = new List<List<Device>>();
	public List<List<Device>> homeDevices = new List<List<Device>>();

	//Tableau des paramètres de santé et des paramètres de maison utilisés dans le simulateur.
	public HealthParameterFactory healthParameters;
	public HomeParameterFactory homeParameters;

	/// <summary>
	/// Crée une référence unique à la classe dans ce projet.
	/// Crée une liste pour chaque paramètre de santé de "healthParameters".
	/// Crée une liste pour chaque paramètre de santé de "homeParameters".
	/// </summary>
	void Awake()
	{
		instance = this;
		for (int i = 0; i < healthParameters.GetSize(); i++)
		{
			healthParameters.Get(i).reference = i;
			healthDevices.Add(new List<Device>());
		}
		for (int i = 0; i < homeParameters.GetSize(); i++)
		{
			homeParameters.Get(i).reference = i;
			homeDevices.Add(new List<Device>());
		}
	}

	/// <summary>
	/// Ajoute l'objet donné à la liste des objets de l'étage "level".
	/// </summary>
	public static void Add(Device device, int level)
	{
		switch (level)
		{
			case 0:
				instance.devices0.Add(device);
				device.index = (instance.devices0.Count - 1);
				break;
			case 1:
				instance.devices1.Add(device);
				device.index = (instance.devices1.Count - 1);
				break;
			case 2:
				instance.devices2.Add(device);
				device.index = (instance.devices2.Count - 1);
				break;
			default:
				break;
		}
		AddParametersToLists(device);
	}

	/// <summary>
	/// Supprime l"objet donné de la liste des objets de l'étage "level".
	/// </summary>
	public static void Remove(Device device, int level)
	{
		if (!device.isDestroyed)
		{
			int index = device.index;
			switch (level)
			{
				case 0:
					instance.devices0.RemoveAt(index);
					foreach (Device item in instance.devices0.GetRange(index, instance.devices0.Count - index))
					{
						item.index -= 1;
					}
					break;
				case 1:
					instance.devices1.RemoveAt(index);
					foreach (Device item in instance.devices1.GetRange(index, instance.devices1.Count - index))
					{
						item.index -= 1;
					}
					break;
				case 2:
					instance.devices2.RemoveAt(index);
					foreach (Device item in instance.devices2.GetRange(index, instance.devices1.Count - index))
					{
						item.index -= 1;
					}
					break;
				default:
					break;
			}
			if (level != -1)
			{
				RemoveParametersToList(device);
			}
		}
		device.isDestroyed = true;
	}

	/// <summary>
	/// Active ou désactive les composants "Outline" des objets connectés de l'étage donné.
	/// </summary>
	public static void SetOutlines(bool enabled, int level)
	{
		switch (level)
		{
			case 0:
				foreach (Device device in instance.devices0)
				{
					device.EnableOutline(enabled, 1);
				}
				break;
			case 1:
				foreach (Device device in instance.devices1)
				{
					device.EnableOutline(enabled, 1);
				}
				break;
			default:
				foreach (Device device in instance.devices2)
				{
					device.EnableOutline(enabled, 1);
				}
				break;
		}
	}

	/// <summary>
	/// Nettoie toutes les listes de la classe.
	/// </summary>
	public static void Clear()
	{
		instance.devices0.Clear();
		instance.devices1.Clear();
		instance.devices2.Clear();
		for (int i = 0; i < instance.healthParameters.GetSize(); i++)
		{
			instance.healthDevices[i].Clear();
		}
		for (int i = 0; i < instance.homeParameters.GetSize(); i++)
		{
			instance.homeDevices[i].Clear();
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des objets de l'étage donné.
	/// </summary>
	public static void EnableColliders(bool enabled, int level)
	{
		switch (level)
		{
			case 0:
				foreach (Device device in instance.devices0)
				{
					device.EnableColliders(enabled);
				}
				break;
			case 1:
				foreach (Device device in instance.devices1)
				{
					device.EnableColliders(enabled);
				}
				break;
			default:
				foreach (Device device in instance.devices2)
				{
					device.EnableColliders(enabled);
				}
				break;
		}
	}

	/// <summary>
	/// Ajoute l'objet donné aux listes de paramètres de cet objet.
	/// </summary>
	public static void AddParametersToLists(Device device)
	{
		HealthParameter[] healthP = device.healthParameters;
		for (int i = 0; i < healthP.Length; i++)
		{
			HealthParameter parameter = healthP[i];
			instance.healthDevices[parameter.reference].Add(device);
			device.healthPIndex[i] = instance.healthDevices[parameter.reference].Count - 1;
		}
		HomeParameter[] deviceFunctions = device.homeParameters;
		for (int i = 0; i < deviceFunctions.Length; i++)
		{
			HomeParameter function = deviceFunctions[i];
			instance.homeDevices[function.reference].Add(device);
			device.homePIndex[i] = instance.homeDevices[function.reference].Count - 1;
		}
	}

	/// <summary>
	/// Supprime l'objet donné des listes de paramètres de cet objet.
	/// </summary>
	public static void RemoveParametersToList(Device device)
	{
		HealthParameter[] healthP = device.healthParameters;
		for (int i = 0; i < healthP.Length; i++)
		{
			HealthParameter parameter = healthP[i];
			instance.healthDevices[parameter.reference].RemoveAt(device.healthPIndex[i]);
			foreach (Device d in instance.healthDevices[parameter.reference].
			         GetRange(device.healthPIndex[i],
			                  instance.healthDevices[parameter.reference].Count - device.healthPIndex[i]))
			{
				int reference = d.HealthReference(parameter);
				d.healthPIndex[reference] -= 1;
			}
			device.healthPIndex[i] = -1;
		}
		HomeParameter[] homeP = device.homeParameters;
		for (int i = 0; i < homeP.Length; i++)
		{
			HomeParameter function = homeP[i];
			instance.homeDevices[function.reference].RemoveAt(device.homePIndex[i]);
			foreach (Device d in instance.homeDevices[function.reference].
			         GetRange(device.homePIndex[i],
			                  instance.homeDevices[function.reference].Count - device.homePIndex[i]))
			{
				int reference = d.HomeReference(function);
				d.homePIndex[reference] -= 1;
			}
			device.homePIndex[i] = -1;
		}
	}

	/// <summary>
	/// Renvoie la liste des objets suivant le parametre de santé donné.
	/// </summary>
	public static Device[] GetList(HealthParameter prm)
	{
		return instance.healthDevices[prm.reference].ToArray();
	}

	/// <summary>
	/// Renvoie la liste des objets suivant le parametre de maison donné.
	/// </summary>
	public static Device[] GetList(HomeParameter prm)
	{
		return instance.homeDevices[prm.reference].ToArray();
	}

	/// <summary>
	/// Renvoie "homeParameters".
	/// Utilisé pour afficher les paramètres de maison suivis ou non.
	/// </summary>
	public static HomeParameterFactory GetHomeFactory()
	{
		return instance.homeParameters;
	}
}
