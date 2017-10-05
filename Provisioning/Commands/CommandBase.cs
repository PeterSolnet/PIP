using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Commands.Model;
using Provisioning.Expressions;
using Provisioning.Extensions;
using Provisioning.Receivers;

namespace Provisioning.Commands
{
    public abstract class CommandBase : ICommand
    {
        private ICommand _commandAction;

        static readonly Regex SplitArgumentsRegex = new Regex(@"((?:[^,{}""]|\{[^{}]*\}|""[^""]*"")+)",
            RegexOptions.Compiled);


        protected CommandStatus status;

        public string Extra { get; set; }

        public string CheckpointName { get; set; }

        public string Returns { get; set; }

        public CommandBase()

        {
            status = new CommandStatus {StatusCode = CommandCode.Ok, Description = string.Empty};
        }


        public CommandBase(IReceiver receiver, string extra) : this()

        {
            this.Extra = extra;
        }


        /// <summary>
        /// Convenience method to parse arguments supplied to a Command.
        /// </summary>
        /// <param name="context">The current context, used to resolve argument expressions</param>
        /// <param name="expected">The number of arguments expected after parsing</param>
        /// <returns></returns>
        public IList<object> GetArguments(ExpressionContext context, int? expected = null)

        {
            var parsed = new List<object>();

            var matches = SplitArgumentsRegex.Matches(this.Extra);


            foreach (Match match in matches)

            {
                var arg = match.Groups[0].Value;

                if (arg.IsExpression())

                {
                    // Evaluate expression and add it to argument list:

                    parsed.Add(arg.AsExpression().Evaluate(context));
                }

                else

                {
                    parsed.Add(arg);
                }
            }


            if (expected.HasValue && expected.Value != parsed.Count)

                throw new ArgumentException(string.Format("Argument count mismatch. Expected: {0}, Supplied: {1}.",
                    expected.Value, parsed.Count));


            return parsed;
        }

        /// <summary>
        /// Executes command logic; subclasses should override this method.
        /// </summary>
        /// <param name="context">
        ///     The current request execution context.
        /// </param>
        /// <returns></returns>
        public abstract CommandStatus Execute(ExpressionContext context);


        public virtual void Log()
        {
            ;
        }


        public virtual CommandStatus RollBack()

        {
            status.StatusCode = CommandCode.Ok;

            return status;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}