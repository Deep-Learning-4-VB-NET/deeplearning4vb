Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.gradient


	''' 
	Public Class GradientBackwardsMarker
		Inherits DynamicCustomOp

		Public Const OP_NAME As String = "gradientbackwards"

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){i_v1, i_v2})
		End Sub
		Public Sub New()
		End Sub

		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		''' <summary>
		''' The opName of this operation
		''' </summary>
		''' <returns> the opName of this operation </returns>
		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return i_v
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Dummy variable
			Return Collections.singletonList(DataType.FLOAT)
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Throw New System.NotSupportedException("calculateOutputShape() is not supported for control flow ops.")
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property
	End Class

End Namespace