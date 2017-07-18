public enum HexDirection {
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions { 
	public static HexDirection Opposite(this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}

public enum SlopeDirection {
	//0      1          2       3          4        5         6         7        8         9        10        11       12
	None, N_Corner, NE_Edge, NE_Corner, E_Edge, SE_Corner, SE_Edge, S_Corner, SW_Edge, SW_Corner, W_Edge, NW_Corner, NW_Edge
}