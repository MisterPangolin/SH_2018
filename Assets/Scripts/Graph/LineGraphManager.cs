using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class LineGraphManager : MonoBehaviour {

	// assets permettant de construire de graphique
	public GameObject linerenderer;
	public GameObject pointer;
	public GameObject pointerRed;
	public GameObject pointerGreen;
	public GameObject HolderPrefb;
	public GameObject holder;
	public GameObject xLineNumber;
	public Material redMat;
	public Material greenMat;

	//textes du graphique
	public Text topValue;
	public TextMesh title, labelX, labelY;
	public TextMesh[] yValues;
	string[] days;

	//données actuelles du graphique
	List<GraphData> graphData1 = new List<GraphData>();
	List<GraphData> graphData2 = new List<GraphData>();

	//informations sur les données utilisées
	float highestValue;
	float lowestValue;
	bool twoDataSets;
	Data data, data2;
	int decimals, currentLength;

	//Placement des éléments du graphique
	public Transform origin, endX, endY;
	float lrWidth = 0.1f;
	int dataGap;
	float lengthX, lengthY;

	//Gestion des coroutines
	IEnumerator redGraph, greenGraph;
	bool redStopped = true, greenStopped = true;

	//Date de référence, texte sous le graphique
	public Text timeText;

	/// <summary>
	/// Récupère l'indice de la date de référence dans la liste des jours utilisés de "TimeClock.cs"
	/// </summary>
	int GetIndex()
	{
		return TimeClock.GetDayIndex(timeText.text);
	}

	/// <summary>
	/// Change le premier jeu de données du graphique.
	/// Modifie l'affichage pour ne faire apparaître qu'un jeu de données.
	/// </summary>
	public void Open(int length, Data newData)
	{
		twoDataSets = false;
		data = newData;
		Open(length);
	}

	/// <summary>
	/// Change les deux jeux de données du graphique.
	/// Modifie l'affichage pour faire apparaître ces deux jeux.
	/// </summary>
	public void Open(int length, Data newData, Data newData2)
	{
		twoDataSets = true;
		data = newData;
		data2 = newData2;
		Open(length);
	}

	/// <summary>
	/// Met à jour le graphique pour afficher les données enregistrées, selon la plage de temps voulue.
	/// L'entier "length" indique la longueur de la plage voulue: 24 pour un jour, 7 pour une semaine, 31 pour un mois,
	/// 365 pour un an.
	/// Si la valeur de "length" vaut -1, la longueur de la plage ne doit pas changer.
	/// La date de référence est récupérée grâce à "getIndex()".
	/// </summary>
	public void Open(int length = -1)
	{
		var index = GetIndex();
		ClearGraph();
		highestValue = data.max;
		lowestValue = data.min;
		decimals = data.decimals;
		if (length != -1)
		{
			currentLength = length;
		}
		else
		{
			length = currentLength;
		}
		if (length != 24)
		{
			if (twoDataSets)
			{
				for (int i = 0; i < length; i++)
				{
					var gd = new GraphData();
					gd.value = data.subData[i].average;
					graphData1.Add(gd);
					var gd2 = new GraphData();
					gd2.value = data2.subData[i].average;
					graphData2.Add(gd2);
				}
			}
			else
			{
				if ((index+1) - length >= 0)
				{
					for (int i = (index+1) - length; i <= index; i++)
					{
						var gd = new GraphData();
						gd.value = data.subData[i].average;
						graphData1.Add(gd);
					}
				}
				else
				{
					for (int i = 0; i < length; i++)
					{
						var gd = new GraphData();
						gd.value = data.subData[i].average;
						graphData1.Add(gd);
					}
				}
			}
			if (length == 7)
			{
				days = TimeClock.GetWeekDays(index);
			}
			else if (length == 31)
			{
				days = TimeClock.GetMonthDays(index);
			}
			else if (length == 73)
			{
				days = TimeClock.GetYearDays(index);
			}
			title.text = "du " + days[0] + " au " + days[days.Length - 1];
			if (length == 31)
			{
				days[0] = InsertLineBreak(days[0]);
				days[days.Length - 1] = InsertLineBreak(days[days.Length - 1]);
			}
		}
		else
		{
			if (twoDataSets)
			{
				for (int i = 0; i < length; i++)
				{
					var gd = new GraphData();
					gd.value = data.subData[i].average;
					graphData1.Add(gd);
					var gd2 = new GraphData();
					gd2.value = data2.subData[i].average;
					graphData2.Add(gd2);
				}
			}
			else
			{
				int l = data.subData[index].values.Count;
				if (l < 24 && index > 0)
				{
					title.text = TimeClock.GetDay(index - 1) + " et " + TimeClock.GetDay(index);
					days = new string[24];
					for (int i = 24; i > l; i--)
					{
						var gd = new GraphData();
						gd.value = data.subData[index-1].values[24 - i + l];
						graphData1.Add(gd);
						days[24 - i] = data.subData[index-1].instants[24 - i + l].text;
					}
					for (int i = l; i > 0; i--)
					{
						var gd = new GraphData();
						gd.value = data.subData[index].values[l - i];
						graphData1.Add(gd);
						days[24 - i] = data.subData[index].instants[l - i].text;
					}
				}
				else
				{
					title.text = "Le " + TimeClock.GetDay(index);
					days = new string[l];
					for (int i = l; i > 0; i--)
					{
						var gd = new GraphData();
						gd.value = data.subData[index].values[l - i];
						graphData1.Add(gd);
						days[l - i] = data.subData[index].instants[l - i].text;
					}
				}
			}
		}

		if (data.prm.isABool && length == 24)
		{
			yValues[0].text = "non -";
			for (int i = 1; i < yValues.Length - 1; i++)
			{
				yValues[i].text = "";
			}
			yValues[yValues.Length - 1].text = "oui -";
		}
		else
		{
			float diff = (highestValue - lowestValue) / (yValues.Length - 1);
			yValues[0].text = Math.Round(lowestValue, decimals) + " -";
			for (int i = 1; i < yValues.Length; i++)
			{
				yValues[i].text = Math.Round(diff * i + lowestValue, decimals) + " - ";
			}
		}

		lengthX = endX.position.x - origin.position.x;
		lengthY = endY.position.y - origin.position.y;

		ShowGraph();
	}

	/// <summary>
	/// Ajoute une donnée au premier jeu de données.
	/// </summary>
	public void AddPlayer1Data(int value){
		var gd = new GraphData();
		gd.value = value;
		graphData1.Add(gd);
	}

	/// <summary>
	/// Ajoute une donnée au second jeu de données.
	/// </summary>
	public void AddPlayer2Data(int value){
		var gd = new GraphData();
		gd.value = value;
		graphData2.Add(gd);
	}

	/// <summary>
	/// Prépare le graphique pour les nouvelles données et débute les coroutines.
	/// </summary>
	public void ShowGraph(){

		if(graphData1.Count >= 1)
		{
			holder = Instantiate(HolderPrefb, Vector3.zero, Quaternion.identity) as GameObject;
			holder.name = "h2";
			holder.transform.SetParent(origin, true);

			GraphData[] gd1 = new GraphData[graphData1.Count];
			if (!data.prm.isABool || currentLength != 24)
			{
				for (int i = 0; i < graphData1.Count; i++)
				{
					var gd = new GraphData();
					gd.value = graphData1[i].value;
					gd1[i] = gd;
				}
			}
			else
			{
				for (int i = 0; i < graphData1.Count; i++)
				{
					var gd = new GraphData();
					if (graphData1[i].value > 0.001f)
					{
						gd.value = highestValue;
					}
					else
					{
						gd.value = 0;
					}
					gd1[i] = gd;
				}
			}

			dataGap = GetDataGap(graphData1.Count);

			int dataCount = 0;
			int gapLength = 1;
			float gap = 1.0f;
			bool flag = false;

			while (dataCount < graphData1.Count)
			{
				if (dataGap > 1)
				{

					if ((dataCount + dataGap) == graphData1.Count)
					{

						dataCount += dataGap - 1;
						flag = true;
					}
					else if ((dataCount + dataGap) > graphData1.Count && !flag)
					{

						dataCount = graphData1.Count - 1;
						flag = true;
					}
					else {
						dataCount += dataGap;
						if (dataCount == (graphData1.Count - 1))
							flag = true;
					}
				}
				else
					dataCount += dataGap;

				gapLength++;
			}

			gap = lengthX / graphData1.Count;

			if (graphData2.Count >= 1)
			{
				GraphData[] gd2 = new GraphData[graphData2.Count];
				for (int i = 0; i < graphData2.Count; i++)
				{
					var gd = new GraphData();
					gd.value = graphData2[i].value;
					gd2[i] = gd;
				}
				redGraph = BarGraphRed(gd1, gap);
				StartCoroutine(redGraph);
				greenGraph = BarGraphGreen(gd2, gap);
				StartCoroutine(greenGraph);
			}
			else
			{
				redGraph = BarGraphRed(gd1, gap);
				StartCoroutine(redGraph);
			}
		}
	}

	/// <summary>
	/// Arrête les coroutines si elles tournent encore, détruit l'objet "holder" et nettoie les listes des données.
	/// </summary>
	public void ClearGraph(){
		if (!redStopped)
		{
			StopCoroutine(redGraph);
		}
		if (!greenStopped)
		{
			StopCoroutine(greenGraph);
		}
		if (holder)
		{
			Destroy(holder);
		}
		graphData1 = new List<GraphData>();
		graphData2 = new List<GraphData>();
	}

	/// <summary>
	/// Calcule la valeur de l'espace sur l'axe des abscisses entre deux données du graphique
	/// </summary>
	int GetDataGap(int dataCount){
		int value = 1;
		int num = 0;
		while((dataCount-(40+num)) >= 0){
			value+= 1;
			num+= 20;
		}
		return value;
	}

	/// <summary>
	/// Coroutine permettant d'afficher les données du premier jeu de données.
	/// Affiche les jours sur l'axe des abscisses.
	/// </summary>
	IEnumerator BarGraphRed(GraphData[] gd,float gap)
	{
		redStopped = false;
		float xIncrement = 0;
		int dataCount = 0;
		float diff = (highestValue - lowestValue);
		var startpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+(gd[dataCount].value - lowestValue)/ diff * lengthY),(origin.position.z));//origin.position;//

		while(dataCount < gd.Length)
		{
			var endpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+(gd[dataCount].value - lowestValue) / diff * lengthY),(origin.position.z));
			startpoint = new Vector3(startpoint.x,startpoint.y,origin.position.z);
			var p = Instantiate(pointer, new Vector3(startpoint.x, startpoint.y, origin.position.z),Quaternion.identity) as GameObject;
			p.transform.parent = holder.transform;

			if (currentLength != 73 && currentLength != 24)
			{
				var lineNumber = Instantiate(xLineNumber, new Vector3(origin.position.x + xIncrement, origin.position.y - 0.18f, origin.position.z), Quaternion.identity) as GameObject;
				lineNumber.transform.parent = holder.transform;
				lineNumber.GetComponent<TextMesh>().text = "\n" + days[dataCount];
			}
			else if (currentLength != 24)
			{
				if (dataCount != 70 && (dataCount % 5 == 0 || dataCount == 72))
				{
					var lineNumber = Instantiate(xLineNumber, new Vector3(origin.position.x + xIncrement, origin.position.y - 0.18f, origin.position.z), Quaternion.identity) as GameObject;
					lineNumber.transform.parent = holder.transform;
					lineNumber.GetComponent<TextMesh>().text = "\n" + InsertLineBreak(days[dataCount]);
				}
			}
			else
			{
				var lineNumber = Instantiate(xLineNumber, new Vector3(origin.position.x + xIncrement, origin.position.y - 0.18f, origin.position.z), Quaternion.identity) as GameObject;
				lineNumber.transform.parent = holder.transform;
				if (dataCount % 2 == 1)
				{
					lineNumber.GetComponent<TextMesh>().text = "\n\n" + InsertLineBreak(days[dataCount]);
				}
				else
				{
					lineNumber.GetComponent<TextMesh>().text = "\n" + InsertLineBreak(days[dataCount]);
				}
			}

			var lineObj = Instantiate(linerenderer,startpoint,Quaternion.identity) as GameObject;
			lineObj.transform.parent = holder.transform;
			lineObj.name = dataCount.ToString();
			
			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
			
			lineRenderer.material = redMat;
			lineRenderer.startWidth = lrWidth;
			lineRenderer.endWidth = lrWidth;
			lineRenderer.positionCount = 2;

			while(Vector3.Distance(p.transform.position,endpoint) > 0.2f)
			{
				float step = 100 * Time.deltaTime;
				p.transform.position = Vector3.MoveTowards(p.transform.position, endpoint, step);
				lineRenderer.SetPosition(0, startpoint);
				lineRenderer.SetPosition(1, p.transform.position);
				
				yield return null;
			}
			
			lineRenderer.SetPosition(0, startpoint);
			lineRenderer.SetPosition(1, endpoint);
			
			
			p.transform.position = endpoint;
			var pointered = Instantiate(pointerRed,endpoint,pointerRed.transform.rotation) as GameObject ;
			pointered.transform.parent = holder.transform;
			startpoint = endpoint;

			dataCount += 1;

			xIncrement+= gap;
			
			yield return null;
		}
		redStopped = true;
	}

	/// <summary>
	/// Coroutine permettant d'afficher les données du second jeu de données.
	/// </summary>
	IEnumerator BarGraphGreen(GraphData[] gd, float gap)
	{
		greenStopped = false;
		float xIncrement = 0;
		int dataCount = 0;
		bool flag = false;
		float diff = (highestValue - lowestValue);
		var startpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+(gd[dataCount].value - lowestValue) / diff * lengthY),(origin.position.z));
		while(dataCount < gd.Length)
		{
			var endpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+(gd[dataCount].value - lowestValue) / diff * lengthY),(origin.position.z));
			startpoint = new Vector3(startpoint.x,startpoint.y,origin.position.z);
			var p = Instantiate(pointer, new Vector3(startpoint.x, startpoint.y, origin.position.z),Quaternion.identity) as GameObject;
			p.transform.parent = holder.transform;

			var lineObj = Instantiate(linerenderer,startpoint,Quaternion.identity) as GameObject;
			lineObj.transform.parent = holder.transform;
			lineObj.name = dataCount.ToString();
			
			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
			
			lineRenderer.material = greenMat;
			lineRenderer.startWidth = lrWidth;
			lineRenderer.endWidth = lrWidth;
			lineRenderer.positionCount = 2;

			while(Vector3.Distance(p.transform.position,endpoint) > 0.2f)
			{
				float step = 100 * Time.deltaTime;
				p.transform.position = Vector3.MoveTowards(p.transform.position, endpoint, step);
				lineRenderer.SetPosition(0, startpoint);
				lineRenderer.SetPosition(1, p.transform.position);
				
				yield return null;
			}
			
			lineRenderer.SetPosition(0, startpoint);
			lineRenderer.SetPosition(1, endpoint);
			
			
			p.transform.position = endpoint;
			var pointerblue = Instantiate(pointerGreen,endpoint,pointerGreen.transform.rotation) as GameObject; 
			pointerblue.transform.parent = holder.transform;
			startpoint = endpoint;

			if(dataGap > 1){
				if((dataCount+dataGap) == gd.Length){
					dataCount+=dataGap-1;
					flag = true;
				}
				else if((dataCount+dataGap) > gd.Length && !flag){
					dataCount =	gd.Length-1;
					flag = true;
				}
				else{
					dataCount+=dataGap;
					if(dataCount == (gd.Length-1))
						flag = true;
				}
			}
			else
				dataCount+=dataGap;

			xIncrement+= gap;
			
			yield return null;
		}
		greenStopped = true;
	}

	/// <summary>
	/// Donnée du graphique, ayant un unique attribut, sa valeur.
	/// </summary>
	public class GraphData
	{
		public float value;
	}

	/// <summary>
	/// Insère un saut de ligne dans la chaîne de caractère "s" avant chaque caractère "/".
	/// </summary>
	string InsertLineBreak(string s)
	{
		if (s.Contains("/"))
		{
			int i = s.IndexOf("/", StringComparison.CurrentCulture);
			if (s.Substring(i + 1).Contains("/"))
			{
				int i2 = s.LastIndexOf("/", StringComparison.CurrentCulture);
				s = s.Substring(0, i) + "\n" + s.Substring(i, i2 - i) + "\n" + s.Substring(i2);
			}
			else
			{
				s = s.Substring(0, i) + "\n" + s.Substring(i);
			}
		}
		return s;
	}
}
