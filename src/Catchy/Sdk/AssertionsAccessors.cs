namespace Catchy.Sdk
{
    public static class AssertionsAccessors
    {
        public static AssertionPipeline GetPipeline(this ValueAssertions a) => a._pipeline;

        public static TValue GetValue<TValue>(this ValueAssertions<TValue> a)
            => a.GetValue();
        public static Task<TValue?> GetValueAsync<TValue>(this ValueAssertions<TValue> a)
        {
            if (a._asyncProvider is not null) return a._asyncProvider();
            return Task.FromResult((TValue?)a.GetValue());
        }

        public static TValue GetValue<TSelf, TValue>(this ValueAssertions<TSelf, TValue> a)
            where TSelf : ValueAssertions<TSelf, TValue>
            => a.GetValue();
        public static Task<TValue?> GetValueAsync<TSelf, TValue>(this ValueAssertions<TSelf, TValue> a)
            where TSelf : ValueAssertions<TSelf, TValue>
        {
            if (a._asyncProvider is not null) return a._asyncProvider();
            return Task.FromResult((TValue?)a.GetValue());
        }

        public static TAssert Op<TAssert>(this TAssert a, CheckOperation op)
            where TAssert : IAssertions
        { ((IAssertions)a).AddOp(op); return a; }

        public static ValueAssertions<T> Op<T>(this ValueAssertions<T> a, Func<ValueAssertions<T>, CheckOperation> opFactory)
        {
            if (a is Catchy.IQuantifiedCapture<T> quantified)
            {
                quantified.AddFactory(opFactory);
                return a;
            }

            ((IAssertions)a).AddOp(opFactory(a));
            return a;
        }

        public static bool IsSkipped<TAssert>(this TAssert a)
            where TAssert : IAssertions
            => ((IAssertions)a).IsSkipped();

        public static TAssert Link<TAssert>(this TAssert a, string method, params string?[] exprs)
            where TAssert : IAssertions
        {
            var parts = new List<string> { $".{method}(" };
            bool first = true;
            foreach (var e in exprs)
            {
                if (string.IsNullOrEmpty(e)) continue;
                if (!first) parts.Add(", ");
                parts.Add(e!);
                first = false;
            }
            parts.Add(")");
            ((IAssertions)a).AddLinks([.. parts]);
            return a;
        }

        public static TAssert Link<TAssert>(this TAssert a, string method, Type type, params string?[] exprs)
            where TAssert : IAssertions
        {
            var parts = new List<string>
            {
                $".{method}<",
                TypeHelper.FriendlyName(type),
                ">",
                "("
            };
            bool first = true;
            foreach (var e in exprs)
            {
                if (string.IsNullOrEmpty(e)) continue;
                if (!first) parts.Add(", ");
                parts.Add(e!);
                first = false;
            }
            parts.Add(")");
            ((IAssertions)a).AddLinks([.. parts]);
            return a;
        }

        public static TAssert Link<TAssert>(this TAssert a, string method, IEnumerable<Type> types, params string?[] exprs)
            where TAssert : IAssertions
        {
            var parts = new List<string> { $".{method}<" };
            bool first = true;
            foreach (var type in types)
            {
                string typeName = TypeHelper.FriendlyName(type);
                if (string.IsNullOrEmpty(typeName)) continue;
                if (!first) parts.Add(", ");
                parts.Add(typeName);
                first = false;
            }
            parts.Add(">(");
            first = true;
            foreach (var e in exprs)
            {
                if (string.IsNullOrEmpty(e)) continue;
                if (!first) parts.Add(", ");
                parts.Add(e!);
                first = false;
            }
            parts.Add(")");
            ((IAssertions)a).AddLinks([.. parts]);
            return a;
        }

        public static TAssert Skip<TAssert>(this TAssert a, string? reason = null)
    where TAssert : IAssertions
        {
            ((IAssertions)a).Skip(reason);
            return a;
        }

        /// <summary>
        /// Creates a projected assertion context for a sub-value.
        /// Prefer <see cref="DelegateTo{TSelf, TValue}(ValueAssertions{TSelf}, Func{TSelf, TValue}, Action{ValueAssertions{TValue}}, string?, string?[])"/>
        /// for custom DSL methods, so internal delegated links do not leak into the outer chain.
        /// </summary>
        [global::System.Diagnostics.DebuggerHidden, global::System.Diagnostics.StackTraceHidden]
        public static ValueAssertions<TValue> For<TSelf, TValue>(
            this ValueAssertions<TSelf> a,
            Func<TSelf, TValue> select,
            string? propertyName = null)
            where TSelf : notnull
        {
            if (propertyName is not null)
                a.Link(propertyName);
            return new ValueAssertions<TValue>(a.GetPipeline(), select(a.GetValue())!);
        }

        /// <summary>
        /// Delegates to assertions for a projected sub-value while keeping the outer chain clean.
        /// Any links produced by delegated assertions are scoped and removed after capture.
        /// </summary>
        /// <remarks>
        /// This enables composition of existing assertion methods without polluting the public chain.
        /// Example visible chain: <c>Check.That(product).HasValidSku()</c>
        /// while delegated checks can internally call <c>StartsWith</c>, <c>IsNotEmpty</c>, etc.
        /// </remarks>
        [global::System.Diagnostics.DebuggerHidden, global::System.Diagnostics.StackTraceHidden]
        public static ValueAssertions<TSelf> DelegateTo<TSelf, TValue>(
            this ValueAssertions<TSelf> a,
            Func<TSelf, TValue> select,
            Action<ValueAssertions<TValue>> delegated,
            string? outerMethod = null,
            params string?[] outerExprs)
            where TSelf : notnull
        {
            if (outerMethod is not null)
                a.Link(outerMethod, outerExprs);

            var pipeline = a.GetPipeline();
            var linkCountBefore = pipeline.Links.Count;

            try
            {
                delegated(new ValueAssertions<TValue>(pipeline, select(a.GetValue())!));
            }
            finally
            {
                if (pipeline.Links.Count > linkCountBefore)
                    pipeline.Links.RemoveRange(linkCountBefore, pipeline.Links.Count - linkCountBefore);
            }

            return a;
        }
    }
}
