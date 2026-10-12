Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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
'ORIGINAL LINE: @Slf4j @NoArgsConstructor public class ZerosLike extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ZerosLike
		Inherits DynamicCustomOp

		Protected Friend outputType As DataType 'Allow customizing dtype for TF import

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(Nothing, sameDiff, input, False, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable)
			Me.New(name, sameDiff, input, False, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dataType As DataType)
			Me.New(name, sameDiff, input, False, dataType)
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean)
			Me.New(name, sameDiff, input, inPlace, input.dataType())
		End Sub

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean, ByVal dataType As DataType)
			MyBase.New(name, sameDiff, New SDVariable(){input}, inPlace)
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray)
			Me.New([in], [out], [in].dataType())
		End Sub

		Public Sub New(ByVal [in] As INDArray)
			addInputArgument([in])
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray, ByVal dataType As DataType)
			MyBase.New(Nothing, [in], [out], Nothing, Nothing)
			If dataType <> Nothing Then
				addDArgument(dataType)
			End If
		End Sub


		Public Overrides Function opName() As String
			Return "zeroslike"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No op found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ZerosLike"
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("T") Then
				outputType = TFGraphMapper.convertType(attributesForNode("T").getType())
			End If
		End Sub


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.zerosLike(outputVariables()(0))
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 1, "Expected list with exactly 1 datatype for %s, got %s", Me.GetType(), dataTypes)
			If outputType <> Nothing Then
				Return Collections.singletonList(outputType)
			Else
				'Output type is same as input type
				Return dataTypes
			End If
		End Function

	End Class

End Namespace