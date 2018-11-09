using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" afin de permettre le positionnement des objets occupant un seul 16eme de cellule.
/// </summary>
public class Feature1x1Manager : Feature{

	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc1x1;
	}

	/// <summary>
	/// Calcule la position de l'objet.
	/// </summary>
	public override void CalculatePosition(int part)
	{
		float halfQuarter = Metrics.innerRadius / 4f;
		switch (part)
		{
			case 0:
				transform.position = new Vector3(-3 * halfQuarter, height / 2f, 3 * halfQuarter);
				break;
			case 1:
				transform.position = new Vector3(-halfQuarter, height / 2f, 3 * halfQuarter);
				break;
			case 2:
				transform.position = new Vector3(halfQuarter, height / 2f, 3 * halfQuarter);
				break;
			case 3:
				transform.position = new Vector3(3 * halfQuarter, height / 2f, 3 * halfQuarter);
				break;
			case 4:
				transform.position = new Vector3(-3 * halfQuarter, height / 2f, halfQuarter);
				break;
			case 5:
				transform.position = new Vector3(-halfQuarter, height / 2f, halfQuarter);
				break;
			case 6:
				transform.position = new Vector3(halfQuarter, height / 2f, halfQuarter);
				break;
			case 7:
				transform.position = new Vector3(3 * halfQuarter, height / 2f, halfQuarter);
				break;
			case 8:
				transform.position = new Vector3(-3 * halfQuarter, height / 2f, -halfQuarter);
				break;
			case 9:
				transform.position = new Vector3(-halfQuarter, height / 2f, -halfQuarter);
				break;
			case 10:
				transform.position = new Vector3(halfQuarter, height / 2f, -halfQuarter);
				break;
			case 11:
				transform.position = new Vector3(3 * halfQuarter, height / 2f, -halfQuarter);
				break;
			case 12:
				transform.position = new Vector3(-3 * halfQuarter, height / 2f, -3 * halfQuarter);
				break;
			case 13:
				transform.position = new Vector3(-halfQuarter, height / 2f, -3 * halfQuarter);
				break;
			case 14:
				transform.position = new Vector3(halfQuarter, height / 2f, -3 * halfQuarter);
				break;
			default:
				transform.position = new Vector3(3 * halfQuarter, height / 2f, -3 * halfQuarter);
			break;
		}
		if (featureType == FeatureType.celling)
		{
			transform.position += new Vector3(0f, Metrics.wallsElevation, 0f);
		}
		AdjustPosition();
	}
}
