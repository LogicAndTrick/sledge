namespace Sledge.Common.Shell.Commands
{
    /// <summary>
    /// A message holding a command and any parameters it might have.
    /// </summary>
    public class CommandMessage
    {
        public string CommandID { get; }
        public CommandParameters Parameters { get; }
        public bool Intercepted { get; set; }

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