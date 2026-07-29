using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Catchy.Sdk;

namespace Catchy
{
    // -------------------------------------------------------------------------
    // Entry point
    // -------------------------------------------------------------------------

    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an <see cref="Assembly"/> value.</summary>
        public static ValueAssertions<Assembly> That(
            this Asserter a, Assembly assembly,
            [CallerArgumentExpression(nameof(a))]        string? aExpr  = null,
            [CallerArgumentExpression(nameof(assembly))] string? vExpr  = null,
            [CallerFilePath]   string? file   = null,
            [CallerLineNumber] int     line   = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Assembly>(p, assembly);
        }
    }

    // -------------------------------------------------------------------------
    // Assembly -> IEnumerable<Type> selectors
    // -------------------------------------------------------------------------

    public static class AssemblyTypeSelectors
    {
        public static ValueAssertions<IEnumerable<Type>?> Types(this ValueAssertions<Assembly> a)
        { a.Link("Types"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes()); }

        public static ValueAssertions<IEnumerable<Type>?> Classes(this ValueAssertions<Assembly> a)
        { a.Link("Classes"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsClass && !t.IsAbstract)); }

        public static ValueAssertions<IEnumerable<Type>?> AbstractClasses(this ValueAssertions<Assembly> a)
        { a.Link("AbstractClasses"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsClass && t.IsAbstract && !t.IsSealed)); }

        public static ValueAssertions<IEnumerable<Type>?> StaticClasses(this ValueAssertions<Assembly> a)
        { a.Link("StaticClasses"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsClass && t.IsAbstract && t.IsSealed)); }

        public static ValueAssertions<IEnumerable<Type>?> SealedClasses(this ValueAssertions<Assembly> a)
        { a.Link("SealedClasses"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsClass && t.IsSealed && !t.IsAbstract)); }

        public static ValueAssertions<IEnumerable<Type>?> Interfaces(this ValueAssertions<Assembly> a)
        { a.Link("Interfaces"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsInterface)); }

        public static ValueAssertions<IEnumerable<Type>?> Enums(this ValueAssertions<Assembly> a)
        { a.Link("Enums"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsEnum)); }

        public static ValueAssertions<IEnumerable<Type>?> Structs(this ValueAssertions<Assembly> a)
        { a.Link("Structs"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(t => t.IsValueType && !t.IsEnum)); }

        public static ValueAssertions<IEnumerable<Type>?> Records(this ValueAssertions<Assembly> a)
        { a.Link("Records"); return new ValueAssertions<IEnumerable<Type>?>(a.GetPipeline(), a.GetValue().GetTypes().Where(ReflectionHelpers.IsRecord)); }
    }

    // -------------------------------------------------------------------------
    // IEnumerable<Type> filters
    // -------------------------------------------------------------------------

    public static class TypeCollectionFilters
    {
        public static ValueAssertions<IEnumerable<Type>?> InNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("InNamespace", expr); return a.Filter(t => t.Namespace?.StartsWith(ns, StringComparison.Ordinal) == true); }

        public static ValueAssertions<IEnumerable<Type>?> InExactNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("InExactNamespace", expr); return a.Filter(t => t.Namespace == ns); }

        public static ValueAssertions<IEnumerable<Type>?> NotInNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("NotInNamespace", expr); return a.Filter(t => t.Namespace?.StartsWith(ns, StringComparison.Ordinal) != true); }

        public static ValueAssertions<IEnumerable<Type>?> Implementing<TInterface>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("Implementing", typeof(TInterface).Name); return a.Filter(t => typeof(TInterface).IsAssignableFrom(t) && t != typeof(TInterface)); }

        public static ValueAssertions<IEnumerable<Type>?> Implementing(this ValueAssertions<IEnumerable<Type>?> a, Type iface, [CallerArgumentExpression(nameof(iface))] string? expr = null)
        { a.Link("Implementing", expr); return a.Filter(t => iface.IsAssignableFrom(t) && t != iface); }

        public static ValueAssertions<IEnumerable<Type>?> NotImplementing<TInterface>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("NotImplementing", typeof(TInterface).Name); return a.Filter(t => !typeof(TInterface).IsAssignableFrom(t)); }

        public static ValueAssertions<IEnumerable<Type>?> InheritingFrom<TBase>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("InheritingFrom", typeof(TBase).Name); return a.Filter(t => t.IsSubclassOf(typeof(TBase))); }

        public static ValueAssertions<IEnumerable<Type>?> InheritingFrom(this ValueAssertions<IEnumerable<Type>?> a, Type baseType, [CallerArgumentExpression(nameof(baseType))] string? expr = null)
        { a.Link("InheritingFrom", expr); return a.Filter(t => t.IsSubclassOf(baseType)); }

        public static ValueAssertions<IEnumerable<Type>?> NotInheritingFrom<TBase>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("NotInheritingFrom", typeof(TBase).Name); return a.Filter(t => !t.IsSubclassOf(typeof(TBase))); }

        public static ValueAssertions<IEnumerable<Type>?> WithAttribute<TAttr>(this ValueAssertions<IEnumerable<Type>?> a) where TAttr : Attribute
        { a.Link("WithAttribute", typeof(TAttr).Name); return a.Filter(t => t.GetCustomAttribute<TAttr>(inherit: true) is not null); }

        public static ValueAssertions<IEnumerable<Type>?> WithAttribute(this ValueAssertions<IEnumerable<Type>?> a, Type attrType, [CallerArgumentExpression(nameof(attrType))] string? expr = null)
        { a.Link("WithAttribute", expr); return a.Filter(t => t.GetCustomAttribute(attrType, inherit: true) is not null); }

        public static ValueAssertions<IEnumerable<Type>?> WithoutAttribute<TAttr>(this ValueAssertions<IEnumerable<Type>?> a) where TAttr : Attribute
        { a.Link("WithoutAttribute", typeof(TAttr).Name); return a.Filter(t => t.GetCustomAttribute<TAttr>(inherit: true) is null); }

        public static ValueAssertions<IEnumerable<Type>?> WithNameStartingWith(this ValueAssertions<IEnumerable<Type>?> a, string prefix, [CallerArgumentExpression(nameof(prefix))] string? expr = null)
        { a.Link("WithNameStartingWith", expr); return a.Filter(t => t.Name.StartsWith(prefix, StringComparison.Ordinal)); }

        public static ValueAssertions<IEnumerable<Type>?> WithNameEndingWith(this ValueAssertions<IEnumerable<Type>?> a, string suffix, [CallerArgumentExpression(nameof(suffix))] string? expr = null)
        { a.Link("WithNameEndingWith", expr); return a.Filter(t => t.Name.EndsWith(suffix, StringComparison.Ordinal)); }

        public static ValueAssertions<IEnumerable<Type>?> WithNameMatching(this ValueAssertions<IEnumerable<Type>?> a, Regex pattern)
        { a.Link("WithNameMatching", pattern.ToString()); return a.Filter(t => pattern.IsMatch(t.Name)); }

        public static ValueAssertions<IEnumerable<Type>?> ThatAreClasses(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ThatAreClasses"); return a.Filter(t => t.IsClass); }

        public static ValueAssertions<IEnumerable<Type>?> ThatAreInterfaces(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ThatAreInterfaces"); return a.Filter(t => t.IsInterface); }

        public static ValueAssertions<IEnumerable<Type>?> ThatArePublic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ThatArePublic"); return a.Filter(t => t.IsPublic || (t.IsNested && t.IsNestedPublic)); }

        public static ValueAssertions<IEnumerable<Type>?> ThatAreNotPublic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ThatAreNotPublic"); return a.Filter(t => !(t.IsPublic || (t.IsNested && t.IsNestedPublic))); }

        public static ValueAssertions<IEnumerable<Type>?> ThatAreGeneric(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ThatAreGeneric"); return a.Filter(t => t.IsGenericTypeDefinition); }

        public static ValueAssertions<IEnumerable<Type>?> Except(this ValueAssertions<IEnumerable<Type>?> a, Func<Type, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("Except", expr); return a.Filter(t => !predicate(t)); }

        public static ValueAssertions<IEnumerable<Type>?> Except(this ValueAssertions<IEnumerable<Type>?> a, Type type, [CallerArgumentExpression(nameof(type))] string? expr = null)
        { a.Link("Except", expr); return a.Filter(t => t != type); }

        internal static ValueAssertions<IEnumerable<Type>?> Filter(this ValueAssertions<IEnumerable<Type>?> a, Func<Type, bool> predicate)
            => new(a.GetPipeline(), a.GetValue()?.Where(predicate));
    }

    // -------------------------------------------------------------------------
    // IEnumerable<Type> assertion methods
    // -------------------------------------------------------------------------

    public static class TypeCollectionAssertionMethods
    {
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreSealed(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreSealed"); a.Op(op =>
        {
            IReadOnlyList<Type> types = [.. a.GetValue()!];
            return ArchTypeChecks.AllAreSealed(types, op.IsSkipped());
        }); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotSealed(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotSealed"); a.Op(op => ArchTypeChecks.NoneAreSealed([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreAbstract(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreAbstract"); a.Op(op => ArchTypeChecks.AllAreAbstract([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotAbstract(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotAbstract"); a.Op(op => ArchTypeChecks.NoneAreAbstract([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> ArePublic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("ArePublic"); a.Op(op => ArchTypeChecks.AllArePublic([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotPublic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotPublic"); a.Op(op => ArchTypeChecks.NoneArePublic([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreInternal(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreInternal"); a.Op(op => ArchTypeChecks.AllAreInternal([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotInternal(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotInternal"); a.Op(op => ArchTypeChecks.NoneAreInternal([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreStatic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreStatic"); a.Op(op => ArchTypeChecks.AllAreStatic([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotStatic(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotStatic"); a.Op(op => ArchTypeChecks.NoneAreStatic([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNested(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNested"); a.Op(op => ArchTypeChecks.AllAreNested([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotNested(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotNested"); a.Op(op => ArchTypeChecks.NoneAreNested([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreGeneric(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreGeneric"); a.Op(op => ArchTypeChecks.AllAreGeneric([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreNotGeneric(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreNotGeneric"); a.Op(op => ArchTypeChecks.NoneAreGeneric([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreImmutable(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreImmutable"); a.Op(op => ArchTypeChecks.AllAreImmutable([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> AreMutable(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("AreMutable"); a.Op(op => ArchTypeChecks.AllAreMutable([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> Implement<TInterface>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("Implement", typeof(TInterface).Name); a.Op(op => ArchTypeChecks.AllImplement([.. a.GetValue()!], typeof(TInterface), op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> Implement(this ValueAssertions<IEnumerable<Type>?> a, Type iface, [CallerArgumentExpression(nameof(iface))] string? expr = null)
        { a.Link("Implement", expr); a.Op(op => ArchTypeChecks.AllImplement([.. a.GetValue()!], iface, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotImplement<TInterface>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("DoNotImplement", typeof(TInterface).Name); a.Op(op => ArchTypeChecks.NoneImplement([.. a.GetValue()!], typeof(TInterface), op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotImplement(this ValueAssertions<IEnumerable<Type>?> a, Type iface, [CallerArgumentExpression(nameof(iface))] string? expr = null)
        { a.Link("DoNotImplement", expr); a.Op(op => ArchTypeChecks.NoneImplement([.. a.GetValue()!], iface, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> InheritFrom<TBase>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("InheritFrom", typeof(TBase).Name); a.Op(op => ArchTypeChecks.AllInheritFrom([.. a.GetValue()!], typeof(TBase), op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> InheritFrom(this ValueAssertions<IEnumerable<Type>?> a, Type baseType, [CallerArgumentExpression(nameof(baseType))] string? expr = null)
        { a.Link("InheritFrom", expr); a.Op(op => ArchTypeChecks.AllInheritFrom([.. a.GetValue()!], baseType, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotInheritFrom<TBase>(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("DoNotInheritFrom", typeof(TBase).Name); a.Op(op => ArchTypeChecks.NoneInheritFrom([.. a.GetValue()!], typeof(TBase), op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotInheritFrom(this ValueAssertions<IEnumerable<Type>?> a, Type baseType, [CallerArgumentExpression(nameof(baseType))] string? expr = null)
        { a.Link("DoNotInheritFrom", expr); a.Op(op => ArchTypeChecks.NoneInheritFrom([.. a.GetValue()!], baseType, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveAttribute<TAttribute>(this ValueAssertions<IEnumerable<Type>?> a) where TAttribute : Attribute
        { a.Link("HaveAttribute", typeof(TAttribute).Name); a.Op(op => ArchTypeChecks.AllHaveAttribute([.. a.GetValue()!], typeof(TAttribute), inherit: true, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveAttribute(this ValueAssertions<IEnumerable<Type>?> a, Type attrType, [CallerArgumentExpression(nameof(attrType))] string? expr = null)
        { a.Link("HaveAttribute", expr); a.Op(op => ArchTypeChecks.AllHaveAttribute([.. a.GetValue()!], attrType, inherit: true, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotHaveAttribute<TAttribute>(this ValueAssertions<IEnumerable<Type>?> a) where TAttribute : Attribute
        { a.Link("DoNotHaveAttribute", typeof(TAttribute).Name); a.Op(op => ArchTypeChecks.NoneHaveAttribute([.. a.GetValue()!], typeof(TAttribute), inherit: true, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotHaveAttribute(this ValueAssertions<IEnumerable<Type>?> a, Type attrType, [CallerArgumentExpression(nameof(attrType))] string? expr = null)
        { a.Link("DoNotHaveAttribute", expr); a.Op(op => ArchTypeChecks.NoneHaveAttribute([.. a.GetValue()!], attrType, inherit: true, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveNameStartingWith(this ValueAssertions<IEnumerable<Type>?> a, string prefix, [CallerArgumentExpression(nameof(prefix))] string? expr = null)
        { a.Link("HaveNameStartingWith", expr); a.Op(op => ArchTypeChecks.AllHaveNameStartingWith([.. a.GetValue()!], prefix, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> NotHaveNameStartingWith(this ValueAssertions<IEnumerable<Type>?> a, string prefix, [CallerArgumentExpression(nameof(prefix))] string? expr = null)
        { a.Link("NotHaveNameStartingWith", expr); a.Op(op => ArchTypeChecks.NoneHaveNameStartingWith([.. a.GetValue()!], prefix, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveNameEndingWith(this ValueAssertions<IEnumerable<Type>?> a, string suffix, [CallerArgumentExpression(nameof(suffix))] string? expr = null)
        { a.Link("HaveNameEndingWith", expr); a.Op(op => ArchTypeChecks.AllHaveNameEndingWith([.. a.GetValue()!], suffix, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> NotHaveNameEndingWith(this ValueAssertions<IEnumerable<Type>?> a, string suffix, [CallerArgumentExpression(nameof(suffix))] string? expr = null)
        { a.Link("NotHaveNameEndingWith", expr); a.Op(op => ArchTypeChecks.NoneHaveNameEndingWith([.. a.GetValue()!], suffix, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveNameContaining(this ValueAssertions<IEnumerable<Type>?> a, string value, [CallerArgumentExpression(nameof(value))] string? expr = null)
        { a.Link("HaveNameContaining", expr); a.Op(op => ArchTypeChecks.AllHaveNameContaining([.. a.GetValue()!], value, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> NotHaveNameContaining(this ValueAssertions<IEnumerable<Type>?> a, string value, [CallerArgumentExpression(nameof(value))] string? expr = null)
        { a.Link("NotHaveNameContaining", expr); a.Op(op => ArchTypeChecks.NoneHaveNameContaining([.. a.GetValue()!], value, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveNameMatching(this ValueAssertions<IEnumerable<Type>?> a, Regex pattern)
        { a.Link("HaveNameMatching", pattern.ToString()); a.Op(op => ArchTypeChecks.AllHaveNameMatching([.. a.GetValue()!], pattern, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveNameMatching(this ValueAssertions<IEnumerable<Type>?> a, string pattern)
            => a.HaveNameMatching(new Regex(pattern));

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> NotHaveNameMatching(this ValueAssertions<IEnumerable<Type>?> a, Regex pattern)
        { a.Link("NotHaveNameMatching", pattern.ToString()); a.Op(op => ArchTypeChecks.NoneHaveNameMatching([.. a.GetValue()!], pattern, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> ResideInNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("ResideInNamespace", expr); a.Op(op => ArchTypeChecks.AllResideInNamespace([.. a.GetValue()!], ns, exactMatch: false, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> ResideInExactNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("ResideInExactNamespace", expr); a.Op(op => ArchTypeChecks.AllResideInNamespace([.. a.GetValue()!], ns, exactMatch: true, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotResideInNamespace(this ValueAssertions<IEnumerable<Type>?> a, string ns, [CallerArgumentExpression(nameof(ns))] string? expr = null)
        { a.Link("DoNotResideInNamespace", expr); a.Op(op => ArchTypeChecks.NoneResideInNamespace([.. a.GetValue()!], ns, exactMatch: false, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HaveDependencyOn(this ValueAssertions<IEnumerable<Type>?> a, string namespaceOrAssembly, [CallerArgumentExpression(nameof(namespaceOrAssembly))] string? expr = null)
        { a.Link("HaveDependencyOn", expr); a.Op(op => ArchTypeChecks.AllHaveDependencyOn([.. a.GetValue()!], namespaceOrAssembly, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> DoNotHaveDependencyOn(this ValueAssertions<IEnumerable<Type>?> a, string namespaceOrAssembly, [CallerArgumentExpression(nameof(namespaceOrAssembly))] string? expr = null)
        { a.Link("DoNotHaveDependencyOn", expr); a.Op(op => ArchTypeChecks.NoneHaveDependencyOn([.. a.GetValue()!], namespaceOrAssembly, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> Satisfy(this ValueAssertions<IEnumerable<Type>?> a, Func<Type, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("Satisfy", expr); a.Op(op => ArchTypeChecks.AllSatisfy([.. a.GetValue()!], predicate, expr, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> NotSatisfy(this ValueAssertions<IEnumerable<Type>?> a, Func<Type, bool> predicate, [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("NotSatisfy", expr); a.Op(op => ArchTypeChecks.NoneSatisfy([.. a.GetValue()!], predicate, expr, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> IsNotEmpty(this ValueAssertions<IEnumerable<Type>?> a)
        { a.Link("IsNotEmpty"); a.Op(op => ArchTypeChecks.IsNotEmpty([.. a.GetValue()!], op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HasCount(this ValueAssertions<IEnumerable<Type>?> a, int count, [CallerArgumentExpression(nameof(count))] string? expr = null)
        { a.Link("HasCount", expr); a.Op(op => ArchTypeChecks.HasCount([.. a.GetValue()!], count, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HasCountGreaterThan(this ValueAssertions<IEnumerable<Type>?> a, int count, [CallerArgumentExpression(nameof(count))] string? expr = null)
        { a.Link("HasCountGreaterThan", expr); a.Op(op => ArchTypeChecks.HasCountGreaterThan([.. a.GetValue()!], count, op.IsSkipped())); return a; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>?> HasCountLessThan(this ValueAssertions<IEnumerable<Type>?> a, int count, [CallerArgumentExpression(nameof(count))] string? expr = null)
        { a.Link("HasCountLessThan", expr); a.Op(op => ArchTypeChecks.HasCountLessThan([.. a.GetValue()!], count, op.IsSkipped())); return a; }
    }

    // -------------------------------------------------------------------------
    // Internal helpers
    // -------------------------------------------------------------------------

    internal static class ReflectionHelpers
    {
        internal static bool IsRecord(Type t)
            => t.IsClass
            && (t.GetProperty("EqualityContract", BindingFlags.Instance | BindingFlags.NonPublic) is not null
                || t.GetMethod("<Clone>$", BindingFlags.Instance | BindingFlags.Public) is not null);
    }

    // -------------------------------------------------------------------------
    // SDK accessors (for Catchy.Cecil and other integrations)
    // -------------------------------------------------------------------------

    namespace Sdk
    {
        public static class AssemblyAssertionsAccessors
        {
            public static Assembly GetAssembly(this ValueAssertions<Assembly> a) => a.GetValue();
        }

        public static class TypeCollectionAssertionsAccessors
        {
            public static IReadOnlyList<Type> GetTypes(this ValueAssertions<IEnumerable<Type>?> a)
                => a.GetValue() is IReadOnlyList<Type> r ? r : [.. a.GetValue() ?? []];
        }
    }
}
