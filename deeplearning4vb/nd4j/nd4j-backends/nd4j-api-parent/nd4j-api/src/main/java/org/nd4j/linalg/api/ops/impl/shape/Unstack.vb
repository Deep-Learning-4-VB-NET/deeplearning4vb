Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.shape


	Public Class Unstack
		Inherits DynamicCustomOp

		' TODO: libnd4j currently doesn't support "num", number of outputs is inferred.
		Private num As Integer = -1
		Private jaxis As Integer

		Public Sub New()
		End Sub

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Unstack(@NonNull INDArray value, int axis, int num)
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

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return num
			End Get
		End Property

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.stack(jaxis, CType(f1, List(Of SDVariable)).ToArray()))

		End Function

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