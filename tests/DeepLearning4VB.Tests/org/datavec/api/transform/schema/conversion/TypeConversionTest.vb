Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform.schema.conversion
Imports DeepLearning4VB.org.datavec.api.writable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.schema.conversion

    ''' <summary><see cref="TypeConversion"/>: writables, strings and arbitrary objects to the primitive column types.</summary>
    <TestClass>
    Public Class TypeConversionTest

        <TestMethod>
        Public Sub testSingleton()
            assertNotNull(TypeConversion.getInstance())
            assertTrue(TypeConversion.getInstance() Is TypeConversion.Instance)
            assertTrue(TypeConversion.getInstance() Is TypeConversion.getInstance())
        End Sub

        <TestMethod>
        Public Sub testConvertInt()
            Dim tc As TypeConversion = TypeConversion.getInstance()
            'strings go through Double.parseDouble and are truncated (Java: (int) Double.parseDouble(s))
            assertEquals(3, tc.convertInt("3"))
            assertEquals(3, tc.convertInt("3.7"))
            assertEquals(-3, tc.convertInt("-3.7"))
            assertEquals(1000, tc.convertInt("1e3"))
            assertEquals(Integer.MaxValue, tc.convertInt("1e20"))
            assertEquals(0, tc.convertInt("NaN"))
            'writables use their own conversion
            assertEquals(5, tc.convertInt(New IntWritable(5)))
            assertEquals(5, tc.convertInt(New Text("5")))
            assertEquals(2, tc.convertInt(New DoubleWritable(2.9)))
            'objects: writables directly, everything else through its string form
            assertEquals(5, tc.convertInt(CObj(New LongWritable(5))))
            assertEquals(7, tc.convertInt(CObj(7)))
            assertEquals(7, tc.convertInt(CObj(7.5)))
            assertEquals(8, tc.convertInt(CObj("8.25")))
            assertThrows(Of NumberFormatException)(Sub() tc.convertInt("abc"))
            assertThrows(Of NumberFormatException)(Sub() tc.convertInt(CObj("x")))
            assertThrows(Of NumberFormatException)(Sub() tc.convertInt(New Text("abc")))
        End Sub

        <TestMethod>
        Public Sub testConvertDoubleAndFloat()
            Dim tc As TypeConversion = TypeConversion.getInstance()
            assertEquals(1.5, tc.convertDouble("1.5"), 0.0)
            assertEquals(1000.0, tc.convertDouble("1e3"), 0.0)
            assertEquals(1.5, tc.convertDouble(" 1.5 "), 0.0)
            assertTrue(Double.IsNaN(tc.convertDouble("NaN")))
            assertTrue(Double.IsPositiveInfinity(tc.convertDouble("Infinity")))
            assertEquals(2.0, tc.convertDouble(New IntWritable(2)), 0.0)
            assertEquals(2.5, tc.convertDouble(New Text("2.5")), 0.0)
            assertEquals(2.5, tc.convertDouble(CObj(New DoubleWritable(2.5))), 0.0)
            assertEquals(2.5, tc.convertDouble(CObj(2.5)), 0.0)
            assertEquals(3.0, tc.convertDouble(CObj(3)), 0.0)
            assertThrows(Of NumberFormatException)(Sub() tc.convertDouble("abc"))
            assertThrows(Of NumberFormatException)(Sub() tc.convertDouble(""))
            assertThrows(Of NumberFormatException)(Sub() tc.convertDouble(CObj("1,5")))

            assertEquals(0.5F, tc.convertFloat("0.5"), 0.0F)
            assertEquals(0.5F, tc.convertFloat("0.5f"), 0.0F)
            assertEquals(2.0F, tc.convertFloat(New IntWritable(2)), 0.0F)
            assertEquals(0.25F, tc.convertFloat(CObj(New FloatWritable(0.25F))), 0.0F)
            assertEquals(0.25F, tc.convertFloat(CObj(0.25F)), 0.0F)
            assertThrows(Of NumberFormatException)(Sub() tc.convertFloat("x"))
        End Sub

        <TestMethod>
        Public Sub testConvertLongAndString()
            Dim tc As TypeConversion = TypeConversion.getInstance()
            assertEquals(12L, tc.convertLong("12"))
            assertEquals(Long.MinValue, tc.convertLong("-9223372036854775808"))
            assertEquals(12L, tc.convertLong(New IntWritable(12)))
            assertEquals(12L, tc.convertLong(New Text("12")))
            assertEquals(12L, tc.convertLong(CObj(New LongWritable(12))))
            assertEquals(12L, tc.convertLong(CObj(12)))
            assertEquals(12L, tc.convertLong(CObj("12")))
            'Long.parseLong does not accept decimals or exponents
            assertThrows(Of NumberFormatException)(Sub() tc.convertLong("1.5"))
            assertThrows(Of NumberFormatException)(Sub() tc.convertLong("1e3"))
            assertThrows(Of NumberFormatException)(Sub() tc.convertLong(CObj(1.5)))
            assertThrows(Of NumberFormatException)(Sub() tc.convertLong(New Text("x")))

            assertEquals("abc", tc.convertString("abc"))
            assertEquals("abc", tc.convertString(New Text("abc")))
            assertEquals("5", tc.convertString(New IntWritable(5)))
            assertEquals("1.5", tc.convertString(New DoubleWritable(1.5)))
            assertEquals("NullWritable", tc.convertString(NullWritable.INSTANCE))
            assertEquals("5", tc.convertString(CObj(New IntWritable(5))))
            assertEquals("7", tc.convertString(CObj(7)))
            assertEquals("x", tc.convertString(CObj("x")))
        End Sub

    End Class

End Namespace
