using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Elément de base de la grille. Chaque grille est divisée en un certain nombre de chunks, donné par les dimensions x 
/// et z de la maison.
/// Chaque chunk a ses propres meshs, canvas, cellules, points et murs.
/// Diviser la grille en chunks permet de rafraîchir seulement une partie de la grille lorsque c'est nécessaire, évitant
/// ainsi des opérations inutiles.
/// </summary>
public class GridChunk : NetworkBehaviour {

	//éléments du chunk
	Cell[] cells;
	public Dot[] dots;
	public Wall[] Walls;
	public SquareMesh terrain, celling, walls;
	public WallsManager wallsManager;

	//références à d'autres scripts
	public NavigationBaker baker;
	public HomeEditor editor;

	//Mode de rafraichissement du chunk. "whole" rafraichit tous les éléments, "cellsOnly" ne rafraichit que les 
	//cellules.
	enum RefreshMode {whole,cellsOny}
	RefreshMode refreshMode;

	//étage du chunk
	public FloorGrid floor;

	//couleur de base des cellules
	static Color color = new Color(1f, 0f, 0f);

	/// <summary>
	/// Etablit les références aux paramètres.
	/// Initialise le tableau des cellules.
	/// </summary>
	void Awake()
	{
		refreshMode = RefreshMode.whole;
		baker = GameObject.Find("NavigationBaker").GetComponent<NavigationBaker>();
		editor = GameObject.Find("Editor").GetComponent<HomeEditor>();
		cells = new Cell[Metrics.chunkSizeX * Metrics.chunkSizeZ];
	}

	/// <summary>
	/// Ajoute la cellule donnée au tableau "cells" avec l'indice "index".
	/// </summary>
	public void AddCell(int index, Cell cell)
	{
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
	}

	/// <summary>
	/// Ajoute le point donné au tableau "dots" avec l'indice "index".
	/// </summary>
	public void AddDot(int index, Dot dot)
	{
		dots[index] = dot;
		dot.chunk = this;
		dot.transform.SetParent(transform, false);
	}

	/// <summary>
	/// Provoque la triangulation de l'ensemble des meshs.
	/// </summary>
	public void Refresh()
	{
		enabled = true;
		refreshMode = RefreshMode.whole;
	}

	/// <summary>
	/// Force la triangulation sans attendre le lateUpdate.
	/// Appelé lorsque les chunks sont combinés ou désunis.
	/// </summary>
	public void ForcedRefresh()
	{
		Triangulate();
		enabled = false;
	}

	/// <summary>
	/// Provoque la triangulation du mesh du sol seulement.
	/// </summary>
	public void RefreshCellsOnly()
	{
		enabled = true;
		refreshMode = RefreshMode.cellsOny;
	}

	/// <summary>
	/// Méthode native à Unity appelée en fin de frame. 
	/// La triangulation est effectuée en LateUpdate pour ne pas provoquer d'erreur avec les autres méthodes.
	/// </summary>
	void LateUpdate()
	{
		if (refreshMode == RefreshMode.whole)
		{
			Triangulate();
		}
		else {
			TriangulateCellsOnly();
		}

		enabled = false;
	}

