using System.Collections.Generic;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    public interface IGrid
    {
        /// <summary>
        /// A relative value to represent the spacing of a grid.
        /// Must support values 1-10 as logical minimums and maximums,
        /// but other values are allowed as well.
        /// </summary>
        int Spacing { get; set; }

        /// <summary>
        /// Snaps the given coordinate to the closest grid point
        /// </summary>
        /// <param name="coordinate">The coordinate to snap</param>
        /// <returns>A snapped coordinate</returns>
        Coordinate Snap(Coordinate coordinate);

        /// <summary>
        /// Add a single step to a given coordinate. The coordinate is not rounded first.
        /// </summary>
        /// <param name="coordinate">The coordinate to add to</param>
        /// <param name="add">The relative number of steps to add to the coordinate</param>
        /// <returns>The new coordinate with the steps added</returns>
        Coordinate AddStep(Coordinate coordinate, Coordinate add);

        /// <summary>
        /// Get the current grid lines for a viewport.
        /// The lines do not have to exactly line up with a viewport, it is just a general guideline.
        /// </summary>
        /// <param name="normal">The normal to the plane, should be UnitX, UnitY, or UnitZ</param>
        /// <param name="scale">The scale of the viewport</param>
        /// <param name="worldMinimum">The start of the viewport in world coordinates</param>
        /// <param name="worldMaximum">The end of the viewport in world coordinates</param>
        /// <returns>The list of lines that are currently visible in the viewport</returns>
        IEnumerable<GridLine> GetLines(Coordinate normal, decimal scale, Coordinate worldMinimum, Coordinate worldMaximum);
    }
}