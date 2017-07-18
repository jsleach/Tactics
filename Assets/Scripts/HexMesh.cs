using UnityEngine;
using System.Collections.Generic;

/*
 * class for creating the visible mesh for the hex grid
 */
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	List<Vector3> vertices;
	List<Color> colors;
	List<int> triangles;

	MeshCollider meshCollider;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		colors = new List<Color>();
		triangles = new List<int>();
	}

	// creates all the cells in the hex mesh via 6 triangles each
	public void Triangulate (HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}

	// creates the part of the hex mesh for a single hexagon
	void Triangulate (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate (d, cell);
		}
	}

	void Triangulate(HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		Vector3 v1 = center + HexMetrics.GetFirstCorner (direction);
		Vector3 v2 = center + HexMetrics.GetSecondCorner (direction);

		center.y += ((float)cell.slopeElevation / 2) * HexMetrics.elevationStep;
		v1.y += cell.GetCornerElevation (direction) * HexMetrics.elevationStep;
		v2.y += cell.GetCornerElevation ((int)direction < 5 ? direction + 1 : direction - 5) * HexMetrics.elevationStep;

		AddTriangle (center, v1, v2);
		AddTriangleColor(cell.color);

		// add cliffs if needed
		if (cell.GetNeighbor (direction) != null) {
			Vector3 nCenter = cell.GetNeighbor(direction).transform.localPosition;
			Vector3 nV1 = nCenter + HexMetrics.GetFirstCorner (direction.Opposite ());
			Vector3 nV2 = nCenter + HexMetrics.GetSecondCorner (direction.Opposite ());

			if (v1.y > nV2.y || v2.y > nV1.y) {
				addQuad (v1, v2, nV1, nV2);
				addQuadColor (cell.color);
			}
		}
	}

	// draws a triangle out of the given vertex points
	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	// draws a quad(two triangles) out of the given vertex points
	void addQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector4 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add (v1);
		vertices.Add (v2);
		vertices.Add (v3);
		vertices.Add (v4);
		triangles.Add (vertexIndex);
		triangles.Add (vertexIndex + 3);
		triangles.Add (vertexIndex + 2);
		triangles.Add (vertexIndex + 2);
		triangles.Add (vertexIndex + 1);
		triangles.Add (vertexIndex);
	}
	void addQuadColor(Color color) {
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
	}

	// adds a color for a triangle
	void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
}