Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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
	''' Upsampling3d operation
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class Upsampling3d extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Upsampling3d
		Inherits DynamicCustomOp


		Protected Friend ncdhw As Boolean
		Protected Friend scaleH As Integer
		Protected Friend scaleW As Integer
		Protected Friend scaleD As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal ncdhw As Boolean, ByVal scaleD As Integer, ByVal scaleH As Integer, ByVal scaleW As Integer)
			MyBase.New("upsampling3d",sameDiff, New SDVariable(){input})
			Me.ncdhw = ncdhw

			Me.scaleD = scaleD
			Me.scaleH = scaleH
			Me.scaleW = scaleW

			addIArgument(scaleD)
			addIArgument(scaleH)
			addIArgument(scaleW)
			addIArgument(scaleD)
			addIArgument(If(ncdhw, 1, 0))
		End Sub




		Public Sub New(ByVal input As INDArray, ByVal ncdhw As Boolean, ByVal scaleH As Integer, ByVal scaleW As Integer, ByVal scaleD As Integer)
			MyBase.New(New INDArray(){input}, Nothing)
			Me.ncdhw = ncdhw

			Me.scaleD = scaleD
			Me.scaleH = scaleH
			Me.scaleW = scaleW

			addIArgument(scaleD)
			addIArgument(scaleH)
			addIArgument(scaleW)
			addIArgument(scaleD)
			addIArgument(If(ncdhw, 0, 1))
		End Sub



		Public Overrides Function opName() As String
			Return "upsampling3d"
		End Function



		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New Upsampling3dBp(sameDiff, arg(0), f1(0), Me.ncdhw)).outputVariables()}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace