using UnityEngine;

/// <summary>
/// Ensemble des constantes du projet, et de fonctions utiles permettant d'alléger le code d'autres classes.
/// </summary>
public static class Metrics{

	public const int houseLevels = 3;
	public const float innerRadius = 5f;
	public const int chunkSizeX = 2, chunkSizeZ = 2;
	public const float wallsElevation = 7f;
	public const float wallsThickness = 0f; //s'ajoute à l'épaisseur déjà existante des murs
	public const float dotRadius = 0.4f;
	public static Vector3 offset = new Vector3(0f, -0.2f, 0f);

	/// <summary>
	/// Les positions relative au centre de la cellule de ses sommets.
	/// La redondance du dernier élément, que l'on peut retrouver dans les différentes méthodes, est nécessaire.
	/// </summary>
	public static Vector3[] cellCorners = {
		new Vector3(-innerRadius, 0f, innerRadius),
		new Vector3(innerRadius, 0f, innerRadius),
		new Vector3(innerRadius, 0f, -innerRadius),
		new Vector3(-innerRadius, 0f, -innerRadius),
		new Vector3(-innerRadius, 0f, innerRadius)
	};

	/// <summary>
	/// Les positions relative au centre de la cellule des milieux de ses côtés.
	/// </summary>
	public static Vector3[] cellMiddleEdge = {
		new Vector3(-innerRadius, 0f, 0f),
		new Vector3(0f, 0f, innerRadius),
		new Vector3(innerRadius, 0f, 0f),
		new Vector3(0f, 0f, -innerRadius),
		new Vector3(-innerRadius, 0f, 0f)
	};

	/// <summary>
	/// Les positions relatives au centre de la cellule des coins d'un carré dont le centre se trouve au centre de la
	/// cellule et dont les côtés mesurent la moîtié des côtés de la cellule.
	/// </summary>
	public static Vector3[] cellInnerSquare = {
		new Vector3(-innerRadius/2, 0f, innerRadius/2),
		new Vector3(innerRadius/2, 0f, innerRadius/2),
		new Vector3(innerRadius/2, 0f, -innerRadius/2),
		new Vector3(-innerRadius/2, 0f, -innerRadius/2),
		new Vector3(-innerRadius/2, 0f, innerRadius/2)
	};

	/// <summary>
	/// Les positions relatives au centre du point de ses sommets.
	/// Le point est représenté par un hexagone dans le code, un cercle dans le canvas et un carré physiquement.
	/// </summary>
	public static Vector3[] dotEdges = {
		new Vector3(-dotRadius/2, 0f, dotRadius),
		new Vector3(dotRadius/2, 0f, dotRadius),
		new Vector3(dotRadius, 0f, dotRadius/2),
		new Vector3(dotRadius, 0f, -dotRadius/2),
		new Vector3(dotRadius/2, 0f, -dotRadius),
		new Vector3(-dotRadius/2, 0f, -dotRadius),
		new Vector3(-dotRadius, 0f, -dotRadius/2),
		new Vector3(-dotRadius, 0f, dotRadius/2),
		new Vector3(-dotRadius/2, 0f, dotRadius)
	};

	/// <summary>
	/// Renvoie le milieu entre deux points.
	/// </summary>
	public static Vector3 GetMiddle(Vector3 a, Vector3 b)
	{
		return Vector3.Lerp(a, b, 0.5f);
	}

	/// <summary>
	/// Renvoie le milieu entre deux sommets du point. Ces deux sommets se trouvent sur le côté se trouvant dans la
	/// direction donnée par rapport au centre du point.
	/// </summary>
	public static Vector3 GetMiddleEdge(CompassDirection direction)
	{
		Vector3 vLeft = dotEdges[(int)direction];
		Vector3 vRight = dotEdges[(int)direction + 1];
		return GetMiddle(vLeft, vRight);
	}

	/// <summary>
	/// Les positions relative au centre du point des sommets d'un hexagone dont le centre se trouve au centre du point
	/// et dont la longueur des côtés est égale à la moîtié de la longueur des côtés du point.
	/// </summary>
	public static Vector3[] dotInnerSquare = {
		new Vector3(-dotRadius/2, 0f, dotRadius/2),
		new Vector3(0f, 0f, dotRadius/2),
		new Vector3(dotRadius/2, 0f, dotRadius/2),
		new Vector3(dotRadius/2, 0f, 0f),
		new Vector3(dotRadius/2, 0f, -dotRadius/2),
		new Vector3(0f, 0f, -dotRadius/2),
		new Vector3(-dotRadius/2, 0f, -dotRadius/2),
		new Vector3(-dotRadius/2, 0f, 0f),
		new Vector3(-dotRadius/2, 0f, dotRadius/2)
	};

