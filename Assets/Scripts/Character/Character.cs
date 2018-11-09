using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cette classe regroupe les informations d'un profil unique. 
/// Elle est un composant du prefab "CharacterPrefab", qui est instancié à chaque création de profil.
/// </summary>
public class Character : PersistableObject {

	// Informations du profil, éditables par l'utilisateur
	public int gender;
	public new string name;
	public string surname;
	public string birthday;
	public string adress;
	public string health;
	public int index;
	public HealthInformation[] chronics;

	// Liste des paramètres de santé et data
	public HealthInformationsList chronicsList;
	public List<HealthParameter> healthPrms;
	public List<HomeParameter> homePrms;
	[HideInInspector]
	public List<int> healthPrmsRefs, homePrmsRefs;
	bool created;
	List<Data> data = new List<Data>();

	/// <summary>
	/// Permet de conserver entre les scènes les objets ayant pour composant ce script grace à la fonction
	/// "DontDestroyOnLoad".
	/// Cette fonction empêche également tout doublon lors du chargement d'une scène.
	/// </summary>
	void Awake()
	{
		if (!created)
		{
			DontDestroyOnLoad(gameObject);
			created = true;
		}
	}

	/// <summary>
	/// Sauvegarde les informations du profil.
	/// </summary>
	public override void Save(HomeDataWriter writer)
	{
		writer.Write(gender);
		writer.Write(name);
		writer.Write(surname);
		writer.Write(birthday);
		writer.Write(adress);
		writer.Write(health);
		writer.Write(chronics.Length);
		foreach (HealthInformation i in chronics)
		{
			writer.Write(chronicsList.GetRef(i));
		}
		writer.Write(data.Count);
		for (int i = 0; i < data.Count; i++)
		{
			List<SubData> sList = new List<SubData>(data[i].subData);
			writer.Write(sList.Count);
			for (int u = 0; u < sList.Count; u++)
			{
				SubData s = sList[u];
				writer.Write(s.average);
				writer.Write(s.values.Count);
				for (int v = 0; v < s.values.Count; v++)
				{
					writer.Write(s.values[v]);
					writer.Write(s.instants[v].hour);
					writer.Write(s.instants[v].minute);
				}
			}
		}
	}

	/// <summary>
	/// Charge les informations du profil.
	/// </summary>
	public override void Load(HomeDataReader reader)
	{
		gender = reader.ReadInt();
		name = reader.ReadString();
		surname = reader.ReadString();
		birthday = reader.ReadString();
		adress = reader.ReadString();
		health = reader.ReadString();
		chronics = new HealthInformation[reader.ReadInt()];
		healthPrms = new List<HealthParameter>();
		healthPrmsRefs = new List<int>();
		homePrms = new List<HomeParameter>();
		homePrmsRefs = new List<int>();

		for (int i = 0; i < chronics.Length; i++)
		{
			chronics[i] = chronicsList.Get(reader.ReadInt());
			HealthParameter[] cHealthPrm = chronics[i].healthP;
			HomeParameter[] cHomePrm = chronics[i].homeP;
			foreach (HealthParameter h in cHealthPrm)
			{
				if (!healthPrmsRefs.Contains(h.reference))
				{
					healthPrms.Add(h);
					healthPrmsRefs.Add(h.reference);
				}
			}
			foreach (HomeParameter h in cHomePrm)
			{
				if (!homePrmsRefs.Contains(h.reference))
				{
					homePrms.Add(h);
					homePrmsRefs.Add(h.reference);
				}
			}
		}
		int dataCount = reader.ReadInt();

		for (int i = 0; i < dataCount; i++)
		{
			data.Add(LoadData(i, reader));
		}
	}

