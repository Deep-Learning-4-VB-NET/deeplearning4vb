Imports System
Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor public class Reshape extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Reshape
		Inherits DynamicCustomOp

		Private shape() As Long

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape() As Long)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v})
			Me.shape = shape
			'c ordering: see (char) 99 for c ordering and (char) 'f' is 102
			'note it has to be negative for the long array case only
			'to flag the difference between an ordering being specified
			'and a dimension.
			addIArgument(-99)
			addIArgument(shape)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal shape As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){i_v, shape})
		End Sub

		Public Sub New(ByVal [in] As INDArray, ParamArray ByVal shape() As Long)
			MyBase.New(New INDArray(){[in]}, Nothing)
			Me.shape = shape
			'c ordering: see (char) 99 for c ordering and (char) 'f' is 102
			'note it has to be negative for the long array case only
			'to flag the difference between an ordering being specified
			'and a dimension.
			addIArgument(-99)
			addIArgument(shape)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Reshape(@NonNull INDArray in, @NonNull INDArray shape, org.nd4j.linalg.api.ndarray.INDArray out)
		Public Sub New(ByVal [in] As INDArray, ByVal shape As INDArray, ByVal [out] As INDArray)
			MyBase.New(Nothing, New INDArray(){[in], shape}, wrapOrNull([out]), Nothing, DirectCast(Nothing, IList(Of Integer)))
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal shape As INDArray)
			Me.New([in], shape, Nothing)
		End Sub


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If Not nodeDef.containsAttr("TShape") AndAlso nodeDef.getInputCount() = 1 Then
				Me.shape = New Long(){}
				Return
			ElseIf nodeDef.getInputCount() = 1 Then
				Dim shape As val = nodeDef.getAttrOrThrow("Tshape")
				If Not shape.hasShape() Then
					Dim shapeRet As val = New Long(1){}
					shapeRet(0) = 1
					shapeRet(1) = shape.getValueCase().getNumber()
					Me.shape = shapeRet
				Else
					Dim shapeVals As val = shape.getShape().getDimList()
					If shapeVals.size() > 1 Then
						Me.shape = New Long(shapeVals.size() - 1){}
						For i As Integer = 0 To shapeVals.size() - 1
							Me.shape(i) = CInt(Math.Truncate(shapeVals.get(i).getSize()))
						Next i
					Else
						Me.shape = New Long(1){}
						Me.shape(0) = 1
						Me.shape(1) = CInt(Math.Truncate(shapeVals.get(0).getSize()))
					End If

				End If

				'all TF is c

				If Me.shape IsNot Nothing Then
					addIArgument(Me.shape)
				End If
			End If
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim shapeMapping As val = PropertyMapping.builder().onnxAttrName("shape").tfInputPosition(-1).propertyNames(New String(){"shape"}).build()

			map("shape") = shapeMapping

			ret(tensorflowName()) = map
			ret(onnxName()) = map

			Return ret
		End Function


		Public Overrides Function opName() As String
			Return "reshape"
		End Function

		Public Overrides Function onnxName() As String
			Return "Reshape"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Reshape"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim origShape As SDVariable = sameDiff.shape(arg())
			Dim ret As SDVariable = sameDiff.reshape(i_v(0), origShape)
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'Output type is always same as input type
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace