namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// The grid line type. The integer value of this enum approximates the desired z-index of the grid line.
    /// </summary>
    public enum GridLineType
    {
        Fractional = -1,
        Standard = 0,
        Primary = 1,
        Secondary = 2,
        Axis = 3,
        Boundary = 4,
    }
}