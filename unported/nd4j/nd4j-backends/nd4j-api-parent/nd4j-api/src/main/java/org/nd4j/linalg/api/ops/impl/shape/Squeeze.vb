Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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


	''' 
	Public Class Squeeze
		Inherits DynamicCustomOp

		Private squeezeDims() As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal squeezeDims As Integer)
			Me.New(sameDiff, arg, New Integer() {squeezeDims})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal squeezeDims() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){arg})
			Me.squeezeDims = squeezeDims
			addIArgument(squeezeDims)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal axis As Integer)
			addInputArgument(x)
			addIArgument(axis)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			nodeDef.getAttrMap().get("squeeze_dims")
			Dim dimList As IList(Of Long) = attributesForNode("squeeze_dims").getList().getIList()
			squeezeDims = New Integer(dimList.Count - 1){}
			For i As Integer = 0 To dimList.Count - 1
				squeezeDims(i) = dimList(i).intValue()
			Next i
			addIArgument(squeezeDims)
		End Sub

		Public Overrides Function opName() As String
			Return "squeeze"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Squeeze"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			If squeezeDims Is Nothing Then
				'TODO Strictly speaking this *is* possible by inspecting the input array
				Throw New System.InvalidOperationException("Cannot do Squeeze backprop with no dimensions")
			End If
			Dim ret As SDVariable = i_v(0)
			For Each d As Integer In squeezeDims
				ret = sameDiff.expandDims(ret, d)
			Next d

			Return New List(Of SDVariable) From {ret}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of org.nd4j.linalg.api.buffer.DataType)) As IList(Of org.nd4j.linalg.api.buffer.DataType)
			Preconditions.checkState(dataTypes.Count > 0, "Expected list with at least 1 datatype for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type
			Return New List(Of org.nd4j.linalg.api.buffer.DataType) From {dataTypes(0)}
		End Function
	End Class

End Namespace