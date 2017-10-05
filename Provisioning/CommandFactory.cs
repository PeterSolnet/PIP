using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Commands;
using Provisioning.Commands.CommandInterfaces;
using Provisioning.Configuration;
using Provisioning.Extensions;
using Provisioning.Receivers;


namespace Provisioning
{
    public class CommandFactory
    {
        private Dictionary<string, ReceiverBase> receiverCache;
        private string id;
        //private string channel;

        //Removed channel from parameters
        public CommandFactory(string id)
        {
            this.receiverCache = new Dictionary<string, ReceiverBase>();
            this.id = id;
           // this.channel = channel;
        }

        /// <summary>
        /// Returns the <see cref="ReceiverBase"/> instance specified by <paramref name="name"/>, and initialized with <paramref name="constructorParams"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="constructorParams"></param>
        /// <returns></returns>
        private ReceiverBase GetReceiver(string name, IRequestInfo info, object[] constructorParams)
        {
            ReceiverBase receiver;
            if (!receiverCache.TryGetValue(name, out receiver))
            {
                receiver = (ReceiverBase)Assembly.GetAssembly(typeof(ReceiverBase)).CreateInstance(name, false, BindingFlags.CreateInstance, null, constructorParams, null, null);
                if (receiver != null)
                {
                    receiver.RequestInfo = info;
                    receiver.RequestId = id;
                    //receiver.Channel = channel;
                    receiverCache[name] = receiver;
                }
            }

            return receiver;
        }

        /// <summary>
        /// Returns the <see cref="ReceiverBase"/> instance specified by <paramref name="info"/>, and initialized with <paramref name="constructorParams"/>.
        /// </summary>
        /// <param name="info">
        /// A <see cref="CommandInfo"/> instance, representing a command to be executed as part of a provisioning request.
        /// </param>
        /// <param name="constructorParams">Constructor parameters used to initialize the receiver.</param>
        private ReceiverBase GetReceiver(ICommandInfo info, params object[] constructorParams)
        {
            var receiverName = string.IsNullOrEmpty(info.DefaultReceiver) ? info.Parent.DefaultReceiver : info.DefaultReceiver;
            receiverName = string.IsNullOrEmpty(receiverName) ? info.Parent.Parent.DefaultReceiver : receiverName;
            if (string.IsNullOrEmpty(receiverName))
            {
                throw new ApplicationException("Could not load receiver.");
            }

            return GetReceiver(receiverName, info.Parent, constructorParams);
        }

        /// <summary>
        /// Returns the <see cref="ReceiverBase"/> instance specified by <paramref name="info"/>, and initialized with <paramref name="constructorParams"/>.
        /// </summary>
        /// <param name="info">A <see cref="RequestInfo"/> instance, representing a product configuration.</param>
        /// <param name="constructorParams">Constructor parameters used to initialize the receiver.</param>
        private ReceiverBase GetReceiver(IRequestInfo info, params object[] constructorParams)
        {
            var receiverName = string.IsNullOrEmpty(info.DefaultReceiver) ? info.Parent.DefaultReceiver : info.DefaultReceiver;
            if (string.IsNullOrEmpty(receiverName))
            {
                throw new ApplicationException("Could not load receiver.");
            }

            return GetReceiver(receiverName, info, constructorParams);
        }

        /// <summary>
        /// Loads the specified command (using reflection) and instantiates it, initializing it with the supplied <paramref name="constructorParams"/>.
        /// </summary>
        /// <param name="name">The fully-qualified name of the command to be loaded.</param>
        /// <param name="constructorParams">Constructor parameters used to initialize the command.</param>
        private ICommand CreateCommandInstance(string name, object[] constructorParams)
        {
            var command = (ICommand)Assembly.GetAssembly(typeof(ICommand)).CreateInstance(name, false, BindingFlags.CreateInstance, null, constructorParams, null, null);
            if (command == null)
            {
                throw new ApplicationException(String.Format(@"Command ""{0}"" could not be found.", name));
            }
            return command;
        }

        /// <summary>
        /// Returns the <see cref="ICommand"/> instance specified by <paramref name="info"/>.
        /// 
        /// This is used by old-style product configurations that specify a single command in <see cref="RequestInfo"/>.
        /// </summary>
        /// <param name="info">A <see cref="RequestInfo"/> instance, representing a product configuration.</param>
        /// <param name="msisdn">The MSISDN of the subscriber making the request.</param>
        /// <param name="message">The code requested by the current subscriber.</param>
        /// <returns></returns>
        public ICommand GetCommand(IRequestInfo info, string msisdn, string message)
        {
            var receiver = GetReceiver(info, msisdn, message, info.ExternalData1);
            var commandName = (string.IsNullOrEmpty(info.CommandName) ? "NoCommand" : info.CommandName);
            var commandNamespace = (info.UseDefaultNameSpace ? info.Parent.DefaultCommandNamespace : string.Empty);
            if (!string.IsNullOrEmpty(commandNamespace))
                commandName = String.Format("{0}.{1}", commandNamespace, commandName);

            return CreateCommandInstance(commandName, new object[] { receiver, info.Extra });
        }

