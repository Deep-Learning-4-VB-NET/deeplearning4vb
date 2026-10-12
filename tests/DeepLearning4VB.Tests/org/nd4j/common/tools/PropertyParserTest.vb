Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4j.common.tools

Namespace org.nd4j.common.tools

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name (originally by gagatust). The Java test
    ''' repeated the same property set in every method; here it is built once. Exception types
    ''' follow Java: malformed numbers throw <see cref="NumberFormatException"/> (an
    ''' <see cref="ArgumentException"/>), while parseFloat/parseDouble/parseChar/parseString of a
    ''' missing property throw <see cref="NullReferenceException"/>.
    ''' </summary>
    <TestClass>
    Public Class PropertyParserTest

        Private Shared Function Build(ByVal value1 As String, ByVal value2 As String) As PropertyParser
            Dim props As New Properties()
            props.put("value1", value1)
            props.put("value2", value2)
            props.put("empty", "")
            props.put("str", "abc")
            props.put("boolean", "true")
            props.put("float", "24.98")
            props.put("int", "12")
            props.put("char", "a")
            Return New PropertyParser(props)
        End Function

        <TestMethod>
        Public Sub testGetSetProperties()
            Dim props As New Properties()
            Dim instance As New PropertyParser(props)
            assertTrue(instance.Properties Is props)
            Dim other As New Properties()
            instance.Properties = other
            assertTrue(instance.getProperties() Is other)
        End Sub

        <TestMethod>
        Public Sub testParseString()
            Dim instance = Build("sTr1", "str_2")
            assertEquals("sTr1", instance.parseString("value1"))
            assertEquals("str_2", instance.parseString("value2"))
            assertEquals("", instance.parseString("empty"))
            assertEquals("abc", instance.parseString("str"))
            assertEquals("true", instance.parseString("boolean"))
            assertEquals("24.98", instance.parseString("float"))
            assertEquals("12", instance.parseString("int"))
            assertEquals("a", instance.parseString("char"))
            assertThrows(Of NullReferenceException)(Sub() instance.parseString("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseInt()
            Dim instance = Build("432", "-242")
            assertEquals(432, instance.parseInt("value1"))
            assertEquals(-242, instance.parseInt("value2"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("empty"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("str"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("boolean"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("float"))
            assertEquals(12, instance.parseInt("int"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("char"))
            assertThrows(Of ArgumentException)(Sub() instance.parseInt("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseBoolean()
            Dim instance = Build("true", "false")
            assertEquals(True, instance.parseBoolean("value1"))
            assertEquals(False, instance.parseBoolean("value2"))
            assertEquals(False, instance.parseBoolean("empty"))
            assertEquals(False, instance.parseBoolean("str"))
            assertEquals(True, instance.parseBoolean("boolean"))
            assertEquals(False, instance.parseBoolean("float"))
            assertEquals(False, instance.parseBoolean("int"))
            assertEquals(False, instance.parseBoolean("char"))
            assertThrows(Of ArgumentException)(Sub() instance.parseBoolean("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseFloat()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789F, instance.parseFloat("value1"), 0.0F)
            assertEquals(-9000.001F, instance.parseFloat("value2"), 0.0F)
            assertThrows(Of ArgumentException)(Sub() instance.parseFloat("empty"))
            assertThrows(Of ArgumentException)(Sub() instance.parseFloat("str"))
            assertThrows(Of ArgumentException)(Sub() instance.parseFloat("boolean"))
            assertEquals(24.98F, instance.parseFloat("float"), 0.0F)
            assertEquals(12.0F, instance.parseFloat("int"), 0.0F)
            assertThrows(Of ArgumentException)(Sub() instance.parseFloat("char"))
            assertThrows(Of NullReferenceException)(Sub() instance.parseFloat("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseDouble()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789, instance.parseDouble("value1"), 0.0)
            assertEquals(-9000.001, instance.parseDouble("value2"), 0.0)
            assertThrows(Of ArgumentException)(Sub() instance.parseDouble("empty"))
            assertThrows(Of ArgumentException)(Sub() instance.parseDouble("str"))
            assertThrows(Of ArgumentException)(Sub() instance.parseDouble("boolean"))
            assertEquals(24.98, instance.parseDouble("float"), 0.0)
            assertEquals(12.0, instance.parseDouble("int"), 0.0)
            assertThrows(Of ArgumentException)(Sub() instance.parseDouble("char"))
            assertThrows(Of NullReferenceException)(Sub() instance.parseDouble("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseLong()
            Dim instance = Build("12345678900", "-9000001")
            assertEquals(12345678900L, instance.parseLong("value1"))
            assertEquals(-9000001L, instance.parseLong("value2"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("empty"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("str"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("boolean"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("float"))
            assertEquals(12L, instance.parseLong("int"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("char"))
            assertThrows(Of ArgumentException)(Sub() instance.parseLong("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testParseChar()
            Dim instance = Build("b", "c")
            assertEquals("b"c, instance.parseChar("value1"))
            assertEquals("c"c, instance.parseChar("value2"))
            assertThrows(Of ArgumentException)(Sub() instance.parseChar("empty"))
            assertThrows(Of ArgumentException)(Sub() instance.parseChar("str"))
            assertThrows(Of ArgumentException)(Sub() instance.parseChar("boolean"))
            assertThrows(Of ArgumentException)(Sub() instance.parseChar("float"))
            assertThrows(Of ArgumentException)(Sub() instance.parseChar("int"))
            assertEquals("a"c, instance.parseChar("char"))
            assertThrows(Of NullReferenceException)(Sub() instance.parseChar("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToString_String()
            Dim instance = Build("sTr1", "str_2")
            assertEquals("sTr1", instance.toString("value1"))
            assertEquals("str_2", instance.toString("value2"))
            assertEquals("", instance.toString("empty"))
            assertEquals("abc", instance.toString("str"))
            assertEquals("true", instance.toString("boolean"))
            assertEquals("24.98", instance.toString("float"))
            assertEquals("12", instance.toString("int"))
            assertEquals("a", instance.toString("char"))
            assertEquals("", instance.toString("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToInt_String()
            Dim instance = Build("123", "-54")
            assertEquals(123, instance.toInt("value1"))
            assertEquals(-54, instance.toInt("value2"))
            assertEquals(0, instance.toInt("empty"))
            assertEquals(0, instance.toInt("str"))
            assertEquals(0, instance.toInt("boolean"))
            assertEquals(0, instance.toInt("float"))
            assertEquals(12, instance.toInt("int"))
            assertEquals(0, instance.toInt("char"))
            assertEquals(0, instance.toInt("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToBoolean_String()
            Dim instance = Build("true", "false")
            assertEquals(True, instance.toBoolean("value1"))
            assertEquals(False, instance.toBoolean("value2"))
            assertEquals(False, instance.toBoolean("empty"))
            assertEquals(False, instance.toBoolean("str"))
            assertEquals(True, instance.toBoolean("boolean"))
            assertEquals(False, instance.toBoolean("float"))
            assertEquals(False, instance.toBoolean("int"))
            assertEquals(False, instance.toBoolean("char"))
            assertEquals(False, instance.toBoolean("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToFloat_String()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789F, instance.toFloat("value1"), 0.0F)
            assertEquals(-9000.001F, instance.toFloat("value2"), 0.0F)
            assertEquals(0.0F, instance.toFloat("empty"), 0.0F)
            assertEquals(0.0F, instance.toFloat("str"), 0.0F)
            assertEquals(0.0F, instance.toFloat("boolean"), 0.0F)
            assertEquals(24.98F, instance.toFloat("float"), 0.0F)
            assertEquals(12.0F, instance.toFloat("int"), 0.0F)
            assertEquals(0.0F, instance.toFloat("char"), 0.0F)
            assertEquals(0.0F, instance.toFloat("nonexistent"), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testToDouble_String()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789, instance.toDouble("value1"), 0.0)
            assertEquals(-9000.001, instance.toDouble("value2"), 0.0)
            assertEquals(0.0, instance.toDouble("empty"), 0.0)
            assertEquals(0.0, instance.toDouble("str"), 0.0)
            assertEquals(0.0, instance.toDouble("boolean"), 0.0)
            assertEquals(24.98, instance.toDouble("float"), 0.0)
            assertEquals(12.0, instance.toDouble("int"), 0.0)
            assertEquals(0.0, instance.toDouble("char"), 0.0)
            assertEquals(0.0, instance.toDouble("nonexistent"), 0.0)
        End Sub

        <TestMethod>
        Public Sub testToLong_String()
            Dim instance = Build("12345678900", "-9000001")
            assertEquals(12345678900L, instance.toLong("value1"))
            assertEquals(-9000001L, instance.toLong("value2"))
            assertEquals(0L, instance.toLong("empty"))
            assertEquals(0L, instance.toLong("str"))
            assertEquals(0L, instance.toLong("boolean"))
            assertEquals(0L, instance.toLong("float"))
            assertEquals(12L, instance.toLong("int"))
            assertEquals(0L, instance.toLong("char"))
            assertEquals(0L, instance.toLong("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToChar_String()
            Dim instance = Build("f", "w")
            assertEquals("f"c, instance.toChar("value1"))
            assertEquals("w"c, instance.toChar("value2"))
            assertEquals(ChrW(0), instance.toChar("empty"))
            assertEquals(ChrW(0), instance.toChar("str"))
            assertEquals(ChrW(0), instance.toChar("boolean"))
            assertEquals(ChrW(0), instance.toChar("float"))
            assertEquals(ChrW(0), instance.toChar("int"))
            assertEquals("a"c, instance.toChar("char"))
            assertEquals(ChrW(0), instance.toChar("nonexistent"))
        End Sub

        <TestMethod>
        Public Sub testToString_String_String()
            Dim instance = Build("sTr1", "str_2")
            assertEquals("sTr1", instance.toString("value1", "defStr"))
            assertEquals("str_2", instance.toString("value2", "defStr"))
            assertEquals("", instance.toString("empty", "defStr"))
            assertEquals("abc", instance.toString("str", "defStr"))
            assertEquals("true", instance.toString("boolean", "defStr"))
            assertEquals("24.98", instance.toString("float", "defStr"))
            assertEquals("12", instance.toString("int", "defStr"))
            assertEquals("a", instance.toString("char", "defStr"))
            assertEquals("defStr", instance.toString("nonexistent", "defStr"))
        End Sub

        <TestMethod>
        Public Sub testToInt_String_int()
            Dim instance = Build("123", "-54")
            assertEquals(123, instance.toInt("value1", 17))
            assertEquals(-54, instance.toInt("value2", 17))
            assertEquals(17, instance.toInt("empty", 17))
            assertEquals(17, instance.toInt("str", 17))
            assertEquals(17, instance.toInt("boolean", 17))
            assertEquals(17, instance.toInt("float", 17))
            assertEquals(12, instance.toInt("int", 17))
            assertEquals(17, instance.toInt("char", 17))
            assertEquals(17, instance.toInt("nonexistent", 17))
        End Sub

        <TestMethod>
        Public Sub testToBoolean_String_boolean()
            Dim instance = Build("true", "false")
            assertEquals(True, instance.toBoolean("value1", True))
            assertEquals(False, instance.toBoolean("value2", True))
            assertEquals(False, instance.toBoolean("empty", True))
            assertEquals(False, instance.toBoolean("str", True))
            assertEquals(True, instance.toBoolean("boolean", True))
            assertEquals(False, instance.toBoolean("float", True))
            assertEquals(False, instance.toBoolean("int", True))
            assertEquals(False, instance.toBoolean("char", True))
            assertEquals(True, instance.toBoolean("nonexistent", True))
        End Sub

        <TestMethod>
        Public Sub testToFloat_String_float()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789F, instance.toFloat("value1", 0.123F), 0.0F)
            assertEquals(-9000.001F, instance.toFloat("value2", 0.123F), 0.0F)
            assertEquals(0.123F, instance.toFloat("empty", 0.123F), 0.0F)
            assertEquals(0.123F, instance.toFloat("str", 0.123F), 0.0F)
            assertEquals(0.123F, instance.toFloat("boolean", 0.123F), 0.0F)
            assertEquals(24.98F, instance.toFloat("float", 0.123F), 0.0F)
            assertEquals(12.0F, instance.toFloat("int", 0.123F), 0.0F)
            assertEquals(0.123F, instance.toFloat("char", 0.123F), 0.0F)
            assertEquals(0.123F, instance.toFloat("nonexistent", 0.123F), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testToDouble_String_double()
            Dim instance = Build("12345.6789", "-9000.001")
            assertEquals(12345.6789, instance.toDouble("value1", 0.123), 0.0)
            assertEquals(-9000.001, instance.toDouble("value2", 0.123), 0.0)
            assertEquals(0.123, instance.toDouble("empty", 0.123), 0.0)
            assertEquals(0.123, instance.toDouble("str", 0.123), 0.0)
            assertEquals(0.123, instance.toDouble("boolean", 0.123), 0.0)
            assertEquals(24.98, instance.toDouble("float", 0.123), 0.0)
            assertEquals(12.0, instance.toDouble("int", 0.123), 0.0)
            assertEquals(0.123, instance.toDouble("char", 0.123), 0.0)
            assertEquals(0.123, instance.toDouble("nonexistent", 0.123), 0.0)
        End Sub

        <TestMethod>
        Public Sub testToLong_String_long()
            Dim instance = Build("12345678900", "-9000001")
            assertEquals(12345678900L, instance.toLong("value1", 3L))
            assertEquals(-9000001L, instance.toLong("value2", 3L))
            assertEquals(3L, instance.toLong("empty", 3L))
            assertEquals(3L, instance.toLong("str", 3L))
            assertEquals(3L, instance.toLong("boolean", 3L))
            assertEquals(3L, instance.toLong("float", 3L))
            assertEquals(12L, instance.toLong("int", 3L))
            assertEquals(3L, instance.toLong("char", 3L))
            assertEquals(3L, instance.toLong("nonexistent", 3L))
        End Sub

        <TestMethod>
        Public Sub testToChar_String_char()
            Dim instance = Build("f", "w")
            assertEquals("f"c, instance.toChar("value1", "t"c))
            assertEquals("w"c, instance.toChar("value2", "t"c))
            assertEquals("t"c, instance.toChar("empty", "t"c))
            assertEquals("t"c, instance.toChar("str", "t"c))
            assertEquals("t"c, instance.toChar("boolean", "t"c))
            assertEquals("t"c, instance.toChar("float", "t"c))
            assertEquals("t"c, instance.toChar("int", "t"c))
            assertEquals("a"c, instance.toChar("char", "t"c))
            assertEquals("t"c, instance.toChar("nonexistent", "t"c))
        End Sub

    End Class

End Namespace
