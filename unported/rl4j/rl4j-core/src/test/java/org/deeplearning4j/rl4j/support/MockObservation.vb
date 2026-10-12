Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


	Public Class MockObservation
		Implements Encodable

'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend ReadOnly data_Conflict As INDArray

		Public Sub New(ByVal value As Integer)
			Me.data_Conflict = Nd4j.ones(1).mul(value)
		End Sub

		Public Overridable Function toArray() As Double() Implements Encodable.toArray
			Return data_Conflict.data().asDouble()
		End Function

		Public Overridable ReadOnly Property Skipped As Boolean Implements Encodable.isSkipped
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property Data As INDArray Implements Encodable.getData
			Get
				Return data_Conflict
			End Get
		End Property

		Public Overridable Function dup() As Encodable
			Return Nothing
		End Function
	End Class

End Namespace