using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Mesh placé en enfant d'un chunk afin de permettre l'affichage du sol, des murs ou des plafonds.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SquareMesh : MonoBehaviour {
	
	//mesh et éléments de construction du mesh
	Mesh squareMesh;
	[NonSerialized]
	List<Vector3> vertices, floorTypes;
	[NonSerialized]
	List<int> triangles;
	[NonSerialized]
	List<Color> colors;
	MeshCollider meshCollider;

	//propriétés optionnelles pouvant être données au mesh
	public bool useCollider, useFloorTypes;

	/// <summary>
	/// Crée un nouveau mesh, et lui ajoute un collider si nécessaire.
	/// </summary>
	void Awake()
	{
		GetComponent<MeshFilter>().mesh = squareMesh = new Mesh();
		if (useCollider)
		{
			meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.enabled = false;
		}
		squareMesh.name = "Square Mesh";
	}

	/// <summary>
	/// Nettoie le mesh en vidant ses listes de couleurs, vertices et triangles.
	/// </summary>
	public void Clear()
	{
		squareMesh.Clear();
		colors = ListPool<Color>.Get();
		vertices = ListPool<Vector3>.Get();
		if (useFloorTypes)
		{
			floorTypes = ListPool<Vector3>.Get();
		}
		triangles = ListPool<int>.Get();
	}

	/// <summary>
	/// Applique au mesh les listes de couleurs, vertices et triangles construites.
	/// </summary>
	public void Apply()
	{
		squareMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		squareMesh.SetColors(colors);
		ListPool<Color>.Add(colors);
		if (useFloorTypes)
		{
			squareMesh.SetUVs(2, floorTypes);
			ListPool<Vector3>.Add(floorTypes);
		}
		squareMesh.SetTriangles(triangles, 0);
		ListPool<int>.Add(triangles);
		squareMesh.RecalculateNormals();
		if (useCollider)
		{
			meshCollider.sharedMesh = squareMesh;
		}
	}

	/// <summary>
	/// Ajoute un triangle à la liste des triangles.
	/// </summary>
	public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	/// <summary>
	/// Ajoute trois couleurs à la liste des couleurs, s'appliquant au triangle précedemment formé.
	/// </summary>
	public void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	/// <summary>
	/// Ajoute un quadrilatère à la liste des triangles en le divisant en deux triangles.
	/// </summary>
	public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		vertices.Add(v4);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	/// <summary>
	/// Ajoute quatre couleurs à la liste des couleurs, s'appliquant au quadrilaètre précedemment formé.
	/// </summary>
	public void AddQuadColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	/// <summary>
	/// Active ou désactive le collider du mesh si useCollider est coché.
	/// </summary>
	public void EnableCollider(bool enabled)
	{
		if (useCollider)
		{
			meshCollider.enabled = enabled;
		}
	}

	/// <summary>
	/// Ajoute à la liste floorTypes trois types de sol.
	/// </summary>
	public void AddTriangleFloorTypes(Vector3 types)
	{
		floorTypes.Add(types);
		floorTypes.Add(types);
		floorTypes.Add(types);
	}

	/// <summary>
	/// Ajoute à la liste floorTypes quatre types de sol.
	/// </summary>
	public void AddQuadFloorTypes(Vector3 types)
	{
		floorTypes.Add(types);
		floorTypes.Add(types);
		floorTypes.Add(types);
		floorTypes.Add(types);
	}

}
