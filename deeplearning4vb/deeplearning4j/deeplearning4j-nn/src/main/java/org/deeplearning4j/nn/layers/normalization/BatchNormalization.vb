Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MKLDNNBatchNormHelper = org.deeplearning4j.nn.layers.mkldnn.MKLDNNBatchNormHelper
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports DivOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.DivOp
Imports SubOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SubOp
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports Longs = org.nd4j.shade.guava.primitives.Longs

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

Namespace org.deeplearning4j.nn.layers.normalization


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BatchNormalization extends org.deeplearning4j.nn.layers.BaseLayer<org.deeplearning4j.nn.conf.layers.BatchNormalization>
	<Serializable>
	Public Class BatchNormalization
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.BatchNormalization)

		Protected Friend Shared ReadOnly ONE_ON_2LOGE_10 As Double = 1.0 / (2 * Math.Log(10.0))

'JAVA TO VB CONVERTER NOTE: The field helper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend helper_Conflict As BatchNormalizationHelper = Nothing
		Protected Friend helperCountFail As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field index was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shadows index_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend listeners_Conflict As IList(Of TrainingListener) = New List(Of TrainingListener)()
		Protected Friend std As INDArray
		Protected Friend xMu As INDArray
		Protected Friend xHat As INDArray
		Public Const BATCH_NORM_CUDNN_HELPER_CLASS_NAME As String = "org.deeplearning4j.cuda.normalization.CudnnBatchNormalizationHelper"
		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			initializeHelper()
		End Sub

		Friend Overridable Sub initializeHelper()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			helper_Conflict = HelperUtils.createHelper(BATCH_NORM_CUDNN_HELPER_CLASS_NAME,GetType(MKLDNNBatchNormHelper).FullName, GetType(BatchNormalizationHelper), layerConf().LayerName, dataType)
			'specific helper with alpha/beta, keep this last check around
			If helper_Conflict IsNot Nothing AndAlso Not helper_Conflict.checkSupported(layerConf().getEps(), layerConf().isLockGammaBeta()) Then
				log.debug("Removed helper {} as not supported with epsilon {}, lockGammaBeta={}", helper_Conflict.GetType(), layerConf().getEps(), layerConf().isLockGammaBeta())
				helper_Conflict = Nothing
			End If
		End Sub

		Public Overrides Function type() As Type
			Return Type.NORMALIZATION
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim nextEpsilon As INDArray
			Dim shape As val = getShape(epsilon)
			Dim batchSize As val = epsilon.size(0) ' number examples in batch
			Dim layerConf As org.deeplearning4j.nn.conf.layers.BatchNormalization = Me.layerConf()

			Dim format As CNN2DFormat = Me.layerConf().getCnn2DFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim chIdx As Integer = If(epsilon.rank() = 2 OrElse nchw, 1, 3)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No-op if correct type

			Dim globalMean As INDArray = params(BatchNormalizationParamInitializer.GLOBAL_MEAN)
			Dim globalVar As INDArray = params(BatchNormalizationParamInitializer.GLOBAL_VAR) 'One of log10std will be null depending on config
			Dim globalLog10Std As INDArray = params(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
			Dim gamma As INDArray = Nothing
			Dim beta As INDArray = Nothing
			Dim dGammaView As INDArray
			Dim dBetaView As INDArray
			Dim dGlobalMeanView As INDArray = gradientViews(BatchNormalizationParamInitializer.GLOBAL_MEAN)
			Dim dGlobalVarView As INDArray = gradientViews(BatchNormalizationParamInitializer.GLOBAL_VAR)
			Dim dGlobalLog10StdView As INDArray = gradientViews(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
			If layerConf.isLockGammaBeta() Then
				Dim tempShape As val = New Long() {1, shape(chIdx)}
				dGammaView = Nd4j.createUninitialized(dataType, tempShape, "c"c)
				dBetaView = Nd4j.createUninitialized(dataType, tempShape, "c"c)
			Else
				gamma = getParam(BatchNormalizationParamInitializer.GAMMA)
				beta = getParam(BatchNormalizationParamInitializer.BETA)
				dGammaView = gradientViews(BatchNormalizationParamInitializer.GAMMA)
				dBetaView = gradientViews(BatchNormalizationParamInitializer.BETA)
			End If

			Dim retGradient As Gradient = New DefaultGradient()


			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not Me.layerConf().isCudnnAllowFallback()) Then
				'Note that cudnn does not support dense (2d) batch norm case as of v5.1
				If layerConf.isLockGammaBeta() Then
					gamma = Nd4j.createUninitialized(dataType, 1, shape(chIdx)).assign(layerConf.getGamma())
				End If

				Dim [in] As INDArray
				Dim eps As INDArray
				If input.rank() = 2 Then
					Dim shapeTemp() As Long = If(nchw, New Long(){input.size(0), input.size(1), 1, 1}, New Long()){input.size(0), 1, 1, input.size(1)}
					[in] = input.reshape(input.ordering(), shapeTemp)
					eps = epsilon.reshape(epsilon.ordering(), shapeTemp)
				Else
					[in] = input
					eps = epsilon
				End If

				Dim ret As Pair(Of Gradient, INDArray) = Nothing
				Try
					ret = helper_Conflict.backpropGradient([in], eps, shape, gamma, beta, dGammaView, dBetaView, layerConf.getEps(), format, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch t As Exception
					If t.getMessage() IsNot Nothing AndAlso t.getMessage().contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw t
					End If

					If Me.layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN BatchNormalization backprop execution failed - falling back on built-in implementation",t)
					Else
						Throw New Exception("Error during BatchNormalization CuDNN helper backprop - isCudnnAllowFallback() is set to false", t)
					End If
				End Try
				If ret IsNot Nothing Then
					ret.First.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_MEAN, dGlobalMeanView)
					If Me.layerConf().isUseLogStd() Then
						ret.First.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_LOG_STD, dGlobalLog10StdView)
					Else
						ret.First.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_VAR, dGlobalVarView)
					End If

					If input.rank() = 2 Then
						Dim e As INDArray = ret.Second
						ret.Second = e.reshape(e.ordering(), e.size(0), e.size(1))
					End If

	'                
	'                Handling of global mean and variance:
	'                Normally the design for batch norm is to:
	'                    globalMean = decay * globalMean + (1-decay) * minibatchMean
	'                    globalVar  = decay * globalVar  + (1-decay) * minibatchVar
	'                However, because of distributed training (gradient sharing), we don't want to do this...
	'                Instead: We'll use the mathematically equivalent but "distributed safe" approach of:
	'                mean[t+1] = mean[t] - updateMean
	'                updateMean = mean[t] - mean[t+1] = (1-d) * (mean[t] - minibatchMean)
	'                And use the same idea for global variance estimate.
	'
	'                Note also that we have 2 supported parameterizations here:
	'                1. global variance estimate (only option until after 1.0.0-beta3)
	'                2. global log10(std) estimate
	'                These make zero difference for local training (other than perhaps when using FP16), but the latter is more
	'                numerically stable and is scaled better for distributed training
	'                 
					Dim batchMean As INDArray = helper_Conflict.getMeanCache(dataType)
					Dim batchVar As INDArray = helper_Conflict.getVarCache(dataType)

					Nd4j.Executioner.exec(New SubOp(globalMean, batchMean, dGlobalMeanView)) 'deltaGlobalMean = globalMean[t] - batchMean
					dGlobalMeanView.muli(1-Me.layerConf().getDecay())

					If Me.layerConf().isUseLogStd() Then
						'Use log10(std) parameterization. This is more numerically stable for FP16 and better for distributed training
						'First: we have log10(var[i]) from last iteration, hence can calculate var[i] and stdev[i]
						'Need to calculate log10{std[i]) - log10(std[i+1]) as the "update"
						'Note, var[i+1] = d*var[i] + (1-d)*batchVar
						Dim vari As INDArray = Nd4j.createUninitialized(dataType, globalLog10Std.shape()).assign(10.0)
						Transforms.pow(vari, globalLog10Std, False) 'variance = (10^log10(s))^2
						vari.muli(vari)

						Dim decay As Double = Me.layerConf().getDecay()
						Dim varip1 As INDArray = vari.mul(decay).addi(batchVar.mul(1-decay))
						Nd4j.Executioner.exec(New DivOp(vari, varip1, dGlobalLog10StdView))
						Transforms.log(dGlobalLog10StdView, False)
						dGlobalLog10StdView.muli(ONE_ON_2LOGE_10)
					Else
						'Use variance estimate parameterization. This was only option up to and including 1.0.0-beta3
						Nd4j.Executioner.exec(New SubOp(globalVar, batchVar, dGlobalVarView)) 'deltaGlobalVar = globalVar[t] - batchVar
						dGlobalVarView.muli(1 - Me.layerConf().getDecay())
					End If

					Return ret
				End If
			End If

			Dim batchMean As INDArray
			Dim batchVar As INDArray
			If epsilon.rank() = 2 Then
				If xHat Is Nothing AndAlso helper_Conflict IsNot Nothing Then
					Dim mean As INDArray = helper_Conflict.getMeanCache(dataType)
					std = Transforms.sqrt(helper_Conflict.getVarCache(dataType).addi(Me.layerConf().getEps()))
					xMu = Nd4j.createUninitialized(dataType, input.shape(), input.ordering())
					xMu = Nd4j.Executioner.exec(New BroadcastSubOp(input, mean, xMu, 1))
					xHat = Nd4j.createUninitialized(dataType, input.shape(), input.ordering())
					xHat = Nd4j.Executioner.exec(New BroadcastDivOp(xMu, std,xHat, 1))
				End If

				'TODO: handle fixed beta/gamma case...
				Dim dBeta As INDArray = epsilon.sum(True, 0) 'dL/dBeta = sum_examples dL/dOut
				Dim dGamma As INDArray = epsilon.mul(xHat).sum(True, 0) 'dL/dGamma = sum_examples dL/dOut .* xHat
				Dim dxhat As INDArray
				If layerConf.isLockGammaBeta() Then
					dxhat = epsilon.mul(layerConf.getGamma())
				Else
					'Standard case
					dxhat = epsilon.mulRowVector(gamma) 'dL/dxHat = dL/dOut . gamma        Shape: [minibatchSize, nOut]
				End If


				'dL/dVariance
				Dim dLdVar As INDArray = dxhat.mul(xMu).sum(True, 0).muli(-0.5).muli(Transforms.pow(std, -3.0, True)) 'Shape: [1, miniBatch]

				'dL/dmu
				Dim dxmu1 As INDArray = dxhat.sum(True, 0).divi(std).negi()
				Dim dxmu2 As INDArray = xMu.sum(True, 0).muli(-2.0 / batchSize).muli(dLdVar)

				Dim dLdmu As INDArray = dxmu1.addi(dxmu2) 'Shape: [1, nOut]

				'Note the array reuse here: dxhat, xMu, dLdVar, dLdmu - all are invalid after this line (but aren't used later anyway)
				Dim dLdx As INDArray = dxhat.diviRowVector(std).addi(xMu.muliRowVector(dLdVar.muli(2.0 / batchSize))).addiRowVector(dLdmu.muli(1.0 / batchSize))

				'TODO rework this to avoid the assign here
				dGammaView.assign(dGamma)
				dBetaView.assign(dBeta)

				retGradient.setGradientFor(BatchNormalizationParamInitializer.GAMMA, dGammaView)
				retGradient.setGradientFor(BatchNormalizationParamInitializer.BETA, dBetaView)

				nextEpsilon = dLdx

				batchMean = input.mean(0)
				batchVar = input.var(False, 0)
			ElseIf epsilon.rank() = 4 Then
				Dim nonChDims() As Integer = If(nchw, New Integer(){0, 2, 3}, New Integer()){0, 1, 2}
				Dim hIdx As Integer = If(nchw, 2, 1)
				Dim wIdx As Integer = If(nchw, 3, 2)

				If xHat Is Nothing AndAlso helper_Conflict IsNot Nothing Then
					Dim mean As INDArray = helper_Conflict.getMeanCache(dataType)
					std = Transforms.sqrt(helper_Conflict.getVarCache(dataType).addi(Me.layerConf().getEps()))
					xMu = Nd4j.createUninitialized(dataType, input.shape(), input.ordering())
					xMu = Nd4j.Executioner.exec(New BroadcastSubOp(input, mean, xMu, chIdx))
					xHat = Nd4j.createUninitialized(dataType, input.shape(), input.ordering())
					xHat = Nd4j.Executioner.exec(New BroadcastDivOp(xMu, std,xHat, chIdx))
				End If

				Dim dBeta As INDArray = epsilon.sum(nonChDims)
				Dim dGamma As INDArray = epsilon.mul(xHat).sum(nonChDims)
				Dim dxhat As INDArray
				If layerConf.isLockGammaBeta() Then
					dxhat = epsilon.mul(layerConf.getGamma())
				Else
					'Standard case
					dxhat = Nd4j.Executioner.exec(New BroadcastMulOp(epsilon, gamma, Nd4j.createUninitialized(epsilon.dataType(), epsilon.shape(), epsilon.ordering()), chIdx))
				End If

				'dL/dVariance
				Dim dLdVar As INDArray = dxhat.mul(xMu).sum(nonChDims).muli(-0.5).muli(Transforms.pow(std, -3.0, True))

				'dL/dmu
				Dim effectiveBatchSize As val = input.size(0) * input.size(hIdx) * input.size(wIdx)
				Dim dxmu1 As INDArray = dxhat.sum(nonChDims).divi(std).negi()
				Dim dxmu2 As INDArray = xMu.sum(nonChDims).muli(-2.0 / effectiveBatchSize).muli(dLdVar)
				Dim dLdmu As INDArray = dxmu1.addi(dxmu2)

				Dim dLdx As INDArray = Nd4j.Executioner.exec(New BroadcastDivOp(dxhat, std, dxhat, chIdx)).addi(Nd4j.Executioner.exec(New BroadcastMulOp(xMu, dLdVar.muli(2.0 / effectiveBatchSize), xMu, chIdx)))
				Nd4j.Executioner.execAndReturn(New BroadcastAddOp(dLdx, dLdmu.muli(1.0 / effectiveBatchSize), dLdx, chIdx))

				'TODO rework this to avoid the assign here
				dGammaView.assign(dGamma)
				dBetaView.assign(dBeta)

				retGradient.setGradientFor(BatchNormalizationParamInitializer.GAMMA, dGammaView)
				retGradient.setGradientFor(BatchNormalizationParamInitializer.BETA, dBetaView)

				nextEpsilon = dLdx
				batchMean = input.mean(nonChDims)
				batchVar = input.var(False, nonChDims)
			Else
				' TODO setup BatchNorm for RNN https://arxiv.org/pdf/1510.01378v1.pdf
				Throw New System.InvalidOperationException("The layer prior to BatchNorm in the configuration is not currently supported. " & layerId())
			End If


	'        
	'        Handling of global mean and variance:
	'        Normally the design for batch norm is to:
	'            globalMean = decay * globalMean + (1-decay) * minibatchMean
	'            globalVar  = decay * globalVar  + (1-decay) * minibatchVar
	'        However, because of distributed training (gradient sharing), we don't want to do this...
	'        Instead: We'll use the mathematically equivalent but "distributed safe" approach of:
	'        mean[t+1] = mean[t] - updateMean
	'        updateMean = mean[t] - mean[t+1] = (1-d) * (mean[t] - minibatchMean)
	'        And use the same idea for global variance estimate
	'         

			Nd4j.Executioner.exec(New SubOp(globalMean, batchMean, dGlobalMeanView)) 'deltaGlobalMean = globalMean[t] - batchMean
			dGlobalMeanView.muli(1-Me.layerConf().getDecay())

			If Me.layerConf().isUseLogStd() Then
				'Use log10(std) parameterization. This is more numerically stable for FP16 and better for distributed training
				'First: we have log10(var[i]) from last iteration, hence can calculate var[i] and stdev[i]
				'Need to calculate log10{std[i]) - log10(std[i+1]) as the "update"
				'Note, var[i+1] = d*var[i] + (1-d)*batchVar
				Dim vari As INDArray = Nd4j.valueArrayOf(globalLog10Std.shape(), 10.0, globalMean.dataType())
				Transforms.pow(vari, globalLog10Std, False) 'variance = (10^log10(s))^2
				vari.muli(vari)

				Dim decay As Double = Me.layerConf().getDecay()
				Dim varip1 As INDArray = vari.mul(decay).addi(batchVar.mul(1-decay))
				Nd4j.Executioner.exec(New DivOp(vari, varip1, dGlobalLog10StdView))
				Transforms.log(dGlobalLog10StdView, False)
				dGlobalLog10StdView.muli(ONE_ON_2LOGE_10)
			Else
				'Use variance estimate parameterization. This was only option up to and including 1.0.0-beta3
				Nd4j.Executioner.exec(New SubOp(globalVar, batchVar, dGlobalVarView)) 'deltaGlobalVar = globalVar[t] - batchVar
				dGlobalVarView.muli(1 - Me.layerConf().getDecay())
			End If

			retGradient.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_MEAN, dGlobalMeanView)
			If Me.layerConf().isUseLogStd() Then
				retGradient.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_LOG_STD, dGlobalLog10StdView)
			Else
				retGradient.setGradientFor(BatchNormalizationParamInitializer.GLOBAL_VAR, dGlobalVarView)
			End If


			'TODO could optimize this
			nextEpsilon = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, nextEpsilon)

			xHat = Nothing
			xMu = Nothing

			Return New Pair(Of Gradient, INDArray)(retGradient, nextEpsilon)
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Return preOutput(input_Conflict,If(training, TrainingMode.TRAIN, TrainingMode.TEST), workspaceMgr)
		End Function

		Public Overrides Function gradient() As Gradient
			Return gradient_Conflict
		End Function

		Public Overridable Overloads Function preOutput(ByVal x As INDArray, ByVal training As TrainingMode, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim [dim] As Integer = 1
			Dim originalInput As INDArray = x
			Dim rnnInput As Boolean = False
			'RNN input
			If x.rank() = 3 Then
				x = x.reshape(Longs.concat(New Long(){1},x.shape()))
				rnnInput = True
			End If
			If x.rank() = 4 AndAlso layerConf().getCnn2DFormat() = CNN2DFormat.NHWC Then
				[dim] = 3
			End If
			If x.size([dim]) <> layerConf().getNOut() Then
				Throw New System.ArgumentException("input.size(" & [dim] & ") does not match expected input size of " & layerConf().getNIn() & " - got input array with shape " & java.util.Arrays.toString(x.shape()))
			End If
			x = x.castTo(dataType) 'No-op if correct type

			Dim activations As INDArray
			' TODO add this directly in layer or get the layer prior...

			Dim layerConf As org.deeplearning4j.nn.conf.layers.BatchNormalization = Me.layerConf()
			Dim shape As val = getShape(x)

			Dim gamma As INDArray = Nothing
			Dim beta As INDArray = Nothing
			Dim globalMeanView As INDArray = getParam(BatchNormalizationParamInitializer.GLOBAL_MEAN)
			Dim globalVarView As INDArray = getParam(BatchNormalizationParamInitializer.GLOBAL_VAR) 'Either this or log10std will be null depending on config
			If layerConf.isLockGammaBeta() Then
				If helper_Conflict IsNot Nothing AndAlso input_Conflict.rank() = 4 Then
					'TODO: don't create these each iteration, when using cudnn
					Dim gammaBetaShape As val = New Long() {1, Me.layerConf().getNOut()}
					gamma = Nd4j.valueArrayOf(gammaBetaShape, Me.layerConf().getGamma(), dataType)
					beta = Nd4j.valueArrayOf(gammaBetaShape, Me.layerConf().getBeta(), dataType)
				End If
			Else
				gamma = getParam(BatchNormalizationParamInitializer.GAMMA)
				beta = getParam(BatchNormalizationParamInitializer.BETA)
			End If

			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not Me.layerConf().isCudnnAllowFallback()) Then

				Dim [in] As INDArray = x
				If x.rank() = 2 Then
					[in] = x.reshape(x.ordering(), [in].size(0), [in].size(1), 1, 1)
				End If

				'Note that cudnn does not support dense (2d) batch norm case as of v7.1
				Dim decay As Double = layerConf.getDecay()

				Dim ret As INDArray = Nothing
				Try
					If globalVarView Is Nothing Then
						'May be null when useLogStd is true
						Dim log10s As INDArray = getParam(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
						globalVarView = Transforms.pow(Nd4j.valueArrayOf(log10s.shape(), 10.0, dataType), log10s, False)
						globalVarView.muli(globalVarView)
					End If

					ret = helper_Conflict.preOutput([in], training = TrainingMode.TRAIN, shape, gamma, beta, globalMeanView, globalVarView, decay, layerConf.getEps(), Me.layerConf().getCnn2DFormat(), workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch t As Exception
					If t.getMessage() IsNot Nothing AndAlso t.getMessage().contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw t
					End If

					If Me.layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN BatchNormalization forward pass execution failed - falling back on built-in implementation",t)
					Else
						Throw New Exception("Error during BatchNormalization CuDNN helper backprop - isCudnnAllowFallback() is set to false", t)
					End If
				End Try
				If ret IsNot Nothing Then
					If input_Conflict.rank() = 2 Then
						Return ret.reshape(ret.ordering(), ret.size(0), ret.size(1))
					ElseIf originalInput.rank() = 3 AndAlso ret.rank() = 4 Then
						Return ret.reshape(ret.ordering(),ret.size(1),ret.size(2),ret.size(3))
					Else
						Return ret
					End If
				End If
			End If

			Dim format As CNN2DFormat = Me.layerConf().getCnn2DFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim chIdx As Integer = If(nchw, 1, 3)
			Dim nonChDims() As Integer = If(nchw, New Integer(){0, 2, 3}, New Integer()){0, 1, 2}
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			' xHat = (x-xmean) / sqrt(var + epsilon)
			'Note that for CNNs, mean and variance are calculated per feature map (i.e., per activation) rather than per activation
			'Pg5 of https://arxiv.org/pdf/1502.03167v3.pdf
			' "For convolutional layers, we additionally want the normalization to obey the convolutional property – so that
			'  different elements of the same feature map, at different locations, are normalized in the same way. To achieve
			'  this, we jointly normalize all the activations in a minibatch, over all locations."
			Dim mean, var As INDArray
			If training = TrainingMode.TRAIN Then
				Select Case x.rank()
					Case 2
						' mean and variance over samples in batch
						mean = x.mean(0)
						var = x.var(False, 0)
					Case 4
						' mean and variance over samples AND locations

						mean = x.mean(nonChDims)
						var = x.var(False, nonChDims)
					Case Else
						Throw New System.InvalidOperationException("Batch normalization on activations of rank " & x.rank() & " not supported " & layerId())
				End Select

				std = Transforms.sqrt(workspaceMgr.dup(ArrayType.INPUT, var).addi(Me.layerConf().getEps()), False)
			Else
				' Global mean and variance estimate - used after training
				mean = getParam(BatchNormalizationParamInitializer.GLOBAL_MEAN)
				If Me.layerConf().isUseLogStd() Then
					'var = (10^(log10(s)))^2
					Dim log10s As INDArray = getParam(BatchNormalizationParamInitializer.GLOBAL_LOG_STD)
					var = Transforms.pow(Nd4j.valueArrayOf(log10s.shape(), 10.0, dataType), log10s)
					var.muli(var)
				Else
					var = getParam(BatchNormalizationParamInitializer.GLOBAL_VAR)
				End If
				std = Transforms.sqrt(workspaceMgr.dup(ArrayType.INPUT, var).addi(Me.layerConf().getEps()), False)
			End If

			' BN(xk) = gamma*xˆ + β (applying gamma and beta for each activation)
			If x.rank() = 2 Then
				xMu = workspaceMgr.leverageTo(ArrayType.INPUT, x.subRowVector(mean))
				xHat = workspaceMgr.leverageTo(ArrayType.INPUT, xMu.divRowVector(std))


				If layerConf.isLockGammaBeta() Then
					'Special case: gamma/beta have fixed values for all outputs
					'Use mul/addi(Number) here to avoid allocating temp arrays of all same value
					Dim g As Double = layerConf.getGamma()
					Dim b As Double = layerConf.getBeta()
					If g <> 1.0 AndAlso b <> 0.0 Then
						'Default and most common case: 1.0 and 0.0 for these parameters. No point executing 1 * x + 0 op
						activations = xHat.mul(g).addi(b)
					Else
						activations = xHat
					End If
				Else
					'Standard case: gamma and beta are learned per parameter
					activations = xHat.mulRowVector(gamma).addiRowVector(beta)
				End If
			ElseIf x.rank() = 4 Then
				If Not Shape.strideDescendingCAscendingF(x) Then
					x = x.dup() 'TODO: temp Workaround for broadcast bug. To be removed when fixed
				End If
				xMu = workspaceMgr.createUninitialized(ArrayType.INPUT, x.dataType(), x.shape(), x.ordering())
				xMu = Nd4j.Executioner.exec(New BroadcastSubOp(x, mean,xMu, chIdx))
				xHat = workspaceMgr.createUninitialized(ArrayType.INPUT, x.dataType(), x.shape(), x.ordering())
				xHat = Nd4j.Executioner.exec(New BroadcastDivOp(xMu, std,xHat, chIdx))

				If layerConf.isLockGammaBeta() Then
					'Special case: gamma/beta have fixed values for all outputs
					'Use mul/addi(Number) here to avoid allocating temp arrays of all same value
					Dim g As Double = layerConf.getGamma()
					Dim b As Double = layerConf.getBeta()
					If g <> 1.0 AndAlso b <> 0.0 Then
						'Default and most common case: 1.0 and 0.0 for these parameters. No point executing 1 * x + 0 op
						activations = xHat.mul(g).addi(b)
					Else
						activations = xHat
					End If
				Else
					'Standard case: gamma and beta are learned per parameter
					activations = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, x.dataType(), x.shape(), x.ordering())
					activations = Nd4j.Executioner.exec(New BroadcastMulOp(xHat, gamma, activations, chIdx))
					activations = Nd4j.Executioner.exec(New BroadcastAddOp(activations, beta, activations, chIdx))
				End If
			Else
				' TODO setup BatchNorm for RNN https://arxiv.org/pdf/1510.01378v1.pdf
				Throw New System.InvalidOperationException("The layer prior to BatchNorm in the configuration is not currently supported. " & layerId())
			End If

	'        
	'        A note regarding running mean and variance updating:
	'        Normally these are updated like globalMean = decay * globalMean + (1-decay) * minibatchMean
	'        However, because of distributed training (gradient sharing), we don't want to do this...
	'        Instead: We'll use the mathematically equivalent but "distributed safe" approach of:
	'        mean[t+1] = mean[t] - updateMean
	'        updateMean = mean[t] - mean[t+1] = (1-d) * (mean[t] - minibatchMean)
	'        And use the same idea for global variance estimate
	'         

			activations = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, activations) 'Most of the time this should be a no-op
			If rnnInput Then
			   'change back the output to rank 3 after running batch norm for rnn inputs
				activations = activations.reshape(ChrW(activations.size(1)), activations.size(2), activations.size(3))
			End If
			Return activations
		End Function

		Public Overrides Property Listeners As ICollection(Of TrainingListener)
			Get
				Return listeners_Conflict
			End Get
			Set(ByVal listeners() As TrainingListener)
				Me.listeners_Conflict = New List(Of TrainingListener)(java.util.Arrays.asList(listeners))
			End Set
		End Property


		Public Overrides Property Index As Integer
			Set(ByVal index As Integer)
				Me.index_Conflict = index
			End Set
			Get
				Return index_Conflict
			End Get
		End Property


		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Helper As LayerHelper
			Get
				Return helper_Conflict
			End Get
		End Property

		Public Overridable Function getShape(ByVal x As INDArray) As Long()
			If x.rank() = 2 Then
				Return New Long() {1, x.size(1)}
			End If
			If x.rank() = 4 Then
				Dim chIdx As Integer = If(layerConf().getCnn2DFormat() = CNN2DFormat.NCHW, 1, 3)
				Return New Long(){1, x.size(chIdx)}
			End If
			If x.rank() = 3 Then
				Dim wDim As val = x.size(1)
				Dim hdim As val = x.size(2)
				If x.size(0) > 1 AndAlso wDim * hdim = x.length() Then
					Throw New System.ArgumentException("Illegal input for batch size " & layerId())
				End If
				Return New Long() {1, wDim * hdim}
			Else
				Throw New System.InvalidOperationException("Unable to process input of rank " & x.rank() & " " & layerId())
			End If
		End Function

		Public Overrides Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean
			'Majority of params's gradients should be... Exception: batch norm mean/variance estimate
			If BatchNormalizationParamInitializer.GLOBAL_MEAN.Equals(paramName) OrElse BatchNormalizationParamInitializer.GLOBAL_VAR.Equals(paramName) OrElse BatchNormalizationParamInitializer.GLOBAL_LOG_STD.Equals(paramName) Then
				Return False
			End If
			Return True
		End Function

	End Class

End Namespace