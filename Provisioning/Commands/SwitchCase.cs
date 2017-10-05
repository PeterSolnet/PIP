using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Expressions;

namespace Provisioning.Commands
{
    public class SwitchCase
    {
        public ICaseExpression Match { get; set; }
        public List<ICommand> Commands { get; set; }

        public bool IsMatch(ExpressionContext context)
        {
            return Match.Evaluate(context);
        }
    }
}
