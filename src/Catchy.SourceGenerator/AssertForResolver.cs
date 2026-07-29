using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Catchy.SourceGenerator
{
    /// <summary>
    /// Resolves which AssertFor candidates apply to a given target type,
    /// enforces deterministic precedence, and reports ambiguities.
    /// </summary>
    public class AssertForResolver
    {
        private readonly Dictionary<string, List<AssertForCandidate>> _candidatesByTarget = [];
        private readonly List<GeneratorDiagnostic> _diagnostics = [];
        private readonly StringBuilder _resolverLog = new();

        /// <summary>
        /// If true, emit verbose resolution trace logs.
        /// </summary>
        public bool VerboseLogging { get; set; } = false;

        /// <summary>
        /// Registers a candidate AssertFor from source metadata.
        /// </summary>
        public void RegisterCandidate(AssertForCandidate candidate)
        {
            foreach (var targetTypeName in candidate.TargetTypeNames)
            {
                if (!_candidatesByTarget.ContainsKey(targetTypeName))
                {
                    _candidatesByTarget[targetTypeName] = [];
                }
                _candidatesByTarget[targetTypeName].Add(candidate);
            }

            if (VerboseLogging)
            {
                _resolverLog.AppendLine(
                    $"[RegisterCandidate] {candidate.DeclaringTypeName} " +
                    $"targets: {string.Join(", ", candidate.TargetTypeNames)}");
            }
        }

        /// <summary>
        /// Resolves the winning candidate for a target type.
        /// Returns null if no candidates or if ambiguous (with diagnostic).
        /// </summary>
        public AssertForCandidate? Resolve(string targetTypeName, out bool isAmbiguous)
        {
            isAmbiguous = false;

            if (!_candidatesByTarget.TryGetValue(targetTypeName, out var candidates))
            {
                if (VerboseLogging)
                    _resolverLog.AppendLine($"[Resolve] No candidates for {targetTypeName}");
                return null;
            }

            if (VerboseLogging)
                _resolverLog.AppendLine($"[Resolve] {candidates.Count} candidate(s) for {targetTypeName}");

            if (candidates.Count == 1)
            {
                if (VerboseLogging)
                    _resolverLog.AppendLine($"[Resolve] Single candidate: {candidates[0].DeclaringTypeName}");
                return candidates[0];
            }

            // Multiple candidates: apply resolution rules
            var resolved = ResolveByPrecedence(candidates, targetTypeName);

            if (resolved.Count > 1)
            {
                isAmbiguous = true;
                var ambiguousDeclarations = string.Join(", ",
                    resolved.Select(c => c.DeclaringTypeName));
                _diagnostics.Add(new GeneratorDiagnostic
                {
                    Code = "ASRT0001",
                    Severity = DiagnosticSeverity.Error,
                    Message = $"Ambiguous AssertFor targets for {targetTypeName}. " +
                              $"Candidates: {ambiguousDeclarations}. " +
                              "Use explicit target typing or remove overlapping AssertFor declarations."
                });
                if (VerboseLogging)
                    _resolverLog.AppendLine($"[Resolve] AMBIGUOUS: {ambiguousDeclarations}");
                return null;
            }

            var winner = resolved.FirstOrDefault();
            if (VerboseLogging && winner != null)
                _resolverLog.AppendLine($"[Resolve] Winner: {winner.DeclaringTypeName} (specificity={winner.ComputeSpecificityScore()})");

            return winner;
        }

        /// <summary>
        /// Applies deterministic precedence rules to narrow down candidates.
        /// </summary>
        private List<AssertForCandidate> ResolveByPrecedence(
            List<AssertForCandidate> candidates,
            string targetTypeName)
        {
            if (VerboseLogging)
                _resolverLog.AppendLine($"[ResolveByPrecedence] Starting with {candidates.Count} candidates for {targetTypeName}");

            // Rule 1: Sort by specificity (closed > open generic, specific > object)
            var bySpecificity = candidates
                .OrderByDescending(c => c.ComputeSpecificityScore())
                .ToList();

            if (bySpecificity.Count == 1)
                return bySpecificity;

            var maxSpecificity = bySpecificity[0].ComputeSpecificityScore();
            var topBySpecificity = bySpecificity.Where(c => c.ComputeSpecificityScore() == maxSpecificity).ToList();

            if (VerboseLogging)
                _resolverLog.AppendLine(
                    $"[ResolveByPrecedence] After specificity filtering: {topBySpecificity.Count} " +
                    $"candidate(s) with score={maxSpecificity}");

            if (topBySpecificity.Count == 1)
                return topBySpecificity;

            // Rule 2: Stable deterministic fallback (lexicographic by declaring type name)
            var final = topBySpecificity
                .OrderBy(c => c.DeclaringTypeName)
                .ToList();

            if (VerboseLogging)
                _resolverLog.AppendLine(
                    $"[ResolveByPrecedence] Lexicographic ordering retained {final.Count} candidate(s) for ambiguity reporting");

            return final;
        }

        /// <summary>
        /// Returns all diagnostics accumulated during resolution.
        /// </summary>
        public IEnumerable<GeneratorDiagnostic> GetDiagnostics() => _diagnostics;

        /// <summary>
        /// Returns the verbose resolution log (if enabled).
        /// </summary>
        public string GetResolverLog() => _resolverLog.ToString();

        /// <summary>
        /// Emits a diagnostic for unsupported transition type.
        /// </summary>
        public void ReportUnsupportedTransitionType(string transitionTypeName)
        {
            _diagnostics.Add(new GeneratorDiagnostic
            {
                Code = "ASRT0003",
                Severity = DiagnosticSeverity.Error,
                Message = $"Unsupported transition type: {transitionTypeName}. " +
                          "Cannot infer transition markers from this type."
            });
            if (VerboseLogging)
                _resolverLog.AppendLine($"[Diagnostic] ASRT0003: Unsupported transition type {transitionTypeName}");
        }

        /// <summary>
        /// Emits a diagnostic for duplicate generated signature.
        /// </summary>
        public void ReportDuplicateSignature(string typeName, string methodName)
        {
            _diagnostics.Add(new GeneratorDiagnostic
            {
                Code = "ASRT0004",
                Severity = DiagnosticSeverity.Warning,
                Message = $"AssertFor duplicate signature generated: {typeName}.{methodName}. " +
                          "Prior assignment will be shadowed."
            });
            if (VerboseLogging)
                _resolverLog.AppendLine($"[Diagnostic] ASRT0004: Duplicate signature {typeName}.{methodName}");
        }

        /// <summary>
        /// Emits a diagnostic for missing base type in ExtensionsOnly mode.
        /// </summary>
        public void ReportMissingBaseType(string baseTypeName)
        {
            _diagnostics.Add(new GeneratorDiagnostic
            {
                Code = "ASRT0005",
                Severity = DiagnosticSeverity.Error,
                Message = $"AssertFor mode=ExtensionsOnly requires existing base type {baseTypeName}. " +
                          "Base not found. Create base type first."
            });
            if (VerboseLogging)
                _resolverLog.AppendLine($"[Diagnostic] ASRT0005: Missing base type {baseTypeName}");
        }

        /// <summary>
        /// Clears all accumulated diagnostics and logs.
        /// </summary>
        public void Reset()
        {
            _diagnostics.Clear();
            _resolverLog.Clear();
        }
    }

    /// <summary>
    /// Represents a single AssertFor declaration that has been parsed from source.
    /// </summary>
    public class AssertForCandidate
    {
        /// <summary>
        /// FQN of the class declaring the [AssertFor] attribute.
        /// </summary>
        public string DeclaringTypeName { get; set; } = "";

        /// <summary>
        /// List of target type names this AssertFor applies to.
        /// </summary>
        public List<string> TargetTypeNames { get; set; } = [];

        /// <summary>
        /// Generation mode: EntryPoint or ExtensionsOnly.
        /// </summary>
        public string Mode { get; set; } = "EntryPoint";

        /// <summary>
        /// If true, generate typed property transitions.
        /// </summary>
        public bool GenerateTransitions { get; set; } = true;

        /// <summary>
        /// If true, transitions use lazy resolvers (no eager property reads).
        /// </summary>
        public bool LazyTransitions { get; set; } = true;

        /// <summary>
        /// Diagnostic codes to suppress.
        /// </summary>
        public List<string> SuppressDiagnostics { get; set; } = [];

        /// <summary>
        /// Computes a specificity score for this candidate.
        /// Higher score = more specific (wins in precedence).
        /// </summary>
        public int ComputeSpecificityScore()
        {
            // Score breakdown:
            // - 1000 if all targets are concrete (non-generic, non-object)
            // - 500 if any targets are generic
            // - 100 if 'object' target
            // - +50 for each target beyond first (explicit multi-target is more specific than fallback)

            int score = 0;

            if (TargetTypeNames.Count == 0)
                return 0;

            bool hasObject = TargetTypeNames.Contains("System.Object");
            bool hasGeneric = TargetTypeNames.Any(t => t.Contains("<"));
            bool allConcrete = TargetTypeNames.All(t => t != "System.Object" && !t.Contains("<"));

            if (allConcrete)
                score += 1000;
            else if (hasGeneric)
                score += 500;
            else if (hasObject)
                score += 100;

            score += (TargetTypeNames.Count - 1) * 50;

            return score;
        }
    }

    /// <summary>
    /// Represents a diagnostic message from the generator.
    /// </summary>
    public class GeneratorDiagnostic
    {
        public string Code { get; set; } = "";
        public DiagnosticSeverity Severity { get; set; } = DiagnosticSeverity.Info;
        public string Message { get; set; } = "";
    }

}
