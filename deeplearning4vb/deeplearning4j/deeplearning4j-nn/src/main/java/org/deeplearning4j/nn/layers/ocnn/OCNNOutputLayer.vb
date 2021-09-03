Imports System
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationReLU = org.nd4j.linalg.activations.impl.ActivationReLU
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
import static org.deeplearning4j.nn.layers.ocnn.OCNNParamInitializer.R_KEY
import static org.deeplearning4j.nn.layers.ocnn.OCNNParamInitializer.V_KEY
import static org.deeplearning4j.nn.layers.ocnn.OCNNParamInitializer.W_KEY

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

Namespace org.deeplearning4j.nn.layers.ocnn


	<Serializable>
	Public Class OCNNOutputLayer
		Inherits BaseOutputLayer(Of org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter private org.nd4j.linalg.activations.IActivation activation = new org.nd4j.linalg.activations.impl.ActivationReLU();
		Private activation As IActivation = New ActivationReLU()
		Private Shared relu As IActivation = New ActivationReLU()


		Private lossFunction As ILossFunction

		Private batchWindowSizeIndex As Integer


		Private window As INDArray

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			Me.lossFunction = New OCNNLossFunction(Me)
			Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
			ocnnOutputLayer.setLossFn(Me.lossFunction)
		End Sub

		Public Overrides WriteOnly Property Labels As INDArray
			Set(ByVal labels As INDArray)
				'no-op
			End Set
		End Property


		''' <summary>
		''' Compute score after labels and input have been set. </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network </param>
		''' <param name="training"> whether score should be calculated at train or test time (this affects things like application of
		'''                 dropout, etc) </param>
		''' <returns> score (loss function) </returns>
		Public Overrides Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double
			If input_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Dim preOut As INDArray = preOutput2d(training, workspaceMgr)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()

			Dim score As Double = lossFunction.computeScore(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict,False)
			If conf().isMiniBatch() Then
				score /= InputMiniBatchSize
			End If

			score += fullNetRegTerm
			Me.score_Conflict = score
			Return score
		End Function

		Public Overrides Function needsLabels() As Boolean
			Return False
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim pair As Pair(Of Gradient, INDArray) = getGradientsAndDelta(preOutput2d(True, workspaceMgr), workspaceMgr) 'Returns Gradient and delta^(this), not Gradient and epsilon^(this-1)
			'150
			Dim inputShape As Long = CType(Me.getConf().getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer).getNIn()
			Dim delta As INDArray = pair.Second
			'4 x 150
			Dim epsilonNext As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input_Conflict.dataType(), New Long(){inputShape, delta.length()}, "f"c)
			epsilonNext = epsilonNext.assign(delta.broadcast(epsilonNext.shape())).transpose()

			'Normally we would clear weightNoiseParams here - but we want to reuse them for forward + backward + score
			' So this is instead done in MultiLayerNetwork/CompGraph backprop methods

			Return New Pair(Of Gradient, INDArray)(pair.First, epsilonNext)
		End Function


		''' <summary>
		''' Returns tuple: {Grafdient,Delta,Output} given preOut </summary>
		Private Function getGradientsAndDelta(ByVal preOut As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim labels2d As INDArray = getLabels2d(workspaceMgr, ArrayType.BP_WORKING_MEM)
			Dim delta As INDArray = lossFunction.computeGradient(labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict)
			Dim conf As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(Me.conf().getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)


			If conf.getLastEpochSinceRUpdated() = 0 AndAlso epochCount_Conflict = 0 Then
				Dim currentR As INDArray = doOutput(False,workspaceMgr)
				If window Is Nothing Then
					window = Nd4j.createUninitializedDetached(preOut.dataType(), conf.getWindowSize()).assign(0.0)
				End If

				If batchWindowSizeIndex < window.length() - currentR.length() Then
					window.put(New INDArrayIndex(){NDArrayIndex.interval(batchWindowSizeIndex,batchWindowSizeIndex + currentR.length())},currentR)
				ElseIf batchWindowSizeIndex < window.length() Then
					Dim windowIdx As Integer = CInt(window.length()) - batchWindowSizeIndex
					window.put(New INDArrayIndex(){NDArrayIndex.interval(window.length() - windowIdx,window.length())},currentR.get(NDArrayIndex.interval(0,windowIdx)))

				End If

				batchWindowSizeIndex += currentR.length()
				conf.setLastEpochSinceRUpdated(epochCount_Conflict)
			ElseIf conf.getLastEpochSinceRUpdated() <> epochCount_Conflict Then
				Dim percentile As Double = window.percentileNumber(100.0 * conf.getNu()).doubleValue()
				getParam(R_KEY).putScalar(0,percentile)
				conf.setLastEpochSinceRUpdated(epochCount_Conflict)
				batchWindowSizeIndex = 0
			Else
				'track a running average per minibatch per epoch
				'calculate the average r value quantl=ile
				'once the epoch changes

				Dim currentR As INDArray = doOutput(False,workspaceMgr)
				window.put(New INDArrayIndex(){NDArrayIndex.interval(batchWindowSizeIndex,batchWindowSizeIndex + currentR.length())},currentR)
			End If


			Dim gradient As Gradient = New DefaultGradient()
			Dim vGradView As INDArray = gradientViews(V_KEY)
			Dim oneDivNu As Double = 1.0 / layerConf().getNu()
			Dim xTimesV As INDArray = input_Conflict.mmul(getParam(V_KEY))
			Dim derivW As INDArray = layerConf().getActivationFn().getActivation(xTimesV.dup(),True).negi()
			Dim w As INDArray = getParam(W_KEY)
			derivW = derivW.muliColumnVector(delta).mean(0).muli(oneDivNu).addi(w.reshape(ChrW(w.length())))
			gradient.setGradientFor(W_KEY,gradientViews(W_KEY).assign(derivW))

			'dG -> sigmoid derivative

			Dim firstVertDerivV As INDArray = layerConf().getActivationFn().backprop(xTimesV.dup(),Nd4j.ones(input_Conflict.dataType(), xTimesV.shape())).getFirst().muliRowVector(getParam(W_KEY).neg())
			firstVertDerivV = firstVertDerivV.muliColumnVector(delta).reshape("f"c,input_Conflict.size(0),1,layerConf().getHiddenSize())
			Dim secondTermDerivV As INDArray = input_Conflict.reshape("f"c, input_Conflict.size(0),getParam(V_KEY).size(0),1)

			Dim shape((firstVertDerivV.shape().Length) - 1) As Long
			Dim i As Integer = 0
			Do While i < firstVertDerivV.rank()
				shape(i) = Math.Max(firstVertDerivV.size(i),secondTermDerivV.size(i))
				i += 1
			Loop

			Dim firstDerivVBroadcast As INDArray = Nd4j.createUninitialized(input_Conflict.dataType(), shape)

			Dim mulResult As INDArray = firstVertDerivV.broadcast(firstDerivVBroadcast)
			Dim bcDims() As Integer = {0, 1}
			Broadcast.mul(mulResult, secondTermDerivV, mulResult, bcDims)

			Dim derivV As INDArray = mulResult.mean(0).muli(oneDivNu).addi(getParam(V_KEY))
			gradient.setGradientFor(V_KEY,vGradView.assign(derivV))



			Dim derivR As INDArray = Nd4j.scalar(delta.meanNumber()).muli(oneDivNu).addi(-1)
			gradient.setGradientFor(R_KEY,gradientViews(R_KEY).assign(derivR))
			clearNoiseWeightParams()

			delta = backpropDropOutIfPresent(delta)
			Return New Pair(Of Gradient, INDArray)(gradient, delta)
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Me.input_Conflict = input
			Return doOutput(training,workspaceMgr)
		End Function

		''' <summary>
		'''{@inheritDoc}
		''' </summary>
		Public Overrides Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Throw New System.NotSupportedException()
		End Function


		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function


		Protected Friend Overrides Function preOutput2d(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return doOutput(training,workspaceMgr)
		End Function

		Protected Friend Overrides Function getLabels2d(ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			Return labels_Conflict
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return doOutput(training,workspaceMgr)
		End Function

		Private Function doOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Dim w As INDArray = getParamWithNoise(W_KEY,training,workspaceMgr)
			Dim v As INDArray = getParamWithNoise(V_KEY,training,workspaceMgr)
			applyDropOutIfNecessary(training, workspaceMgr)

			Dim first As INDArray = Nd4j.createUninitialized(input_Conflict.dataType(), input_Conflict.size(0), v.size(1))
			input_Conflict.mmuli(v, first)
			Dim act2d As INDArray = layerConf().getActivationFn().getActivation(first, training)
			Dim output As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input_Conflict.dataType(), input_Conflict.size(0))
			act2d.mmuli(w.reshape(ChrW(w.length())), output)
			Me.labels_Conflict = output
			Return output
		End Function




		''' <summary>
		'''Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overrides Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			'For RNN: need to sum up the score over each time step before returning.

			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Dim preOut As INDArray = preOutput2d(False, workspaceMgr)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict)
			Dim summedScores As INDArray = scoreArray.sum(1)

			If fullNetRegTerm <> 0.0 Then
				summedScores.addi(fullNetRegTerm)
			End If

			Return summedScores
		End Function

		<Serializable>
		Public Class OCNNLossFunction
			Implements ILossFunction

			Private ReadOnly outerInstance As OCNNOutputLayer

			Public Sub New(ByVal outerInstance As OCNNOutputLayer)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
				Dim wSum As Double = Transforms.pow(outerInstance.getParam(W_KEY),2).sumNumber().doubleValue() * 0.5
				Dim vSum As Double = Transforms.pow(outerInstance.getParam(V_KEY),2).sumNumber().doubleValue() * 0.5
				Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(outerInstance.conf().getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
				Dim rSubPre As INDArray = preOutput.rsub(outerInstance.getParam(R_KEY).getDouble(0))
				Dim rMeanSub As INDArray = relu.getActivation(rSubPre,True)
				Dim rMean As Double = rMeanSub.meanNumber().doubleValue()
				Dim rSum As Double = outerInstance.getParam(R_KEY).getDouble(0)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim nuDiv As Double = (1 / ocnnOutputLayer.getNu()) * rMean
				Dim lastTerm As Double = -rSum
				Return (wSum + vSum + nuDiv + lastTerm)
			End Function

			Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
				Dim r As INDArray = outerInstance.getParam(R_KEY).sub(preOutput)
				Return r
			End Function

			Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
				Dim preAct As INDArray = preOutput.rsub(outerInstance.getParam(R_KEY).getDouble(0))
				Dim target As INDArray = relu.backprop(preAct,Nd4j.ones(preOutput.dataType(), preAct.shape())).First
				Return target
			End Function

			Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
				'TODO: probably a more efficient way to do this...
				Return New Pair(Of Double, INDArray)(computeScore(labels, preOutput, activationFn, mask, average), computeGradient(labels, preOutput, activationFn, mask))
			End Function

			Public Overridable Function name() As String Implements ILossFunction.name
				Return "OCNNLossFunction"
			End Function
		End Class
	End Class

End Namespace