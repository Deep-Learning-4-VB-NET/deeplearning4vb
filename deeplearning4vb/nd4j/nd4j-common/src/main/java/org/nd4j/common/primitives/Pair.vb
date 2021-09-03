Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.common.primitives

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor @Builder public class Pair<K, V> implements java.io.Serializable
	<Serializable>
	Public Class Pair(Of K, V)
		Private Const serialVersionUID As Long = 119L

		Protected Friend key As K
		Protected Friend value As V

		Public Overrides Function ToString() As String
			Return "Pair{" & "key=" & (If(TypeOf key Is Integer(), Arrays.toString(CType(key, Integer())), key)) & ", value=" & (If(TypeOf value Is Integer(), Arrays.toString(CType(value, Integer())), value)) & "}"c
		End Function

		Public Overridable ReadOnly Property Left As K
			Get
				Return key
			End Get
		End Property

		Public Overridable ReadOnly Property Right As V
			Get
				Return value
			End Get
		End Property

		Public Overridable Property First As K
			Get
				Return key
			End Get
			Set(ByVal first As K)
				key = first
			End Set
		End Property

		Public Overridable Property Second As V
			Get
				Return value
			End Get
			Set(ByVal second As V)
				value = second
			End Set
		End Property



		Public Shared Function [of](Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
			Return New Pair(Of T, E)(key, value)
		End Function

		Public Shared Function makePair(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
			Return New Pair(Of T, E)(key, value)
		End Function

		Public Shared Function create(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
			Return New Pair(Of T, E)(key, value)
		End Function

		Public Shared Function pairOf(Of T, E)(ByVal key As T, ByVal value As E) As Pair(Of T, E)
			Return New Pair(Of T, E)(key, value)
		End Function

		Public Shared Function fromArray(Of T)(ByVal arr() As T) As Pair(Of T, T)
			Preconditions.checkArgument(arr.Length = 2, "Can only create a pair from an array with two values, got %s", arr.Length)
			Return New Pair(Of T, T)(arr(0), arr(1))
		End Function
	End Class

End Namespace