Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LSTMConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration
Imports LSTMWeights = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>lstmBlockCell</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell</c>).</summary>
    Public Class LSTMBlockCell
        Inherits DynamicCustomOp

        Private configuration As LSTMConfiguration

        Private weights As LSTMWeights

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal weights As LSTMWeights, ByVal configuration As LSTMConfiguration)
			MyBase.New(Nothing, sameDiff, weights.argsWithInputs(x, cLast, yLast))
			Me.configuration = configuration
			Me.weights = weights
			addIArgument(configuration.iArgs(False))
			addTArgument(configuration.tArgs())
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal cLast As INDArray, ByVal yLast As INDArray, ByVal lstmWeights As LSTMWeights, ByVal lstmConfiguration As LSTMConfiguration)
            MyBase.New(Nothing, Nothing, lstmWeights.argsWithInputs(x, cLast, yLast))
            Me.configuration = lstmConfiguration
            Me.weights = lstmWeights
            addIArgument(configuration.iArgs(False))
            addTArgument(configuration.tArgs())
        End Sub

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 8, "Expected exactly 8 inputs to LSTMBlockCell, got %s", inputDataTypes)
            '7 outputs, all of same type as input
            Dim dt As DataType = inputDataTypes(0)
            Preconditions.checkState(dt.isFPType(), "Input type 0 must be a floating point type, got %s", dt)
            Return New List(Of DataType) From {dt, dt, dt, dt, dt, dt, dt}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			configuration = LSTMConfiguration.builder().forgetBias(attributesForNode("forget_bias").getF()).clippingCellValue(attributesForNode("cell_clip").getF()).peepHole(attributesForNode("use_peephole").getB()).build()
			addIArgument(configuration.iArgs(False))
			addTArgument(configuration.tArgs())
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "lstmBlockCell"
        End Function

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            If configuration IsNot Nothing Then
                Return configuration.toProperties(False)
            End If
            Return New Dictionary(Of String, Object)()
        End Function

        Public Overrides Function tensorflowName() As String
            Return "LSTMBlockCell"
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>weights</c>.</summary>
        Public Overridable Function getWeights() As LSTMWeights
            Return weights
        End Function

    End Class

End Namespace
