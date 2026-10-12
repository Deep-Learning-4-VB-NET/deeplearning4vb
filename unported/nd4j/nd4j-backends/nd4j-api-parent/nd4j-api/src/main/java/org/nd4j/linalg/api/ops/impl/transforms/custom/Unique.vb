Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Unique extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Unique
		Inherits DynamicCustomOp

		Public Const DEFAULT_IDX_DTYPE As DataType = DataType.INT

		Private idxDataType As DataType

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sd, New SDVariable(){[in]}, False)
		End Sub

		Public Overrides Function opName() As String
			Return "unique"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Unique", "UniqueV2"}
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not implemented yet")
		End Function

		Public Overrides Function numOutputArguments() As Integer
			Return 2
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			idxDataType = TFGraphMapper.convertType(nodeDef.getAttrOrThrow("out_idx").getType())
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 1, "Expected exactly 1 or more input datatypes for %s, got %s", Me.GetType(), dataTypes)
			If dataTypes.Count > 1 Then
				log.warn("Using returning first data type of type " & dataTypes(0) & " for input")
			End If
			Return New List(Of DataType) From {dataTypes(0), (If(idxDataType = Nothing, DEFAULT_IDX_DTYPE, idxDataType))}
		End Function
	End Class

End Namespace