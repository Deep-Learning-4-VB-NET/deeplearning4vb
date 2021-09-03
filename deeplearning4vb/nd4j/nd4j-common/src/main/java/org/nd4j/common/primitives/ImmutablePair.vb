Imports System
Imports lombok

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
'ORIGINAL LINE: @AllArgsConstructor @Data @Builder public class ImmutablePair<K, V> implements java.io.Serializable
	<Serializable>
	Public Class ImmutablePair(Of K, V)
		Private Const serialVersionUID As Long = 119L

		Protected Friend Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected K key;
		Protected Friend key As K
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected V value;
		Protected Friend value As V

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

		Public Overridable ReadOnly Property First As K
			Get
				Return key
			End Get
		End Property

		Public Overridable ReadOnly Property Second As V
			Get
				Return value
			End Get
		End Property


		Public Shared Function [of](Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
			Return New ImmutablePair(Of T, E)(key, value)
		End Function

		Public Shared Function makePair(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
			Return New ImmutablePair(Of T, E)(key, value)
		End Function

		Public Shared Function create(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
			Return New ImmutablePair(Of T, E)(key, value)
		End Function

		Public Shared Function pairOf(Of T, E)(ByVal key As T, ByVal value As E) As ImmutablePair(Of T, E)
			Return New ImmutablePair(Of T, E)(key, value)
		End Function
	End Class

End Namespace