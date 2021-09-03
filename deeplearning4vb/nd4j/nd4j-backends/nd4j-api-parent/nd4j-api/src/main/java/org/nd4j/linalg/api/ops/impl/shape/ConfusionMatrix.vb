Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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


	''' 
	Public Class ConfusionMatrix
		Inherits DynamicCustomOp

		Public Const DEFAULT_DTYPE As DataType = DataType.INT

		Private outputType As DataType = DEFAULT_DTYPE

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, @NonNull DataType dataType)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal dataType As DataType)
			MyBase.New(New INDArray(){labels, predicted}, Nothing)
			Me.outputType = dataType
			addDArgument(dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, int numClasses)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal numClasses As Integer)
			Me.New(labels, predicted, numClasses, DEFAULT_DTYPE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, org.nd4j.linalg.api.ndarray.INDArray weights)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray)
			Me.New(labels, predicted, weights, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, org.nd4j.linalg.api.ndarray.INDArray weights, System.Nullable<Integer> numClasses)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray, ByVal numClasses As Integer?)
			Me.New(labels, predicted, weights, numClasses, DEFAULT_DTYPE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, System.Nullable<Integer> numClasses, @NonNull DataType dataType)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal numClasses As Integer?, ByVal dataType As DataType)
			Me.New(labels, predicted, Nothing, numClasses, dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConfusionMatrix(@NonNull INDArray labels, @NonNull INDArray predicted, org.nd4j.linalg.api.ndarray.INDArray weights, System.Nullable<Integer> numClasses, @NonNull DataType dataType)
		Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray, ByVal numClasses As Integer?, ByVal dataType As DataType)
			MyBase.New(wrapFilterNull(labels, predicted, weights), Nothing)
			Me.outputType = dataType
			If numClasses IsNot Nothing Then
				addIArgument(numClasses)
			End If
			addDArgument(dataType)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable, ByVal dataType As DataType)
			Me.New(sameDiff, labels, pred, weights)
			Me.outputType = dataType
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, pred})
			Me.outputType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, pred, weights})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal numClasses As Integer?)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, pred})
			addIArgument(numClasses)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable, ByVal numClasses As Integer?)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, pred, weights})
			addIArgument(numClasses)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal numClasses As Integer?, ByVal weights As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){labels, pred, weights})
			If numClasses IsNot Nothing Then
				addIArgument(numClasses)
			End If
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			'Looks like this is implemented in practice using a large collection of discrete ops - not single TF import op?
		End Sub

		Public Overrides Function opName() As String
			Return "confusion_matrix"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ConfusionMatrix"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg(0)), sameDiff.zerosLike(arg(1))}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(outputType)
		End Function
	End Class

End Namespace