using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2.WebApi.Models;
using log4net;
using log4net.Config;
using Provisioning.Expressions;
using Provisioning.Receivers;
using Provisioning.Services;

namespace Provisioning.Commands.PIP
{
    public class CreateConceptDocumentCommand:CommandBase
    {
        public CreateConceptDocumentCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        protected static readonly ILog log = LogManager.GetLogger(typeof(CreateConceptDocumentCommand));
        ProvisionService productService=new ProvisionService();
        
        public override CommandStatus Execute(ExpressionContext context)
        {
           
            CommandStatus result = new CommandStatus();
            
            var args = GetArguments(context);
           
            try
            {
                var documentTypeId = (long)args[0];
                var documentName = args[1].ToString();
                DocumentInfo documentInfo = new DocumentInfo
                {
                    DocumentTypeId = documentTypeId,
                    DocumentName = documentName
                    
                };

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
           
        }
    }
}
