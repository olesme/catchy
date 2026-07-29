using System.IO.Compression;
using Catchy;
using CatchyCoreTests.Helpers;
using Xunit;

namespace CatchyCoreTests.Assertions.IO
{
    /// <summary>
    /// Integration tests for IO assertions (FileInfo, DirectoryInfo, Stream).
    /// Covers file/directory operations, existence checks, and stream properties.
    /// </summary>
    public class IOAssertionsTests : IAsyncLifetime
    {
        private string _testDir = string.Empty;
        private string _testFile = string.Empty;

        public async ValueTask InitializeAsync()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "CatchyIOTests", Guid.NewGuid().ToString("N"));
            _testFile = Path.Combine(_testDir, "test.txt");
            Directory.CreateDirectory(_testDir);
            await File.WriteAllTextAsync(_testFile, "test content");
        }

        public async ValueTask DisposeAsync()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
            await Task.CompletedTask;
        }

        // ===== FileInfo Assertions =====

        [Fact]
        public async Task FileInfo_Exists_WithExistingFile_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).Exists();
        }

        [Fact]
        public async Task FileInfo_Exists_WithNonExistentFile_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                var fileInfo = new FileInfo(Path.Combine(_testDir, "nonexistent.txt"));
                await Stateless.Assert.That(fileInfo).Exists();
            });
        }

        [Fact]
        public async Task FileInfo_DoesNotExist_WithNonExistentFile_Passes()
        {
            var fileInfo = new FileInfo(Path.Combine(_testDir, "nonexistent.txt"));
            await Stateless.Assert.That(fileInfo).DoesNotExist();
        }

        [Fact]
        public async Task FileInfo_Extension_Is_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).Extension().Is(".txt");
        }

        [Fact]
        public async Task FileInfo_Name_Is_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).Name().Is("test.txt");
        }

        [Fact]
        public async Task FileInfo_Length_IsGreaterThan_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).Length().IsGreaterThan(0);
        }

        [Fact]
        public async Task FileInfo_IsReadOnly_WithReadOnlyFile_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            fileInfo.Attributes |= FileAttributes.ReadOnly;
            await Stateless.Assert.That(fileInfo).IsReadonly();
            fileInfo.Attributes &= ~FileAttributes.ReadOnly;
        }

        [Fact]
        public async Task FileInfo_CreationTime_IsLessThanOrEqual_NowPlus_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).CreationTime().IsLessThan(DateTime.Now.AddSeconds(5));
        }

        [Fact]
        public async Task FileInfo_LastWriteTime_IsLessThanOrEqual_NowPlus_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).LastWriteTime().IsLessThan(DateTime.Now.AddSeconds(5));
        }

        // ===== DirectoryInfo Assertions =====

        [Fact]
        public async Task DirectoryInfo_Exists_WithExistingDirectory_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).Exists();
        }

        [Fact]
        public async Task DirectoryInfo_Exists_WithNonExistentDirectory_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                var dirInfo = new DirectoryInfo(Path.Combine(_testDir, "nonexistent"));
                await Stateless.Assert.That(dirInfo).Exists();
            });
        }

        [Fact]
        public async Task DirectoryInfo_DoesNotExist_WithNonExistentDirectory_Passes()
        {
            var dirInfo = new DirectoryInfo(Path.Combine(_testDir, "nonexistent"));
            await Stateless.Assert.That(dirInfo).DoesNotExist();
        }

        [Fact]
        public async Task DirectoryInfo_Name_Is_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).Name().Is(Path.GetFileName(_testDir));
        }

        [Fact]
        public async Task DirectoryInfo_GetFiles_IsNotEmpty_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var files = dirInfo.GetFiles();
            await Stateless.Assert.That(files).IsNotEmpty();
        }

        [Fact]
        public async Task DirectoryInfo_GetDirectories_IsEmpty_WhenNoSubdirs_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var subdirs = dirInfo.GetDirectories();
            await Stateless.Assert.That(subdirs).IsEmpty();
        }

        [Fact]
        public async Task DirectoryInfo_CreationTime_IsLessThan_NowPlus_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).CreationTime().IsLessThan(DateTime.Now.AddSeconds(5));
        }

        // ===== Stream Assertions =====

        [Fact]
        public async Task Stream_CanRead_WithReadableStream_Passes()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
            await Stateless.Assert.That(stream).CanRead();
        }

        [Fact]
        public async Task Stream_CanWrite_WithWritableStream_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).CanWrite();
        }

        [Fact]
        public async Task Stream_CanSeek_WithMemoryStream_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).CanSeek();
        }

        [Fact]
        public async Task Stream_Length_IsGreaterThan_WithContentStream_Passes()
        {
            using var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).Length().IsGreaterThan(0);
        }

        [Fact]
        public async Task Stream_Position_Is_AtStart_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).Position().Is(0);
        }

        [Fact]
        public async Task Stream_IsClosed_WithClosedStream_Passes()
        {
            var stream = new MemoryStream();
            stream.Close();
            await Stateless.Assert.That(stream).IsClosed();
        }

        // ===== AmbientSoft Mode =====

        [Fact]
        public async Task IO_SoftMode_AccumulatesFailures()
        {
            var verify = Asserter.NewSoft();
            var fileInfo = new FileInfo(_testFile);
            var dirInfo = new DirectoryInfo(_testDir);

            await verify.That(fileInfo).Exists();                          // Pass
            await verify.That(fileInfo).Extension().Is(".pdf");           // Fail
            await verify.That(dirInfo).Exists();                          // Pass
            await verify.That(dirInfo.GetFiles()).IsEmpty();              // Fail

            if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
        }

        [Fact]
        public async Task FileInfo_HasSize_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasSize(12);
        }

        [Fact]
        public async Task FileInfo_HasSizeGreaterThan_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasSizeGreaterThan(5);
        }

        [Fact]
        public async Task FileInfo_HasSizeInRange_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasSizeInRange(10, 20);
        }

        [Fact]
        public async Task FileInfo_IsInDirectory_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).IsInDirectory(_testDir);
        }

        [Fact]
        public async Task FileInfo_HasSize_fails_when_size_differs()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSize(999));
            Assert.Contains("999", msg);
        }

        [Fact]
        public async Task Directory_HasFile_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasFile("test.txt");
        }

        [Fact]
        public async Task Directory_HasFileCount_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasFileCount(1);
        }

        [Fact]
        public async Task Directory_HasFileMatching_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasFileMatching("*test*");
        }

        [Fact]
        public async Task Directory_HasSubdirectory_Passes()
        {
            var subDir = Path.Combine(_testDir, "sub");
            Directory.CreateDirectory(subDir);
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasSubdirectory("sub");
        }

        [Fact]
        public async Task Directory_DoesNotHaveSubdirectory_fails_when_subdir_exists()
        {
            var subDir = Path.Combine(_testDir, "sub2");
            Directory.CreateDirectory(subDir);
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveSubdirectory("sub2"));
            Assert.Contains("sub2", msg);
        }

        [Fact]
        public async Task Stream_IsReadable_Passes()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
            await Stateless.Assert.That(stream).IsReadable();
        }

        [Fact]
        public async Task Stream_IsWritable_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).IsWritable();
        }

        [Fact]
        public async Task Stream_IsSeekable_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).IsSeekable();
        }

        [Fact]
        public async Task Stream_HasLengthInRange_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).HasLengthInRange(1, 5);
        }

        [Fact]
        public async Task Stream_IsAtStart_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).IsAtStart();
        }

        [Fact]
        public async Task Stream_IsAtEnd_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            stream.Position = stream.Length;
            await Stateless.Assert.That(stream).IsAtEnd();
        }

        [Fact]
        public async Task Stream_CannotTimeout_WithMemoryStream_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).CannotTimeout();
        }

        [Fact]
        public async Task Stream_CanTimeout_WithMemoryStream_Throws()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).CanTimeout();
            });
            Assert.Contains("timeout", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_HasName_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasName("test.txt");
        }

        [Fact]
        public async Task FileInfo_HasExtension_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasExtension(".txt");
        }

        [Fact]
        public async Task FileInfo_HasSizeLessThan_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).HasSizeLessThan(100);
        }

        [Fact]
        public async Task FileInfo_IsNotInDirectory_Passes()
        {
            var fileInfo = new FileInfo(_testFile);
            await Stateless.Assert.That(fileInfo).IsNotInDirectory(Path.Combine(_testDir, "child"));
        }

        [Fact]
        public async Task Directory_HasName_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasName(Path.GetFileName(_testDir));
        }

        [Fact]
        public async Task Directory_DoesNotHaveFile_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).DoesNotHaveFile("missing.txt");
        }

        [Fact]
        public async Task Directory_HasFileCountGreaterThan_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasFileCountGreaterThan(0);
        }

        [Fact]
        public async Task Directory_HasFileCountLessThan_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).HasFileCountLessThan(5);
        }

        [Fact]
        public async Task Directory_DoesNotHaveFileMatching_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).DoesNotHaveFileMatching("*.pdf");
        }

        [Fact]
        public async Task Directory_IsInDirectory_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).IsInDirectory(Path.Combine(Path.GetTempPath(), "CatchyIOTests"));
        }

        [Fact]
        public async Task Directory_IsNotInDirectory_Passes()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).IsNotInDirectory(Path.Combine(_testDir, "child"));
        }

        [Fact]
        public async Task Stream_IsNotReadable_Passes()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Write);
            await Stateless.Assert.That(stream).IsNotReadable();
        }

        [Fact]
        public async Task Stream_IsNotWritable_Passes()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
            await Stateless.Assert.That(stream).IsNotWritable();
        }

        [Fact]
        public async Task Stream_IsNotSeekable_Passes()
        {
            using var inner = new MemoryStream();
            using Stream stream = new DeflateStream(inner, CompressionMode.Compress);
            await Stateless.Assert.That(stream).IsNotSeekable();
        }

        [Fact]
        public async Task Stream_IsEmpty_Passes()
        {
            using var stream = new MemoryStream();
            await Stateless.Assert.That(stream).IsEmpty();
        }

        [Fact]
        public async Task Stream_IsNotEmpty_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1 });
            await Stateless.Assert.That(stream).IsNotEmpty();
        }

        [Fact]
        public async Task Stream_HasLength_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).HasLength(3);
        }

        [Fact]
        public async Task Stream_HasLengthGreaterThan_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).HasLengthGreaterThan(2);
        }

        [Fact]
        public async Task Stream_HasLengthLessThan_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).HasLengthLessThan(10);
        }

        [Fact]
        public async Task Stream_IsNotAtStart_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            stream.Position = 1;
            await Stateless.Assert.That(stream).IsNotAtStart();
        }

        [Fact]
        public async Task Stream_IsNotAtEnd_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            await Stateless.Assert.That(stream).IsNotAtEnd();
        }

        [Fact]
        public async Task Stream_HasPosition_Passes()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            stream.Position = 1;
            await Stateless.Assert.That(stream).HasPosition(1);
        }

        [Fact]
        public async Task FileInfo_HasSizeLessThan_Throws_when_size_is_too_large()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSizeLessThan(1));
            Assert.Contains("1", msg);
        }

        [Fact]
        public async Task Directory_DoesNotHaveFile_fails_when_file_exists()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveFile("test.txt"));
            Assert.Contains("test.txt", msg);
        }

        [Fact]
        public async Task Stream_IsNotReadable_Throws_for_readable_stream()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
                await Stateless.Assert.That(stream).IsNotReadable();
            });
            Assert.Contains("read", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_HasSizeInRange_Throws_when_outside_range()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSizeInRange(1, 5));
            Assert.Contains("5", msg);
        }

        [Fact]
        public async Task Directory_IsInDirectory_Throws_when_parent_is_wrong()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).IsInDirectory(Path.Combine(_testDir, "other")));
            Assert.Contains("other", msg);
        }

        [Fact]
        public async Task Directory_DoesNotHaveFileMatching_Throws_when_match_exists()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveFileMatching("*.txt"));
            Assert.Contains("*.txt", msg);
        }

        [Fact]
        public async Task Stream_HasLengthInRange_Throws_when_outside_range()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).HasLengthInRange(10, 20));
            Assert.Contains("10", msg);
        }

        [Fact]
        public async Task FileInfo_HasName_Throws_when_name_differs()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasName("wrong.txt"));
            Assert.Contains("wrong.txt", msg);
        }

        [Fact]
        public async Task Directory_HasFileCount_Throws_when_count_differs()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasFileCount(2));
            Assert.Contains("2", msg);
        }

        [Fact]
        public async Task Stream_IsNotAtEnd_Throws_when_at_end()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
                stream.Position = stream.Length;
                await Stateless.Assert.That(stream).IsNotAtEnd();
            });
            Assert.Contains("end", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_IsNotInDirectory_Throws_when_inside_directory()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).IsNotInDirectory(_testDir));
            Assert.Contains(Path.GetFileName(_testDir), msg);
        }

        [Fact]
        public async Task Directory_IsNotInDirectory_Throws_when_inside_directory()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).IsNotInDirectory(Path.Combine(Path.GetTempPath(), "CatchyIOTests")));
            Assert.Contains("CatchyIOTests", msg);
        }

        [Fact]
        public async Task Stream_IsNotWritable_Throws_for_writable_stream()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).IsNotWritable();
            });
            Assert.Contains("writable", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Directory_IsEmpty_Passes_for_new_directory()
        {
            var emptyDir = Path.Combine(_testDir, "empty");
            Directory.CreateDirectory(emptyDir);
            var dirInfo = new DirectoryInfo(emptyDir);
            await Stateless.Assert.That(dirInfo).IsEmpty();
        }

        [Fact]
        public async Task Directory_IsNotEmpty_Passes_for_directory_with_file()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            await Stateless.Assert.That(dirInfo).IsNotEmpty();
        }

        [Fact]
        public async Task Directory_HasSubdirectory_Throws_when_missing()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasSubdirectory("missing-subdir"));
            Assert.Contains("missing-subdir", msg);
        }

        [Fact]
        public async Task FileInfo_HasExtension_Throws_when_extension_differs_1()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasExtension(".pdf"));
            Assert.Contains(".pdf", msg);
        }

        [Fact]
        public async Task Stream_HasPosition_Throws_when_position_differs()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
                await Stateless.Assert.That(stream).HasPosition(1);
            });
            Assert.Contains("Expected stream to be at position", msg);
        }

        [Fact]
        public async Task FileInfo_HasSizeGreaterThan_Throws_when_size_too_small()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSizeGreaterThan(100));
            Assert.Contains("Expected file size to be > ", msg);
        }

        [Fact]
        public async Task Directory_HasName_Throws_when_name_differs()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasName("other"));
            Assert.Contains("Expected directory name to be", msg);
        }

        [Fact]
        public async Task Stream_HasLengthGreaterThan_Throws_when_length_too_small()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).HasLengthGreaterThan(10));
            Assert.Contains("Expected stream to have length > ", msg);
        }

        [Fact]
        public async Task Directory_HasFileCountGreaterThan_Throws_when_count_too_small()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasFileCountGreaterThan(5));
            Assert.Contains("Expected directory to have > ", msg);
        }

        [Fact]
        public async Task Directory_HasFileCountLessThan_Throws_when_count_too_large()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasFileCountLessThan(1));
            Assert.Contains("Expected directory to have < ", msg);
        }

        [Fact]
        public async Task Stream_IsEmpty_Throws_when_stream_has_content()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream(new byte[] { 1 });
                await Stateless.Assert.That(stream).IsEmpty();
            });
            Assert.Contains("empty", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_IsNotEmpty_Throws_when_stream_is_empty()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).IsNotEmpty();
            });
            Assert.Contains("empty", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_DoesNotExist_Throws_when_file_exists()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).DoesNotExist());
            Assert.Contains(_testFile, msg);
        }

        [Fact]
        public async Task Directory_Exists_Throws_when_directory_missing()
        {
            var dirInfo = new DirectoryInfo(Path.Combine(_testDir, "missing"));
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).Exists());
            Assert.Contains("missing", msg);
        }

        [Fact]
        public async Task Stream_CanTimeout_Throws_for_non_timeout_stream()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).CanTimeout();
            });
            Assert.Contains("timeout", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_IsReadonly_Throws_when_file_is_not_readonly()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).IsReadonly());
            Assert.Contains("readonly", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Directory_DoesNotHaveSubdirectory_Throws_when_subdirectory_exists()
        {
            var subDir = Path.Combine(_testDir, "sub-keep");
            Directory.CreateDirectory(subDir);
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveSubdirectory("sub-keep"));
            Assert.Contains("sub-keep", msg);
        }

        [Fact]
        public async Task Stream_IsClosed_Throws_when_stream_is_open()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).IsClosed();
            });
            Assert.Contains("closed", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_CanRead_Throws_when_stream_cannot_read()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Write);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).CanRead());
            Assert.Contains("read", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_CanWrite_Throws_when_stream_cannot_write()
        {
            using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).CanWrite());
            Assert.Contains("writable", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_CanSeek_Throws_when_stream_cannot_seek()
        {
            using var inner = new MemoryStream();
            using Stream stream = new DeflateStream(inner, CompressionMode.Compress);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).CanSeek());
            Assert.Contains("seek", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Directory_DoesNotHaveFileMatching_Throws_when_file_matches()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveFileMatching("*.txt"));
            Assert.Contains("*.txt", msg);
        }

        [Fact]
        public async Task Stream_IsNotReadable_Throws_when_stream_is_readable()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new FileStream(_testFile, FileMode.Open, FileAccess.Read);
                await Stateless.Assert.That(stream).IsNotReadable();
            });
            Assert.Contains("read", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_IsNotSeekable_Throws_when_stream_is_seekable()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream();
                await Stateless.Assert.That(stream).IsNotSeekable();
            });
            Assert.Contains("seek", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Stream_IsNotAtStart_Throws_when_stream_is_at_start()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
                await Stateless.Assert.That(stream).IsNotAtStart();
            });
            Assert.Contains("start", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_HasSizeLessThan_Throws_when_size_is_not_small_enough()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSizeLessThan(1));
            Assert.Contains("1", msg);
        }

        [Fact]
        public async Task Directory_HasFile_Throws_when_file_is_missing()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasFile("missing.txt"));
            Assert.Contains("missing.txt", msg);
        }

        [Fact]
        public async Task Stream_HasLength_Throws_when_length_differs()
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(stream).HasLength(10));
            Assert.Contains("10", msg);
        }

        [Fact]
        public async Task Directory_IsNotEmpty_Throws_when_directory_is_empty()
        {
            var emptyDir = Path.Combine(_testDir, "empty-again");
            Directory.CreateDirectory(emptyDir);
            var dirInfo = new DirectoryInfo(emptyDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).IsNotEmpty());
            Assert.Contains("empty", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task FileInfo_HasExtension_Throws_when_extension_differs_2()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasExtension(".pdf"));
            Assert.Contains(".pdf", msg);
        }

        [Fact]
        public async Task Stream_IsAtStart_Throws_when_stream_is_not_at_start()
        {
            var msg = await Catch.FailureOf(async () =>
            {
                using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
                stream.Position = 1;
                await Stateless.Assert.That(stream).IsAtStart();
            });
            Assert.Contains("start", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task Directory_HasFileMatching_Throws_when_file_matches()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).HasFileMatching("*.xyz"));
            Assert.Contains("*.xyz", msg);
        }

        [Fact]
        public async Task Directory_DoesNotHaveFile_Throws_when_file_exists()
        {
            var dirInfo = new DirectoryInfo(_testDir);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(dirInfo).DoesNotHaveFile("test.txt"));
            Assert.Contains("test.txt", msg);
        }

        [Fact]
        public async Task FileInfo_HasSizeInRange_Throws_when_range_excludes_size()
        {
            var fileInfo = new FileInfo(_testFile);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(fileInfo).HasSizeInRange(1, 5));
            Assert.Contains("5", msg);
        }
    }
}









