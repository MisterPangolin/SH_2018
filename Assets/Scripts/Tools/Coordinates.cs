using UnityEngine;

/// <summary>
/// Système de coordonnées des éléments de la grille.
/// </summary>
[System.Serializable]
public struct Coordinates
{

	[SerializeField]
	private int x, z;

	/// <summary>
	/// Renvoie l'abscisse.
	/// </summary>
	public int X { 
		get
		{
			return x;
		}
		}

	/// <summary>
	/// Renvoie l'ordonnée.
	/// </summary>
	public int Z { get
		{
			return z;
		}
		}

	/// <summary>
	/// Attribue les coordonnées.
	/// </summary>
	public Coordinates(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	/// <summary>
	/// Crée de nouvelles coordonnées.
	/// </summary>
	public static Coordinates FromOffsetCoordinates(int x, int z)
	{
		return new Coordinates(x, z);
	}

	/// <summary>
	/// Renvoie une chaîne de caractères donnant les coordonnées.
	/// </summary>
	public override string ToString()
	{
		return "(" + X + ", " + Z + ")";
	}

	/// <summary>
	/// Renvoie une chaîne de caractères sur deux lignes donnant les coordonnées.
	/// </summary>
	/// 
	public string ToStringOnSeparateLines()
	{
		return X + "\n" + Z;
	}

	/// <summary>
	/// Détermine les coordonnées selon la position donnée.
	/// Appelée pour déterminer les coordonnées des cellules.
	/// </summary>
	public static Coordinates FromPosition(Vector3 position)
	{
		float x = position.x / (Metrics.innerRadius * 2f);
		float z = position.z / (Metrics.innerRadius * 2f);

		int iX = Mathf.RoundToInt(x);
		int iZ = Mathf.RoundToInt(z);

		return new Coordinates(iX, iZ);
	}

	/// <summary>
	/// Détermine les coordonnées selon la position donnée.
	/// Appelée pour déterminer les coordonnées des points.
	/// </summary>
	public static Coordinates FromDotPosition(Vector3 position)
	{
		float x = position.x / (Metrics.innerRadius);
		float z = position.z / (Metrics.innerRadius);
		int iX = Mathf.RoundToInt(x) + 1;
		int iZ = Mathf.RoundToInt(z) + 1;
		return new Coordinates(iX, iZ);
	}
}
