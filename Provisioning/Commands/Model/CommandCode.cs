using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Commands.Model
{
    public enum CommandCode

    {
        /// <summary>
        /// Everything's peachy...really.
        /// </summary>
        Ok,


        /// <summary>
        /// Provisioning failed, due to some undetermined error.
        /// </summary>
        Failed,


        /// <summary>
        /// The subscriber is not on one or more whitelists.
        /// </summary>
        NotWhitelisted,


        /// <summary>
        /// The subscriber has insufficient value in a relevant account.
        /// </summary>
        InsufficientBalance,


        /// <summary>
        /// The subscriber requested a code that has doesn't match a configured plan.
        /// </summary>
        RouteNotFound,


        /// <summary>
        /// Provisioning was terminated because a business rule was violated.
        /// Additional information about the rule violation may be present in the "ErrorCode" property.
        /// </summary>
        RuleViolation,


        /// <summary>
        /// Provisioning was terminated due to a network error.
        /// </summary>
        NetworkError,


        /// <summary>
        /// Provisioning was terminated due to a database error.
        /// </summary>
        DatabaseError,


        /// <summary>
        /// Provisioning was terminated due to an AIR server failure.
        /// </summary>
        AirFailure,


        /// <summary>
        /// Provisioning was terminated due to a failure on TABS.
        /// </summary>
        TabsFailure,


        /// <summary>
        /// Provisioning was terminated due to an error on an external node.
        /// </summary>
        ExternalNodeError,


        /// <summary>
        /// Processing was terminated and no action was taken.
        /// </summary>
        Exit,


        /// <summary>
        /// Provisioning was terminated because of a configuration error.
        /// </summary>
        ConfigurationError,


        /// <summary>
        /// Provisioning was terminated because no MSISDN was received.
        /// </summary>
        MissingMsisdn,

        /// <summary>
        /// Provisioning was terminated because no UserId was received.
        /// </summary>
        MissingUserId,


        /// <summary>
        /// Provisioning was terminated because no code was received.
        /// </summary>
        MissingMessage
    }
}