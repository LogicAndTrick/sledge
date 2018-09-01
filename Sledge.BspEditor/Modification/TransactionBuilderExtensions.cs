using System.Collections.Generic;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification
{
    public static class TransactionBuilderExtensions
    {
        /// <summary>
        /// Adds an operation to the transaction that will attach the given children to the first item in the query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="children"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static MapObjectQuery Attach(this MapObjectQuery query, IEnumerable<IMapObject> children, Transaction transaction)
        {
            if (query.Any()) transaction.Add(new Attach(query[0].ID, children));
            return query;
        }

        /// <summary>
        /// Adds an operation to the transaction that will attach the context to the selected parent
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parent"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static MapObjectQuery AttachTo(this MapObjectQuery query, IMapObject parent, Transaction transaction)
        {
            transaction.Add(new Attach(parent.ID, query));
            return query;
        }
    }
}