using System;
using K2.WebApi.Controllers;
using K2.WebApi.Models;
using log4net;
using log4net.Config;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Receivers;

namespace Provisioning.Commands.PIP
{
    class AddDocumentCommand:CommandBase
    {
        readonly DocumentInfosApiController _documentInfosApi=new DocumentInfosApiController();
        protected static readonly ILog log = LogManager.GetLogger(typeof(AddDocumentCommand));
        public AddDocumentCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        public override CommandStatus Execute(ExpressionContext context)
        {
            CommandStatus result;

            var args = GetArguments(context);

            try
            {
                var documentTypeId = (long)args[0];
                var documentName = args[1].ToString();
                var documentPath = args[2].ToString();
                var tag = args[3].ToString();
                var requestId = args[4].ToString();
                var documentInfo = new DocumentInfo
                {
                    DocumentName = documentName,
                    DocumentTypeId = documentTypeId,
                    DocumentPath = documentPath,
                    Tag = tag,
                    RequestId = requestId
                };
               
                _documentInfosApi.PostDocumentInfo(documentInfo).RunSynchronously();
                result = new CommandStatus {Data = documentInfo,StatusCode = CommandCode.Ok};
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error occured in AddDocumentCommand " + ex);
                result = new CommandStatus { StatusCode = CommandCode.Failed };
                return result;
            }
        }
    }
}
