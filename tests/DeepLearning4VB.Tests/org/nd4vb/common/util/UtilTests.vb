Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Numerics
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.org.nd4vb.common.config
Imports DeepLearning4VB.org.nd4vb.common.util

Namespace org.nd4vb.common.util

    <TestClass>
    Public Class LinkedMultiValueMapTest

        <TestMethod>
        Public Sub AddSetFirstAndSingleValueView()
            Dim m As New LinkedMultiValueMap(Of String, Integer)()
            m.add("a", 1)
            m.add("a", 2)
            m.add("b", 3)
            assertEquals(2, m.size())
            assertEquals(1, m.getFirst("a"))
            assertEquals(New Integer() {1, 2}, m("a").ToArray())
            assertEquals(0, m.getFirst("missing"))
            assertNull(m.[get]("missing"))
            m.[set]("a", 9)
            assertEquals(New Integer() {9}, m("a").ToArray())
            Dim singleView = m.toSingleValueMap()
            assertEquals(9, singleView("a"))
            assertEquals(3, singleView("b"))
            assertEquals(New String() {"a", "b"}, m.Keys.ToArray())
            m.setAll(New Dictionary(Of String, Integer) From {{"c", 4}})
            assertEquals(4, m.getFirst("c"))
            assertTrue(m.ContainsKey("c"))
            assertTrue(m.Remove("c"))
            assertEquals(2, m.Count)
            Dim copy As New LinkedMultiValueMap(Of String, Integer)(m)
            assertEquals(m, copy)
            m.Clear()
            assertTrue(m.isEmpty())
        End Sub

    End Class

    <TestClass>
    Public Class RationalTest

        <TestMethod>
        Public Sub ArithmeticIsNormalised()
            Dim half As New Rational(2, 4)
            assertEquals("1/2", half.ToString())
            assertEquals(New Rational(1, 2), half)
            assertEquals(New Rational(3, 4), half.add(New Rational(1, 4)))
            assertEquals(New Rational(1, 4), half.subtract(New Rational(1, 4)))
            assertEquals(New Rational(1, 6), half.multiply(New Rational(1, 3)))
            assertEquals(New Rational(3, 2), half.divide(New Rational(1, 3)))
            assertEquals(New Rational(1, 8), half.pow(3))
            assertEquals(New Rational(8, 1), half.pow(-3))
            assertEquals(Rational.ONE, half.pow(0))
            assertEquals(New Rational(-1, 2), half.negate())
            assertEquals(New Rational(1, 2), New Rational(-1, -2))
            assertEquals(New Rational(-1, 2), New Rational(1, -2))
            assertEquals(0.5, half.doubleValue(), 1.0E-15)
            assertEquals(0.5F, half.floatValue(), 0.0F)
            assertEquals(0.5D, half.BigDecimalValue())
            assertEquals(-1, half.compareTo(Rational.ONE))
            assertEquals(1, half.compareTo(Rational.ZERO))
            assertEquals(0, half.compareTo(New Rational(5, 10)))
            assertEquals(half, half.min(Rational.ONE))
            assertEquals(Rational.ONE, half.max(Rational.ONE))
            assertEquals(New BigInteger(2), New Rational(7, 3).trunc())
            assertEquals(New BigInteger(-3), New Rational(-7, 3).floor())
            assertEquals(New BigInteger(2), New Rational(7, 3).floor())
            assertEquals(New Rational(7, 3), New Rational(-7, 3).abs())
            assertThrows(Of DivideByZeroException)(Function() New Rational(1, 0))
        End Sub

        <TestMethod>
        Public Sub ParsingBinomialAndPochhammer()
            assertEquals(New Rational(3, 7), New Rational("6/14"))
            assertEquals(New Rational(5, 1), New Rational("5"))
            assertEquals(New Rational(255, 1), New Rational("ff", 16))
            assertEquals(New Rational(-5, 2), New Rational("-101/10", 2))
            assertEquals(New Rational(10, 1), Rational.binomial(New Rational(5), 2))
            assertEquals(New Rational(10, 1), Rational.binomial(New Rational(5), New BigInteger(3)))
            assertEquals(Rational.ONE, Rational.binomial(New Rational(5), 0))
            ' (3)_3 = 3 * 4 * 5
            assertEquals(New Rational(60), New Rational(3).Pochhammer(3))
            assertEquals(Rational.ONE, New Rational(3).Pochhammer(0))
            assertNull(New Rational(3).Pochhammer(-1))
            assertEquals("0.333", New Rational(1, 3).toFString(3))
            assertEquals("7", New Rational(7).toFString(3))
            assertEquals(New Rational(1, 3), New Rational(1, 3).clone())
        End Sub

        <TestMethod>
        Public Sub BernoulliAndFactorial()
            Dim b As New Bernoulli()
            assertEquals(Rational.ONE, b.at(0))
            assertEquals(New Rational(-1, 2), b.at(1))
            assertEquals(New Rational(1, 6), b.at(2))
            assertEquals(Rational.ZERO, b.at(3))
            assertEquals(New Rational(-1, 30), b.at(4))
            assertEquals(New Rational(1, 42), b.at(6))
            assertEquals(New Rational(-1, 30), b.at(8))
            assertEquals(New Rational(5, 66), b.at(10))
            assertEquals(New Rational(-691, 2730), b.at(12))
            assertThrows(Of ArgumentOutOfRangeException)(Function() b.at(-1))

            Dim f As New Factorial()
            assertEquals(BigInteger.One, f.at(0))
            assertEquals(New BigInteger(120), f.at(5))
            assertEquals(BigInteger.Parse("2432902008176640000"), f.at(20))
        End Sub

    End Class

    <TestClass>
    Public Class MathUtilsTest

        <TestMethod>
        Public Sub BasicStatisticsAndDistances()
            assertEquals(2.5, MathUtils.mean(New Double() {1, 2, 3, 4}), 1.0E-12)
            assertEquals(4.0, MathUtils.max(New Double() {1, 4, 3}), 0.0)
            assertEquals(1.0, MathUtils.min(New Double() {1, 4, 3}), 0.0)
            assertEquals(1, MathUtils.maxIndex(New Double() {1, 4, 3}))
            ' the Java original returns the *squared* euclidean distance
            assertEquals(25.0, MathUtils.euclideanDistance(New Double() {0, 0}, New Double() {3, 4}), 1.0E-12)
            assertEquals(7.0, MathUtils.manhattanDistance(New Double() {0, 0}, New Double() {3, 4}), 1.0E-12)
            assertEquals(5.0, MathUtils.hypotenuse(3, 4), 1.0E-12)
            assertEquals(0.5, MathUtils.sigmoid(0.0), 1.0E-12)
            assertEquals(120.0, MathUtils.factorial(5), 1.0E-9)
            assertEquals(10.0, MathUtils.combination(5, 2), 1.0E-9)
            assertEquals(20.0, MathUtils.permutation(5, 2), 1.0E-9)
            assertEquals(1, MathUtils.kroneckerDelta(2, 2))
            assertEquals(0, MathUtils.kroneckerDelta(2, 3))
            assertEquals(5, MathUtils.clamp(10, 0, 5))
            assertEquals(0, MathUtils.clamp(-10, 0, 5))
            assertEquals(3.14, MathUtils.roundDouble(3.14159, 2), 1.0E-12)
            assertEquals(3, MathUtils.round(3.4))
            assertEquals(4, MathUtils.round(3.5))
            assertEquals(0.5, MathUtils.normalize(5, 0, 10), 1.0E-12)
            assertArrayEquals(New Double() {0.25, 0.75}, MathUtils.normalizeToOne(New Double() {1, 3}), 1.0E-12)
            assertEquals(0.0, MathUtils.rootMeansSquaredError(New Double() {1, 2}, New Double() {1, 2}), 0.0)
            assertEquals(0.0, MathUtils.ssError(New Double() {1, 2}, New Double() {1, 2}), 0.0)
            assertArrayEquals(New Double() {1.5, 2.5}, MathUtils.fromString("1.5,2.5", ","), 0.0)
        End Sub

        <TestMethod>
        Public Sub RandomHelpersStayInRange()
            Dim rng As New Random(42)
            For i As Integer = 0 To 200
                Dim n = MathUtils.randomNumberBetween(3, 7, rng)
                assertTrue(n >= 3 AndAlso n <= 7, "got " & n)
                Dim d = MathUtils.randomDoubleBetween(-1.0, 1.0)
                assertTrue(d >= -1.0 AndAlso d <= 1.0)
            Next
            Dim successes = MathUtils.binomial(rng, 1000, 0.5)
            assertTrue(successes > 400 AndAlso successes < 600, "got " & successes)
            assertEquals(0, MathUtils.binomial(rng, 10, 2.0))
            Dim uniform = MathUtils.generateUniform(10)
            assertEquals(10, uniform.Length)
            assertThrows(Of ArgumentException)(Sub() MathUtils.randomNumberBetween(7, 3, rng))
        End Sub

    End Class

    <TestClass>
    Public Class MiscUtilTest

        <TestMethod>
        Public Sub SetUtils_Operations()
            Dim a As ISet(Of Integer) = New HashSet(Of Integer)(New Integer() {1, 2, 3})
            Dim b As ISet(Of Integer) = New HashSet(Of Integer)(New Integer() {2, 3, 4})
            assertTrue(SetUtils.union(Of Integer, Integer, Integer)(a, b).SetEquals(New Integer() {1, 2, 3, 4}))
            assertTrue(SetUtils.difference(Of Integer, Integer, Integer)(a, b).SetEquals(New Integer() {1}))
            assertTrue(SetUtils.intersection(a, b).SetEquals(New Integer() {2, 3}))
            assertTrue(SetUtils.intersectionP(Of Integer, Integer, Integer)(a, b))
            assertFalse(SetUtils.intersectionP(Of Integer, Integer, Integer)(a, New HashSet(Of Integer)(New Integer() {9})))
        End Sub

        <TestMethod>
        Public Sub InputStreamUtil_CountsLines()
            Dim bytes = Encoding.UTF8.GetBytes("a" & ControlChars.Lf & "b" & ControlChars.Lf & "c")
            assertEquals(2, InputStreamUtil.countLines(New MemoryStream(bytes)))
            assertEquals(1, InputStreamUtil.countLines(New MemoryStream(Encoding.UTF8.GetBytes("no newline"))))
            assertEquals(0, InputStreamUtil.countLines(New MemoryStream()))
            Dim tmp = Path.GetTempFileName()
            Try
                File.WriteAllText(tmp, "x" & ControlChars.Lf & "y" & ControlChars.Lf)
                assertEquals(2, InputStreamUtil.countLines(tmp))
            Finally
                File.Delete(tmp)
            End Try
        End Sub

        <TestMethod>
        Public Sub ND4VBFileUtils_HonoursTempDirProperty()
            Dim custom = Path.Combine(Path.GetTempPath(), "dl4vb-tmp-" & Guid.NewGuid().ToString("N"))
            JavaSystem.setProperty(ND4VBSystemProperties.ND4VB_TEMP_DIR_PROPERTY, custom)
            Try
                assertEquals(custom, ND4VBFileUtils.TempDir)
                Dim f = ND4VBFileUtils.createTempFile("pre", ".suf")
                assertTrue(File.Exists(f))
                assertTrue(f.StartsWith(custom))
                assertTrue(Path.GetFileName(f).StartsWith("pre") AndAlso f.EndsWith(".suf"))
                Dim g = ND4VBFileUtils.createTempFile("pre", Nothing)
                assertTrue(g.EndsWith(".tmp"))
                assertNotEquals(f, g)
            Finally
                JavaSystem.clearProperty(ND4VBSystemProperties.ND4VB_TEMP_DIR_PROPERTY)
                If Directory.Exists(custom) Then Directory.Delete(custom, True)
            End Try
            assertEquals(Path.GetTempPath(), ND4VBFileUtils.getTempDir())
        End Sub

        <TestMethod>
        Public Sub Paths_FindsExecutablesOnPath()
            assertTrue(Paths.nameExistsInPath("cmd.exe") OrElse Paths.nameExistsInPath("sh"))
            assertFalse(Paths.nameExistsInPath("definitely-not-a-real-program-" & Guid.NewGuid().ToString("N") & ".exe"))
            assertFalse(Paths.nameExistsInPath(""))
        End Sub

        <TestMethod>
        Public Sub Index_AssignsSequentialIndices()
            Dim idx As New Index()
            assertTrue(idx.add("a"))
            assertTrue(idx.add("b"))
            assertFalse(idx.add("a"))
            assertEquals(0, idx.indexOf("a"))
            assertEquals(1, idx.indexOf("b"))
            assertEquals(-1, idx.indexOf("zzz"))
            assertEquals(2, idx.size())
            assertEquals("a", idx.[get](0))
            assertEquals("b", idx.[get](1))
        End Sub

        <TestMethod>
        Public Sub ThreadUtils_SleepDoesNotThrow()
            ThreadUtils.uncheckedSleep(1)
        End Sub

    End Class

End Namespace