	/// <summary>
	/// Renvoie un entier correspondant au côté du mur auquel appartient le point d'impact donné.
	/// -1 si gauche, 1 si droite.
	/// </summary>
	public static int GetWallSide(Wall wall, Vector3 hitPoint)
	{
		Vector3 position = wall.transform.position;
		CompassDirection direction = wall.direction;
		float dx = hitPoint.x - position.x;
		float dz = hitPoint.z - position.z;

		if (direction == CompassDirection.N)
		{
			if (dx > 0f)
			{
				return 1;
			}
			else {
				return -1;
			}
		}
		else if (direction == CompassDirection.NE)
		{
			
			if (dx > dz)
			{
				return 1;
			}
			else {
				return -1;
			}
		}
		else if (direction == CompassDirection.E)
		{
			if (dz < 0)
			{
				return 1;
			}
			else {
				return -1;
			}
		}
		else if (direction == CompassDirection.SE)
		{
			if (dx < -dz)
			{
				return 1;
			}
			else {
				return -1;
			}
		}
		else {
			return 0;
		}
	}

	/// <summary>
	/// Renvoie le 16eme du côté du mur donné correspondant au point d'impact donné.
	/// Renvoie -1 s'il n'y a pas de correspondance.
	/// </summary>
	public static int GetWallPart(Wall wall, Vector3 hitPoint, int side)
	{
		Vector3 position = wall.transform.position;
		float dy = hitPoint.y - position.y;
		float di;
		if (wall.direction != CompassDirection.N)
		{
			di = hitPoint.x - position.x;
		}
		else
		{
			di = hitPoint.z - position.z;
		}
		if (dy > 0)
		{
			if (dy >= 3 * wallsElevation / 4f)
			{
				if (di >= innerRadius / 4f)
				{
					return side == 1 ? 3 : 0;
				}
				else if (di >= 0f)
				{
					return side == 1 ? 2 : 1;
				}
				else if (di >= -innerRadius / 4f)
				{
					return side == 1 ? 1 : 2;
				}
				else
				{
					return side == 1 ? 0 : 3;
				}
			}
			else if (dy > 2 * wallsElevation / 4f)
			{
				if (di >= innerRadius / 4f)
				{
					return side == 1 ? 7 : 4;
				}
				else if (di >= 0f)
				{
					return side == 1 ? 6 : 5;
				}
				else if (di >= -innerRadius / 4f)
				{
					return side == 1 ? 5 : 6;
				}
				else
				{
					return side == 1 ? 4 : 7;
				}
			}
			else if (dy > wallsElevation / 4f)
			{
				if (di >= innerRadius / 4f)
				{
					return side == 1 ? 11 : 8;
				}
				else if (di >= 0f)
				{
					return side == 1 ? 10 : 9;
				}
				else if (di >= -innerRadius / 4f)
				{
					return side == 1 ? 9 : 10;
				}
				else
				{
					return side == 1 ? 8 : 11;
				}
			}
			else
			{
				if (di >= innerRadius / 4f)
				{
					return side == 1 ? 15 : 12;
				}
				else if (di >= 0f)
				{
					return side == 1 ? 14 : 13;
				}
				else if (di >= -innerRadius / 4f)
				{
					return side == 1 ? 13 : 14;
				}
				else
				{
					return side == 1 ? 12 : 15;
				}
			}
		}
		else
		{
			return -1;
		}
	}

	/// <summary>
	/// Appelé pour déterminer les 16eme de murs à traiter.
	/// Renvoie un tableau d'entiers correspondant aux mêmes 16eme donnés mais de l'autre côté du mur.
	/// </summary>
	public static int[] OppositeWallParts(int[] parts)
	{
		var oppositeParts = new int[parts.Length];
		for (int i = 0; i < parts.Length; i++)
		{
			if (parts[i] == 0 || parts[i] == 4 || parts[i] == 8 || parts[i] == 12)
			{
				oppositeParts[i] = parts[i] + 3;
			}
			else if (parts[i] == 1 || parts[i] == 5 || parts[i] == 9 || parts[i] == 13)
			{
				oppositeParts[i] = parts[i] + 1;
			}
			else if (parts[i] == 2 || parts[i] == 6 || parts[i] == 10 || parts[i] == 14)
			{
				oppositeParts[i] = parts[i] - 1;
			}
			else
			{
				oppositeParts[i] = parts[i] - 3;
			}
		}
		return oppositeParts;
	}

