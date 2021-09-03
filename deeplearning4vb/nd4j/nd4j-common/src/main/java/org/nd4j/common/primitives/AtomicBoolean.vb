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

	Public Class AtomicBoolean
		Inherits java.util.concurrent.atomic.AtomicBoolean

		Public Sub New(ByVal initialValue As Boolean)
			MyBase.New(initialValue)
		End Sub

		Public Sub New()
			Me.New(False)
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If TypeOf o Is AtomicBoolean Then
				Return get() = DirectCast(o, AtomicBoolean).get()
			ElseIf TypeOf o Is Boolean? Then
				Return get() = (DirectCast(o, Boolean?))
			End If
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(get(), 1, 0)
		End Function

	End Class

End Namespace