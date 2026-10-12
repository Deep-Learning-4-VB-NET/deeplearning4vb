Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports TileBp = org.nd4j.linalg.api.ops.impl.shape.bp.TileBp
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


	Public Class Tile
		Inherits DynamicCustomOp

		Private jaxis() As Integer
		Private is_static_reps As Boolean = False

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal axis() As Integer)
			MyBase.New(Nothing,sameDiff, New SDVariable(){i_v}, False)
			Me.jaxis = axis
			addArguments()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal axis As SDVariable)
			MyBase.New(Nothing,sameDiff, New SDVariable(){i_v, axis}, False)
			Me.jaxis = Nothing
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal axis() As Integer, ByVal is_static_reps As Boolean)
			MyBase.New(Nothing, inputs, outputs)
			Me.jaxis = axis
			Me.is_static_reps = is_static_reps
			addArguments()
		End Sub


		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal axis() As Integer)
			Me.New(inputs,outputs,axis,False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal repeat As INDArray)
			MyBase.New(Nothing, New INDArray() {x, repeat}, Nothing)
			Me.jaxis = Nothing
		End Sub

		Public Sub New(ByVal inputs As INDArray, ParamArray ByVal axis() As Integer)
			MyBase.New(Nothing, New INDArray() {inputs}, Nothing)
			Me.jaxis = axis
			Me.is_static_reps = True
			addArguments()
		End Sub

		Public Sub New()
		End Sub

		Private Sub addArguments()
			Me.is_static_reps = True
			addIArgument(jaxis)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub


		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim axisMapping As val = PropertyMapping.builder().onnxAttrName("axis").tfInputPosition(-1).propertyNames(New String(){"axis"}).build()

			map("axis") = axisMapping

			ret(tensorflowName()) = map
			ret(onnxName()) = map

			Return ret
		End Function

		Public Overrides Function opName() As String
			Return "tile"
		End Function

		Public Overrides Function onnxName() As String
			Return "Tile"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Tile"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			If jaxis IsNot Nothing Then
				Return (New TileBp(sameDiff, arg(), i_v(0), jaxis)).outputs()
			Else
				Return (New TileBp(sameDiff, arg(0), arg(1), i_v(0))).outputs()
			End If
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'2nd isput is dynamic repeat
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse (jaxis Is Nothing AndAlso dataTypes.Count = 2)), "Expected 1 or 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace