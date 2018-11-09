using System.Collections.Generic;

/// <summary>
/// Classe utile pour les meshs afin de définir des piles.
/// </summary>
public static class ListPool<T>
{

	static Stack<List<T>> stack = new Stack<List<T>>();

	/// <summary>
	/// Renvoie le dernier élément de la pile.
	/// </summary>
	public static List<T> Get()
	{
		if (stack.Count > 0)
		{
			return stack.Pop();
		}
		return new List<T>();
	}

	/// <summary>
	/// Empile un élément.
	/// </summary>
	public static void Add(List<T> list)
	{
		list.Clear();
		stack.Push(list);
	}
}