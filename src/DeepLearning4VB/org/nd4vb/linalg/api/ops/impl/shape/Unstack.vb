Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports ND4JIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
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

    ''' <summary>The custom op <c>unstack</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.Unstack</c>).</summary>
    Public Class Unstack
        Inherits DynamicCustomOp

        ' TODO: libnd4j currently doesn't support "num", number of outputs is inferred.
        Private num As Integer = -1
        Private jaxis As Integer

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal value As SDVariable, ByVal axis As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){value}, False)
			Me.jaxis = axis
			If value.Shape IsNot Nothing Then
				If value.Shape(axis) <> -1 Then
					num = CInt(Math.Truncate(value.Shape(axis)))
				End If
			End If
			If num <= 0 Then
				Throw New ND4JIllegalStateException("Unstack: Unable to infer number of outputs from input. Provide number of outputs explicitly.")
			End If
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal value As SDVariable, ByVal axis As Integer, ByVal num As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){value}, False)
			Me.jaxis = axis
			Me.num = num
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal value As INDArray, ByVal axis As Integer, ByVal num As Integer)
            MyBase.New(New INDArray(){value}, Nothing)
            Me.jaxis = axis
            Me.num = num
            addArgs()
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal [out]() As INDArray, ByVal axis As Integer)
            MyBase.New(Nothing, New INDArray(){[in]}, [out], Nothing, DirectCast(Nothing, Integer()))
            Me.jaxis = axis
            addArgs()
        End Sub

        Public Overridable Sub addArgs()
            addIArgument(jaxis)
        End Sub

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Unstack", "Unpack"}
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Unstack"
        End Function

        Public Overrides Function opName() As String
            Return "unstack"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim attrAxis As val = nodeDef.getAttrOrThrow("axis")
			Dim axis As Integer = CInt(Math.Truncate(attrAxis.getI()))
			Me.jaxis = axis
			Dim attrNum As val = nodeDef.getAttrOrDefault("num", Nothing)
			If attrNum IsNot Nothing Then
				Me.num = CInt(Math.Truncate(attrNum.getI()))
			End If
			addArgs()
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim axisMapping As val = PropertyMapping.builder().onnxAttrName("axis").tfInputPosition(-1).propertyNames(New String(){"axis"}).build()

			map("axis") = axisMapping

			ret(tensorflowName()) = map

			Return ret
		End Function

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("No analog found for onnx for " & opName())
		End Sub
#End If

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return num
            End Get
        End Property

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.stack(jaxis, CType(f1, List(Of SDVariable)).ToArray()))

		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
            'Output types are same as input type - i.e., just unpack rank R array into N rank R-1 arrays
            Dim [out] As IList(Of DataType) = New List(Of DataType)()
            For i As Integer = 0 To num - 1
                [out].Add(dataTypes(0))
            Next i
            Return [out]
        End Function

    End Class

End Namespace
