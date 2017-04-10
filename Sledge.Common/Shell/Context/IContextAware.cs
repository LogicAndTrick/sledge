namespace Sledge.Common.Shell.Context
{
    public interface IContextAware
    {
        /// <summary>
        /// Test if this object is valid for the given context
        /// </summary>
        /// <param name="context">The context to test</param>
        /// <returns>True if this object is in context</returns>
        bool IsInContext(IContext context);
    }
}