	/// <summary>
	/// Permet de modifier les problèmes de santé du profil en lui en attribuant de nouveaux.
	/// </summary>
	public void SetChronics(HealthInformation[] newChronics)
	{
		chronics = newChronics;
		healthPrms = new List<HealthParameter>();
		healthPrmsRefs = new List<int>();
		homePrms = new List<HomeParameter>();
		homePrmsRefs = new List<int>();

		foreach (HealthInformation c in chronics)
		{
			HealthParameter[] cHealthPrm = c.healthP;
			HomeParameter[] cHomePrm = c.homeP;
			foreach (HealthParameter h in cHealthPrm)
			{
				if (!healthPrmsRefs.Contains(h.reference))
				{
					healthPrms.Add(h);
					healthPrmsRefs.Add(h.reference);
				}
			}
			foreach (HomeParameter h in cHomePrm)
			{
				if (!homePrmsRefs.Contains(h.reference))
				{
					homePrms.Add(h);
					homePrmsRefs.Add(h.reference);
				}
			}
		}

		if (data.Count > 0)
		{
			if (healthPrms.Count == 0)
			{
				data = new List<Data>();
			}
			else
			{
				
				var newData = new List<Data>();
				var dataRefs = new List<int>();
				for (int i = 0; i < data.Count; i++)
				{
					dataRefs.Add(data[i].reference);
				}
				for (int i = 0; i < healthPrmsRefs.Count; i++)
				{
					if (dataRefs.Contains(healthPrmsRefs[i]))
					{
						int reference = dataRefs.IndexOf(healthPrmsRefs[i]);
						newData.Add(data[reference]);
					}
					else
					{
						newData.Add(GenerateData(i));
					}
				}
				data = new List<Data>(newData);
			}
		}
		else
		{
			GenerateAllData();
		}
	}

	/// <summary>
	/// Copie les informations du profil dans un autre profil.
	/// Cette fonction est utilisée pour la modification des profils déjà existants.
	/// </summary>
	public void Copy(Character character)
	{
		character.gender = gender;
		character.name = name;
		character.surname = surname;
		character.birthday = birthday;
		character.adress = adress;
		character.health = health;
		character.chronics = chronics;
		character.healthPrms = healthPrms;
		character.homePrms = homePrms;
		character.healthPrmsRefs = healthPrmsRefs;
		character.data = new List<Data>(data);
	}

	/// <summary>
	/// Génère des données aléatoires pour le paramètre correspondant à la référence donnée.
	/// </summary>
	public Data GenerateData(int index = 0)
	{
		Data singleData = new Data();
		if (healthPrms.Count > 0)
		{
			singleData.subData = healthPrms[index].GenerateData();
			singleData.name = healthPrms[index].name;
			singleData.decimals = healthPrms[index].decimals;
			singleData.min = healthPrms[index].minValue;
			singleData.max = healthPrms[index].maxValue;
			singleData.reference = healthPrms[index].reference;
			singleData.prm = healthPrms[index];
		}
		return singleData;
	}

	/// <summary>
	/// Charge les données lors du chargement du profil.
	/// </summary>
	Data LoadData(int i, HomeDataReader reader)
	{
		Data singleData = new Data();
		int count = reader.ReadInt();
		singleData.subData = new List<SubData>();
		for (int u = 0; u < count; u++)
		{
			SubData subData = new SubData();
			subData.values = new List<float>();
			subData.instants = new List<Instant>();
			subData.average = reader.ReadFloat();
			int count2 = reader.ReadInt();
			for (int v = 0; v < count2; v++)
			{
				subData.values.Add(reader.ReadFloat());
				subData.instants.Add(new Instant(reader.ReadInt(), reader.ReadInt()));
			}
			singleData.subData.Add(subData);
		}
		singleData.name = healthPrms[i].name;
		singleData.decimals = healthPrms[i].decimals;
		singleData.min = healthPrms[i].minValue;
		singleData.max = healthPrms[i].maxValue;
		singleData.reference = healthPrms[i].reference;
		singleData.prm = healthPrms[i];
		return singleData;
	}

	/// <summary>
	/// Génère aléatoirement toutes les données du profil.
	/// </summary>
	void GenerateAllData()
	{
		for (int i = 0; i < healthPrms.Count; i++)
		{
			data.Add(GenerateData(i));
		}
	}

	/// <summary>
	/// Renvoie les données du paramètre correspondant à la référence donnée.
	/// </summary>
	public Data GetData(int index = 0)
	{
		int prmIndex = 0;
		if (healthPrms.Count > 0)
		{
			for (int i = 1; i < healthPrms.Count; i++)
			{
				if (healthPrms[i].IsMeasured())
				{
					prmIndex += 1;
					if (index == prmIndex)
					{
						return data[i];
					}
				}
			}
		}
		return data[0];

	}

