Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports SquaredDifferenceBpOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.SquaredDifferenceBpOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic


	Public Class SquaredDifferenceOp
		Inherits BaseDynamicTransformOp

		Public Const OP_NAME As String = "squaredsubtract"

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, New SDVariable(){x, y}, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			Me.New(sameDiff, x, y, False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal output As INDArray)
			MyBase.New(New INDArray(){x, y}, New INDArray(){output})
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			addInputArgument(New INDArray(){x, y})
		End Sub

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SquaredDifference"
		End Function

		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New SquaredDifferenceBpOp(sameDiff, New SDVariable(){larg(), rarg(), i_v1(0)})).outputs()
		End Function

	End Class

End Namespace