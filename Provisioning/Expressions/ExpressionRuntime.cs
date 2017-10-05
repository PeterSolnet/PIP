using System;
using System.Linq.Expressions;
using Irony.Interpreter;
using Irony.Parsing;
using Provisioning.Commands.Model;

namespace Provisioning.Expressions
{
    public class ExpressionRuntime : LanguageRuntime
    {
        public ExpressionRuntime(LanguageData language) : base(language) { }

        public override void Init()
        {
            base.Init();

            BuiltIns.ImportStaticMembers(typeof(System.Convert));
            BuiltIns.ImportStaticMembers(typeof(System.Math));
            BuiltIns.ImportStaticMembers(typeof(System.DateTime));
            BuiltIns.ImportStaticMembers(typeof(Environment));
            BuiltIns.ImportStaticMembers(typeof(CommandCode));
            BuiltIns.ImportStaticMembers(typeof(System.DayOfWeek));
        }

        protected override void InitOperatorImplementations()
        {
            base.InitOperatorImplementations();

            AddConverter(typeof(ClrMethodBindingTargetInfo), typeof(CommandCode),
                new UnaryOperatorMethod(v => Enum.Parse(typeof(Enum), ((ClrMethodBindingTargetInfo)v).Symbol, false)));

            AddBinary(ExpressionType.Power, typeof(System.Int32), new BinaryOperatorMethod((a, b) => Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b))));
            AddBinary(ExpressionType.Power, typeof(System.Int64), new BinaryOperatorMethod((a, b) => Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b))));
            AddBinary(ExpressionType.Equal, typeof(System.Int32), new BinaryOperatorMethod((a, b) => Int32.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(System.Int32), new BinaryOperatorMethod((a, b) => !object.Equals(a, b)));
            AddBinary(ExpressionType.Equal, typeof(System.Int64), new BinaryOperatorMethod((a, b) => Int64.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(System.Int64), new BinaryOperatorMethod((a, b) => !object.Equals(a, b)));
            AddBinary(ExpressionType.Equal, typeof(System.Boolean), new BinaryOperatorMethod((a, b) => object.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(System.Boolean), new BinaryOperatorMethod((a, b) => !object.Equals(a, b)));
            AddBinary(ExpressionType.Equal, typeof(System.String), new BinaryOperatorMethod((a, b) => string.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(System.String), new BinaryOperatorMethod((a, b) => !string.Equals(a, b)));
            AddBinary(ExpressionType.Equal, typeof(DayOfWeek), new BinaryOperatorMethod((a, b) => Enum.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(DayOfWeek), new BinaryOperatorMethod((a, b) => !Enum.Equals(a, b)));
            AddBinary(ExpressionType.Equal, typeof(CommandCode), new BinaryOperatorMethod((a, b) => Enum.Equals(a, b)));
            AddBinary(ExpressionType.NotEqual, typeof(CommandCode), new BinaryOperatorMethod((a, b) => !Enum.Equals(a, b)));
            //AddBinary(ExpressionType.Equal, typeof(DataSubscription), new BinaryOperatorMethod((a, b) => object.Equals(a, b)));
            //AddBinary(ExpressionType.NotEqual, typeof(DataSubscription), new BinaryOperatorMethod((a, b) => !object.Equals(a, b)));

            AddBinary(ExpressionType.Subtract, typeof(System.DateTime),
                new BinaryOperatorMethod((a, b) => (DateTime.Parse(a.ToString()).Subtract(DateTime.Parse(b.ToString())))));
        }

        public override void CreateBinaryOperatorImplementationsForMismatchedTypes()
        {
            base.CreateBinaryOperatorImplementationsForMismatchedTypes();

            UnaryOperatorMethod converter;
            OperatorImplementation impl;

            // Integer and Double.

            converter = GetConverter(typeof(Int32), typeof(Double));

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Power, typeof(Int32), typeof(Double)),
                typeof(Double), new BinaryOperatorMethod((a, b) => Math.Pow(Convert.ToDouble(a), (double)b)),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Power, typeof(Double), typeof(Int32)),
                typeof(Double), new BinaryOperatorMethod((a, b) => Math.Pow((double)a, Convert.ToDouble(b))),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            // Integer and Decimal arithmetic.

            converter = GetConverter(typeof(Int32), typeof(Decimal));

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.AddChecked, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) + (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.AddChecked, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a + Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.SubtractChecked, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) - (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.SubtractChecked, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a - Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.MultiplyChecked, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) * (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.MultiplyChecked, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a * Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Divide, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) - (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Divide, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a / Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            // Integer and Decimal comparisons.

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.GreaterThan, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) > (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.GreaterThan, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a > Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.LessThan, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) < (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.LessThan, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a < Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.GreaterThanOrEqual, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) >= (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.GreaterThanOrEqual, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a >= Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.LessThanOrEqual, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) <= (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.LessThanOrEqual, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a <= Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Equal, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) == (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Equal, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a == Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.NotEqual, typeof(Int32), typeof(Decimal)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => Convert.ToDecimal(a) != (decimal)b),
                converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.NotEqual, typeof(Decimal), typeof(Int32)),
                typeof(Decimal), new BinaryOperatorMethod((a, b) => (decimal)a != Convert.ToDecimal(b)),
                null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            // CommandCode Enum comparisons.

            converter = GetConverter(typeof(ClrMethodBindingTargetInfo), typeof(CommandCode));

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Equal, typeof(CommandCode), typeof(ClrMethodBindingTargetInfo)),
                typeof(CommandCode),
                new BinaryOperatorMethod((a, b) => Enum.Equals(a, b)), null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.NotEqual, typeof(CommandCode), typeof(ClrMethodBindingTargetInfo)),
                typeof(CommandCode),
                new BinaryOperatorMethod((a, b) => !Enum.Equals(a, b)), null, converter, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.Equal, typeof(ClrMethodBindingTargetInfo), typeof(CommandCode)),
                typeof(CommandCode),
                new BinaryOperatorMethod((a, b) => Enum.Equals(a, b)), converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            impl = new OperatorImplementation(
                new OperatorDispatchKey(ExpressionType.NotEqual, typeof(ClrMethodBindingTargetInfo), typeof(CommandCode)),
                typeof(CommandCode),
                new BinaryOperatorMethod((a, b) => !Enum.Equals(a, b)), converter, null, null);
            OperatorImplementations[impl.Key] = impl;

            // DataSubscription comparisons.

            //converter = GetConverter(typeof(DataSubscription), typeof(NoneClass));

            //impl = new OperatorImplementation(
            //    new OperatorDispatchKey(ExpressionType.Equal, typeof(DataSubscription), typeof(NoneClass)),
            //    typeof(DataSubscription),
            //    new BinaryOperatorMethod((a, b) => object.Equals(a, b)), null, converter, null);
            //OperatorImplementations[impl.Key] = impl;

            //impl = new OperatorImplementation(
            //    new OperatorDispatchKey(ExpressionType.NotEqual, typeof(DataSubscription), typeof(NoneClass)),
            //    typeof(DataSubscription),
            //    new BinaryOperatorMethod((a, b) => !object.Equals(a, b)), null, converter, null);
            //OperatorImplementations[impl.Key] = impl;

            //impl = new OperatorImplementation(
            //    new OperatorDispatchKey(ExpressionType.Equal, typeof(NoneClass), typeof(DataSubscription)),
            //    typeof(DataSubscription),
            //    new BinaryOperatorMethod((a, b) => object.Equals(a, b)), converter, null, null);
            //OperatorImplementations[impl.Key] = impl;

            //impl = new OperatorImplementation(
            //    new OperatorDispatchKey(ExpressionType.NotEqual, typeof(NoneClass), typeof(DataSubscription)),
            //    typeof(DataSubscription),
            //    new BinaryOperatorMethod((a, b) => !object.Equals(a, b)), converter, null, null);
            //OperatorImplementations[impl.Key] = impl;

        }
    }
    
}