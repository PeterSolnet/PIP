using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Receivers;

namespace Provisioning.Commands
{
    class HelloWorldCommand : CommandBase
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(HelloWorldCommand));
        public HelloWorldCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        public override  CommandStatus Execute(ExpressionContext context)
        {
            
           
            var args = GetArguments(context);
            string openStatus = args[0].ToString();
           
            try
            {
               
                //log.Info($"Found : {openBatch.Result.BatchStatus}: {openBatch.Result.BatchNo}: {openBatch.Result.Id}");
                return new CommandStatus { Data = {}, StatusCode = CommandCode.Ok };
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0}", ex));
                return new CommandStatus { StatusCode = CommandCode.Failed };
            }
        }
    }
}
