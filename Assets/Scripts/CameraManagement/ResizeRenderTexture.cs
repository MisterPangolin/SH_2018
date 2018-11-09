using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utilisée pour le mode 1ere personne afin d'améliorer les performances du simulateur.
/// Crée une texture, de dimensions égales à celles de l'écran divisées par deux, où est sauvegardé le rendu de la
/// caméra.
/// Cette texture est ensuite appliquée à une image qui sera rendue par une seconde caméra, mise en enfant de
/// "CameraPalyer".
/// Cette technique permet d'observer la maison dans une résolution inférieure à celle de l'écran mais toujours
/// agréable, tout en gardant pour l'UI la même résolution que celle de l'écran.
/// Composant de "CameraPlayer".
/// </summary>

public class ResizeRenderTexture : MonoBehaviour
{
	//camera rendant la texture
	public new Camera camera;

	//dimensions de l'écran
	Vector2 resolution;

	//texture
	public RawImage image;

	//options, dictant si la résolution doit être réduite ou non
	bool halfResolutionOn;

	/// <summary>
	/// Sauvegarde la résolution de l'écran et crée une texture.
	/// </summary>
	void Awake()
	{
		halfResolutionOn = ApplicationParameters.halfResolutionOn;
		resolution = new Vector2(Screen.width, Screen.height);
		Resize();
	}

	/// <summary>
	/// Dès que la résolution de l'écran change, met à jour le vecteur "resolution" et crée une nouvelle texture.
	/// Les textures sont temporaires donc non sauvegardées ensuite.
	/// </summary>
	void Update()
	{
		if (halfResolutionOn != ApplicationParameters.halfResolutionOn)
		{
			halfResolutionOn = !halfResolutionOn;
			Resize();
		}
		   
		if (resolution.x != Screen.width || resolution.y != Screen.height)
		{
			Resize();

			resolution.x = Screen.width;
			resolution.y = Screen.width;
		}
	}

	/// <summary>
	/// Crée une nouvelle texture à appliquer à "image", adaptée aux nouvelles dimensions de l'écran.
	/// </summary>
	void Resize()
	{
		if (camera.targetTexture != null)
		{
			camera.targetTexture.Release();
		}
		if (halfResolutionOn)
		{
			camera.targetTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
			image.texture = camera.targetTexture;
		}
		else
		{
			camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
			image.texture = camera.targetTexture;
		}
	}
}
