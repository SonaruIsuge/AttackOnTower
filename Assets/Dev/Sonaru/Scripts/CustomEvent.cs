namespace Dev.Sonaru
{
    public abstract class CustomEvent { }


    public class OnGridDataChanged<TGridData> : CustomEvent
    {
        public Grid<TGridData> grid;
        public int xIndex;
        public int yIndex;
        public TGridData data;

        public OnGridDataChanged(Grid<TGridData> grid, int xIndex, int yIndex, TGridData data)
        {
            this.grid = grid;
            this.xIndex = xIndex;
            this.yIndex = yIndex;
            this.data = data;
        }
    }
}