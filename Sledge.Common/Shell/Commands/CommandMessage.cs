namespace Sledge.Common.Shell.Commands
{
    public class CommandMessage
    {
        public string CommandID { get; }
        public CommandParameters Parameters { get; }
        public bool Intercepted { get; set; }

        public CommandMessage(string commandID, object parameters = null)
        {
            CommandID = commandID;
            Parameters = new CommandParameters();

            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var name = prop.Name;
                    var val = prop.GetValue(parameters);
                    Parameters.Add(name, val);
                }
            }
        }
    }
}