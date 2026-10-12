Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.spark.transform.misc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class SequenceWritablesToStringFunction implements org.apache.spark.api.java.function.@Function<java.util.List<java.util.List<org.datavec.api.writable.Writable>>, String>
	Public Class SequenceWritablesToStringFunction
		Implements [Function](Of IList(Of IList(Of Writable)), String)

		Public Const DEFAULT_DELIMITER As String = ","
		Public Shared ReadOnly DEFAULT_TIME_STEP_DELIMITER As String = vbLf

		Private ReadOnly delimiter As String
		Private ReadOnly timeStepDelimiter As String
		Private ReadOnly quote As String

		''' <summary>
		''' Function with default delimiters ("," and "\n")
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_DELIMITER)
		End Sub

		''' <summary>
		''' function with default delimiter ("\n") between time steps </summary>
		''' <param name="delim"> Delimiter between values within a single time step </param>
		Public Sub New(ByVal delim As String)
			Me.New(delim, DEFAULT_TIME_STEP_DELIMITER, Nothing)
		End Sub

		''' 
		''' <param name="delim">             The delimiter between column values in a single time step (for example, ",") </param>
		''' <param name="timeStepDelimiter"> The delimiter between time steps (for example, "\n") </param>
		Public Sub New(ByVal delim As String, ByVal timeStepDelimiter As String)
			Me.New(delim, timeStepDelimiter, Nothing)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public String call(java.util.List<java.util.List<org.datavec.api.writable.Writable>> c) throws Exception
		Public Overrides Function [call](ByVal c As IList(Of IList(Of Writable))) As String

			Dim sb As New StringBuilder()
			Dim firstLine As Boolean = True
			For Each timeStep As IList(Of Writable) In c
				If Not firstLine Then
					sb.Append(timeStepDelimiter)
				End If
				WritablesToStringFunction.append(timeStep, sb, delimiter, quote)
				firstLine = False
			Next timeStep

			Return sb.ToString()
		End Function
	End Class

End Namespace