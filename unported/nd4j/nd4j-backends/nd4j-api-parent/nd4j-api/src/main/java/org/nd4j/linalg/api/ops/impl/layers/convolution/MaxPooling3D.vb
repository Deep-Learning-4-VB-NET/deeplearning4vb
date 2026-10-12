Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class MaxPooling3D extends Pooling3D
	Public Class MaxPooling3D
		Inherits Pooling3D


		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal config As Pooling3DConfig)
			MyBase.New(sameDiff, New SDVariable(){input}, Nothing, Nothing, False, config, Pooling3DType.MAX)
		End Sub

		Public Sub New(ByVal arrayInput As INDArray, ByVal arrayOutput As INDArray, ByVal config As Pooling3DConfig)
			addInputArgument(arrayInput)
			If arrayOutput IsNot Nothing Then
				addOutputArgument(arrayOutput)
			End If
			Me.config = config
			addArgs()
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal pooling3DConfig As Pooling3DConfig)
			MyBase.New(Nothing, Nothing, New INDArray(){input},Nothing, False, pooling3DConfig, Pooling3DType.MAX)
		End Sub

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "config"
		End Function


		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config IsNot Nothing Then
				Return config.toProperties()
			End If
			Return Collections.emptyMap()
		End Function

		Public Overrides ReadOnly Property PoolingPrefix As String
			Get
				Return "max"
			End Get
		End Property

		Public Overrides Function opName() As String
			Return "maxpool3dnew"
		End Function

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub

		Public Overrides Function tensorflowName() As String
			Return "MaxPool3D"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace