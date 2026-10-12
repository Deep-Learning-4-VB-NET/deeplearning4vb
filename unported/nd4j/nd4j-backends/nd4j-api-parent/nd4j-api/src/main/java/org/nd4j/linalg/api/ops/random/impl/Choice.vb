Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor

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

Namespace org.nd4j.linalg.api.ops.random.impl


	Public Class Choice
		Inherits BaseRandomOp

		Public Sub New()
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Choice(@NonNull INDArray source, @NonNull INDArray probabilities, @NonNull INDArray z)
		Public Sub New(ByVal source As INDArray, ByVal probabilities As INDArray, ByVal z As INDArray)
			MyBase.New(source, probabilities, z)
			Preconditions.checkArgument(source.dataType() = probabilities.dataType() AndAlso z.dataType() = source.dataType(), "Data types of all arguments should match")
			Preconditions.checkState(source.length() = probabilities.length(), "From & probabilities length mismatch: %s vs. %s", source.length(), probabilities.length())
			If probabilities.elementWiseStride() < 1 OrElse source.elementWiseStride() < 1 Then
				Throw New System.InvalidOperationException("Source and probabilities should have element-wise stride >= 1")
			End If
			Me.extraArgs = New Object() {0.0}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 5
		End Function

		Public Overrides Function opName() As String
			Return "choice"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Return calculateOutputShape()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Dim longShapeDescriptor As LongShapeDescriptor = LongShapeDescriptor.fromShape(shape,dataType)
			Return New List(Of LongShapeDescriptor) From {longShapeDescriptor}
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Choice does not have a derivative")
		End Function
	End Class

End Namespace