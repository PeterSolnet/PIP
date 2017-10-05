using System;
using K2.WebApi.Controllers;
using K2.WebApi.Models;
using log4net;
using log4net.Config;
using Provisioning.Commands.Model;
using Provisioning.Receivers;
using Provisioning.Services;

namespace Provisioning.Commands.PIP
{
    class AddStakeHolderCommand:CommandBase
    {
        public AddStakeHolderCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        protected static readonly ILog log = LogManager.GetLogger(typeof(AddStakeHolderCommand));
         StakeHoldersApiController stakeHoldersApi=new StakeHoldersApiController();
        //ProductService productService = new ProductService();
        ProvisionService provisionService = new ProvisionService();
        public override CommandStatus Execute(Expressions.ExpressionContext context)
        {
            CommandStatus result;

            var args = GetArguments(context);
           
            try
            {
               
                //var userId = args[0].ToString();
                var stakeHolders = args[0].ToString();
                var tag = args[1].ToString();
                var requestId = args[2].ToString();
                StakeHolder stakeHolder = new StakeHolder
                {
                    StakeHolderName = stakeHolders,
                    Tag = tag,
                    RequestId = requestId
                    
                };


                stakeHoldersApi.PostStakeHolder(stakeHolder);
                result = new CommandStatus { Data = stakeHolder, StatusCode = CommandCode.Ok };
                
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error occured in AddStakeHolderCommand " + ex);
                result = new CommandStatus {StatusCode = CommandCode.Failed};
               
                return result;
            }
        }
    }
}
