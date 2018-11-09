using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Composant de l'objet "GraphTimeSet", activé lorsque l'utilisateur décide de changer la date de référence du 
/// graphique montrant l'évolution des paramètres.
/// Le choix de la date s'effectue grâce à des menus déroulants de plusieurs options.
/// </summary>
public class ChooseGraphTimeSet : MonoBehaviour {

	// choix du jour, du mois, et de l'année
	public Dropdown dayDropdown, monthDropdown, yearDropdown;

	// texte apparaissant sous le graphique pour indiquer la date de référence
	public Text timeText;

	// Message d'erreur et message indicatif
	public GameObject errorMessage;
	public Text indicationText;

	//référence au graphe
	public LineGraphManager graph;

	//liste des jours, mois, années
	string[] years, months, days;

	/// <summary>
	/// Construit les tableaux donnant les jours, mois et années.
	/// </summary>
	void SetArrays()
	{
		years = new string[]{ (TimeClock.instance.startYear - 2).ToString(), 
			(TimeClock.instance.startYear - 1).ToString(), TimeClock.instance.startYear.ToString()};
		months = new string[12];
		for (int i = 0; i < 12; i++)
		{
			months[i] = (i + 1).ToString();
		}
		days = new string[31];
		for (int i = 0; i < 31; i++)
		{
			days[i] = (i + 1).ToString();
		}
	}

	/// <summary>
	/// Ouvre le panneau de séléction de date.
	/// Les jours, mois, années correspondent à ceux de la date de référence actuelle du graphique.
	/// Change le texte sous les menus déroulants pour donner des indications à l'utilisateur pour sélectionner la date.
	/// </summary>
	public void Open()
	{
		gameObject.SetActive(true);
		SetArrays();
		dayDropdown.ClearOptions();
		dayDropdown.AddOptions(new List<string>(days));
		dayDropdown.value = TimeClock.GetDayInt(0, timeText.text) - 1;
		monthDropdown.ClearOptions();
		monthDropdown.AddOptions(new List<string>(months));
		monthDropdown.value = TimeClock.GetMonthInt(0, timeText.text) - 1;
		yearDropdown.ClearOptions();
		yearDropdown.AddOptions(new List<string>(years));
		yearDropdown.value = TimeClock.GetYearInt(0, timeText.text) - 2016;
		indicationText.text = "Veuillez choisir une date entre le " + TimeClock.GetDay(0) + " inclus et le " +
			TimeClock.GetDay() + " inclus, cette date sera la fin de la plage de temps si possible";
		errorMessage.SetActive(false);
	}

	/// <summary>
	/// Sélectionne le mois à utiliser.
	/// Appelé dès que l'utilisateur change la valeur du menu "MonthDropdown".
	/// </summary>
	public void SelectMonth()
	{
		int m = monthDropdown.value + 1;
		if ((m < 8 && m % 2 == 1) || (m >= 8 && m % 2 == 0))
		{
			AjustDay(31);
		}
		else
		{
			if (m != 2)
			{
				AjustDay(30);
			}
			else
			{
				AjustDay(28);
			}
		}
	}

	/// <summary>
	/// Ajuste la liste des jours selon le mois choisi. Le mois choisi permet de définir l'entier "maxDay", paramètre
	/// de la méthode.
	/// Si le jour qui était sélectionné est supérieur à "maxDay", on change la valeur du menu "DayDropdown" pour 
	/// correspondre à "maxDay".
	/// </summary>
	void AjustDay(int maxDay)
	{
		if (days.Length >= maxDay)
		{
			int d = dayDropdown.value + 1;
			dayDropdown.ClearOptions();
			dayDropdown.AddOptions(new List<string>(days));
			for (int i = days.Length- 1; i >= maxDay; i--)
			{
				dayDropdown.options.RemoveAt(i);
			}
			if (d > maxDay)
			{
				dayDropdown.value = maxDay - 1;
			}
			else
			{
				dayDropdown.value = d - 1;
			}
		}
	}

	/// <summary>
	/// Vérifie que la date sélectionné est bien dans l'intervalle des dates utilisé par le système.
	/// Si oui, ferme le panneau et change la date de référence du graphique.
	/// Sinon, affiche un message d'erreur incitant l'utilisateur à changer la date.
	/// </summary>
	public void ValidateTimeSet()
	{
		if (TimeClock.GetDayIndex((dayDropdown.value + 1) + "/" + (monthDropdown.value + 1)
								  + "/" + (yearDropdown.value + 2016)) != -1)
		{
			gameObject.SetActive(false);
			timeText.text = (dayDropdown.value + 1) + "/" + (monthDropdown.value + 1)
				+ "/" + (yearDropdown.value + 2016);
			graph.Open();
			
		}
		else
		{
			errorMessage.SetActive(true);
		}
	}

	/// <summary>
	/// Change la date affichée sous le graphique pour correspondre à la date actuelle du simulateur.
	/// Appelée lors de l'initialisation du panneau.
	/// </summary>
	public void SetTime()
	{
		if (timeText.text == "00/00/0000")
		{
			timeText.text = TimeClock.GetDay();
		}
	}

	/// <summary>
	/// Change la date de référence pour correspondre à la date actuelle du simulateur, puis met à jour le graphique.
	/// </summary>
	public void Reset()
	{
		timeText.text = TimeClock.GetDay();
		graph.Open();
	}
}
