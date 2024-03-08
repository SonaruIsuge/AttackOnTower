namespace Dev.Sonaru
{
    public interface IGridTile
    {
        int XIndex { get; }
        int YIndex { get; }
        bool Walkable { get; }

        void SetWalkable(bool walkable);
    }
}