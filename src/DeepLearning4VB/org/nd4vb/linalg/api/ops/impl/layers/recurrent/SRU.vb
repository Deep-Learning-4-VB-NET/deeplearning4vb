Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports SRUWeights = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights.SRUWeights
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>sru</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU</c>).</summary>
    Public Class SRU
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private weights As SRUWeights

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.autodiff.samediff.SDVariable mask;
		Private mask As SDVariable

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SRU(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable initialC, org.nd4j.autodiff.samediff.SDVariable mask, @NonNull SRUWeights weights)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal initialC As SDVariable, ByVal mask As SDVariable, ByVal weights As SRUWeights)
			MyBase.New(Nothing, sameDiff, wrapFilterNull(x, weights.getWeights(), weights.getBias(), initialC, mask))
			Me.mask = mask
			Me.weights = weights
		End Sub

#End If

        ''' <summary>
        ''' The op over arrays with a mask. Java also stores the mask in its SDVariable field, a cast that fails for a
        ''' non-null array; here the mask is only an input array.
        ''' </summary>
        Public Sub New(ByVal x As INDArray, ByVal initialC As INDArray, ByVal mask As INDArray, ByVal sruWeights As SRUWeights)
            MyBase.New(wrapFilterNull(x, sruWeights.getIWeights(), sruWeights.getIBias(), initialC, mask), Nothing)
            Me.weights = sruWeights
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal initialC As INDArray, ByVal sruWeights As SRUWeights)
            MyBase.New(wrapFilterNull(x, sruWeights.getIWeights(), sruWeights.getIBias(), initialC), Nothing)
            Me.weights = sruWeights
        End Sub

        Public Overrides Function opName() As String
            Return "sru"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op name for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op name for " & opName())
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>weights</c>.</summary>
        Public Overridable Function getWeights() As SRUWeights
            Return weights
        End Function

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>mask</c>.</summary>
        Public Overridable Function getMask() As SDVariable
            Return mask
        End Function
#End If

    End Class

End Namespace
