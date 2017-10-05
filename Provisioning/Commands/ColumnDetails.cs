using System;

namespace Provisioning.Commands
{
    public class ColumnDetails
    {
        private string queryOperator = "=";
        private string dbParameterType = null;

        public virtual String Name { get; set; }
        public virtual String Value { get; set; }
        public virtual Boolean IsKey { get; set; }
        public virtual String ParameterName { get; set; }
        public virtual String QueryOperator
        {
            get
            {
                return queryOperator;
            }
            set
            {
                queryOperator = value;
            }
        }
        public virtual string DbParameterType
        {
            get
            {
                return dbParameterType;
            }
            set
            {
                dbParameterType = value;
            }
        }
    }
}