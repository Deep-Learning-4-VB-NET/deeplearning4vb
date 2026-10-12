Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading

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

Namespace org.nd4j.common.base


	Public NotInheritable Class Preconditions
		Private Shared ReadOnly FORMATTERS As IDictionary(Of String, PreconditionsFormat) = New Dictionary(Of String, PreconditionsFormat)()
		Shared Sub New()
			Dim sl As ServiceLoader(Of PreconditionsFormat) = ND4JClassLoading.loadService(GetType(PreconditionsFormat))
			For Each pf As PreconditionsFormat In sl
				Dim formatTags As IList(Of String) = pf.formatTags()
				For Each s As String In formatTags
					FORMATTERS(s) = pf
				Next s
			Next pf
		End Sub

		Private Sub New()
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalArgumentException if {@code b} is false
		''' </summary>
		''' <param name="b"> Argument to check </param>
		Public Shared Sub checkArgument(ByVal b As Boolean)
			If Not b Then
				Throw New System.ArgumentException()
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalArgumentException with the specified message if {@code b} is false
		''' </summary>
		''' <param name="b">       Argument to check </param>
		''' <param name="message"> Message for exception. May be null </param>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal message As String)
			If Not b Then
				throwEx(message)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer)
			If Not b Then
				throwEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long)
			If Not b Then
				throwEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double)
			If Not b Then
				throwEx(msg, arg1)
			End If
		End Sub


		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object)
			If Not b Then
				throwEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer)
			If Not b Then
				throwEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long)
			If Not b Then
				throwEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double)
			If Not b Then
				throwEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object)
			If Not b Then
				throwEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer, ByVal arg4 As Integer)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long, ByVal arg4 As Long)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub


		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double, ByVal arg4 As Double)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object, ByVal arg5 As Object)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4, arg5)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkArgument(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object, ByVal arg5 As Object, ByVal arg6 As Object)
			If Not b Then
				throwEx(msg, arg1, arg2, arg3, arg4, arg5, arg6)
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalArgumentException with the specified message if {@code b} is false.
		''' Note that the message may specify argument locations using "%s" - for example,
		''' {@code checkArgument(false, "Got %s values, expected %s", 3, "more"} would throw an IllegalArgumentException
		''' with the message "Got 3 values, expected more"
		''' </summary>
		''' <param name="b">       Argument to check </param>
		''' <param name="message"> Message for exception. May be null. </param>
		''' <param name="args">    Arguments to place in message </param>
		Public Shared Sub checkArgument(ByVal b As Boolean, ByVal message As String, ParamArray ByVal args() As Object)
			If Not b Then
				throwEx(message, args)
			End If
		End Sub


		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalStateException if {@code b} is false
		''' </summary>
		''' <param name="b"> State to check </param>
		Public Shared Sub checkState(ByVal b As Boolean)
			If Not b Then
				Throw New System.InvalidOperationException()
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalStateException with the specified message if {@code b} is false
		''' </summary>
		''' <param name="b">       State to check </param>
		''' <param name="message"> Message for exception. May be null </param>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal message As String)
			If Not b Then
				throwStateEx(message)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer)
			If Not b Then
				throwStateEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long)
			If Not b Then
				throwStateEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double)
			If Not b Then
				throwStateEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object)
			If Not b Then
				throwStateEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer)
			If Not b Then
				throwStateEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long)
			If Not b Then
				throwStateEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double)
			If Not b Then
				throwStateEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object)
			If Not b Then
				throwStateEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer, ByVal arg4 As Integer)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long, ByVal arg4 As Long)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double, ByVal arg4 As Double)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object, ByVal arg5 As Object)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4, arg5)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkState(Boolean, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object, ByVal arg5 As Object, ByVal arg6 As Object)
			If Not b Then
				throwStateEx(msg, arg1, arg2, arg3, arg4, arg5, arg6)
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalStateException with the specified message if {@code b} is false.
		''' Note that the message may specify argument locations using "%s" - for example,
		''' {@code checkArgument(false, "Got %s values, expected %s", 3, "more"} would throw an IllegalStateException
		''' with the message "Got 3 values, expected more"
		''' </summary>
		''' <param name="b">       Argument to check </param>
		''' <param name="message"> Message for exception. May be null. </param>
		''' <param name="args">    Arguments to place in message </param>
		Public Shared Sub checkState(ByVal b As Boolean, ByVal message As String, ParamArray ByVal args() As Object)
			If Not b Then
				throwStateEx(message, args)
			End If
		End Sub


		''' <summary>
		''' Check the specified boolean argument. Throws an NullPointerException if {@code o} is false
		''' </summary>
		''' <param name="o"> Object to check </param>
		Public Shared Sub checkNotNull(ByVal o As Object)
			If o Is Nothing Then
				Throw New System.NullReferenceException()
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an NullPointerException with the specified message if {@code o} is false
		''' </summary>
		''' <param name="o">       Object to check </param>
		''' <param name="message"> Message for exception. May be null </param>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal message As String)
			If o Is Nothing Then
				throwNullPointerEx(message)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Integer)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Long)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Double)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Object)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Integer, ByVal arg2 As Integer, ByVal arg3 As Integer, ByVal arg4 As Integer)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Long, ByVal arg2 As Long, ByVal arg3 As Long, ByVal arg4 As Long)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Double, ByVal arg2 As Double, ByVal arg3 As Double, ByVal arg4 As Double)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="checkNotNull(Object, String, Object...)"/>
		''' </summary>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal msg As String, ByVal arg1 As Object, ByVal arg2 As Object, ByVal arg3 As Object, ByVal arg4 As Object)
			If o Is Nothing Then
				throwNullPointerEx(msg, arg1, arg2, arg3, arg4)
			End If
		End Sub

		''' <summary>
		''' Check the specified boolean argument. Throws an IllegalStateException with the specified message if {@code o} is false.
		''' Note that the message may specify argument locations using "%s" - for example,
		''' {@code checkArgument(false, "Got %s values, expected %s", 3, "more"} would throw an IllegalStateException
		''' with the message "Got 3 values, expected more"
		''' </summary>
		''' <param name="o">       Object to check </param>
		''' <param name="message"> Message for exception. May be null. </param>
		''' <param name="args">    Arguments to place in message </param>
		Public Shared Sub checkNotNull(ByVal o As Object, ByVal message As String, ParamArray ByVal args() As Object)
			If o Is Nothing Then
				throwStateEx(message, args)
			End If
		End Sub

		Public Shared Sub throwEx(ByVal message As String, ParamArray ByVal args() As Object)
			Dim f As String = format(message, args)
			Throw New System.ArgumentException(f)
		End Sub

		Public Shared Sub throwStateEx(ByVal message As String, ParamArray ByVal args() As Object)
			Dim f As String = format(message, args)
			Throw New System.InvalidOperationException(f)
		End Sub

		Public Shared Sub throwNullPointerEx(ByVal message As String, ParamArray ByVal args() As Object)
			Dim f As String = format(message, args)
			Throw New System.NullReferenceException(f)
		End Sub

		Private Shared Function format(ByVal message As String, ParamArray ByVal args() As Object) As String
			If message Is Nothing Then
				message = ""
			End If
			If args Is Nothing Then
				args = New Object(){"null"}
			End If

			Dim sb As New StringBuilder()

			Dim indexOfStart As Integer = 0
			Dim consumedMessageFully As Boolean = False
			For i As Integer = 0 To args.Length - 1
				'First: scan for next tag. This could be a %s, or it could be a custom loader for Preconditions class (PreconditionsFormat)
				Dim nextIdx As Integer = message.IndexOf("%s", indexOfStart, StringComparison.Ordinal)

				Dim nextCustom As Integer = -1
				Dim nextCustomTag As String = Nothing
				For Each s As String In FORMATTERS.Keys
					Dim idxThis As Integer = message.IndexOf(s, indexOfStart, StringComparison.Ordinal)
					If idxThis > 0 AndAlso (nextCustom < 0 OrElse idxThis < nextCustom) Then
						nextCustom = idxThis
						nextCustomTag = s
					End If
				Next s

				If nextIdx < 0 AndAlso nextCustom < 0 Then
					'Malformed message: No more "%s" (or custom tags) to replace, but more message args
					If Not consumedMessageFully Then
						sb.Append(message.Substring(indexOfStart))
						consumedMessageFully = True
						sb.Append(" [")
						Do While i < args.Length
							sb.Append(formatArg(args(i)))
							If i < args.Length - 1 Then
								sb.Append(",")
							End If
							i += 1
						Loop
						sb.Append("]")
					End If
				Else
					If nextCustom < 0 OrElse (nextIdx > 0 AndAlso nextIdx < nextCustom) Then
						'%s tag
						sb.Append(message.Substring(indexOfStart, nextIdx - indexOfStart)).Append(formatArg(args(i)))
						indexOfStart = nextIdx + 2
					Else
						'Custom tag
						sb.Append(message.Substring(indexOfStart, nextCustom - indexOfStart))
						Dim s As String = FORMATTERS(nextCustomTag).format(nextCustomTag, args(i))
						sb.Append(s)
						indexOfStart = nextCustom + nextCustomTag.Length
					End If
				End If
			Next i
			If Not consumedMessageFully Then
				sb.Append(message.Substring(indexOfStart))
			End If

			Return sb.ToString()
		End Function

		Private Shared Function formatArg(ByVal o As Object) As String
			If o Is Nothing Then
				Return "null"
			End If
			If o.GetType().IsArray Then
				Return formatArray(o)
			End If
			Return o.ToString()
		End Function

		Public Shared Function formatArray(ByVal o As Object) As String
			If o Is Nothing Then
				Return "null"
			End If

			If o.GetType().GetElementType().isPrimitive() Then
				If TypeOf o Is SByte() Then
					Return java.util.Arrays.toString(DirectCast(o, SByte()))
				ElseIf TypeOf o Is Integer() Then
					Return java.util.Arrays.toString(DirectCast(o, Integer()))
				ElseIf TypeOf o Is Long() Then
					Return java.util.Arrays.toString(DirectCast(o, Long()))
				ElseIf TypeOf o Is Single() Then
					Return java.util.Arrays.toString(DirectCast(o, Single()))
				ElseIf TypeOf o Is Double() Then
					Return java.util.Arrays.toString(DirectCast(o, Double()))
				ElseIf TypeOf o Is Char() Then
					Return java.util.Arrays.toString(DirectCast(o, Char()))
				ElseIf TypeOf o Is Boolean() Then
					Return java.util.Arrays.toString(DirectCast(o, Boolean()))
				ElseIf TypeOf o Is Short() Then
					Return java.util.Arrays.toString(DirectCast(o, Short()))
				Else
					'Should never happen
					Return o.ToString()
				End If
			Else
				Dim arr() As Object = DirectCast(o, Object())
				Return java.util.Arrays.toString(arr)
			End If
		End Function

	End Class

End Namespace