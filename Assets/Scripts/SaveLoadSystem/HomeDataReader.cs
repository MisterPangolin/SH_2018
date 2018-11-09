using System.IO;
using UnityEngine;

/// <summary>
/// Permet de lire les informations utiles pour le chargement de la maison. Contrairement à "HomeDataWriter" où il n'est
/// pas nécessaire d'indiquer le type de valeur sauvegardé, il faut pour le chargement connaître la structure du fichier
/// de sauvegarde.
/// </summary>
public class HomeDataReader
{
	BinaryReader reader;
	/// <summary>
	/// Initialise une nouvelle instance de la classe.
	/// </summary>
	public HomeDataReader(BinaryReader reader)
	{
		this.reader = reader;
	}

	/// <summary>
	/// Lis un flottant et le renvoie.
	/// </summary>
	public float ReadFloat()
	{
		return reader.ReadSingle();
	}
	/// <summary>
	/// Lis un entier et le renvoie.
	/// </summary>
	public int ReadInt()
	{
		return reader.ReadInt32();
	}

	/// <summary>
	/// Lis un quaternion et le renvoie.
	/// </summary>
	public Quaternion ReadQuaternion()
	{
		Quaternion value;
		value.x = reader.ReadSingle();
		value.y = reader.ReadSingle();
		value.z = reader.ReadSingle();
		value.w = reader.ReadSingle();
		return value;
	}

	/// <summary>
	/// Lis un point et le renvoie.
	/// </summary>
	public Vector3 ReadVector3()
	{
		Vector3 value;
		value.x = reader.ReadSingle();
		value.y = reader.ReadSingle();
		value.z = reader.ReadSingle();
		return value;
	}

	/// <summary>
	/// Lis une couleur et la renvoie.
	/// </summary>
	public Color ReadColor()
	{
		Color value;
		value.r = reader.ReadSingle();
		value.g = reader.ReadSingle();
		value.b = reader.ReadSingle();
		value.a = reader.ReadSingle();
		return value;
	}

	/// <summary>
	/// Lis un booleen et le renvoie.
	/// </summary>
	public bool ReadBool()
	{
		return reader.ReadBoolean();
	}

	/// <summary>
	/// Lis un byte et le renvoie.
	/// </summary>
	public byte ReadByte()
	{
		return reader.ReadByte();
	}

	/// <summary>
	/// Lis une chaîne de caractères et la renvoie.
	/// </summary>
	public string ReadString()
	{
		return reader.ReadString();
	}
}
