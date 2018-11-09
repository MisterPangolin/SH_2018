using UnityEngine;

/// <summary>
/// Composant définissant une surface possible pour le placement des objets ayant le composant "PlaceableObject".
/// Ce composant est attaché au prefab "AvailableSurface", qui doit être placé en enfant du meuble donc on veut définir
/// les surfaces. On ajuste ensuite le collider du prefab pour couvrir la surface souhaitée.
/// </summary>
public class AvailableSurface : MonoBehaviour {

	public BoxCollider surface;
	public Vector3[] vertex;
	public Vector3 yOffset;

	/// <summary>
	/// Initialise le tableau "vertex" grâce aux 4 coins du collider "surface".
	/// </summary>
	void Awake()
	{
		vertex = Metrics.GetColliderVertexPositions(surface);
		yOffset = new Vector3(0f,surface.bounds.size.y,0f);
	}

	/// <summary>
	/// Renvoie true si l'objet "o" peut être placé sur la surface, false sinon.
	/// Pour pouvoir être placé, le quadrilatère servant de base à l'objet "o" doit pouvoir être contenu dans la 
	/// surface.
	/// </summary>
	public bool CheckAvailability(PlaceableObject o)
	{
		vertex = Metrics.GetColliderVertexPositions(surface);
		bool available = true;
		var oVertex = o.vertex =  Metrics.GetColliderVertexPositions(o.surface);
		float right = vertex[0].x, left = vertex[0].x, top = vertex[0].z, bottom = vertex[0].z;
		float oRight = oVertex[0].x, oLeft = oVertex[0].x, oTop = oVertex[0].z, oBottom = oVertex[0].z;

		for (int i = 1; i < vertex.Length; i++)
		{
			if (vertex[i].x < right)
			{
				right = vertex[i].x;
			}
			if (vertex[i].x > left)
			{
				left = vertex[i].x;
			}
			if (vertex[i].z < bottom)
			{
				bottom = vertex[i].z;
			}
			if (vertex[i].z > top)
			{
				top = vertex[i].z;
			}
			if (oVertex[i].x < oRight)
			{
				oRight = oVertex[i].x;
			}
			if (oVertex[i].x > oLeft)
			{
				oLeft = oVertex[i].x;
			}
			if (oVertex[i].z < oBottom)
			{
				oBottom = oVertex[i].z;
			}
			if (oVertex[i].z > oTop)
			{
				oTop = oVertex[i].z;
			}
		}
		if (right > oRight || left < oLeft || bottom > oBottom || top < oTop)
		{
			available = false;
		}

		return available;
	}

	/// <summary>
	/// Détruit tous les objets placés sur la surface.
	/// Appelée lorsque l'utilisateur détruit le meuble comportant la surface.
	/// </summary>
	public void DestroyObjects()
	{
		int count = transform.childCount;
		for (int i = 0; i < count; i++)
		{
			transform.GetChild(i).GetComponent<PlaceableObject>().DestroyObject();
		}
	}
}
