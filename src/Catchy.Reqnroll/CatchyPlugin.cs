using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Bindings.Reflection;
using Reqnroll.Plugins;
using Reqnroll.UnitTestProvider;

[assembly: global::Reqnroll.Plugins.RuntimePlugin(typeof(Catchy.ReqnrollPlugin.CatchyPlugin))]

namespace Catchy.ReqnrollPlugin
{
    public sealed class CatchyPlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents events, RuntimePluginParameters parameters,
            UnitTestProviderConfiguration config)
        {
            events.CustomizeGlobalDependencies += (_, args) =>
            {
                var container = args.ObjectContainer;
                var registry = container.Resolve<IBindingRegistry>();
                var factory = container.Resolve<IBindingFactory>();
                RegisterInternalHooks(registry, factory);
                ReqnrollAsserterProvider.EnsureRegistered();
            };

            events.CustomizeScenarioDependencies += (_, args) =>
            {
                var output = args.ObjectContainer.Resolve<IReqnrollOutputHelper>();
                var statefulAsserter = Asserter.NewStateful(s => s.OnSoftFailure = [.. s.OnSoftFailure, (info =>
                {
                    output.WriteLine($"[SOFT FAIL]: {info.Exception?.Message ?? info.ToString()}");
#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER
                    return new ValueTask(Task.CompletedTask);
#else
                    return ValueTask.CompletedTask;
#endif
                })]);
                args.ObjectContainer.RegisterInstanceAs(statefulAsserter);
                ReqnrollAsserterProvider.SetStateful(statefulAsserter);
            };
        }

        private static void RegisterInternalHooks(IBindingRegistry registry, IBindingFactory factory)
        {
            var type = typeof(Catchy.ReqnrollPlugin.CatchyHooks);
            // Register as an AfterScenario hook so the softAsserter-assert flush runs at scenario end.
            registry.RegisterHookBinding(factory.CreateHookBinding(
                new RuntimeBindingMethod(type.GetMethod(nameof(Catchy.ReqnrollPlugin.CatchyHooks.SetSofError))!),
                HookType.AfterScenario, null, int.MinValue));

            registry.RegisterHookBinding(factory.CreateHookBinding(
                new RuntimeBindingMethod(type.GetMethod(nameof(Catchy.ReqnrollPlugin.CatchyHooks.ThrowOnSofError))!),
                HookType.AfterScenario, null, int.MaxValue));
        }
    }
}
