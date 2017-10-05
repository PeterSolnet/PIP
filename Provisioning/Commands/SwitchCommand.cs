using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Commands.Model;
using Provisioning.Receivers;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Exceptions;

namespace Provisioning.Commands
{
    public class SwitchCommand : CommandBase
    {
        public ICommand TestCommand { get; set; }
        public List<SwitchCase> Cases { get; set; }
        public SwitchCase DefaultCase { get; set; }

        public SwitchCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        { }

        public override CommandStatus Execute(Provisioning.Expressions.ExpressionContext context)
        {
            var status = TestCommand.Execute(context);
            if (status.StatusCode != CommandCode.Ok)
            {
                return status;
            }
            else
            {
                // Initialize our command list with the DefaultCommand, if present:
                var commands = (DefaultCase != null ? DefaultCase.Commands : new List<ICommand>());
                var controller = new PartyModeController();

                // Check if `TestCommand` returned any data in `status.Data`; 
                // if it did, add it to the context so it'll be available for matching:
                if (status.Data != null)
                    context.AddToGlobals(TestCommand.Returns, status.Data);

                // Attempt to match it against our list of cases:
                SwitchCase matchedCase = null;
                var matchedCases = Cases.Where(x => x.IsMatch(context));
                if (matchedCases.Count() > 1)
                {
                    throw new MultipleCaseMatchException("{0}",
                        matchedCases.Select(x => x.Match.Expression).ToArray());
                }
                else
                {
                    matchedCase = matchedCases.SingleOrDefault();
                    commands = (matchedCase != null ? matchedCase.Commands : commands);
                }

                if (commands.Count == 0)
                {
                    // If no cases were matched, fall through (return status "Ok").
                    // This is the expected behavior for a conditional command (otherwise
                    // subsequent commands would NOT get executed):
                    return new CommandStatus { StatusCode = CommandCode.Ok };
                }
                else
                {
                    var ctx = String.Format(":: Case({0})",
                        matchedCase == null ? "Default" : matchedCase.Match.Expression);
                    using (log4net.ThreadContext.Stacks["NDC"].Push(ctx))
                    {
                        controller.SetCommand(commands);
                        return controller.Execute(context);
                    }
                }
            }
        }
    }
}
