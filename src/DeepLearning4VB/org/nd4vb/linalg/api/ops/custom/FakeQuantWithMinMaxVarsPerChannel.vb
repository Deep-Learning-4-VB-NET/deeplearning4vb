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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>fake_quant_with_min_max_vars_per_channel</c> (Java <c>org.nd4j.linalg.api.ops.custom.FakeQuantWithMinMaxVarsPerChannel</c>).</summary>
    Public Class FakeQuantWithMinMaxVarsPerChannel
        Inherits DynamicCustomOp

        Protected Friend narrowRange As Boolean
        Protected Friend numBits As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal min As INDArray, ByVal max As INDArray, ByVal num_bits As Integer, ByVal narrow As Boolean)
            Preconditions.checkArgument(min.Vector AndAlso max.Vector AndAlso min.length() = max.length(), "FakeQuantWithMinMaxVarsPerChannel: min and max should be 1D tensors with the same length")
            addInputArgument(x,min,max)
            addIArgument(num_bits)
            addBArgument(narrow)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal min As INDArray, ByVal max As INDArray, ByVal num_bits As Integer)
            Me.New(x, min, max, num_bits, False)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal min As INDArray, ByVal max As INDArray, ByVal narrow As Boolean)
            Me.New(x, min, max, 8, narrow)
        End Sub

        Public Sub New(ByVal x As INDArray, ByVal min As INDArray, ByVal max As INDArray)
            Me.New(x, min, max, 8, False)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal min As SDVariable, ByVal max As SDVariable, ByVal num_bits As Integer, ByVal narrow As Boolean)
			MyBase.New("", sameDiff, New SDVariable(){x, min, max})
			addIArgument(num_bits)
			addBArgument(narrow)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "fake_quant_with_min_max_vars_per_channel"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "FakeQuantWithMinMaxVarsPerChannel"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("narrow_range") Then
				Me.narrowRange = attributesForNode("narrow_range").getB()
			End If
			If attributesForNode.ContainsKey("num_bits") Then
				Me.numBits = CInt(Math.Truncate(attributesForNode("num_bits").getI()))
			End If
			addIArgument(numBits)
			addBArgument(narrowRange)
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected exactly 3 inputs, got %s", inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
