using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Provisioning.Commands.Model;
using Provisioning.Receivers;
using SmartFormat;

namespace Provisioning.Commands
{
    public class SendMailCommand : CommandBase
    {
        static readonly ILog log = LogManager.GetLogger(typeof(SendMailCommand));

        public string Subject { get; set; }
        public string Body { get; set; }

        public SendMailCommand(IReceiver receiver, string extra)
            : base(receiver, extra)
        {; }

        public override  CommandStatus Execute(Expressions.ExpressionContext context)
        {
            var recipients = GetArguments(context); // Expected: single or multiple comma-separated recipients
            var contextDictionary = context.AsDictionary();

            MailMessage message = new MailMessage();
            message.Subject = Smart.Format(Subject, contextDictionary);
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = Smart.Format(Body, contextDictionary);
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;

            foreach (string address in recipients)
            {
                message.To.Add(address.Trim());
            }

            bool queued = ThreadPool.QueueUserWorkItem(m =>
            {
                try
                {
                    var smtpClient = new SmtpClient();
                    var emailMessage = (MailMessage)m;
                    smtpClient.Send(emailMessage);
                    SendMailCommand.log.InfoFormat(@"Email: ""{0}"" sent to: {1}.",
                        emailMessage.Subject, emailMessage.To);
                }
                catch (SmtpException exc)
                {
                    SendMailCommand.log.Error(string.Format("Error while sending email: {0}", exc.Message), exc);
                }
            }, message);

            return new CommandStatus { StatusCode = (queued ? CommandCode.Ok : CommandCode.Failed) };
        }
    }
}