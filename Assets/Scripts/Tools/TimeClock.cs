using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Ajoute une dimension temporelle au simulateur, en créant un système de temps.
/// La date ne correspond pas à la date réelle. De base, elle correspond au 24 juillet 2018.
/// Le temps passe dans le simulateur à vitesse normale, ou accelérée.
/// Une sauvegarde de la maison conservera la dernière mesure du temps. Lors du chargement, la date est mise à jour pour
/// correspondre à celle de la sauvegarde.
/// </summary>

public class TimeClock : MonoBehaviour
{
	public static TimeClock instance;

	//mesure du temps
	public float timeScale;
	public int timeMode;
	public int startHour, startDay, startMonth, startYear;
	[HideInInspector]
	public string monthName;
	public bool am, noonAm, leapYear;
	[HideInInspector]
	public double minute, day, second;
	[HideInInspector]
	public int hour, month, year;
	private int oldtimeMode;
	[HideInInspector]
	public int lastpick;

	//affichage du temps
	public Text calendarText;

	//booléen indiquant si le temps doit défiler
	public bool passTime = true;

	//images des boutons permettant de choisir la vitesse de défilement
	public Image tgl1, tgl2, tgl3;
	public Sprite[] tglSprites;

	//listes des noms des jours passés pris en compte par le simulateur
	List<string> daysNames = new List<string>();

	/// <summary>
	/// Initialise les paramètres.
	/// </summary>
	void Awake()
	{
		instance = this;
		lastpick = 0;
		startHour = ApplicationParameters.startHour;
		startDay = ApplicationParameters.startDay;
		startMonth = ApplicationParameters.startMonth;
		startYear = ApplicationParameters.startYear;
		year = startYear;
		month = startMonth;
		day = startDay;
		hour = startHour;
		minute = 0;
		SetMode(1); //mode 2 and 3 for fast forwarding hours and days respectively
		oldtimeMode = 1;
		timeScale = 100.0f; //change time speed: 200 = one hour takes 18 seconds
		am = true;
		noonAm = false; //true = noon is AM; option for 12 am/pm confusion
		DetermineMonth();
		PassTime = false;
		SetDaysNames();
	}

	/// <summary>
	/// Sauvegarde le jour et l'heure.
	/// </summary>
	public static void Save(HomeDataWriter writer)
	{
		writer.Write(instance.year);
		writer.Write(instance.month);
		writer.Write((int)instance.day);
		writer.Write(instance.hour);
		writer.Write((int)instance.minute);
	}

	/// <summary>
	/// Charge le jour et l'heure enregistrés.
	/// </summary>
	public static void Load(HomeDataReader reader)
	{
		instance.year = reader.ReadInt();
		instance.month = reader.ReadInt();
		instance.day = reader.ReadInt();
		instance.hour = reader.ReadInt();
		instance.minute = reader.ReadInt();
		instance.TextCallFunction();
	}

	/// <summary>
	/// Appelée à chaque frame pour faire défiler le temps s'il doit défiler.
	/// </summary>
	void Update()
	{
		if (passTime)
		{
			if (lastpick == 10)
			{
				lastpick = 0;
			}
			CalculateTime();
		}
		if (Input.GetKey(KeyCode.Keypad1))
		{
			SetMode(1);
		}
		else if (Input.GetKey(KeyCode.Keypad2))
		{
			SetMode(2);
		}
		else if (Input.GetKey(KeyCode.Keypad3))
		{
			SetMode(3);
		}

		if (oldtimeMode != timeMode)
		{
			oldtimeMode = timeMode;
			if (timeMode == 1)
			{
				TextCallFunction();
			}
		}
	}

