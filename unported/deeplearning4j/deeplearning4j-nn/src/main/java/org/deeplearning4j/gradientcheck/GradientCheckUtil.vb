Imports System
Imports System.Collections.Generic
Imports lombok
Imports Accessors = lombok.experimental.Accessors
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports org.nd4j.common.function
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports org.nd4j.common.primitives
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports org.deeplearning4j.nn.layers
Imports LossLayer = org.deeplearning4j.nn.layers.LossLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports UpdaterCreator = org.deeplearning4j.nn.updater.UpdaterCreator
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.gradientcheck


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class GradientCheckUtil
	Public Class GradientCheckUtil

		Private Shared ReadOnly VALID_ACTIVATION_FUNCTIONS As IList(Of Type) = New List(Of Type) From {Activation.CUBE.getActivationFunction().GetType(), Activation.ELU.getActivationFunction().GetType(), Activation.IDENTITY.getActivationFunction().GetType(), Activation.RATIONALTANH.getActivationFunction().GetType(), Activation.SIGMOID.getActivationFunction().GetType(), Activation.SOFTMAX.getActivationFunction().GetType(), Activation.SOFTPLUS.getActivationFunction().GetType(), Activation.SOFTSIGN.getActivationFunction().GetType(), Activation.TANH.getActivationFunction().GetType()}

		Private Sub New()
		End Sub


		Private Shared Sub configureLossFnClippingIfPresent(ByVal outputLayer As IOutputLayer)

			Dim lfn As ILossFunction = Nothing
			Dim afn As IActivation = Nothing
			If TypeOf outputLayer Is BaseOutputLayer Then
				Dim o As BaseOutputLayer = DirectCast(outputLayer, BaseOutputLayer)
				lfn = DirectCast(o.layerConf(), org.deeplearning4j.nn.conf.layers.BaseOutputLayer).getLossFn()
				afn = o.layerConf().getActivationFn()
			ElseIf TypeOf outputLayer Is LossLayer Then
				Dim o As LossLayer = DirectCast(outputLayer, LossLayer)
				lfn = o.layerConf().getLossFn()
				afn = o.layerConf().getActivationFn()
			End If

			If TypeOf lfn Is LossMCXENT AndAlso TypeOf afn Is ActivationSoftmax AndAlso DirectCast(lfn, LossMCXENT).getSoftmaxClipEps() <> 0 Then
				log.info("Setting softmax clipping epsilon to 0.0 for " & lfn.GetType() & " loss function to avoid spurious gradient check failures")
				DirectCast(lfn, LossMCXENT).setSoftmaxClipEps(0.0)
			ElseIf TypeOf lfn Is LossBinaryXENT AndAlso DirectCast(lfn, LossBinaryXENT).getClipEps() <> 0 Then
				log.info("Setting clipping epsilon to 0.0 for " & lfn.GetType() & " loss function to avoid spurious gradient check failures")
				DirectCast(lfn, LossBinaryXENT).setClipEps(0.0)
			End If
		End Sub

		Public Enum PrintMode
			ALL
			ZEROS
			FAILURES_ONLY
		End Enum

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Accessors(fluent = true) @Data @NoArgsConstructor public static class MLNConfig
		Public Class MLNConfig
			Friend net As MultiLayerNetwork
			Friend input As INDArray
			Friend labels As INDArray
			Friend inputMask As INDArray
			Friend labelMask As INDArray
			Friend epsilon As Double = 1e-6
			Friend maxRelError As Double = 1e-3
			Friend minAbsoluteError As Double = 1e-8
			Friend print As PrintMode = PrintMode.ZEROS
			Friend exitOnFirstError As Boolean = False
			Friend subset As Boolean
			Friend maxPerParam As Integer
			Friend excludeParams As ISet(Of String)
			Friend callEachIter As Consumer(Of MultiLayerNetwork)
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Accessors(fluent = true) @Data @NoArgsConstructor public static class GraphConfig
		Public Class GraphConfig
			Friend net As ComputationGraph
			Friend inputs() As INDArray
			Friend labels() As INDArray
			Friend inputMask() As INDArray
			Friend labelMask() As INDArray
			Friend epsilon As Double = 1e-6
			Friend maxRelError As Double = 1e-3
			Friend minAbsoluteError As Double = 1e-8
			Friend print As PrintMode = PrintMode.ZEROS
			Friend exitOnFirstError As Boolean = False
			Friend subset As Boolean
			Friend maxPerParam As Integer
			Friend excludeParams As ISet(Of String)
			Friend callEachIter As Consumer(Of ComputationGraph)
		End Class

		''' <summary>
		''' Check backprop gradients for a MultiLayerNetwork. </summary>
		''' <param name="mln"> MultiLayerNetwork to test. This must be initialized. </param>
		''' <param name="epsilon"> Usually on the order/ of 1e-4 or so. </param>
		''' <param name="maxRelError"> Maximum relative error. Usually < 1e-5 or so, though maybe more for deep networks or those with nonlinear activation </param>
		''' <param name="minAbsoluteError"> Minimum absolute error to cause a failure. Numerical gradients can be non-zero due to precision issues.
		'''                         For example, 0.0 vs. 1e-18: relative error is 1.0, but not really a failure </param>
		''' <param name="print"> Whether to print full pass/failure details for each parameter gradient </param>
		''' <param name="exitOnFirstError"> If true: return upon first failure. If false: continue checking even if
		'''  one parameter gradient has failed. Typically use false for debugging, true for unit tests. </param>
		''' <param name="input"> Input array to use for forward pass. May be mini-batch data. </param>
		''' <param name="labels"> Labels/targets to use to calculate backprop gradient. May be mini-batch data. </param>
		''' <returns> true if gradients are passed, false otherwise. </returns>
		<Obsolete>
		Public Shared Function checkGradients(ByVal mln As MultiLayerNetwork, ByVal epsilon As Double, ByVal maxRelError As Double, ByVal minAbsoluteError As Double, ByVal print As Boolean, ByVal exitOnFirstError As Boolean, ByVal input As INDArray, ByVal labels As INDArray) As Boolean
			Return checkGradients((New MLNConfig()).net(mln).epsilon(epsilon).maxRelError(maxRelError).minAbsoluteError(minAbsoluteError).print(PrintMode.FAILURES_ONLY).exitOnFirstError(exitOnFirstError).input(input).labels(labels))
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Deprecated public static boolean checkGradients(org.deeplearning4j.nn.multilayer.MultiLayerNetwork mln, double epsilon, double maxRelError, double minAbsoluteError, boolean print, boolean exitOnFirstError, org.nd4j.linalg.api.ndarray.INDArray input, org.nd4j.linalg.api.ndarray.INDArray labels, org.nd4j.linalg.api.ndarray.INDArray inputMask, org.nd4j.linalg.api.ndarray.INDArray labelMask, boolean subset, int maxPerParam, @Set<String> excludeParams, final System.Nullable<Integer> rngSeedResetEachIter)
		<Obsolete>
		Public Shared Function checkGradients(ByVal mln As MultiLayerNetwork, ByVal epsilon As Double, ByVal maxRelError As Double, ByVal minAbsoluteError As Double, ByVal print As Boolean, ByVal exitOnFirstError As Boolean, ByVal input As INDArray, ByVal labels As INDArray, ByVal inputMask As INDArray, ByVal labelMask As INDArray, ByVal subset As Boolean, ByVal maxPerParam As Integer, ByVal excludeParams As ISet(Of String), ByVal rngSeedResetEachIter As Integer?) As Boolean
			Dim c As Consumer(Of MultiLayerNetwork) = Nothing
			If rngSeedResetEachIter IsNot Nothing Then
				c = New ConsumerAnonymousInnerClass(rngSeedResetEachIter)
			End If

			Return checkGradients((New MLNConfig()).net(mln).epsilon(epsilon).maxRelError(maxRelError).minAbsoluteError(minAbsoluteError).print(PrintMode.FAILURES_ONLY).exitOnFirstError(exitOnFirstError).input(input).labels(labels).inputMask(inputMask).labelMask(labelMask).subset(subset).maxPerParam(maxPerParam).excludeParams(excludeParams).callEachIter(c))
		End Function

		Private Class ConsumerAnonymousInnerClass
			Implements Consumer(Of MultiLayerNetwork)

			Private rngSeedResetEachIter As Integer?

			Public Sub New(ByVal rngSeedResetEachIter As Integer?)
				Me.rngSeedResetEachIter = rngSeedResetEachIter
			End Sub

			Public Sub accept(ByVal multiLayerNetwork As MultiLayerNetwork)
				Nd4j.Random.setSeed(rngSeedResetEachIter)
			End Sub
		End Class

		Public Shared Function checkGradients(ByVal c As MLNConfig) As Boolean

			'Basic sanity checks on input:
			If c.epsilon <= 0.0 OrElse c.epsilon > 0.1 Then
				Throw New System.ArgumentException("Invalid epsilon: expect epsilon in range (0,0.1], usually 1e-4 or so")
			End If
			If c.maxRelError <= 0.0 OrElse c.maxRelError > 0.25 Then
				Throw New System.ArgumentException("Invalid maxRelativeError: " & c.maxRelError)
			End If
			If Not (TypeOf c.net.OutputLayer Is IOutputLayer) Then
				Throw New System.ArgumentException("Cannot check backprop gradients without OutputLayer")
			End If

			Dim dataType As DataType = DataTypeUtil.DtypeFromContext
			If dataType <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Cannot perform gradient check: Datatype is not set to double precision (" & "is: " & dataType & "). Double precision must be used for gradient checks. Set " & "DataTypeUtil.setDTypeForContext(DataType.DOUBLE); before using GradientCheckUtil")
			End If

			Dim netDataType As DataType = c.net.LayerWiseConfigurations.getDataType()
			If netDataType <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Cannot perform gradient check: Network datatype is not set to double precision (" & "is: " & netDataType & "). Double precision must be used for gradient checks. Create network with .dataType(DataType.DOUBLE) before using GradientCheckUtil")
			End If

			If netDataType <> c.net.params().dataType() Then
				Throw New System.InvalidOperationException("Parameters datatype does not match network configuration datatype (" & "is: " & c.net.params().dataType() & "). If network datatype is set to DOUBLE, parameters must also be DOUBLE.")
			End If


			'Check network configuration:
			Dim layerCount As Integer = 0
			For Each n As NeuralNetConfiguration In c.net.LayerWiseConfigurations.getConfs()
				If TypeOf n.getLayer() Is BaseLayer Then
					Dim bl As BaseLayer = CType(n.getLayer(), BaseLayer)
					Dim u As IUpdater = bl.getIUpdater()
					If TypeOf u Is Sgd Then
						'Must have LR of 1.0
						Dim lr As Double = DirectCast(u, Sgd).getLearningRate()
						If lr <> 1.0 Then
							Throw New System.InvalidOperationException("When using SGD updater, must also use lr=1.0 for layer " & layerCount & "; got " & u & " with lr=" & lr & " for layer """ & n.getLayer().getLayerName() & """")
						End If
					ElseIf Not (TypeOf u Is NoOp) Then
						Throw New System.InvalidOperationException("Must have Updater.NONE (or SGD + lr=1.0) for layer " & layerCount & "; got " & u)
					End If

					Dim activation As IActivation = bl.getActivationFn()
					If activation IsNot Nothing Then
						If Not VALID_ACTIVATION_FUNCTIONS.Contains(activation.GetType()) Then
							log.warn("Layer " & layerCount & " is possibly using an unsuitable activation function: " & activation.GetType() & ". Activation functions for gradient checks must be smooth (like sigmoid, tanh, softmax) and not " & "contain discontinuities like ReLU or LeakyReLU (these may cause spurious failures)")
						End If
					End If
				End If

				If n.getLayer().getIDropout() IsNot Nothing AndAlso c.callEachIter Is Nothing Then
					Throw New System.InvalidOperationException("When gradient checking dropout, need to reset RNG seed each iter, or no" & " dropout should be present during gradient checks - got dropout = " & n.getLayer().getIDropout() & " for layer " & layerCount)
				End If
			Next n

			'Set softmax clipping to 0 if necessary, to avoid spurious failures due to clipping
			For Each l As Layer In c.net.Layers
				If TypeOf l Is IOutputLayer Then
					configureLossFnClippingIfPresent(DirectCast(l, IOutputLayer))
				End If
			Next l

			c.net.Input = c.input
			c.net.Labels = c.labels
			c.net.setLayerMaskArrays(c.inputMask, c.labelMask)
			If c.callEachIter IsNot Nothing Then
				c.callEachIter.accept(c.net)
			End If
			c.net.computeGradientAndScore()
			Dim gradAndScore As Pair(Of Gradient, Double) = c.net.gradientAndScore()

			Dim updater As Updater = UpdaterCreator.getUpdater(c.net)
			updater.update(c.net, gradAndScore.First, 0, 0, c.net.batchSize(), LayerWorkspaceMgr.noWorkspaces())

			Dim gradientToCheck As INDArray = gradAndScore.First.gradient().dup() 'need dup: gradients are a *view* of the full gradient array (which will change every time backprop is done)
			Dim originalParams As INDArray = c.net.params().dup() 'need dup: params are a *view* of full parameters

			Dim nParams As val = originalParams.length()

			Dim paramTable As IDictionary(Of String, INDArray) = c.net.paramTable()
			Dim paramNames As IList(Of String) = New List(Of String)(paramTable.Keys)
			Dim paramEnds As val = New Long(paramNames.Count - 1){}
			paramEnds(0) = paramTable(paramNames(0)).length()
			Dim stepSizeForParam As IDictionary(Of String, Integer)
			If c.subset Then
				stepSizeForParam = New Dictionary(Of String, Integer)()
				stepSizeForParam(paramNames(0)) = CInt(Math.Max(1, paramTable(paramNames(0)).length() \ c.maxPerParam))
			Else
				stepSizeForParam = Nothing
			End If
			For i As Integer = 1 To paramEnds.length - 1
				Dim n As val = paramTable(paramNames(i)).length()
				paramEnds(i) = paramEnds(i - 1) + n
				If c.subset Then
					Dim ss As Long = n / c.maxPerParam
					If ss = 0 Then
						ss = n
					End If

					If ss > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					stepSizeForParam(paramNames(i)) = CInt(ss)
				End If
			Next i

			If c.print = PrintMode.ALL Then
				Dim i As Integer=0
				For Each l As Layer In c.net.Layers
					Dim s As ISet(Of String) = l.paramTable().Keys
					log.info("Layer " & i & ": " & l.GetType().Name & " - params " & s)
					i += 1
				Next l
			End If


			Dim totalNFailures As Integer = 0
			Dim maxError As Double = 0.0
			Dim ds As New DataSet(c.input, c.labels, c.inputMask, c.labelMask)
			Dim currParamNameIdx As Integer = 0

			If c.excludeParams IsNot Nothing AndAlso c.excludeParams.Count > 0 Then
				log.info("NOTE: parameters will be skipped due to config: {}", c.excludeParams)
			End If

			Dim params As INDArray = c.net.params() 'Assumption here: params is a view that we can modify in-place
			Dim i As Long = 0
			Do While i < nParams
				'Get param name
				If i >= paramEnds(currParamNameIdx) Then
					currParamNameIdx += 1
				End If
				Dim paramName As String = paramNames(currParamNameIdx)
				If c.excludeParams IsNot Nothing AndAlso c.excludeParams.Contains(paramName) Then
	'                log.info("Skipping parameters for parameter name: {}", paramName);
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: i = paramEnds[currParamNameIdx++];
					i = paramEnds(currParamNameIdx)
						currParamNameIdx += 1
					Continue Do
				End If

				'(w+epsilon): Do forward pass and score
				Dim origValue As Double = params.getDouble(i)
				params.putScalar(i, origValue + c.epsilon)
				If c.callEachIter IsNot Nothing Then
					c.callEachIter.accept(c.net)
				End If
				Dim scorePlus As Double = c.net.score(ds, True)

				'(w-epsilon): Do forward pass and score
				params.putScalar(i, origValue - c.epsilon)
				If c.callEachIter IsNot Nothing Then
					c.callEachIter.accept(c.net)
				End If
				Dim scoreMinus As Double = c.net.score(ds, True)

				'Reset original param value
				params.putScalar(i, origValue)

				'Calculate numerical parameter gradient:
				Dim scoreDelta As Double = scorePlus - scoreMinus

				Dim numericalGradient As Double = scoreDelta / (2 * c.epsilon)
				If Double.IsNaN(numericalGradient) Then
					Throw New System.InvalidOperationException("Numerical gradient was NaN for parameter " & i & " of " & nParams)
				End If

				Dim backpropGradient As Double = gradientToCheck.getDouble(i)
				'http://cs231n.github.io/neural-networks-3/#gradcheck
				'use mean centered
				Dim relError As Double = Math.Abs(backpropGradient - numericalGradient) / (Math.Abs(numericalGradient) + Math.Abs(backpropGradient))
				If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
					relError = 0.0 'Edge case: i.e., RNNs with time series length of 1.0
				End If

				If relError > maxError Then
					maxError = relError
				End If
				If relError > c.maxRelError OrElse Double.IsNaN(relError) Then
					Dim absError As Double = Math.Abs(backpropGradient - numericalGradient)
					If absError < c.minAbsoluteError Then
						If c.print = PrintMode.ALL OrElse c.print = PrintMode.ZEROS AndAlso absError = 0.0 Then
							log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & c.minAbsoluteError)
						End If
					Else
						log.info("Param " & i & " (" & paramName & ") FAILED: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus & ", paramValue = " & origValue)
						If c.exitOnFirstError Then
							Return False
						End If
						totalNFailures += 1
					End If
				ElseIf c.print = PrintMode.ALL Then
					log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError)
				End If

				Dim [step] As Long
				If c.subset Then
					[step] = stepSizeForParam(paramName)
					If i + [step] > paramEnds(currParamNameIdx)+1 Then
						[step] = paramEnds(currParamNameIdx)+1 - i
					End If
				Else
					[step] = 1
				End If

				i += [step]
			Loop

			Dim nPass As val = nParams - totalNFailures
			log.info("GradientCheckUtil.checkGradients(): " & nParams & " params checked, " & nPass & " passed, " & totalNFailures & " failed. Largest relative error = " & maxError)

			Return totalNFailures = 0
		End Function

		Public Shared Function checkGradients(ByVal c As GraphConfig) As Boolean
			'Basic sanity checks on input:
			If c.epsilon <= 0.0 OrElse c.epsilon > 0.1 Then
				Throw New System.ArgumentException("Invalid epsilon: expect epsilon in range (0,0.1], usually 1e-4 or so")
			End If
			If c.maxRelError <= 0.0 OrElse c.maxRelError > 0.25 Then
				Throw New System.ArgumentException("Invalid maxRelativeError: " & c.maxRelError)
			End If

			If c.net.NumInputArrays <> c.inputs.Length Then
				Throw New System.ArgumentException("Invalid input arrays: expect " & c.net.NumInputArrays & " inputs")
			End If
			If c.net.NumOutputArrays <> c.labels.Length Then
				Throw New System.ArgumentException("Invalid labels arrays: expect " & c.net.NumOutputArrays & " outputs")
			End If

			Dim dataType As DataType = DataTypeUtil.DtypeFromContext
			If dataType <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Cannot perform gradient check: Datatype is not set to double precision (" & "is: " & dataType & "). Double precision must be used for gradient checks. Set " & "DataTypeUtil.setDTypeForContext(DataType.DOUBLE); before using GradientCheckUtil")
			End If

			Dim netDataType As DataType = c.net.Configuration.getDataType()
			If netDataType <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Cannot perform gradient check: Network datatype is not set to double precision (" & "is: " & netDataType & "). Double precision must be used for gradient checks. Create network with .dataType(DataType.DOUBLE) before using GradientCheckUtil")
			End If

			If netDataType <> c.net.params().dataType() Then
				Throw New System.InvalidOperationException("Parameters datatype does not match network configuration datatype (" & "is: " & c.net.params().dataType() & "). If network datatype is set to DOUBLE, parameters must also be DOUBLE.")
			End If

			'Check configuration
			Dim layerCount As Integer = 0
			For Each vertexName As String In c.net.Configuration.getVertices().keySet()
				Dim gv As GraphVertex = c.net.Configuration.getVertices().get(vertexName)
				If Not (TypeOf gv Is LayerVertex) Then
					Continue For
				End If
				Dim lv As LayerVertex = DirectCast(gv, LayerVertex)

				If TypeOf lv.getLayerConf().getLayer() Is BaseLayer Then
					Dim bl As BaseLayer = CType(lv.getLayerConf().getLayer(), BaseLayer)
					Dim u As IUpdater = bl.getIUpdater()
					If TypeOf u Is Sgd Then
						'Must have LR of 1.0
						Dim lr As Double = DirectCast(u, Sgd).getLearningRate()
						If lr <> 1.0 Then
							Throw New System.InvalidOperationException("When using SGD updater, must also use lr=1.0 for layer " & layerCount & "; got " & u & " with lr=" & lr & " for layer """ & lv.getLayerConf().getLayer().getLayerName() & """")
						End If
					ElseIf Not (TypeOf u Is NoOp) Then
						Throw New System.InvalidOperationException("Must have Updater.NONE (or SGD + lr=1.0) for layer " & layerCount & "; got " & u)
					End If

					Dim activation As IActivation = bl.getActivationFn()
					If activation IsNot Nothing Then
						If Not VALID_ACTIVATION_FUNCTIONS.Contains(activation.GetType()) Then
							log.warn("Layer """ & vertexName & """ is possibly using an unsuitable activation function: " & activation.GetType() & ". Activation functions for gradient checks must be smooth (like sigmoid, tanh, softmax) and not " & "contain discontinuities like ReLU or LeakyReLU (these may cause spurious failures)")
						End If
					End If
				End If

				If lv.getLayerConf().getLayer().getIDropout() IsNot Nothing AndAlso c.callEachIter Is Nothing Then
					Throw New System.InvalidOperationException("When gradient checking dropout, rng seed must be reset each iteration, or no" & " dropout should be present during gradient checks - got dropout = " & lv.getLayerConf().getLayer().getIDropout() & " for layer " & layerCount)
				End If
			Next vertexName

			'Set softmax clipping to 0 if necessary, to avoid spurious failures due to clipping
			For Each l As Layer In c.net.Layers
				If TypeOf l Is IOutputLayer Then
					configureLossFnClippingIfPresent(DirectCast(l, IOutputLayer))
				End If
			Next l

			For i As Integer = 0 To c.inputs.Length - 1
				c.net.setInput(i, c.inputs(i))
			Next i
			For i As Integer = 0 To c.labels.Length - 1
				c.net.setLabel(i, c.labels(i))
			Next i

			c.net.setLayerMaskArrays(c.inputMask, c.labelMask)

			If c.callEachIter IsNot Nothing Then
				c.callEachIter.accept(c.net)
			End If

			c.net.computeGradientAndScore()
			Dim gradAndScore As Pair(Of Gradient, Double) = c.net.gradientAndScore()

			Dim updater As New ComputationGraphUpdater(c.net)
			updater.update(gradAndScore.First, 0, 0, c.net.batchSize(), LayerWorkspaceMgr.noWorkspaces())

			Dim gradientToCheck As INDArray = gradAndScore.First.gradient().dup() 'need dup: gradients are a *view* of the full gradient array (which will change every time backprop is done)
			Dim originalParams As INDArray = c.net.params().dup() 'need dup: params are a *view* of full parameters

			Dim nParams As val = originalParams.length()

			Dim paramTable As IDictionary(Of String, INDArray) = c.net.paramTable()
			Dim paramNames As IList(Of String) = New List(Of String)(paramTable.Keys)
			Dim paramEnds As val = New Long(paramNames.Count - 1){}
			paramEnds(0) = paramTable(paramNames(0)).length()
			For i As Integer = 1 To paramEnds.length - 1
				paramEnds(i) = paramEnds(i - 1) + paramTable(paramNames(i)).length()
			Next i

			If c.excludeParams IsNot Nothing AndAlso c.excludeParams.Count > 0 Then
				log.info("NOTE: parameters will be skipped due to config: {}", c.excludeParams)
			End If

			Dim currParamNameIdx As Integer = 0
			Dim totalNFailures As Integer = 0
			Dim maxError As Double = 0.0
			Dim mds As New MultiDataSet(c.inputs, c.labels, c.inputMask, c.labelMask)
			Dim params As INDArray = c.net.params() 'Assumption here: params is a view that we can modify in-place
			For i As Long = 0 To nParams - 1
				'Get param name
				If i >= paramEnds(currParamNameIdx) Then
					currParamNameIdx += 1
				End If
				Dim paramName As String = paramNames(currParamNameIdx)
				If c.excludeParams IsNot Nothing AndAlso c.excludeParams.Contains(paramName) Then
					'log.info("Skipping parameters for parameter name: {}", paramName);
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: i = paramEnds[currParamNameIdx++];
					i = paramEnds(currParamNameIdx)
						currParamNameIdx += 1
					Continue For
				End If

				'(w+epsilon): Do forward pass and score
				Dim origValue As Double = params.getDouble(i)

				params.putScalar(i, origValue + c.epsilon)
				If c.callEachIter IsNot Nothing Then
					c.callEachIter.accept(c.net)
				End If
				Dim scorePlus As Double = c.net.score(mds, True) 'training == true for batch norm, etc (scores and gradients need to be calculated on same thing)

				'(w-epsilon): Do forward pass and score
				params.putScalar(i, origValue - c.epsilon)
				If c.callEachIter IsNot Nothing Then
					c.callEachIter.accept(c.net)
				End If
				Dim scoreMinus As Double = c.net.score(mds, True)

				'Reset original param value
				params.putScalar(i, origValue)

				'Calculate numerical parameter gradient:
				Dim scoreDelta As Double = scorePlus - scoreMinus

				Dim numericalGradient As Double = scoreDelta / (2 * c.epsilon)
				If Double.IsNaN(numericalGradient) Then
					Throw New System.InvalidOperationException("Numerical gradient was NaN for parameter " & i & " of " & nParams)
				End If

				Dim backpropGradient As Double = gradientToCheck.getDouble(i)
				'http://cs231n.github.io/neural-networks-3/#gradcheck
				'use mean centered
				Dim relError As Double = Math.Abs(backpropGradient - numericalGradient) / (Math.Abs(numericalGradient) + Math.Abs(backpropGradient))
				If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
					relError = 0.0 'Edge case: i.e., RNNs with time series length of 1.0
				End If

				If relError > maxError Then
					maxError = relError
				End If
				If relError > c.maxRelError OrElse Double.IsNaN(relError) Then
					Dim absError As Double = Math.Abs(backpropGradient - numericalGradient)
					If absError < c.minAbsoluteError Then
						If c.print = PrintMode.ALL OrElse c.print = PrintMode.ZEROS AndAlso absError = 0.0 Then
							log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & c.minAbsoluteError)
						End If
					Else
						log.info("Param " & i & " (" & paramName & ") FAILED: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus & ", paramValue = " & origValue)
						If c.exitOnFirstError Then
							Return False
						End If
						totalNFailures += 1
					End If
				ElseIf c.print = PrintMode.ALL Then
					log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError)
				End If
			Next i

			Dim nPass As val = nParams - totalNFailures
			log.info("GradientCheckUtil.checkGradients(): " & nParams & " params checked, " & nPass & " passed, " & totalNFailures & " failed. Largest relative error = " & maxError)

			Return totalNFailures = 0
		End Function



		''' <summary>
		''' Check backprop gradients for a pretrain layer
		''' 
		''' NOTE: gradient checking pretrain layers can be difficult...
		''' </summary>
		Public Shared Function checkGradientsPretrainLayer(ByVal layer As Layer, ByVal epsilon As Double, ByVal maxRelError As Double, ByVal minAbsoluteError As Double, ByVal print As Boolean, ByVal exitOnFirstError As Boolean, ByVal input As INDArray, ByVal rngSeed As Integer) As Boolean

			Dim mgr As LayerWorkspaceMgr = LayerWorkspaceMgr.noWorkspaces()

			'Basic sanity checks on input:
			If epsilon <= 0.0 OrElse epsilon > 0.1 Then
				Throw New System.ArgumentException("Invalid epsilon: expect epsilon in range (0,0.1], usually 1e-4 or so")
			End If
			If maxRelError <= 0.0 OrElse maxRelError > 0.25 Then
				Throw New System.ArgumentException("Invalid maxRelativeError: " & maxRelError)
			End If

			Dim dataType As DataType = DataTypeUtil.DtypeFromContext
			If dataType <> DataType.DOUBLE Then
				Throw New System.InvalidOperationException("Cannot perform gradient check: Datatype is not set to double precision (" & "is: " & dataType & "). Double precision must be used for gradient checks. Set " & "DataTypeUtil.setDTypeForContext(DataType.DOUBLE); before using GradientCheckUtil")
			End If

			'Check network configuration:
			layer.setInput(input, LayerWorkspaceMgr.noWorkspaces())
			Nd4j.Random.setSeed(rngSeed)
			layer.computeGradientAndScore(mgr)
			Dim gradAndScore As Pair(Of Gradient, Double) = layer.gradientAndScore()

			Dim updater As Updater = UpdaterCreator.getUpdater(layer)
			updater.update(layer, gradAndScore.First, 0, 0, layer.batchSize(), LayerWorkspaceMgr.noWorkspaces())

			Dim gradientToCheck As INDArray = gradAndScore.First.gradient().dup() 'need dup: gradients are a *view* of the full gradient array (which will change every time backprop is done)
			Dim originalParams As INDArray = layer.params().dup() 'need dup: params are a *view* of full parameters

			Dim nParams As val = originalParams.length()

			Dim paramTable As IDictionary(Of String, INDArray) = layer.paramTable()
			Dim paramNames As IList(Of String) = New List(Of String)(paramTable.Keys)
			Dim paramEnds As val = New Long(paramNames.Count - 1){}
			paramEnds(0) = paramTable(paramNames(0)).length()
			For i As Integer = 1 To paramEnds.length - 1
				paramEnds(i) = paramEnds(i - 1) + paramTable(paramNames(i)).length()
			Next i


			Dim totalNFailures As Integer = 0
			Dim maxError As Double = 0.0
			Dim currParamNameIdx As Integer = 0

			Dim params As INDArray = layer.params() 'Assumption here: params is a view that we can modify in-place
			For i As Integer = 0 To nParams - 1
				'Get param name
				If i >= paramEnds(currParamNameIdx) Then
					currParamNameIdx += 1
				End If
				Dim paramName As String = paramNames(currParamNameIdx)

				'(w+epsilon): Do forward pass and score
				Dim origValue As Double = params.getDouble(i)
				params.putScalar(i, origValue + epsilon)

				'TODO add a 'score' method that doesn't calculate gradients...
				Nd4j.Random.setSeed(rngSeed)
				layer.computeGradientAndScore(mgr)
				Dim scorePlus As Double = layer.score()

				'(w-epsilon): Do forward pass and score
				params.putScalar(i, origValue - epsilon)
				Nd4j.Random.setSeed(rngSeed)
				layer.computeGradientAndScore(mgr)
				Dim scoreMinus As Double = layer.score()

				'Reset original param value
				params.putScalar(i, origValue)

				'Calculate numerical parameter gradient:
				Dim scoreDelta As Double = scorePlus - scoreMinus

				Dim numericalGradient As Double = scoreDelta / (2 * epsilon)
				If Double.IsNaN(numericalGradient) Then
					Throw New System.InvalidOperationException("Numerical gradient was NaN for parameter " & i & " of " & nParams)
				End If

				Dim backpropGradient As Double = gradientToCheck.getDouble(i)
				'http://cs231n.github.io/neural-networks-3/#gradcheck
				'use mean centered
				Dim relError As Double = Math.Abs(backpropGradient - numericalGradient) / (Math.Abs(numericalGradient) + Math.Abs(backpropGradient))
				If backpropGradient = 0.0 AndAlso numericalGradient = 0.0 Then
					relError = 0.0 'Edge case: i.e., RNNs with time series length of 1.0
				End If

				If relError > maxError Then
					maxError = relError
				End If
				If relError > maxRelError OrElse Double.IsNaN(relError) Then
					Dim absError As Double = Math.Abs(backpropGradient - numericalGradient)
					If absError < minAbsoluteError Then
						log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & minAbsoluteError)
					Else
						If print Then
							log.info("Param " & i & " (" & paramName & ") FAILED: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus & ", paramValue = " & origValue)
						End If
						If exitOnFirstError Then
							Return False
						End If
						totalNFailures += 1
					End If
				ElseIf print Then
					log.info("Param " & i & " (" & paramName & ") passed: grad= " & backpropGradient & ", numericalGrad= " & numericalGradient & ", relError= " & relError)
				End If
			Next i

			If print Then
				Dim nPass As val = nParams - totalNFailures
				log.info("GradientCheckUtil.checkGradients(): " & nParams & " params checked, " & nPass & " passed, " & totalNFailures & " failed. Largest relative error = " & maxError)
			End If

			Return totalNFailures = 0
		End Function
	End Class

End Namespace