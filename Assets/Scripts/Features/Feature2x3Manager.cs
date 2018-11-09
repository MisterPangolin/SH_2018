using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 2*3 16emes de cellule.
/// </summary>
public class Feature2x3Manager : Feature{

	/// <summary>
	/// Détermine la taille de l'objet.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc2x3;
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
						transform.position = new Vector3(-5f, 0 / 2f, 5 * halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(-2.5f, 0 / 2f, 5 * halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(0f, 0 / 2f, 5 * halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(2.5f, 0 / 2f, 5 * halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(-5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(-2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(0f, 0 / 2f, 3 * halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(2.5f, 0 / 2f, 3 * halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(-5f, 0 / 2f, halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(-2.5f, 0 / 2f, halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(2.5f, 0 / 2f, halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(-5f, 0 / 2f, -halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(-2.5f, 0 / 2f, -halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					default:
						transform.position = new Vector3(2.5f, 0 / 2f, -halfQuarter);
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 5f);
						break;
					case 1:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 5f);
						break;
					case 2:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 5f);
						break;
					case 3:
						transform.position = new Vector3(5 * halfQuarter, 0 / 2f, 5f);
						break;
					case 4:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 2.5f);
						break;
					case 5:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 2.5f);
						break;
					case 6:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 7:
						transform.position = new Vector3(5 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 8:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 9:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					case 10:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 11:
						transform.position = new Vector3(5 * halfQuarter, 0 / 2f, 0f);
						break;
					case 12:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -2.5f);
						break;
					case 13:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -2.5f);
						break;
					case 14:
						transform.position = new Vector3(3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					default:
						transform.position = new Vector3(5 * halfQuarter, 0 / 2f, -2.5f);
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-2.5f, 0 / 2f, halfQuarter);
						break;
					case 1:
						transform.position = new Vector3(0f, 0 / 2f, halfQuarter);
						break;
					case 2:
						transform.position = new Vector3(2.5f, 0 / 2f, halfQuarter);
						break;
					case 3:
						transform.position = new Vector3(5f, 0 / 2f, halfQuarter);
						break;
					case 4:
						transform.position = new Vector3(-2.5f, 0 / 2f, - halfQuarter);
						break;
					case 5:
						transform.position = new Vector3(0f, 0 / 2f, -halfQuarter);
						break;
					case 6:
						transform.position = new Vector3(2.5f, 0 / 2f, -halfQuarter);
						break;
					case 7:
						transform.position = new Vector3(5f, 0 / 2f, -halfQuarter);
						break;
					case 8:
						transform.position = new Vector3(-2.5f, 0 / 2f, - 3 * halfQuarter);
						break;
					case 9:
						transform.position = new Vector3(0f, 0 / 2f, -3 * halfQuarter);
						break;
					case 10:
						transform.position = new Vector3(2.5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 11:
						transform.position = new Vector3(5f, 0 / 2f, -3 * halfQuarter);
						break;
					case 12:
						transform.position = new Vector3(-2.5f, 0 / 2f, -5 * halfQuarter);
						break;
					case 13:
						transform.position = new Vector3(0f, 0 / 2f, -5 * halfQuarter);
						break;
					case 14:
						transform.position = new Vector3(2.5f, 0 / 2f, -5 * halfQuarter);
						break;
					default:
						transform.position = new Vector3(5f, 0 / 2f, -5 * halfQuarter);
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-5 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 1:
						transform.position = new Vector3(- 3 * halfQuarter, 0 / 2f, 2.5f);
						break;
					case 2:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 2.5f);
						break;
					case 3:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 2.5f);
						break;
					case 4:
						transform.position = new Vector3(-5 * halfQuarter, 0 / 2f, 0f);
						break;
					case 5:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, 0f);
						break;
					case 6:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, 0f);
						break;
					case 7:
						transform.position = new Vector3(halfQuarter, 0 / 2f, 0f);
						break;
					case 8:
						transform.position = new Vector3(-5 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 9:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -2.5f);
						break;
					case 10:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -2.5f);
						break;
					case 11:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -2.5f);
						break;
					case 12:
						transform.position = new Vector3(-5 * halfQuarter, 0 / 2f, -5f);
						break;
					case 13:
						transform.position = new Vector3(-3 * halfQuarter, 0 / 2f, -5f);
						break;
					case 14:
						transform.position = new Vector3(-halfQuarter, 0 / 2f, -5f);
						break;
					default:
						transform.position = new Vector3(halfQuarter, 0 / 2f, -5f);
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
		var neighbors = new Cell[6];
		neighbors[5] = cell;
		switch (orientation)
		{
			case SquareDirection.N:
				if (part >= 9 && part != 12)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell,cell };
				}
				else if (part >= 1 && part != 4 && part <= 7)
				{
					neighbors[0] = cell;
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.N);
					if (part >= 5)
					{
						neighbors[1] = neighbors[4] = cell;
					}
				}
				else
				{
					neighbors[0] = neighbors[1] = neighbors[2] = cell.GetNeighbor(SquareDirection.W);
					neighbors[3] = neighbors[4] = cell;
					if (part == 4 && neighbors[0])
					{
						neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.N);
						neighbors[3] = cell.GetNeighbor(SquareDirection.N);
					}
					else if (part == 0 && neighbors[0])
					{
						neighbors[1] = neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.N);
						neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.N);
					}
				}
				break;
			case SquareDirection.E:
				if (part == 4 || part ==5 || part == 8 || part == 9 || part == 12 || part == 13)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell, cell };
				}
				else if (part == 6 || part == 7 || part == 10 || part == 11 || part == 14 || part == 15)
				{
					neighbors[0] = cell;
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.E);
					if (part == 6 || part == 10 || part == 14)
					{
						neighbors[1] = neighbors[4] = cell;
					}
				}
				else
				{
					neighbors[0] = neighbors[1] = neighbors[2] = cell.GetNeighbor(SquareDirection.N);
					neighbors[3] = neighbors[4] = cell;
					if (part == 2 && neighbors[0])
					{
						neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.E);
						neighbors[3] = cell.GetNeighbor(SquareDirection.E);
					}
					else if (part == 3 && neighbors[0])
					{
						neighbors[1] = neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.E);
						neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.E);
					}
				}
				break;
			case SquareDirection.S:
				if (part <= 6 && part != 3)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell,cell };
				}
				else if (part >= 8 && part != 11 && part <= 14)
				{
					neighbors[0] = cell;
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.S);
					if (part <= 10)
					{
						neighbors[1] = neighbors[4] = cell;
					}
				}
				else
				{
					neighbors[0] = neighbors[1] = neighbors[2] = cell.GetNeighbor(SquareDirection.E);
					neighbors[3] = neighbors[4] = cell;
					if (part == 11 && neighbors[0])
					{
						neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.S);
						neighbors[3] = cell.GetNeighbor(SquareDirection.S);
					}
					else if (part == 15 && neighbors[0])
					{
						neighbors[1] = neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.S);
						neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.S);
					}
				}
				break;
			default:
				if (part == 2 || part == 3 || part == 6 || part == 7 || part == 10 || part == 11)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell,cell };
				}
				else if (part == 0 || part == 1 || part == 4 || part == 5 || part == 8 || part == 9)
				{
					neighbors[0] = cell;
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.W);
					if (part == 1 || part == 5 || part == 9)
					{
						neighbors[1] = neighbors[4] = cell;
					}
				}
				else
				{
					neighbors[0] = neighbors[1] = neighbors[2] = cell.GetNeighbor(SquareDirection.S);
					neighbors[3] = neighbors[4] = cell;
					if (part == 13 && neighbors[0])
					{
						neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.W);
						neighbors[3] = cell.GetNeighbor(SquareDirection.W);
					}
					else if (part == 12 && neighbors[0])
					{
						neighbors[1] = neighbors[2] = neighbors[0].GetNeighbor(SquareDirection.W);
						neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.W);
					}
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
		var neighborParts = new int[6];
		switch (orientation)
		{
			case SquareDirection.N:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { 3, 1, 11, 8, 12, part };
						break;
					case 1:
						neighborParts = new int[] { 0, 12, 8, 9, 13, part  };
						break;
					case 2:
						neighborParts = new int[] { 1, 13, 9, 10, 14, part  };
						break;
					case 3:
						neighborParts = new int[] { 2, 14, 10, 11, 15, part  };
						break;
					case 4:
						neighborParts = new int[] { 7, 3, 15, 12, 0, part };
						break;
					case 5:
						neighborParts = new int[] { 4, 0, 12, 13, 1, part  };
						break;
					case 6:
						neighborParts = new int[] { 5, 1, 13, 14, 2, part };
						break;
					case 7:
						neighborParts = new int[] { 6, 2, 14, 15, 3 , part };
						break;
					case 8:
						neighborParts = new int[] { 11, 7, 3, 0, 4 , part };
						break;
					case 9:
						neighborParts = new int[] { 8, 4, 0, 1, 5 , part };
						break;
					case 10:
						neighborParts = new int[] { 9, 5, 1, 2, 6, part };
						break;
					case 11:
						neighborParts = new int[] { 10, 6, 2, 3, 7, part };
						break;
					case 12:
						neighborParts = new int[] { 15, 11, 7, 4, 8, part };
						break;
					case 13:
						neighborParts = new int[] { 12, 8, 4, 5, 9 , part};
						break;
					case 14:
						neighborParts = new int[] { 13, 9, 5, 6, 10 , part};
						break;
					default:
						neighborParts = new int[] { 14, 10, 6, 7, 11 , part};
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { 12,13,14,2,1, part };
						break;
					case 1:
						neighborParts = new int[] { 13,14,15,3,2, part };
						break;
					case 2:
						neighborParts = new int[] { 14,15,12,0,3, part };
						break;
					case 3:
						neighborParts = new int[] { 15,12,13,1,0, part };
						break;
					case 4:
						neighborParts = new int[] { 0,1,2,6,5, part };
						break;
					case 5:
						neighborParts = new int[] {1,2,3,7,6, part };
						break;
					case 6:
						neighborParts = new int[] { 2,3,0,4,7, part };
						break;
					case 7:
						neighborParts = new int[] { 3,0,1,5,4, part };
						break;
					case 8:
						neighborParts = new int[] { 4,5,6,10,9, part };
						break;
					case 9:
						neighborParts = new int[] { 5,6,7,11,10, part };
						break;
					case 10:
						neighborParts = new int[] { 6,7,4,8,11, part };
						break;
					case 11:
						neighborParts = new int[] { 7,4,5,9,8, part };
						break;
					case 12:
						neighborParts = new int[] { 8,9,10,14,13, part };
						break;
					case 13:
						neighborParts = new int[] { 9,10,11,15,14, part };
						break;
					case 14:
						neighborParts = new int[] { 10,11,8,12,15, part };
						break;
					default:
						neighborParts = new int[] { 11,8,9,13,12, part };
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { 1,5,9,8,4, part };
						break;
					case 1:
						neighborParts = new int[] { 2,6,10,9,5, part };
						break;
					case 2:
						neighborParts = new int[] { 3,7,11,10,6 , part};
						break;
					case 3:
						neighborParts = new int[] { 0,4,8,11,7, part };
						break;
					case 4:
						neighborParts = new int[] { 5,9,13,12,8 , part};
						break;
					case 5:
						neighborParts = new int[] { 6,10,14,13,9 , part};
						break;
					case 6:
						neighborParts = new int[] { 7,11,15,14,10, part };
						break;
					case 7:
						neighborParts = new int[] { 4,8,12,15,11, part };
						break;
					case 8:
						neighborParts = new int[] { 9,13,1,0,12, part };
						break;
					case 9:
						neighborParts = new int[] { 10,14,2,1,13, part };
						break;
					case 10:
						neighborParts = new int[] { 11,15,3,2,14, part };
						break;
					case 11:
						neighborParts = new int[] { 8,12,0,3,15, part };
						break;
					case 12:
						neighborParts = new int[] { 13,1,5,4,0, part };
						break;
					case 13:
						neighborParts = new int[] { 14,2,6,5,1, part };
						break;
					case 14:
						neighborParts = new int[] { 15,3,7,6,2, part };
						break;
					default:
						neighborParts = new int[] { 12,0,4,7,3, part };
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { 4,7,6,2,3, part };
						break;
					case 1:
						neighborParts = new int[] { 5,4,7,3,0, part };
						break;
					case 2:
						neighborParts = new int[] { 6,5,4,0,1 , part};
						break;
					case 3:
						neighborParts = new int[] { 7,6,5,1,2 , part};
						break;
					case 4:
						neighborParts = new int[] { 8,11,10,6,7 , part};
						break;
					case 5:
						neighborParts = new int[] { 9,8,11,7,4, part };
						break;
					case 6:
						neighborParts = new int[] { 10,9,8,4,5, part };
						break;
					case 7:
						neighborParts = new int[] { 11,10,9,5,6 , part};
						break;
					case 8:
						neighborParts = new int[] { 12,15,14,10,11, part};
						break;
					case 9:
						neighborParts = new int[] { 13,12,15,11,8, part };
						break;
					case 10:
						neighborParts = new int[] { 14,13,12,8,9, part };
						break;
					case 11:
						neighborParts = new int[] { 15,14,13,9,10, part };
						break;
					case 12:
						neighborParts = new int[] { 0,3,2,14,15, part};
						break;
					case 13:
						neighborParts = new int[] {1,0,3,15,12, part };
						break;
					case 14:
						neighborParts = new int[] { 2,1,0,12,13, part };
						break;
					default:
						neighborParts = new int[] { 3,2,1,13,14 , part};
						break;
				}
				break;
		}

		return neighborParts;
	}
}
