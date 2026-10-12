Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LSTMConfiguration = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMConfiguration
Imports LSTMWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights
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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent


	Public Class LSTMBlockCell
		Inherits DynamicCustomOp

		Private configuration As LSTMConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMWeights weights;
		Private weights As LSTMWeights

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal weights As LSTMWeights, ByVal configuration As LSTMConfiguration)
			MyBase.New(Nothing, sameDiff, weights.argsWithInputs(x, cLast, yLast))
			Me.configuration = configuration
			Me.weights = weights
			addIArgument(configuration.iArgs(False))
			addTArgument(configuration.tArgs())
		End Sub

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

		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			configuration = LSTMConfiguration.builder().forgetBias(attributesForNode("forget_bias").getF()).clippingCellValue(attributesForNode("cell_clip").getF()).peepHole(attributesForNode("use_peephole").getB()).build()
			addIArgument(configuration.iArgs(False))
			addTArgument(configuration.tArgs())
		End Sub

		Public Overrides Function opName() As String
			Return "lstmBlockCell"
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If configuration IsNot Nothing Then
				Return configuration.toProperties(False)
			End If
			Return Collections.emptyMap()
		End Function

		Public Overrides Function tensorflowName() As String
			Return "LSTMBlockCell"
		End Function

	End Class

End Namespace