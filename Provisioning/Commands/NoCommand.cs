using System.Threading.Tasks;
using Provisioning.Commands.Model;
using Provisioning.Receivers;

namespace Provisioning.Commands
{
    /// <summary>
    /// A command that does...nothing :)
    /// </summary>
    public class NoCommand : CommandBase

    {
        public NoCommand(IReceiver receiver, string extra)
            : base(receiver, extra)

        {
        }


        public override  CommandStatus Execute(Provisioning.Expressions.ExpressionContext context)

        {
            return  new CommandStatus {StatusCode = CommandCode.Ok};
        }
    }
}