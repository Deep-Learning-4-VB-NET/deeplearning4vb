Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
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

    ''' <summary>
    ''' Gather op
    ''' </summary>
    Public Class Gather
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend indices() As Integer
        Protected Friend jaxis As Integer = 0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal df As SDVariable, ByVal indices As SDVariable, ByVal axis As Integer)
			Me.New(sameDiff, df, indices, axis, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal df As SDVariable, ByVal indices() As Integer, ByVal axis As Integer)
			Me.New(sameDiff, df, indices, axis, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices() As Integer, ByVal axis As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, inPlace)

			addIArgument(axis)
			addIArgument(indices)
			Me.jaxis = axis
			Me.indices = indices
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal indices As SDVariable, ByVal axis As Integer, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, indices}, inPlace)
			addIArgument(axis)
			Me.jaxis = axis
		End Sub
#End If

        Public Sub New(ByVal df As INDArray, ByVal indexes() As Integer, ByVal axis As Integer)
            addInputArgument(df)
            addIArgument(axis)
            addIArgument(indexes)
            Me.jaxis = axis
            Me.indices = indices
        End Sub

        Public Sub New(ByVal df As INDArray, ByVal indexes As INDArray, ByVal axis As Integer)
            addInputArgument(df, indexes)
            addIArgument(axis)
            Me.jaxis = axis
            Me.indices = indices
        End Sub

        Public Overrides Function onnxName() As String
            Return "Gather"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Gather", "GatherV2"}
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim broadcast As val = PropertyMapping.builder().onnxAttrName("indices").tfInputPosition(1).propertyNames(New String(){"indices"}).build()

			map("indices") = broadcast

			ret(tensorflowNames()(0)) = map
			ret(onnxName()) = map

			Dim map2 As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()
			Dim broadcast2 As val = PropertyMapping.builder().tfInputPosition(1).propertyNames(New String(){"indices"}).build()
			map2("indices") = broadcast2

			Dim axis2 As val = PropertyMapping.builder().tfInputPosition(2).propertyNames(New String(){"axis"}).build()
			map2("axis") = axis2

			ret("GatherV2") = map2

			Return ret
		End Function
#End If

        Public Overrides Function opName() As String
            Return "gather"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'2 args: input and indices. Plus integer dimension arg
			'Gather backprop is just scatter add

			Dim indicesGrad As SDVariable = sameDiff.zerosLike(arg(1))
			Dim inputGrad As SDVariable = sameDiff.zerosLike(arg(0))

			Dim inputs() As SDVariable = args()
			Dim axis As SDVariable
			Dim rank As SDVariable = inputs(0).rank()
			If inputs.Length = 2 Then
				axis = sameDiff.constant(jaxis)
				If jaxis < 0 Then
					axis = axis.add(rank)
				End If
			Else
				axis = inputs(2)
			End If

			'Use scatter add plus permute
			Dim dimsExAxis As SDVariable = sameDiff.range(Nothing, sameDiff.constant(0), rank, sameDiff.constant(1), DataType.INT)
			Dim axisRank1 As SDVariable = axis.reshape(1)
			dimsExAxis = sameDiff.math().listDiff(dimsExAxis, axisRank1)(0) 'Don't need indices
			Dim permuteDims As SDVariable = sameDiff.concat(0, axisRank1, dimsExAxis)
			Dim invertDims As SDVariable = sameDiff.invertPermutation(permuteDims)

			'Permute gradients so original axis is at position 0... then scatter add, and reverse
			Dim gradAtOut As SDVariable = i_v(0)
			Dim permuteGrad As SDVariable = gradAtOut.permute(permuteDims)
			Dim inputGradPermute As SDVariable = inputGrad.permute(permuteDims)
			inputGrad = sameDiff.scatterAdd(inputGradPermute, arg(1), permuteGrad)

			'Now, invert the permutation so axis is back where it was
			inputGrad = inputGrad.permute(invertDims)

			Return New List(Of SDVariable) From {inputGrad, indicesGrad}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output type is same as (first) input type
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
