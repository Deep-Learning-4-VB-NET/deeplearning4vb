Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>dynamic_stitch</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.DynamicStitch</c>).</summary>
    Public Class DynamicStitch
        Inherits DynamicCustomOp

        Private numPartitions As Integer
#If SAMEDIFF Then
		Private indices() As SDVariable
#End If

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal indices() As SDVariable, ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sameDiff, ArrayUtils.addAll(indices, inputs), False)

			Me.indices = indices
			Me.numPartitions = inputs.Length
		End Sub
#End If

        Public Sub New(ByVal indices() As INDArray, ByVal inputs() As INDArray)
            MyBase.New(indices.Concat(inputs).ToArray(), Nothing)
        End Sub

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			' DynamicPartition and DynamicStitch are mutually inverse
			Dim gradient As SDVariable = i_v(0)
			Dim partitionData(indices.Length - 1) As SDVariable
			For i As Integer = 0 To indices.Length - 1
				partitionData(i) = sameDiff.onesLike(indices(i)).mul(i)
			Next i
			Dim partitions As SDVariable = sameDiff.dynamicStitch(indices, partitionData)

			Dim partition() As SDVariable = sameDiff.dynamicPartition(gradient, partitions, numPartitions)
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			For Each i As SDVariable In indices
				ret.Add(sameDiff.zerosLike(i))
			Next i
			Collections.addAll(ret, partition)
			Return ret
		End Function
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Me.numPartitions = CInt(Math.Truncate(attributesForNode("N").getI()))
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "dynamic_stitch"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"DynamicStitch", "ParallelDynamicStitch"}
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2*numPartitions, "Expected %s input datatypes for %s partitions for %s, got %s", 2*numPartitions, numPartitions, Me.GetType(), dataTypes)
            'Output type: same as (data) input type... only 1 output, however
            Dim inputType As DataType = dataTypes(dataTypes.Count - 1)
            Return New List(Of DataType) From {inputType}
        End Function

    End Class

End Namespace
