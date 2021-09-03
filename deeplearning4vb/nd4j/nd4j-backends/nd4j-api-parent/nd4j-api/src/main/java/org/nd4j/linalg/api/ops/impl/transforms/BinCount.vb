Imports System.Collections.Generic
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

Namespace org.nd4j.linalg.api.ops.impl.transforms


	Public Class BinCount
		Inherits DynamicCustomOp

		Private minLength As Integer?
		Private maxLength As Integer?
		Private outputType As DataType

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal weights As SDVariable, ByVal minLength As Integer?, ByVal maxLength As Integer?, ByVal outputType As DataType)
			MyBase.New(sd,If(weights Is Nothing, New SDVariable(){[in]}, New SDVariable()){[in], weights}, False)
			Preconditions.checkState((minLength Is Nothing) <> (maxLength Is Nothing), "Cannot have only one of minLength and maxLength" & "non-null: both must be simultaneously null or non-null. minLength=%s, maxLength=%s", minLength, maxLength)
			Me.minLength = minLength
			Me.maxLength = maxLength
			Me.outputType = outputType
			addArgs()
		End Sub

		Private Sub addArgs()
			If minLength IsNot Nothing Then
				addIArgument(minLength)
			End If
			If maxLength IsNot Nothing Then
				addIArgument(maxLength)
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "bincount"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Bincount"
		End Function



		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("T") Then
				outputType = TFGraphMapper.convertType(attributesForNode("T").getType())
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputTypes IsNot Nothing AndAlso (inputTypes.Count >= 1 AndAlso inputTypes.Count <= 4), "Expected 1 to 4 input types, got %s for op %s", inputTypes, Me.GetType())

			'If weights present, same type as weights. Otherwise specified dtype
			If inputTypes.Count >= 2 Then
				'weights available case or TF import case (args 2/3 are min/max)
				Return Collections.singletonList(inputTypes(1))
			Else
				Preconditions.checkNotNull(outputType, "No output type available - output type must be set unless weights input is available")
				Return Collections.singletonList(outputType)
			End If
		End Function
	End Class

End Namespace