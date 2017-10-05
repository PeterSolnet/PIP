using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter;
using Irony.Parsing;

namespace Provisioning.Expressions
{
    public class ExpressionContext : IExpressionContext

    {

        private Grammar grammar;

        //private Parser parser;

        private LanguageData language;

        private LanguageRuntime runtime;

        private ScriptApp app;



        //public IDictionary<string, object> Globals

        //{

        //    get { return App.Globals; }

        //}



        public ExpressionContext()

            : this(new Grammar())

        {; }



        public ExpressionContext(Grammar grammar)

        {

            this.grammar = grammar;

            this.language = new LanguageData(grammar);

            //this.parser = new Parser(language);

            this.runtime = grammar.CreateRuntime(language);

            this.app = new ScriptApp(runtime);



            app.Globals.Add("null", runtime.NoneValue);

            app.Globals.Add("true", true);

            app.Globals.Add("false", false);

        }



        /// <summary>

        /// Provides dictionary-like access to context variables.

        /// </summary>

        /// <param name="key"></param>

        /// <returns></returns>

        public object this[string key]

        {

            get

            {

                return app.Globals[key];

            }

            set

            {

                app.Globals[key] = value;

            }

        }



        /// <summary>

        /// Add the returned data to this context under the specified keys.

        /// </summary>

        /// <param name="returnedKeys"></param>

        /// <param name="returnedData"></param>

        public void AddToGlobals(string returnedKeys, object returnedData)

        {

            if (!string.IsNullOrEmpty(returnedKeys) && returnedData != null)

            {

                var keys = returnedKeys.Split(',').Select(x => x.Trim()).ToArray();

                var numKeys = keys.Count();

                var data = returnedData as IEnumerable<object>;



                if (data == null)

                {

                    // Command returned a single item, so:

                    app.Globals.Add(keys[0], returnedData);

                }

                else

                {

                    // If a single key was specified, return the data as-is.

                    // Otherwise, attempt to "unpack" the values in the data (enumerable).

                    // This'll fail if we've got an unequal number of values and keys:

                    if (numKeys > 1 && numKeys != data.Count())

                        throw new Exception("Number of expected values does not equal number of actual values returned.");



                    if (numKeys == 1)

                    {

                        app.Globals.Add(keys[0], data);

                    }

                    else

                    {

                        // Unpack the returned values into the context:

                        foreach (var item in Enumerable.Zip(keys, data, (k, d) => new KeyValuePair<string, object>(k, d)))

                        {

                            app.Globals.Add(item);

                        }

                    }

                }

            }

        }



        /// <summary>

        /// Returns all defined variables in the current context as a Dictionary (required by Smart.Format).

        /// </summary>

        /// <returns></returns>

        public IDictionary<string, object> AsDictionary()

        {

            return new Dictionary<string, object>(app.Globals);

        }



        private object Unpack(object result)

        {

            if (result.GetType() == typeof(object[]) && ((object[])result).Length == 1)

                return ((object[])result)[0];

            else

                return result;

        }



        /// <summary>

        /// Evaluates the supplied <paramref name="script"/> in this context.

        /// </summary>

        /// <param name="script">A valid expression, to be evaluated.</param>

        /// <returns></returns>

        public object Evaluate(string script)

        {

            return Unpack(app.Evaluate(script));

        }



        #region Not Used Presently



        //public object Evaluate(ParseTree parsedScript)

        //{

        //    return Unpack(App.Evaluate(parsedScript));

        //}



        //public object Evaluate()

        //{

        //    return Unpack(App.Evaluate());

        //}



        //public void ClearOutput()

        //{

        //    App.ClearOutputBuffer();

        //}



        //public string GetOutput()

        //{

        //    return App.GetOutput();

        //}



        #endregion

    }
}
