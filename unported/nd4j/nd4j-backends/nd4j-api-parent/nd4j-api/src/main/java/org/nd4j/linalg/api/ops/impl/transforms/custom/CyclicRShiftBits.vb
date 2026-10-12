Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class CyclicRShiftBits
		Inherits BaseDynamicTransformOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal shift As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {x, shift},False)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal shift As INDArray, ByVal output As INDArray)
			MyBase.New(New INDArray(){input, shift}, New INDArray(){output})
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal shift As INDArray)
			Me.New(input, shift,input.ulike())
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "cyclic_rshift_bits"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No TensorFlow op opName found for " & opName())
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented: " & opName())
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes(0).isIntType(), "Input 0 datatype must be a integer type, got %s", dataTypes(0))
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace