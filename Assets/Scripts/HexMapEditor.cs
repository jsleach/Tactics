using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private HexCell selectedCell;

	private Canvas uiCanvas;

	private Text selectedCellCoordsText;

	private Dropdown selectedColorDropdown;

	private Slider selectedElevationSlider;

	private ToggleGroup selectedSlopeDirection;
	private Slider selectedSlopeValue;

	//private int currentSlope;
	//private SlopeDirection slopeDirection;

	void Awake () {
		this.uiCanvas = this.GetComponent<Canvas> ();
		this.selectedCellCoordsText = this.transform.Find ("Selected Cell Panel/Selected Cell Group/Selected Cell Coords").GetComponent<Text>();
		this.selectedColorDropdown = this.transform.Find ("Selected Cell Panel/Color Panel/Color Dropdown").GetComponent<Dropdown> ();
		this.selectedElevationSlider = this.transform.Find ("Selected Cell Panel/Elevation Panel/Elevation Slider").GetComponent<Slider> ();
		this.selectedSlopeDirection = this.transform.Find ("Selected Cell Panel/Slope Panel").GetComponent<ToggleGroup> ();
		this.selectedSlopeValue = this.transform.Find ("Selected Cell Panel/Slope Panel/Slope Slider").GetComponent<Slider> ();
	}

	void Update () {
		if (
			Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject()
		) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (inputRay, out hit)) {
			this.SelectCell (hexGrid.CellFromPosition (hit.point));
			if (!uiCanvas.enabled) {
				uiCanvas.enabled = true;
			}
		}else {
			this.selectedCell = null;
			this.selectedCellCoordsText.text = "Coords: none";
			if (uiCanvas.enabled) {
				uiCanvas.enabled = false;
			}
		}
	}

	// turn the selected cell into the selected color
	public void SelectColor (int index) {
		if (selectedCell != null) {
			selectedCell.color = colors [index];
			selectedCell.colorName = (CellColor)index;
			hexGrid.Refresh ();
		}
	}

	// update the selected cell's elvation
	public void SelectElevation (float elevation) {
		if (selectedCell != null) {
			selectedCell.Elevation = (int)elevation;
			hexGrid.Refresh ();
		}
	}

	// update the selected cell's slope direction
	public void SelectSlopeDirection(int direction) {
		this.SelectSlope ((SlopeDirection)direction, (int)selectedSlopeValue.value);
	}

	// update the selected cell's slope elevation
	public void SelectSlopeElevation(float elevation) {
		int slopeDirection = this.selectedSlopeDirection.gameObject.transform.Cast<Transform> ().ToList ().ConvertAll<GameObject> (t => t.gameObject).IndexOf (this.selectedSlopeDirection.ActiveToggles ().FirstOrDefault ().gameObject);
		this.SelectSlope ((SlopeDirection)slopeDirection, (int)elevation);
	}

	// update the selected cell's slope
	private void SelectSlope(SlopeDirection direction, int elevation) {
		if (selectedCell != null) {
			selectedCell.SetSlope (direction, elevation);
			hexGrid.Refresh ();
		}
	}

	// update the select cell's color dropdown to represent the color it is when selected
	private void SetColor(int color) {
		selectedColorDropdown.value = color;
	}

	// update the select cell's elevation slider to represent the elevation of it when selected
	private void SetElevation(int elevation) {
		selectedElevationSlider.value = elevation;
	}

	// update the selected cell's slope  when selected
	private void SetSlope(SlopeDirection direction, int elevation) {
		this.selectedSlopeDirection.gameObject.transform.Cast<Transform> ().ToList ().ConvertAll<GameObject> (t => t.gameObject).ElementAt ((int)direction).GetComponent<Toggle> ().isOn = true;
		this.selectedSlopeValue.value = elevation;
	}

	// set the selected cell
	private void SelectCell(HexCell select) {
		this.selectedCell = select;

		// if there is no cell to select, hide the selected cell UI element, otherwise show it
		if (select != null) {
			this.selectedCellCoordsText.text = "Coords: " + select.coordinates.ToString ();
			this.SetColor ((int)selectedCell.colorName);
			this.SetElevation (selectedCell.Elevation);
			this.SetSlope (selectedCell.slopeDirection, selectedCell.slopeElevation);
		}
	}
}