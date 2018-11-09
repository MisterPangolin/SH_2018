using UnityEngine;

/// <summary>
/// Regroupe en un seul mesh tous les meshs du même nom d'un étage.
/// Cette combinaison de meshs permet d'éviter des problèmes non désirables de lumières en vue à la première personne,
/// lorsque la qualité de la caméra doit augmenter pour rendre un environnement le plus réaliste possible.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CombineMesh : MonoBehaviour {

	public FloorGrid floor;

	/// <summary>
	/// Combine en un seul mesh tous les meshs portant le nom "meshType" dans la liste de chunks donnée.
	/// </summary>
	public void Combine(GridChunk[] chunks, string meshType)
	{
		MeshFilter[] meshFilters = new MeshFilter[chunks.Length];
		for (int u = 0; u < chunks.Length; u++)
		{
			meshFilters[u] = chunks[u].transform.Find(meshType).GetComponent<MeshFilter>();
		}
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length)
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
			Transform t = meshFilters[i].transform;
			t.position = new Vector3(t.position.x, 0f, t.position.z);
			combine[i].transform = t.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
		transform.gameObject.SetActive(true);
	}

	/// <summary>
	/// Sépare le mesh et réattribue les meshs portant le nom meshType à leurs chunks.
	/// </summary>
	public void Desunite(GridChunk[] chunks, string meshType)
	{
		transform.GetComponent<MeshFilter>().mesh = null;
		transform.GetComponent<MeshCollider>().sharedMesh = null;

		for (int u = 0; u < chunks.Length; u++)
		{
			chunks[u].transform.Find(meshType).gameObject.SetActive(true);
			Transform t = chunks[u].transform.Find(meshType).transform;
			t.localPosition = new Vector3(t.localPosition.x, 0f, t.localPosition.z);
		}
		transform.gameObject.SetActive(false);
	}
}
