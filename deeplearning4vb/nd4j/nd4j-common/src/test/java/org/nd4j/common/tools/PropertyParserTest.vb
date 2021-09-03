Imports System
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports AfterClass = org.junit.AfterClass
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports BeforeClass = org.junit.BeforeClass
Imports Test = org.junit.jupiter.api.Test
Imports PropertyParser = org.nd4j.common.tools.PropertyParser

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.common.tools

	''' <summary>
	''' Tests for PropertyParser
	''' 
	''' @author gagatust
	''' </summary>
	Public Class PropertyParserTest

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void setUpClass()
		Public Shared Sub setUpClass()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterClass public static void tearDownClass()
		Public Shared Sub tearDownClass()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
		End Sub

		''' <summary>
		''' Test of getProperties method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGetProperties()
		Public Overridable Sub testGetProperties()

		End Sub

		''' <summary>
		''' Test of setProperties method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSetProperties()
		Public Overridable Sub testSetProperties()

		End Sub

		''' <summary>
		''' Test of parseString method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseString()
		Public Overridable Sub testParseString()
			Console.WriteLine("parseString")
			Dim expResult As String
			Dim result As String

			Dim props As New Properties()
			props.put("value1", "sTr1")
			props.put("value2", "str_2")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "sTr1"
			result = instance.parseString("value1")
			assertEquals(expResult, result)

			expResult = "str_2"
			result = instance.parseString("value2")
			assertEquals(expResult, result)

			expResult = ""
			result = instance.parseString("empty")
			assertEquals(expResult, result)

			expResult = "abc"
			result = instance.parseString("str")
			assertEquals(expResult, result)

			expResult = "true"
			result = instance.parseString("boolean")
			assertEquals(expResult, result)

			expResult = "24.98"
			result = instance.parseString("float")
			assertEquals(expResult, result)

			expResult = "12"
			result = instance.parseString("int")
			assertEquals(expResult, result)

			expResult = "a"
			result = instance.parseString("char")
			assertEquals(expResult, result)

			Try
				instance.parseString("nonexistent")
				fail("no exception")
			Catch e As System.NullReferenceException
			End Try
		End Sub

		''' <summary>
		''' Test of parseInt method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseInt()
		Public Overridable Sub testParseInt()
			Console.WriteLine("parseInt")
			Dim expResult As Integer
			Dim result As Integer

			Dim props As New Properties()
			props.put("value1", "432")
			props.put("value2", "-242")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 432
			result = instance.parseInt("value1")
			assertEquals(expResult, result)

			expResult = -242
			result = instance.parseInt("value2")
			assertEquals(expResult, result)

			Try
				instance.parseInt("empty")
				fail("no exception")
			Catch e As System.FormatException
			End Try

			Try
				instance.parseInt("str")
				fail("no exception")
			Catch e As System.FormatException
			End Try

			Try
				instance.parseInt("boolean")
				assertEquals(expResult, result)
				fail("no exception")
			Catch e As System.FormatException
			End Try

			Try
				instance.parseInt("float")
				fail("no exception")
			Catch e As System.FormatException
			End Try

			expResult = 12
			result = instance.parseInt("int")
			assertEquals(expResult, result)

			Try
				instance.parseInt("char")
				fail("no exception")
			Catch e As System.FormatException
			End Try

			Try
				expResult = 0
				result = instance.parseInt("nonexistent")
				fail("no exception")
				assertEquals(expResult, result)
			Catch e As System.ArgumentException
			End Try
		End Sub

		''' <summary>
		''' Test of parseBoolean method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseBoolean()
		Public Overridable Sub testParseBoolean()
			Console.WriteLine("parseBoolean")
			Dim expResult As Boolean
			Dim result As Boolean

			Dim props As New Properties()
			props.put("value1", "true")
			props.put("value2", "false")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = True
			result = instance.parseBoolean("value1")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("value2")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("empty")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("str")
			assertEquals(expResult, result)

			expResult = True
			result = instance.parseBoolean("boolean")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("float")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("int")
			assertEquals(expResult, result)

			expResult = False
			result = instance.parseBoolean("char")
			assertEquals(expResult, result)

			Try
				expResult = False
				result = instance.parseBoolean("nonexistent")
				fail("no exception")
				assertEquals(expResult, result)
			Catch e As System.ArgumentException
			End Try
		End Sub

		''' <summary>
		''' Test of parseDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseFloat()
		Public Overridable Sub testParseFloat()
			Console.WriteLine("parseFloat")
			Dim expResult As Double
			Dim result As Double

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789f
			result = instance.parseFloat("value1")
			assertEquals(expResult, result, 0)

			expResult = -9000.001f
			result = instance.parseFloat("value2")
			assertEquals(expResult, result, 0)

			Try
				instance.parseFloat("empty")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseFloat("str")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseFloat("boolean")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			expResult = 24.98f
			result = instance.parseFloat("float")
			assertEquals(expResult, result, 0)

			expResult = 12f
			result = instance.parseFloat("int")
			assertEquals(expResult, result, 0)

			Try
				instance.parseFloat("char")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseFloat("nonexistent")
				fail("no exception")
			Catch e As System.NullReferenceException
			End Try
		End Sub

		''' <summary>
		''' Test of parseDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseDouble()
		Public Overridable Sub testParseDouble()
			Console.WriteLine("parseDouble")
			Dim expResult As Double
			Dim result As Double

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789
			result = instance.parseDouble("value1")
			assertEquals(expResult, result, 0)

			expResult = -9000.001
			result = instance.parseDouble("value2")
			assertEquals(expResult, result, 0)

			Try
				instance.parseDouble("empty")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseDouble("str")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseDouble("boolean")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			expResult = 24.98
			result = instance.parseDouble("float")
			assertEquals(expResult, result, 0)

			expResult = 12
			result = instance.parseDouble("int")
			assertEquals(expResult, result, 0)

			Try
				instance.parseDouble("char")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseDouble("nonexistent")
				fail("no exception")
			Catch e As System.NullReferenceException
			End Try
		End Sub

		''' <summary>
		''' Test of parseLong method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseLong()
		Public Overridable Sub testParseLong()
			Console.WriteLine("parseLong")
			Dim expResult As Long
			Dim result As Long

			Dim props As New Properties()
			props.put("value1", "12345678900")
			props.put("value2", "-9000001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345678900L
			result = instance.parseLong("value1")
			assertEquals(expResult, result)

			expResult = -9000001L
			result = instance.parseLong("value2")
			assertEquals(expResult, result)

			Try
				instance.parseLong("empty")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseLong("str")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseLong("boolean")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseLong("float")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			expResult = 12L
			result = instance.parseLong("int")
			assertEquals(expResult, result)

			Try
				instance.parseLong("char")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseLong("nonexistent")
				fail("no exception")
			Catch e As System.ArgumentException
			End Try
		End Sub

		''' <summary>
		''' Test of parseChar method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParseChar()
		Public Overridable Sub testParseChar()
			Console.WriteLine("parseChar")
			Dim expResult As Char
			Dim result As Char

			Dim props As New Properties()
			props.put("value1", "b")
			props.put("value2", "c")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "b"c
			result = instance.parseChar("value1")
			assertEquals(expResult, result)

			expResult = "c"c
			result = instance.parseChar("value2")
			assertEquals(expResult, result)

			Try
				instance.parseChar("empty")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseChar("str")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseChar("boolean")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseChar("float")
			Catch e As System.ArgumentException
			End Try

			Try
				instance.parseChar("int")
			Catch e As System.ArgumentException
			End Try

			expResult = "a"c
			result = instance.parseChar("char")
			assertEquals(expResult, result)

			Try
				instance.parseChar("nonexistent")
				fail("no exception")
				assertEquals(expResult, result)
			Catch e As System.NullReferenceException
			End Try
		End Sub

		''' <summary>
		''' Test of toString method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToString_String()
		Public Overridable Sub testToString_String()
			Console.WriteLine("toString")
			Dim expResult As String
			Dim result As String

			Dim props As New Properties()
			props.put("value1", "sTr1")
			props.put("value2", "str_2")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "sTr1"
			result = instance.toString("value1")
			assertEquals(expResult, result)

			expResult = "str_2"
			result = instance.toString("value2")
			assertEquals(expResult, result)

			expResult = ""
			result = instance.toString("empty")
			assertEquals(expResult, result)

			expResult = "abc"
			result = instance.toString("str")
			assertEquals(expResult, result)

			expResult = "true"
			result = instance.toString("boolean")
			assertEquals(expResult, result)

			expResult = "24.98"
			result = instance.toString("float")
			assertEquals(expResult, result)

			expResult = "12"
			result = instance.toString("int")
			assertEquals(expResult, result)

			expResult = "a"
			result = instance.toString("char")
			assertEquals(expResult, result)

			expResult = ""
			result = instance.toString("nonexistent")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toInt method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToInt_String()
		Public Overridable Sub testToInt_String()
			Console.WriteLine("toInt")
			Dim expResult As Integer
			Dim result As Integer

			Dim props As New Properties()
			props.put("value1", "123")
			props.put("value2", "-54")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 123
			result = instance.toInt("value1")
			assertEquals(expResult, result)

			expResult = -54
			result = instance.toInt("value2")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("empty")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("str")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("boolean")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("float")
			assertEquals(expResult, result)

			expResult = 12
			result = instance.toInt("int")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("char")
			assertEquals(expResult, result)

			expResult = 0
			result = instance.toInt("nonexistent")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toBoolean method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToBoolean_String()
		Public Overridable Sub testToBoolean_String()
			Console.WriteLine("toBoolean")
			Dim expResult As Boolean
			Dim result As Boolean

			Dim props As New Properties()
			props.put("value1", "true")
			props.put("value2", "false")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = True
			result = instance.toBoolean("value1")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("value2")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("empty")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("str")
			assertEquals(expResult, result)

			expResult = True
			result = instance.toBoolean("boolean")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("float")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("int")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("char")
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("nonexistent")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToFloat_String()
		Public Overridable Sub testToFloat_String()
			Console.WriteLine("toFloat")
			Dim expResult As Single
			Dim result As Single

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789f
			result = instance.toFloat("value1")
			assertEquals(expResult, result, 0f)

			expResult = -9000.001f
			result = instance.toFloat("value2")
			assertEquals(expResult, result, 0f)

			expResult = 0f
			result = instance.toFloat("empty")
			assertEquals(expResult, result, 0f)

			expResult = 0f
			result = instance.toFloat("str")
			assertEquals(expResult, result, 0f)

			expResult = 0f
			result = instance.toFloat("boolean")
			assertEquals(expResult, result, 0f)

			expResult = 24.98f
			result = instance.toFloat("float")
			assertEquals(expResult, result, 0f)

			expResult = 12f
			result = instance.toFloat("int")
			assertEquals(expResult, result, 0f)

			expResult = 0f
			result = instance.toFloat("char")
			assertEquals(expResult, result, 0f)

			expResult = 0f
			result = instance.toFloat("nonexistent")
			assertEquals(expResult, result, 0f)
		End Sub

		''' <summary>
		''' Test of toDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToDouble_String()
		Public Overridable Sub testToDouble_String()
			Console.WriteLine("toDouble")
			Dim expResult As Double
			Dim result As Double

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789
			result = instance.toDouble("value1")
			assertEquals(expResult, result, 0)

			expResult = -9000.001
			result = instance.toDouble("value2")
			assertEquals(expResult, result, 0)

			expResult = 0
			result = instance.toDouble("empty")
			assertEquals(expResult, result, 0)

			expResult = 0
			result = instance.toDouble("str")
			assertEquals(expResult, result, 0)

			expResult = 0
			result = instance.toDouble("boolean")
			assertEquals(expResult, result, 0)

			expResult = 24.98
			result = instance.toDouble("float")
			assertEquals(expResult, result, 0)

			expResult = 12
			result = instance.toDouble("int")
			assertEquals(expResult, result, 0)

			expResult = 0
			result = instance.toDouble("char")
			assertEquals(expResult, result, 0)

			expResult = 0
			result = instance.toDouble("nonexistent")
			assertEquals(expResult, result, 0)
		End Sub

		''' <summary>
		''' Test of toLong method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToLong_String()
		Public Overridable Sub testToLong_String()
			Console.WriteLine("toLong")
			Dim expResult As Long
			Dim result As Long

			Dim props As New Properties()
			props.put("value1", "12345678900")
			props.put("value2", "-9000001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345678900L
			result = instance.toLong("value1")
			assertEquals(expResult, result)

			expResult = -9000001L
			result = instance.toLong("value2")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("empty")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("str")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("boolean")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("float")
			assertEquals(expResult, result)

			expResult = 12L
			result = instance.toLong("int")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("char")
			assertEquals(expResult, result)

			expResult = 0L
			result = instance.toLong("nonexistent")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toChar method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToChar_String()
		Public Overridable Sub testToChar_String()
			Console.WriteLine("toChar")
			Dim expResult As Char
			Dim result As Char

			Dim props As New Properties()
			props.put("value1", "f")
			props.put("value2", "w")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "f"c
			result = instance.toChar("value1")
			assertEquals(expResult, result)

			expResult = "w"c
			result = instance.toChar("value2")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("empty")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("str")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("boolean")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("float")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("int")
			assertEquals(expResult, result)

			expResult = "a"c
			result = instance.toChar("char")
			assertEquals(expResult, result)

			expResult = ChrW(&H0000)
			result = instance.toChar("nonexistent")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toString method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToString_String_String()
		Public Overridable Sub testToString_String_String()
			Console.WriteLine("toString")
			Dim expResult As String
			Dim result As String

			Dim props As New Properties()
			props.put("value1", "sTr1")
			props.put("value2", "str_2")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "sTr1"
			result = instance.toString("value1", "defStr")
			assertEquals(expResult, result)

			expResult = "str_2"
			result = instance.toString("value2", "defStr")
			assertEquals(expResult, result)

			expResult = ""
			result = instance.toString("empty", "defStr")
			assertEquals(expResult, result)

			expResult = "abc"
			result = instance.toString("str", "defStr")
			assertEquals(expResult, result)

			expResult = "true"
			result = instance.toString("boolean", "defStr")
			assertEquals(expResult, result)

			expResult = "24.98"
			result = instance.toString("float", "defStr")
			assertEquals(expResult, result)

			expResult = "12"
			result = instance.toString("int", "defStr")
			assertEquals(expResult, result)

			expResult = "a"
			result = instance.toString("char", "defStr")
			assertEquals(expResult, result)

			expResult = "defStr"
			result = instance.toString("nonexistent", "defStr")
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toInt method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToInt_String_int()
		Public Overridable Sub testToInt_String_int()
			Console.WriteLine("toInt")
			Dim expResult As Integer
			Dim result As Integer

			Dim props As New Properties()
			props.put("value1", "123")
			props.put("value2", "-54")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 123
			result = instance.toInt("value1", 17)
			assertEquals(expResult, result)

			expResult = -54
			result = instance.toInt("value2", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("empty", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("str", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("boolean", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("float", 17)
			assertEquals(expResult, result)

			expResult = 12
			result = instance.toInt("int", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("char", 17)
			assertEquals(expResult, result)

			expResult = 17
			result = instance.toInt("nonexistent", 17)
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toBoolean method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToBoolean_String_boolean()
		Public Overridable Sub testToBoolean_String_boolean()
			Console.WriteLine("toBoolean")

			Dim expResult As Boolean
			Dim result As Boolean

			Dim props As New Properties()
			props.put("value1", "true")
			props.put("value2", "false")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = True
			result = instance.toBoolean("value1", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("value2", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("empty", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("str", True)
			assertEquals(expResult, result)

			expResult = True
			result = instance.toBoolean("boolean", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("float", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("int", True)
			assertEquals(expResult, result)

			expResult = False
			result = instance.toBoolean("char", True)
			assertEquals(expResult, result)

			expResult = True
			result = instance.toBoolean("nonexistent", True)
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToFloat_String_float()
		Public Overridable Sub testToFloat_String_float()
			Console.WriteLine("toFloat")
			Dim expResult As Single
			Dim result As Single

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789f
			result = instance.toFloat("value1", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = -9000.001f
			result = instance.toFloat("value2", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 0.123f
			result = instance.toFloat("empty", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 0.123f
			result = instance.toFloat("str", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 0.123f
			result = instance.toFloat("boolean", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 24.98f
			result = instance.toFloat("float", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 12
			result = instance.toFloat("int", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 0.123f
			result = instance.toFloat("char", 0.123f)
			assertEquals(expResult, result, 0)

			expResult = 0.123f
			result = instance.toFloat("nonexistent", 0.123f)
			assertEquals(expResult, result, 0)
		End Sub

		''' <summary>
		''' Test of toDouble method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToDouble_String_double()
		Public Overridable Sub testToDouble_String_double()
			Console.WriteLine("toDouble")
			Dim expResult As Double
			Dim result As Double

			Dim props As New Properties()
			props.put("value1", "12345.6789")
			props.put("value2", "-9000.001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345.6789
			result = instance.toDouble("value1", 0.123)
			assertEquals(expResult, result, 0)

			expResult = -9000.001
			result = instance.toDouble("value2", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 0.123
			result = instance.toDouble("empty", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 0.123
			result = instance.toDouble("str", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 0.123
			result = instance.toDouble("boolean", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 24.98
			result = instance.toDouble("float", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 12
			result = instance.toDouble("int", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 0.123
			result = instance.toDouble("char", 0.123)
			assertEquals(expResult, result, 0)

			expResult = 0.123
			result = instance.toDouble("nonexistent", 0.123)
			assertEquals(expResult, result, 0)
		End Sub

		''' <summary>
		''' Test of toLong method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToLong_String_long()
		Public Overridable Sub testToLong_String_long()
			Console.WriteLine("toLong")
			Dim expResult As Long
			Dim result As Long

			Dim props As New Properties()
			props.put("value1", "12345678900")
			props.put("value2", "-9000001")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = 12345678900L
			result = instance.toLong("value1", 3L)
			assertEquals(expResult, result)

			expResult = -9000001L
			result = instance.toLong("value2", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("empty", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("str", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("boolean", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("float", 3L)
			assertEquals(expResult, result)

			expResult = 12L
			result = instance.toLong("int", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("char", 3L)
			assertEquals(expResult, result)

			expResult = 3L
			result = instance.toLong("nonexistent", 3L)
			assertEquals(expResult, result)
		End Sub

		''' <summary>
		''' Test of toChar method, of class PropertyParser.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testToChar_String_char()
		Public Overridable Sub testToChar_String_char()
			Console.WriteLine("toChar")
			Dim expResult As Char
			Dim result As Char

			Dim props As New Properties()
			props.put("value1", "f")
			props.put("value2", "w")
			props.put("empty", "")
			props.put("str", "abc")
			props.put("boolean", "true")
			props.put("float", "24.98")
			props.put("int", "12")
			props.put("char", "a")
			Dim instance As New PropertyParser(props)

			expResult = "f"c
			result = instance.toChar("value1", "t"c)
			assertEquals(expResult, result)

			expResult = "w"c
			result = instance.toChar("value2", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("empty", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("str", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("boolean", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("float", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("int", "t"c)
			assertEquals(expResult, result)

			expResult = "a"c
			result = instance.toChar("char", "t"c)
			assertEquals(expResult, result)

			expResult = "t"c
			result = instance.toChar("nonexistent", "t"c)
			assertEquals(expResult, result)
		End Sub

	End Class

End Namespace