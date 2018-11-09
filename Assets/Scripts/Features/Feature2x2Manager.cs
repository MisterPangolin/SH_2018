using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 2*2 16emes de cellule.
/// </summary>
public class Feature2x2Manager : Feature{

	/// <summary>
	/// Détermine la taille de l'objet.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		blocType = Bloc.bloc2x2;
	}

	/// <summary>
	/// Calcule la position de l'objet.
	/// </summary>
	public override void CalculatePosition(int part)
	{
		switch (part)
		{
			case 0:
				transform.position = new Vector3(-2.5f, 0 / 2f, 2.5f);
				break;
			case 1:
				transform.position = new Vector3(0f, 0 / 2f, 2.5f);
				break;
			case 2:
				transform.position = new Vector3(2.5f, 0 / 2f, 2.5f);
				break;
			case 3:
				transform.position = new Vector3(5f, 0 / 2f, 2.5f);
				break;
			case 4:
				transform.position = new Vector3(-2.5f, 0 / 2f, 0f);
				break;
			case 5:
				transform.position = new Vector3(0f, 0 / 2f, 0f);
				break;
			case 6:
				transform.position = new Vector3(2.5f, 0 / 2f, 0f);
				break;
			case 7:
				transform.position = new Vector3(5f, 0 / 2f, 0f);
				break;
			case 8:
				transform.position = new Vector3(-2.5f, 0 / 2f, -2.5f);
				break;
			case 9:
				transform.position = new Vector3(0f, 0 / 2f, -2.5f);
				break;
			case 10:
				transform.position = new Vector3(2.5f, 0 / 2f, -2.5f);
				break;
			case 11:
				transform.position = new Vector3(5f, 0 / 2f, -2.5f);
				break;
			case 12:
				transform.position = new Vector3(-2.5f, 0 / 2f, -5f);
				break;
			case 13:
				transform.position = new Vector3(0f, 0 / 2f, -5f);
				break;
			case 14:
				transform.position = new Vector3(2.5f, 0 / 2f, -5f);
				break;
			default:
				transform.position = new Vector3(5f, 0 / 2f, -5f);
				break;
		}
		AdjustPosition();
	}

	/// <summary>
	/// Renvoie les cellules voisines sur lesquelles l'objet va se placer.
	/// </summary>
	public override Cell[] GetNeighbors(Cell cell, int part)
	{
		var neighbors = new Cell[4];
		neighbors[3] = cell;
		if (part <= 10 && part != 3 && part != 7)
		{
			neighbors[0] = neighbors[1] = neighbors[2] = cell;
		}
		else if (part == 3 || part == 7 || part == 11)
		{
			neighbors[0] = neighbors[1] = cell.GetNeighbor(SquareDirection.E);
			neighbors[2] = cell;
		}
		else if (part == 15)
		{
			neighbors[0] = cell.GetNeighbor(SquareDirection.E);
			if (neighbors[0] != null)
			{
				neighbors[1] = neighbors[0].GetNeighbor(SquareDirection.S);
			}
			neighbors[2] = cell.GetNeighbor(SquareDirection.S);
		}
		else
		{
			neighbors[0] = cell;
			neighbors[1] = neighbors[2] = cell.GetNeighbor(SquareDirection.S);
		}
		return neighbors;
	}

	/// <summary>
	/// Renvoie les autres 16eme de cellules sur lesquels l'objet va se placer.
	/// </summary>
	public override int[] GetNeighborsParts(int part)
	{
		var neighborParts = new int[4];
		neighborParts[3] = part;
		switch (part)
		{
			case 0:
				neighborParts[0] = 1;
				neighborParts[1] = 5;
				neighborParts[2] = 4;
				break;
			case 1:
				neighborParts[0] = 2;
				neighborParts[1] = 6;
				neighborParts[2] = 5;
				break;
			case 2:
				neighborParts[0] = 3;
				neighborParts[1] = 7;
				neighborParts[2] = 6;
				break;
			case 3:
				neighborParts[0] = 0;
				neighborParts[1] = 4;
				neighborParts[2] = 7;
				break;
			case 4:
				neighborParts[0] = 5;
				neighborParts[1] = 9;
				neighborParts[2] = 8;
				break;
			case 5:
				neighborParts[0] = 6;
				neighborParts[1] = 10;
				neighborParts[2] = 9;
				break;
			case 6:
				neighborParts[0] = 7;
				neighborParts[1] = 11;
				neighborParts[2] = 10;
				break;
			case 7:
				neighborParts[0] = 4;
				neighborParts[1] = 8;
				neighborParts[2] = 11;
				break;
			case 8:
				neighborParts[0] = 9;
				neighborParts[1] = 13;
				neighborParts[2] = 12;
				break;
			case 9:
				neighborParts[0] = 10;
				neighborParts[1] = 14;
				neighborParts[2] = 13;
				break;
			case 10:
				neighborParts[0] = 11;
				neighborParts[1] = 15;
				neighborParts[2] = 14;
				break;
			case 11:
				neighborParts[0] = 8;
				neighborParts[1] = 12;
				neighborParts[2] = 15;
				break;
			case 12:
				neighborParts[0] = 13;
				neighborParts[1] = 1;
				neighborParts[2] = 0;
				break;
			case 13:
				neighborParts[0] = 14;
				neighborParts[1] = 2;
				neighborParts[2] = 1;
				break;
			case 14:
				neighborParts[0] = 15;
				neighborParts[1] = 3;
				neighborParts[2] = 2;
				break;
			default:
				neighborParts[0] = 12;
				neighborParts[1] = 0;
				neighborParts[2] = 3;
				break;
		}

		return neighborParts;
	}
}
