using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Affiche le FPS lorsque l'option est activée.
/// </summary>

public class DebugMode : MonoBehaviour {

	//texte du fps
	public Text fpsText;
	public float deltaTime;

	/// <summary>
	/// Active le texte si l'option est activée.
	/// </summary>
	void Awake()
	{
		if (!ApplicationParameters.debugModeOn)
		{
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Si l'option est activée, affiche à chaque nouvelle frame le fps.
	/// </summary>
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		fpsText.text = "FPS : " + Mathf.Ceil(fps);
	}
}
