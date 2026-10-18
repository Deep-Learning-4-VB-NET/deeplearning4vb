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
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
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

    ''' <summary>The custom op <c>reverse_v2</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ReverseV2</c>).</summary>
    Public Class ReverseV2
        Inherits DynamicCustomOp

        Protected Friend ReadOnly isLegacy As Boolean = True

        Public Sub New()
            iArguments.Add(If(isLegacy, 1L, 0L))
        End Sub

        Public Overrides Function opName() As String
            Return "reverse_v2"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ReverseV2"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("BP mode isn't supported for this op")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            '2nd input is dynamic axis
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
