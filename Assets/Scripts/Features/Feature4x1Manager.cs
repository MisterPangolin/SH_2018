using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 1*4 16emes de cellule.
/// </summary>
public class Feature4x1Manager : Feature{

	/// <summary>
	/// Détermine la taille de l'objet.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc1x4;
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
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 7.5f);
						break;
					case 1:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 7.5f);
						break;
					case 2:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 7.5f);
						break;
					case 3:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 7.5f);
						break;
					case 4:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 5f);
						break;
					case 5:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 5f);
						break;
					case 6:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 5f);
						break;
					case 7:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 5f);
						break;
					case 8:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 9:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 2.5f);
						break;
					case 10:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 2.5f);
						break;
					case 11:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 12:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 13:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 14:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					default:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 0f);
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(0f, 0 / 2f, 3 * halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(7.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(2.5f, 0 / 2f, halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(5f, 0 / 2f, halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(7.5f, 0 / 2f, halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(2.5f, 0 / 2f, -halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(5f, 0 / 2f, -halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(7.5f, 0 / 2f, -halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(0f, 0 / 2f, -3 * halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(5f, 0 / 2f, -3 * halfQuarter);
						break;
					default:
						transform.position = new Vector3(7.5f, 0 / 2f, -3 * halfQuarter);
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 1:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 2:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					case 3:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 4:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 5:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -2.5f);
						break;
					case 6:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -2.5f);
						break;
					case 7:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 8:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -5f);
						break;
					case 9:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -5f);
						break;
					case 10:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -5f);
						break;
					case 11:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -5f);
						break;
					case 12:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -7.5f);
						break;
					case 13:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -7.5f);
						break;
					case 14:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -7.5f);
						break;
					default:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -7.5f);
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-7.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(-5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(-2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(0f, 0 / 2f, 3 * halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(-7.5f, 0 / 2f, halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(-5f, 0 / 2f, halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(-2.5f, 0 / 2f, halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(-7.5f, 0 / 2f, -halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(-5f, 0 / 2f, -halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(-2.5f, 0 / 2f, -halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(-7.5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(-5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(-2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					default:
						transform.position = new Vector3(0f, 0 / 2f, -3 * halfQuarter);
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
		Cell[] neighbors = new Cell[4];
		neighbors[0] = cell;
		switch (orientation)
		{
			case SquareDirection.N:
				if (part >= 12)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell;
				}
				else if (part >= 8)
				{
					neighbors[1] = neighbors[2] = cell;
					neighbors[3] = cell.GetNeighbor(SquareDirection.N);
				}
				else if (part >= 4)
				{
					neighbors[1] = cell;
					neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.N);
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.N);
				}
				break;
			case SquareDirection.E:
				if (part == 3 || part == 7 || part == 11 || part == 15)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.E);
				}
				else if (part == 2 || part == 6 || part == 10 || part == 14)
				{
					neighbors[1] = cell;
					neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.E);
				}
				else if (part == 1 || part == 5 || part == 9 || part == 13)
				{
					neighbors[1] = neighbors[2] = cell;
					neighbors[3] = cell.GetNeighbor(SquareDirection.E);
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell;
				}
				break;
			case SquareDirection.S:
				if (part < 4)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell;
				}
				else if (part < 8)
				{
					neighbors[1] = neighbors[2] = cell;
					neighbors[3] = cell.GetNeighbor(SquareDirection.S);
				}
				else if (part < 12)
				{
					neighbors[1] =  cell;
					neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.S);
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.S);
				}
				break;
			default:
				if (part == 0 || part == 4 || part == 8 || part == 12)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell.GetNeighbor(SquareDirection.W);
				}
				else if (part == 1 || part == 5 || part == 9 || part == 13)
				{
					neighbors[1] = cell;
					neighbors[2] = neighbors[3] =  cell.GetNeighbor(SquareDirection.W);
				}
				else if (part == 2 || part == 6 || part == 10 || part == 14)
				{
					neighbors[1] = neighbors[2] = cell;
					neighbors[3] = cell.GetNeighbor(SquareDirection.W);
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = cell;
				}
				break;
		}
		return neighbors;
	}

	/// <summary>
	/// Renvoie les autres 16eme de cellules sur lesquels l'objet va se placer.
	/// </summary>
	public override int[] GetNeighborsParts(int part)
	{
		var neighborParts = new int[4];
		neighborParts[0] = part;
		switch (orientation)
		{
			case SquareDirection.N:
				neighborParts[1] = (part >= 4) ? part - 4 : part + 12;
				neighborParts[2] = (part >= 8) ? part - 8 : part + 8;
				neighborParts[3] = (part >= 12) ? part - 12 : part + 4;
				break;
			case SquareDirection.E:
				if (part != 3 && part != 7 && part != 11 && part != 15)
				{
					neighborParts[1] = part + 1;
				}
				else
				{
					neighborParts[1] = part - 3;
				}
				if (part != 3 && part != 7 && part != 11 && part != 15 && part != 2 && part != 6 && part != 10 && part != 14)
				{
					neighborParts[2] = part + 2;
				}
				else
				{
					neighborParts[2] = part - 2;
				}
				if (part == 0 || part == 4 || part == 8 || part == 12)
				{
					neighborParts[3] = part + 3;
				}
				else
				{
					neighborParts[3] = part - 1;
				}
				break;
			case SquareDirection.S:
				neighborParts[1] = (part <= 11) ? part + 4 : part - 12;
				neighborParts[2] = (part <= 7) ? part + 8 : part - 8;
				neighborParts[3] = (part <= 3) ? part + 12 : part - 4;
				break;
			default:
				if (part != 0 && part != 4 && part != 8 && part != 12)
				{
					neighborParts[1] = part - 1;
				}
				else
				{
					neighborParts[1] = part + 3;
				}
				if (part != 0 && part != 1 && part != 4 && part != 5 && part != 8 && part != 9 && part != 12 && part != 13)
				{
					neighborParts[2] = part - 2;
				}
				else
				{
					neighborParts[2] = part + 2;
				}
				if (part == 3 || part == 7 || part == 11 || part == 15)
				{
					neighborParts[3] = part - 3;
				}
				else
				{
					neighborParts[3] = part + 1;
				}
				break;
		}
		return neighborParts;
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