	/// <summary>
	/// Permet d'afficher la date.
	/// </summary>
	public void TextCallFunction()
	{
		if (timeMode <= 3)
		{
			if (am == true)
			{
				if (hour <= 9)
				{
					if (minute <= 9)
					{
						calendarText.text = monthName + " " + day + " " + year + " " + "0" + hour + ":0" + minute + " AM";
					}
					else
					{
						calendarText.text = monthName + " " + day + " " + year + " " + "0" + hour + ":" + minute + " AM";
					}
				}
				else if (minute <= 9)
				{
					calendarText.text = monthName + " " + day + " " + year + " " + hour + ":0" + minute + " AM";
					if (noonAm == false && hour == 12)
					{
						calendarText.text = monthName + " " + day + " " + year + " " + hour + ":0" + minute + " PM";
					}
				}
				else
				{
					calendarText.text = monthName + " " + day + " " + year + " " + hour + ":" + minute + " AM";
					if (noonAm == false && hour == 12)
					{
						calendarText.text = monthName + " " + day + " " + year + " " + hour + ":" + minute + " PM";
					}
				}
			}
			else if (am == false)
			{
				if (hour <= 9)
				{
					if (minute <= 9)
					{
						calendarText.text = monthName + " " + day + " " + year + " " + "0" + hour + ":0" + minute + " PM";
					}
					else
					{
						calendarText.text = monthName + " " + day + " " + year + " " + "0" + hour + ":" + minute + " PM";
					}
				}
				else
				{
					if (minute <= 9)
					{
						calendarText.text = monthName + " " + day + " " + year + " " + hour + ":0" + minute + " PM";
						if (noonAm == false && hour == 12)
						{
							calendarText.text = monthName + " " + day + " " + year + " " + hour + ":0" + minute + " AM";
						}
					}
					else
					{
						calendarText.text = monthName + " " + day + " " + year + " " + hour + ":" + minute + " PM";
						if (noonAm == false && hour == 12)
						{
							calendarText.text = monthName + " " + day + " " + year + " " + hour + ":" + minute + " AM";
						}
					}
				}
			}
		}

		else if (timeMode == 3)
		{
			calendarText.text = monthName + " " + day + " " + year;
		}
	}

	/// <summary>
	/// Détermine le nom du mois.
	/// </summary>
	public void DetermineMonth()
	{
		switch (month)
		{
			case 1:
				monthName = "Jan";
				break;
			case 2:
				monthName = "Feb";
				break;
			case 3:
				monthName = "Mar";
				break;
			case 4:
				monthName = "Apr";
				break;
			case 5:
				monthName = "May";
				break;
			case 6:
				monthName = "Jun";
				break;
			case 7:
				monthName = "Jul";
				break;
			case 8:
				monthName = "Aug";
				break;
			case 9:
				monthName = "Sep";
				break;
			case 10:
				monthName = "Oct";
				break;
			case 11:
				monthName = "Nov";
				break;
			default:
				monthName = "Dec";
				break;
		}
		TextCallFunction();
	}

	/// <summary>
	/// Calcule la durée du mois en nombre de jours.
	/// </summary>
	void CalculateMonthLength()
	{
		if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
		{
			if (day >= 32)
			{
				month++;
				day = 1;
				DetermineMonth();
			}
		}
		if (month == 2)
		{
			if (day >= 29)
			{
				month++;
				day = 1;
				DetermineMonth();
			}

		}

		if (month == 4 || month == 6 || month == 9 || month == 11)
		{
			if (day >= 31)
			{
				month++;
				day = 1;
				DetermineMonth();
			}
		}
	}

	/// <summary>
	/// Appelée dès que "lastPick" retourne à 0.
	/// </summary>
	void CalculateTime()
	{
		bool addADay = false;
		if (timeMode == 1)
		{
			timeScale = 1;
		}
		else if (timeMode == 2)
		{
			timeScale = 60;
		}
		else if (timeMode == 3)
		{
			timeScale = 400 * 3;  //le *3 est à supprimer par la suite
		}
		second += Time.fixedDeltaTime * timeScale;
		if (second >= 60)
		{
			lastpick += 1;
			minute++;
			second = 0;
			TextCallFunction();
		}
		else if (minute >= 60)
		{
			if (hour <= 12 && am == true)
			{
				hour++;
				if (hour >= 13)
				{
					am = false;
					hour = 1;
					CharacterStorage.AddSubDataValue(new Instant(13, 0));
				}
				else
				{
					CharacterStorage.AddSubDataValue(new Instant(hour,0));
				}
			}
			else if (hour <= 12 && am == false)
			{
				hour++;
				if (hour == 12)
				{
					day++;
					addADay = true;
					CharacterStorage.AddSubDataValue(new Instant(0, 0));
				}
				else if (hour >= 13)
				{
					hour = 1;
					am = true;
					CharacterStorage.AddSubDataValue(new Instant(1, 0));
				}
				else
				{
					CharacterStorage.AddSubDataValue(new Instant(hour + 12, 0));
				}
			}
			minute = 0;
			TextCallFunction();
		}
		else if (day >= 28)
		{
			CalculateMonthLength();
		}
		else if (month >= 13)
		{
			month = 1;
			year++;
			DetermineMonth();
		}
		if (addADay)
		{
			daysNames.Add((int)day + "/" + month + "/" + year);
		}
	}

