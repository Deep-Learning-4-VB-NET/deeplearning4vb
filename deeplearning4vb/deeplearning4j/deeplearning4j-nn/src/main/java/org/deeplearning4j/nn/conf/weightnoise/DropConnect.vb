Imports System
Imports Data = lombok.Data
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DropOut = org.nd4j.linalg.api.ops.random.impl.DropOut
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.weightnoise

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class DropConnect implements IWeightNoise
	<Serializable>
	Public Class DropConnect
		Implements IWeightNoise

		Private weightRetainProb As Double
		Private weightRetainProbSchedule As ISchedule
		Private applyToBiases As Boolean

		''' <param name="weightRetainProbability"> Probability of retaining a weight </param>
		Public Sub New(ByVal weightRetainProbability As Double)
			Me.New(weightRetainProbability, False)
		End Sub

		''' <param name="weightRetainProbability"> Probability of retaining a weight </param>
		''' <param name="applyToBiases"> If true: apply to biases (default: weights only) </param>
		Public Sub New(ByVal weightRetainProbability As Double, ByVal applyToBiases As Boolean)
			Me.New(weightRetainProbability, Nothing, applyToBiases)
		End Sub

		''' <param name="weightRetainProbSchedule"> Probability (schedule) of retaining a weight </param>
		Public Sub New(ByVal weightRetainProbSchedule As ISchedule)
			Me.New(Double.NaN, weightRetainProbSchedule, False)
		End Sub

		''' <param name="weightRetainProbSchedule"> Probability (schedule) of retaining a weight </param>
		''' <param name="applyToBiases"> If true: apply to biases (default: weights only) </param>
		Public Sub New(ByVal weightRetainProbSchedule As ISchedule, ByVal applyToBiases As Boolean)
			Me.New(Double.NaN, weightRetainProbSchedule, applyToBiases)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private DropConnect(@JsonProperty("weightRetainProbability") double weightRetainProbability, @JsonProperty("weightRetainProbSchedule") org.nd4j.linalg.schedule.ISchedule weightRetainProbSchedule, @JsonProperty("applyToBiases") boolean applyToBiases)
		Private Sub New(ByVal weightRetainProbability As Double, ByVal weightRetainProbSchedule As ISchedule, ByVal applyToBiases As Boolean)
			Me.weightRetainProb = weightRetainProbability
			Me.weightRetainProbSchedule = weightRetainProbSchedule
			Me.applyToBiases = applyToBiases
		End Sub

		Public Overridable Function getParameter(ByVal layer As Layer, ByVal paramKey As String, ByVal iteration As Integer, ByVal epoch As Integer, ByVal train As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IWeightNoise.getParameter
			Dim init As ParamInitializer = layer.conf().getLayer().initializer()
			Dim param As INDArray = layer.getParam(paramKey)

			Dim p As Double
			If weightRetainProbSchedule Is Nothing Then
				p = weightRetainProb
			Else
				p = weightRetainProbSchedule.valueAt(iteration, epoch)
			End If

			If train AndAlso init.isWeightParam(layer.conf().getLayer(), paramKey) OrElse (applyToBiases AndAlso init.isBiasParam(layer.conf().getLayer(), paramKey)) Then
				Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.INPUT, param.dataType(), param.shape(), param.ordering())
				Nd4j.Executioner.exec(New DropOut(param, [out], p))
				Return [out]
			End If
			Return param
		End Function

		Public Overridable Function clone() As DropConnect
			Return New DropConnect(weightRetainProb, weightRetainProbSchedule, applyToBiases)
		End Function
	End Class

End Namespace