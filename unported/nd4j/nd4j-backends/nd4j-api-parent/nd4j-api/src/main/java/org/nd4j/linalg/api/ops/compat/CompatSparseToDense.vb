Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.api.ops.compat

	Public Class CompatSparseToDense
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

		Public Sub New(ByVal indices As INDArray, ByVal shape As INDArray, ByVal values As INDArray)
			Preconditions.checkArgument(shape.Z AndAlso indices.Z, "Shape & indices arrays must have one integer data types")
			inputArguments_Conflict.Add(indices)
			inputArguments_Conflict.Add(shape)
			inputArguments_Conflict.Add(values)
		End Sub

		Public Sub New(ByVal indices As INDArray, ByVal shape As INDArray, ByVal values As INDArray, ByVal defaultVaule As INDArray)
			Me.New(indices, shape, values)
			Preconditions.checkArgument(defaultVaule.dataType() = values.dataType(), "Values array must have the same data type as defaultValue array")
			inputArguments_Conflict.Add(defaultVaule)
		End Sub

		Public Overrides Function opName() As String
			Return "compat_sparse_to_dense"
		End Function
	End Class

End Namespace