using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script mettant le plan servant de zonnes de tp en RV sous le sol de chque étages
/// </summary>
public class Planconstructor : MonoBehaviour {

    public GameObject plan;
    private GameObject child;

	// Use this for initialization
	void Awake ()
    {
        plan.transform.localScale = new Vector3(GetComponent<FloorGrid>().cellCountX, 1f, GetComponent<FloorGrid>().cellCountZ);
        GameObject pouet = Instantiate(plan);
        pouet.transform.SetParent(GetComponent<Transform>());
        child = pouet;
        float x = ((10 * GetComponent<FloorGrid>().cellCountX) / 2) - 5;
        float z = ((10 * GetComponent<FloorGrid>().cellCountZ) / 2) - 5;
        child.transform.localPosition = new Vector3 (x, 0.1f, z);
        GameObject.Find("Teleporting").GetComponent<Valve.VR.InteractionSystem.Teleport>().UpdatePlan();
	}

    private void Update()
    {
        child.transform.localScale = new Vector3(GetComponent<FloorGrid>().cellCountX, 1f, GetComponent<FloorGrid>().cellCountZ);
        float x = ((10 * GetComponent<FloorGrid>().cellCountX) / 2) - 5;
        float z = ((10 * GetComponent<FloorGrid>().cellCountZ) / 2) - 5;
        child.transform.localPosition = new Vector3(x, 0.1f, z);
    }

}
