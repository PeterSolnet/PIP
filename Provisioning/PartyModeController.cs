using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Provisioning.Commands;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Commands.Model;
using Provisioning.Expressions;

namespace Provisioning
{
    public class PartyModeController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PartyModeController));

        private List<ICommand> commands; // Commands to be executed.

        public List<ICommand> Commands
        {
            get
            {
                return commands;
            }
        }

        public PartyModeController()
        {
            commands = new List<ICommand>();
        }

        public CommandStatus Execute(ExpressionContext context)
        {
            CommandStatus status;
            string statusMessage = string.Empty;

            foreach (ICommand item in commands)
            {
                status = item.Execute(context);

                try
                {
                    switch (status.StatusCode)
                    {
                        case CommandCode.Ok:
                            // Successful; add returned data to context:
                            context.AddToGlobals(item.Returns, status.Data);
                            if (!string.IsNullOrEmpty(status.Description))
                            {
                                // Save the description, so we can return it when we're done:
                                statusMessage = status.Description;
                            }
                            log.InfoFormat("Command {0} completed.", item.GetType().Name);
                            continue;
                        case CommandCode.Exit:
                            // Early termination; return status as-is:
                            log.InfoFormat("Command {0} exited.", item.GetType().Name);
                            return status;
                        default:
                            // Failed; halt processing, rollback and exit:
                            log.InfoFormat("Command {0} failed.", item.GetType().Name);
                            return status;
                    }
                }
                catch (Exception exc)
                {
                    if (!string.IsNullOrEmpty(item.CheckpointName))
                    {
                        // Save checkpoint so we know to escalate later:
                        exc.Data["CheckpointName"] = item.CheckpointName;
                    }
                    throw;
                }
            }

            return new CommandStatus { StatusCode = CommandCode.Ok, Description = statusMessage };
        }

       

        public void SetCommand(ICommand commandIn)
        {
            commands.Add(commandIn);
        }

        public void SetCommand(List<ICommand> commands)
        {
            this.commands = commands;
        }
    }
}