	/// <summary>
	/// Renvoie le temps sous forme de chaîne de caractère.
	/// </summary>
	public string TimeToString()
	{
		string date = "";
		if (day < 10)
		{
			date += "0";
		}
		date += day + "-";
		if (month < 10)
		{
			date += "0";
		}
		date += month + "-";
		date += year + " ";
		if (!am)
		{
			int heure = hour + 12;
			if (heure == 24)
			{
				date += "00:";
			}
			else
			{
				date += heure + ":";
			}
		}
		else
		{
			if (hour < 10)
			{
				date += "0" + hour + ":";
			}
			else
			{
				date += hour + ":";
			}
		}
		if (minute < 10)
		{
			date += "0" + minute + ":";
		}
		else
		{
			date += minute + ":";
		}
		if (second < 10)
		{
			date += "0" + ((int)second);
		}
		else
		{
			date += ((int)second).ToString();
		}

		return date;
	}

	/// <summary>
	/// Renvoie le nombre de jours complets enregistrés.
	/// Appelée lors de la génération des données.
	/// </summary>
	public static int GetFullDaysCount()
	{
		return instance.daysNames.Count - 1;
	}

	/// <summary>
	/// Renvoie la date.
	/// </summary>
	public static string GetTime()
	{
		return instance.TimeToString();
	}

	/// <summary>
	/// Permet de passer d'une vitesse de défilement à une autre, et de changer les sprites des boutons des vitesses
	/// en fonction.
	/// </summary>
	public void SetMode(int mode)
	{
		timeMode = mode;
		if (tgl1)
		{
			switch (mode)
			{
				case 1:
					tgl1.sprite = tglSprites[0];
					tgl2.sprite = tglSprites[3];
					tgl3.sprite = tglSprites[5];
					break;
				case 2:
					tgl1.sprite = tglSprites[1];
					tgl2.sprite = tglSprites[2];
					tgl3.sprite = tglSprites[5];
					break;
				default:
					tgl1.sprite = tglSprites[1];
					tgl2.sprite = tglSprites[3];
					tgl3.sprite = tglSprites[4];
				break;
			}
		}
	}

	/// <summary>
	/// Active ou désactive ce script. Appelée lors de l'ouverture et de la fermeture du menu.
	/// </summary>
	public static bool Locked
	{
		set
		{
			instance.enabled = !value;
		}
	}

	/// <summary>
	/// Arrête ou redémarre le défilement du temps.
	/// </summary>
	public static bool PassTime
	{
		set
		{
			instance.passTime = value;
			instance.gameObject.SetActive(value);
		}
	}

	/// <summary>
	/// Renvoie l'heure sous forme d'entier, compris entre 0 et 23.
	/// </summary>
	public static int Hour
	{
		get
		{
			if (instance.am)
			{
				return instance.hour;
			}
			else
			{
				if (instance.hour == 12)
				{
					return 0;
				}
				else
				{
					return instance.hour + 12;
				}
			}

		}
	}

	/// <summary>
	/// Renvoie la date d'indice "index".
	/// </summary>
	public static string GetDay(int index = -1)
	{
		if (0 <= index && index < instance.daysNames.Count)
		{
			return instance.daysNames[index];
		}
		else
		{
			return instance.day + "/" + instance.month + "/" + instance.year;
		}
	}

