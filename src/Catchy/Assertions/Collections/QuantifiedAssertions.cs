using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    internal interface IQuantifiedCapture<T>
    {
        void AddFactory(Func<ValueAssertions<T>, CheckOperation> factory);
    }

    /// <summary>
    /// Represents a quantified assertion scope that replays the collected chain per item.
    /// </summary>
    public class QuantifiedAssertions<T> : ValueAssertions<QuantifiedAssertions<T>, T>, Catchy.Sdk.IAssertions, IQuantifiedCapture<T>
    {
        private readonly IReadOnlyList<T> _values;
        private readonly QuantifiedMode _mode;
        private readonly List<Func<ValueAssertions<T>, CheckOperation>> _capturedFactories = [];
        private bool _quantifiedOpAdded;

        /// <summary>Initializes a quantified assertion scope for the provided values and mode.</summary>
        public QuantifiedAssertions(IEnumerable<T> values, AssertionPipeline pipeline, QuantifiedMode mode)
            : base(pipeline, default(T)!)
        {
            _values = values is IReadOnlyList<T> r ? r : [.. values];
            _mode = mode;
        }

        void Catchy.Sdk.IAssertions.AddOp(CheckOperation op)
            => CaptureFactory(_ => op);

        void IQuantifiedCapture<T>.AddFactory(Func<ValueAssertions<T>, CheckOperation> factory)
            => CaptureFactory(factory);

        private void CaptureFactory(Func<ValueAssertions<T>, CheckOperation> factory)
        {
            _capturedFactories.Add(factory);
            EnsureQuantifiedOpAdded();
        }

        private void EnsureQuantifiedOpAdded()
        {
            if (_quantifiedOpAdded)
            {
                return;
            }

            _quantifiedOpAdded = true;

            _pipeline.AddOp(QuantifiedModeChecks.Apply(
                _mode,
                _values,
                BuildPerItemChainOperation,
                _pipeline.IsSkipped,
                () => _pipeline.Settings.CollectionFailFast,
                () => _pipeline.Settings.SyncParallelThreshold));
        }

        private CheckOperation BuildPerItemChainOperation(T item)
        {
            string? failure = null;

            return CheckOperation.Async(async () =>
            {
                try
                {
                    await ExecuteReplayAsync(item).ConfigureAwait(false);
                    return true;
                }
                catch (AssertionException ex)
                {
                    failure = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    failure = ex.Message;
                    return false;
                }
            },
            () => failure ?? "Expected item to satisfy the condition",
            _pipeline.IsSkipped);
        }

        private async Task ExecuteReplayAsync(T item)
        {
            var replayPipeline = _pipeline.NewPipeline(true);
            replayPipeline.IsSkipped = _pipeline.IsSkipped;
            replayPipeline.BecauseMessage = _pipeline.BecauseMessage;

            var replay = new ValueAssertions<T>(replayPipeline, item);

            for (int i = 0; i < _capturedFactories.Count; i++)
            {
                replayPipeline.AddOp(_capturedFactories[i](replay));
            }

            await replayPipeline.RunAsync().ConfigureAwait(false);
        }

        AssertionPipeline Catchy.Sdk.IAssertions.GetPipeline() => _pipeline;
        bool Catchy.Sdk.IAssertions.IsSkipped() => _pipeline.IsSkipped;
        void Catchy.Sdk.IAssertions.AddLink(string link) { if (!string.IsNullOrEmpty(link)) _pipeline.Links.Add(link); }
        void Catchy.Sdk.IAssertions.AddLinks(string?[] links) { foreach (var l in links) if (!string.IsNullOrEmpty(l)) _pipeline.Links.Add(l!); }
        void Catchy.Sdk.IAssertions.Skip(string? reason) { _pipeline.IsSkipped = true; if (!string.IsNullOrEmpty(reason)) _pipeline.BecauseMessage += reason; }

        internal QuantifiedMode GetMode() => _mode;
        internal IReadOnlyList<T> GetValues() => _values;
    }

    public static partial class AsserterExtensions
    {
        /// <summary>Starts a quantified chain that requires every item to satisfy the collected assertions.</summary>
        [GenerateArityOverloads(target: nameof(values))]
        public static QuantifiedAssertions<T> ThatEachOf<T>(this Asserter a, IEnumerable<T> values,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(values))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatEachOf", valueExpr: vExpr, file: file, line: line, member: member);
            return new QuantifiedAssertions<T>(values, p, QuantifiedMode.Each);
        }

        /// <summary>Starts a quantified chain that requires at least one item to satisfy the collected assertions.</summary>
        [GenerateArityOverloads(target: nameof(values))]
        public static QuantifiedAssertions<T> ThatAnyOf<T>(this Asserter a, IEnumerable<T> values,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(values))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatAnyOf", valueExpr: vExpr, file: file, line: line, member: member);
            return new QuantifiedAssertions<T>(values, p, QuantifiedMode.Any);
        }

        /// <summary>Starts a quantified chain that requires no item to satisfy the collected assertions.</summary>
        [GenerateArityOverloads(target: nameof(values))]
        public static QuantifiedAssertions<T> ThatNoneOf<T>(this Asserter a, IEnumerable<T> values,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(values))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatNoneOf", valueExpr: vExpr, file: file, line: line, member: member);
            return new QuantifiedAssertions<T>(values, p, QuantifiedMode.None);
        }

    }

    namespace Sdk
    {
        /// <summary>Provides accessors for quantified assertion metadata for external integrations.</summary>
        public static partial class QuantifiedAccessors
        {
            /// <summary>Gets the quantified mode used by the assertion scope.</summary>
            public static QuantifiedMode GetMode<T>(this QuantifiedAssertions<T> a) => a.GetMode();

            /// <summary>Gets the values captured by the quantified assertion scope.</summary>
            public static IReadOnlyList<T> GetValues<T>(this QuantifiedAssertions<T> a) => a.GetValues();
        }
    }
}
