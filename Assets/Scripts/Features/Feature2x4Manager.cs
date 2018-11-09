using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 2*4 16emes de cellule.
/// </summary>
public class Feature2x4Manager : Feature {

	/// <summary>
	/// Détermine la taille de l'objet.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc2x4;
	}

	/// <summary>
	/// Calcule la position de l'objet.
	/// </summary>
	public override void CalculatePosition(int part)
	{
		switch (orientation)
		{
			case SquareDirection.N:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-5f, 0 / 2f, 7.5f);
						break;
					case 1:
						transform.position = new Vector3(-2.5f, 0 / 2f, 7.5f);
						break;
					case 2:
						transform.position = new Vector3(0f, 0 / 2f, 7.5f);
						break;
					case 3:
						transform.position = new Vector3(2.5f, 0 / 2f, 7.5f);
						break;
					case 4:
						transform.position = new Vector3(-5f, 0 / 2f, 5f);
						break;
					case 5:
						transform.position = new Vector3(-2.5f, 0 / 2f, 5f);
						break;
					case 6:
						transform.position = new Vector3(0f, 0 / 2f, 5f);
						break;
					case 7:
						transform.position = new Vector3(2.5f, 0 / 2f, 5f);
						break;
					case 8:
						transform.position = new Vector3(-5f, 0 / 2f, 2.5f);
						break;
					case 9:
						transform.position = new Vector3(-2.5f, 0 / 2f, 2.5f);
						break;
					case 10:
						transform.position = new Vector3(0f, 0 / 2f, 2.5f);
						break;
					case 11:
						transform.position = new Vector3(2.5f, 0 / 2f, 2.5f);
						break;
					case 12:
						transform.position = new Vector3(-5f, 0 / 2f, 0f);
						break;
					case 13:
						transform.position = new Vector3(-2.5f, 0 / 2f, 0f);
						break;
					case 14:
						transform.position = new Vector3(0f, 0 / 2f, 0f);
						break;
					default:
						transform.position = new Vector3(2.5f, 0 / 2f, 0f);
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(0f, 0 / 2f, 5f);
						break;
					case 1:
						transform.position = new Vector3(2.5f, 0 / 2f, 5f);
						break;
					case 2:
						transform.position = new Vector3(5f, 0 / 2f, 5f);
						break;
					case 3:
						transform.position = new Vector3(7.5f, 0 / 2f, 5f);
						break;
					case 4:
						transform.position = new Vector3(0f, 0 / 2f, 2.5f);
						break;
					case 5:
						transform.position = new Vector3(2.5f, 0 / 2f, 2.5f);
						break;
					case 6:
						transform.position = new Vector3(5f, 0 / 2f, 2.5f);
						break;
					case 7:
						transform.position = new Vector3(7.5f, 0 / 2f, 2.5f);
						break;
					case 8:
						transform.position = new Vector3(0f, 0 / 2f, 0f);
						break;
					case 9:
						transform.position = new Vector3(2.5f, 0 / 2f, 0f);
						break;
					case 10:
						transform.position = new Vector3(5f, 0 / 2f, 0f);
						break;
					case 11:
						transform.position = new Vector3(7.5f, 0 / 2f, 0f);
						break;
					case 12:
						transform.position = new Vector3(0f, 0 / 2f, -2.5f);
						break;
					case 13:
						transform.position = new Vector3(2.5f, 0 / 2f, -2.5f);
						break;
					case 14:
						transform.position = new Vector3(5f, 0 / 2f, -2.5f);
						break;
					default:
						transform.position = new Vector3(7.5f, 0 / 2f, -2.5f);
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-2.5f, 0 / 2f, 0f);
						break;
					case 1:
						transform.position = new Vector3(0f, 0 / 2f, 0f);
						break;
					case 2:
						transform.position = new Vector3(2.5f, 0 / 2f, 0f);
						break;
					case 3:
						transform.position = new Vector3(5f, 0 / 2f, 0f);
						break;
					case 4:
						transform.position = new Vector3(-2.5f, 0 / 2f, -2.5f);
						break;
					case 5:
						transform.position = new Vector3(0f, 0 / 2f, -2.5f);
						break;
					case 6:
						transform.position = new Vector3(2.5f, 0 / 2f, -2.5f);
						break;
					case 7:
						transform.position = new Vector3(5f, 0 / 2f, -2.5f);
						break;
					case 8:
						transform.position = new Vector3(-2.5f, 0 / 2f, -5f);
						break;
					case 9:
						transform.position = new Vector3(0f, 0 / 2f, -5f);
						break;
					case 10:
						transform.position = new Vector3(2.5f, 0 / 2f, -5f);
						break;
					case 11:
						transform.position = new Vector3(5f, 0 / 2f, -5f);
						break;
					case 12:
						transform.position = new Vector3(-2.5f, 0 / 2f, -7.5f);
						break;
					case 13:
						transform.position = new Vector3(0f, 0 / 2f, -7.5f);
						break;
					case 14:
						transform.position = new Vector3(2.5f, 0 / 2f, -7.5f);
						break;
					default:
						transform.position = new Vector3(5f, 0 / 2f, -7.5f);
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						transform.position = new Vector3(-7.5f, 0 / 2f, 2.5f);
						break;
					case 1:
						transform.position = new Vector3(-5f, 0 / 2f, 2.5f);
						break;
					case 2:
						transform.position = new Vector3(-2.5f, 0 / 2f, 2.5f);
						break;
					case 3:
						transform.position = new Vector3(0f, 0 / 2f, 2.5f);
						break;
					case 4:
						transform.position = new Vector3(-7.5f, 0 / 2f, 0f);
						break;
					case 5:
						transform.position = new Vector3(-5f, 0 / 2f, 0f);
						break;
					case 6:
						transform.position = new Vector3(-2.5f, 0 / 2f, 0f);
						break;
					case 7:
						transform.position = new Vector3(0f, 0 / 2f, 0f);
						break;
					case 8:
						transform.position = new Vector3(-7.5f, 0 / 2f, -2.5f);
						break;
					case 9:
						transform.position = new Vector3(-5f, 0 / 2f, -2.5f);
						break;
					case 10:
						transform.position = new Vector3(-2.5f, 0 / 2f, -2.5f);
						break;
					case 11:
						transform.position = new Vector3(0f, 0 / 2f, -2.5f);
						break;
					case 12:
						transform.position = new Vector3(-7.5f, 0 / 2f, -5f);
						break;
					case 13:
						transform.position = new Vector3(-5f, 0 / 2f, -5f);
						break;
					case 14:
						transform.position = new Vector3(-2.5f, 0 / 2f, -5f);
						break;
					default:
						transform.position = new Vector3(0f, 0 / 2f, -5f);
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
		var neighbors = new Cell[8];
		neighbors[0] = cell;
		switch (orientation)
		{
			case SquareDirection.N:
				if (part >= 13)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell, cell, cell, cell };
				}
				else if (part !=0 && part != 4 && part != 8 && part != 12)
				{
					neighbors[1] = cell;
					neighbors[2] = neighbors[3] = neighbors[4] = neighbors[5] = 
						neighbors[6] = neighbors[7] = cell.GetNeighbor(SquareDirection.N);
					if (part >= 5)
					{
						neighbors[2] = neighbors[7] = cell;
						if (part >= 9)
						{
							neighbors[3] = neighbors[6] = cell;
						}
					}
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = 
						cell.GetNeighbor(SquareDirection.W);
					neighbors[5] = neighbors[6] = neighbors[7] = cell;
					if (part == 8 && neighbors[1])
					{
						neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.N);
						neighbors[5] = cell.GetNeighbor(SquareDirection.N);
					}
					if (part == 4 && neighbors[1])
					{
						neighbors[3] = neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.N);
						neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.N);
					}
					else if (part == 0 && neighbors[1])
					{
						neighbors[2] = neighbors[3] = neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.N);
						neighbors[4] = neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.N);
					}
				}
				break;
			case SquareDirection.E:
				if (part > 3)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = neighbors[5] = neighbors[6] =
						neighbors[7] = cell.GetNeighbor(SquareDirection.E);
					if (part != 7 || part != 11 || part != 15)
					{
						neighbors[2] = neighbors[7] = cell;
						if (part != 6 || part != 10 || part != 14)
						{
							neighbors[3] = neighbors[6] = cell;
							if (part != 5 || part != 9 || part != 13)
							{
								neighbors[2] = neighbors[7] = cell;
							}
						}
					}
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.N);
					neighbors[4] = neighbors[5] = neighbors[6] = cell;
					if (part == 1 && neighbors[1])
					{
						neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.E);
						neighbors[5] = cell.GetNeighbor(SquareDirection.E);
					}
					if (part == 2 && neighbors[1])
					{
						neighbors[2] = neighbors[3] = neighbors[1].GetNeighbor(SquareDirection.E);
						neighbors[4] = neighbors[3] = cell.GetNeighbor(SquareDirection.E);
					}
					else if (part == 3 && neighbors[1])
					{
						neighbors[1] = neighbors[2] = neighbors[3] = neighbors[1].GetNeighbor(SquareDirection.E);
						neighbors[4] = neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.E);
					}
				}
				break;
			case SquareDirection.S:
				if (part <= 2)
				{
					neighbors = new Cell[] { cell, cell, cell, cell, cell, cell, cell, cell };
				}
				else if (part != 3 && part != 7 && part != 11 && part != 15)
				{
					neighbors[1] = cell;
					neighbors[2] = neighbors[3] = neighbors[4] = neighbors[5] =
						neighbors[6] = neighbors[7] = cell.GetNeighbor(SquareDirection.S);
					if (part <= 10)
					{
						neighbors[2] = neighbors[7] = cell;
						if (part <= 6)
						{
							neighbors[1] = neighbors[4] = cell;
						}
					}
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] =
						cell.GetNeighbor(SquareDirection.E);
					neighbors[5] = neighbors[6] = neighbors[7] = cell;
					if (part == 7 && neighbors[1])
					{
						neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.S);
						neighbors[5] = cell.GetNeighbor(SquareDirection.S);
					}
					if (part == 11 && neighbors[1])
					{
						neighbors[3] = neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.S);
						neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.S);
					}
					else if (part == 15 && neighbors[1])
					{
						neighbors[2] = neighbors[3] = neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.S);
						neighbors[4] = neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.S);
					}
				}
				break;
			default:
				if (part < 12)
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = neighbors[5] = neighbors[6] =
						neighbors[7] = cell.GetNeighbor(SquareDirection.W);
					if (part != 4 || part != 8 || part != 12)
					{
						neighbors[2] = neighbors[7] = cell;
						if (part != 5 || part != 9 || part != 13)
						{
							neighbors[3] = neighbors[6] = cell;
							if (part != 6 || part != 10 || part != 14)
							{
								neighbors[2] = neighbors[7] = cell;
							}
						}
					}
				}
				else
				{
					neighbors[1] = neighbors[2] = neighbors[3] = neighbors[4] = cell.GetNeighbor(SquareDirection.S);
					neighbors[4] = neighbors[5] = neighbors[6] = cell;
					if (part == 14 && neighbors[1])
					{
						neighbors[4] = neighbors[1].GetNeighbor(SquareDirection.W);
						neighbors[5] = cell.GetNeighbor(SquareDirection.W);
					}
					if (part == 13 && neighbors[1])
					{
						neighbors[2] = neighbors[3] = neighbors[1].GetNeighbor(SquareDirection.W);
						neighbors[4] = neighbors[5] = cell.GetNeighbor(SquareDirection.W);
					}
					else if (part == 12 && neighbors[1])
					{
						neighbors[1] = neighbors[2] = neighbors[3] = neighbors[1].GetNeighbor(SquareDirection.W);
						neighbors[4] = neighbors[5] = neighbors[6] = cell.GetNeighbor(SquareDirection.W);
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
		var neighborParts = new int[8];
		switch (orientation)
		{
			case SquareDirection.N:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { part,3, 15, 11, 7, 4, 8, 12};
						break;
					case 1:
						neighborParts = new int[] { part,0, 12, 8, 4, 5, 9, 13 };
						break;
					case 2:
						neighborParts = new int[] { part,1, 13, 9, 5, 6, 10, 14};
						break;
					case 3:
						neighborParts = new int[] { part,2, 14, 10, 6, 7, 11, 15 };
						break;
					case 4:
						neighborParts = new int[] { part,7, 3, 15, 11, 8, 12, 0 };
						break;
					case 5:
						neighborParts = new int[] { part,4, 0, 12, 8, 9, 13, 1 };
						break;
					case 6:
						neighborParts = new int[] { part,5, 1, 13, 9, 10, 14, 2 };
						break;
					case 7:
						neighborParts = new int[] { part,6, 2, 14, 10, 11, 15, 3 };
						break;
					case 8:
						neighborParts = new int[] { part,11, 7, 3, 15, 12, 0, 4 };
						break;
					case 9:
						neighborParts = new int[] { part,8, 4, 0, 12, 13, 1, 5 };
						break;
					case 10:
						neighborParts = new int[] { part,9, 5, 1, 13, 14, 2, 6 };
						break;
					case 11:
						neighborParts = new int[] { part,10, 6, 2, 14, 15, 3, 7 };
						break;
					case 12:
						neighborParts = new int[] { part,15, 11, 7, 3, 0, 4, 8 };
						break;
					case 13:
						neighborParts = new int[] { part,12, 8, 4, 0, 1, 5, 9 };
						break;
					case 14:
						neighborParts = new int[] { part,13, 9, 5, 1, 2, 6, 10 };
						break;
					default:
						neighborParts = new int[] { part,14, 10, 6, 2, 3, 7, 11 };
						break;
				}
				break;
			case SquareDirection.E:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { part,12, 13, 14, 15, 3,  2, 1 };
						break;
					case 1:
						neighborParts = new int[] { part,13, 14, 15, 12, 0,  3, 2 };
						break;
					case 2:
						neighborParts = new int[] { part,14, 15, 12, 13, 1, 0, 3 };
						break;
					case 3:
						neighborParts = new int[] { part,15, 12, 13, 14, 2, 1, 0 };
						break;
					case 4:
						neighborParts = new int[] { part,0, 1, 2, 3, 7,  6, 5 };
						break;
					case 5:
						neighborParts = new int[] { part,1, 2, 3, 0, 4, 7, 6 };
						break;
					case 6:
						neighborParts = new int[] { part,2, 3, 0, 1, 5, 4, 7 };
						break;
					case 7:
						neighborParts = new int[] { part,3, 0, 1, 2, 6, 5, 4 };
						break;
					case 8:
						neighborParts = new int[] { part,4, 5, 6, 7, 11, 10, 9 };
						break;
					case 9:
						neighborParts = new int[] {part, 5, 6, 7, 4, 8, 11, 10 };
						break;
					case 10:
						neighborParts = new int[] { part,6, 7, 4, 5, 9, 8, 11 };
						break;
					case 11:
						neighborParts = new int[] { part,7, 4, 5, 6, 10, 9, 8 };
						break;
					case 12:
						neighborParts = new int[] { part,8, 9, 10, 11, 15, 14, 13 };
						break;
					case 13:
						neighborParts = new int[] { part,9, 10, 11, 8, 12, 15, 14 };
						break;
					case 14:
						neighborParts = new int[] { part,10, 11, 8, 9, 13, 12, 15 };
						break;
					default:
						neighborParts = new int[] { part,11, 8, 9, 10, 14, 13, 12 };
						break;
				}
				break;
			case SquareDirection.S:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { part,1, 5, 9, 13, 12,  8, 4 };
						break;
					case 1:
						neighborParts = new int[] { part,2, 6, 10, 14, 13, 9, 5 };
						break;
					case 2:
						neighborParts = new int[] { part,3, 7, 11, 15, 14, 10, 6 };
						break;
					case 3:
						neighborParts = new int[] { part,0, 4, 8, 12, 15, 11, 7 };
						break;
					case 4:
						neighborParts = new int[] { part,5, 9, 13, 1, 0, 12, 8 };
						break;
					case 5:
						neighborParts = new int[] { part,6, 10, 14, 2, 1, 13, 9};
						break;
					case 6:
						neighborParts = new int[] { part,7, 11, 15, 3, 2, 14, 10 };
						break;
					case 7:
						neighborParts = new int[] { part,4, 8, 12, 0, 3, 15, 11 };
						break;
					case 8:
						neighborParts = new int[] { part,9, 13, 1, 5, 4, 0, 12 };
						break;
					case 9:
						neighborParts = new int[] { part,10, 14, 2, 6, 5, 1, 13 };
						break;
					case 10:
						neighborParts = new int[] { part,11, 15, 3, 7, 6, 2, 14};
						break;
					case 11:
						neighborParts = new int[] { part, 8, 12, 0, 4, 7, 3, 15 };
						break;
					case 12:
						neighborParts = new int[] { part,13, 1, 5, 9, 8, 4, 0 };
						break;
					case 13:
						neighborParts = new int[] { part, 14, 2, 6, 10, 9, 5, 1};
						break;
					case 14:
						neighborParts = new int[] { part,15, 3, 7, 11, 10, 6, 2 };
						break;
					default:
						neighborParts = new int[] { part,12, 0, 4, 8, 11, 7, 3 };
						break;
				}
				break;
			default:
				switch (part)
				{
					case 0:
						neighborParts = new int[] { part,4, 7, 6, 5, 1, 2, 3};
						break;
					case 1:
						neighborParts = new int[] { part,5, 4, 7, 6, 2, 3, 0};
						break;
					case 2:
						neighborParts = new int[] { part,6, 5, 4, 7, 3, 0, 1};
						break;
					case 3:
						neighborParts = new int[] { part,7, 6, 5, 4, 0, 1, 2};
						break;
					case 4:
						neighborParts = new int[] { part,8, 11, 10, 9, 5, 6, 7 };
						break;
					case 5:
						neighborParts = new int[] { part,9, 8, 11, 10, 6, 7, 4};
						break;
					case 6:
						neighborParts = new int[] { part,10, 9, 8, 11, 7, 4, 5  };
						break;
					case 7:
						neighborParts = new int[] { part,11, 10, 9, 8, 4, 5, 6 };
						break;
					case 8:
						neighborParts = new int[] { part,12, 15, 14, 13, 9, 10, 11 };
						break;
					case 9:
						neighborParts = new int[] { part,13, 12, 15, 14, 10, 11, 8};
						break;
					case 10:
						neighborParts = new int[] { part,14, 13, 12, 15, 11, 8, 9};
						break;
					case 11:
						neighborParts = new int[] { part,15, 14, 13, 12, 8, 9, 10};
						break;
					case 12:
						neighborParts = new int[] { part,0, 3, 2, 1, 13, 14, 15 };
						break;
					case 13:
						neighborParts = new int[] { part,1, 0, 3, 2, 14, 15, 12};
						break;
					case 14:
						neighborParts = new int[] { part,2, 1, 0, 3, 15, 12, 13};
						break;
					default:
						neighborParts = new int[] { part,3, 2, 1, 0, 12, 13, 14};
						break;
				}
				break;
		}

		return neighborParts;
	}
}