        /// <summary>
        /// Returns the <see cref="ICommand"/> instance specified by <paramref name="info"/>, and initialized with <paramref name="constructorParams"/>.
        /// </summary>
        /// <param name="info">
        /// A <see cref="CommandInfo"/> instance, representing a command to be executed as part of a provisioning request.
        /// </param>
        /// <param name="defaultNamespace">The default namespace where commands are looked up.</param>
        /// <param name="constructorParams">Constructor parameters used to initialize the command.</param>
        /// <returns></returns>
        public ICommand GetCommand(ICommandInfo info, string defaultNamespace, params object[] constructorParams)
        {
            var commandName = (string.IsNullOrEmpty(info.CommandName) ? "NoCommand" : info.CommandName);
            var commandNamespace = (info.UseDefaultNameSpace ? defaultNamespace : string.Empty);
            if (!string.IsNullOrEmpty(commandNamespace))
                commandName = String.Format("{0}.{1}", commandNamespace, commandName);

            var command = CreateCommandInstance(commandName, constructorParams);

            if (!string.IsNullOrEmpty(info.TestCommand) && (info.Cases.Count() > 0))
            {
                // We have a `SwitchCommand` -- set its `TestCommand`:
                commandName = String.Format("{0}.{1}", commandNamespace, info.TestCommand);
                ((SwitchCommand)command).TestCommand = CreateCommandInstance(commandName, constructorParams);
            }

            return command;
        }