	/// <summary>
	/// Renvoie le jour souhaité sous forme d'entier.
	/// Le jour peut être trouvé grâce à son indice "index" dans la liste des jours enregistrés, ou grâce à sa date.
	/// </summary>
	public static int GetDayInt(int index = -1, string day = "")
	{
		if (day == "")
		{
			if (0 <= index && index < instance.daysNames.Count)
			{
				return int.Parse(instance.daysNames[index].Substring(0,
					  instance.daysNames[index].IndexOf("/", System.StringComparison.CurrentCulture)));
			}
			else
			{
				return (int)instance.day;
			}
		}
		else
		{
			return int.Parse(day.Substring(0,
					  day.IndexOf("/", System.StringComparison.CurrentCulture)));
		}
	}

	/// <summary>
	/// Renvoie le mois souhaité sous forme d'entier.
	/// Le mois peut être trouvé grâce à son indice "index" dans la liste des jours enregistrés, ou grâce à sa date.
	/// </summary>
	public static int GetMonthInt(int index = -1, string day = "")
	{
		if (day == "")
		{
			if (0 <= index && index < instance.daysNames.Count)
			{
				string subString = instance.daysNames[index].Substring(
					instance.daysNames[index].IndexOf("/", System.StringComparison.CurrentCulture) + 1);
				return int.Parse(subString.Substring(0,
													 subString.IndexOf("/", System.StringComparison.CurrentCulture)));
			}
			else
			{
				return instance.month;
			}
		}
		else
		{
			string subString = day.Substring(
					day.IndexOf("/", System.StringComparison.CurrentCulture) + 1);
			return int.Parse(subString.Substring(0,
												 subString.IndexOf("/", System.StringComparison.CurrentCulture)));
		}
	}

	/// <summary>
	/// Renvoie l'année souhaitée sous forme d'entier.
	/// L'année peut être trouvée grâce à son indice "index" dans la liste des jours enregistrés, ou grâce à sa date.
	public static int GetYearInt(int index = -1, string day = "")
	{
		if (day == "")
		{
			if (0 <= index && index < instance.daysNames.Count)
			{
				string subString = instance.daysNames[index].Substring(
					instance.daysNames[index].LastIndexOf("/", System.StringComparison.CurrentCulture) + 1, 4);
				return int.Parse(subString);
			}
			else
			{
				return instance.year;
			}
		}
		else
		{
			string subString = day.Substring(
					day.LastIndexOf("/", System.StringComparison.CurrentCulture) + 1, 4);
			return int.Parse(subString);
		}
	}

	/// <summary>
	/// Renvoie un tableau de chaînes de caractère, donnant la semaine se terminant par la date d'indice
	/// "index" dans la liste des jours enregistrés.
	/// Si l'indice est inférieur à 6, renvoie la première semaine enregistrée.
	/// </summary>
	public static string[] GetWeekDays(int index = -1)
	{
		string[] days = new string[7];
		int d, m, y;
		if (index - 6 >= 0)
		{
			d = GetDayInt(index);
			m = GetMonthInt(index);
			y = GetYearInt(index);
		}
		else
		{
			d = 30;
			m = 7;
			y = 2016;
		}
		days[6] = d + "/" + m + "/" + y;

		for (int i = 5; i >= 0; i--)
		{
			d -= 1;
			if (d < 1)
			{
				if ((m < 8 && m % 2 == 1) || (m >= 8 && m % 2 == 0))
				{
					d = 31;
				}
				else
				{
					if (m != 2)
					{
						d = 30;
					}
					else
					{
						d = 28;
					}
				}
				m -= 1;
				if (m == 0)
				{
					y -= 1;
					m = 12;
				}
			}
			days[i] = d + "/" + m;
		}

		days[0] += "/" + y;
		return days;
	}

