Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

	Public Class BarnesEdgeForces
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal rowP As INDArray, ByVal colP As INDArray, ByVal valP As INDArray, ByVal dataP As INDArray, ByVal N As Long, ByVal output As INDArray)

			inputArguments_Conflict.Add(rowP)
			inputArguments_Conflict.Add(colP)
			inputArguments_Conflict.Add(valP)
			inputArguments_Conflict.Add(dataP)

			iArguments.Add(N)

			outputArguments_Conflict.Add(output)
		End Sub

		Public Overrides Function opName() As String
			Return "barnes_edge_forces"
		End Function
	End Class

End Namespace