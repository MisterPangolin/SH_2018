using UnityEngine;

/// <summary>
/// Classe permettant de modifier les meshs "walls" pour prendre en considération la présence des portes.
/// </summary>
public class DoorsManager : MonoBehaviour {
	
	//épaisseur supplémentaire du mur
	Vector3 offset = Metrics.offset;

	/// <summary>
	/// Méthode pour ajouter la porte à travers un mur. Est appelée lorsque le mesh est triangulé.
	/// Elle permet de créer un trou dans le mesh de mêmes dimensions que la porte.
	/// </summary>
	public void AddDoor(Wall wall, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
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

		Vector3 v24, v42, v24w2, v42w2, v13, v31, v13w2, v31w2;

		Vector3 vUnit;
		float rotation, width, height;

		Door door = wall.o_Door.GetComponent<Door>();
		width = door.width;
		height = door.height;
		rotation = door.rotation;

		//Définit le vecteur unité selon la direction du mur, ainsi que la rotation de la porte.
		if (direction == CompassDirection.N)
		{
			vUnit = new Vector3(0f, 0f, 1f);
			rotation += -90f;
		}
		else if (direction == CompassDirection.NE)
		{
			vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, Mathf.Sqrt(2) / 2f);
			rotation += -45f;
		}
		else if (direction == CompassDirection.E)
		{
			vUnit = new Vector3(1f, 0f, 0f);
			rotation += 0f;
		}
		else
		{
			vUnit = new Vector3(Mathf.Sqrt(2) / 2f, 0f, -Mathf.Sqrt(2) / 2f);
			rotation += 45f;
		}

		v24 = middleRight + vUnit * -width;
		v42 = middleRight + vUnit * width;

		v24w2 = v24 - new Vector3(0f, height, 0f) - offset;
		v42w2 = v42 - new Vector3(0f, height, 0f) - offset;

		v13 = middleLeft + vUnit * -width;
		v31 = middleLeft + vUnit * width;

		v13w2 = v13 - new Vector3(0f, height, 0f) - offset;
		v31w2 = v31 - new Vector3(0f, height, 0f) - offset;

		wall.AddCollider(new Vector3(7f, 1f, 0.2f), new Vector3(0f, 0.5f, 0.4f));

		walls.AddQuad(v2 + offset, v24 + offset, v2 + vElevation, v24 + vElevation);
		walls.AddQuad(v24w2 + vElevation, v42w2 + vElevation, v24 + vElevation, v42 + vElevation);
		walls.AddQuad(v42 + offset, v4 + offset, v42 + vElevation, v4 + vElevation);

		walls.AddQuad(v3 + offset, v31 + offset, v3 + vElevation, v31 + vElevation);
		walls.AddQuad(v31w2 + vElevation, v13w2 + vElevation, v31 + vElevation, v13 + vElevation);
		walls.AddQuad(v13 + offset, v1 + offset, v13 + vElevation, v1 + vElevation);

