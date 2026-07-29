using System;

namespace Catchy
{
    /// <summary>
    /// Instructs the source generator to emit a concrete overload of this method for each
    /// <paramref name="targetTypes"/> by substituting the template type throughout.
    /// </summary>
    /// <remarks>
    /// The template type is inferred automatically from the receiver's single generic argument
    /// (e.g. <c>ValueAssertions&lt;double&gt;</c> → <c>double</c>). Use the <see cref="TemplateType"/>
    /// named property to override when inference is not possible or correct.
    /// <para>Example — auto-inferred:</para>
    /// <code>[GenerateTypedOverloads(typeof(float), typeof(decimal))]</code>
    /// <para>Example — explicit override:</para>
    /// <code>[GenerateTypedOverloads(typeof(float), TemplateType = typeof(double))]</code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class GenerateTypedOverloadsAttribute : Attribute
    {
        /// <param name="targetTypes">Concrete types to generate overloads for.</param>
        public GenerateTypedOverloadsAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes ?? Array.Empty<Type>();
        }

        /// <summary>Concrete types to generate overloads for.</summary>
        public Type[] TargetTypes { get; }

        /// <summary>
        /// Explicitly sets the type to substitute. When omitted, inferred from the receiver's
        /// single generic argument.
        /// </summary>
        public Type? TemplateType { get; set; }
    }
}
