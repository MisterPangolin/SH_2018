using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Seule classe utilisée dans la scène "RenderStudio".
/// Permet d'enregistrer dans le dossier des fichiers persistants, aussi utilisé pour les sauvegardes, une image de
/// l'objet filmé par la caméra en png.
/// </summary>
public class RenderToPng : MonoBehaviour {

	//image de la caméra
	public RenderTexture tex;

	//texture où s'enregistre l'image de la caméra
	public Texture2D myTexture;

	//nom du fichier de sauvegarde de l'image
	public string texName;
	public InputField nameField;

	/// <summary>
	/// Initialise le texte du champ d'écriture.
	/// </summary>
	void Awake()
	{
		nameField.text = texName;
	}

	/// <summary>
	/// Fonction du bouton "Save", permettant de sauvegarder l'image de la caméra.
	/// Il faut s'assurer que le nom de l'image ait bien été changée pour ne pas écraser une autre image.
	/// </summary>
	public void SaveTexture()
	{
		myTexture = ToTexture2D(tex);
	}

	/// <summary>
	/// Transforme la texture "RenderTexture" en une image png.
	/// </summary>
	Texture2D ToTexture2D(RenderTexture rTex)
	{
		var texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
		RenderTexture.active = rTex;
		texture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		texture.Apply();

		byte[] bytes;
		bytes = texture.EncodeToPNG();
		System.IO.File.WriteAllBytes(ImageLocation(), bytes);

		return texture;
	}

	/// <summary>
	/// Renvoie l'emplacement de l'image.
	/// </summary>
	string ImageLocation()
	{
		string path = Application.persistentDataPath + "/" + texName + ".png";
		return path;
	}

	/// <summary>
	/// Fonction du champ d'écriture.
	/// Change le nom du fichier png lorsque l'utilisateur écrit dans le champ.
	/// </summary>
	public void SetName()
	{
		texName = nameField.text;
	}

}
