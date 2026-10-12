Imports System
Imports Data = lombok.Data
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports Distributions = org.deeplearning4j.nn.conf.distribution.Distributions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @Data public class WeightNoise implements IWeightNoise
	<Serializable>
	Public Class WeightNoise
		Implements IWeightNoise

		Private distribution As Distribution
		Private applyToBias As Boolean
		Private additive As Boolean

		''' <param name="distribution"> Distribution for additive noise </param>
		Public Sub New(ByVal distribution As Distribution)
			Me.New(distribution, False, True)
		End Sub

		''' <param name="distribution"> Distribution for noise </param>
		''' <param name="additive">     If true: noise is added to weights. If false: noise is multiplied by weights </param>
		Public Sub New(ByVal distribution As Distribution, ByVal additive As Boolean)
			Me.New(distribution, False, additive)
		End Sub

		''' <param name="distribution"> Distribution for noise </param>
		''' <param name="applyToBias">  If true: apply to biases also. If false (default): apply only to weights </param>
		''' <param name="additive">     If true: noise is added to weights. If false: noise is multiplied by weights </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WeightNoise(@JsonProperty("distribution") org.deeplearning4j.nn.conf.distribution.Distribution distribution, @JsonProperty("applyToBias") boolean applyToBias, @JsonProperty("additive") boolean additive)
		Public Sub New(ByVal distribution As Distribution, ByVal applyToBias As Boolean, ByVal additive As Boolean)
			Me.distribution = distribution
			Me.applyToBias = applyToBias
			Me.additive = additive
		End Sub

		Public Overridable Function getParameter(ByVal layer As Layer, ByVal paramKey As String, ByVal iteration As Integer, ByVal epoch As Integer, ByVal train As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IWeightNoise.getParameter

			Dim init As ParamInitializer = layer.conf().getLayer().initializer()
			Dim param As INDArray = layer.getParam(paramKey)
			If train AndAlso init.isWeightParam(layer.conf().getLayer(), paramKey) OrElse (applyToBias AndAlso init.isBiasParam(layer.conf().getLayer(), paramKey)) Then

				Dim dist As org.nd4j.linalg.api.rng.distribution.Distribution = Distributions.createDistribution(distribution)
				Dim noise As INDArray = dist.sample(param.ulike())
				Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.INPUT, param.dataType(), param.shape(), param.ordering())

				If additive Then
					Nd4j.Executioner.exec(New AddOp(param, noise,[out]))
				Else
					Nd4j.Executioner.exec(New MulOp(param, noise, [out]))
				End If
				Return [out]
			End If
			Return param
		End Function

		Public Overridable Function clone() As WeightNoise
			Return New WeightNoise(distribution, applyToBias, additive)
		End Function
	End Class

End Namespace