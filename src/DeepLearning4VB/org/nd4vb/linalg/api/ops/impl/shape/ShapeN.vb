Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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

    ''' <summary>The custom op <c>shapes_of</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.ShapeN</c>).</summary>
    Public Class ShapeN
        Inherits DynamicCustomOp

        Protected Friend dataTypeField As DataType?

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, inputs, inPlace)
		End Sub
#End If

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
        End Function

        Public Overrides Function opName() As String
            Return "shapes_of"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ShapeN"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)()
			For Each [in] As SDVariable In args()
				[out].Add(sameDiff.zerosLike([in]))
			Next [in]
			Return [out]
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length
			End Get
		End Property
#Else
        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return inputArguments().Count
            End Get
        End Property
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			dataType = TFGraphMapper.convertType(nodeDef.getAttrOrThrow("out_type").getType())
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output type is always long (i.e., shape of array) - for each input
            'TODO TF allows customizing int or long
            Dim n As Integer = NumOutputs
            Dim outputTypes As IList(Of DataType) = New List(Of DataType)(n)
            For i As Integer = 0 To n - 1
                outputTypes.Add(dataTypeField.GetValueOrDefault(DataType.LONG))
            Next i
            Return outputTypes
        End Function

    End Class

End Namespace
