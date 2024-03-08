namespace Dev.Sonaru
{
    public class PathGridTile : IGridTile
    {
        private Grid<PathGridTile> grid;
        
        public int XIndex { get; private set; }
        public int YIndex { get; private set; }
        public bool Walkable { get; private set; }
        
        public int gCost;
        public int hCost;
        public int fCost;

        public PathGridTile cameFromNode;

        public PathGridTile(Grid<PathGridTile> grid, int xIndex, int yIndex)
        {
            this.grid = grid;
            this.XIndex = xIndex;
            this.YIndex = yIndex;
            Walkable = true;
            
            gCost = 0;
            hCost = 0;
            fCost = 0;
            cameFromNode = null;
        }


        public void SetWalkable(bool walkable)
        {
            if(this.Walkable == walkable)
                return;
            
            this.Walkable = walkable;
            EventManager.RaiseEvent(new OnGridChangeWalkable(this, walkable));
        }


        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}