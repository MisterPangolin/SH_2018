using UnityEngine;

/// <summary>
/// Classe permettant de modifier les meshs "walls" pour prendre en considération la présence des fenêtres.
/// </summary>
public class WindowsManager : MonoBehaviour {

	//épaisseur supplémentaire ajoutée au mur
	Vector3 offset = Metrics.offset;

	/// <summary>
	/// Méthode pour ajouter la fenêtre à travers un mur. Est appelée lorsque le mesh est triangulé.
	/// Elle permet de créer un trou dans le mesh de mêmes dimensions que la fenêtre.
	/// </summary>
	public void AddWindow(Wall wall, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
	{
		float elevation = wall.Elevation;
		var vElevation = new Vector3(0f, elevation, 0f) + offset;
		Color colorLeft = wall.ColorLeft;
		Color colorRight = wall.ColorRight;

		walls.AddQuad(v1 + vElevation, v2 + vElevation, v3 + vElevation, v4 + vElevation);
		walls.AddQuad(v2 + offset, v1 + offset, v4 + offset, v3 + offset);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		Vector3 middleLeft = Metrics.GetMiddle(v1, v3);
		Vector3 middleRight = Metrics.GetMiddle(v2, v4);

		Vector3 v24, v42, v24w, v42w, v24w2, v42w2, v13, v31, v13w, v31w, v13w2, v31w2;

		Vector3 vUnit;
		float rotation, width, heightw, heightw2;

		Window window = wall.o_Window.GetComponent<Window>();
		width = window.width;
		heightw = window.heightw;
		heightw2 = window.heightw2;
		switch (direction)
		{
			case CompassDirection.N:
				vUnit = new Vector3(0f, 0f, 1f);
				rotation = -90f;
				break;
			case CompassDirection.NE:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, Mathf.Sqrt(2) / 2f);
				rotation = -45f;
				break;
			case CompassDirection.E:
				vUnit = new Vector3(1f, 0f, 0f);
				rotation = 0f;
				break;
			default:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, -Mathf.Sqrt(2) / 2f);
				rotation = 45f;
				break;
		}

		v24 = middleRight + vUnit * (-width);
		v42 = middleRight + vUnit * width;

		v24w = v24 + new Vector3(0f, heightw, 0f);
		v42w = v42 + new Vector3(0f, heightw, 0f);
		v24w2 = v24 - new Vector3(0f, heightw2, 0f);
		v42w2 = v42 - new Vector3(0f, heightw2, 0f);

		v13 = middleLeft + vUnit *(-width);
		v31 = middleLeft + vUnit * width;

		v13w = v13 + new Vector3(0f, heightw, 0f);
		v31w = v31 + new Vector3(0f, heightw, 0f);
		v13w2 = v13 - new Vector3(0f, heightw2, 0f);
		v31w2 = v31 - new Vector3(0f, heightw2, 0f);

		walls.AddQuad(v2 + offset, v24 + offset, v2 + vElevation, v24 + vElevation);
		walls.AddQuad(v24 + offset, v42 + offset, v24w, v42w);
		walls.AddQuad(v24w2 + vElevation, v42w2 + vElevation, v24 + vElevation, v42 + vElevation);
		walls.AddQuad(v42 + offset, v4 + offset, v42 + vElevation, v4 + vElevation);

		walls.AddQuad(v3 + offset, v31 + offset, v3 + vElevation, v31 + vElevation);
		walls.AddQuad(v31 + offset, v13 + offset, v31w, v13w);
		walls.AddQuad(v31w2 + vElevation, v13w2 + vElevation, v31 + vElevation, v13 + vElevation);
		walls.AddQuad(v13 + offset, v1 + offset, v13 + vElevation, v1 + vElevation);

