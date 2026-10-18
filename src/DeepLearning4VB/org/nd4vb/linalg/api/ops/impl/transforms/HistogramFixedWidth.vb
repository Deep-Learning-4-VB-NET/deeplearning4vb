Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms

    ''' <summary>The custom op <c>histogram_fixed_width</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.HistogramFixedWidth</c>).</summary>
    Public Class HistogramFixedWidth
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal values As SDVariable, ByVal valuesRange As SDVariable, ByVal numBins As SDVariable)
			MyBase.New(sameDiff,If(numBins Is Nothing, New SDVariable(){values, valuesRange}, New SDVariable()){values, valuesRange, numBins}, False)
		End Sub
#End If

        Public Sub New()
            'no-op
        End Sub

        Public Overrides Function opName() As String
            Return "histogram_fixed_width"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "HistogramFixedWidth"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'No op - just need the inputs
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            '1 or 2 possible: 2 for TF import (fill with specified value
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 2 OrElse dataTypes.Count = 3), "Expected 2 or 3 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            'TODO MAKE CONFIGURABLE
            Return New List(Of DataType) From {DataType.INT}
        End Function

    End Class

End Namespace
