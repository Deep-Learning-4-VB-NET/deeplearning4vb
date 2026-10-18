Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
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

    ''' <summary>The custom op <c>unique</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Unique</c>).</summary>
    Public Class Unique
        Inherits DynamicCustomOp

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Unique))

        Public Const DEFAULT_IDX_DTYPE As DataType = DataType.INT

        Private idxDataType As DataType?

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sd, New SDVariable(){[in]}, False)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "unique"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Unique", "UniqueV2"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
#End If

        Public Overrides Function numOutputArguments() As Integer
            Return 2
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			idxDataType = TFGraphMapper.convertType(nodeDef.getAttrOrThrow("out_idx").getType())
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 1, "Expected exactly 1 or more input datatypes for %s, got %s", Me.GetType(), dataTypes)
            If dataTypes.Count > 1 Then
                log.warn("Using returning first data type of type " & dataTypes(0) & " for input")
            End If
            Return New List(Of DataType) From {dataTypes(0), (idxDataType.GetValueOrDefault(DEFAULT_IDX_DTYPE))}
        End Function

    End Class

End Namespace
