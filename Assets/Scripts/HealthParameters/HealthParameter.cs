using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// Paramètre de santé.
/// Il est possible de créer de nouveaux paramètres dans les assets de Unity et d'en choisir les informations.
/// Pour créer un nouveau paramètre, il suffit d'aller dans le sous-menu asset puis "Create/Health Parameter".
/// </summary>
//Permet la création de nouveaux paramètres de santé dans le sous-menu asset
[CreateAssetMenu]
public class HealthParameter : ScriptableObject
{
	//champs de texte
	public new string name;
	[TextArea(4, 6)]
	public string description;
	[TextArea(4, 6)]
	public string indications;
	public string measure;

	//paramètres de mesure
	public int decimals;
	public float minValue, maxValue;
	public bool isABool;

	//référence du paramètre dans le tableau des paramètres
	[HideInInspector]
	public int reference;

	//Loi de génération des données
	public enum RandowLaw { flat, pureRandom, increasing };
	public RandowLaw randomLaw;
	public float variationValue;
	public float daysRate; //si booleen
	// si croissante
	public float targetValue, criticalValue;
	bool isIncreasing = true;

	/// <summary>
	/// Instancie le bouton "button" et change son texte pour le nom du paramètre.
	/// </summary>
	public GameObject InstantiateHealthButton(GameObject button)
	{
		GameObject b = Instantiate(button);
		b.GetComponentInChildren<Text>().text = name;
		return b;
	}

	/// <summary>
	/// Vérifie que le paramètre est suivi grâce à la liste des capteurs présents dans la maison. Si la liste de 
	/// capteurs pouvant relever ce paramètre à une taille supérieure ou égale à 1, le paramètre est considéré comme
	/// suivi.
	/// Renvoie true si le paramètre est suivi, false sinon.
	/// </summary>
	public bool IsMeasured()
	{
		if (DeviceStorage.GetList(this).Length > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Génère aléatoirement une nouvelle mesure, afin d'être ajoutée aux données ensuite.
	/// Renvoie cette mesure.
	/// Si c'est un booléen, la mesure ne peut prendre comme valeur que 0 ou 1.
	/// Sinon, la valeur est prise dans l'intervalle [minValue,maxValue], avec le nombre de décimales indiqué.
	/// </summary>
	public float GenerateSingleValue(float value)
	{
		if (!isABool)
		{
			if (randomLaw == RandowLaw.flat)
			{
				return Mathf.Clamp((float)Math.Round(
					value + UnityEngine.Random.Range(-variationValue, variationValue), decimals), minValue, maxValue);
			}
			else if (randomLaw == RandowLaw.pureRandom)
			{
				return (float)Math.Round(UnityEngine.Random.Range(minValue, maxValue), decimals);
			}
			else if (randomLaw == RandowLaw.increasing)
			{
				if (isIncreasing)
				{
					if (value > criticalValue)
					{
						if ((float)Math.Round(UnityEngine.Random.Range(0f, 1f), 3) > 0.3)
						{
							isIncreasing = false;
							return Mathf.Clamp((float)Math.Round(
						value - variationValue * 2, decimals), minValue, maxValue);
						}
					}
					return Mathf.Clamp((float)Math.Round(
						value + variationValue, decimals), minValue, maxValue);
				}
				else
				{
					value = Mathf.Clamp((float)Math.Round(
						value - variationValue * 2, decimals), minValue, maxValue);
					if (value <= targetValue)
					{
						isIncreasing = true;
					}
					return Mathf.Clamp((float)Math.Round(
						value - variationValue * 2, decimals), minValue, maxValue);
				}
			}
			else
			{
				return 0;
			}
		}
		else
		{
			if ((float)Math.Round(UnityEngine.Random.Range(0f, 1f), 3) <= daysRate/24)
			{
				return 1f;
			}
			else
			{
				return 0f;
			}
		}
	}

	/// <summary>
	/// Génère aléatoirement un jeu de données.
	/// </summary>
	public List<SubData> GenerateData()
	{
		var data = new List<SubData>();
		float value;
		if (isABool)
		{
			if ((float)Math.Round(UnityEngine.Random.Range(0f, 1f), 3) > 0.99)
			{
				value = 1f;
			}
			else
			{
				value = 0f;
			}
		}
		else
		{
			value = (float)Math.Round(UnityEngine.Random.Range(minValue, maxValue), decimals);
		}

		if (TimeClock.instance)
		{
			for (int i = 0; i < TimeClock.GetFullDaysCount(); i++)
			{
				data.Add(GenerateSubData(new Instant(23, 59), value));
				value = data[i].values[23];
			}
		}
		else
		{
			for (int i = 0; i < ApplicationParameters.startDaysLength - 1; i++)
			{
				data.Add(GenerateSubData(new Instant(23, 59), value));
				value = data[i].values[23];
			}
		}
		data.Add(GenerateSubData(new Instant(ApplicationParameters.startHour, 0), value));
		return data;
	}

	/// <summary>
	/// Génère aléatoirement les données d'une journée. L'instant "end" indiquant la dernière heure de la journée.
	/// </summary>
	public SubData GenerateSubData(Instant end, float value)
	{
		var subData = new SubData();
		subData.values = new List<float>();
		subData.instants = new List<Instant>();
		int hour, minute;

		for (int i = 0; i < end.hour; i++)
		{
			hour = i;
			minute = UnityEngine.Random.Range(0, 60);

			value = GenerateSingleValue(value);
			subData.values.Add( value );
			subData.instants.Add(new Instant(hour, minute));
		}

		hour = end.hour;
		minute = UnityEngine.Random.Range(0, end.minute + 1);

		value = GenerateSingleValue(value);
		subData.values.Add(value);
		subData.instants.Add(new Instant(hour, minute));

		if (!isABool)
		{
			float total = 0f;
			foreach (float f in subData.values)
			{
				total += f;
			}
			subData.average = total / subData.values.Count;
		}
		else
		{
			subData.average = 0f;
			foreach (float f in subData.values)
			{
				if (Mathf.Abs(f - 1) < 0.01)
				{
					subData.average += 1f;
				}
			}
			subData.average = Mathf.Clamp(subData.average, minValue, maxValue);
		}
		return subData;
	}



}
