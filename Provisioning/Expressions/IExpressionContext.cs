using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Expressions
{
    public interface IExpressionContext

    {

        object this[string key] { get; set; }



        void AddToGlobals(string returnedKeys, object returnedData);

        IDictionary<string, object> AsDictionary();

        object Evaluate(string script);

    }
}
