using UnityEngine;

/// <summary>
/// Classe définissant un objet à sauvegarder.
/// Les classes "persistentStorage", "feature", "placeableObject", "window" et "door" héritent de cette classe.
/// </summary>
[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour {

	int id = int.MinValue;
	/// <summary>
	/// Renvoie ou modifie la référence de l'objet à sauvegarder.
	/// Renvoie un message d'erreur si la nouvelle référence n'est pas acceptable.
	/// </summary>
	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			if (id == int.MinValue && value != int.MinValue)
			{
				id = value;
			}
			else
			{
				Debug.LogError("Not allowed to change shapeId");
			}
		}
	}

	/// <summary>
	/// Sauvegarde la position, rotation et taille de l'objet.
	/// </summary>
	public virtual void Save(HomeDataWriter writer)
	{
		writer.Write(transform.localPosition);
		writer.Write(transform.localRotation);
		writer.Write(transform.localScale);
	}

	/// <summary>
	/// Charge la position, rotation et taille de l'objet.
	/// </summary>
	public virtual void Load(HomeDataReader reader)
	{
		transform.localPosition = reader.ReadVector3();
		transform.localRotation = reader.ReadQuaternion();
		transform.localScale = reader.ReadVector3();
	}
}
