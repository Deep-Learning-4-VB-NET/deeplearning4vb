Imports org.deeplearning4j.rl4j.space
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.support

	Public Class MockObservationSpace
		Implements ObservationSpace

'JAVA TO VB CONVERTER NOTE: The field shape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly shape_Conflict() As Integer

		Public Sub New()
			Me.New(New Integer() { 1 })
		End Sub

		Public Sub New(ByVal shape() As Integer)
			Me.shape_Conflict = shape
		End Sub

		Public Overridable ReadOnly Property Name As String
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property Shape As Integer()
			Get
				Return shape_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Low As INDArray
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property High As INDArray
			Get
				Return Nothing
			End Get
		End Property
	End Class
End Namespace