		walls.AddQuad(v24 + offset,Metrics.GetMiddle(v24,v13),v24w2 + vElevation,Metrics.GetMiddle(v24w2,v13w2)+ vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24, v13), v13, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation);
		walls.AddQuad(v31 + offset, Metrics.GetMiddle(v31, v42), v31w2 + vElevation, Metrics.GetMiddle(v31w2, v42w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v31, v42), v42, Metrics.GetMiddle(v31w2, v42w2) + vElevation, v42w2 + vElevation);
		walls.AddQuad(v24w2 + vElevation, Metrics.GetMiddle(v24w2, v13w2) + vElevation, v42w2 + vElevation, Metrics.GetMiddle(v42w2, v31w2) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w2, v13w2) + vElevation, v13w2 + vElevation, Metrics.GetMiddle(v42w2, v31w2) + vElevation, v31w2 + vElevation);

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
		walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		Transform t_door = wall.o_Door.transform;
		t_door.position = Metrics.GetMiddle(v13, v42) + new Vector3(0f, wall.level * Metrics.wallsElevation, 0f);
		t_door.eulerAngles = new Vector3(t_door.eulerAngles.x, rotation, t_door.eulerAngles.z);
		t_door.SetParent(wall.transform);
	}

	/// <summary>
	/// Triangule le mur en créant un trou pour la porte traversant deux murs attachées à ce mur.
	/// </summary>
	public void AddOutgoingDoor(Wall wall, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
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
		float rotation, width, height;

		Door door = wall.o_Door.GetComponent<Door>();
		width = door.width;
		height = door.height;
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

		Vector3 v13w, v24w, v3w, v4w;

		v13w = v13 - new Vector3(0f, height, 0f);
		v24w = v24 - new Vector3(0f, height, 0f);
		v3w = v3 - new Vector3(0f, height, 0f);
		v4w = v4 - new Vector3(0f, height, 0f);

		wall.AddCollider(new Vector3(7f, 1f, 0.2f), new Vector3(0f, 0.5f, 0.4f));

		walls.AddQuad(v2 + offset, v24 + offset, v2 + vElevation, v24 + vElevation);
		walls.AddQuad(v24w + vElevation, v4w + vElevation, v24 + vElevation, v4 + vElevation);
		walls.AddQuad(v13 + offset, v1 + offset, v13 + vElevation, v1 + vElevation);
		walls.AddQuad(v3w + vElevation, v13w + vElevation, v3 + vElevation, v13 + vElevation);
		walls.AddQuad(v24 + offset, Metrics.GetMiddle(v24, v13) + offset, v24w + vElevation, Metrics.GetMiddle(v24w, v13w) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24, v13) + offset, v13 + offset, Metrics.GetMiddle(v24w, v13w) + vElevation, v13w + vElevation);
		walls.AddQuad(v24w + vElevation, Metrics.GetMiddle(v24w, v13w) + vElevation, v4w + vElevation, Metrics.GetMiddle(v4w, v3w) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v24w, v13w) + vElevation, v13w + vElevation, Metrics.GetMiddle(v4w, v3w) + vElevation, v3w + vElevation);

		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);

		//overview
		walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
		walls.AddQuadColor(Color.white);

		Transform t_door = wall.o_Door.transform;
		t_door.position = Metrics.GetMiddle(v13, v24) + new Vector3(0f, wall.level * Metrics.wallsElevation, 0f);
		t_door.eulerAngles = new Vector3(t_door.eulerAngles.x, rotation, t_door.eulerAngles.z);
		t_door.SetParent(wall.transform);
	}

	/// <summary>
	/// Triangule le mur en créant un trou dedans pour la porte traversant deux murs attachées au mur voisin.
	/// </summary>
	public void AddIncomingDoor(Wall wall, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CompassDirection direction, SquareMesh walls)
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
		float width, height;

		Door door = wall.dot.GetNeighbor(direction.Opposite()).GetWall(direction).o_Door.GetComponent<Door>();
		width = door.width;
		height = door.height;
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

		Vector3 v13w, v24w, v1w, v2w;

		v13w = v13 - new Vector3(0f, height, 0f);
		v24w = v24 - new Vector3(0f, height, 0f);
		v1w = v1 - new Vector3(0f, height, 0f);
		v2w = v2 - new Vector3(0f, height, 0f);

		wall.AddCollider(new Vector3(7f, 1f, 0.2f), new Vector3(0f, 0.5f, 0.4f));

		walls.AddQuad(v24 + offset, v4 + offset, v24 + vElevation, v4 + vElevation);
		walls.AddQuad(v2w + vElevation, v24w + vElevation, v2 + vElevation, v24 + vElevation);

		walls.AddQuad(v3 + offset, v13 + offset, v3 + vElevation, v13 + vElevation);
		walls.AddQuad(v13w + vElevation, v1w + vElevation, v13 + vElevation, v1 + vElevation);

		walls.AddQuad(Metrics.GetMiddle(v24, v13) + offset, v24 + offset, Metrics.GetMiddle(v24, v13) + vElevation, v24 + vElevation);
		walls.AddQuad(v13 + offset, Metrics.GetMiddle(v24, v13) + offset, v13 + vElevation, Metrics.GetMiddle(v24, v13) + vElevation);

		walls.AddQuad(v2w + vElevation, Metrics.GetMiddle(v2w, v1w) + vElevation, v24w + vElevation, Metrics.GetMiddle(v24w, v13w) + vElevation);
		walls.AddQuad(Metrics.GetMiddle(v2w, v1w) + vElevation, v1w + vElevation, Metrics.GetMiddle(v24w, v13w) + vElevation, v13w + vElevation);

		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(colorLeft);

		//overview
		walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
		walls.AddQuadColor(Color.white);
	}
}
