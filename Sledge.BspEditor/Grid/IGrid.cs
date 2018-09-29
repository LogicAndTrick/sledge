using System.Collections.Generic;
using System.Numerics;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// A grid is used for guidelines and vertex snapping.
    /// 
    /// The grid interface should represent both a list of lines
    /// to render on the screen, as well as a snapping rule set
    /// to use during user interaction.
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        /// A relative value to represent the spacing of a grid.
        /// Must support values 1-10 as logical minimums and maximums,
        /// but other values are allowed as well.
        /// </summary>
        int Spacing { get; set; }

        /// <summary>
        /// A description of the current grid's status
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Snaps the given vector to the closest grid point
        /// </summary>
        /// <param name="vector">The vector to snap</param>
        /// <returns>A snapped vector</returns>
        Vector3 Snap(Vector3 vector);

        /// <summary>
        /// Add a single step to a given vector. The vector is not rounded first.
        /// </summary>
        /// <param name="vector">The vector to add to</param>
        /// <param name="add">The relative number of steps to add to the vector</param>
        /// <returns>The new vector with the steps added</returns>
        Vector3 AddStep(Vector3 vector, Vector3 add);

        /// <summary>
        /// Get the current grid lines for a viewport.
        /// The lines do not have to exactly line up with a viewport, it is just a general guideline.
        /// </summary>
        /// <param name="normal">The normal to the plane, should be UnitX, UnitY, or UnitZ</param>
        /// <param name="scale">The scale of the viewport</param>
        /// <param name="worldMinimum">The start of the viewport in world vectors</param>
        /// <param name="worldMaximum">The end of the viewport in world vectors</param>
        /// <returns>The list of lines that are currently visible in the viewport</returns>
        IEnumerable<GridLine> GetLines(Vector3 normal, float scale, Vector3 worldMinimum, Vector3 worldMaximum);
    }
}