using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// A factory that creates grid objects.
    /// </summary>
    public interface IGridFactory
    {
        /// <summary>
        /// The name of the grid
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A short explanation of the grid
        /// </summary>
        string Details { get; set; }

        /// <summary>
        /// An icon for this grid type
        /// </summary>
        Image Icon { get; }

        /// <summary>
        /// Create a grid for the provided environment
        /// </summary>
        /// <param name="environment">The environment to use</param>
        Task<IGrid> Create(IEnvironment environment);

        /// <summary>
        /// Test if a grid is an instance of this factory class
        /// </summary>
        /// <param name="grid">The grid to test</param>
        bool IsInstance(IGrid grid);
    }
}