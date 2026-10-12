Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.slf4j

Namespace JavaCompat

    ''' <summary>Tests for the Java compatibility helpers (java.util.Arrays, String.format, Math, System).</summary>
    <TestClass>
    Public Class JavaCompatTest

        <TestMethod>
        Public Sub Arrays_CopyOf_PadsAndTruncatesLikeJava()
            Dim src = New Integer() {1, 2, 3}
            assertArrayEquals(New Integer() {1, 2, 3, 0, 0}, Arrays.CopyOf(src, 5))
            assertArrayEquals(New Integer() {1, 2}, Arrays.CopyOf(src, 2))
            assertArrayEquals(New Integer() {}, Arrays.CopyOf(src, 0))
            assertArrayEquals(New Integer() {2, 3, 0}, Arrays.CopyOfRange(src, 1, 4))
            assertThrows(Of ArgumentException)(Sub() Arrays.CopyOfRange(src, 2, 1))
        End Sub

        <TestMethod>
        Public Sub Arrays_ToString_UsesJavaFormatting()
            assertEquals("[1, 2, 3]", Arrays.toString(New Integer() {1, 2, 3}))
            assertEquals("[1.0, 2.5]", Arrays.toString(New Double() {1.0, 2.5}))
            assertEquals("[true, false]", Arrays.toString(New Boolean() {True, False}))
            assertEquals("[a, null]", Arrays.toString(New String() {"a", Nothing}))
            assertEquals("null", Arrays.toString(DirectCast(Nothing, Integer())))
            assertEquals("[[1, 2], [3]]", Arrays.toString(New Integer()() {New Integer() {1, 2}, New Integer() {3}}))
            assertEquals("NaN", Arrays.FormatElement(Double.NaN))
            assertEquals("1.0E10", Arrays.FormatElement(1.0E10).Replace("E+", "E"))
        End Sub

        <TestMethod>
        Public Sub Arrays_HashCodeAndEquals_MatchJava()
            ' java.util.Arrays.hashCode(new int[]{1, 2, 3}) == 30817
            assertEquals(30817, Arrays.hashCode(New Integer() {1, 2, 3}))
            assertEquals(1, Arrays.hashCode(New Integer() {}))
            assertEquals(0, Arrays.hashCode(DirectCast(Nothing, Integer())))
            assertTrue(Arrays.equals(New Integer() {1, 2}, New Integer() {1, 2}))
            assertFalse(Arrays.equals(New Integer() {1, 2}, New Integer() {2, 1}))
            assertTrue(Arrays.equals(DirectCast(Nothing, Integer()), DirectCast(Nothing, Integer())))
            Dim filled(3) As Integer
            Arrays.Fill(filled, 7)
            assertArrayEquals(New Integer() {7, 7, 7, 7}, filled)
            Arrays.Fill(filled, 1, 3, 0)
            assertArrayEquals(New Integer() {7, 0, 0, 7}, filled)
            assertEquals(Integer.MinValue, Arrays.WrapToInt32(2147483648L))
            assertEquals(-1, Arrays.WrapToInt32(&HFFFFFFFFL))
        End Sub

        <TestMethod>
        Public Sub JavaString_Format_SupportsCommonSpecifiers()
            assertEquals("x=5, y=hello", JavaString.format("x=%d, y=%s", 5, "hello"))
            assertEquals("0042", JavaString.format("%04d", 42))
            assertEquals("-042", JavaString.format("%04d", -42))
            assertEquals("   7", JavaString.format("%4d", 7))
            assertEquals("7   |", JavaString.format("%-4d|", 7))
            assertEquals("1,234,567", JavaString.format("%,d", 1234567))
            assertEquals("+5", JavaString.format("%+d", 5))
            assertEquals("3.141593", JavaString.format("%f", Math.PI))
            assertEquals("3.14", JavaString.format("%.2f", Math.PI))
            assertEquals("1.234560e+01", JavaString.format("%e", 12.3456))
            assertEquals("ff", JavaString.format("%x", 255))
            assertEquals("FF", JavaString.format("%X", 255))
            assertEquals("ffffffff", JavaString.format("%x", -1))
            assertEquals("true/false", JavaString.format("%b/%b", True, Nothing))
            assertEquals("50%", JavaString.format("%d%%", 50))
            assertEquals("a" & Environment.NewLine & "b", JavaString.format("a%nb"))
            assertEquals("b a", JavaString.format("%2$s %1$s", "a", "b"))
            assertEquals("null", JavaString.format("%s", DirectCast(Nothing, Object)))
            assertEquals("1.0", JavaString.format("%s", 1.0))
            assertEquals("abc", JavaString.format("%.3s", "abcdef"))
            assertEquals("ABC", JavaString.format("%S", "abc"))
            assertEquals("x", JavaString.format("%c", "x"c))
            assertThrows(Of FormatException)(Sub() JavaString.format("%d %d", 1))
        End Sub

        <TestMethod>
        Public Sub JavaString_DatePattern_ConvertsJavaLetters()
            assertEquals("mm\:ss\.fff", JavaString.ConvertJavaDatePattern("mm:ss.SSS"))
            assertEquals("yyyyMMdd'_'HHmmss\.fff", JavaString.ConvertJavaDatePattern("yyyyMMdd'_'HHmmss.SSS"))
            Dim d As New DateTime(2024, 3, 5, 14, 7, 9, 123)
            assertEquals("07:09.123", d.ToString(JavaString.ConvertJavaDatePattern("mm:ss.SSS")))
        End Sub

        <TestMethod>
        Public Sub MessageFormatter_ReplacesBracePlaceholders()
            assertEquals("a=1, b=two", MessageFormatter.format("a={}, b={}", 1, "two"))
            assertEquals("missing {}", MessageFormatter.format("missing {}"))
            assertEquals("extra 1", MessageFormatter.format("extra {}", 1, 2))
            assertEquals("escaped {} 1", MessageFormatter.format("escaped \{} {}", 1))
            assertEquals("arr=[1, 2]", MessageFormatter.format("arr={}", New Integer() {1, 2}))
            Dim ex As New InvalidOperationException("boom")
            Dim s = MessageFormatter.format("failed: {}", "x", ex)
            assertTrue(s.StartsWith("failed: x"))
            assertTrue(s.Contains("boom"))
        End Sub

        <TestMethod>
        Public Sub MathHelper_Log1pAndExpm1_AreAccurateForSmallValues()
            assertEquals(1.0E-10, MathHelper.Log1p(1.0E-10), 1.0E-20)
            assertEquals(1.0E-10, MathHelper.Expm1(1.0E-10), 1.0E-20)
            assertEquals(Math.Log(2.0), MathHelper.Log1p(1.0), 1.0E-12)
            assertEquals(Math.E - 1, MathHelper.Expm1(1.0), 1.0E-12)
            assertEquals(0.0, MathHelper.Log1p(0.0), 0.0)
            For i As Integer = 0 To 100
                Dim r = MathHelper.NextDouble
                assertTrue(r >= 0.0 AndAlso r < 1.0)
            Next
        End Sub

        <TestMethod>
        Public Sub JavaSystem_Properties_RoundTrip()
            assertNull(JavaSystem.getProperty("dl4vb.test.missing"))
            assertEquals("fallback", JavaSystem.getProperty("dl4vb.test.missing", "fallback"))
            assertNull(JavaSystem.setProperty("dl4vb.test.key", "v1"))
            assertEquals("v1", JavaSystem.setProperty("dl4vb.test.key", "v2"))
            assertEquals("v2", JavaSystem.getProperty("dl4vb.test.key"))
            assertEquals("v2", JavaSystem.clearProperty("dl4vb.test.key"))
            assertNull(JavaSystem.getProperty("dl4vb.test.key"))
            assertEquals(IO.Path.GetTempPath(), JavaSystem.getProperty("java.io.tmpdir"))
            assertEquals(Environment.NewLine, JavaSystem.getProperty("line.separator"))
            Dim t0 = JavaSystem.currentTimeMillis()
            assertTrue(t0 > 1600000000000L)
            Dim n0 = JavaSystem.nanoTime()
            Dim n1 = JavaSystem.nanoTime()
            assertTrue(n1 >= n0)
        End Sub

        <TestMethod>
        Public Sub NumberFormatException_IsAnArgumentException()
            Dim ex As Exception = NumberFormatException.ForInputString("abc", Nothing)
            assertTrue(TypeOf ex Is ArgumentException)
            assertEquals("For input string: ""abc""", ex.Message)
        End Sub

    End Class

End Namespace
