Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Pooling3DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling3D</c>.</summary>
    Public Class AvgPooling3D
        Inherits Pooling3D

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal config As Pooling3DConfig)
			MyBase.New(sameDiff, New SDVariable(){input}, Nothing, Nothing, False, config, Pooling3DType.AVG)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal pooling3DConfig As Pooling3DConfig)
            MyBase.New(New INDArray() {input}, Nothing, False, pooling3DConfig, Pooling3DType.AVG)
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
            Return New Dictionary(Of String, Object)()
        End Function

        Public Overrides ReadOnly Property PoolingPrefix As String
            Get
                Return "avg"
            End Get
        End Property

        Public Overrides Function opName() As String
            Return "avgpool3dnew"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub
#End If

        Public Overrides Function tensorflowName() As String
            Return "AvgPool3D"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

    End Class

End Namespace
