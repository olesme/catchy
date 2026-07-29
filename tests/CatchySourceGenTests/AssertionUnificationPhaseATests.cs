using Catchy;
using Catchy.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Catchy.StatelessAlias;

namespace CatchySourceGenTests
{
    /// <summary>
    /// Tests for Phase A foundation: resolver engine, template gating, and transition generation.
    /// These tests verify the new assertion unification architecture.
    /// </summary>
    public class AssertionUnificationPhaseATests
    {
        #region AssertForResolver Tests

        [Test]
        public async Task AssertForResolver_SingleCandidate_ReturnsWinner()
        {
            // Arrange
            var resolver = new AssertForResolver();
            var candidate = new AssertForCandidate
            {
                DeclaringTypeName = "UserAssertions",
                TargetTypeNames = ["User"]
            };
            resolver.RegisterCandidate(candidate);

            // Act
            var result = resolver.Resolve("User", out var isAmbiguous);

            // Assert
            await Check.That(result).IsNotNull();
            await Check.That(result!.DeclaringTypeName).Is("UserAssertions");
            await Check.That(isAmbiguous).IsFalse();
        }

        [Test]
        public async Task AssertForResolver_NoCandidates_ReturnsNull()
        {
            // Arrange
            var resolver = new AssertForResolver();

            // Act
            var result = resolver.Resolve("UnknownType", out var isAmbiguous);

            // Assert
            await Check.That(result).IsNull();
            await Check.That(isAmbiguous).IsFalse();
        }

        [Test]
        public async Task AssertForResolver_MultipleCandidates_WithEqualSpecificity_IsAmbiguous()
        {
            // Arrange
            var resolver = new AssertForResolver();
            var candidate1 = new AssertForCandidate
            {
                DeclaringTypeName = "UserAssertionsV1",
                TargetTypeNames = ["User"]
            };
            var candidate2 = new AssertForCandidate
            {
                DeclaringTypeName = "UserAssertionsV2",
                TargetTypeNames = ["User"]
            };
            resolver.RegisterCandidate(candidate1);
            resolver.RegisterCandidate(candidate2);

            // Act
            var result = resolver.Resolve("User", out var isAmbiguous);

            // Assert
            await Check.That(result).IsNull();
            await Check.That(isAmbiguous).IsTrue();
        }

        [Test]
        public async Task AssertForResolver_AmbiguousCandidates_ReportsAmbiguity()
        {
            // Arrange
            var resolver = new AssertForResolver();
            var candidate1 = new AssertForCandidate
            {
                DeclaringTypeName = "UserAssertionsA",
                TargetTypeNames = ["User"]
            };
            var candidate2 = new AssertForCandidate
            {
                DeclaringTypeName = "UserAssertionsB",
                TargetTypeNames = ["User"]
            };
            resolver.RegisterCandidate(candidate1);
            resolver.RegisterCandidate(candidate2);

            // Act
            var result = resolver.Resolve("User", out var isAmbiguous);

            // Assert
            await Check.That(result).IsNull();
            await Check.That(isAmbiguous).IsTrue();
            var diagnostics = resolver.GetDiagnostics().ToList();
            await Check.That(diagnostics).Contains(d => d.Code == "ASRT0001", out _);
        }

        [Test]
        public async Task AssertForResolver_Specificity_SpecificWinsOverGeneric()
        {
            // Arrange
            var resolver = new AssertForResolver();
            var specificCandidate = new AssertForCandidate
            {
                DeclaringTypeName = "StringAssertions",
                TargetTypeNames = ["System.String"]  // Concrete type
            };
            var genericCandidate = new AssertForCandidate
            {
                DeclaringTypeName = "StructuralAssertions",
                TargetTypeNames = ["System.Object"]  // Generic fallback
            };
            resolver.RegisterCandidate(specificCandidate);
            resolver.RegisterCandidate(genericCandidate);

            // Act
            var result = resolver.Resolve("System.String", out var isAmbiguous);

            // Assert
            await Check.That(result).IsNotNull();
            await Check.That(result!.DeclaringTypeName).Is("StringAssertions");
            await Check.That(isAmbiguous).IsFalse();
        }

        [Test]
        public async Task AssertForResolver_MultiTarget_RegistersMultiple()
        {
            // Arrange
            var resolver = new AssertForResolver();
            var multiCandidate = new AssertForCandidate
            {
                DeclaringTypeName = "NumberAssertions",
                TargetTypeNames = ["System.Int32", "System.Double"]
            };
            resolver.RegisterCandidate(multiCandidate);

            // Act
            var resultInt = resolver.Resolve("System.Int32", out _);
            var resultDouble = resolver.Resolve("System.Double", out _);

            // Assert
            await Check.That(resultInt).IsNotNull();
            await Check.That(resultDouble).IsNotNull();
            await Check.That(resultInt!.DeclaringTypeName).Is("NumberAssertions");
            await Check.That(resultDouble!.DeclaringTypeName).Is("NumberAssertions");
        }

        #endregion

        #region Specificity Scoring Tests

        [Test]
        public async Task AssertForCandidate_ComputeSpecificityScore_ConcreteTypeScoresHigher()
        {
            // Arrange
            var concrete = new AssertForCandidate
            {
                TargetTypeNames = ["System.String"]
            };
            var generic = new AssertForCandidate
            {
                TargetTypeNames = ["System.Object"]
            };

            // Act
            var concreteScore = concrete.ComputeSpecificityScore();
            var genericScore = generic.ComputeSpecificityScore();

            // Assert
            await Check.That(concreteScore).IsGreaterThan(genericScore);
        }

        [Test]
        public async Task AssertForCandidate_ComputeSpecificityScore_MultiTargetIsMoreSpecific()
        {
            // Arrange
            var single = new AssertForCandidate
            {
                TargetTypeNames = ["System.String"]
            };
            var multi = new AssertForCandidate
            {
                TargetTypeNames = ["System.String", "System.Int32"]
            };

            // Act
            var singleScore = single.ComputeSpecificityScore();
            var multiScore = multi.ComputeSpecificityScore();

            // Assert
            await Check.That(multiScore).IsGreaterThan(singleScore);
        }

        #endregion

        #region Diagnostics Tests

        [Test]
        public async Task AssertForResolver_ReportUnsupportedTransitionType_AddsDiagnostic()
        {
            // Arrange
            var resolver = new AssertForResolver();

            // Act
            resolver.ReportUnsupportedTransitionType("CustomType");

            // Assert
            var diagnostics = resolver.GetDiagnostics().ToList();
            await Check.That(diagnostics.Count).IsGreaterThan(0);
            await Check.That(diagnostics[0].Code).Is("ASRT0003");
        }

        [Test]
        public async Task AssertForResolver_ReportDuplicateSignature_AddsDiagnostic()
        {
            // Arrange
            var resolver = new AssertForResolver();

            // Act
            resolver.ReportDuplicateSignature("Demo.TypeAssertions", "IsConfigured");

            // Assert
            var diagnostics = resolver.GetDiagnostics().ToList();
            await Check.That(diagnostics.Count).IsGreaterThan(0);
            await Check.That(diagnostics[0].Code).Is("ASRT0004");
            await Check.That(diagnostics[0].Message).Contains("Demo.TypeAssertions.IsConfigured");
        }

        [Test]
        public async Task AssertForResolver_ReportMissingBaseType_AddsDiagnostic()
        {
            // Arrange
            var resolver = new AssertForResolver();

            // Act
            resolver.ReportMissingBaseType("Demo.CustomBaseAssertions");

            // Assert
            var diagnostics = resolver.GetDiagnostics().ToList();
            await Check.That(diagnostics.Count).IsGreaterThan(0);
            await Check.That(diagnostics[0].Code).Is("ASRT0005");
            await Check.That(diagnostics[0].Message).Contains("Demo.CustomBaseAssertions");
        }

        [Test]
        public async Task AssertForResolver_VerboseLogging_EmitsTraceLog()
        {
            // Arrange
            var resolver = new AssertForResolver { VerboseLogging = true };
            var candidate = new AssertForCandidate
            {
                DeclaringTypeName = "TestAssertions",
                TargetTypeNames = ["Test"]
            };

            // Act
            resolver.RegisterCandidate(candidate);
            var log = resolver.GetResolverLog();

            // Assert
            await Check.That(log.Length).IsGreaterThan(0);
            await Check.That(log).Contains("RegisterCandidate");
            await Check.That(log).Contains("TestAssertions");
        }

        #endregion

        #region Transition Node Generation Tests

        [Test]
        public async Task TransitionNodeGenerator_GeneratesTransitionNode_WithCorrectName()
        {
            // Arrange & Act
            var output = TransitionNodeGenerator.GenerateTransitionNode(
                targetTypeName: "User",
                propertyName: "Name",
                propertyTypeName: "System.String",
                lazyTransitions: true);

            // Assert
            await Check.That(output.TransitionNodeTypeName).Is("UserNameAssertions");
            await Check.That(output.TransitionNodeTypeCode.Length).IsGreaterThan(0);
            await Check.That(output.TransitionMethodCode.Length).IsGreaterThan(0);
        }

        [Test]
        public async Task TransitionNodeGenerator_LazyTransitions_UsesResolver()
        {
            // Arrange & Act
            var output = TransitionNodeGenerator.GenerateTransitionNode(
                targetTypeName: "User",
                propertyName: "Email",
                propertyTypeName: "System.String",
                lazyTransitions: true);

            // Assert
            await Check.That(output.TransitionNodeTypeCode).Contains("Func");
            await Check.That(output.TransitionNodeTypeCode).Contains("valueResolver");
        }

        #endregion
    }
}



