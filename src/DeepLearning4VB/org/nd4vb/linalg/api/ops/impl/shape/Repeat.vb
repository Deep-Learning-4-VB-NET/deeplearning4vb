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

    ''' <summary>The custom op <c>repeat</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.Repeat</c>).</summary>
    Public Class Repeat
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private jaxis As Integer

        Public Sub New(ByVal axis As Integer)
            Me.jaxis = axis
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, args)
			Me.jaxis = axis
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer), ByVal axis As Integer)
            MyBase.New(Nothing, inputs, outputs, tArguments, iArguments)
            Me.jaxis = axis
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal axis As Integer)
            MyBase.New(Nothing, inputs, outputs)
            Me.jaxis = axis
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, args, inPlace)
			Me.jaxis = axis
		End Sub
#End If

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {{"axis", axis}}
        End Function

        Public Overrides Function opName() As String
            Return "repeat"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim axisMapping As val = PropertyMapping.builder().onnxAttrName("axis").tfInputPosition(-1).propertyNames(New String(){"axis"}).build()

			map("axis") = axisMapping

			ret(tensorflowName()) = map
			ret(onnxName()) = map

			Return ret
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addIArgument(jaxis)
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub
#End If

        Public Overrides Function onnxName() As String
            Return "Repeat"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = outputVariables()(0)
			Return Collections.singletonList(ret)
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Output type is always same as input type
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
