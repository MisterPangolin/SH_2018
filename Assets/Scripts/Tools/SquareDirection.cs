/// <summary>
/// Système à 4 directions utilisés pour les cellules et les features.
/// </summary>
public enum SquareDirection
{
	N, W, S, E
}

public static class SquareDirectionExtensions
{
	/// <summary>
	/// Renvoie la direction opposée.
	/// </summary>
	public static SquareDirection Opposite(this SquareDirection direction)
	{
		return (int)direction < 2 ? (direction + 2) : (direction - 2);
	}

	/// <summary>
	/// Renvoie la direction suivante.
	/// </summary>
	public static SquareDirection GetNext(this SquareDirection direction)
	{
		return (int)direction < 3 ? (direction + 1) : 0;
	}
}
