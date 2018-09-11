namespace Sledge.Common.Shell.Commands
{
    /// <summary>
    /// A message holding a command and any parameters it might have.
    /// </summary>
    public class CommandMessage
    {
        /// <summary>
        /// The ID of the command
        /// </summary>
        public string CommandID { get; }

        /// <summary>
        /// The parameters of the command
        /// </summary>
        public CommandParameters Parameters { get; }

        /// <summary>
        /// Set to true to mark this command as intercepted, the command will not be processed any further.
        /// </summary>
        public bool Intercepted { get; set; }

        /// <summary>
        /// Construct a new command message
        /// </summary>
        /// <param name="commandID">The command ID</param>
        /// <param name="parameters">The command parameters</param>
        public CommandMessage(string commandID, object parameters = null)
        {
            CommandID = commandID;
            Parameters = new CommandParameters();

            if (parameters == null) return;

            foreach (var prop in parameters.GetType().GetProperties())
            {
                var name = prop.Name;
                var val = prop.GetValue(parameters);
                Parameters.Add(name, val);
            }
        }
    }
}