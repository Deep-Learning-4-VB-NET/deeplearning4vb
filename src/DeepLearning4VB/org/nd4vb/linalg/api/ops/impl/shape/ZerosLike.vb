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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>zeroslike</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.ZerosLike</c>).</summary>
    Public Class ZerosLike
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend outputType As DataType? 'Allow customizing dtype for TF import

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(Nothing, sameDiff, input, False, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(name, sameDiff, input, False, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dataType As DataType)
			Me.New(name, sameDiff, input, False, dataType)
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean)
			Me.New(name, sameDiff, input, inPlace, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean, ByVal dataType As DataType)
			MyBase.New(name, sameDiff, New SDVariable(){input}, inPlace)
			addDArgument(dataType)
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray)
            Me.New([in], [out], [in].dataType())
        End Sub

        Public Sub New(ByVal [in] As INDArray)
            addInputArgument([in])
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray, ByVal dataType As DataType?)
            MyBase.New(Nothing, [in], [out], Nothing, Nothing)
            If dataType.HasValue Then
                addDArgument(dataType)
            End If
        End Sub

        Public Overrides Function opName() As String
            Return "zeroslike"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No op found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ZerosLike"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("T") Then
				outputType = TFGraphMapper.convertType(attributesForNode("T").getType())
			End If
		End Sub
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.zerosLike(outputVariables()(0))
			Return Collections.singletonList(ret)
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
            If outputType.HasValue Then
                Return New List(Of DataType) From {outputType}
            Else
                'Output type is same as input type
                Return dataTypes
            End If
        End Function

    End Class

End Namespace
