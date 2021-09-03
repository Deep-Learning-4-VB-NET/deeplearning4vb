Imports Value = lombok.Value
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

Namespace org.deeplearning4j.rl4j.space

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public class ArrayObservationSpace<O> implements ObservationSpace<O>
	Public Class ArrayObservationSpace(Of O)
		Implements ObservationSpace(Of O)

		Friend name As String
		Friend shape() As Integer
		Friend low As INDArray
		Friend high As INDArray

		Public Sub New(ByVal shape() As Integer)
			name = "Custom"
			Me.shape = shape
			low = Nd4j.create(1)
			high = Nd4j.create(1)
		End Sub

	End Class

End Namespace