Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' 
    Public Class ConfusionMatrix
        Inherits DynamicCustomOp

        Public Const DEFAULT_DTYPE As DataType = DataType.INT

        Private outputType As DataType = DEFAULT_DTYPE

        Public Sub New()
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal dataType As DataType)
            MyBase.New(New INDArray(){labels, predicted}, Nothing)
            Me.outputType = dataType
            addDArgument(dataType)
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal numClasses As Integer)
            Me.New(labels, predicted, numClasses, DEFAULT_DTYPE)
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray)
            Me.New(labels, predicted, weights, Nothing)
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray, ByVal numClasses As Integer?)
            Me.New(labels, predicted, weights, numClasses, DEFAULT_DTYPE)
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal numClasses As Integer?, ByVal dataType As DataType)
            Me.New(labels, predicted, Nothing, numClasses, dataType)
        End Sub

        Public Sub New(ByVal labels As INDArray, ByVal predicted As INDArray, ByVal weights As INDArray, ByVal numClasses As Integer?, ByVal dataType As DataType)
            MyBase.New(wrapFilterNull(labels, predicted, weights), Nothing)
            Me.outputType = dataType
            If numClasses IsNot Nothing Then
                addIArgument(numClasses.Value)
            End If
            addDArgument(dataType)
        End Sub

#If SAMEDIFF Then
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
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			'Looks like this is implemented in practice using a large collection of discrete ops - not single TF import op?
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "confusion_matrix"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ConfusionMatrix"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg(0)), sameDiff.zerosLike(arg(1))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {outputType}
        End Function

    End Class

End Namespace
