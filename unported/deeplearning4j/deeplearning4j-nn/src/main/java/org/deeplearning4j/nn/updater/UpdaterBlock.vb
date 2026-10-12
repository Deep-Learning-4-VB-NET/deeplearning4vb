Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Trainable = org.deeplearning4j.nn.api.Trainable
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.linalg.learning
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.deeplearning4j.nn.updater


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class UpdaterBlock
	Public Class UpdaterBlock
		Private paramOffsetStart As Integer
		Private paramOffsetEnd As Integer
		Private updaterViewOffsetStart As Integer
		Private updaterViewOffsetEnd As Integer
		Private layersAndVariablesInBlock As IList(Of ParamState) = New List(Of ParamState)()

		Private updaterView As INDArray
		Private gradientView As INDArray
		Private updaterViewRequiresInitialization As Boolean

'JAVA TO VB CONVERTER NOTE: The field gradientUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private gradientUpdater_Conflict As GradientUpdater


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class ParamState
		Public Class ParamState
			Friend ReadOnly layer As Trainable
			Friend ReadOnly paramName As String
			Friend ReadOnly paramOffsetStart As Integer
			Friend ReadOnly paramOffsetEnd As Integer
			Friend ReadOnly paramView As INDArray
			Friend ReadOnly gradView As INDArray
		End Class

		''' <param name="paramOffsetStart">          Start offset of the parameters in this block (relative to overall net params
		'''                                  view array) </param>
		''' <param name="paramOffsetEnd">            End offset of the parameters in this block (relative to overall net params
		'''                                  view array) </param>
		''' <param name="updaterViewOffsetStart">    Start offset of the updater state array in this block (relative to overall net
		'''                                  updater state view array) </param>
		''' <param name="updaterViewOffsetEnd">      End offset of the updater state array in this block (relative to overall net
		'''                                  updater state view array) </param>
		''' <param name="layersAndVariablesInBlock"> List of layers and variables in this updater block. By definition, all layers
		'''                                  and variables in this list <i>must</i> have an identical updater configuration. </param>
		Public Sub New(ByVal paramOffsetStart As Integer, ByVal paramOffsetEnd As Integer, ByVal updaterViewOffsetStart As Integer, ByVal updaterViewOffsetEnd As Integer, ByVal layersAndVariablesInBlock As IList(Of ParamState))
			Me.paramOffsetStart = paramOffsetStart
			Me.paramOffsetEnd = paramOffsetEnd
			Me.updaterViewOffsetStart = updaterViewOffsetStart
			Me.updaterViewOffsetEnd = updaterViewOffsetEnd
			Me.layersAndVariablesInBlock = layersAndVariablesInBlock
		End Sub

		Public Overridable Sub init()
			If gradientUpdater_Conflict Is Nothing Then
				Dim varState As ParamState = layersAndVariablesInBlock(0)
				Dim varName As String = varState.getParamName()
				gradientUpdater_Conflict = varState.getLayer().getConfig().getUpdaterByParam(varName).instantiate(updaterView, updaterViewRequiresInitialization) 'UpdaterUtils.getGradientUpdater(varState.getLayer(), varState.getParamName());
			End If
		End Sub

		Public Overridable ReadOnly Property PretrainUpdaterBlock As Boolean
			Get
				'All in block should be the same layer, and all be pretrain params
				Dim vs As ParamState = layersAndVariablesInBlock(0)
				Return vs.getLayer().getConfig().isPretrainParam(vs.getParamName())
			End Get
		End Property

		Public Overridable Function skipDueToPretrainConfig(ByVal isLayerUpdater As Boolean) As Boolean
			If Not PretrainUpdaterBlock Then
				Return False
			End If
			Return Not isLayerUpdater
		End Function

		Public Overridable ReadOnly Property GradientUpdater As GradientUpdater
			Get
				If gradientUpdater_Conflict Is Nothing Then
					init()
				End If
				Return gradientUpdater_Conflict
			End Get
		End Property

		''' <summary>
		''' Update the gradient for this block
		''' </summary>
		''' <param name="iteration"> The current iteration (i.e., total number of parameter updates so far) </param>
		Public Overridable Sub update(ByVal iteration As Integer, ByVal epoch As Integer)
			update(iteration, epoch, False, gradientView, Nothing)
		End Sub

		Public Overridable Sub updateExternalGradient(ByVal iteration As Integer, ByVal epoch As Integer, ByVal fullNetworkGradientView As INDArray, ByVal fullNetworkParamsArray As INDArray)
			'Extract the relevant subset from the external network
			update(iteration, epoch, True, fullNetworkGradientView, fullNetworkParamsArray)
		End Sub

		Private Sub update(ByVal iteration As Integer, ByVal epoch As Integer, ByVal externalGradient As Boolean, ByVal fullNetworkGradientView As INDArray, ByVal fullNetworkParamsArray As INDArray)
			'Initialize the updater, if necessary
			If gradientUpdater_Conflict Is Nothing Then
				init()
			End If

			Dim blockGradViewArray As INDArray
			If externalGradient Then
				blockGradViewArray = fullNetworkGradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramOffsetStart, paramOffsetEnd))
			Else
				blockGradViewArray = gradientView
			End If

			'First: Pre-apply gradient clipping etc: some are done on a per-layer basis
			'Therefore: it's already done by this point, in MultiLayerUpdater or ComputationGraphUpdater

			'Second: apply learning rate policy. Note that by definition we have the same LR policy for every single
			' variable in the block
			Dim l0 As Trainable = layersAndVariablesInBlock(0).getLayer()
			If l0.numParams() = 0 Then
				'No params for this layer
				Return
			End If

			'Pre-updater regularization: l1 and l2
			applyRegularizationAllVariables(Regularization.ApplyStep.BEFORE_UPDATER, iteration, epoch, externalGradient, fullNetworkGradientView, fullNetworkParamsArray)

			'Apply the updater itself
			gradientUpdater_Conflict.applyUpdater(blockGradViewArray, iteration, epoch)

			'Post updater regularization: weight decay
			applyRegularizationAllVariables(Regularization.ApplyStep.POST_UPDATER, iteration, epoch, externalGradient, fullNetworkGradientView, fullNetworkParamsArray)
		End Sub

		Protected Friend Overridable Sub applyRegularizationAllVariables(ByVal applyStep As Regularization.ApplyStep, ByVal iteration As Integer, ByVal epoch As Integer, ByVal externalGradient As Boolean, ByVal fullNetworkGradientView As INDArray, ByVal fullNetworkParamsArray As INDArray)
			For Each p As ParamState In layersAndVariablesInBlock
				Dim paramView As INDArray
				Dim gradView As INDArray
				If externalGradient Then
					paramView = fullNetworkParamsArray.get(NDArrayIndex.point(0), NDArrayIndex.interval(p.getParamOffsetStart(), p.getParamOffsetEnd()))
					gradView = fullNetworkGradientView.get(NDArrayIndex.point(0), NDArrayIndex.interval(p.getParamOffsetStart(), p.getParamOffsetEnd()))
				Else
					'Standard case
					paramView = p.getParamView()
					gradView = p.getGradView()
				End If

				Dim hasLR As Boolean = gradientUpdater_Conflict.getConfig().hasLearningRate()
				Dim lr As Double = (If(hasLR, gradientUpdater_Conflict.getConfig().getLearningRate(iteration, epoch), 1.0))
				applyRegularization(applyStep, p.getLayer(), p.getParamName(), gradView, paramView, iteration, epoch, lr)
			Next p
		End Sub

		''' <summary>
		''' Apply L1 and L2 regularization, if necessary. Note that L1/L2 may differ for different layers in the same block
		''' </summary>
		''' <param name="layer">        The layer to apply L1/L2 to </param>
		''' <param name="paramName">    Parameter name in the given layer </param>
		''' <param name="gradientView"> Gradient view array for the layer + param </param>
		''' <param name="paramsView">   Parameter view array for the layer + param </param>
		Protected Friend Overridable Sub applyRegularization(ByVal [step] As Regularization.ApplyStep, ByVal layer As Trainable, ByVal paramName As String, ByVal gradientView As INDArray, ByVal paramsView As INDArray, ByVal iter As Integer, ByVal epoch As Integer, ByVal lr As Double)
			'TODO: do this for multiple contiguous params/layers (fewer, larger ops)

			Dim l As IList(Of Regularization) = layer.Config.getRegularizationByParam(paramName)
			If l IsNot Nothing AndAlso l.Count > 0 Then
				For Each r As Regularization In l
					If r.applyStep() = [step] Then
						r.apply(paramsView, gradientView, lr, iter, epoch)
					End If
				Next r
			End If
		End Sub
	End Class

End Namespace