using System.IO;
using UnityEngine;

/// <summary>
/// Permet d'écrire les informations utiles pour la sauvegarde de la maison.
/// </summary>
public class HomeDataWriter
{
	BinaryWriter writer;

	/// <summary>
	/// Initialise une nouvelle instance de la classe.
	/// </summary>
	public HomeDataWriter(BinaryWriter writer)
	{
		this.writer = writer;
	}

	/// <summary>
	/// Écrit un flottant dans "writer". 
	/// </summary>
	public void Write(float value)
	{
		writer.Write(value);
	}

	/// <summary>
	/// Écrit un entier dans "writer".
	/// </summary>
	/// <param name="value">Value.</param>
	public void Write(int value)
	{
		writer.Write(value);
	}

	/// <summary>
	/// Écrit un quaternion (rotation) dans "writer".
	/// </summary>
	public void Write(Quaternion value)
	{
		writer.Write(value.x);
		writer.Write(value.y);
		writer.Write(value.z);
		writer.Write(value.w);
	}

	/// <summary>
	/// Écrit un point dans "writer".
	/// </summary>
	public void Write(Vector3 value)
	{
		writer.Write(value.x);
		writer.Write(value.y);
		writer.Write(value.z);
	}

	/// <summary>
	/// Écrit une couleur dans "writer".
	/// </summary>
	/// <param name="value">Value.</param>
	public void Write(Color value)
	{
		writer.Write(value.r);
		writer.Write(value.g);
		writer.Write(value.b);
		writer.Write(value.a);
	}

	/// <summary>
	/// Écrit un booleen dans "writer".
	/// </summary>
	public void Write(bool value)
	{
		writer.Write(value);
	}

	/// <summary>
	/// Écrit un byte dans "writer".
	/// </summary>
	public void Write(byte value)
	{
		writer.Write(value);
	}

	/// <summary>
	/// Écrit une chaîne de caractères dans "writer".
	/// </summary>
	public void Write(string value)
	{
		writer.Write(value);
	}
}
