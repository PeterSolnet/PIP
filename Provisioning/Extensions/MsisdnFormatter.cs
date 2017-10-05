using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Extensions
{
    public enum MsisdnFormats
    {
        /// <summary>
        /// International format, prefixed with a (+) e.g: +2348022229385
        /// </summary>
        PlusInternational,

        /// <summary>
        /// International format, without the leading (+) e.g: 2348022229385
        /// </summary>
        International,

        /// <summary>
        /// Without the country prefix, including the leading 0, e.g: 08022229385
        /// </summary>
        Friendly,

        /// <summary>
        /// Without the country prefix or the leading 0, e.g: 8022229385
        /// </summary>
        Internal
    }

    public abstract class MsisdnFormatter
    {
        public abstract string Format(string msisdn);

        public static MsisdnFormatter GetFormat(MsisdnFormats format)
        {
            switch (format)
            {
                case MsisdnFormats.PlusInternational:
                    return new PlusInternationalFormatter();

                case MsisdnFormats.International:
                    return new InternationalFormatter();

                case MsisdnFormats.Friendly:
                    return new FriendlyFormatter();

                case MsisdnFormats.Internal:
                    return new InternalFormatter();

                default:
                    return new FriendlyFormatter();

            }
        }
    }

    public class PlusInternationalFormatter : MsisdnFormatter
    {
        public override string Format(string msisdn)
        {
            return string.Format("+234{0}", msisdn.Right(10));
        }
    }

    public class InternationalFormatter : MsisdnFormatter
    {
        public override string Format(string msisdn)
        {
            return String.Format("234{0}", msisdn.Right(10));
        }
    }

    public class FriendlyFormatter : MsisdnFormatter
    {
        public override string Format(string msisdn)
        {
            return String.Format("0{0}", msisdn.Right(10));//internal format
        }
    }

    public class InternalFormatter : MsisdnFormatter
    {
        public override string Format(string msisdn)
        {
            return msisdn.Right(10);
        }
    }
}
