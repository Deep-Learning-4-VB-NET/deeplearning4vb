Imports System

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
	''' PropertyParser
	''' 
	''' @author gagatust
	''' </summary>
	Public Class PropertyParser

'JAVA TO VB CONVERTER NOTE: The field properties was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private properties_Conflict As Properties

		Public Sub New(ByVal properties As Properties)
			Me.properties_Conflict = properties
		End Sub

		Public Overridable Property Properties As Properties
			Get
				Return properties_Conflict
			End Get
			Set(ByVal properties As Properties)
				Me.properties_Conflict = properties
			End Set
		End Property


		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseString(ByVal name As String) As String
			Dim [property] As String = Properties.getProperty(name)
			If [property] Is Nothing Then
				Throw New System.NullReferenceException()
			End If
			Return [property]
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseInt(ByVal name As String) As Integer
			Return Integer.Parse(Properties.getProperty(name))
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseBoolean(ByVal name As String) As Boolean
			Dim [property] As String = Properties.getProperty(name)
			If [property] Is Nothing Then
				Throw New System.ArgumentException()
			End If
			Return Boolean.Parse([property])
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseFloat(ByVal name As String) As Single
			Return Single.Parse(Properties.getProperty(name))
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseDouble(ByVal name As String) As Double
			Return Double.Parse(Properties.getProperty(name))
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseLong(ByVal name As String) As Long
			Return Long.Parse(Properties.getProperty(name))
		End Function

		''' <summary>
		''' Parse property.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function parseChar(ByVal name As String) As Char
			Dim [property] As String = Properties.getProperty(name)
			If [property].Length <> 1 Then
				Throw New System.ArgumentException(name & " property is't char")
			End If
			Return [property].Chars(0)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toString(ByVal name As String) As String
			Return toString(name, "")
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toInt(ByVal name As String) As Integer
			Return toInt(name, 0)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toBoolean(ByVal name As String) As Boolean
			Return toBoolean(name, False)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toFloat(ByVal name As String) As Single
			Return toFloat(name, 0.0f)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toDouble(ByVal name As String) As Double
			Return toDouble(name, 0.0)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toLong(ByVal name As String) As Long
			Return toLong(name, 0)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <returns> property </returns>
		Public Overridable Function toChar(ByVal name As String) As Char
			Return toChar(name, ChrW(&H0000))
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toString(ByVal name As String, ByVal defaultValue As String) As String
			Dim [property] As String = Properties.getProperty(name)
			Return If([property] IsNot Nothing, [property], defaultValue)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toInt(ByVal name As String, ByVal defaultValue As Integer) As Integer
			Try
				Return parseInt(name)
			Catch e As Exception
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toBoolean(ByVal name As String, ByVal defaultValue As Boolean) As Boolean
			Dim [property] As String = Properties.getProperty(name)
			Return If([property] IsNot Nothing, Boolean.Parse([property]), defaultValue)
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toFloat(ByVal name As String, ByVal defaultValue As Single) As Single
			Try
				Return parseFloat(name)
			Catch e As Exception
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toDouble(ByVal name As String, ByVal defaultValue As Double) As Double
			Try
				Return parseDouble(name)
			Catch e As Exception
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toLong(ByVal name As String, ByVal defaultValue As Long) As Long
			Try
				Return parseLong(name)
			Catch e As Exception
				Return defaultValue
			End Try
		End Function

		''' <summary>
		''' Get property. The method returns the default value if the property is not parsed.
		''' </summary>
		''' <param name="name"> property name </param>
		''' <param name="defaultValue"> default value </param>
		''' <returns> property </returns>
		Public Overridable Function toChar(ByVal name As String, ByVal defaultValue As Char) As Char
			Try
				Return parseChar(name)
			Catch e As Exception
				Return defaultValue
			End Try
		End Function
	End Class

End Namespace