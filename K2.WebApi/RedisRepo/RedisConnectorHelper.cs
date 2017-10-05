using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange.Redis;

namespace K2.WebApi.RedisRepo
{
    public class RedisConnectorHelper
    {
        static RedisConnectorHelper()
        {
            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost"));

        }

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }  
    }
}