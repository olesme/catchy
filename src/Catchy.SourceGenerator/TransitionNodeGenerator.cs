using System;
using System.Linq;

namespace Catchy.SourceGenerator
{
    /// <summary>
    /// Generates transition node types for properties/fields on assertion target types.
    /// </summary>
    public static class TransitionNodeGenerator
    {
        /// <summary>
        /// Generates code for a transition property and its backing assertion node type.
        /// </summary>
        public static TransitionNodeGeneratorOutput GenerateTransitionNode(
            string targetTypeName,
            string propertyName,
            string propertyTypeName,
            bool lazyTransitions)
        {
            var output = new TransitionNodeGeneratorOutput();

            var transitionNodeTypeName = SanitizeIdentifier(targetTypeName) +
                                         SanitizeIdentifier(propertyName) + "Assertions";

            var nodeTypeCode = GenerateTransitionNodeClass(
                transitionNodeTypeName,
                propertyTypeName,
                propertyName,
                lazyTransitions);

            output.TransitionNodeTypeCode = nodeTypeCode;
            output.TransitionNodeTypeName = transitionNodeTypeName;

            var methodCode = GenerateTransitionMethod(
                propertyName,
                transitionNodeTypeName,
                lazyTransitions);

            output.TransitionMethodCode = methodCode;

            return output;
        }

        private static string GenerateTransitionNodeClass(
            string transitionNodeTypeName,
            string propertyTypeName,
            string propertyName,
            bool lazyTransitions)
        {
            var constructorBody = lazyTransitions
                ? """
                        {
                            // Lazy resolver: property value is not fetched until assertion execution
                            // This allows modifiers at chain end to affect resolution behavior
                        }
                """
                : """
                        {
                            // Eager value capture (deprecated; use lazy mode)
                        }
                """;

            var constructorParam = lazyTransitions
                ? $"Func<{propertyTypeName}> valueResolver"
                : $"{propertyTypeName} value";

            return $$"""
                /// <summary>
                /// Assertions for {{propertyName}}.
                /// </summary>
                public partial class {{transitionNodeTypeName}}
                    : ValueAssertions<{{transitionNodeTypeName}}, {{propertyTypeName}}>
                {
                    /// <summary>
                    /// Creates assertions for {{propertyName}}.
                    /// </summary>
                    public {{transitionNodeTypeName}}({{constructorParam}})
            {{constructorBody}}
                }

            """;
        }

        private static string GenerateTransitionMethod(
            string propertyName,
            string transitionNodeTypeName,
            bool lazyTransitions)
        {
            var returnStatement = lazyTransitions
                ? $"return new {transitionNodeTypeName}(() => this.Target.{propertyName});"
                : $"return new {transitionNodeTypeName}(this.Target.{propertyName});";

            return $$"""
                    /// <summary>
                    /// Starts assertions for {{propertyName}}.
                    /// </summary>
                    public {{transitionNodeTypeName}} {{propertyName}}()
                    {
                        {{returnStatement}}
                    }

            """;
        }

        private static string SanitizeIdentifier(string name)
        {
            return new string([.. name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_')]);
        }
    }

    /// <summary>
    /// Output from transition node generation.
    /// </summary>
    public class TransitionNodeGeneratorOutput
    {
        public string TransitionNodeTypeCode { get; set; } = "";

        public string TransitionNodeTypeName { get; set; } = "";

        public string TransitionMethodCode { get; set; } = "";
    }

}
