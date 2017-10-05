using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Provisioning.Commands;
using Provisioning.Commands.Model;
using Provisioning.Configuration;
using Provisioning.Expressions;
using System.Configuration;
using SmartFormat;

namespace Provisioning
{
    public class ResponseHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ResponseHandler));
        private string defaultErrorMessage;

        public ResponseHandler(string defaultErrorMessage)
        {
            this.defaultErrorMessage = defaultErrorMessage;
        }

        /// <summary>
        /// Gets the response returned to the subscriber.
        /// </summary>
        /// <param name="info">
        /// The configuration info for this plan, including configured messages.
        /// </param>
        /// <param name="status">The provisioning status.</param>
        /// <param name="context">
        /// The current execution context, used to resolve "{...}" message placeholders.
        /// </param>
        /// <returns>A formatted message</returns>
        public string GetResponse(IRequestInfo info, CommandStatus status, IExpressionContext context)
        {
            if (info == null)
            {
                // Early bailout...naught else we can do:
                return Smart.Format(status.Description ?? defaultErrorMessage, context.AsDictionary());
            }

            var message = status.Description;
            if (string.IsNullOrEmpty(message))
            {
                context.AddToGlobals("status", status.StatusCode);
                context.AddToGlobals("errorCode", status.ErrorCode);

                var response = info.Responses.FirstOrDefault(x => x.When.Evaluate(context));
                if (response == null)
                {
                    // Fall back to old-school message attributes:
                    switch (status.StatusCode)
                    {
                        case CommandCode.Ok:
                            message = info.SuccessMessage;
                            break;
                        case CommandCode.InsufficientBalance:
                            message = info.InsufficientFundMessage;
                            break;
                        case CommandCode.NotWhitelisted:
                        case CommandCode.RuleViolation:
                            message = info.BlacklistMessage;
                            break;
                        case CommandCode.Exit:
                            message = info.WelcomeBackMessage;
                            break;
                        default:
                            message = (string.IsNullOrEmpty(info.ErrorMessage) ?
                                defaultErrorMessage : info.ErrorMessage);
                            break;
                    }
                }
                else
                {
                    message = response.Message;
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                // Possibly the user forgot to configure a message required by the product.
                // Raise an error, so it gets resolved quickly (hopefully, before it gets to the customer):
                log.ErrorFormat("No response found for status: {0}", status);
                throw new ConfigurationErrorsException(String.Format("No response found for: {0}.", status));
            }

            return Smart.Format(message, context.AsDictionary());
        }
    }
}
