Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports CompositeReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.CompositeReconstructionDistribution
Imports LossFunctionWrapper = org.deeplearning4j.nn.conf.layers.variational.LossFunctionWrapper
Imports ReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.ReconstructionDistribution
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports VariationalAutoencoderParamInitializer = org.deeplearning4j.nn.params.VariationalAutoencoderParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Solver = org.deeplearning4j.optimize.Solver
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
import static org.deeplearning4j.nn.params.VariationalAutoencoderParamInitializer.BIAS_KEY_SUFFIX
import static org.deeplearning4j.nn.params.VariationalAutoencoderParamInitializer.WEIGHT_KEY_SUFFIX

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

Namespace org.deeplearning4j.nn.layers.variational


	<Serializable>
	Public Class VariationalAutoencoder
		Implements Layer

'JAVA TO VB CONVERTER NOTE: The field input was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend input_Conflict As INDArray
		Protected Friend paramsFlattened As INDArray
		Protected Friend gradientsFlattened As INDArray
'JAVA TO VB CONVERTER NOTE: The field params was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend params_Conflict As IDictionary(Of String, INDArray)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient Map<String, org.nd4j.linalg.api.ndarray.INDArray> gradientViews;
		<NonSerialized>
		Protected Friend gradientViews As IDictionary(Of String, INDArray)
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend score_Conflict As Double = 0.0
'JAVA TO VB CONVERTER NOTE: The field optimizer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend optimizer_Conflict As ConvexOptimizer
'JAVA TO VB CONVERTER NOTE: The field gradient was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradient_Conflict As Gradient
		Protected Friend trainingListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()
'JAVA TO VB CONVERTER NOTE: The field index was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend index_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field maskArray was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend maskArray_Conflict As INDArray
		Protected Friend solver As Solver

		Protected Friend encoderLayerSizes() As Integer
		Protected Friend decoderLayerSizes() As Integer
		Protected Friend reconstructionDistribution As ReconstructionDistribution
		Protected Friend pzxActivationFn As IActivation
		Protected Friend numSamples As Integer
