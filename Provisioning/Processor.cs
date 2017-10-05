using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Provisioning.Commands;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Extensions;
using Provisioning.Configuration;
using Provisioning.Exceptions;

namespace Provisioning
{
    public class Processor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Processor));
        private static readonly log4net.ILog escalationLog = log4net.LogManager.GetLogger("EscalationLog");
        private static readonly log4net.ILog addonSubscriptionLog = log4net.LogManager.GetLogger("AddonSubscriptionLog");
        private static readonly MsisdnFormatter msisdnFormatter = MsisdnFormatter.GetFormat(MsisdnFormats.International);

        private ResponseHandler responseHandler;

        public Processor()
        {
            this.responseHandler = new ResponseHandler(ConfigurationManager.AppSettings["FailureMessage"]);
        }

        public static Processor Instance
        {
            get
            {
                return new Processor();
            }
        }

        /// <summary>
        /// Builds the expression context for the current request.
        /// </summary>
        /// <param name="info">The configuration info for the current plan</param>
        /// <param name="requestId">The unique identifier for this request</param>
        /// <param name="msisdn">The MSISDN of the subscriber making the request</param>
        /// <param name="message">The message sent by the subscriber</param>
        /// <param name="channel">Usually the IP address of the host sending the request</param>
        /// <returns>A new <see cref="ExpressionContext"/></returns>
        private ExpressionContext BuildContext(IRequestInfo info, string requestId, NameValueCollection args)
        {
            //var accountDetails = new Ucip(requestId).GetAccountDetails(args["msisdn"]);
            var serviceClass = 451;
            //var serviceOfferings = accountDetails.serviceOfferings.Select(x => x.serviceOfferingActiveFlag).ToArray();
            //var communityInformation = accountDetails.communityInformationCurrent;

            // Create a new context, and seed with initial variables:
            var context = new ExpressionContext();
            foreach (var key in args.AllKeys) // Add all GET, POST and Header parameters
                context[key] = args[key];

            context["id"] = requestId; // Add the current request identifier
            context["serviceClass"] = serviceClass; // The requestor's current Service Class
            //context["serviceOfferings"] = serviceOfferings;
            //context["communityInformation"] = communityInformation;
            context["now"] = DateTime.Now; // The Date/Time this request was made
            context["externalData1"] = info.ExternalData1; // Used to tag UCIP calls to the AIR servers
            context["whitelistKey"] = info.WhitelistKey; // The set of subscribers allowed access to this plan
            log.Info(String.Format(@"Loaded Service class {0} ", serviceClass));
            // Include any captured arguments from the current request:
            var regex = new Regex(info.MatchString, RegexOptions.ExplicitCapture);
            var m = regex.Match(args["message"]);
            foreach (var i in regex.GetGroupNumbers())
            {
                var name = regex.GroupNameFromNumber(i);
                if (name != i.ToString())
                {
                    context[name] = m.Groups[name].Value;
                }
            }

            return context;
        }

        /// <summary>
        /// Adds a few variables from the <see cref="ExpressionContext"/> to log4net's mapped diagnostic context.
        /// </summary>
        /// <param name="context">The current expression context.</param>
        private void PopulateLoggingContext(ExpressionContext context)
        {
            log4net.ThreadContext.Properties["userId"] = context["userId"];
            log4net.ThreadContext.Properties["message"] = context["message"];
            //log4net.ThreadContext.Properties["channel"] = context["channel"];
            log4net.ThreadContext.Properties["requestId"] = context["id"];
            log4net.ThreadContext.Properties["serviceClass"] = context["serviceClass"];
           
            log4net.ThreadContext.Properties["tag"] = context["externalData1"];
        }

        #region Deleted Methods
        ///// <summary>
        ///// Gets the response returned to the subscriber.
        ///// </summary>
        ///// <param name="info">
        ///// The configuration info for this plan, including configured messages.
        ///// </param>
        ///// <param name="status">The provisioning status.</param>
        ///// <param name="context">
        ///// The current execution context, used to resolve "{...}" message placeholders.
        ///// </param>
        ///// <returns>A formatted message</returns>
        
        #endregion

        /// <summary>
        /// Handles a provisioning request.
        /// </summary>
        /// <param name="msisdn">The MSISDN of the requesting subscriber.</param>
        /// <param name="message">A code representing the plan requested by the subscriber.</param>
        /// <param name="channel">(Optional) the channel the request was received from.</param>
        /// <returns>
        /// A <see cref="CommandStatus"/> representing the result of the provisioning request.
        /// </returns>
        public virtual CommandStatus Process(NameValueCollection args)
        {
            CommandStatus status;
            //userId
           // string msisdn = args["msisdn"] = msisdnFormatter.Format(args["msisdn"] ?? string.Empty);
            string userId = args["userId"] = args["userId"] ?? string.Empty;
            string message = args["message"] = args["msg"];
            //string channel = args["channel"] = (args["channel"] ?? args["X-Channel-IP"]);
            ExpressionContext context = new ExpressionContext();
            string requestId = CachePool.NextRequestId;
            RequestInfo info = null;

            DateTime started = DateTime.Now;
            Stopwatch stopWatch = new Stopwatch();

            // Add the Originator's UserId, requested plan, and requesting channel to the Diagnostic Context:
            var threadContext = String.Format("() {0} {1}: {2}", requestId, userId, message);
            using (log4net.ThreadContext.Stacks["NDC"].Push(threadContext))
            {
                log.Info("====> Begin processing request.");
                stopWatch.Start();

                if (userId.Length < 2) // No UserId should be shorter than 2 letters
                {
                    status = new CommandStatus { StatusCode = CommandCode.MissingUserId };
                }
                else if (string.IsNullOrEmpty(message))
                {
                    status = new CommandStatus { StatusCode = CommandCode.MissingMessage };
                }
                else
                {
                    try
                    {
                        info = CachePool.GetRequestInfo(message);
                        context = BuildContext(info, requestId, args);
                        PopulateLoggingContext(context);
                        status = ProcessRequest(info, context);
                        
                    }
                    catch (Exception ex)
                    {
                        // Do we need to escalate?
                        if (ex.Data.Contains("CheckpointName"))
                        {
                            escalationLog.FatalFormat("Date={0:yyyy-MM-dd'T'HH:mm:ss},{1},{2},{3},{4},{5}",
                                DateTime.Now, requestId, userId, message, ex.Data["CheckpointName"]);
                        }

                        status = new CommandStatus
                        {
                            StatusCode = CommandCode.Failed,
                            ErrorMessage = String.Format("{0}: {1}", ex.GetType().Name, ex.Message)
                        };

                        if (ex is RouteNotFound)
                        {
                            status.StatusCode = CommandCode.RouteNotFound;
                            status.Description = ex.Message;
                        }
                        else if (ex is ConfigurationErrorsException)
                        {
                            status.StatusCode = CommandCode.ConfigurationError;
                            status.Description = ex.Message;
                        }
                        
                        else if (ex is CustomException)
                        {
                            log.Error("Business rule violation:", ex);
                            status.StatusCode = CommandCode.RuleViolation;
                            status.Description = ex.Message;
                        }
                        //else if (ex is TabsException)
                        //{
                        //    log.Error("TABS service failure:", ex);
                        //    status.StatusCode = CommandCode.TabsFailure;
                        //    status.ErrorCode = ((TabsException)ex).Code.ToString();
                        //}
                        else if (ex is WebException)
                        {
                            var wx = (WebException)ex;
                            if (wx.Data.Contains("Url"))
                            {
                                var url = wx.Data["Url"];
                                log.Fatal(
                                    String.Format("Network error: {0} while connecting to: {1}.", wx.Status, url), wx);
                            }

                            status.StatusCode = CommandCode.NetworkError;
                            status.ErrorCode = String.Format("{0}", wx.Status);
                            status.ErrorLocation = (string)wx.Data["Location"];
                        }
                        else if (ex is DataException)
                        {
                            log.Error("Database error:", ex);
                            status.StatusCode = CommandCode.DatabaseError;
                            status.ErrorLocation = (string)ex.Data["Location"];
                        }
                        else
                        {
                            log.Error("Unknown error:", ex);
                        }
                    }
                }

                stopWatch.Stop();
                status.Description = responseHandler.GetResponse(info, status, context);
                status.Started = started;
                status.Finished = DateTime.Now;
                status.Duration = stopWatch.ElapsedMilliseconds / 1000m;
                status.Tag = (info == null ? null : info.ExternalData1);

                log.InfoFormat("Request status: {0}", status);
                log.InfoFormat("<==== Completed {0}.", (status.StatusCode == CommandCode.Ok || status.StatusCode == CommandCode.Exit ?
                    "successfully" : "with errors"));
                return status;
            }
        }


        private CommandStatus ProcessRequest(RequestInfo info, ExpressionContext context)
        {
            string userId = (string)context["userId"];
            string message = (string)context["message"];
            //string channel = (string)context["channel"];
            string requestId = (string)context["id"];
            int serviceClass = (int)context["serviceClass"];

            // Checks whether this plan specifies a whitelist, and if it does, whether the 
            // requesting subscriber's service class is included in the whitelist:
            if (!CachePool.IsAllowed(info.WhitelistKey, serviceClass))
            {
                return new CommandStatus { StatusCode = CommandCode.NotWhitelisted };
            }

            var controller = new PartyModeController();
            var commandFactory = new CommandFactory(requestId);

            if (!string.IsNullOrEmpty(info.CommandName)) // A single inline command is supported for backward compatibility.
            {
                var command = commandFactory.GetCommand(info, userId, message);
                //var receiver = commandFactory.GetReceiver(info, msisdn, message, info.externalData1);
                //var command = commandFactory.GetCommand(info, receiver, info.extra);
                controller.SetCommand(command);
            }
            else
            {
                foreach (CommandInfo item in info.CommandInfos)
                {
                    item.Parent = info;
                    var command = commandFactory.GetCommand(item, userId, message);
                    controller.SetCommand(command);
                }
            }

            if (controller.Commands.Count == 0)
            {
                throw new ConfigurationErrorsException(
                    string.Format("At least one command must be defined in RequestInfo [{0}].", info.Name));
            }

            var status = controller.Execute(context);
            if (status.StatusCode == CommandCode.Ok)
            {
                // Log CSV-style, so it's easy to import into our table later:
                var tag = Helpers.TagHelper.GetTag(info, context);
                addonSubscriptionLog.InfoFormat(@"{0},{1:yyyy-MM-dd'T'HH:mm:ss},{2},true,{3},{4},0,{5}",
                    userId, DateTime.Now, message, serviceClass, tag, requestId);
            }

            return status;
        }
    }
}