	/// <summary>
	/// Ajoute pour chaque paramètre de santé une nouvelle donnée correspondant à l'instant "t" à sa liste des données. 
	/// Si l'heure vaut 0, crée une nouvelle "subData", c'est à dire une nouvelle journée dans la liste des données.
	/// </summary>
	public void AddSubDataValue(Instant t)
	{
		if (t.hour != 0)
		{
			for (int i = 0; i < data.Count; i++)
			{
				int count = data[i].subData.Count;
				bool isABool = healthPrms[i].isABool;
				if (healthPrms[i].IsMeasured())
				{
					SubData subData = data[i].subData[count - 1];
					if (!isABool)
					{
						subData.values.Add(healthPrms[i].GenerateSingleValue(subData.values[t.hour - 1]));
					}
					else
					{
						subData.values.Add(0f);
					}
					Metrics.RecalculateAverage(subData, subData.values[subData.values.Count - 1], isABool);
				}
				else
				{
					SubData subData = data[i].subData[count - 1];
					if (!isABool)
					{
						subData.values.Add(data[i].subData[count - 1].values[t.hour - 1]);
					}
					else
					{
						subData.values.Add(0f);
					}
					Metrics.RecalculateAverage(subData, subData.values[subData.values.Count - 1], isABool);
				}
				data[i].subData[count - 1].instants.Add(t);
			}
		}
		else
		{
			for (int i = 0; i < data.Count; i++)
			{
				int count = data[i].subData.Count;
				bool isABool = healthPrms[i].isABool;
				SubData subData = new SubData();
				subData.values = new List<float>();
				if (healthPrms[i].IsMeasured())
				{
					if (!isABool)
					{
						subData.values.Add(healthPrms[i].GenerateSingleValue(data[i].subData[count - 1].values[23]));
					}
					else
					{
						subData.values.Add(0f);
					}
					Metrics.RecalculateAverage(subData, subData.values[subData.values.Count - 1], isABool);
				}
				else
				{
					if (!isABool)
					{
						subData.values.Add(data[i].subData[count - 1].values[23]);
					}
					else
					{
						subData.values.Add(0f);
					}
					Metrics.RecalculateAverage(subData, subData.values[subData.values.Count - 1], isABool);
				}

				subData.instants = new List<Instant>();
				subData.instants.Add(t);

				data[i].subData.Add(subData);
			}
		}
	}

	/// <summary>
	/// Change la valeur de la donnée du paramètre "prm" correspondant à l'instant "t" par la nouvelle valeur "f".
	/// </summary>
	public void RefreshData(HealthParameter prm, Instant t, float f = 0f)
	{
		if (healthPrmsRefs.Contains(prm.reference))
		{
			int i = healthPrms.IndexOf(prm);
			int sCount = data[i].subData.Count;
			int vCount = data[i].subData[sCount - 1].values.Count;
			if (Mathf.Abs(f) < 0.00001f)
			{
				if (vCount > 1)
				{
					data[i].subData[sCount - 1].values[vCount - 1] =
						prm.GenerateSingleValue(data[i].subData[sCount - 1].values[vCount - 2]);
				}
				else
				{
					data[i].subData[sCount - 1].values[vCount - 1] =
						prm.GenerateSingleValue(data[i].subData[sCount - 2].values[23]);
				}
			}
			else
			{
				data[i].subData[sCount - 1].values[vCount - 1] = f;
			}
			data[i].subData[sCount - 1].instants[vCount - 1] = t;
		}
	}
}

/// <summary>
/// Structure de donnée des paramètres.
/// La liste "subData" contient autant de SubData que de jours utilisés pour la base.
/// De base, 731 jours sont utilisés.
/// </summary>
public struct Data
{
	public string name;
	public List<SubData> subData;
	public int decimals;
	public float min, max;
	public int reference;
	public HealthParameter prm;
}

/// <summary>
/// Donnée d'une journée.
/// "values" contient une valeur par heure de la journée. Elle contiendra donc 24 flottants si la journée est passée.
/// "instants" contient autant d'éléments que "values". Chaque Instant correspond à l'heure d'une mesure de la liste
/// "values".
/// "average" est la moyenne des mesures, ou leur somme si le paramètre est un booléen.
/// </summary>
public struct SubData
{
	public float average;
	public List<float> values;
	public List<Instant> instants;
}