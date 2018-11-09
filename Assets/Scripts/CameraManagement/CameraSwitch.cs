using UnityEngine;

/// <summary>
/// Attachée à l'objet "CameraSwitch" dans la scene "Creation_architecture".
/// Permet d'effectuer les transitions entre les différents modes de vue.
/// </summary>
public class CameraSwitch : MonoBehaviour {

	//référence à lui-même
	static CameraSwitch instance;

	//lumière globale
	public Light sunLight;

	//éditeur, étages et générateur de surfaces
	HomeEditor editor;
	public FloorGrid[] floors;
	public NavigationBaker baker;

	//boutons permettant de passer d'un mode à l'autre
	public GameObject editorToOverview, editorToPlayer, playerToEditor, playerToOverview, overviewToPlayer, 
	overviewToEditor, overviewInterface, editorInterface;

	//référence aux cameras
	public Camera cameraPlayer, renderCameraPlayer;
	public Camera cameraEditor;
	public Camera cameraOverview;
	public SphereCollider overviewCollider;
	Camera activeCamera;
	MapCamera mapCamera;
	public NavigationBehavior navBehavior;

	/// <summary>
	/// Crée une référence unique à la classe dans le projet.
	/// </summary>
	void Awake () {
		
		editor = GameObject.Find("Editor").GetComponent<HomeEditor>();
		instance = this;
		mapCamera = cameraEditor.GetComponentInParent<MapCamera>();
		cameraPlayer.enabled = false;
		renderCameraPlayer.enabled = false;
		cameraOverview.enabled = false;
		overviewCollider.enabled = false;
		cameraEditor.enabled = true;
	}

	/// <summary>
	/// Crée la référence aux étages de "editor".
	/// </summary>
	void Start()
	{
		floors = editor.floors;
	}

	/// <summary>
	/// Met à jour la référence aux étages de "editor".
	/// </summary>
	void Update()
	{
		if (!floors[0])
		{
			floors = editor.floors;
		}
	}

	/// <summary>
	/// Effectue la transition entre les modes Édition et 1ere personne.
	/// </summary>
	public void SetEditorToPlayer()
	{
		if (!cameraOverview.enabled)
		{
			switchMode mode = switchMode.editorToPlayer;
			SetActiveCamera(mode);
		}
	}

	/// <summary>
	/// Effectue la transition entre les modes Édition et vision d'ensemble.
	/// </summary>
	public void SetEditorToOverview()
	{
		if (!cameraPlayer.enabled)
		{
			switchMode mode = switchMode.editorToOverview;
			SetActiveCamera(mode);
		}
	}

	/// <summary>
	/// Effectue la transition entre les modes 1ere personnes et vision d'ensemble.
	/// </summary>
	public void SetPlayerToOverview()
	{
		if (!cameraEditor.enabled)
		{
			switchMode mode = switchMode.playerToOverview;
			SetActiveCamera(mode);
		}
	}

