﻿using UnityEngine;

/// <summary>
/// Classe héritant de "Feature" pour permettre le positionnement des objets de dimensions 1*2 16emes de mur.
/// </summary>
public class WallFeature1x2Manager : Feature {

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
	public override void CalculatePosition(CompassDirection wallDirection, int part, int side)
	{
		float x = 0f, y = 0f, z = 0f;
		switch (wallDirection)
		{
			case CompassDirection.N:
				transform.eulerAngles = (side == 1) ? baseRotation + new Vector3(0f, 270f, 0f)
					: baseRotation + new Vector3(0f, 90f, 0f);
				x = (side == 1) ? 0.21f : -0.21f;
				switch (part)
				{
					case 0:
					case 4:
					case 8:
					case 12:
						z = (side == 1) ? -2f * Metrics.innerRadius / 8f : 2f * Metrics.innerRadius / 8f;
						break;
					case 1:
					case 5:
					case 9:
					case 13:
						z = 0f;
						break;
					case 2:
					case 6:
					case 10:
					case 14:
						z = (side == 1) ? 2f * Metrics.innerRadius / 8f : -2f * Metrics.innerRadius / 8f;
						break;
					default:
						z = (side == 1) ? 4f * Metrics.innerRadius / 8f : -4f * Metrics.innerRadius / 8f;
						break;
				}
				break;
			case CompassDirection.NE:
				transform.eulerAngles = (side == 1) ? baseRotation + new Vector3(0f, -45f, 0f)
					: baseRotation + new Vector3(0f, -225f, 0f);
				switch (part)
				{
					case 0:
					case 4:
					case 8:
					case 12:
						x = (side == 1) ? -2f * Metrics.innerRadius / 8f : 2f * Metrics.innerRadius / 8f;
						break;
					case 1:
					case 5:
					case 9:
					case 13:
						x = 0f;
						break;
					case 2:
					case 6:
					case 10:
					case 14:
						x = (side == 1) ? 2f * Metrics.innerRadius / 8f : -2f * Metrics.innerRadius / 8f;
						break;
					default:
						x = (side == 1) ? 4f * Metrics.innerRadius / 8f : -4f * Metrics.innerRadius / 8f;
						break;
				}
				z = x;
				x += (side == 1) ? 0.11f : -0.11f;
				z += (side == 1) ? -0.11f : 0.11f;
				break;
			case CompassDirection.E:
				transform.eulerAngles = (side == 1) ? baseRotation : baseRotation + new Vector3(0f, 180f, 0f);
				z = (side == 1) ? -0.21f : 0.21f;
				switch (part)
				{
					case 0:
					case 4:
					case 8:
					case 12:
						x = (side == 1) ? -2f * Metrics.innerRadius / 8f : 2f * Metrics.innerRadius / 8f;
						break;
					case 1:
					case 5:
					case 9:
					case 13:
						x = 0f;
						break;
					case 2:
					case 6:
					case 10:
					case 14:
						x = (side == 1) ? 2f * Metrics.innerRadius / 8f : -2f * Metrics.innerRadius / 8f;
						break;
					default:
						x = (side == 1) ? 4f * Metrics.innerRadius / 8f : -4f * Metrics.innerRadius / 8f;
						break;
				}
				break;
			default:
				transform.eulerAngles = (side == 1) ? baseRotation + new Vector3(0f, 45f, 0f)
					: baseRotation + new Vector3(0f, 225f, 0f);
				switch (part)
				{
					case 0:
					case 4:
					case 8:
					case 12:
						x = (side == 1) ? -2f * Metrics.innerRadius / 8f : 2f * Metrics.innerRadius / 8f;
						break;
					case 1:
					case 5:
					case 9:
					case 13:
						x = 0f;
						break;
					case 2:
					case 6:
					case 10:
					case 14:
						x = (side == 1) ? 2f * Metrics.innerRadius / 8f : -2f * Metrics.innerRadius / 8f;
						break;
					default:
						x = (side == 1) ? 4f * Metrics.innerRadius / 8f : -4f * Metrics.innerRadius / 8f;
						break;
				}
				z = -x;
				x += (side == 1) ? -0.11f : 0.11f;
				z += (side == 1) ? -0.11f : 0.11f;
				break;

		}
		switch (part)
		{
			case 0:
			case 1:
			case 2:
			case 3:
				y = 7f;
				break;
			case 4:
			case 5:
			case 6:
			case 7:
				y = 5f;
				break;
			case 8:
			case 9:
			case 10:
			case 11:
				y = 3f;
				break;
			default:
				y = 1f;
				break;
		}
		transform.position = new Vector3(x, y * Metrics.wallsElevation / 8f,
										 z);
	}

	public override void AdjustPosition(Wall wall, int side)
	{
		if (xOffset != 0f || zOffset != 0f)
		{
			switch (wall.direction)
			{
				case CompassDirection.N:
					transform.position += (side == 1) ? new Vector3(xOffset, 0f, 0f) : new Vector3(-xOffset, 0f, 0f);
					break;
				case CompassDirection.NE:
					transform.position += (side == 1) ? new Vector3(zOffset, 0f, -zOffset) : new Vector3(-zOffset, 0f, zOffset);
					break;
				case CompassDirection.E:
					transform.position += (side == 1) ? new Vector3(0f, 0f, -xOffset) : new Vector3(0f, 0f, xOffset);
					break;
				default:
					transform.position += (side == 1) ? new Vector3(-zOffset, 0f, -zOffset) : new Vector3(zOffset, 0f, zOffset);
					break;
			}
		}
	}

	/// <summary>
	/// Renvoie les murs voisins sur lesquels l'objet va se placer.
	/// </summary>
	public override Wall[] GetWallNeighbors(Wall wall, int part, int side)
	{
		var neighbors = new Wall[1];
		if (part == 3 || part == 7 || part == 11 || part == 15)
		{
			if (side == 1)
			{
				neighbors[0] = wall.neighbor.GetWall(wall.direction);
			}
			else
			{
				if (wall.dot.GetNeighbor(wall.direction.Opposite()))
				{
					neighbors[0] = wall.dot.GetNeighbor(wall.direction.Opposite()).GetWall(wall.direction);
				}
			}
		}
		else
		{
			neighbors[0] = wall;
		}
		return neighbors;
	}

	/// <summary>
	/// Renvoie les autres 16eme de murs sur lesquels l'objet va se placer.
	/// </summary>
	public override int[] GetWallNeighborsParts(int part)
	{
		var neighborsParts = new int[1];
		if (part == 3 || part == 7 || part == 11 || part == 15)
		{
			neighborsParts[0] = part - 3;
		}
		else
		{
			neighborsParts[0] = part + 1;
		}
		return neighborsParts;
	}
}
