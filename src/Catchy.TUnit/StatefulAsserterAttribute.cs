using System;
using System.Collections.Generic;

namespace Catchy.TUnit
{
    /// <summary>
    /// Injects StatefulAsserter into test parameter or property.
    /// Uses same ambient instance as Ambient.Assert.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class StatefulAsserterAttribute : DataSourceGeneratorAttribute<StatefulAsserter>
    {
        protected override IEnumerable<Func<StatefulAsserter>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
        {
            yield return () =>
            {
                return AmbientAsserterSource.Stateful.TryGetCurrent() ?? AmbientAsserterSource.Stateful.Factory();
            };
        }
    }
}
