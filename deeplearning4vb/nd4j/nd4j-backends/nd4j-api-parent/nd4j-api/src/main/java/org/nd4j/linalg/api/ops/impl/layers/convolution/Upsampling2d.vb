Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution



	''' <summary>
	''' Upsampling operation
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class Upsampling2d extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Upsampling2d
		Inherits DynamicCustomOp


		Protected Friend nchw As Boolean
		Protected Friend scaleH As Integer
		Protected Friend scaleW As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal nchw As Boolean, ByVal scaleH As Integer, ByVal scaleW As Integer)
			MyBase.New(Nothing,sameDiff, New SDVariable(){input})
			Me.nchw = nchw
			Me.scaleH = scaleH
			Me.scaleW = scaleW

			addIArgument(scaleH)
			addIArgument(scaleW)
			addIArgument(If(nchw, 1, 0))
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal nchw As Boolean)
			Me.New(sameDiff, input, nchw, scaleH, scaleW)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal scale As Integer)
			MyBase.New(Nothing,sameDiff, New SDVariable(){input})
			addIArgument(scale)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal scale As Integer)
			Me.New(input, scale, scale, True)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal nchw As Boolean)
			MyBase.New(New INDArray(){input}, Nothing)
			Me.nchw = nchw
			Me.scaleH = scaleH
			Me.scaleW = scaleW

			addIArgument(scaleH)
			addIArgument(scaleW)
			addIArgument(If(nchw, 1, 0))
		End Sub


		Public Overrides Function opName() As String
			Return "upsampling2d"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New Upsampling2dDerivative(sameDiff, arg(), f1(0), nchw, scaleH, scaleW)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace