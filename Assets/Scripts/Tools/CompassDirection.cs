/// <summary>
/// Système à huit directions utilisés notamment pour orienter les murs.
/// </summary>
public enum CompassDirection
{
	N, NE, E, SE, S, SW, W, NW
}

public static class CompassDirectionExtensions
{
	/// <summary>
	/// Renvoie la direction opposée.
	/// </summary>
	/// <param name="direction">Direction.</param>
	public static CompassDirection Opposite(this CompassDirection direction)
	{
		return (int)direction < 4 ? (direction + 4) : (direction - 4);
	}

	/// <summary>
	/// Renvoie la direction précédente.
	/// </summary>
	public static CompassDirection GetPrevious(this CompassDirection direction)
	{
		return (int)direction > 0 ? (direction - 1) : CompassDirection.NW;
	}

	/// <summary>
	/// Renvoie la direction suivante.
	/// </summary>
	public static CompassDirection GetNext(this CompassDirection direction)
	{
		return (int)direction < 7 ? (direction + 1) : CompassDirection.N;
	}

	/// <summary>
	/// Renvoie la deuxième direction précédant la direction donnée.
	/// </summary>
	public static CompassDirection GetSecondPrevious(this CompassDirection direction)
	{
		return (int)direction > 1 ? (direction - 2) : (direction + 6);
	}

	/// <summary>
	/// Renvoie la deuxième direction suivant la dircetion donnée.
	/// </summary>
	public static CompassDirection GetSecondNext(this CompassDirection direction)
	{
		return (int)direction < 6 ? (direction + 2) : (direction - 6);
	}
}