	/// <summary>
	/// Renvoie la direction par rapport au centre de la cellule donnée du quart de cellule touché.
	/// </summary>
	public static CompassDirection GetCellQuarter(Cell cell, Vector3 hitPoint)
	{
		CompassDirection direction;
		Dot pivot = cell.pivot;
		float dX = hitPoint.x - pivot.transform.position.x;
		float dZ = hitPoint.z - pivot.transform.position.z;
		if (dX >= 0)
		{
			if (dZ >= 0)
			{
				direction = CompassDirection.NE;
			}
			else {
				direction = CompassDirection.SE;
			}
		}
		else {
			if (dZ >= 0)
			{
				direction = CompassDirection.NW;
			}
			else {
				direction = CompassDirection.SW;
			}
		}
		return direction;
	}

	/// <summary>
	/// Renvoie le 16eme de cellule touché.
	/// </summary>
	public static int GetCellPart(Cell cell, Vector3 hitPoint)
	{
		int part;
		Dot pivot = cell.pivot;
		float dX = hitPoint.x - pivot.transform.position.x;
		float dZ = hitPoint.z - pivot.transform.position.z;
		if (dX >= 2.5f)
		{
			if (dZ >= 2.5f)
			{
				part = 3;
			}
			else if (dZ >= 0f)
			{
				part = 7;
			}
			else if (dZ >= -2.5f)
			{
				part = 11;
			}
			else
			{
				part = 15;
			}
		}
		else if (dX >= 0f)
		{
			if (dZ >= 2.5f)
			{
				part = 2;
			}
			else if (dZ >= 0f)
			{
				part = 6;
			}
			else if (dZ >= -2.5f)
			{
				part = 10;
			}
			else
			{
				part = 14;
			}
		}
		else if (dX >= -2.5f)
		{
			if (dZ >= 2.5f)
			{
				part = 1;
			}
			else if (dZ >= 0f)
			{
				part = 5;
			}
			else if (dZ >= -2.5f)
			{
				part = 9;
			}
			else
			{
				part = 13;
			}
		}
		else
		{
			if (dZ >= 2.5f)
			{
				part = 0;
			}
			else if (dZ >= 0f)
			{
				part = 4;
			}
			else if (dZ >= -2.5f)
			{
				part = 8;
			}
			else
			{
				part = 12;
			}
		}
		return part;
	}

	/// <summary>
	/// Donne le triangle suivant dans le quart de cellule le triangle donné, dans le sens horaire.
	/// </summary>
	public static int GetNextTriangle(int triangle)
	{
		if (triangle > 11)
		{
			return triangle < 15 ? (triangle + 1) : 12;
		}
		else if (triangle > 7)
		{
			return triangle < 11 ? (triangle + 1) : 8;
		}
		else if (triangle > 3)
		{
			return triangle < 7 ? (triangle + 1) : 4;
		}
		else {
			return triangle < 3 ? (triangle + 1) : 0;
		}
	}

	/// <summary>
	/// Donne le second triangle suivant dans le quart de cellule le triangle donné, dans le sens horaire.
	/// </summary>
	public static int GetSecondNextTriangle(int triangle)
	{
		if (triangle > 11)
		{
			return triangle < 14 ? (triangle + 2) : (triangle - 2);
		}
		else if (triangle > 7)
		{
			return triangle < 10 ? (triangle + 2) : (triangle - 2);
		}
		else if (triangle > 3)
		{
			return triangle < 6 ? (triangle + 2) : (triangle - 2);
		}
		else {
			return triangle < 2 ? (triangle + 2) : (triangle - 2);
		}
	}

	/// <summary>
	/// Donne le triangle précédent le triangle donné dans le quart de cellule, dans le sens horaire.
	/// </summary>
	public static int GetThirdNextTriangle(int triangle)
	{
		if (triangle > 11)
		{
			return triangle < 13 ? (triangle + 3) : (triangle - 1);
		}
		else if (triangle > 7)
		{
			return triangle < 9 ? (triangle + 3) : (triangle - 1);
		}
		else if (triangle > 3)
		{
			return triangle < 5 ? (triangle + 3) : (triangle - 1);
		}
		else {
			return triangle < 1 ? (triangle + 3) : (triangle - 1);
		}
	}

