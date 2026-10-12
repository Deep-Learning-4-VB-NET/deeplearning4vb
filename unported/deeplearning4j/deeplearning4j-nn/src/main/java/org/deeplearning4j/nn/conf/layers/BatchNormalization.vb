Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports FeedForwardToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToCnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports BatchNormalizationParamInitializer = org.deeplearning4j.nn.params.BatchNormalizationParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) @Builder public class BatchNormalization extends FeedForwardLayer
	<Serializable>
	Public Class BatchNormalization
		Inherits FeedForwardLayer

		'Note: need to set defaults here in addition to builder, in case user uses no-op constructor...
		Protected Friend decay As Double = 0.9
		Protected Friend eps As Double = 1e-5
		Protected Friend isMinibatch As Boolean = True
		Protected Friend gamma As Double = 1.0
		Protected Friend beta As Double = 0.0
		Protected Friend lockGammaBeta As Boolean = False
		Protected Friend cudnnAllowFallback As Boolean = True
		Protected Friend useLogStd As Boolean = False 'Default for deserialized models (1.0.0-beta3) and earlier: store variance as variance. Post 1.0.0-beta3: use log stdev instead
		Protected Friend cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW 'Default for deserialized models, 1.0.0-beta6 and earlier

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.decay = builder.decay_Conflict
			Me.eps = builder.eps_Conflict
			Me.isMinibatch = builder.isMinibatch
			Me.gamma = builder.gamma_Conflict
			Me.beta = builder.beta_Conflict
			Me.lockGammaBeta = builder.lockGammaBeta_Conflict
			Me.cudnnAllowFallback = builder.cudnnAllowFallback_Conflict
			Me.useLogStd = builder.useLogStd_Conflict
			Me.cnn2DFormat = builder.cnn2DFormat
			initializeConstraints(builder)
		End Sub

		Public Sub New()
			Me.New(New Builder()) 'Defaults from builder
		End Sub

		Public Overrides Function clone() As BatchNormalization
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As BatchNormalization = CType(MyBase.clone(), BatchNormalization)
			Return clone_Conflict
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNOutSet("BatchNormalization", getLayerName(), layerIndex, getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.normalization.BatchNormalization(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return BatchNormalizationParamInitializer.Instance
		End Function


		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input type: Batch norm layer expected input of type CNN, got null for layer """ & getLayerName() & """")
			End If

			'Can handle CNN, flat CNN, CNN3D or FF input formats only
			Select Case inputType.getType()
				Case FF, CNN, CNNFlat, CNN3D, RNN
					Return inputType 'OK
				Case Else
					Throw New System.InvalidOperationException("Invalid input type: Batch norm layer expected input of type CNN, CNN Flat or FF, got " & inputType & " for layer index " & layerIndex & ", layer name = " & getLayerName())
			End Select
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If nIn <= 0 OrElse override Then
				Select Case inputType.getType()
					Case FF
						nIn = DirectCast(inputType, InputType.InputTypeFeedForward).getSize()
					Case CNN
						nIn = DirectCast(inputType, InputType.InputTypeConvolutional).getChannels()
						cnn2DFormat = DirectCast(inputType, InputType.InputTypeConvolutional).getFormat()
					Case CNN3D
						nIn = DirectCast(inputType, InputType.InputTypeConvolutional3D).getChannels()
					Case CNNFlat
						nIn = DirectCast(inputType, InputType.InputTypeConvolutionalFlat).getDepth()
					Case RNN
						Dim inputTypeRecurrent As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
						nIn = inputTypeRecurrent.getSize()
					Case Else
						Throw New System.InvalidOperationException("Invalid input type: Batch norm layer expected input of type CNN, CNN Flat or FF, got " & inputType & " for layer " & getLayerName() & """")
				End Select
				nOut = nIn
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			If inputType.getType() = InputType.Type.CNNFlat Then
				Dim i As InputType.InputTypeConvolutionalFlat = DirectCast(inputType, InputType.InputTypeConvolutionalFlat)
				Return New FeedForwardToCnnPreProcessor(i.getHeight(), i.getWidth(), i.getDepth())
			End If

			Return Nothing
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'Don't regularize batch norm params: similar to biases in the sense that there are not many of them...
			Return Nothing
		End Function

		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			Select Case paramName
				Case BatchNormalizationParamInitializer.BETA, BatchNormalizationParamInitializer.GAMMA
					Return iUpdater
				Case BatchNormalizationParamInitializer.GLOBAL_MEAN, BatchNormalizationParamInitializer.GLOBAL_VAR, BatchNormalizationParamInitializer.GLOBAL_LOG_STD
					Return New NoOp()
				Case Else
					Throw New System.ArgumentException("Unknown parameter: """ & paramName & """")
			End Select
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			'TODO CuDNN helper etc

			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As Integer = 0

			For Each s As String In BatchNormalizationParamInitializer.Instance.paramKeys(Me)
				updaterStateSize += getUpdaterByParam(s).stateSize(nOut)
			Next s

			'During forward pass: working memory size approx. equal to 2x input size (copy ops, etc)
			Dim inferenceWorkingSize As val = 2 * inputType.arrayElementsPerExample()

			'During training: we calculate mean and variance... result is equal to nOut, and INDEPENDENT of minibatch size
			Dim trainWorkFixed As val = 2 * nOut
			'During backprop: multiple working arrays... output size, 2 * output size (indep. of example size),
			Dim trainWorkingSizePerExample As val = inferenceWorkingSize + (outputType.arrayElementsPerExample() + 2 * nOut) 'Backprop gradient calculation

			Return (New LayerMemoryReport.Builder(layerName, GetType(BatchNormalization), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, trainWorkFixed, trainWorkingSizePerExample).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False 'No pretrain params in BN
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Getter @Setter public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			''' <summary>
			''' At test time: we can use a global estimate of the mean and variance, calculated using a moving average of the
			''' batch means/variances. This moving average is implemented as:<br> globalMeanEstimate = decay *
			''' globalMeanEstimate + (1-decay) * batchMean<br> globalVarianceEstimate = decay * globalVarianceEstimate +
			''' (1-decay) * batchVariance<br>
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field decay was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend decay_Conflict As Double = 0.9

			''' <summary>
			''' Epsilon value for batch normalization; small floating point value added to variance (algorithm 1 in <a
			''' href="https://arxiv.org/pdf/1502.03167v3.pdf">https://arxiv.org/pdf/1502.03167v3.pdf</a>) to reduce/avoid
			''' underflow issues.<br> Default: 1e-5
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field eps was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend eps_Conflict As Double = 1e-5

			''' <summary>
			''' If doing minibatch training or not. Default: true. Under most circumstances, this should be set to true. If
			''' doing full batch training (i.e., all examples in a single DataSet object - very small data sets) then this
			''' should be set to false. Affects how global mean/variance estimates are calculated.
			''' 
			''' </summary>
			Protected Friend isMinibatch As Boolean = True ' TODO auto set this if layer conf is batch

			''' <summary>
			''' If set to true: lock the gamma and beta parameters to the values for each activation, specified by {@link
			''' #gamma(double)} and <seealso cref="beta(Double)"/>. Default: false -> learn gamma and beta parameter values during
			''' network training.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field lockGammaBeta was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lockGammaBeta_Conflict As Boolean = False

			''' <summary>
			''' Used only when 'true' is passed to <seealso cref="lockGammaBeta(Boolean)"/>. Value is not used otherwise.<br> Default:
			''' 1.0
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field gamma was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gamma_Conflict As Double = 1.0

			''' <summary>
			''' Used only when 'true' is passed to <seealso cref="lockGammaBeta(Boolean)"/>. Value is not used otherwise.<br> Default:
			''' 0.0
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field beta was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend beta_Conflict As Double = 0.0

			''' <summary>
			''' Set constraints to be applied to the beta parameter of this batch normalisation layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' 
			''' </summary>
			Protected Friend betaConstraints As IList(Of LayerConstraint)

			''' <summary>
			''' Set constraints to be applied to the gamma parameter of this batch normalisation layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' 
			''' </summary>
			Protected Friend gammaConstraints As IList(Of LayerConstraint)

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for BatchNormalization will be used
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cudnnAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAllowFallback_Conflict As Boolean = True

			''' <summary>
			''' How should the moving average of variance be stored? Two different parameterizations are supported.
			''' useLogStd(false): equivalent to 1.0.0-beta3 and earlier. The variance "parameter" is stored directly as
			''' variable<br> useLogStd(true): (Default) variance is stored as log10(stdev)<br> The motivation here is for
			''' numerical stability (FP16 etc) and also distributed training: storing the variance directly can cause
			''' numerical issues. For example, a standard deviation of 1e-3 (something that could be encountered in practice)
			''' gives a variance of 1e-6, which can be problematic for 16-bit floating point
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field useLogStd was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useLogStd_Conflict As Boolean = True

			Protected Friend cnn2DFormat As CNN2DFormat = CNN2DFormat.NCHW 'Default for deserialized models, 1.0.0-beta6 and earlier

			Public Sub New(ByVal decay As Double, ByVal isMinibatch As Boolean)
				Me.setDecay(decay)
				Me.setMinibatch(isMinibatch)
			End Sub

			Public Sub New(ByVal gamma As Double, ByVal beta As Double)
				Me.setGamma(gamma)
				Me.setBeta(beta)
			End Sub

			Public Sub New(ByVal gamma As Double, ByVal beta As Double, ByVal lockGammaBeta As Boolean)
				Me.setGamma(gamma)
				Me.setBeta(beta)
				Me.setLockGammaBeta(lockGammaBeta)
			End Sub

			Public Sub New(ByVal lockGammaBeta As Boolean)
				Me.setLockGammaBeta(lockGammaBeta)
			End Sub

			Public Sub New()
			End Sub

			''' <summary>
			''' Set the input and output array data format. Defaults to NCHW format - i.e., channels first.
			''' See <seealso cref="CNN2DFormat"/> for more details </summary>
			''' <param name="format"> Format to use </param>
			Public Overridable Function dataFormat(ByVal format As CNN2DFormat) As Builder
				Me.cnn2DFormat = format
				Return Me
			End Function

			''' <summary>
			''' If doing minibatch training or not. Default: true. Under most circumstances, this should be set to true. If
			''' doing full batch training (i.e., all examples in a single DataSet object - very small data sets) then this
			''' should be set to false. Affects how global mean/variance estimates are calculated.
			''' </summary>
			''' <param name="minibatch"> Minibatch parameter </param>
'JAVA TO VB CONVERTER NOTE: The parameter minibatch was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minibatch(ByVal minibatch_Conflict As Boolean) As Builder
				Me.setMinibatch(minibatch_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Used only when 'true' is passed to <seealso cref="lockGammaBeta(Boolean)"/>. Value is not used otherwise.<br> Default:
			''' 1.0
			''' </summary>
			''' <param name="gamma"> Gamma parameter for all activations, used only with locked gamma/beta configuration mode </param>
'JAVA TO VB CONVERTER NOTE: The parameter gamma was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gamma(ByVal gamma_Conflict As Double) As Builder
				Me.setGamma(gamma_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Used only when 'true' is passed to <seealso cref="lockGammaBeta(Boolean)"/>. Value is not used otherwise.<br> Default:
			''' 0.0
			''' </summary>
			''' <param name="beta"> Beta parameter for all activations, used only with locked gamma/beta configuration mode </param>
'JAVA TO VB CONVERTER NOTE: The parameter beta was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function beta(ByVal beta_Conflict As Double) As Builder
				Me.setBeta(beta_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Epsilon value for batch normalization; small floating point value added to variance (algorithm 1 in <a
			''' href="https://arxiv.org/pdf/1502.03167v3.pdf">https://arxiv.org/pdf/1502.03167v3.pdf</a>) to reduce/avoid
			''' underflow issues.<br> Default: 1e-5
			''' </summary>
			''' <param name="eps"> Epsilon values to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter eps was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function eps(ByVal eps_Conflict As Double) As Builder
				Me.setEps(eps_Conflict)
				Return Me
			End Function

			''' <summary>
			''' At test time: we can use a global estimate of the mean and variance, calculated using a moving average of the
			''' batch means/variances. This moving average is implemented as:<br> globalMeanEstimate = decay *
			''' globalMeanEstimate + (1-decay) * batchMean<br> globalVarianceEstimate = decay * globalVarianceEstimate +
			''' (1-decay) * batchVariance<br>
			''' </summary>
			''' <param name="decay"> Decay value to use for global stats calculation </param>
'JAVA TO VB CONVERTER NOTE: The parameter decay was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function decay(ByVal decay_Conflict As Double) As Builder
				Me.setDecay(decay_Conflict)
				Return Me
			End Function

			''' <summary>
			''' If set to true: lock the gamma and beta parameters to the values for each activation, specified by {@link
			''' #gamma(double)} and <seealso cref="beta(Double)"/>. Default: false -> learn gamma and beta parameter values during
			''' network training.
			''' </summary>
			''' <param name="lockGammaBeta"> If true: use fixed beta/gamma values. False: learn during </param>
'JAVA TO VB CONVERTER NOTE: The parameter lockGammaBeta was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lockGammaBeta(ByVal lockGammaBeta_Conflict As Boolean) As Builder
				Me.setLockGammaBeta(lockGammaBeta_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to the beta parameter of this batch normalisation layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to the beta parameter of this layer </param>
			Public Overridable Function constrainBeta(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.setBetaConstraints(Arrays.asList(constraints))
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to the gamma parameter of this batch normalisation layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of parameters,
			''' max-norm regularization, etc). These constraints are applied at each iteration, after the parameters have
			''' been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to the gamma parameter of this layer </param>
			Public Overridable Function constrainGamma(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.setGammaConstraints(Arrays.asList(constraints))
				Return Me
			End Function

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If true, the built-in
			''' (non-CuDNN) implementation for BatchNormalization will be used
			''' </summary>
			''' @deprecated Use <seealso cref="helperAllowFallback(Boolean)"/>
			''' 
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			<Obsolete("Use <seealso cref=""helperAllowFallback(Boolean)""/>")>
			Public Overridable Function cudnnAllowFallback(ByVal allowFallback As Boolean) As Builder
				Me.setCudnnAllowFallback(allowFallback)
				Return Me
			End Function

			''' <summary>
			''' When using CuDNN or MKLDNN and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If true, the built-in
			''' (non-MKL/CuDNN) implementation for BatchNormalizationLayer will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-CuDNN implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As Builder
				Me.cudnnAllowFallback_Conflict = allowFallback
				Return Me
			End Function

			''' <summary>
			''' How should the moving average of variance be stored? Two different parameterizations are supported.
			''' useLogStd(false): equivalent to 1.0.0-beta3 and earlier. The variance "parameter" is stored directly as
			''' variable<br> useLogStd(true): (Default) variance is stored as log10(stdev)<br> The motivation here is for
			''' numerical stability (FP16 etc) and also distributed training: storing the variance directly can cause
			''' numerical issues. For example, a standard deviation of 1e-3 (something that could be encountered in practice)
			''' gives a variance of 1e-6, which can be problematic for 16-bit floating point
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter useLogStd was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useLogStd(ByVal useLogStd_Conflict As Boolean) As Builder
				Me.setUseLogStd(useLogStd_Conflict)
				Return Me
			End Function

			Public Overrides Function build() As BatchNormalization
				Return New BatchNormalization(Me)
			End Function
		End Class

	End Class

End Namespace