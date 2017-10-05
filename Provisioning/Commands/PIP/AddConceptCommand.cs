using System;
using System.Linq;
using K2.WebApi.Controllers;
using K2.WebApi.Models;
using log4net;
using log4net.Config;
using Provisioning.Commands.Model;
using Provisioning.Receivers;
using Provisioning.Services;

namespace Provisioning.Commands.PIP
{
    class AddConceptCommand : CommandBase
    {
        public AddConceptCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {
            XmlConfigurator.Configure();
        }
        protected static readonly ILog log = LogManager.GetLogger(typeof(AddConceptCommand));
         ConceptInfosApiController _conceptInfosApi=new ConceptInfosApiController();
        //ProductService productService = new ProductService();
        ProvisionService provisionService = new ProvisionService();
        public override CommandStatus Execute(Expressions.ExpressionContext context)
        {
            CommandStatus result;

            var args = GetArguments(context);
           
            try
            {
               
                var userId = args[0].ToString();
                
              
                var creationDate = DateTime.Now;
                bool isNewConcept = Convert.ToBoolean(args[1].ToString());
                var conceptName = args[2].ToString();
                var description = args[3].ToString();
                string conceptOwner = args[4].ToString();
                string folio = args[5].ToString();
                string tag = args[6].ToString();
                string stakeHolders = args[7].ToString();
               
                ConceptInfo conceptInfo=new ConceptInfo
                {
                    OriginatorUserName = userId,
                    RequestId = folio,
                    CreationDate = creationDate,
                    IsNewConcept = isNewConcept,
                    ConceptOwner = conceptOwner,
                    ConceptName = conceptName,
                    CurrentActivityState = "Concept Created",
                    ProductDescription = description,
                    Tag = tag,
                    //OriginatorName = adUser.FirstName + " " + adUser.LastName
                };


                _conceptInfosApi.PostConceptInfo(conceptInfo);
                result = new CommandStatus { Data = conceptInfo,StatusCode = CommandCode.Ok };
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