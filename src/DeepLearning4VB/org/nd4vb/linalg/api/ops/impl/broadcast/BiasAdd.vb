Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.broadcast

    ''' <summary>
    ''' Bias addition gradient operation.
    ''' </summary>
    Public Class BiasAdd
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend nchw As Boolean = True

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal bias As SDVariable, ByVal nchw As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, bias}, False)
			bArguments.Clear()
			bArguments.Add(nchw)
			Me.nchw = nchw
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal nchw As Boolean)
            Me.New(input, bias, Nothing, nchw)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal nchw As Boolean)
            MyBase.New(New INDArray(){input, bias}, wrapOrNull(output))
            bArguments.Clear()
            bArguments.Add(nchw)
            Me.nchw = nchw
        End Sub

        Public Overrides Function opName() As String
            Return "biasadd"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			If attributesForNode.ContainsKey("data_format") Then
				nchw = "NCHW".Equals(attributesForNode("data_format").getS().toStringUtf8(), StringComparison.OrdinalIgnoreCase)
			Else
				nchw = False 'TF default is NHWC
			End If
			bArguments.Clear()
			bArguments.Add(nchw)
		End Sub
#End If

        Public Overrides Function onnxName() As String
            Return "BiasAdd"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"BiasAdd", "BiasAddV1"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New BiasAddGrad(sameDiff, arg(0), arg(1), gradient(0), nchw)).outputs()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
