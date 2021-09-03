Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor @Builder public class Triple<F, S, T> implements java.io.Serializable
	<Serializable>
	Public Class Triple(Of F, S, T)
		Private Const serialVersionUID As Long = 119L

		Protected Friend first As F
		Protected Friend second As S
		Protected Friend third As T


		Public Overridable ReadOnly Property Left As F
			Get
				Return first
			End Get
		End Property

		Public Overridable ReadOnly Property Middle As S
			Get
				Return second
			End Get
		End Property

		Public Overridable ReadOnly Property Right As T
			Get
				Return third
			End Get
		End Property

		Public Shared Function tripleOf(Of F, S, T)(ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
			Return New Triple(Of F, S, T)(first, second, third)
		End Function

		Public Shared Function [of](Of F, S, T)(ByVal first As F, ByVal second As S, ByVal third As T) As Triple(Of F, S, T)
			Return New Triple(Of F, S, T)(first, second, third)
		End Function
	End Class

End Namespace