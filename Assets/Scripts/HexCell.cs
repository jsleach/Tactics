using UnityEngine;
using System.Collections.Generic;

/*
 * Class for storing information about an individual cell
 */
public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public RectTransform uiRect;

	public Color color;
	public CellColor colorName;

	[SerializeField]
	private HexCell[] neighbors;

	public int Elevation {
		get {
			return elevation;
		}
		set {
			elevation = value;

			// move the cell vertically
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			transform.localPosition = position;

			// move the cell label to follow the cell
			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.elevationStep;
			uiRect.localPosition = uiPosition;
		}
	}
	private int elevation;

	public SlopeDirection slopeDirection { get; private set; }
	public int slopeElevation { get; private set; }
	private float[] cornerElevation = { 0, 0, 0, 0, 0, 0 };

	public HexCell GetNeighbor(HexDirection direction) {
		return neighbors [(int)direction];
	}

	public void SetNeighbor(HexDirection direction, HexCell cell) {
		neighbors [(int)direction] = cell;
		cell.neighbors [(int)direction.Opposite()] = this;
	}

	public float GetCornerElevation(HexDirection direction) {
		return cornerElevation [(int)direction];
	}

	private void SetCornerElevation(HexDirection direction, int elevation) {
		cornerElevation [(int)direction] = elevation;
	}

	public void SetSlope(SlopeDirection direction, int elevation) {
		// store the values to be read out by editor
		this.slopeDirection = direction;
		this.slopeElevation = elevation;

		// no slope
		if ((int)direction == 0) {
			for (int i = 0; i < 6; i++) {
				cornerElevation [i] = 0;
			}
		}

		// slope towards edge
		else if ((int)direction % 2 == 0) {
			// full elevation for the corners on either side of selected edge
			cornerElevation [((int)direction / 2) - 1 >= 0 ? ((int)direction / 2) - 1 : ((int)direction / 2) + 5] = elevation;
			cornerElevation [((int)direction / 2) <= 5 ? ((int)direction / 2) : 0] = elevation;

			// half elevation for corners in the middle of the cell from selected edge
			cornerElevation [((int)direction / 2) - 2 >= 0 ? ((int)direction / 2) - 2 : ((int)direction / 2) + 4] = (float)elevation / 2;
			cornerElevation [((int)direction / 2) + 1 <= 5 ? ((int)direction / 2) + 1 : ((int)direction / 2) - 5 ] = (float)elevation / 2;

			// no elevation for corners on the opposite side of the cell from the selected edge
			cornerElevation [((int)direction / 2) - 3 >= 0 ? ((int)direction / 2) - 3 : ((int)direction / 2) + 3] = 0;
			cornerElevation [((int)direction / 2) + 2 <= 5 ? ((int)direction / 2) + 2 : ((int)direction / 2) - 4] = 0;
		}

		// slope towards corner
		else if ((int)direction % 2 == 1){
			// full elevation for the selected corner
			cornerElevation[(int)direction / 2] = elevation;

			// ~2/3 elevation for two corners next to selected corner

			// ~1/3 elevation for the next two corners

			// no elevation for the corner opposite the selected corner
			//cornerElevation[((int)direction / 2) + 3 <= 5 ? ((int)direction / 2) + 3 : ((int)direction / 2) - 3] = 0;
		}
	}
}