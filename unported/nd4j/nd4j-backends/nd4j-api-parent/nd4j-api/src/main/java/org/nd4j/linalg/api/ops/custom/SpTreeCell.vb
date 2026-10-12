Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.custom

	Public Class SpTreeCell
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal corner As INDArray, ByVal width As INDArray, ByVal point As INDArray, ByVal N As Long, ByVal contains As Boolean)
			inputArguments_Conflict.Add(corner)
			inputArguments_Conflict.Add(width)
			inputArguments_Conflict.Add(point)

			iArguments.Add(N)

			outputArguments_Conflict.Add(Nd4j.scalar(contains))
		End Sub

		Public Overrides Function opName() As String
			Return "cell_contains"
		End Function
	End Class

End Namespace