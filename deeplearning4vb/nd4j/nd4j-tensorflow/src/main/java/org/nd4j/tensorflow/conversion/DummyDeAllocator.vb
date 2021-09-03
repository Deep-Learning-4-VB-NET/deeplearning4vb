Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Deallocator_Pointer_long_Pointer = org.bytedeco.tensorflow.Deallocator_Pointer_long_Pointer

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

Namespace org.nd4j.tensorflow.conversion

	Public Class DummyDeAllocator
		Inherits Deallocator_Pointer_long_Pointer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As New DummyDeAllocator()

		Public Shared ReadOnly Property Instance As DummyDeAllocator
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overrides Sub [call](ByVal pointer As Pointer, ByVal l As Long, ByVal pointer1 As Pointer)
		End Sub
	End Class

End Namespace