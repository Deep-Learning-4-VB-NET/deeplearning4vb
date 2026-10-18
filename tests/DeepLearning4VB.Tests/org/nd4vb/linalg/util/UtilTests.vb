Imports System
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.util

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' The array-free helpers of <c>linalg.util</c>: <c>LongUtils</c>, <c>HashUtil</c> (values computed with Java's
    ''' 64-bit arithmetic), <c>AtomicThrowable</c>, <c>Nd4vbValidator.validateNumpyTxtFile</c>,
    ''' <c>ND4VBTestUtils</c>'s argument checks and <c>NDArrayPreconditionsFormat</c>'s tags.
    ''' </summary>
    <TestClass>
    Public Class UtilTests

        <TestMethod>
        Public Sub testLongUtils()
            assertArrayEquals(New Integer() {1, 5, -1}, LongUtils.toInts(New Long() {1, (1L << 33) + 5, &HFFFFFFFFL}))
            assertArrayEquals(New Long() {1, -2, Integer.MaxValue}, LongUtils.toLongs(New Integer() {1, -2, Integer.MaxValue}))
            assertEquals(0, LongUtils.toInts(New Long() {}).Length)
        End Sub

        <TestMethod>
        Public Sub testHashUtilMatchesJava()
            assertEquals(-4953706369002393500L, HashUtil.getLongHash(New SByte() {}))
            assertEquals(-4953706369002393500L, HashUtil.getLongHash(""))
            assertEquals(403558661585640082L, HashUtil.getLongHash(New SByte() {1, -1, 127, -128, 0}))
            assertEquals(-1273651058132380003L, HashUtil.getLongHash("abc"))
            assertEquals(774570076111807812L, HashUtil.getLongHash(ChrW(&H20AC) & "1"))
            assertNotEquals(HashUtil.getLongHash("abc"), HashUtil.getLongHash("abd"))
            assertThrows(Of ArgumentNullException)(Sub() HashUtil.getLongHash(DirectCast(Nothing, String)))
        End Sub

        <TestMethod>
        Public Sub testAtomicThrowable()
            Dim t As New AtomicThrowable()
            assertNull(t.get())
            assertFalse(t.Triggered)

            Dim first As New InvalidOperationException("first")
            Dim second As New ArgumentException("second")
            t.setIfFirst(first)
            t.setIfFirst(second)
            assertTrue(t.get() Is first)
            assertTrue(t.Triggered)
            t.set(second)
            assertTrue(t.get() Is second)
            t.set(Nothing)
            assertFalse(t.Triggered)

            assertTrue(New AtomicThrowable(first).Triggered)

            ' exactly one of many concurrent setIfFirst calls wins
            Dim holder As New AtomicThrowable()
            Dim errors(63) As System.Exception
            For i As Integer = 0 To errors.Length - 1
                errors(i) = New System.Exception("e" & i)
            Next i
            Parallel.For(0, errors.Length, Sub(i) holder.setIfFirst(errors(i)))
            assertTrue(Array.IndexOf(errors, holder.get()) >= 0)
        End Sub

        <TestMethod>
        Public Sub testValidateNumpyTxtFile()
            Dim dir As String = Path.Combine(Path.GetTempPath(), "dl4vb_util_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(dir)
            Try
                Dim good As String = Path.Combine(dir, "good.txt")
                File.WriteAllText(good, "1 2 3" & vbLf & "4.5 -5 6e2" & vbLf, Encoding.UTF8)
                Dim vr = Nd4vbValidator.validateNumpyTxtFile(good, " ", Encoding.UTF8)
                assertTrue(vr.isValid())
                assertEquals("Numpy text file", vr.getFormatType())

                Dim ragged As String = Path.Combine(dir, "ragged.txt")
                File.WriteAllText(ragged, "1 2 3" & vbLf & "4 5" & vbLf, Encoding.UTF8)
                vr = Nd4vbValidator.validateNumpyTxtFile(ragged, " ", Encoding.UTF8)
                assertFalse(vr.isValid())
                assertTrue(vr.getIssues()(0).Contains("Number of values in each line is not the same"))

                Dim notNumbers As String = Path.Combine(dir, "words.txt")
                File.WriteAllText(notNumbers, "1,x,3" & vbLf, Encoding.UTF8)
                vr = Nd4vbValidator.validateNumpyTxtFile(notNumbers, ",", Encoding.UTF8)
                assertFalse(vr.isValid())
                assertTrue(TypeOf vr.getException() Is FormatException)

                ' the delimiter is a regular expression, as in Java
                Dim tabs As String = Path.Combine(dir, "tabs.txt")
                File.WriteAllText(tabs, "1" & vbTab & " 2" & vbLf & "3  " & vbTab & "4" & vbLf, Encoding.UTF8)
                assertTrue(Nd4vbValidator.validateNumpyTxtFile(tabs, "\s+", Encoding.UTF8).isValid())

                assertFalse(Nd4vbValidator.validateNumpyTxtFile(Path.Combine(dir, "missing.txt"), " ", Encoding.UTF8).isValid())
                assertThrows(Of ArgumentNullException)(Sub() Nd4vbValidator.validateNumpyTxtFile(good, Nothing, Encoding.UTF8))
                assertThrows(Of NotSupportedException)(Sub() Nd4vbValidator.validateSameDiffFlatBuffers(good))
            Finally
                Directory.Delete(dir, True)
            End Try
        End Sub

        <TestMethod>
        Public Sub testValidateSerializedArraysNeedsFiles()
            Dim dir1 As String = Path.Combine(Path.GetTempPath(), "dl4vb_util_" & Guid.NewGuid().ToString("N"))
            Dim dir2 As String = dir1 & "_2"
            Directory.CreateDirectory(dir1)
            Directory.CreateDirectory(dir2)
            Try
                Dim e As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() ND4VBTestUtils.validateSerializedArrays(dir1, dir2, True))
                assertTrue(e.Message.StartsWith("No files found for directory 1"))
            Finally
                Directory.Delete(dir1, True)
                Directory.Delete(dir2, True)
            End Try
        End Sub

        <TestMethod>
        Public Sub testPreconditionsFormatTags()
            Dim f As New NDArrayPreconditionsFormat()
            assertEquals(New String() {"%ndRank", "%ndShape", "%ndStride", "%ndLength", "%ndSInfo", "%nd10"}, f.formatTags())
            assertEquals("null", f.format("%ndShape", Nothing))
        End Sub

    End Class

End Namespace
