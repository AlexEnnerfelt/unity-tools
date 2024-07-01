namespace UnpopularOpinion.Tools {
	public static class SortingUtility {
		public static int pixelsPerUnit = 200;
		public static int CalculateSortOrder(float YPos) {
			var eval = YPos * -pixelsPerUnit;
			return (int)eval;
		}
	}
}