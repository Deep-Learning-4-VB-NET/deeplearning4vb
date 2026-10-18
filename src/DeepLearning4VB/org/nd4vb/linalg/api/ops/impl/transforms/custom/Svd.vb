Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>svd</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Svd</c>).</summary>
    Public Class Svd
        Inherits DynamicCustomOp

        Public Const DEFAULT_SWITCHNUM As Integer = 16

        Private fullUV As Boolean
        Private computeUv As Boolean
        Private switchNum As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal full_matrices As Boolean, ByVal s As INDArray, ByVal u As INDArray, ByVal v As INDArray)
            inputArgumentsField.Add(input)
            fullUV = full_matrices
            computeUv = True
            switchNum = DEFAULT_SWITCHNUM

            outputArgumentsField.Add(s)
            outputArgumentsField.Add(u)
            outputArgumentsField.Add(v)

            addIArgument(ArrayUtil.fromBoolean(fullUV), ArrayUtil.fromBoolean(computeUv), switchNum)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUv As Boolean)
			Me.New(sd, input, fullUV, computeUv, DEFAULT_SWITCHNUM)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUv As Boolean, ByVal switchNum As Integer)
			MyBase.New(sd, New SDVariable(){input}, False)
			Me.fullUV = fullUV
			Me.computeUv = computeUv
			Me.switchNum = switchNum
			addIArgument(ArrayUtil.fromBoolean(fullUV), ArrayUtil.fromBoolean(computeUv), switchNum)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal fullUV As Boolean, ByVal computeUV As Boolean, ByVal switchNum As Integer)
            addInputArgument(input)
            addBArgument(fullUV, computeUV)
            addIArgument(switchNum)
        End Sub

        Public Overrides Function opName() As String
            Return "svd"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Svd"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Me.fullUV = attributesForNode("full_matrices").getB()
			Me.computeUv = attributesForNode("compute_uv").getB()
			Me.switchNum = 16
			addIArgument(ArrayUtil.fromBoolean(fullUV), ArrayUtil.fromBoolean(computeUv), switchNum)
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
#End If

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return If(computeUv, 3, 1)
            End Get
        End Property

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            If computeUv Then
                Dim d As DataType = dataTypes(0)
                Return New List(Of DataType) From {d, d, d}
            Else
                Return New List(Of DataType) From {dataTypes(0)}
            End If
        End Function

    End Class

End Namespace
