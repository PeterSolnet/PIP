using System.Threading.Tasks;
using Provisioning.Expressions;

namespace Provisioning.Commands.CommandInterfaces
{
    public interface ICommand
    {
        string CheckpointName { get; set; }
        string Returns { get; set; }

        CommandStatus Execute(ExpressionContext context);
        void Log();
        CommandStatus RollBack();
    }
}