	/// <summary>
	/// Renvoie les sommets du collider donné.
	/// </summary>
	public static Vector3[] GetColliderVertexPositions(BoxCollider b)
	{
		Vector3[] vertices = new Vector3[8];
		vertices[0] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x,-b.size.y,-b.size.z) * 0.5f);
		vertices[1] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f);
		vertices[2] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f);
		vertices[3] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f);
		vertices[4] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, -b.size.z) * 0.5f);
		vertices[5] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, -b.size.z) * 0.5f);
		vertices[6] = b.transform.TransformPoint(b.center + new Vector3(b.size.x, b.size.y, b.size.z) * 0.5f);
		vertices[7] = b.transform.TransformPoint(b.center + new Vector3(-b.size.x, b.size.y, b.size.z) * 0.5f);
		return vertices;
	}

	/// <summary>
	/// Renvoie true si le mur traversant le 16eme de cellule donné existe.
	/// Renvoie false sinon.
	/// </summary>
	public static bool CheckWallRaised(Cell cell, int part)
	{
		bool raised = false;
		Wall wall = GetWall(cell, part);
		raised = wall.Raised;
		return raised;
	}

	/// <summary>
	/// Renvoie le mur traversant le 16eme de cellule donné.
	/// </summary>
	public static Wall GetWall(Cell cell, int part)
	{
		Wall wall;
		switch (part)
		{
			case 0:
				wall = cell.GetWall(1);
				break;
			case 1:
				wall = cell.GetWall(13);
				break;
			case 2:
				wall = cell.GetWall(3);
				break;
			case 3:
				wall = cell.GetWall(17);
				break;
			case 4:
				wall = cell.GetWall(13);
				break;
			case 5:
				wall = cell.GetWall(1);
				break;
			case 6:
				wall = cell.GetWall(17);
				break;
			case 7:
				wall = cell.GetWall(3);
				break;
			case 8:
				wall = cell.GetWall(15);
				break;
			case 9:
				wall = cell.GetWall(10);
				break;
			case 10:
				wall = cell.GetWall(19);
				break;
			case 11:
				wall = cell.GetWall(7);
				break;
			case 12:
				wall = cell.GetWall(10);
				break;
			case 13:
				wall = cell.GetWall(15);
				break;
			case 14:
				wall = cell.GetWall(7);
				break;
			default:
				wall = cell.GetWall(19);
				break;
		}
		return wall;
	}

	/// <summary>
	/// Renvoie le mur situé entre deux 16eme de cellule donnés.
	/// </summary>
	public static Wall GetWallBetween(Cell cell, int part, int previousPart)
	{
		Wall wall;
		switch (part)
		{
			case 0:
			case 1:
			case 4:
			case 5:
				switch (previousPart)
				{
					case 12:
					case 13:
						wall = cell.GetWall(0);
						break;
					case 2:
					case 6:
						wall = cell.GetWall(16);
						break;
					case 8:
					case 9:
						wall = cell.GetWall(14);
						break;
					case 3:
					case 7:
						wall = cell.GetWall(12);
						break;
					default:
						wall = null;
						break;
				}
				break;
			case 2:
			case 3:
			case 6:
			case 7:
				switch (previousPart)
				{
					case 14:
					case 15:
						wall = cell.GetWall(2);
						break;
					case 0:
					case 4:
						wall = cell.GetWall(4);
						break;
					case 10:
					case 11:
						wall = cell.GetWall(18);
						break;
					case 1:
					case 5:
						wall = cell.GetWall(16);
						break;
					default:
						wall = null;
						break;
				}
				break;
			case 8:
			case 9:
			case 12:
			case 13:
				switch (previousPart)
				{
					case 4:
					case 5:
						wall = cell.GetWall(14);
						break;
					case 10:
					case 14:
						wall = cell.GetWall(6);
						break;
					case 0:
					case 1:
						wall = cell.GetWall(11);
						break;
					case 11:
					case 15:
						wall = cell.GetWall(9);
						break;
					default:
						wall = null;
						break;
				}
				break;
			case 10:
			case 11:
			case 14:
			default:
				switch (previousPart)
				{
					case 6:
					case 7:
						wall = cell.GetWall(18);
						break;
					case 8:
					case 12:
						wall = cell.GetWall(5);
						break;
					case 2:
					case 3:
						wall = cell.GetWall(8);
						break;
					case 9:
					case 13:
						wall = cell.GetWall(6);
						break;
					default:
						wall = null;
						break;
				}
				break;
		}
		return wall;
	}

	/// <summary>
	/// Renvoie true si le mur entre deux 16eme de cellule donnés existe.
	/// Renvoie false sinon.
	/// </summary>
	public static bool CheckWallRaisedBetween(Cell cell, int part, int previousPart)
	{
		bool raised = false;
		Wall wall = GetWallBetween(cell, part, previousPart);
		if (wall)
		{
			raised = wall.Raised;
		}
		return raised;
	}

	/// <summary>
	/// Recalcule le flottant "average" de "subData" en prenant en compte la dernière valeur ajoutée "f".
	/// </summary>
	public static void RecalculateAverage(SubData subData, float f, bool isABool = false)
	{
		if (!isABool)
		{
			subData.average = (subData.average + f) / 2f;
		}
		else
		{
			if (Mathf.Abs(f-1) < 0.01)
			{
				subData.average = 1f;
			}
		}
	}
}
