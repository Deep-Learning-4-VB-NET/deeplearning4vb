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

    ''' <summary>The custom op <c>stack</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.Stack</c>).</summary>
    Public Class Stack
        Inherits DynamicCustomOp

        Protected Friend jaxis As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal inputs() As INDArray, ByVal output As INDArray, ByVal axis As Integer)
            MyBase.New(Nothing, inputs,If(output Is Nothing, Nothing, New INDArray() {output}), Nothing, DirectCast(Nothing, IList(Of Integer)))
            Me.jaxis = axis
            addArgs()
        End Sub

        Public Sub New(ByVal input() As INDArray, ByVal axis As Integer)
            addInputArgument(input)
            Me.jaxis = axis
            addArgs()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal values As SDVariable, ByVal axis As Integer)
			Me.New(sameDiff, New SDVariable(){values}, axis)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal values() As SDVariable, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, values, False)
			Me.jaxis = axis
			addArgs()
		End Sub
#End If

        Public Overridable Sub addArgs()
            addIArgument(jaxis)
        End Sub

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "stack"
        End Function

        Public Overrides Function ToString() As String
            Return opName()
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Pack", "Stack"}
        End Function

        Public Overrides Function opName() As String
            Return "stack"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("No analog found for onnx for " & opName())
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim axisMapping As val = PropertyMapping.builder().onnxAttrName("axis").tfAttrName("axis").propertyNames(New String(){"jaxis"}).build()

			map("jaxis") = axisMapping

			For Each name As val In tensorflowNames()
				ret(name) = map
			Next name

			Return ret
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.unstack(f1(0), jaxis, args().Length)}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Dim first As DataType = dataTypes(0)
            For i As Integer = 1 To dataTypes.Count - 1
                Dim dt As DataType = dataTypes(i)
                Preconditions.checkState(first = dt, "All inputs must have same datatype - got %s and %s for inputs 0 and %s respectively", first, dt, i)
            Next i
            'Output type is same as input types
            Return New List(Of DataType) From {first}
        End Function

    End Class

End Namespace
