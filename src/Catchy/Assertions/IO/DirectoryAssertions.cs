using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a directory path.</summary>
        public static ValueAssertions<DirectoryInfo?> ThatDirectory(this Asserter a, string? path,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(path))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatDirectory", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<DirectoryInfo?>(p, path is null ? null : new DirectoryInfo(path));
        }

        /// <summary>Starts assertions for a <see cref="DirectoryInfo"/>.</summary>
        public static ValueAssertions<DirectoryInfo?> That(this Asserter a, DirectoryInfo? dirInfo,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(dirInfo))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<DirectoryInfo?>(p, dirInfo);
        }
    }

    public static class DirectoryAssertExtensions
    {
        /// <summary>Asserts that the directory exists.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> Exists(this ValueAssertions<DirectoryInfo?> a)
        { 
            a.Link("Exists"); 
            a.Op(a => DirectoryChecks.Exists(a.GetValue()?.FullName, a.IsSkipped())); 
            return a; 
        }

        /// <summary>Asserts that the directory does not exist.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> DoesNotExist(this ValueAssertions<DirectoryInfo?> a)
        { 
            a.Link("DoesNotExist"); 
            a.Op(a => DirectoryChecks.DoesNotExist(a.GetValue()?.FullName, a.IsSkipped())); 
            return a; 
        }

        /// <summary>Asserts that the directory is empty (contains no files).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> IsEmpty(this ValueAssertions<DirectoryInfo?> a)
        { 
            a.Link("IsEmpty"); 
            a.Op(a => DirectoryChecks.IsEmpty(a.GetValue()?.FullName, a.IsSkipped())); 
            return a; 
        }

        /// <summary>Asserts that the directory is not empty (contains at least one file).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> IsNotEmpty(this ValueAssertions<DirectoryInfo?> a)
        { 
            a.Link("IsNotEmpty"); 
            a.Op(a => DirectoryChecks.IsNotEmpty(a.GetValue()?.FullName, a.IsSkipped())); 
            return a; 
        }

        /// <summary>Asserts that the directory contains a file with the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasFile(this ValueAssertions<DirectoryInfo?> a, string fileName,
            [CallerArgumentExpression(nameof(fileName))] string? expr = null)
        { 
            a.Link("HasFile", expr); 
            a.Op(a => DirectoryChecks.HasFile(a.GetValue()?.FullName, fileName, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory does not contain a file with the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> DoesNotHaveFile(this ValueAssertions<DirectoryInfo?> a, string fileName,
            [CallerArgumentExpression(nameof(fileName))] string? expr = null)
        { 
            a.Link("DoesNotHaveFile", expr); 
            a.Op(a => DirectoryChecks.DoesNotHaveFile(a.GetValue()?.FullName, fileName, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory contains a subdirectory with the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasSubdirectory(this ValueAssertions<DirectoryInfo?> a, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { 
            a.Link("HasSubdirectory", expr); 
            a.Op(a => DirectoryChecks.HasSubdirectory(a.GetValue()?.FullName, name, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory does not contain a subdirectory with the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> DoesNotHaveSubdirectory(this ValueAssertions<DirectoryInfo?> a, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { 
            a.Link("DoesNotHaveSubdirectory", expr); 
            a.Op(a => DirectoryChecks.DoesNotHaveSubdirectory(a.GetValue()?.FullName, name, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory has exactly the specified number of files.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasFileCount(this ValueAssertions<DirectoryInfo?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { 
            a.Link("HasFileCount", expr); 
            a.Op(a => DirectoryChecks.HasFileCount(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory has more than the specified number of files.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasFileCountGreaterThan(this ValueAssertions<DirectoryInfo?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { 
            a.Link("HasFileCountGreaterThan", expr); 
            a.Op(a => DirectoryChecks.HasFileCountGreaterThan(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory has fewer than the specified number of files.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasFileCountLessThan(this ValueAssertions<DirectoryInfo?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { 
            a.Link("HasFileCountLessThan", expr); 
            a.Op(a => DirectoryChecks.HasFileCountLessThan(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory has the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasName(this ValueAssertions<DirectoryInfo?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { 
            a.Link("HasName", expr); 
            a.Op(a => DirectoryChecks.HasName(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory contains a file matching the given search pattern.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> HasFileMatching(this ValueAssertions<DirectoryInfo?> a, string searchPattern,
            [CallerArgumentExpression(nameof(searchPattern))] string? expr = null)
        { 
            a.Link("HasFileMatching", expr); 
            a.Op(a => DirectoryChecks.HasFileMatching(a.GetValue()?.FullName, searchPattern, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory does not contain a file matching the given search pattern.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> DoesNotHaveFileMatching(this ValueAssertions<DirectoryInfo?> a, string searchPattern,
            [CallerArgumentExpression(nameof(searchPattern))] string? expr = null)
        { 
            a.Link("DoesNotHaveFileMatching", expr); 
            a.Op(a => DirectoryChecks.DoesNotHaveFileMatching(a.GetValue()?.FullName, searchPattern, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory is located within the given parent directory.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> IsInDirectory(this ValueAssertions<DirectoryInfo?> a, string parentPath,
            [CallerArgumentExpression(nameof(parentPath))] string? expr = null)
        { 
            a.Link("IsInDirectory", expr); 
            a.Op(a => DirectoryChecks.IsInDirectory(a.GetValue()?.FullName, parentPath, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Asserts that the directory is not located within the given parent directory.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DirectoryInfo?> IsNotInDirectory(this ValueAssertions<DirectoryInfo?> a, string parentPath,
            [CallerArgumentExpression(nameof(parentPath))] string? expr = null)
        { 
            a.Link("IsNotInDirectory", expr); 
            a.Op(a => DirectoryChecks.IsNotInDirectory(a.GetValue()?.FullName, parentPath, a.IsSkipped(), expr)); 
            return a; 
        }

        /// <summary>Projects the directory's creation time for further assertions.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<DateTime?> CreationTime(this ValueAssertions<DirectoryInfo?> a)
        {
            a.Link("CreationTime");
            var p = a.GetPipeline();
            var creationTime = a.GetValue()?.CreationTime;
            return new ValueAssertions<DateTime?>(p, creationTime);
        }

        /// <summary>Projects the directory's name for further assertions.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Name(this ValueAssertions<DirectoryInfo?> a)
        {
            a.Link("Name");
            var p = a.GetPipeline();
            var name = a.GetValue()?.Name;
            return new ValueAssertions<string>(p, name!);
        }
    }
}

