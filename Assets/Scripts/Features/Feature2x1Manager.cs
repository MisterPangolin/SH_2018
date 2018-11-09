using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 1*2 16emes de cellule.
/// </summary>
public class Feature2x1Manager : Feature{

	/// <summary>
	/// Détermine la taille de l'objet.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc1x2;
	}

	/// <summary>
	/// Calcule la position de l'objet.
	/// </summary>
	public override void CalculatePosition(int part)
	{
		float halfQuarter = Metrics.innerRadius / 4f;
		switch (orientation)
		{
			case SquareDirection.N:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 5f);
						break;
					case 1:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 5f);
						break;
					case 2:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 5f);
						break;
					case 3:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 5f);
						break;
					case 4:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 5:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 2.5f);
						break;
					case 6:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 2.5f);
						break;
					case 7:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 8:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 9:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 10:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					case 11:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 12:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 13:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -2.5f);
						break;
					case 14:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -2.5f);
						break;
					default:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -2.5f);
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(0f, 0 / 2f, 3 * halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(-2.5f, 0 / 2f, halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(2.5f, 0 / 2f, halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(5f, 0 / 2f, halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(-2.5f, 0 / 2f, -halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(2.5f, 0 / 2f, -halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(5f, 0 / 2f, -halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(-2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(0f, 0 / 2f, -3 * halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					default:
						transform.position = new Vector3(5f, 0 / 2f, -3 * halfQuarter);
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 1:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 2.5f);
						break;
					case 2:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 2.5f);
						break;
					case 3:
						transform.position = new Vector3(3 * halfQuarter,  0 / 2f, 2.5f);
						break;
					case 4:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 5:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 6:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					case 7:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 8:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 9:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -2.5f);
						break;
					case 10:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -2.5f);
						break;
					case 11:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 12:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -5f);
						break;
					case 13:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -5f);
						break;
					case 14:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -5f);
						break;
					default:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -5f);
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(-2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(0f, 0 / 2f, 3 * halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(-5f, 0 / 2f, halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(-2.5f, 0 / 2f, halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(2.5f, 0 / 2f, halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(-5f, 0 / 2f, -halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(-2.5f, 0 / 2f, -halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(2.5f, 0 / 2f, -halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(-5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(-2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(0f, 0 / 2f, -3 * halfQuarter);
						break;
					default:
						transform.position = new Vector3(2.5f, 0 / 2f, -3 * halfQuarter);
						break;
				}
				break;
		}
		AdjustPosition();
	}

	/// <summary>
	/// Renvoie les cellules voisines sur lesquelles l'objet va se placer.
	/// </summary>
	public override Cell[] GetNeighbors(Cell cell, int part)
	{
		var neighbor = new Cell[2];
		neighbor[0] = cell;
		if (orientation == SquareDirection.N)
		{
			if (part == 0 || part == 1 || part == 2 || part == 3)
			{
				neighbor[1] = cell.GetNeighbor(orientation);
			}
			else
			{
				neighbor[1] = cell;
			}
		}
		else if (orientation == SquareDirection.E)
		{
			if (part == 3 || part == 7 || part == 11 || part == 15)
			{
				neighbor[1] = cell.GetNeighbor(orientation);
			}
			else
			{
				neighbor[1] = cell;
			}
		}
		else if (orientation == SquareDirection.S)
		{
			if (part == 12 || part == 13 || part == 14 || part == 15)
			{
				neighbor[1] = cell.GetNeighbor(orientation);
			}
			else
			{
				neighbor[1] = cell;
			}
		}
		else
		{
			if (part == 0 || part == 4 || part == 8 || part == 12)
			{
				neighbor[1] = cell.GetNeighbor(orientation);
			}
			else
			{
				neighbor[1] = cell;
			}
		}
		return neighbor;
	}

	/// <summary>
	/// Renvoie les autres 16eme de cellules sur lesquels l'objet va se placer.
	/// </summary>
	public override int[] GetNeighborsParts(int part)
	{
		var neighborPart = new int[2];
		neighborPart[0] = part;
		switch (orientation)
		{
			case SquareDirection.N:
				neighborPart[1] = part <= 3 ? part + 12 : part - 4;
				break;
			case SquareDirection.E:
				neighborPart[1] = (part == 3 || part == 7 || part == 11 || part == 15) ? part - 3 : part + 1;
				break;
			case SquareDirection.S:
				neighborPart[1] = part > 11 ? part - 12 : part + 4;
				break;
			default:
				neighborPart[1] = (part == 0 || part == 4 || part == 8 || part == 12) ? part + 3 : part - 1;
				break;
		}
		return neighborPart;
	}

	/// <summary>
	/// Vérifie que les murs que va traverser l'objet n'existent pas.
	/// Renvoie true si l'emplacement est disponible, false sinon.
	/// </summary>
	public override bool CheckWallsRaised(Cell[] cells, int[] parts)
	{
		bool available = true;
		int i = 0;
		while (available && i < cells.Length)
		{
			Cell cell = cells[i];
			int part = parts[i];
			available = !Metrics.CheckWallRaised(cell, part);
			if (i > 0 && available)
			{
				available = !Metrics.CheckWallRaisedBetween(cell, part, parts[i - 1]);
			}
			i += 1;
		}
		return available;
	}

	/// <summary>
	/// Ajoute aux murs traversés par l'objet une référence à l'objet, ce qui permettra de détruire efficacement l'objet
	/// si le mur venait à être construit.
	/// </summary>
	public override void AddReferenceToWall()
	{
		Cell[] cells = GetNeighbors(BaseCell, BasePart);
		int[] parts = GetNeighborsParts(BasePart);
		for (int i = 0; i < cells.Length; i++)
		{
			Cell cell = cells[i];
			int part = parts[i];
			Wall wall = Metrics.GetWall(cell, part);
			if (featureType == FeatureType.furniture)
			{
				wall.AddCellFeature(this);
			}
			else if (featureType == FeatureType.carpet)
			{
				wall.AddCarpetFeature(this);
			}
			if (i > 0)
			{
				wall = Metrics.GetWallBetween(cell, part, parts[i - 1]);
				if (wall)
				{
					if (featureType == FeatureType.furniture)
					{
						wall.AddCellFeature(this);
					}
					else if (featureType == FeatureType.carpet)
					{
						wall.AddCarpetFeature(this);
					}
				}
			}
		}
	}
}
