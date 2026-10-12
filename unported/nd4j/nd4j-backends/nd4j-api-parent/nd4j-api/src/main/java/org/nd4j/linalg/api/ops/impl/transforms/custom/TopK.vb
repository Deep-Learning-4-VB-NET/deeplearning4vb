Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class TopK
		Inherits DynamicCustomOp

		Private sorted As Boolean
		Private k As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal k As Integer, ByVal sorted As Boolean)
			MyBase.New(sd, New SDVariable(){[in]}, False)
			Me.k = k
			Me.sorted = sorted
			addIArgument(ArrayUtil.fromBoolean(sorted), k)
		End Sub

		Public Overrides Function opName() As String
			Return "top_k"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "TopKV2"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

			Dim thisName As String = nodeDef.getName()

			' FIXME: ????
			Dim inputName As String = thisName & "/k"
			Dim kNode As NodeDef = Nothing
			Dim i As Integer = 0
			Do While i < graph.getNodeCount()
				If graph.getNode(i).getName().Equals(inputName) Then
					kNode = graph.getNode(i)
					Exit Do
				End If
				i += 1
			Loop

			Me.sorted = nodeDef.getAttrOrThrow("sorted").getB()

			If kNode IsNot Nothing Then
				Preconditions.checkState(kNode IsNot Nothing, "Could not find 'k' parameter node for op: %s", thisName)

				Dim arr As INDArray = TFGraphMapper.getNDArrayFromTensor(kNode)
				Me.k = arr.getInt(0)

				addIArgument(ArrayUtil.fromBoolean(sorted), k)
			Else
				addIArgument(ArrayUtil.fromBoolean(sorted))
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not implemented yet")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'2 outputs: values and indices
			'TODO make thit configurable
			Return New List(Of DataType) From {dataTypes(0), DataType.INT}
		End Function
	End Class

End Namespace