		walls.AddQuad(v24w, Metrics.GetMiddle(v24w, v13w), v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w, v13w), v13w, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation);
		walls.AddQuad(v31w, Metrics.GetMiddle(v31w, v42w), v31w2 + vElevation, Metrics.GetMiddle(v31w2, v42w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v31w, v42w), v42w, Metrics.GetMiddle(v31w2, v42w2) + vElevation, v42w2 + vElevation);
		walls.AddQuad(v13w, Metrics.GetMiddle(v13w, v24w), v31w, Metrics.GetMiddle(v31w, v42w));
		walls.AddQuad(Metrics.GetMiddle(v13w, v24w), v24w, Metrics.GetMiddle(v31w, v42w), v42w);

		walls.AddQuad(v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v42w2 + vElevation, Metrics.GetMiddle(v42w2, v31w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation, Metrics.GetMiddle(v42w2, v31w2) + vElevation, v31w2 + vElevation);

		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);

		//overview
		walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
		walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		Transform t_window = wall.o_Window.transform;
		t_window.position = Metrics.GetMiddle(v13w, v42w2 + vElevation) + new Vector3(0f, wall.level * Metrics.wallsElevation, 0f);
		t_window.eulerAngles = new Vector3(t_window.eulerAngles.x, rotation, t_window.eulerAngles.z);
		t_window.SetParent(wall.transform);
	}

	/// <summary>
	/// Triangule le mur en créant un trou pour la fenêtre traversant deux murs attachées à ce mur.
	/// </summary>
	public void AddOutgoingWindow(Wall wall, Vector3 v1, Vector3 v2,Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
	{
		float elevation = wall.Elevation;
		var vElevation = new Vector3(0f, elevation, 0f) + offset;
		Color colorLeft = wall.ColorLeft;
		Color colorRight = wall.ColorRight;

		walls.AddQuad(v1 + vElevation, v2 + vElevation, v3 + vElevation, v4 + vElevation);
		walls.AddQuad(v2 + offset, v1 + offset, v4 + offset, v3 + offset);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		Vector3 vUnit;
		float rotation, width, heightw, heightw2;

		Window window = wall.o_Window.GetComponent<Window>();
		width = window.width;
		heightw = window.heightw;
		heightw2 = window.heightw2;
		switch (direction)
		{
			case CompassDirection.N:
				vUnit = new Vector3(0f, 0f, 1f);
				rotation = -90f;
				break;
			case CompassDirection.NE:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, Mathf.Sqrt(2) / 2f);
				rotation = -45f;
				break;
			case CompassDirection.E:
				vUnit = new Vector3(1f, 0f, 0f);
				rotation = 0f;
				break;
			default:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, -Mathf.Sqrt(2) / 2f);
				rotation = 45f;
				break;
		}

		Vector3 v13 = -(width - Metrics.dotRadius) * vUnit + wall.neighbor.Position + Metrics.dotEdges[(int)direction.Opposite() + 1];
		Vector3 v24 = -(width - Metrics.dotRadius) * vUnit + wall.neighbor.Position + Metrics.dotEdges[(int)direction.Opposite()];

		Vector3 v13w, v13w2, v24w, v24w2, v3w, v3w2, v4w, v4w2;

		v13w = v13 + new Vector3(0f, heightw, 0f);
		v24w = v24 + new Vector3(0f, heightw, 0f);
		v13w2 = v13 - new Vector3(0f, heightw2, 0f);
		v24w2 = v24 - new Vector3(0f, heightw2, 0f);
		v3w = v3 + new Vector3(0f, heightw, 0f);
		v4w = v4 + new Vector3(0f, heightw, 0f);
		v3w2 = v3 - new Vector3(0f, heightw2, 0f);
		v4w2 = v4 - new Vector3(0f, heightw2, 0f);

		walls.AddQuad(v2 + offset, v24 + offset, v2 + vElevation, v24 + vElevation);
		walls.AddQuad(v24 + offset, v4 + offset, v24w, v4w);
		walls.AddQuad(v24w2 + vElevation, v4w2 + vElevation, v24 + vElevation, v4 + vElevation);

		walls.AddQuad(v13 + offset, v1 + offset, v13 + vElevation, v1 + vElevation);
		walls.AddQuad(v3 + offset, v13 + offset, v3w, v13w);
		walls.AddQuad(v3w2 + vElevation, v13w2 + vElevation, v3 + vElevation, v13 + vElevation);

		walls.AddQuad(v24w, Metrics.GetMiddle(v24w, v13w), v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w, v13w), v13w, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation);
		walls.AddQuad(v13w, Metrics.GetMiddle(v13w, v24w), v3w, Metrics.GetMiddle(v3w, v4w));
		walls.AddQuad(Metrics.GetMiddle(v13w, v24w), v24w, Metrics.GetMiddle(v3w, v4w), v4w);
		walls.AddQuad(v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v4w2 + vElevation, Metrics.GetMiddle(v4w2, v3w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation, Metrics.GetMiddle(v4w2, v3w2) + vElevation, v3w2 + vElevation);

		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);

		//overview
		walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
		walls.AddQuadColor(Color.white);

		Vector3 yw = wall.neighbor.Position + new Vector3(0f, heightw, 0f);
		Vector3 yw2 = wall.neighbor.Position - new Vector3(0f, heightw2, 0f);
		Transform t_window = wall.o_Window.transform;
		t_window.position = Metrics.GetMiddle(yw, yw2 + vElevation) + new Vector3(0f, wall.level * Metrics.wallsElevation, 0f);
		t_window.eulerAngles = new Vector3(t_window.eulerAngles.x, rotation, t_window.eulerAngles.z);
		t_window.SetParent(wall.transform);
	}

	/// <summary>
	/// Triangule le mur en créant un trou dedans pour la fenêtre traversant deux murs attachées au mur voisin.
	/// </summary>
	public void AddIncomingWindow(Wall wall, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
	{
		float elevation = wall.Elevation;
		var vElevation = new Vector3(0f, elevation, 0f) + offset;
		Color colorLeft = wall.ColorLeft;
		Color colorRight = wall.ColorRight;

		walls.AddQuad(v1 + vElevation, v2 + vElevation, v3 + vElevation, v4 + vElevation);
		walls.AddQuad(v2 + offset, v1 + offset, v4 + offset, v3 + offset);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		Vector3 vUnit;
		float width, heightw, heightw2;

		Window window = wall.dot.GetNeighbor(direction.Opposite()).GetWall(direction).o_Window.GetComponent<Window>();
		width = window.width;
		heightw = window.heightw;
		heightw2 = window.heightw2;
		switch (direction)
		{
			case CompassDirection.N:
				vUnit = new Vector3(0f, 0f, 1f);
				break;
			case CompassDirection.NE:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, Mathf.Sqrt(2) / 2f);
				break;
			case CompassDirection.E:
				vUnit = new Vector3(1f, 0f, 0f);
				break;
			default:
				vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, -Mathf.Sqrt(2) / 2f);
				break;
		}

		Vector3 v13 = (width - Metrics.dotRadius) * vUnit + wall.dot.Position + Metrics.dotEdges[(int)direction];
		Vector3 v24 = (width - Metrics.dotRadius) * vUnit + wall.dot.Position + Metrics.dotEdges[(int)direction + 1];

		Vector3 v13w, v13w2, v24w, v24w2, v1w, v1w2, v2w, v2w2;

		v13w = v13 + new Vector3(0f, heightw, 0f);
		v24w = v24 + new Vector3(0f, heightw, 0f);
		v13w2 = v13 - new Vector3(0f, heightw2, 0f);
		v24w2 = v24 - new Vector3(0f, heightw2, 0f);
		v1w = v1 + new Vector3(0f, heightw, 0f);
		v2w = v2 + new Vector3(0f, heightw, 0f);
		v1w2 = v1 - new Vector3(0f, heightw2, 0f);
		v2w2 = v2 - new Vector3(0f, heightw2, 0f);

		walls.AddQuad(v24 + offset, v4 + offset, v24 + vElevation, v4 + vElevation);
		walls.AddQuad(v2 + offset, v24 + offset, v2w, v24w);
		walls.AddQuad(v2w2 + vElevation, v24w2 + vElevation, v2 + vElevation, v24 + vElevation);

		walls.AddQuad(v3 + offset, v13 + offset, v3 + vElevation, v13 + vElevation);
		walls.AddQuad(v13 + offset, v1 + offset, v13w, v1w);
		walls.AddQuad(v13w2 + vElevation, v1w2 + vElevation, v13 + vElevation, v1 + vElevation);

		walls.AddQuad(Metrics.GetMiddle(v24w, v13w),v24w, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v24w2 + vElevation);
		walls.AddQuad(v13w,Metrics.GetMiddle(v24w, v13w), v13w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation);

		walls.AddQuad(v1w, Metrics.GetMiddle(v1w, v2w),v13w, Metrics.GetMiddle(v13w, v24w));

		walls.AddQuad(v24w,Metrics.GetMiddle(v13w, v24w), v2w, Metrics.GetMiddle(v1w, v2w));

		walls.AddQuad(v2w2 + vElevation, Metrics.GetMiddle(v2w2, v1w2) + vElevation,v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v2w2, v1w2) + vElevation, v1w2 + vElevation,Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation);

		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);

		//overview
		walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
		walls.AddQuadColor(Color.white);
	}
}