	/// <summary>
	/// Nettoie les meshs, puis les recalcule.
	/// </summary>
	public void Triangulate()
	{
		terrain.Clear();
		celling.Clear();
		walls.Clear();
		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}
		for (int i = 0; i < dots.Length; i++)
		{
			Triangulate(dots[i]);
		}
		terrain.Apply();
		celling.Apply();
		walls.Apply();
	}

	/// <summary>
	/// Nettoie le mesh du sol seulement puis le recalcule.
	/// </summary>
	void TriangulateCellsOnly()
	{
		terrain.Clear();
		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}
		terrain.Apply();
	}

	/// <summary>
	/// Triangule la cellule donnée.
	/// </summary>
	void Triangulate(Cell cell)
	{
		for (SquareDirection d = SquareDirection.N; d <= SquareDirection.E; d++)
		{
			Triangulate(d, cell);
		}
	}

	/// <summary>
	/// Triangule le quart de la cellule donnée correspondant à la direction donnée.
	/// Les variables "ti" correspondent aux types de la cellule, permettant d'attribuer les textures.
	/// Un type nul correspond à une texture vide.
	/// </summary>
	public void Triangulate(SquareDirection direction, Cell cell)
	{
		Vector3 center = cell.Position;
		var offSet = new Vector3(0f, -0.2f, 0f);
		int i = (int)direction;
		Color c1, c2, c3, c4;
		c1 = color;
		c2 = color;
		c3 = color;
		c4 = color;
		int t1, t2, t3, t4;
		t1 = cell.Colors[i * 4];
		t2 = cell.Colors[1 + i * 4];
		t3 = cell.Colors[2 + i * 4];
		t4 = cell.Colors[3 + i * 4];

		Vector3 edgeMiddleLeft = Metrics.cellMiddleEdge[i];
		Vector3 edgeMiddleRight = Metrics.cellMiddleEdge[i + 1];

		if (t1 > 0)
		{
			terrain.AddTriangle(center, center + edgeMiddleLeft, center + Metrics.cellInnerSquare[i]);
			terrain.AddTriangleColor(color);
			terrain.AddTriangleFloorTypes(new Vector3(t1, t1, t1));

			if (t1 != 2)
			{
				celling.AddTriangle(center + edgeMiddleLeft + offSet, center + offSet, center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddTriangleColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center, center + edgeMiddleLeft, 
				                center + offSet, center + edgeMiddleLeft + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + edgeMiddleLeft, center + Metrics.cellInnerSquare[i],
								center + edgeMiddleLeft + offSet, center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + Metrics.cellInnerSquare[i], center,
								center + Metrics.cellInnerSquare[i] + offSet, center + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));
			}
		}
		if (t2 > 0)
		{
			terrain.AddTriangle(center + edgeMiddleLeft, center + Metrics.cellCorners[i], center + Metrics.cellInnerSquare[i]);
			terrain.AddTriangleColor(color);
			terrain.AddTriangleFloorTypes(new Vector3(t2, t2, t2));

			if (t2 != 2)
			{
				celling.AddTriangle(center + Metrics.cellCorners[i] + offSet, center + edgeMiddleLeft + offSet, center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddTriangleColor(new Color(1f, 1f, 1f, c2.a));

				celling.AddQuad(center + edgeMiddleLeft, center + Metrics.cellCorners[i],
				                center + edgeMiddleLeft + offSet, center + Metrics.cellCorners[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + Metrics.cellCorners[i], center + Metrics.cellInnerSquare[i],
				                center + Metrics.cellCorners[i] + offSet, center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + Metrics.cellInnerSquare[i], center + edgeMiddleLeft,
								center + Metrics.cellInnerSquare[i] + offSet, center + edgeMiddleLeft + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));
			}
		}
		if (t3 > 0)
		{
			terrain.AddTriangle(center + Metrics.cellInnerSquare[i], center + Metrics.cellCorners[i], center + edgeMiddleRight);
			terrain.AddTriangleColor(color);
			terrain.AddTriangleFloorTypes(new Vector3(t3, t3, t3));

			if (t3 != 2)
			{
				celling.AddTriangle(center + Metrics.cellCorners[i] + offSet, center + Metrics.cellInnerSquare[i] + offSet, center + edgeMiddleRight + offSet);
				celling.AddTriangleColor(new Color(1f, 1f, 1f, c3.a));

				celling.AddQuad(center + Metrics.cellCorners[i], center + edgeMiddleRight,
				                center + Metrics.cellCorners[i] + offSet, center + edgeMiddleRight + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + edgeMiddleRight, center + Metrics.cellInnerSquare[i],
								center + edgeMiddleRight + offSet,center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + Metrics.cellInnerSquare[i], center + Metrics.cellCorners[i],
								center + Metrics.cellInnerSquare[i] + offSet, center + Metrics.cellCorners[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));
			}
		}
		if (t4 > 0)
		{
			terrain.AddTriangle(center, center + Metrics.cellInnerSquare[i], center + edgeMiddleRight);
			terrain.AddTriangleColor(color);
			terrain.AddTriangleFloorTypes(new Vector3(t4, t4, t4));

			if (t4 != 2)
			{
				celling.AddTriangle(center + Metrics.cellInnerSquare[i] + offSet, center + offSet, center + edgeMiddleRight + offSet);
				celling.AddTriangleColor(new Color(1f, 1f, 1f, c4.a));

				celling.AddQuad(center + edgeMiddleRight,center,
								center + edgeMiddleRight + offSet, center + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center, center + Metrics.cellInnerSquare[i],
								center + offSet, center + Metrics.cellInnerSquare[i] + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));

				celling.AddQuad(center + Metrics.cellInnerSquare[i], center + edgeMiddleRight,
								center + Metrics.cellInnerSquare[i] + offSet, center + edgeMiddleRight + offSet);
				celling.AddQuadColor(new Color(1f, 1f, 1f, c1.a));
			}
		}
	}

	/// <summary>
	/// Triangule le point donné.
	/// </summary>
	void Triangulate(Dot dot)
	{
		for (CompassDirection d = CompassDirection.N; d <= CompassDirection.SE; d++)
		{
			Triangulate(d, dot);
		}
		wallsManager.CmdAddWallWedge(dot);
	}

	/// <summary>
	/// Triangule le point donné selon la dircetion donnée.
	/// Si le point a un mur dans cette direction, le triangule.
	/// </summary>
	void Triangulate(CompassDirection direction, Dot dot)
	{
		Vector3 center = dot.Position;
		Wall wall = dot.GetWall(direction);
		if (dot.isWalled(direction))
		{
			wallsManager.CmdAddWall(wall, direction);
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des cellules du chunk.
	/// </summary>
	public void EnableCellsCollider(bool enabled)
	{
		foreach (Cell cell in cells)
		{
			cell.BoxActive = enabled;
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des features des cellules du chunk.
	/// </summary>
	public void EnableFeaturesCollider(bool enabled)
	{
		foreach (Cell cell in cells)
		{
			cell.FeaturesActive = enabled;
		}
	}

	/// <summary>
	/// Active ou désactive les colliders des placeableObjects des cellules du chunk.
	/// </summary>
	public void EnableObjectsCollider(bool enabled)
	{
		foreach (Cell cell in cells)
		{
			cell.ObjectsColliderActive = enabled;
		}
	}

	/// <summary>
	/// Active ou désactive le meshCollider du sol.
	/// </summary>
	public void EnableFloorCollider(bool enabled)
	{
		terrain.EnableCollider(enabled);
	}
}
