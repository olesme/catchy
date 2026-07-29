using Catchy;
using CatchyCoreTests.Helpers;
using System.Reflection;

namespace CatchyCoreTests.Assertions.Objects;

/// <summary>
/// Integration tests for object assertions (Type, object, reflection).
/// Covers type equality, inheritance, interface implementation, and type inspection.
/// </summary>
public class StructuralAssertionsTests
{
    private class BaseClass { }
    private class DerivedClass : BaseClass { }
    private interface ITestInterface { }
    private class ImplementingClass : ITestInterface { }

    // ===== Type Assertions =====

    [Fact]
    public async Task Type_Is_WithSameType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).Is(typeof(string));
    }

    [Fact]
    public async Task Type_Is_WithDifferentType_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(string);
        await Stateless.Assert.That(type).Is(typeof(int));
        });
    }

    [Fact]
    public async Task Type_IsNot_WithDifferentType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNot(typeof(int));
    }

    [Fact]
    public async Task Type_Name_IsEqualTo_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).Name().Is("String");
    }

    [Fact]
    public async Task Type_Namespace_IsEqualTo_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).Namespace().Is("System");
    }

    [Fact]
    public async Task Type_FullName_IsEqualTo_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).FullName().Is("System.String");
    }

    [Fact]
    public async Task Type_IsGenericType_WithGenericType_Passes()
    {
        var type = typeof(List<>);
        await Stateless.Assert.That(type).IsGenericType();
    }

    [Fact]
    public async Task Type_IsGenericType_WithNonGenericType_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsGenericType();
        });
    }

    [Fact]
    public async Task Type_IsNotGenericType_WithNonGenericType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotGenericType();
    }

    [Fact]
    public async Task Type_IsValueType_WithStruct_Passes()
    {
        var type = typeof(int);
        await Stateless.Assert.That(type).IsValueType();
    }

    [Fact]
    public async Task Type_IsValueType_WithClass_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsValueType();
        });
    }

    [Fact]
    public async Task Type_IsReferenceType_WithClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsReferenceType();
    }

    [Fact]
    public async Task Type_IsAbstract_WithAbstractClass_Passes()
    {
        var type = typeof(object);
        // Note: object is special; using a different approach for true abstract class
        var abstractType = typeof(System.IO.Stream);
        await Stateless.Assert.That(abstractType).IsAbstract();
    }

    [Fact]
    public async Task Type_IsSealed_WithSealedClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsSealed();
    }

    [Fact]
    public async Task Type_IsInterface_WithInterface_Passes()
    {
        var type = typeof(IEnumerable<object>);
        await Stateless.Assert.That(type).IsInterface();
    }

    [Fact]
    public async Task Type_IsInterface_WithClass_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsInterface();
        });
    }

    [Fact]
    public async Task Type_IsNotInterface_WithClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotInterface();
    }

    // ===== Type Inheritance & Interfaces =====

    [Fact]
    public async Task Type_Inherits_WithDirectInheritance_Passes()
    {
        var derived = typeof(DerivedClass);
        var base_class = typeof(BaseClass);
        await Stateless.Assert.That(derived).Inherits(base_class);
        await Stateless.Assert.That(derived).Inherits<BaseClass>();
    }

    [Fact]
    public async Task Type_Inherits_WithNoInheritance_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type1 = typeof(string);
        var type2 = typeof(int);
        await Stateless.Assert.That(type1).Inherits(type2);
        await Stateless.Assert.That(type1).Inherits<int>();
        });
    }

    [Fact]
    public async Task Type_Implements_WithDirectImplementation_Passes()
    {
        var implementing = typeof(ImplementingClass);
        var interface_type = typeof(ITestInterface);
        await Stateless.Assert.That(implementing).Implements(interface_type);
        await Stateless.Assert.That(implementing).Implements<ITestInterface>();
    }

    [Fact]
    public async Task Type_Implements_WithNoImplementation_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(BaseClass);
        var interface_type = typeof(IEnumerable<object>);
        await Stateless.Assert.That(type).Implements(interface_type);
        await Stateless.Assert.That(type).Implements<IEnumerable<object>>();
        });
    }

    [Fact]
    public async Task Type_Implements_WithListAndIEnumerable_Passes()
    {
        var type = typeof(List<int>);
        var interface_type = typeof(IEnumerable<int>);
        await Stateless.Assert.That(type).Implements(interface_type);
        await Stateless.Assert.That(type).Implements<IEnumerable<int>>();
    }

    [Fact]
    public async Task Type_DoesNotImplement_WithUnrelatedInterface_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).DoesNotImplement<IDisposable>();
    }

    [Fact]
    public async Task Type_DoesNotInherit_WithUnrelatedType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).DoesNotInherit<Stream>();
        await Stateless.Assert.That(type).DoesNotInherit(typeof(Stream));
    }

    [Fact]
    public async Task Type_IsNotAssignableTo_WithUnrelatedType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotAssignableTo<int>();
    }

    [Fact]
    public async Task Type_IsNotAssignableFrom_WithUnrelatedType_Passes()
    {
        var type = typeof(int);
        await Stateless.Assert.That(type).IsNotAssignableFrom<string>();
    }

    [Fact]
    public async Task Type_GetMethods_Count_Passes()
    {
        var type = typeof(string);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        // String has many methods; just verify we can assert on count
        await Stateless.Assert.That(methods).IsNotEmpty();
    }

    [Fact]
    public async Task Type_GetProperties_Count_Passes()
    {
        var type = typeof(string);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        // String has Length and Chars properties
        await Stateless.Assert.That(properties.Length).IsGreaterThan(0);
    }

    [Fact]
    public async Task Type_IsClass_WithClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsClass();
    }

    [Fact]
    public async Task Type_IsNotClass_WithInterface_Passes()
    {
        var type = typeof(IEnumerable<object>);
        await Stateless.Assert.That(type).IsNotClass();
    }

    [Fact]
    public async Task Type_IsEnum_WithEnum_Passes()
    {
        var type = typeof(DayOfWeek);
        await Stateless.Assert.That(type).IsEnum();
    }

    [Fact]
    public async Task Type_IsNotEnum_WithClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotEnum();
    }

    [Fact]
    public async Task Type_IsGenericTypeDefinition_WithOpenGeneric_Passes()
    {
        var type = typeof(List<>);
        await Stateless.Assert.That(type).IsGenericTypeDefinition();
    }

    [Fact]
    public async Task Type_IsNotGenericTypeDefinition_WithClosedType_Passes()
    {
        var type = typeof(List<int>);
        await Stateless.Assert.That(type).IsNotGenericTypeDefinition();
    }

    [Fact]
    public async Task Type_IsNotAbstract_WithNonAbstractType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotAbstract();
    }

    [Fact]
    public async Task Type_IsNotSealed_WithNonSealedType_Passes()
    {
        var type = typeof(Stream);
        await Stateless.Assert.That(type).IsNotSealed();
    }

    [Fact]
    public async Task Type_IsPublic_WithPublicType_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsPublic();
    }

    [Fact]
    public async Task Type_IsNotPublic_WithNestedPrivateType_Passes()
    {
        var type = typeof(Person);
        await Stateless.Assert.That(type).IsNotPublic();
    }

    [Fact]
    public async Task Type_IsStatic_WithStaticClass_Passes()
    {
        var type = typeof(Math);
        await Stateless.Assert.That(type).IsStatic();
    }

    [Fact]
    public async Task Type_IsNotStatic_WithRegularClass_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotStatic();
    }

    [Fact]
    public async Task Type_IsInNamespace_WithSystemNamespace_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsInNamespace("System");
    }

    [Fact]
    public async Task Type_IsNotInNamespace_WithDifferentNamespace_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsNotInNamespace("Catchy");
    }

    [Fact]
    public async Task Type_HasProperty_And_HasMethod_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).HasProperty("Length");
        await Stateless.Assert.That(type).HasMethod("GetHashCode"); // single overload, no ambiguity
    }

    [Fact]
    public async Task Type_DoesNotHaveProperty_And_DoesNotHaveMethod_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).DoesNotHaveProperty("DefinitelyMissingProperty");
        await Stateless.Assert.That(type).DoesNotHaveMethod("DefinitelyMissingMethod");
    }

    [Fact]
    public async Task Type_HasGenericArgumentCount_Passes()
    {
        var type = typeof(Dictionary<string, int>);
        await Stateless.Assert.That(type).HasGenericArgumentCount(2);
    }

    // ===== Object Assertions =====

    [Fact]
    public async Task Object_Is_WithSameObject_Passes()
    {
        var obj = new object();
        await Stateless.Assert.That(obj).Is(obj);
    }

    [Fact]
    public async Task Object_Is_WithDifferentObjects_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var obj1 = new object();
        var obj2 = new object();
        await Stateless.Assert.That(obj1).Is(obj2);
        });
    }

    [Fact]
    public async Task Object_IsNot_WithDifferentObjects_Passes()
    {
        var obj1 = new object();
        var obj2 = new object();
        await Stateless.Assert.That(obj1).IsNot(obj2);
    }

    [Fact]
    public async Task Object_IsNull_WithNull_Passes()
    {
        object? obj = null;
        await Stateless.Assert.That(obj).IsNull();
    }

    [Fact]
    public async Task Object_IsNull_WithNonNullObject_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var obj = new object();
        await Stateless.Assert.That(obj).IsNull();
        });
    }

    [Fact]
    public async Task Object_IsNotNull_WithNonNullObject_Passes()
    {
        var obj = new object();
        await Stateless.Assert.That(obj).IsNotNull();
    }

    // ===== Type Compatibility =====

    [Fact]
    public async Task Type_IsAssignableTo_WithCompatibleType_Passes()
    {
        var type = typeof(DerivedClass);
        var targetType = typeof(BaseClass);
        await Stateless.Assert.That(type).IsAssignableTo(targetType);
    }

    [Fact]
    public async Task Type_IsAssignableTo_WithGenericTarget_Passes()
    {
        var type = typeof(string);
        await Stateless.Assert.That(type).IsAssignableTo<object>();
    }

    [Fact]
    public async Task Type_IsAssignableFrom_WithGenericSource_Passes()
    {
        var type = typeof(object);
        await Stateless.Assert.That(type).IsAssignableFrom<string>();
    }

    [Fact]
    public async Task Type_IsAssignableTo_WithIncompatibleType_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var type = typeof(string);
        var targetType = typeof(int);
        await Stateless.Assert.That(type).IsAssignableTo(targetType);
        await Stateless.Assert.That(type).IsAssignableTo<int>();
        });
    }

    [Fact]
    public async Task Type_IsAssignableFrom_WithCompatibleType_Passes()
    {
        var type = typeof(BaseClass);
        var sourceType = typeof(DerivedClass);
        await Stateless.Assert.That(type).IsAssignableFrom(sourceType);
        await Stateless.Assert.That(type).IsAssignableFrom<DerivedClass>();
    }

    // ===== Generic Type Handling =====

    [Fact]
    public async Task Type_GenericTypeDefinition_Is_Passes()
    {
        var type = typeof(List<int>);
        await Stateless.Assert.That(type).GenericTypeDefinition().Is(typeof(List<>));
    }

    [Fact]
    public async Task Type_GenericArguments_Count_Passes()
    {
        var type = typeof(Dictionary<string, int>);
        var args = type.GetGenericArguments();
        await Stateless.Assert.That(args).HasCount(2);
    }

    // ===== Modifiers & Chaining =====

    [Fact]
    public async Task Type_Is_WithBecause_IncludesMessage()
    {
        try
        {
            await Stateless.Assert.That(typeof(int)).Is(typeof(string)).Because("type must match");
            throw new AssertionException("Expected AssertionException");
        }
        catch (AssertionException ex)
        {
            if (!ex.Message.Contains("type must match"))
                throw new AssertionException($"Message missing reason. Got: {ex.Message}");
        }
    }

    [Fact]
    public async Task Type_Name_WithAnd_Chains()
    {
        var type = typeof(List<int>);
        await Stateless.Assert.That(type).IsGenericType().And().Name().Is("List`1");
    }

    // ===== AmbientSoft Mode =====

    [Fact]
    public async Task Object_SoftMode_AccumulatesFailures()
    {
        var verify = Asserter.NewSoft();
        var obj = "test";

        await verify.That(obj).Is("test");        // Pass
        await verify.That(obj).Is("other");       // Fail
        await verify.That(obj).IsNull();          // Fail

        if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
    }

    [Fact]
    public async Task Type_SoftMode_AccumulatesFailures()
    {
        var verify = Asserter.NewSoft();
        var type = typeof(string);

        await verify.That(type).Is(typeof(string));     // Pass
        await verify.That(type).IsValueType();          // Fail
        await verify.That(type).IsInterface();          // Fail

        if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
    }

    // ===== Custom Objects with Deep Equality =====

    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }

        public override bool Equals(object? obj) =>
            obj is Person p && p.Name == Name && p.Age == Age;

        public override int GetHashCode() => HashCode.Combine(Name, Age);
    }

    [Fact]
    public async Task Object_Equals_WithEqualCustomObjects_Passes()
    {
        var p1 = new Person { Name = "Alice", Age = 30 };
        var p2 = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That(p1).Is(p2);
    }

    [Fact]
    public async Task Object_Equals_WithUnequalCustomObjects_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var p1 = new Person { Name = "Alice", Age = 30 };
        var p2 = new Person { Name = "Bob", Age = 25 };
        await Stateless.Assert.That(p1).Is(p2);
        });
    }

    [Fact]
    public async Task Object_IsInstanceOf_passes_for_derived_instance()
    {
        BaseClass value = new DerivedClass();
        await Stateless.Assert.That<BaseClass>(value).IsInstanceOf(typeof(DerivedClass));
    }

    [Fact]
    public async Task Object_IsInstanceOf_fails_for_wrong_type()
    {
        BaseClass value = new BaseClass();
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That<BaseClass>(value).IsInstanceOf(typeof(DerivedClass)));
        Assert.Contains("DerivedClass", msg);
    }

    [Fact]
    public async Task Object_IsNotInstanceOf_passes_for_unrelated_type()
    {
        BaseClass value = new BaseClass();
        await Stateless.Assert.That<BaseClass>(value).IsNotInstanceOf(typeof(DerivedClass));
    }

    [Fact]
    public async Task Object_IsExactTypeOf_passes_for_exact_type()
    {
        BaseClass value = new BaseClass();
        await Stateless.Assert.That<BaseClass>(value).IsExactTypeOf<BaseClass, BaseClass>();
    }

    [Fact]
    public async Task Object_IsNotExactTypeOf_passes_for_different_exact_type()
    {
        BaseClass value = new DerivedClass();
        await Stateless.Assert.That<BaseClass>(value).IsNotExactTypeOf<BaseClass, BaseClass>();
    }

    [Fact]
    public async Task Object_IsSameReferenceAs_passes_for_same_reference()
    {
        var obj = new Person { Name = "Alice", Age = 30 };
        var same = obj;
        await Stateless.Assert.That(obj).IsSameReferenceAs(same);
    }

    [Fact]
    public async Task Object_IsNotSameReferenceAs_passes_for_different_instances()
    {
        var p1 = new Person { Name = "Alice", Age = 30 };
        var p2 = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That(p1).IsNotSameReferenceAs(p2);
    }

    [Fact]
    public async Task Object_IsDeepCloneOf_passes_for_equivalent_but_different_instance()
    {
        var original = new Person { Name = "Alice", Age = 30 };
        var clone = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That(clone).IsDeepCloneOf(original);
    }

    [Fact]
    public async Task Object_IsDeepCloneOf_fails_for_same_reference()
    {
        var original = new Person { Name = "Alice", Age = 30 };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(original).IsDeepCloneOf(original));
        Assert.Contains("reference", msg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Object_IsEquivalentTo_passes_for_equal_objects()
    {
        var left = new Person { Name = "Alice", Age = 30 };
        var right = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That(left).IsEquivalentTo(right);
    }

    [Fact]
    public async Task Object_IsNotEquivalentTo_passes_for_different_objects()
    {
        var left = new Person { Name = "Alice", Age = 30 };
        var right = new Person { Name = "Alice", Age = 31 };
        await Stateless.Assert.That(left).IsNotEquivalentTo(right);
    }

    [Fact]
    public async Task Object_IsNotEquivalentTo_throws_for_equivalent_objects()
    {
        var left = new Person { Name = "Alice", Age = 30 };
        var right = new Person { Name = "Alice", Age = 30 };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(left).IsNotEquivalentTo(right));
        Assert.Contains("not to be equivalent", msg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Object_ThatHas_projects_value_for_followup_assertions()
    {
        var person = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That<Person>(person).ThatHas(p => p.Name).Is("Alice");
    }

    [Fact]
    public async Task Object_Property_alias_projects_value_for_followup_assertions()
    {
        var person = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That<Person>(person).Property(p => p.Name).Is("Alice");
    }

    [Fact]
    public async Task Object_IsNotSameReferenceAs_GenericType_Passes()
    {
        var p1 = new Person { Name = "Alice", Age = 30 };
        var p2 = new Person { Name = "Alice", Age = 30 };
        await Stateless.Assert.That<Person>(p1).IsNotSameReferenceAs(p2);
    }
}




