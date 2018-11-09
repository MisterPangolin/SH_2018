using UnityEngine;

namespace cakeslice
{
	public class RemoveOutline : MonoBehaviour
	{
		public bool parent;
		public Outline[] outlines;
		public Camera CameraPlayer;

		void Awake()
		{
			CameraPlayer = GameObject.Find("CameraPlayer").GetComponent<Camera>();
			if (!parent)
			{
				outlines = GetComponents<Outline>();
				OutlinesActive = false;
			}
			else
			{
				outlines = GetComponentsInChildren<Outline>();
				OutlinesActive = false;
			}
		}

		void Update()
		{
			if (CameraPlayer.enabled)
			{
				Ray inputRay = CameraPlayer.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(inputRay, out hit) && hit.transform == transform)
				{
					OutlinesActive = true;
				}
				else
				{
					OutlinesActive = false;
				}
			}
			else
			{
				OutlinesActive = false;
			}
		}

		bool outlinesActive = true;
		/// <summary>
		/// Active ou désactive les composants outline de l'objet. 
		/// </summary>
		bool OutlinesActive
		{
			get
			{
				return outlinesActive;
			}
			set
			{
				if (outlinesActive != value)
				{
					foreach (Outline outline in outlines)
					{
						outline.enabled = value;
					}
					outlinesActive = value;
				}
			}
		}
	}
}
