Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
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


	''' <summary>
	''' Split op
	''' </summary>
	Public Class Split
		Inherits DynamicCustomOp

		Private numSplit As Integer
		Private splitDim As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal numSplit As Integer, ByVal splitDim As Integer)
			MyBase.New(Nothing,sameDiff,New SDVariable(){input})
			Me.numSplit = numSplit
			Me.splitDim = splitDim
			addIArgument(numSplit,splitDim)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Split(@NonNull INDArray in, org.nd4j.linalg.api.ndarray.INDArray out)
		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray)
			MyBase.New(Nothing, New INDArray(){[in]}, wrapOrNull([out]), Nothing, DirectCast(Nothing, IList(Of Integer)))
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal numSplit As Integer, ByVal splitDim As Integer)
			MyBase.New(Nothing,input,Nothing,java.util.Collections.emptyList(),New Integer(){})
			addIArgument(numSplit,splitDim)
		End Sub


		Public Overrides Function opName() As String
			Return "split"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Split"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim numSplits As val = CInt(Math.Truncate(attributesForNode("num_split").getI()))
			Me.numSplit = numSplits
			addIArgument(numSplits)

			Dim splitDim As val = TFGraphMapper.getArrayFrom(TFGraphMapper.getNodeWithNameFromGraph(graph,nodeDef.getInput(0)),graph)
			If splitDim IsNot Nothing Then
				Me.splitDim = splitDim.getInt(0)
				addIArgument(splitDim.getInt(0))
			End If
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim splitDim As val = PropertyMapping.builder().tfInputPosition(0).propertyNames(New String(){"splitDim"}).build()

			Dim numSplit As val = PropertyMapping.builder().tfAttrName("num_split").propertyNames(New String(){"numSplit"}).build()

			map("numSplit") = numSplit
			map("splitDim") = splitDim

			ret(tensorflowName()) = map

			Return ret
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return numSplit
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count > 0, "No datatypes were provided for %s: %s", Me.GetType(), dataTypes)
			Dim dt As DataType
			If dataTypes.Count = 1 Then
				dt = dataTypes(0)
			Else
				'Order seems to usually be axis first for TF import? libnd4j supports both...
				If dataTypes(0).isIntType() Then
					dt = dataTypes(1)
				Else
					dt = dataTypes(0)
				End If
			End If
			'Output types are same as first input type - just numSplits of them...
			Dim [out] As IList(Of DataType) = New List(Of DataType)(numSplit)
			For i As Integer = 0 To numSplit - 1
				[out].Add(dt)
			Next i
			Return [out]
		End Function
	End Class

End Namespace