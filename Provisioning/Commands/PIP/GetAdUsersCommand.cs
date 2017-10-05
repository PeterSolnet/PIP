using System;
using K2.WebApi.Models;
using log4net;
using log4net.Config;
using Provisioning.Commands.Model;
using Provisioning.Receivers;
using Provisioning.Services;

namespace Provisioning.Commands.PIP
{
    class GetAdUsersCommand : CommandBase
    {
        public GetAdUsersCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        protected static readonly ILog log = LogManager.GetLogger(typeof(GetAdUsersCommand));
        //ConceptInfosApiController conceptInfosApi=new ConceptInfosApiController();
        ProductService productService = new ProductService();
        public override CommandStatus Execute(Expressions.ExpressionContext context)
        {
            CommandStatus result;

            var args = GetArguments(context);
           
            try
            {

                var theUsers = new object();
                var allUsers = CachePool.GetAllAdUserEntries(theUsers,)
                //result = new CommandStatus { Data = conceptInfo,StatusCode = CommandCode.Ok };
                //productService.CreateNewDocument()
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error occured in AddConceptCommand "+ex);
                result = new CommandStatus {StatusCode = CommandCode.Failed};
                return result;
            }
        }
    }
}