        /// <summary>
        /// Returns the <see cref="ICommand"/> instance specified by <paramref name="cmdInfo"/>.
        /// </summary>
        /// <param name="cmdInfo">A <see cref="CommandInfo"/> instance, representing the command to be returned.</param>
        /// <param name="msisdn">The MSISDN of the subscriber making the request.</param>
        /// <param name="message">The code requested by the current subscriber.</param>
        /// <returns></returns>
        public ICommand GetCommand(CommandInfo cmdInfo, string userId, string message)
        {
            var info = cmdInfo.Parent;
            var defaultNamespace = info.Parent.DefaultCommandNamespace;
            var receiver = GetReceiver(cmdInfo, userId, message, info.ExternalData1);
            var command = GetCommand(cmdInfo, defaultNamespace, receiver, cmdInfo.Extra);

            // If this command is marked as a checkpoint, we'll log to the escalation log if it fails:
            command.CheckpointName = cmdInfo.CheckpointName;
            // If this command returns any data, it'll be made available in the context under the key(s) specified by `item.Returns`:
            command.Returns = cmdInfo.Returns;

            // Test if we have a complex command; if so, we need to properly initialize it:
            //
            // 1. SwitchCommand:
            //
            if (!string.IsNullOrEmpty(cmdInfo.TestCommand) && (cmdInfo.Cases.Count() > 0))
            {
                // Ensure we have no more than one default case:
                if (cmdInfo.Cases.Count(x => x.IsDefault) > 1)
                    throw new ConfigurationErrorsException("You can't have more than one default case in a SwitchCommand.");

                var switchCommand = command as SwitchCommand;

                // The SwitchCommand itself doesn't return any data, so use its key for the TestCommand:
                switchCommand.TestCommand.Returns = switchCommand.Returns;
                switchCommand.Returns = null;
                switchCommand.Cases = new List<SwitchCase>();

                foreach (CaseInfo cs in cmdInfo.Cases)
                {
                    var thisCase = new SwitchCase();
                    thisCase.Match = (cs.Match.IsExpression() ? (ICaseExpression)new CaseExpression2()
                        : (ICaseExpression)new CaseExpression());
                    thisCase.Match.Expression = cs.Match;
                    thisCase.Commands = new List<ICommand>();

                    foreach (CommandInfo subCmd in cs.Commands)
                    {
                        subCmd.Parent = info;
                        var subCommand = GetCommand(subCmd, userId, message);
                        thisCase.Commands.Add(subCommand);
                    }

                    if (cs.IsDefault)
                    {
                        switchCommand.DefaultCase = thisCase;
                    }
                    else
                    {
                        switchCommand.Cases.Add(thisCase);
                    }
                }
            }
            //
            // 2. UpdateAccountCommand:
            //
            //else if (cmdInfo.Accounts.Count() > 0)
            //{
            //    if (cmdInfo.Accounts.Count(x => x.MainAccount) > 1)
            //        throw new ConfigurationErrorsException(string.Format("You can't have more than one entry for MainAccount ({0}).", info.Name));

            //    var updateAccountCommand = command as UpdateAccountCommand;
            //    updateAccountCommand.Details = new List<UpdateDetails>();

            //    foreach (AccountInfo ac in cmdInfo.Accounts)
            //    {
            //        var details = new UpdateDetails();
            //        details.AccountType = (ac.MainAccount ? AccountType.MainAccount : AccountType.DedicatedAccount);
            //        details.AccountId = ac.AccountId;

            //        if (ac.Validity != null) details.Validity = ac.Validity;
            //        if (ac.Extend != null) details.Extend = ac.Extend;
            //        if (ac.Prorate != null) details.Prorate = ac.Prorate;
            //        if (ac.AddValue != null)
            //        {
            //            details.UpdateType = UpdateType.Add;
            //            details.Value = ac.AddValue;
            //        }
            //        else if (ac.DeductValue != null)
            //        {
            //            details.UpdateType = UpdateType.Deduct;
            //            details.Value = ac.DeductValue;
            //        }
            //        else
            //        {
            //            details.UpdateType = UpdateType.Set;
            //            details.Value = ac.NewValue;
            //        }

            //        if (details.AccountType == AccountType.MainAccount && details.UpdateType == UpdateType.Set)
            //            throw new ConfigurationErrorsException(string.Format("'NewValue' is not supported for the Main Account ({0}).", info.Name));

            //        if (details.Value == null)
            //            throw new ConfigurationErrorsException(string.Format("You must specify a value with one of 'AddValue', 'DeductValue' or 'NewValue' ({0}).", info.Name));

            //        if (details.AccountType == AccountType.MainAccount && details.Validity != null)
            //            throw new ConfigurationErrorsException(string.Format("You can't specify 'Validity' for adjustments to the Main Account ({0}).", info.Name));

            //        if ((details.Prorate != null && details.Validity != null) || (details.Prorate != null && details.Extend != null))
            //            throw new ConfigurationErrorsException(string.Format("You can't specify 'Prorate' with either of 'Validity' or 'Extend' ({0}).", info.Name));

            //        updateAccountCommand.Details.Add(details);
            //    }
            //}
            //
            // 3. SendMailCommand:
            //
            else if (cmdInfo.Body != null)
            {
                if (cmdInfo.Subject == null)
                    throw new ConfigurationErrorsException(string.Format("You need to provide an email Subject ({0}).", info.Name));

                SendMailCommand mailCommand;

                try
                {
                    mailCommand = (SendMailCommand)command;
                    mailCommand.Subject = cmdInfo.Subject ?? string.Empty;
                    mailCommand.Body = cmdInfo.Body ?? string.Empty;
                }
                catch (InvalidCastException exc)
                {
                    var errorMessage =
                        String.Format("Expected: SendMailCommand; Got: {0} instead ({1}).", command.GetType().Name,
                            info.Name);
                    throw new ConfigurationErrorsException(errorMessage, exc);
                }
            }
            //
            // 4. SqlQueryCommand:
            //
            else if (cmdInfo.QueryManager != null)
            {
                SqlQueryCommand queryCommand;
                try
                {
                    queryCommand = (SqlQueryCommand)command;
                    queryCommand.IsSelectStatement = cmdInfo.IsSelectStatement;
                    queryCommand.QueryManager = cmdInfo.QueryManager;
                }
                catch (InvalidCastException exc)
                {
                    var errorMessage =
                        String.Format("Expected: SqlQueryCommand; Got: {0} instead ({1}).", command.GetType().Name,
                            info.Name);
                    throw new ConfigurationErrorsException(errorMessage, exc);
                }
            }
            //
            // 5. QueryCommand:
            //
            else if (cmdInfo.Columns != null && cmdInfo.Columns.Count() > 0)
            {
                var queryCommand = command as QueryCommand;
                queryCommand.Details = new List<ColumnDetails>();

                foreach (var cdDetails in cmdInfo.Columns)
                {
                    var details = new ColumnDetails();
                    if (cdDetails.DbParameterType != null)
                    {
                        details.DbParameterType = cdDetails.DbParameterType.ToString();
                    }
                    details.IsKey = cdDetails.IsKey;
                    details.Name = cdDetails.Name.ToString();
                    if (cdDetails.ParameterName != null)
                    {
                        details.ParameterName = cdDetails.ParameterName.ToString();
                    }
                    if (cdDetails.QueryOperator != null)
                    {
                        if (cdDetails.QueryOperator != "")
                        {
                            details.QueryOperator = cdDetails.QueryOperator.ToString();
                        }
                    }
                    if (cdDetails.Value != null)
                    {
                        details.Value = cdDetails.Value.ToString();
                    }
                    queryCommand.Details.Add(details);
                }
            }
            //
            // 6. AccountQueryBalance
            //
            //else if (cmdInfo.Balances != null && cmdInfo.Balances.Count() > 0)
            //{
            //    var accountBalanceQueryCommand = command as AccountBalanceQueryCommand;
            //    accountBalanceQueryCommand.Details = new List<BalanceDetails>();

            //    foreach (var cdDetails in cmdInfo.Balances)
            //    {
            //        var details = new BalanceDetails();
            //        details.AccountIds = cdDetails.AccountIds;
            //        details.DateFormatter = cdDetails.DateFormatter;
            //        details.Divisor = cdDetails.Divisor;
            //        details.Name = cdDetails.Name;

            //        accountBalanceQueryCommand.Details.Add(details);
            //    }
            //}
            return command;
        }
    }
    
}
