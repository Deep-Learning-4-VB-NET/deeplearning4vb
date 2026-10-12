Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils

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

Namespace org.nd4j.linalg.profiler.data.primitives


	''' <summary>
	''' @author raver119
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StackDescriptor
	Public Class StackDescriptor
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected StackTraceElement stackTrace[];
		Protected Friend stackTrace() As StackTraceElement

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StackDescriptor(@NonNull StackTraceElement stack[])
		Public Sub New(ByVal stack() As StackTraceElement)
			' we cut off X first elements from stack, because they belong to profiler
			' basically, we just want to make sure, no profiler-related code is mentioned in stack trace
			Dim start As Integer = 0
			Do While start < stack.Length
				If stack(start).getClassName().contains("DefaultOpExecutioner") Then
					Exit Do
				End If
				start += 1
			Loop

			' in tests it's quite possible to have no DefaultOpExecutioner calls being used
			If start = stack.Length Then

				For start = 0 To stack.Length - 1
					If Not stack(start + 1).getClassName().contains("OpProfiler") AndAlso Not stack(start + 1).getClassName().contains("StackAggregator") Then
						Exit For
					End If
				Next start
			Else
				Do While start < stack.Length
					If Not stack(start).getClassName().contains("DefaultOpExecutioner") Then
						Exit Do
					End If
					start += 1
				Loop
			End If

			Do While start < stack.Length
				If Not stack(start).getClassName().contains("OpProfiler") Then
					Exit Do
				End If
				start += 1
			Loop

			Me.stackTrace = Arrays.CopyOfRange(stack, start, stack.Length)
			ArrayUtils.reverse(Me.stackTrace)
		End Sub

		Public Overridable ReadOnly Property EntryName As String
			Get
				Return getElementName(0)
			End Get
		End Property

		Public Overridable Function getElementName(ByVal idx As Integer) As String
			Return stackTrace(idx).getClassName() & "." & stackTrace(idx).getMethodName() & ":" & stackTrace(idx).getLineNumber()
		End Function

		Public Overridable Function size() As Integer
			Return stackTrace.Length
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As StackDescriptor = DirectCast(o, StackDescriptor)

			' Probably incorrect - comparing Object[] arrays with Arrays.equals
			Return stackTrace.SequenceEqual(that.stackTrace)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Arrays.hashCode(stackTrace)
		End Function

		Public Overrides Function ToString() As String
			Dim builder As New StringBuilder()
			builder.Append("Stack trace: " & vbLf)

			Dim i As Integer = 0
			Do While i < size()
				builder.Append("         ").Append(i).Append(": ").Append(getElementName(i)).Append(vbLf)
				i += 1
			Loop

			Return builder.ToString()
		End Function
	End Class

End Namespace