	/// <summary>
	/// Appelée lors du chargement d'une maison pour réinitialiser l'interface.
	/// </summary>
	public static void Load()
	{
		NavigationBehavior.RemoveAgentComponent();
		if (instance.cameraPlayer.enabled)
		{
			instance.SetEditorToPlayer();
		}
		else if (instance.cameraOverview.enabled)
		{
			instance.SetEditorToOverview();
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// Affiche les fonctions pour le mode 1ere personne dans l'interface.
	/// </summary>
	void InterfacePlayer()
	{
		overviewInterface.SetActive(false);
		editorInterface.SetActive(false);
		editorToOverview.SetActive(false);
		editorToPlayer.SetActive(false);
		playerToEditor.SetActive(true);
		playerToOverview.SetActive(true);
		overviewToPlayer.SetActive(false);
		overviewToEditor.SetActive(false);
	}

	/// <summary>
	/// Affiche les fonctions pour le mode vision d'ensemble dans l'interface.
	/// </summary>
	void InterfaceOverview()
	{
		overviewInterface.SetActive(true);
		editorInterface.SetActive(false);
		editorToOverview.SetActive(false);
		editorToPlayer.SetActive(false);
		playerToEditor.SetActive(false);
		playerToOverview.SetActive(false);
		overviewToPlayer.SetActive(true);
		overviewToEditor.SetActive(true);
	}

	/// <summary>
	/// Affiche les fonctions pour le mode Édition dans l'interface.
	/// </summary>
	void InterfaceEditor()
	{
		overviewInterface.SetActive(false);
		editorInterface.SetActive(true);
		editorToOverview.SetActive(true);
		editorToPlayer.SetActive(true);
		playerToEditor.SetActive(false);
		playerToOverview.SetActive(false);
		overviewToPlayer.SetActive(false);
		overviewToEditor.SetActive(false);
	}

	//Etat de la transition
	public enum switchMode { editorToPlayer, editorToOverview, playerToOverview };

	/// <summary>
	/// Active la caméra du mode choisi en désactivant les autres.
	/// Met à jour la maison pour une interaction adéquate avec le mode choisi.
	/// </summary>
	void SetActiveCamera(switchMode mode)
	{
		if (mode == switchMode.editorToPlayer)
		{
			cameraPlayer.enabled = !cameraPlayer.enabled;
			renderCameraPlayer.enabled = !renderCameraPlayer.enabled;
			cameraEditor.enabled = !cameraEditor.enabled;
			mapCamera.SwitchMotion();
			if (cameraPlayer.enabled)
			{
				editor.Clear();
				editor.ShowLevels();
				editor.CheckWallsHidden();
				editor.ShowWalls();

				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.Refresh();
						floor.InteractableUnder(false);
						floor.EnableFloorCollider(true);
						floor.EnableFeaturesCollider(true);
						floor.EnableObjectsCollider(true);
						floor.WallsCollide = true;
						floor.SwitchColliders(true);
						floor.HideCanvas(true);
						floor.SwitchLights(true);
						floor.CombineMesh();
						EnableReflectionProbes(floor, true);
						DeviceStorage.SetOutlines(false, floor.level);
					}
				}
				InterfacePlayer();
				baker.Bake();
				navBehavior.AddAgentComponent();
				sunLight.intensity = 0.2f;
				TimeClock.PassTime = true;
			}
			else {
				NavigationBehavior.Clear();
				editor.HideLevels();
				editor.CheckWallsHidden();
				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.DesuniteMesh();
						floor.Refresh();
						floor.EnableFloorCollider(false);
						floor.EnableFeaturesCollider(false);
						floor.EnableObjectsCollider(false);
						floor.SwitchColliders(false);
						floor.WallsCollide = false;
						floor.SwitchLights(false);
						EnableReflectionProbes(floor, false);
						DeviceStorage.SetOutlines(false, floor.level);
					}
				}
				InterfaceEditor();
				editor.currentFloor.HideCanvas(false);
				editor.ChangeMode();
				sunLight.intensity = 1f;
				TimeClock.PassTime = false;
			}
		}
		else if (mode == switchMode.editorToOverview)
		{
			cameraOverview.enabled = !cameraOverview.enabled;
			overviewCollider.enabled = !overviewCollider.enabled;
			cameraEditor.enabled = !cameraEditor.enabled;
			mapCamera.SwitchMotion();
			if (cameraOverview.enabled)
			{
				editor.Clear();
				editor.ShowWalls();
				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.InteractableUnder(false);
						floor.WallsCollide = false;
						floor.HideCanvas(true);
						DeviceStorage.SetOutlines(false, floor.level);
					}
				}
				editor.currentFloor.WallsCollide = true;
				InterfaceOverview();
				DeviceStorage.SetOutlines(true, editor.currentFloor.level);
				DeviceStorage.EnableColliders(true, editor.currentFloor.level);
			}
			else {
				OverviewCamera.Clear();
				editor.ShowWalls();
				if (editor.hidden)
				{
					editor.HideWalls();
				}

				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.EnableFloorCollider(false);
						floor.WallsCollide = false;
						DeviceStorage.SetOutlines(false, floor.level);
					}
				}
				InterfaceEditor();
				DeviceStorage.EnableColliders(false, editor.currentFloor.level);
				editor.currentFloor.HideCanvas(false);
				editor.ChangeMode();
			}
		}
		else {
			cameraOverview.enabled = !cameraOverview.enabled;
			overviewCollider.enabled = !overviewCollider.enabled;
			cameraPlayer.enabled = !cameraPlayer.enabled;
			renderCameraPlayer.enabled = !renderCameraPlayer.enabled;
			if (cameraPlayer.enabled)
			{
				OverviewCamera.Clear();
				editor.ShowLevels();
				editor.CheckWallsHidden();
				editor.ShowWalls();

				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.InteractableUnder(false);
						floor.EnableFloorCollider(true);
						floor.EnableFeaturesCollider(true);
						floor.EnableObjectsCollider(true);
						floor.WallsCollide = true;
						floor.SwitchColliders(true);
						floor.SwitchLights(true);
						floor.CombineMesh();
						EnableReflectionProbes(floor, true);
						DeviceStorage.SetOutlines(false, floor.level);
						DeviceStorage.EnableColliders(true, floor.level);
					}
				}
				InterfacePlayer();
				baker.Bake();
				navBehavior.AddAgentComponent();
				sunLight.intensity = 0.2f;
				TimeClock.PassTime = true;
			}
			else {
				NavigationBehavior.Clear();
				editor.HideLevels();
				editor.CheckWallsHidden();
				foreach (FloorGrid floor in floors)
				{
					if (floor)
					{
						floor.InteractableUnder(false);
						floor.DesuniteMesh();
						floor.EnableFloorCollider(false);
						floor.EnableFeaturesCollider(false);
						floor.EnableObjectsCollider(false);
						floor.SwitchColliders(false);
						floor.WallsCollide = false;
						floor.SwitchLights(false);
						floor.Refresh();
						EnableReflectionProbes(floor, false);
						DeviceStorage.SetOutlines(false, floor.level);
						DeviceStorage.EnableColliders(false, floor.level);
					}
				}
				editor.currentFloor.WallsCollide = true;
				InterfaceOverview();
				DeviceStorage.SetOutlines(true, editor.currentFloor.level);
				DeviceStorage.EnableColliders(true, editor.currentFloor.level);
				sunLight.intensity = 1f;
				TimeClock.PassTime = false;
			}
		}
	}

	/// <summary>
	/// Active ou desactive les orbes de reflexion, attachés à certains objets comme les miroirs.
	/// Ces orbes demandent beaucoup de ressources, ils ne sont activés qu'en vue à la 1ere personne.
	/// </summary>
	void EnableReflectionProbes(FloorGrid floor, bool enable)
	{
		ReflectionProbe[] probes;
		probes = floor.GetComponentsInChildren<ReflectionProbe>();
		if (enable)
		{
			foreach (ReflectionProbe probe in probes)
			{
				probe.enabled = true;
				probe.RenderProbe();
			}
		}
		else
		{
			foreach (ReflectionProbe probe in probes)
			{
				probe.enabled = false;
			}
		}
	}
}