'JAVA TO VB CONVERTER NOTE: The field cacheMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend cacheMode_Conflict As CacheMode = CacheMode.NONE
		Protected Friend dataType As DataType

		Protected Friend zeroedPretrainParamGradients As Boolean = False

		Protected Friend weightNoiseParams As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int iterationCount;
		Protected Friend iterationCount As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int epochCount;
		Protected Friend epochCount As Integer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			Me.conf_Conflict = conf
			Me.dataType = dataType

			Me.encoderLayerSizes = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder).getEncoderLayerSizes()
			Me.decoderLayerSizes = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder).getDecoderLayerSizes()
			Me.reconstructionDistribution = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder).getOutputDistribution()
			Me.pzxActivationFn = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder).getPzxActivationFn()
			Me.numSamples = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder).getNumSamples()
		End Sub

		Protected Friend Overridable Function layerConf() As org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
			Return CType(conf().getLayer(), org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder)
		End Function

		Public Overridable WriteOnly Property CacheMode Implements Layer.setCacheMode As CacheMode
			Set(ByVal mode As CacheMode)
				If mode = Nothing Then
					mode = CacheMode.NONE
				End If
    
				Me.cacheMode_Conflict = mode
			End Set
		End Property

		Protected Friend Overridable Function layerId() As String
			Dim name As String = Me.conf().getLayer().getLayerName()
			Return "(layer name: " & (If(name Is Nothing, """""", name)) & ", layer index: " & index_Conflict & ")"
		End Function

		''' <summary>
		''' Init the model
		''' </summary>
		Public Overridable Sub init()

		End Sub

		Public Overridable Sub update(ByVal gradient As Gradient)
			Throw New System.NotSupportedException("Not supported " & layerId())
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String)
			Throw New System.NotSupportedException("Not supported " & layerId())
		End Sub

		Public Overridable Function score() As Double
			Return score_Conflict
		End Function

		Protected Friend Overridable Function getParamWithNoise(ByVal param As String, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim p As INDArray
			If layerConf().getWeightNoise() IsNot Nothing Then
				If training AndAlso weightNoiseParams.Count > 0 AndAlso weightNoiseParams.ContainsKey(param) Then
					'Re-use these weights for both forward pass and backprop - don't want to use 2 different params here
					'These should be cleared during  backprop
					Return weightNoiseParams(param)
				Else
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						p = layerConf().getWeightNoise().getParameter(Me, param, IterationCount, EpochCount, training, workspaceMgr)
					End Using
				End If

				If training Then
					'Store for re-use in backprop
					weightNoiseParams(param) = p
				End If
			Else
				Return getParam(param)
			End If

			Return p
		End Function

		Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			'Forward pass through the encoder and mean for P(Z|X)
			Dim fwd As VAEFwdHelper = doForward(True, True, workspaceMgr)
			Dim afn As IActivation = layerConf().getActivationFn()

			'Forward pass through logStd^2 for P(Z|X)
			Dim pzxLogStd2W As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W, True, workspaceMgr)
			Dim pzxLogStd2b As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B, True, workspaceMgr)

			Dim pzxLogStd2Pre As INDArray = fwd.encoderActivations(fwd.encoderActivations.Length - 1).mmul(pzxLogStd2W).addiRowVector(pzxLogStd2b)

			Dim meanZ As INDArray = fwd.pzxMeanPreOut.dup()
			Dim logStdev2Z As INDArray = pzxLogStd2Pre.dup()
			pzxActivationFn.getActivation(meanZ, True)
			pzxActivationFn.getActivation(logStdev2Z, True)


			Dim pzxSigmaSquared As INDArray = Transforms.exp(logStdev2Z, True)
			Dim pzxSigma As INDArray = Transforms.sqrt(pzxSigmaSquared, True)

			Dim minibatch As val = input_Conflict.size(0)
			Dim size As val = fwd.pzxMeanPreOut.size(1)


			Dim gradientMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim scaleFactor As Double = 1.0 / numSamples
			Dim blasL1 As Level1 = Nd4j.BlasWrapper.level1()
			Dim encoderActivationDerivs() As INDArray = (If(numSamples > 1, New INDArray(encoderLayerSizes.Length - 1){}, Nothing))
			For l As Integer = 0 To numSamples - 1 'Default (and in most cases) numSamples == 1
				Dim gemmCConstant As Double = (If(l = 0, 0.0, 1.0)) '0 for first one (to get rid of previous buffer data), otherwise 1 (for adding)

				Dim e As INDArray = Nd4j.randn(dataType, minibatch, size)
				Dim z As INDArray = pzxSigma.mul(e).addi(meanZ) 'z = mu + sigma * e, with e ~ N(0,1)


				'Need to do forward pass through decoder layers
				Dim nDecoderLayers As Integer = decoderLayerSizes.Length
				Dim current As INDArray = z
				Dim decoderPreOut(nDecoderLayers - 1) As INDArray 'Need pre-out for backprop later
				Dim decoderActivations(nDecoderLayers - 1) As INDArray
				For i As Integer = 0 To nDecoderLayers - 1
					Dim wKey As String = "d" & i + WEIGHT_KEY_SUFFIX
					Dim bKey As String = "d" & i + BIAS_KEY_SUFFIX

					Dim weights As INDArray = getParamWithNoise(wKey, True, workspaceMgr)
					Dim bias As INDArray = getParamWithNoise(bKey, True, workspaceMgr)

					current = current.mmul(weights).addiRowVector(bias)
					decoderPreOut(i) = current.dup()
					afn.getActivation(current, True)
					decoderActivations(i) = current
				Next i

				Dim pxzw As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_W, True, workspaceMgr)
				Dim pxzb As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_B, True, workspaceMgr)

				If l = 0 Then
					'Need to add other component of score, in addition to negative log probability
					'Note the negative here vs. the equation in Kingma & Welling: this is because we are minimizing the negative of
					' variational lower bound, rather than maximizing the variational lower bound
					'Unlike log probability (which is averaged over samples) this should be calculated just once
					Dim temp As INDArray = meanZ.mul(meanZ).addi(pzxSigmaSquared).negi()
					temp.addi(logStdev2Z).addi(1.0)
					Dim scorePt1 As Double = -0.5 / minibatch * temp.sumNumber().doubleValue()
					Me.score_Conflict = scorePt1 + calcRegularizationScore(False)
				End If

				Dim pxzDistributionPreOut As INDArray = current.mmul(pxzw).addiRowVector(pxzb)
				Dim logPTheta As Double = reconstructionDistribution.negLogProbability(input_Conflict, pxzDistributionPreOut, True)
				Me.score_Conflict += logPTheta / numSamples

				'If we have any training listeners (for example, for UI StatsListener - pass on activations)
				If trainingListeners IsNot Nothing AndAlso trainingListeners.Count > 0 AndAlso l = 0 Then 'Note: only doing this on the *first* sample
					Dim activations As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
					For i As Integer = 0 To fwd.encoderActivations.Length - 1
						activations("e" & i) = fwd.encoderActivations(i)
					Next i
					activations(VariationalAutoencoderParamInitializer.PZX_PREFIX) = z
					For i As Integer = 0 To decoderActivations.Length - 1
						activations("d" & i) = decoderActivations(i)
					Next i
					activations(VariationalAutoencoderParamInitializer.PXZ_PREFIX) = reconstructionDistribution.generateAtMean(pxzDistributionPreOut)
					If trainingListeners.Count > 0 Then
						Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							For Each tl As TrainingListener In trainingListeners
								tl.onForwardPass(Me, activations)
							Next tl
						End Using
					End If
				End If

				'///////////////////////////////////////////////////////
				'Backprop

				'First: calculate the gradients at the input to the reconstruction distribution
				Dim dpdpxz As INDArray = reconstructionDistribution.gradient(input_Conflict, pxzDistributionPreOut)

				'Do backprop for output reconstruction distribution -> final decoder layer
				Dim dLdxzw As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PXZ_W)
				Dim dLdxzb As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PXZ_B)
				Dim lastDecActivations As INDArray = decoderActivations(decoderActivations.Length - 1)
				Nd4j.gemm(lastDecActivations, dpdpxz, dLdxzw, True, False, scaleFactor, gemmCConstant)
				If l = 0 Then
					dpdpxz.sum(dLdxzb, 0) 'dLdxzb array is initialized/zeroed first in sum op
					If numSamples > 1 Then
						dLdxzb.muli(scaleFactor)
					End If
				Else
					blasL1.axpy(dLdxzb.length(), scaleFactor, dpdpxz.sum(0), dLdxzb)
				End If

				gradientMap(VariationalAutoencoderParamInitializer.PXZ_W) = dLdxzw
				gradientMap(VariationalAutoencoderParamInitializer.PXZ_B) = dLdxzb

				Dim epsilon As INDArray = pxzw.mmul(dpdpxz.transpose()).transpose()

				'Next: chain derivatives backwards through the decoder layers
				For i As Integer = nDecoderLayers - 1 To 0 Step -1
					Dim wKey As String = "d" & i + WEIGHT_KEY_SUFFIX
					Dim bKey As String = "d" & i + BIAS_KEY_SUFFIX

					Dim currentDelta As INDArray = afn.backprop(decoderPreOut(i), epsilon).First 'TODO activation functions with params

					Dim weights As INDArray = getParamWithNoise(wKey, True, workspaceMgr)
					Dim dLdW As INDArray = gradientViews(wKey)
					Dim dLdB As INDArray = gradientViews(bKey)

					Dim actInput As INDArray
					If i = 0 Then
						actInput = z
					Else
						actInput = decoderActivations(i - 1)
					End If

					Nd4j.gemm(actInput, currentDelta, dLdW, True, False, scaleFactor, gemmCConstant)

					If l = 0 Then
						currentDelta.sum(dLdB, 0)
						If numSamples > 1 Then
							dLdB.muli(scaleFactor)
						End If
					Else
						blasL1.axpy(dLdB.length(), scaleFactor, currentDelta.sum(0), dLdB)
					End If

					gradientMap(wKey) = dLdW
					gradientMap(bKey) = dLdB

					epsilon = weights.mmul(currentDelta.transpose()).transpose()
				Next i

				'Do backprop through p(z|x)
				Dim eZXMeanW As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_W, True, workspaceMgr)
				Dim eZXLogStdev2W As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W, True, workspaceMgr)

				Dim dLdz As INDArray = epsilon
				'If we were maximizing the equation in Kinga and Welling, this would be a .sub(meanZ). Here: we are minimizing the negative instead
				Dim dLdmu As INDArray = dLdz.add(meanZ)

				Dim dLdLogSigma2 As INDArray = dLdz.mul(e).muli(pzxSigma).addi(pzxSigmaSquared).subi(1).muli(0.5)


				Dim dLdPreMu As INDArray = pzxActivationFn.backprop(fwd.getPzxMeanPreOut().dup(), dLdmu).First

				Dim dLdPreLogSigma2 As INDArray = pzxActivationFn.backprop(pzxLogStd2Pre.dup(), dLdLogSigma2).First

				'Weight gradients for weights feeding into p(z|x)
				Dim lastEncoderActivation As INDArray = fwd.encoderActivations(fwd.encoderActivations.Length - 1)
				Dim dLdZXMeanW As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_MEAN_W)
				Dim dLdZXLogStdev2W As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W)
				Nd4j.gemm(lastEncoderActivation, dLdPreMu, dLdZXMeanW, True, False, scaleFactor, gemmCConstant)
				Nd4j.gemm(lastEncoderActivation, dLdPreLogSigma2, dLdZXLogStdev2W, True, False, scaleFactor, gemmCConstant)

				'Bias gradients for p(z|x)
				Dim dLdZXMeanb As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_MEAN_B)
				Dim dLdZXLogStdev2b As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B)
				'If we were maximizing the equation in Kinga and Welling, this would be a .sub(meanZ). Here: we are minimizing the negative instead
				If l = 0 Then
					dLdZXMeanb.assign(pzxActivationFn.backprop(fwd.getPzxMeanPreOut().dup(), dLdz.add(meanZ)).First.sum(0))

					dLdPreLogSigma2.sum(dLdZXLogStdev2b, 0)
					If numSamples > 1 Then
						dLdZXMeanb.muli(scaleFactor)
						dLdZXLogStdev2b.muli(scaleFactor)
					End If
				Else
					blasL1.axpy(dLdZXMeanb.length(), scaleFactor, pzxActivationFn.backprop(fwd.getPzxMeanPreOut().dup(), dLdz.add(meanZ)).First.sum(0), dLdZXMeanb)

					blasL1.axpy(dLdZXLogStdev2b.length(), scaleFactor, dLdPreLogSigma2.sum(0), dLdZXLogStdev2b)
				End If

				gradientMap(VariationalAutoencoderParamInitializer.PZX_MEAN_W) = dLdZXMeanW
				gradientMap(VariationalAutoencoderParamInitializer.PZX_MEAN_B) = dLdZXMeanb
				gradientMap(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W) = dLdZXLogStdev2W
				gradientMap(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B) = dLdZXLogStdev2b

				'Epsilon (dL/dActivation) at output of the last encoder layer:
				epsilon = Nd4j.gemm(dLdPreMu, eZXMeanW, False, True) 'Equivalent to: epsilon = eZXMeanW.mmul(dLdPreMu.transpose()).transpose(); using   (AxB^T)^T = BxA^T
				'Next line: equivalent to epsilon.addi(eZXLogStdev2W.mmul(dLdPreLogSigma2.transpose()).transpose());       using: (AxB^T)^T = BxA^T
				Nd4j.gemm(dLdPreLogSigma2, eZXLogStdev2W, epsilon, False, True, 1.0, 1.0)

				'Backprop through encoder:
				Dim nEncoderLayers As Integer = encoderLayerSizes.Length
				For i As Integer = nEncoderLayers - 1 To 0 Step -1
					Dim wKey As String = "e" & i + WEIGHT_KEY_SUFFIX
					Dim bKey As String = "e" & i + BIAS_KEY_SUFFIX

					Dim weights As INDArray = getParamWithNoise(wKey, True, workspaceMgr)

					Dim dLdW As INDArray = gradientViews(wKey)
					Dim dLdB As INDArray = gradientViews(bKey)

					Dim preOut As INDArray = fwd.encoderPreOuts(i)

					Dim currentDelta As INDArray
					If numSamples > 1 Then
						'Re-use sigma-prime values for the encoder - these don't change based on multiple samples,
						' only the errors do
						If l = 0 Then
							'Not the most elegent implementation (with the ND4j.ones()), but it works...
							encoderActivationDerivs(i) = afn.backprop(fwd.encoderPreOuts(i), Nd4j.ones(fwd.encoderPreOuts(i).shape())).First
						End If
						currentDelta = epsilon.muli(encoderActivationDerivs(i))
					Else
						currentDelta = afn.backprop(preOut, epsilon).First
					End If

					Dim actInput As INDArray
					If i = 0 Then
						actInput = input_Conflict.castTo(dLdW.dataType())
					Else
						actInput = fwd.encoderActivations(i - 1)
					End If
					Nd4j.gemm(actInput, currentDelta, dLdW, True, False, scaleFactor, gemmCConstant)
					If l = 0 Then
						currentDelta.sum(dLdB, 0)
						If numSamples > 1 Then
							dLdB.muli(scaleFactor)
						End If
					Else
						blasL1.axpy(dLdB.length(), scaleFactor, currentDelta.sum(0), dLdB)
					End If

					gradientMap(wKey) = dLdW
					gradientMap(bKey) = dLdB

					epsilon = weights.mmul(currentDelta.transpose()).transpose()
				Next i
			Next l

			'Insert the gradients into the Gradient map in the correct order, in case we need to flatten the gradient later
			' to match the parameters iteration order
			Dim gradient As Gradient = New DefaultGradient(gradientsFlattened)
			Dim g As IDictionary(Of String, INDArray) = gradient.gradientForVariable()
			For i As Integer = 0 To encoderLayerSizes.Length - 1
				Dim w As String = "e" & i & VariationalAutoencoderParamInitializer.WEIGHT_KEY_SUFFIX
				g(w) = gradientMap(w)
				Dim b As String = "e" & i & VariationalAutoencoderParamInitializer.BIAS_KEY_SUFFIX
				g(b) = gradientMap(b)
			Next i
			g(VariationalAutoencoderParamInitializer.PZX_MEAN_W) = gradientMap(VariationalAutoencoderParamInitializer.PZX_MEAN_W)
			g(VariationalAutoencoderParamInitializer.PZX_MEAN_B) = gradientMap(VariationalAutoencoderParamInitializer.PZX_MEAN_B)
			g(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W) = gradientMap(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W)
			g(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B) = gradientMap(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B)
			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim w As String = "d" & i & VariationalAutoencoderParamInitializer.WEIGHT_KEY_SUFFIX
				g(w) = gradientMap(w)
				Dim b As String = "d" & i & VariationalAutoencoderParamInitializer.BIAS_KEY_SUFFIX
				g(b) = gradientMap(b)
			Next i
			g(VariationalAutoencoderParamInitializer.PXZ_W) = gradientMap(VariationalAutoencoderParamInitializer.PXZ_W)
			g(VariationalAutoencoderParamInitializer.PXZ_B) = gradientMap(VariationalAutoencoderParamInitializer.PXZ_B)

			weightNoiseParams.Clear()

			Me.gradient_Conflict = gradient
		End Sub

		Public Overridable Function params() As INDArray
			Return paramsFlattened
		End Function

		Public Overridable ReadOnly Property Config As TrainingConfig
			Get
				Return conf_Conflict.getLayer()
			End Get
		End Property

		Public Overridable Function numParams() As Long
			Return numParams(False)
		End Function

		Public Overridable Function numParams(ByVal backwards As Boolean) As Long
			Dim ret As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In params_Conflict.SetOfKeyValuePairs()
				If backwards AndAlso isPretrainParam(entry.Key) Then
					Continue For
				End If
				ret += entry.Value.length()
			Next entry
			Return ret
		End Function

		Public Overridable WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				If params.length() <> Me.paramsFlattened.length() Then
					Throw New System.ArgumentException("Cannot set parameters: expected parameters vector of length " & Me.paramsFlattened.length() & " but got parameters array of length " & params.length() & " " & layerId())
				End If
				Me.paramsFlattened.assign(params)
			End Set
		End Property

		Public Overridable WriteOnly Property ParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				If Me.params_Conflict IsNot Nothing AndAlso params.length() <> numParams() Then
					Throw New System.ArgumentException("Invalid input: expect params of length " & numParams() & ", got params of length " & params.length() & " " & layerId())
				End If
				Me.paramsFlattened = params
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray
			Get
				Return gradientsFlattened
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				If Me.params_Conflict IsNot Nothing AndAlso gradients.length() <> numParams() Then
					Throw New System.ArgumentException("Invalid input: expect gradients array of length " & numParams() & ", got gradient array of length of length " & gradients.length() & " " & layerId())
				End If
    
				Me.gradientsFlattened = gradients
				Me.gradientViews = conf_Conflict.getLayer().initializer().getGradientsFromFlattened(conf_Conflict, gradients)
			End Set
		End Property

		Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Me.setInput(data, workspaceMgr)
			fit()
		End Sub

		Public Overridable Function gradient() As Gradient
			Return gradient_Conflict
		End Function

		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double)
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overridable Function batchSize() As Integer
			If input_Conflict.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return CInt(input_Conflict.size(0))
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return conf_Conflict
		End Function

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				Me.conf_Conflict = conf
			End Set
		End Property

		Public Overridable Function input() As INDArray
			Return input_Conflict
		End Function

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer
			Get
				Return optimizer_Conflict
			End Get
		End Property

		Public Overridable Function getParam(ByVal param As String) As INDArray
			Return params(param)
		End Function

		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return New LinkedHashMap(Of String, INDArray)(params_Conflict)
		End Function

		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			Dim map As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each e As KeyValuePair(Of String, INDArray) In params_Conflict.SetOfKeyValuePairs()
				If Not backpropParamsOnly OrElse Not isPretrainParam(e.Key) Then
					map(e.Key) = e.Value
				End If
			Next e
			Return map
		End Function

		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			Return True
		End Function

		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				Me.params_Conflict = paramTable
			End Set
		End Property

		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray)
			If paramTable().ContainsKey(key) Then
				paramTable()(key).assign(val)
			Else
				Throw New System.ArgumentException("Unknown parameter: " & key & " - " & layerId())
			End If
		End Sub

		Public Overridable Sub clear()
			Me.input_Conflict = Nothing
			Me.maskArray_Conflict = Nothing
		End Sub

		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer)
			If layerConf().getConstraints() IsNot Nothing Then
				For Each lc As LayerConstraint In layerConf().getConstraints()
					lc.applyConstraint(Me, iteration, epoch)
				Next lc
			End If
		End Sub

		Public Overridable Function isPretrainParam(ByVal param As String) As Boolean
			Return Not (param.StartsWith("e", StringComparison.Ordinal) OrElse param.StartsWith(VariationalAutoencoderParamInitializer.PZX_MEAN_PREFIX, StringComparison.Ordinal))
		End Function

		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double Implements Layer.calcRegularizationScore
			Dim scoreSum As Double = 0.0
			For Each e As KeyValuePair(Of String, INDArray) In paramTable().SetOfKeyValuePairs()
				If backpropParamsOnly AndAlso isPretrainParam(e.Key) Then
					Continue For
				End If
				Dim l As IList(Of Regularization) = layerConf().getRegularizationByParam(e.Key)
				If l Is Nothing OrElse l.Count = 0 Then
					Continue For
				End If
				For Each r As Regularization In l
					scoreSum += r.score(e.Value, IterationCount, EpochCount)
				Next r
			Next e
			Return scoreSum
		End Function

		Public Overridable Function type() As Type Implements Layer.type
			Return Type.FEED_FORWARD
		End Function

		Public Overridable Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements Layer.backpropGradient
			assertInputSet(True)
			If Not zeroedPretrainParamGradients Then
				For Each entry As KeyValuePair(Of String, INDArray) In gradientViews.SetOfKeyValuePairs()
					If isPretrainParam(entry.Key) Then
						entry.Value.assign(0)
					End If
				Next entry
				zeroedPretrainParamGradients = True
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			Dim gradient As Gradient = New DefaultGradient()

			Dim fwd As VAEFwdHelper = doForward(True, True, workspaceMgr)
			Dim currentDelta As INDArray = pzxActivationFn.backprop(fwd.pzxMeanPreOut, epsilon).First

			'Finally, calculate mean value:
			Dim meanW As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_W, True, workspaceMgr)
			Dim dLdMeanW As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_MEAN_W) 'f order
			Dim lastEncoderActivation As INDArray = fwd.encoderActivations(fwd.encoderActivations.Length - 1)
			Nd4j.gemm(lastEncoderActivation, currentDelta, dLdMeanW, True, False, 1.0, 0.0)
			Dim dLdMeanB As INDArray = gradientViews(VariationalAutoencoderParamInitializer.PZX_MEAN_B)
			currentDelta.sum(dLdMeanB, 0) 'dLdMeanB is initialized/zeroed first in sum op

			gradient.gradientForVariable()(VariationalAutoencoderParamInitializer.PZX_MEAN_W) = dLdMeanW
			gradient.gradientForVariable()(VariationalAutoencoderParamInitializer.PZX_MEAN_B) = dLdMeanB

			epsilon = meanW.mmul(currentDelta.transpose()).transpose()

			Dim nEncoderLayers As Integer = encoderLayerSizes.Length

			Dim afn As IActivation = layerConf().getActivationFn()
			For i As Integer = nEncoderLayers - 1 To 0 Step -1
				Dim wKey As String = "e" & i + WEIGHT_KEY_SUFFIX
				Dim bKey As String = "e" & i + BIAS_KEY_SUFFIX

				Dim weights As INDArray = getParamWithNoise(wKey, True, workspaceMgr)

				Dim dLdW As INDArray = gradientViews(wKey)
				Dim dLdB As INDArray = gradientViews(bKey)

				Dim preOut As INDArray = fwd.encoderPreOuts(i)

				currentDelta = afn.backprop(preOut, epsilon).First

				Dim actInput As INDArray
				If i = 0 Then
					actInput = input
				Else
					actInput = fwd.encoderActivations(i - 1)
				End If
				Nd4j.gemm(actInput, currentDelta, dLdW, True, False, 1.0, 0.0)
				currentDelta.sum(dLdB, 0) 'dLdB is initialized/zeroed first in sum op

				gradient.gradientForVariable()(wKey) = dLdW
				gradient.gradientForVariable()(bKey) = dLdB

				If i = 0 Then
					epsilon = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, currentDelta.dataType(), New Long(){weights.size(0), currentDelta.size(0)}, "f"c)
					weights.mmuli(currentDelta.transpose(), epsilon)
					epsilon = epsilon.transpose()
				Else
					epsilon = weights.mmul(currentDelta.transpose()).transpose()
				End If
			Next i

			Return New Pair(Of Gradient, INDArray)(gradient, epsilon)
		End Function

		Public Overridable Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim f As VAEFwdHelper = doForward(training, False, workspaceMgr)
			Return f.pzxMeanPreOut
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data private static class VAEFwdHelper
		Private Class VAEFwdHelper
			Friend encoderPreOuts() As INDArray
			Friend pzxMeanPreOut As INDArray
			Friend encoderActivations() As INDArray
		End Class


		Private Function doForward(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As VAEFwdHelper
			assertInputSet(False)

			'TODO input validation

			Dim nEncoderLayers As Integer = encoderLayerSizes.Length

			Dim encoderPreOuts(encoderLayerSizes.Length - 1) As INDArray
			Dim encoderActivations(encoderLayerSizes.Length - 1) As INDArray
			Dim current As INDArray = input_Conflict.castTo(getParam("e0" & WEIGHT_KEY_SUFFIX).dataType())
			For i As Integer = 0 To nEncoderLayers - 1
				Dim wKey As String = "e" & i + WEIGHT_KEY_SUFFIX
				Dim bKey As String = "e" & i + BIAS_KEY_SUFFIX

				Dim weights As INDArray = getParamWithNoise(wKey, training, workspaceMgr)
				Dim bias As INDArray = getParamWithNoise(bKey, training, workspaceMgr)

				current = current.mmul(weights).addiRowVector(bias)
				If forBackprop Then
					encoderPreOuts(i) = current.dup()
				End If
				layerConf().getActivationFn().getActivation(current, training)
				encoderActivations(i) = current
			Next i

			'Finally, calculate mean value:
			Dim mW As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_W, training, workspaceMgr)
			Dim mB As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_B, training, workspaceMgr)

			Dim pzxMean As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, mW.dataType(), New Long(){current.size(0), mW.size(1)}, "f"c)
			pzxMean = current.mmuli(mW, pzxMean).addiRowVector(mB)


			Return New VAEFwdHelper(encoderPreOuts, pzxMean, encoderActivations)
		End Function

		Public Overridable Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			Dim output As INDArray = preOutput(training, workspaceMgr) 'Mean values for p(z|x)
			pzxActivationFn.getActivation(output, training)

			Return output
		End Function

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			setInput(input, workspaceMgr)
			Return activate(training, workspaceMgr)
		End Function

		Public Overridable Property Listeners As ICollection(Of TrainingListener)
			Get
				If trainingListeners Is Nothing Then
					Return Nothing
				End If
    
				Return New List(Of TrainingListener)(trainingListeners)
			End Get
			Set(ByVal listeners() As TrainingListener)
				setListeners(java.util.Arrays.asList(Of TrainingListener)(listeners))
			End Set
		End Property


		Public Overridable WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				If trainingListeners Is Nothing Then
					trainingListeners = New List(Of TrainingListener)()
				Else
					trainingListeners.Clear()
				End If
				If trainingListeners Is Nothing Then
					trainingListeners = New List(Of TrainingListener)()
				Else
					trainingListeners.Clear()
				End If
    
				If listeners IsNot Nothing AndAlso listeners.Count > 0 Then
					trainingListeners.addAll(listeners)
				End If
			End Set
		End Property


		''' <summary>
		''' This method ADDS additional TrainingListener to existing listeners
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable Sub addListeners(ParamArray ByVal listeners() As TrainingListener)
			If Me.trainingListeners Is Nothing Then
				setListeners(listeners)
				Return
			End If

			For Each listener As TrainingListener In listeners
				trainingListeners.Add(listener)
			Next listener
		End Sub

		Public Overridable Property Index Implements Layer.setIndex As Integer
			Set(ByVal index As Integer)
				Me.index_Conflict = index
			End Set
			Get
				Return index_Conflict
			End Get
		End Property


		Public Overridable Sub setInput(ByVal input As INDArray, ByVal layerWorkspaceMgr As LayerWorkspaceMgr) Implements Layer.setInput
			Me.input_Conflict = input
		End Sub

		Public Overridable Property InputMiniBatchSize Implements Layer.setInputMiniBatchSize As Integer
			Set(ByVal size As Integer)
    
			End Set
			Get
				If input_Conflict.size(0) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Return CInt(input_Conflict.size(0))
			End Get
		End Property


		Public Overridable Property MaskArray Implements Layer.setMaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				Me.maskArray_Conflict = maskArray
			End Set
			Get
				Return maskArray_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property PretrainLayer As Boolean Implements Layer.isPretrainLayer
			Get
				Return True
			End Get
		End Property

		Public Overridable Sub clearNoiseWeightParams() Implements Layer.clearNoiseWeightParams
			weightNoiseParams.Clear()
		End Sub

		Public Overridable Sub allowInputModification(ByVal allow As Boolean) Implements Layer.allowInputModification
			'No op
		End Sub

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)


			Throw New System.NotSupportedException("Not yet implemented " & layerId())
		End Function

		Public Overridable ReadOnly Property Helper As LayerHelper Implements Layer.getHelper
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Sub fit()
			If input_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot fit layer: layer input is null (not set) " & layerId())
			End If

			If solver Is Nothing Then
				Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					solver = (New Solver.Builder()).model(Me).configure(conf()).listeners(getListeners()).build()
				End Using
			End If
			Me.optimizer_Conflict = solver.Optimizer
			solver.optimize(LayerWorkspaceMgr.noWorkspaces()) 'TODO FIXME
		End Sub

		''' <summary>
		''' Calculate the reconstruction probability, as described in An & Cho, 2015 - "Variational Autoencoder based
		''' Anomaly Detection using Reconstruction Probability" (Algorithm 4)<br>
		''' The authors describe it as follows: "This is essentially the probability of the data being generated from a given
		''' latent variable drawn from the approximate posterior distribution."<br>
		''' <br>
		''' Specifically, for each example x in the input, calculate p(x). Note however that p(x) is a stochastic (Monte-Carlo)
		''' estimate of the true p(x), based on the specified number of samples. More samples will produce a more accurate
		''' (lower variance) estimate of the true p(x) for the current model parameters.<br>
		''' <br>
		''' Internally uses <seealso cref="reconstructionLogProbability(INDArray, Integer)"/> for the actual implementation.
		''' That method may be more numerically stable in some cases.<br>
		''' <br>
		''' The returned array is a column vector of reconstruction probabilities, for each example. Thus, reconstruction probabilities
		''' can (and should, for efficiency) be calculated in a batched manner.
		''' </summary>
		''' <param name="data">       The data to calculate the reconstruction probability for </param>
		''' <param name="numSamples"> Number of samples with which to base the reconstruction probability on. </param>
		''' <returns> Column vector of reconstruction probabilities for each example (shape: [numExamples,1]) </returns>
		Public Overridable Function reconstructionProbability(ByVal data As INDArray, ByVal numSamples As Integer) As INDArray
			Dim reconstructionLogProb As INDArray = reconstructionLogProbability(data, numSamples)
			Return Transforms.exp(reconstructionLogProb.castTo(DataType.DOUBLE), False) 'Cast to double to reduce risk of numerical underflow
		End Function

		''' <summary>
		''' Return the log reconstruction probability given the specified number of samples.<br>
		''' See <seealso cref="reconstructionLogProbability(INDArray, Integer)"/> for more details
		''' </summary>
		''' <param name="data">       The data to calculate the log reconstruction probability </param>
		''' <param name="numSamples"> Number of samples with which to base the reconstruction probability on. </param>
		''' <returns> Column vector of reconstruction log probabilities for each example (shape: [numExamples,1]) </returns>
		Public Overridable Function reconstructionLogProbability(ByVal data As INDArray, ByVal numSamples As Integer) As INDArray
			If numSamples <= 0 Then
				Throw New System.ArgumentException("Invalid input: numSamples must be > 0. Got: " & numSamples & " " & layerId())
			End If
			If TypeOf reconstructionDistribution Is LossFunctionWrapper Then
				Throw New System.NotSupportedException("Cannot calculate reconstruction log probability when using " & "a LossFunction (via LossFunctionWrapper) instead of a ReconstructionDistribution: ILossFunction " & "instances are not in general probabilistic, hence it is not possible to calculate reconstruction probability " & layerId())
			End If

			data = data.castTo(dataType)

			'Forward pass through the encoder and mean for P(Z|X)
			Dim workspaceMgr As LayerWorkspaceMgr = LayerWorkspaceMgr.noWorkspaces() 'TODO add workspace support to this method
			setInput(data, workspaceMgr)
			Dim fwd As VAEFwdHelper = doForward(True, True, workspaceMgr)
			Dim afn As IActivation = layerConf().getActivationFn()

			'Forward pass through logStd^2 for P(Z|X)
			Dim pzxLogStd2W As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_W, False, workspaceMgr)
			Dim pzxLogStd2b As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_B, False, workspaceMgr)

			Dim meanZ As INDArray = fwd.pzxMeanPreOut
			Dim logStdev2Z As INDArray = fwd.encoderActivations(fwd.encoderActivations.Length - 1).mmul(pzxLogStd2W).addiRowVector(pzxLogStd2b)
			pzxActivationFn.getActivation(meanZ, False)
			pzxActivationFn.getActivation(logStdev2Z, False)

			Dim pzxSigma As INDArray = Transforms.exp(logStdev2Z, False)
			Transforms.sqrt(pzxSigma, False)

			Dim minibatch As val = input_Conflict.size(0)
			Dim size As val = fwd.pzxMeanPreOut.size(1)

			Dim pxzw As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_W, False, workspaceMgr)
			Dim pxzb As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_B, False, workspaceMgr)

			Dim decoderWeights(decoderLayerSizes.Length - 1) As INDArray
			Dim decoderBiases(decoderLayerSizes.Length - 1) As INDArray

			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim wKey As String = "d" & i + WEIGHT_KEY_SUFFIX
				Dim bKey As String = "d" & i + BIAS_KEY_SUFFIX
				decoderWeights(i) = getParamWithNoise(wKey, False, workspaceMgr)
				decoderBiases(i) = getParamWithNoise(bKey, False, workspaceMgr)
			Next i

			Dim sumReconstructionNegLogProbability As INDArray = Nothing
			For i As Integer = 0 To numSamples - 1
				Dim e As INDArray = Nd4j.randn(dataType, minibatch, size)
				Dim z As INDArray = e.muli(pzxSigma).addi(meanZ) 'z = mu + sigma * e, with e ~ N(0,1)

				'Do forward pass through decoder
				Dim nDecoderLayers As Integer = decoderLayerSizes.Length
				Dim currentActivations As INDArray = z
				For j As Integer = 0 To nDecoderLayers - 1
					currentActivations = currentActivations.mmul(decoderWeights(j)).addiRowVector(decoderBiases(j))
					afn.getActivation(currentActivations, False)
				Next j

				'And calculate reconstruction distribution preOut
				Dim pxzDistributionPreOut As INDArray = currentActivations.mmul(pxzw).addiRowVector(pxzb)

				If i = 0 Then
					sumReconstructionNegLogProbability = reconstructionDistribution.exampleNegLogProbability(data, pxzDistributionPreOut)
				Else
					sumReconstructionNegLogProbability.addi(reconstructionDistribution.exampleNegLogProbability(data, pxzDistributionPreOut))
				End If
			Next i

			setInput(Nothing, workspaceMgr)
			Return sumReconstructionNegLogProbability.divi(-numSamples)
		End Function

		''' <summary>
		''' Given a specified values for the latent space as input (latent space being z in p(z|data)), generate output
		''' from P(x|z), where x = E[P(x|z)]<br>
		''' i.e., return the mean value for the distribution P(x|z)
		''' </summary>
		''' <param name="latentSpaceValues">    Values for the latent space. size(1) must equal nOut configuration parameter </param>
		''' <returns> Sample of data: E[P(x|z)] </returns>
		Public Overridable Function generateAtMeanGivenZ(ByVal latentSpaceValues As INDArray) As INDArray
			Dim pxzDistributionPreOut As INDArray = decodeGivenLatentSpaceValues(latentSpaceValues, LayerWorkspaceMgr.noWorkspaces()) 'TODO workspace support
			Return reconstructionDistribution.generateAtMean(pxzDistributionPreOut)
		End Function

		''' <summary>
		''' Given a specified values for the latent space as input (latent space being z in p(z|data)), randomly generate output
		''' x, where x ~ P(x|z)
		''' </summary>
		''' <param name="latentSpaceValues">    Values for the latent space. size(1) must equal nOut configuration parameter </param>
		''' <returns> Sample of data: x ~ P(x|z) </returns>
		Public Overridable Function generateRandomGivenZ(ByVal latentSpaceValues As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim pxzDistributionPreOut As INDArray = decodeGivenLatentSpaceValues(latentSpaceValues, workspaceMgr)
			Return reconstructionDistribution.generateRandom(pxzDistributionPreOut)
		End Function

		Private Function decodeGivenLatentSpaceValues(ByVal latentSpaceValues As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If latentSpaceValues.size(1) <> getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_W, False, workspaceMgr).size(1) Then
				Throw New System.ArgumentException("Invalid latent space values: expected size " & getParamWithNoise(VariationalAutoencoderParamInitializer.PZX_MEAN_W, False, workspaceMgr).size(1) & ", got size (dimension 1) = " & latentSpaceValues.size(1) & " " & layerId())
			End If

			'Do forward pass through decoder

			Dim nDecoderLayers As Integer = decoderLayerSizes.Length
			Dim currentActivations As INDArray = latentSpaceValues
			Dim afn As IActivation = layerConf().getActivationFn()

			For i As Integer = 0 To nDecoderLayers - 1
				Dim wKey As String = "d" & i + WEIGHT_KEY_SUFFIX
				Dim bKey As String = "d" & i + BIAS_KEY_SUFFIX
				Dim w As INDArray = getParamWithNoise(wKey, False, workspaceMgr)
				Dim b As INDArray = getParamWithNoise(bKey, False, workspaceMgr)
				currentActivations = currentActivations.mmul(w).addiRowVector(b)
				afn.getActivation(currentActivations, False)
			Next i

			Dim pxzw As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_W, False, workspaceMgr)
			Dim pxzb As INDArray = getParamWithNoise(VariationalAutoencoderParamInitializer.PXZ_B, False, workspaceMgr)
			Return currentActivations.mmul(pxzw).addiRowVector(pxzb)
		End Function

		''' <summary>
		''' Does the reconstruction distribution have a loss function (such as mean squared error) or is it a standard
		''' probabilistic reconstruction distribution?
		''' </summary>
		Public Overridable Function hasLossFunction() As Boolean
			Return reconstructionDistribution.hasLossFunction()
		End Function

		''' <summary>
		''' Return the reconstruction error for this variational autoencoder.<br>
		''' <b>NOTE (important):</b> This method is used ONLY for VAEs that have a standard neural network loss function (i.e.,
		''' an <seealso cref="ILossFunction"/> instance such as mean squared error) instead of using a
		''' probabilistic reconstruction distribution P(x|z) for the reconstructions (as presented in the VAE architecture by
		''' Kingma and Welling).<br>
		''' You can check if the VAE has a loss function using <seealso cref="hasLossFunction()"/><br>
		''' Consequently, the reconstruction error is a simple deterministic function (no Monte-Carlo sampling is required,
		''' unlike <seealso cref="reconstructionProbability(INDArray, Integer)"/> and <seealso cref="reconstructionLogProbability(INDArray, Integer)"/>)
		''' </summary>
		''' <param name="data">       The data to calculate the reconstruction error on </param>
		''' <returns> Column vector of reconstruction errors for each example (shape: [numExamples,1]) </returns>
		Public Overridable Function reconstructionError(ByVal data As INDArray) As INDArray
			If Not hasLossFunction() Then
				Throw New System.InvalidOperationException("Cannot use reconstructionError method unless the variational autoencoder is " & "configured with a standard loss function (via LossFunctionWrapper). For VAEs utilizing a reconstruction " & "distribution, use the reconstructionProbability or reconstructionLogProbability methods " & layerId())
			End If

			Dim pZXMean As INDArray = activate(data, False, LayerWorkspaceMgr.noWorkspaces())
			Dim reconstruction As INDArray = generateAtMeanGivenZ(pZXMean) 'Not probabilistic -> "mean" == output

			If TypeOf reconstructionDistribution Is CompositeReconstructionDistribution Then
				Dim c As CompositeReconstructionDistribution = DirectCast(reconstructionDistribution, CompositeReconstructionDistribution)
				Return c.computeLossFunctionScoreArray(data, reconstruction)
			Else

				Dim lfw As LossFunctionWrapper = DirectCast(reconstructionDistribution, LossFunctionWrapper)
				Dim lossFunction As ILossFunction = lfw.getLossFunction()

				'Re: the activation identity here - the reconstruction array already has the activation function applied,
				' so we don't want to apply it again. i.e., we are passing the output, not the pre-output.
				Return lossFunction.computeScoreArray(data, reconstruction, New ActivationIdentity(), Nothing)
			End If
		End Function

		Public Overridable Sub assertInputSet(ByVal backprop As Boolean)
			If input_Conflict Is Nothing Then
				If backprop Then
					Throw New System.InvalidOperationException("Cannot perform backprop in layer " & Me.GetType().Name & ": layer input field is not set")
				Else
					Throw New System.InvalidOperationException("Cannot perform forward pass in layer " & Me.GetType().Name & ": layer input field is not set")
				End If
			End If
		End Sub

		Public Overridable Sub close()
			'No-op for individual layers
		End Sub
	End Class

End Namespace