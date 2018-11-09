using UnityEngine;

/// <summary>
/// Classe permettant de trianguler le mesh "walls" des murs.
/// </summary>

public class WallsManager : MonoBehaviour {

	//éditeur
	public HomeEditor editor;

	//épaisseur supplémentaire du mur
	Vector3 offset = Metrics.offset;

	//références aux autres scripts de triangulation
	public DoorsManager doors;
	public WindowsManager windows;

	//mesh où se construisent les murs
	public SquareMesh walls;

	/// <summary>
	/// Etablit la référence à "HomeEditor".
	/// </summary>
	void Awake()
	{
		editor = GameObject.Find("Editor").GetComponent<HomeEditor>();
	}

	/// <summary>
	/// Appelle la méthode homonyme de la classe "SquareMesh" pour former le mesh.
	/// </summary>
	public void Apply()
	{
		walls.Apply();
	}

	/// <summary>
	/// Triangule le mur dans la direction donnée.
	/// </summary>
	public void AddWall(Wall wall, CompassDirection direction)
	{
		Vector3 v1, v2, v3, v4;

		v1 = wall.dot.Position + Metrics.dotEdges[(int)direction];
		v2 = wall.dot.Position + Metrics.dotEdges[(int)direction + 1];
		v4 = wall.neighbor.Position + Metrics.dotEdges[(int)direction.Opposite()];
		v3 = wall.neighbor.Position + Metrics.dotEdges[(int)direction.Opposite() + 1];
		wall.CheckDoorAvaibility();
		wall.CheckWindowAvaibiliy();

		if (!wall.hideOpening)
		{
			if (wall.HasIncomingOpening)
			{
				if (wall.HasIncomingWindow)
				{
					wall.EraseFeatures(wall.dot.GetWall(direction.Opposite()).o_Window.GetComponent<Window>().busyParts);
					windows.AddIncomingWindow(wall, v1, v2, v3, v4, direction, walls);
				}
				else
				{
					wall.EraseFeatures(wall.dot.GetWall(direction.Opposite()).o_Door.GetComponent<Door>().busyParts);
					doors.AddIncomingDoor(wall, v1, v2, v3, v4, direction, walls);
				}
			}
			else if (wall.Window >= 0)
			{
				wall.EraseFeatures(wall.o_Window.GetComponent<Window>().busyParts);
				if (wall.HasOutgoingOpening)
				{
					windows.AddOutgoingWindow(wall, v1, v2, v3, v4, direction, walls);
				}
				else
				{
					windows.AddWindow(wall, v1, v2, v3, v4, direction, walls);
				}
			}
			else if (wall.Door >= 0)
			{
				wall.EraseFeatures(wall.o_Door.GetComponent<Door>().busyParts);
				if (wall.hasOutgoingOpening)
				{
					doors.AddOutgoingDoor(wall, v1, v2, v3, v4, direction, walls);
				}
				else
				{
					doors.AddDoor(wall, v1, v2, v3, v4, direction, walls);
				}

			}
			else
			{
				AddWallSegment(wall.Elevation, v1, v2, v3, v4, wall.ColorLeft, wall.ColorRight);
			}
		}
		else
		{
			AddWallSegment(wall.Elevation, v1, v2, v3, v4, wall.ColorLeft, wall.ColorRight);
		}
		wall.Raised = true;

	}

