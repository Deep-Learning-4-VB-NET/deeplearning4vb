Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class SequenceMask extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class SequenceMask
		Inherits DynamicCustomOp

		Public Const DEFAULT_DTYPE As DataType = DataType.BOOL

		Private maxLen As Integer
		Private is_static_maxlen As Boolean = False
		Private dataType As DataType

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal maxLen As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, maxLen}, False)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal maxLen As Integer, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)
			Me.maxLen = maxLen
			Me.is_static_maxlen = True
			addIArgument(maxLen)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceMask(@NonNull INDArray input, int maxLen, org.nd4j.linalg.api.buffer.DataType dataType)
		Public Sub New(ByVal input As INDArray, ByVal maxLen As Integer, ByVal dataType As DataType)
			addInputArgument(input)
			addIArgument(maxLen)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceMask(@NonNull INDArray input, @NonNull DataType dataType)
		Public Sub New(ByVal input As INDArray, ByVal dataType As DataType)
			Me.New(input, Nothing, dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceMask(@NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray maxLength, @NonNull DataType dataType)
		Public Sub New(ByVal input As INDArray, ByVal maxLength As INDArray, ByVal dataType As DataType)
			MyBase.New(wrapFilterNull(input, maxLength), Nothing)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim targetNode As val = TFGraphMapper.getNodeWithNameFromGraph(graph, nodeDef.getInput(1))
'JAVA TO VB CONVERTER NOTE: The variable maxlen was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim maxlen_Conflict As val = TFGraphMapper.getNDArrayFromTensor(targetNode)
			If maxlen_Conflict Is Nothing Then
				' No 2nd input
				Me.is_static_maxlen = True
			End If
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			If is_static_maxlen Then
				addIArgument(Me.maxLen)
			End If

		End Sub
		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()
			If is_static_maxlen Then
				Dim maxLen As val = PropertyMapping.builder().propertyNames(New String(){"maxLen"}).tfAttrName("maxlen").build()
				attrs("maxLen") = maxLen
			End If
			ret(tensorflowName()) = attrs
			Return ret
		End Function

		Public Overrides Function opName() As String
			Return "sequence_mask"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'Input is integer indices
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim args() As SDVariable = Me.args()
			Preconditions.checkState(dataTypes.Count = args.Length, "Expected list with exactly %s datatypes for %s, got %s", args.Length, Me.GetType(), dataTypes)
			'Output type is same as input by default
			Return Collections.singletonList(If(dataType = Nothing, DEFAULT_DTYPE, dataType))
		End Function
	End Class

End Namespace