	/// <summary>
	/// Renvoie un tableau de chaînes de caractère, donnant 31 dates successives, se terminant par la date d'indice
	/// "index" dans la liste des jours enregistrés.
	/// Si l'indice est inférieur à 30, renvoie les 31 premieres dates enregistrées.
	/// </summary>
	public static string[] GetMonthDays(int index = -1)
	{
		string[] days = new string[31];
		int d, m, y;
		if (index - 30 >= 0)
		{
			d = GetDayInt(index);
			m = GetMonthInt(index);
			y = GetYearInt(index);
		}
		else
		{
			d = 23;
			m = 8;
			y = 2016;
		}
		days[30] = d + "/" + m + "/ " + y;

		for (int i = 29; i >= 0; i--)
		{
			d -= 1;
			if (d < 1)
			{
				m -= 1;
				if (m == 0)
				{
					y -= 1;
					m = 12;
				}
				if ((m < 8 && m % 2 == 1) || (m >= 8 && m % 2 == 0))
				{
					d = 31;
				}
				else
				{
					if (m != 2)
					{
						d = 30;
					}
					else
					{
						d = 28;
					}
				}
			}
			days[i] = d.ToString();
			if (i == 0)
			{
				days[i] = d + "/" + m + "/" + y;
			}
		}
		return days;
	}

	/// <summary>
	/// Renvoie un tableau de chaînes de caractère, donnant 73 dates, espacées de 5 jours maximum, se terminant par la 
	/// date d'indice "index" dans la liste des jours enregistrés, permettant de couvrir une année.
	/// Si l'indice est inférieur à 364, renvoie les 365 premières dates enregistrées.
	/// </summary>
	public static string[] GetYearDays(int index)
	{
		string[] days = new string[73];
		int d, m, y;
		if (index - 364 >= 0)
		{
			d = GetDayInt(index);
			m = GetMonthInt(index);
			y = GetYearInt(index);
		}
		else
		{
			d = 24;
			m = 7;
			y = 2017;
		}
		days[72] = d + "/" + m + "/" + y;

		for (int i = 364; i >= 0; i--)
		{
			d -= 1;
			if (d < 1)
			{
				m -= 1;
				if (m == 0)
				{
					y -= 1;
					m = 12;
				}
				if ((m < 8 && m % 2 == 1) || (m >= 8 && m % 2 == 0))
				{
					d = 31;
				}
				else
				{
					if (m != 2)
					{
						d = 30;
					}
					else
					{
						d = 28;
					}
				}
			}
			if (i == 0)
			{
				days[i] = d + "/" + m + "/" + y;
			}
			else if (i % 5 == 0)
			{
				days[i / 5 - 1] = d + "/" + m;
			}
		}
		return days;
	}

	/// <summary>
	/// Renvoie un nouvel instant, correspondant à l'heure de la simulation.
	/// </summary>
	public static Instant GetInstant()
	{
		return (new Instant(Hour, (int)instance.minute));
	}

	/// <summary>
	/// Construit la liste des dates enregistrées.
	/// </summary>
	void SetDaysNames()
	{
		int d = (int)instance.day;
		int m = (int)instance.month;
		int y = instance.year;
		daysNames.Add(d + "/" + m + "/" + y);
		for (int i = 729; i >= 0; i--)
		{
			d -= 1;
			if (d < 1)
			{
				m -= 1;
				if (m == 0)
				{
					y -= 1;
					m = 12;
				}
				if ((m < 8 && m % 2 == 1) || (m >= 8 && m % 2 == 0))
				{
					d = 31;
				}
				else
				{
					if (m != 2)
					{
						d = 30;
					}
					else
					{
						d = 28;
					}
				}
			}
			daysNames.Add(d + "/" + m + "/" + y);
		}
		daysNames.Reverse();
	}

	/// <summary>
	/// Renvoie l'indice du jour donné, s'il est dans la liste.
	/// Sinon, renvoie -1.
	/// </summary>
	public static int GetDayIndex(string dayName)
	{
		int i = -1;
		if (instance.daysNames.Contains(dayName))
		{
			i = instance.daysNames.IndexOf(dayName);
		}
		return i;
	}
}

/// <summary>
/// Structure d'un instant, défini par une heure et une minute.
/// Le texte se construit à partir des deux.
/// </summary>
public struct Instant
{
	public string text;
	public int hour, minute;

	public void ReadTime()
	{
		text = hour + ":" + minute;
	}

	public Instant(int h, int m)
	{
		hour = h;
		minute = m;
		text = hour + ":" + minute;
	}
}