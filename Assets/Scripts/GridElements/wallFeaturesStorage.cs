using UnityEngine;

/// <summary>
/// Stocke les objets placés sur les murs en enfants du gameObject ayant ce composant.
/// </summary>
public class wallFeaturesStorage : MonoBehaviour {

	/// <summary>
	/// Réinitialise le storage en détruisant tous ses objets enfants.
	/// </summary>
	public void Load(HomeDataReader reader)
	{
		Feature[] features = GetComponentsInChildren<Feature>();
		foreach (Feature feature in features)
		{
			Destroy(feature.gameObject);
		}
	}
}