	/// <summary>
	/// Triangule l'espace entre les murs.
	/// Cet espace correspond au point sur lequel se base les murs.
	/// La triangulation de ce point dépend de l'existence ou non des murs attachés à ce point.
	/// </summary>
	public void AddWallWedge(Dot dot)
	{
		Wall wall;
		float elevation = 0f;
		Color colorLeft = Color.white;
		Color colorRight = Color.white;
		var vElevation = new Vector3(0f,0f,0f);

		for (CompassDirection d = CompassDirection.N; d <= CompassDirection.NW; d++)
		{
			if (dot.isWalled(d))
			{
				elevation = dot.GetWall(d).Elevation;
				colorLeft = dot.GetWall(d).ColorLeft;
				colorRight = dot.GetWall(d).ColorRight;
				vElevation = new Vector3(0f, elevation, 0f) + offset;
			}
		}

		if (elevation > 0)
		{
			CompassDirection[] directions = {
			CompassDirection.NW,
			CompassDirection.NE,
			CompassDirection.SE,
			CompassDirection.SW};

			bool prevWalled, nextWalled, thisWalled, prevHasWindow, nextHasWindow, prevHasDoor, nextHasDoor;

			foreach (CompassDirection d in directions)
			{
				thisWalled = dot.isWalled(d);
				prevWalled = dot.isWalled(d.GetPrevious());
				nextWalled = dot.isWalled(d.GetNext());
				prevHasWindow = prevWalled && dot.HasAWindow;
				nextHasWindow = nextWalled && dot.HasAWindow;
				prevHasDoor = prevWalled && dot.HasADoor;
				nextHasDoor = nextWalled && dot.HasADoor;

				if (!prevWalled)
				{
					if (!nextWalled)
					{
						if (thisWalled)
						{
							wall = dot.GetWall(d);
							colorLeft = dot.GetWall(d).ColorLeft;
							colorRight = dot.GetWall(d).ColorRight;

							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d];
							Vector3 v2 = dot.Position + Metrics.dotEdges[(int)d.GetNext()];

							if ((!dot.HasAWindow && !dot.HasADoor) || (elevation - 1f) < 0.1f)
							{
								Vector3 v3 = dot.Position + Metrics.dotInnerSquare[(int)d];
								Vector3 v4 = dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()];

								walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
								walls.AddQuadColor(Color.white);
								if (d <= CompassDirection.SE)
								{
									AddWallSegment(wall.Elevation, v3, v4, v1, v2, wall.ColorLeft, wall.ColorRight);
								}
								else
								{
									AddWallSegment(wall.Elevation, v3, v4, v1, v2, wall.ColorRight, wall.ColorLeft);
								}
								//ceiling
								walls.AddTriangle(dot.Position + vElevation, v3 + vElevation, v4 + vElevation);
								walls.AddTriangle(v3 + offset, dot.Position + offset, v4 + offset);
								walls.AddTriangleColor(Color.white);
								walls.AddTriangleColor(Color.white);
							}
							else
							{
								if (dot.HasAWindow)
								{
									Vector3 v3 = dot.Position + Metrics.GetMiddle(Metrics.dotEdges[(int)d], Metrics.dotEdges[(int)d.GetNext().Opposite()]);
									Vector3 v4 = dot.Position + Metrics.GetMiddle(Metrics.dotEdges[(int)d.GetNext()], Metrics.dotEdges[(int)d.Opposite()]);
									Window window;
									float heightw, heightw2;
									if (wall.HasOutgoingOpening)
									{
										window = wall.o_Window.GetComponent<Window>();
									}
									else
									{
										window = dot.GetWall(d.Opposite()).o_Window.GetComponent<Window>();
									}
									heightw = window.heightw;
									heightw2 = window.heightw2;
									Vector3 v1w, v2w, v3w, v4w, v1w2, v2w2, v3w2, v4w2;
									v1w = v1 + new Vector3(0f, heightw, 0f);
									v2w = v2 + new Vector3(0f, heightw, 0f);
									v3w = v3 + new Vector3(0f, heightw, 0f);
									v4w = v4 + new Vector3(0f, heightw, 0f);
									v1w2 = v1 - new Vector3(0f, heightw2, 0f);
									v2w2 = v2 - new Vector3(0f, heightw2, 0f);
									v3w2 = v3 - new Vector3(0f, heightw2, 0f);
									v4w2 = v4 - new Vector3(0f, heightw2, 0f);

									walls.AddQuad(v1 + offset, v3 + offset, v1w, v3w);
									walls.AddQuad(v1w2 + vElevation, v3w2 + vElevation, v1 + vElevation, v3 + vElevation);
									walls.AddQuad(v4 + offset, v2 + offset, v4w, v2w);
									walls.AddQuad(v4w2 + vElevation, v2w2 + vElevation, v4 + vElevation, v2 + vElevation);

									walls.AddQuad(v1w, v3w, Metrics.GetMiddle(v1w, v2w), Metrics.GetMiddle(v3w, v4w));
									walls.AddQuad(v3w2 + vElevation, v1w2 + vElevation,
												  Metrics.GetMiddle(v3w2, v4w2) + vElevation, Metrics.GetMiddle(v1w2, v2w2) + vElevation);
									walls.AddQuad(v4w, v2w, Metrics.GetMiddle(v3w, v4w), Metrics.GetMiddle(v1w, v2w));
									walls.AddQuad(v2w2 + vElevation, v4w2 + vElevation,
												  Metrics.GetMiddle(v1w2, v2w2) + vElevation, Metrics.GetMiddle(v3w2, v4w2) + vElevation);

									if (d > CompassDirection.SE)
									{
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorLeft);
									}
									else
									{
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorRight);
									}
									//ceiling
									walls.AddQuad(v1 + offset, v2 + offset, v3 + offset, v4 + offset);
									walls.AddQuad(v3 + vElevation, v4 + vElevation, v1 + vElevation, v2 + vElevation);
									walls.AddQuadColor(Color.white);
									walls.AddQuadColor(Color.white);
								}
								else
								{
									Vector3 v3 = dot.Position + Metrics.GetMiddle(Metrics.dotEdges[(int)d], Metrics.dotEdges[(int)d.GetNext().Opposite()]);
									Vector3 v4 = dot.Position + Metrics.GetMiddle(Metrics.dotEdges[(int)d.GetNext()], Metrics.dotEdges[(int)d.Opposite()]);
									Door door;
									float heightw;
									if (wall.HasOutgoingOpening)
									{
										door = wall.o_Door.GetComponent<Door>();
									}
									else
									{
										door = dot.GetWall(d.Opposite()).o_Door.GetComponent<Door>();
									}
									heightw = door.height;
									Vector3 v1w, v2w, v3w, v4w;
									v1w = v1 - new Vector3(0f, heightw, 0f);
									v2w = v2 - new Vector3(0f, heightw, 0f);
									v3w = v3 - new Vector3(0f, heightw, 0f);
									v4w = v4 - new Vector3(0f, heightw, 0f);

									walls.AddQuad(v1w + vElevation, v3w + vElevation, v1 + vElevation, v3 + vElevation);
									walls.AddQuad(v4w + vElevation, v2w + vElevation, v4 + vElevation, v2 + vElevation);
									walls.AddQuad(v3w + vElevation, v1w + vElevation,
												  Metrics.GetMiddle(v3w, v4w) + vElevation, Metrics.GetMiddle(v1w, v2w) + vElevation);
									walls.AddQuad(v2w + vElevation, v4w + vElevation,
												  Metrics.GetMiddle(v1w, v2w) + vElevation, Metrics.GetMiddle(v3w, v4w) + vElevation);

									if (d > CompassDirection.SE)
									{
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
									}
									else
									{
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
										walls.AddQuadColor(colorLeft);
										walls.AddQuadColor(colorRight);
									}
									//ceiling
									walls.AddQuad(v1 + offset, v2 + offset, v3 + offset, v4 + offset);
									walls.AddQuad(v3 + vElevation, v4 + vElevation, v1 + vElevation, v2 + vElevation);
									walls.AddQuadColor(Color.white);
									walls.AddQuadColor(Color.white);
								}
							}
						}
						else
						{
							Vector3 v3 = dot.Position + Metrics.dotInnerSquare[(int)d];
							Vector3 v4 = dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()];
							if (!dot.HasAWindow && !dot.HasADoor || (elevation - 1f) < 0.1f)
							{
								//walls
								CompassDirection dLeft = d.GetNext();
								CompassDirection dRight = d.GetPrevious();
								while (!dot.isWalled(dLeft) && !dot.isWalled(dRight))
								{
									dLeft = dLeft.GetNext();
									dRight = dRight.GetPrevious();
								}
								if (dLeft == d.Opposite().GetPrevious() && dRight == d.Opposite().GetNext())
								{
									Vector3 v5 = dot.Position + Metrics.dotInnerSquare[(int)d.GetNext()];
									walls.AddQuad(v4 + offset, v5 + offset, v4 + vElevation, v5 + vElevation);
									walls.AddQuad(v5 + offset, v3 + offset, v5 + vElevation, v3 + vElevation);
									Wall leftWall, rightWall;
									if (dot.isWalled(dLeft))
									{
										leftWall = dot.GetWall(dLeft);
										if (dot.isWalled(dRight))
										{
											rightWall = dot.GetWall(dRight);
											switch (d)
											{
												case CompassDirection.NW:
													walls.AddQuadColor(leftWall.ColorLeft);
													walls.AddQuadColor(rightWall.ColorLeft);
													break;
												case CompassDirection.NE:
													walls.AddQuadColor(leftWall.ColorRight);
													walls.AddQuadColor(rightWall.ColorLeft);
													break;
												case CompassDirection.SE:
													walls.AddQuadColor(leftWall.ColorRight);
													walls.AddQuadColor(rightWall.ColorRight);
													break;
												default:
													walls.AddQuadColor(leftWall.ColorLeft);
													walls.AddQuadColor(rightWall.ColorRight);
													break;
											}
										}
										else
										{
											switch (d)
											{
												case CompassDirection.NW:
													walls.AddQuadColor(leftWall.ColorLeft);
													walls.AddQuadColor(leftWall.ColorLeft);
													break;
												case CompassDirection.NE:
													walls.AddQuadColor(leftWall.ColorRight);
													walls.AddQuadColor(leftWall.ColorRight);
													break;
												case CompassDirection.SE:
													walls.AddQuadColor(leftWall.ColorRight);
													walls.AddQuadColor(leftWall.ColorRight);
													break;
												default:
													walls.AddQuadColor(leftWall.ColorLeft);
													walls.AddQuadColor(leftWall.ColorLeft);
													break;
											}
										}
									}
									else
									{
										rightWall = dot.GetWall(dRight);
										switch (d)
										{
											case CompassDirection.NW:
												walls.AddQuadColor(rightWall.ColorLeft);
												walls.AddQuadColor(rightWall.ColorLeft);
												break;
											case CompassDirection.NE:
												walls.AddQuadColor(rightWall.ColorLeft);
												walls.AddQuadColor(rightWall.ColorLeft);
												break;
											case CompassDirection.SE:
												walls.AddQuadColor(rightWall.ColorRight);
												walls.AddQuadColor(rightWall.ColorRight);
												break;
											default:
												walls.AddQuadColor(rightWall.ColorRight);
												walls.AddQuadColor(rightWall.ColorRight);
												break;
										}
									}

									walls.AddTriangle(v3 + vElevation, v5 + vElevation, v4 + vElevation);
									walls.AddTriangle(v5 + offset, v3 + offset, v4 + offset);
									walls.AddTriangleColor(Color.white);
									walls.AddTriangleColor(Color.white);
								}
								else if (dot.isWalled(dLeft))
								{
									walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
									wall = dot.GetWall(dLeft);
									if (dLeft <= CompassDirection.SE)
									{
										walls.AddQuadColor(wall.ColorLeft);
									}
									else {
										walls.AddQuadColor(wall.ColorRight);
									}
								}
								else {
									walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
									wall = dot.GetWall(dRight);
									if (dRight <= CompassDirection.SE)
									{
										walls.AddQuadColor(wall.ColorRight);
									}
									else {
										walls.AddQuadColor(wall.ColorLeft);
									}
								}
								//Ceiling
								walls.AddTriangle(dot.Position + vElevation, v3 + vElevation, v4 + vElevation);
								walls.AddTriangle(v3 + offset, dot.Position + offset, v4 + offset);
								walls.AddTriangleColor(Color.white);
								walls.AddTriangleColor(Color.white);
							}
						}
					}
					else
					{
						if (thisWalled)
						{
							wall = dot.GetWall(d);
							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d];
							Vector3 v2 = dot.Position + Metrics.dotInnerSquare[(int)d];
							//walls
							walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
							if (d <= CompassDirection.SE)
							{
								walls.AddQuadColor(wall.ColorLeft);
							}
							else {
								walls.AddQuadColor(wall.ColorRight);
							}
							//ceiling
							walls.AddTriangle(dot.Position + vElevation, v2 + vElevation, v1 + vElevation);
							walls.AddTriangle(dot.Position + vElevation, 
							                  v1 + vElevation, 
							                  dot.Position + Metrics.dotEdges[(int)d.GetNext()]+ vElevation);
							walls.AddTriangle(dot.Position + vElevation,
							                  dot.Position + Metrics.dotEdges[(int)d.GetNext()]+ vElevation,
							                  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation);
							
							walls.AddTriangle(v2 + offset, dot.Position + offset, v1 + offset);
							walls.AddTriangle(v1 + offset, dot.Position + offset,
											  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset);
							walls.AddTriangle(dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset,
												dot.Position + offset,
											  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset);
							
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);

							//overview
							walls.AddQuad(dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset,
							              v1 + offset,
										  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + vElevation,
										  v1 + vElevation);
							walls.AddQuad(dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset,
										  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset,
										  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation,
										  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + vElevation);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
						}
						else {
							wall = dot.GetWall(d.GetNext());
							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d.GetNext()];
							Vector3 v2 = dot.Position + Metrics.dotInnerSquare[(int)d];

							if (!nextHasWindow && !nextHasDoor)
							{
								//walls
								walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
								if (d == CompassDirection.NW || d == CompassDirection.NE)
								{
									walls.AddQuadColor(wall.ColorLeft);
								}
								else {
									walls.AddQuadColor(wall.ColorRight);
								}
								//overview
								Vector3 v3 = dot.Position + 
								                Metrics.GetMiddle(Metrics.dotEdges[(int)d.GetNext()], 
								                                  Metrics.dotEdges[(int)d.GetSecondNext()]);
								walls.AddQuad(v3 + offset, v1 + offset, v3 + vElevation, v1 + vElevation);
								walls.AddQuadColor(Color.white);
							}
							else
							{
								if ((elevation - 1f) > 0.1f)
								{
									if (nextHasWindow)
									{
										Window window;
										if (wall.HasOutgoingOpening)
										{
											window = wall.o_Window.GetComponent<Window>();
										}
										else
										{
											window = dot.GetWall(d.GetNext().Opposite()).o_Window.GetComponent<Window>();
										}
										Vector3 v1w, v2w, v1w2, v2w2, v3w, v4w, v3w2, v4w2;
										v1w = v1 + new Vector3(0f, window.heightw, 0f);
										v2w = v2 + new Vector3(0f, window.heightw, 0f);
										v1w2 = v1 - new Vector3(0f, window.heightw2, 0f);
										v2w2 = v2 - new Vector3(0f, window.heightw2, 0f);
										v3w = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetSecondNext()]) + new Vector3(0f, window.heightw, 0f);
										v4w = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.Opposite()]) + new Vector3(0f, window.heightw, 0f);
										v3w2 = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetSecondNext()]) - new Vector3(0f, window.heightw2, 0f);
										v4w2 = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.Opposite()]) - new Vector3(0f, window.heightw2, 0f);

										walls.AddQuad(v1 + offset, v2 + offset, v1w, v2w);
										walls.AddQuad(v1w, v2w, v3w, v4w);
										walls.AddQuad(v2w2 + vElevation, v1w2 + vElevation, v4w2 + vElevation, v3w2 + vElevation);
										walls.AddQuad(v1w2 + vElevation, v2w2 + vElevation, v1 + vElevation, v2 + vElevation);

										if (d == CompassDirection.NW || d == CompassDirection.NE)
										{
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
										}
										else {
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
										}
									}
									else
									{
										Door door;
										if (wall.HasOutgoingOpening)
										{
											door = wall.o_Door.GetComponent<Door>();
										}
										else
										{
											door = dot.GetWall(d.GetNext().Opposite()).o_Door.GetComponent<Door>();
										}
										Vector3 v1w, v2w, v3w, v4w;
										v1w = v1 - new Vector3(0f, door.height, 0f);
										v2w = v2 - new Vector3(0f, door.height, 0f);
										v3w = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetSecondNext()]) - new Vector3(0f, door.height, 0f);
										v4w = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.Opposite()]) - new Vector3(0f, door.height, 0f);

										walls.AddQuad(v2w + vElevation, v1w + vElevation, v4w + vElevation, v3w + vElevation);
										walls.AddQuad(v1w + vElevation, v2w + vElevation, v1 + vElevation, v2 + vElevation);

										if (d == CompassDirection.NW || d == CompassDirection.NE)
										{
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
										}
										else {
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
										}
									}
								}
								else
								{
									//walls
									walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
									if (d == CompassDirection.NW || d == CompassDirection.NE)
									{
										walls.AddQuadColor(wall.ColorLeft);
									}
									else {
										walls.AddQuadColor(wall.ColorRight);
									}
								}
							}
							//ceiling
							walls.AddQuad(v2 + vElevation,
										  dot.Position + vElevation,
										  v1 + vElevation,
										  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation);
							walls.AddQuad(dot.Position + offset, v2 + offset, dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset, v1 + offset);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
						}
					}
				}

				else
				{
					if (!nextWalled)
					{
						if (thisWalled)
						{
							wall = dot.GetWall(d);

							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d.GetNext()];
							Vector3 v2 = dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()]; 
							//wall
							walls.AddQuad(v2 + offset, v1 + offset, v2 + vElevation, v1 + vElevation);
							if (d <= CompassDirection.SE)
							{
								walls.AddQuadColor(wall.ColorRight);
							}
							else {
								walls.AddQuadColor(wall.ColorLeft);
							}
							//ceiling
							walls.AddTriangle(dot.Position + vElevation,v1 + vElevation, v2 + vElevation);
							walls.AddTriangle(dot.Position + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d] + vElevation,
							                 v1 + vElevation);
							walls.AddTriangle(dot.Position + vElevation,
											  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d] + vElevation);

							walls.AddTriangle(v1 + offset, dot.Position + offset, v2 + offset);
							walls.AddTriangle(dot.Position + Metrics.dotEdges[(int)d] + offset,
							                  dot.Position + offset, v1 + offset);
							walls.AddTriangle(dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset,dot.Position + offset,
											  dot.Position + Metrics.dotEdges[(int)d] + offset);
							
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);

							//overview
							walls.AddQuad(v1 + offset,
										  dot.Position + Metrics.dotEdges[(int)d] + offset,
										  v1 + vElevation,
										  dot.Position + Metrics.dotEdges[(int)d] + vElevation);
							walls.AddQuad(dot.Position + Metrics.dotEdges[(int)d] + offset,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset,
										  dot.Position + Metrics.dotEdges[(int)d] + vElevation,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
						}
						else {
							wall = dot.GetWall(d.GetPrevious());
							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d];
							Vector3 v2 = dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()];

							if (!prevHasWindow && !prevHasDoor)
							{
								//wall
								walls.AddQuad(v2 + offset, v1 + offset, v2 + vElevation, v1 + vElevation);
								if (d < CompassDirection.S)
								{
									walls.AddQuadColor(wall.ColorRight);
								}
								else {
									walls.AddQuadColor(wall.ColorLeft);
								}
								Vector3 v3 = dot.Position + Metrics.GetMiddle(Metrics.dotEdges[(int)d],
																			  Metrics.dotEdges[(int)d.GetPrevious()]);
								walls.AddQuad(v1 + offset, v3 + offset, v1 + vElevation, v3 + vElevation);
								walls.AddQuadColor(Color.white);
							}
							else
							{
								if ((elevation - 1f) > 0.1f)
								{
									if (prevHasWindow)
									{
										Window window;
										if (wall.HasOutgoingOpening)
										{
											window = wall.o_Window.GetComponent<Window>();
										}
										else
										{
											window = dot.GetWall(d.GetPrevious().Opposite()).o_Window.GetComponent<Window>();
										}

										Vector3 v1w, v2w, v1w2, v2w2, v3w, v4w, v3w2, v4w2;
										v1w = v1 + new Vector3(0f, window.heightw, 0f);
										v2w = v2 + new Vector3(0f, window.heightw, 0f);
										v1w2 = v1 - new Vector3(0f, window.heightw2, 0f);
										v2w2 = v2 - new Vector3(0f, window.heightw2, 0f);
										v3w = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetPrevious()]) + new Vector3(0f, window.heightw, 0f);
										v4w = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondPrevious()]) + new Vector3(0f, window.heightw, 0f);
										v3w2 = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetPrevious()]) - new Vector3(0f, window.heightw2, 0f);
										v4w2 = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondPrevious()]) - new Vector3(0f, window.heightw2, 0f);

										walls.AddQuad(v2 + offset, v1 + offset, v2w, v1w);
										walls.AddQuad(v2w, v1w, v4w, v3w);
										walls.AddQuad(v1w2 + vElevation, v2w2 + vElevation, v3w2 + vElevation, v4w2 + vElevation);
										walls.AddQuad(v2w2 + vElevation, v1w2 + vElevation, v2 + vElevation, v1 + vElevation);

										if (d < CompassDirection.S)
										{
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
										}
										else {
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
										}
									}
									else
									{
										Door door;
										if (wall.HasOutgoingOpening)
										{
											door = wall.o_Door.GetComponent<Door>();
										}
										else
										{
											door = dot.GetWall(d.GetPrevious().Opposite()).o_Door.GetComponent<Door>();
										}

										Vector3 v1w, v2w, v3w, v4w;
										v1w = v1 - new Vector3(0f, door.height, 0f);
										v2w = v2 - new Vector3(0f, door.height, 0f);
										v3w = Metrics.GetMiddle(v1, dot.Position + Metrics.dotEdges[(int)d.GetPrevious()]) - new Vector3(0f, door.height, 0f);
										v4w = Metrics.GetMiddle(v2, dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondPrevious()]) - new Vector3(0f, door.height, 0f);

										walls.AddQuad(v1w + vElevation, v2w + vElevation, v3w + vElevation, v4w + vElevation);
										walls.AddQuad(v2w + vElevation, v1w + vElevation, v2 + vElevation, v1 + vElevation);

										if (d < CompassDirection.S)
										{
											walls.AddQuadColor(colorRight);
											walls.AddQuadColor(colorRight);
										}
										else {
											walls.AddQuadColor(colorLeft);
											walls.AddQuadColor(colorLeft);
										}
									}
								}
								else
								{
									walls.AddQuad(v2 + offset, v1 + offset, v2 + vElevation, v1 + vElevation);
									if (d < CompassDirection.S)
									{
										walls.AddQuadColor(wall.ColorRight);
									}
									else {
										walls.AddQuadColor(wall.ColorLeft);
									}
								}
							}
							//ceiling
							walls.AddQuad(dot.Position + vElevation,
										  v2 + vElevation,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation,
							              v1 + vElevation);
							walls.AddQuad(v2 + offset,dot.Position + offset,v1 + offset,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
						}
					}
					else {
						if (!thisWalled)
						{
							wall = dot.GetWall(d.GetPrevious());
							Wall neighbor = dot.GetWall(d.GetNext());

							Vector3 v1 = dot.Position + Metrics.dotEdges[(int)d];
							Vector3 v2 = dot.Position + Metrics.dotInnerSquare[(int)d.GetNext()];
							Vector3 v3 = dot.Position + Metrics.dotEdges[(int)d.GetNext()];
							//walls
							walls.AddQuad(v2 + offset, v1 + offset, v2 + vElevation, v1 + vElevation);
							walls.AddQuad(v3 + offset, v2 + offset, v3 + vElevation, v2 + vElevation);
							if (d.GetPrevious() <= CompassDirection.SE)
							{
								walls.AddQuadColor(wall.ColorRight);
							}
							else {
								walls.AddQuadColor(wall.ColorLeft);
							}
							if (d.GetNext() <= CompassDirection.SE)
							{
								walls.AddQuadColor(neighbor.ColorLeft);
							}
							else {
								walls.AddQuadColor(neighbor.ColorRight);
							}
							//ceiling
							walls.AddQuad(dot.Position + vElevation,
										  dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()] + vElevation,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation,
										  v1 + vElevation);
							walls.AddQuad(v2 + vElevation,
										  dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()] + vElevation,
										  v3 + vElevation,
										  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation);

							walls.AddQuad(dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()] + offset,
										  dot.Position + offset, v1 + offset,
										  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset);
							walls.AddQuad(dot.Position + Metrics.dotInnerSquare[(int)d.GetSecondNext()] + offset,v2 + offset,
										  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset,
										  v3 + offset);
							
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);

							//overview
							walls.AddQuad(v1 + offset,
							              dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset,
										  v1 + vElevation,
							              dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation);
							walls.AddQuad(dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset, 
							              v3 + offset, 
							              dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation, 
							              v3 + vElevation);
							walls.AddQuadColor(Color.white);
							walls.AddQuadColor(Color.white);
						}
						else
						{
							wall = dot.GetWall(d);
							//ceiling
							walls.AddTriangle(dot.Position + vElevation,
											  dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d] + vElevation);
							walls.AddTriangle(dot.Position + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d] + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + vElevation);
							walls.AddTriangle(dot.Position + vElevation,
											  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + vElevation,
											  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + vElevation);

							walls.AddTriangle(dot.Position + Metrics.GetMiddleEdge(d.GetPrevious()) + offset,
												dot.Position + offset, dot.Position + Metrics.dotEdges[(int)d] + offset);
							walls.AddTriangle(dot.Position + Metrics.dotEdges[(int)d] + offset,
							                  dot.Position + offset,
											  dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset);
							walls.AddTriangle(dot.Position + Metrics.dotEdges[(int)d.GetNext()] + offset,
							                  dot.Position + offset,
											  dot.Position + Metrics.GetMiddleEdge(d.GetNext()) + offset);
							
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
							walls.AddTriangleColor(Color.white);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Triangule un pan de mur sans fenêtre ou porte.
	/// </summary>
	void AddWallSegment(float elevation, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color colorLeft, Color colorRight)
	{
		var vElevation = new Vector3(0f, elevation, 0f) + offset;
		walls.AddQuad(v3 + offset, v1 + offset, v3 + vElevation, v1 + vElevation);
		walls.AddQuad(v2 + offset, v4 + offset, v2 + vElevation, v4 + vElevation);
		walls.AddQuad(v1 + vElevation, v2 + vElevation, v3 + vElevation, v4 + vElevation);
		walls.AddQuad(v2 + offset, v1 + offset, v4 + offset, v3 + offset);

		walls.AddQuadColor(colorLeft);
		walls.AddQuadColor(colorRight);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);

		//overview
		walls.AddQuad(v1 + offset, v2 + offset, v1 + vElevation, v2 + vElevation);
		walls.AddQuad(v4 + offset, v3 + offset, v4 + vElevation, v3 + vElevation);
		walls.AddQuadColor(Color.white);
		walls.AddQuadColor(Color.white);
	}
}