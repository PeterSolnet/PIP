using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Expressions;

namespace Provisioning.Commands.Invoker
{
    public class Invoker
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Invoker));
        public void  ExecuteCommand(ICommand command)
        {
            XmlConfigurator.Configure();
            command.Execute();
            
        }
    }
}
