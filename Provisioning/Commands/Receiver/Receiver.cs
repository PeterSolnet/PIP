using System;
using log4net;
using log4net.Config;
using Provisioning.Receivers;
using System.Configuration;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Configuration;

namespace Provisioning.Commands.Receiver
{
   public  class Receiver:ReceiverBase
    {
        //This class contains all actions to be performed
        protected static readonly ILog log = LogManager.GetLogger(typeof(Receiver));
       
       
        public Receiver()
       {
           XmlConfigurator.Configure();
       }

        public string Msisdn { get; set; }

        public string Message { get; set; }

        public string ExternalData1 { get; set; }

        public string Extra { get; set; }


        public Receiver(string msisdn, string message, string externalData1)

            : this()

        {

            //this.formatter = MsisdnFormatter.GetFormat(MsisdnFormats.International);

            //this.Msisdn = formatter.Format(msisdn);
            Msisdn = msisdn;
            Message = message;

            ExternalData1 = externalData1;

        }



        protected void OptOut(int destinationServiceClass)
        {
            if (ConfigurationManager.AppSettings["EnableOptout"] == "1")
            {
                OptOutSection config = OptOutSection.GetConfig();
                GenericElementCollection<CommandInfo> commands = null;
                int currentServiceClass = 451;

                OptOutInfo optOut = config.OptOutInfos[string.Format("{0},{1}", currentServiceClass, destinationServiceClass)];
                if (null != optOut)
                {
                    commands = optOut.CommandInfos;
                }

                //add wildcard e.g. *,327 i.e. all service classes going to 327 suffers this fate of cleanup
                optOut = config.OptOutInfos[String.Format("*,{0}", destinationServiceClass)];
                if (null != optOut) //wildcard configuration exist...
                {
                    commands = optOut.CommandInfos;
                }

                var controller = new PartyModeController();
                var commandFactory = new CommandFactory(RequestId);

                if (null != commands)
                {
                    Receiver receiver = new Receiver(Msisdn,
                        String.Format("{0} - {1}", currentServiceClass, destinationServiceClass), ExternalData1);
                    var context = new Provisioning.Expressions.ExpressionContext();
                    foreach (CommandInfo item in commands)
                    {
                        ICommand command = commandFactory.GetCommand(item, (string)config.DefaultCommandNamespace, receiver, item.Extra); // TODO: Test this properly!
                        controller.SetCommand(command);
                    }

                    controller.Execute(context);
                }
            }
        }

    }
}
