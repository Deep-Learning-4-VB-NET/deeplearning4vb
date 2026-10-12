Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports org.deeplearning4j.nn.conf
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports org.deeplearning4j.nn.conf.layers
Imports StepFunction = org.deeplearning4j.nn.conf.stepfunctions.StepFunction
Imports IWeightNoise = org.deeplearning4j.nn.conf.weightnoise.IWeightNoise
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports org.nd4j.common.primitives
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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

Namespace org.deeplearning4j.nn.transferlearning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "type") @JsonInclude(JsonInclude.Include.NON_NULL) @NoArgsConstructor @AllArgsConstructor @Data public class FineTuneConfiguration
	Public Class FineTuneConfiguration

		Protected Friend activationFn As IActivation
		Protected Friend weightInitFn As IWeightInit
		Protected Friend biasInit As Double?
		Protected Friend regularization As IList(Of Regularization)
		Protected Friend regularizationBias As IList(Of Regularization)
		Protected Friend removeL2 As Boolean = False 'For: .l2(0.0) -> user means "no l2" so we should remove it if it is present in the original model...
		Protected Friend removeL2Bias As Boolean = False
		Protected Friend removeL1 As Boolean = False
		Protected Friend removeL1Bias As Boolean = False
		Protected Friend removeWD As Boolean = False
		Protected Friend removeWDBias As Boolean = False
		Protected Friend dropout As [Optional](Of IDropout)
		Protected Friend weightNoise As [Optional](Of IWeightNoise)
		Protected Friend updater As IUpdater
		Protected Friend biasUpdater As IUpdater
		Protected Friend miniBatch As Boolean?
		Protected Friend maxNumLineSearchIterations As Integer?
		Protected Friend seed As Long?
		Protected Friend optimizationAlgo As OptimizationAlgorithm
		Protected Friend stepFunction As StepFunction
		Protected Friend minimize As Boolean?
		Protected Friend gradientNormalization As [Optional](Of GradientNormalization)
		Protected Friend gradientNormalizationThreshold As Double?
		Protected Friend convolutionMode As ConvolutionMode
		Protected Friend cudnnAlgoMode As ConvolutionLayer.AlgoMode
		Protected Friend constraints As [Optional](Of IList(Of LayerConstraint))

		Protected Friend pretrain As Boolean?
		Protected Friend backprop As Boolean?
		Protected Friend backpropType As BackpropType
		Protected Friend tbpttFwdLength As Integer?
		Protected Friend tbpttBackLength As Integer?

		Protected Friend trainingWorkspaceMode As WorkspaceMode
		Protected Friend inferenceWorkspaceMode As WorkspaceMode

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

	'    
	'     * Can't use Lombok @Builder annotation due to optionals (otherwise we have a bunch of ugly .x(Optional<T> value)
	'      * methods - lombok builder doesn't support excluding fields? :(
	'     * Note the use of optional here: gives us 3 states...
	'     * 1. Null: not set
	'     * 2. Optional (empty): set to null
	'     * 3. Optional (not empty): set to specific value
	'     *
	'     * Obviously, having null only makes sense for some things (dropout, etc) whereas null for other things doesn't
	'     * make sense
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ToString public static class Builder
		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field activation was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend activation_Conflict As IActivation
			Friend weightInitFn As IWeightInit
'JAVA TO VB CONVERTER NOTE: The field biasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend biasInit_Conflict As Double?
			Protected Friend regularization As IList(Of Regularization) = New List(Of Regularization)()
			Protected Friend regularizationBias As IList(Of Regularization) = New List(Of Regularization)()
			Protected Friend removeL2 As Boolean = False 'For: .l2(0.0) -> user means "no l2" so we should remove it if it is present in the original model...
			Protected Friend removeL2Bias As Boolean = False
			Protected Friend removeL1 As Boolean = False
			Protected Friend removeL1Bias As Boolean = False
			Protected Friend removeWD As Boolean = False
			Protected Friend removeWDBias As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field dropout was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dropout_Conflict As [Optional](Of IDropout)
'JAVA TO VB CONVERTER NOTE: The field weightNoise was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend weightNoise_Conflict As [Optional](Of IWeightNoise)
'JAVA TO VB CONVERTER NOTE: The field updater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend updater_Conflict As IUpdater
'JAVA TO VB CONVERTER NOTE: The field biasUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend biasUpdater_Conflict As IUpdater
'JAVA TO VB CONVERTER NOTE: The field miniBatch was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend miniBatch_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field maxNumLineSearchIterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maxNumLineSearchIterations_Conflict As Integer?
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend seed_Conflict As Long?
'JAVA TO VB CONVERTER NOTE: The field optimizationAlgo was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend optimizationAlgo_Conflict As OptimizationAlgorithm
'JAVA TO VB CONVERTER NOTE: The field stepFunction was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stepFunction_Conflict As StepFunction
'JAVA TO VB CONVERTER NOTE: The field minimize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minimize_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field gradientNormalization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend gradientNormalization_Conflict As [Optional](Of GradientNormalization)
'JAVA TO VB CONVERTER NOTE: The field gradientNormalizationThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend gradientNormalizationThreshold_Conflict As Double?
'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend convolutionMode_Conflict As ConvolutionMode
'JAVA TO VB CONVERTER NOTE: The field cudnnAlgoMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend cudnnAlgoMode_Conflict As ConvolutionLayer.AlgoMode
'JAVA TO VB CONVERTER NOTE: The field constraints was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend constraints_Conflict As [Optional](Of IList(Of LayerConstraint))
'JAVA TO VB CONVERTER NOTE: The field pretrain was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend pretrain_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field backprop was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend backprop_Conflict As Boolean?
'JAVA TO VB CONVERTER NOTE: The field backpropType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend backpropType_Conflict As BackpropType
'JAVA TO VB CONVERTER NOTE: The field tbpttFwdLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend tbpttFwdLength_Conflict As Integer?
'JAVA TO VB CONVERTER NOTE: The field tbpttBackLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend tbpttBackLength_Conflict As Integer?
'JAVA TO VB CONVERTER NOTE: The field trainingWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend trainingWorkspaceMode_Conflict As WorkspaceMode
'JAVA TO VB CONVERTER NOTE: The field inferenceWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inferenceWorkspaceMode_Conflict As WorkspaceMode

			Public Sub New()

			End Sub

			''' <summary>
			''' Activation function / neuron non-linearity
			''' </summary>
			Public Overridable Function activation(ByVal activationFn As IActivation) As Builder
				Me.activation_Conflict = activationFn
				Return Me
			End Function

			''' <summary>
			''' Activation function / neuron non-linearity
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Me.activation_Conflict = activation_Conflict.getActivationFunction()
				Return Me
			End Function

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' </summary>
			''' <seealso cref= IWeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As IWeightInit) As Builder
				Me.weightInitFn = weightInit_Conflict
				Return Me
			End Function

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' </summary>
			''' <seealso cref= WeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As WeightInit) As Builder
				If weightInit_Conflict = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, User weightInit(Distribution distribution) instead!")
				End If

				Me.weightInitFn = weightInit_Conflict.getWeightInitFunction()
				Return Me
			End Function


			''' <summary>
			''' Set weight initialization scheme to random sampling via the specified distribution.
			''' Equivalent to: {@code .weightInit(new WeightInitDistribution(distribution))}
			''' </summary>
			''' <param name="distribution"> Distribution to use for weight initialization </param>
			Public Overridable Function weightInit(ByVal distribution As Distribution) As Builder
				Return weightInit(New WeightInitDistribution(distribution))
			End Function

			''' <summary>
			''' Constant for bias initialization. Default: 0.0
			''' </summary>
			''' <param name="biasInit"> Constant for bias initialization </param>
'JAVA TO VB CONVERTER NOTE: The parameter biasInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function biasInit(ByVal biasInit_Conflict As Double) As Builder
				Me.biasInit_Conflict = biasInit_Conflict
				Return Me
			End Function

			''' <summary>
			''' Distribution to sample initial weights from.
			''' Equivalent to: {@code .weightInit(new WeightInitDistribution(distribution))}
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter dist was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function dist(ByVal dist_Conflict As Distribution) As Builder
				Return weightInit(dist_Conflict)
			End Function

			''' <summary>
			''' L1 regularization coefficient for the weights (excluding biases)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1(ByVal l1_Conflict As Double) As Builder
				NetworkUtils.removeInstances(regularization, GetType(L1Regularization))
				If l1_Conflict > 0.0 Then
					regularization.Add(New L1Regularization(l1_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' L2 regularization coefficient for the weights (excluding biases)<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecay(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l2 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2(ByVal l2_Conflict As Double) As Builder
				NetworkUtils.removeInstances(regularization, GetType(L2Regularization))
				If l2_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(regularization, GetType(WeightDecay), "WeightDecay regularization removed: incompatible with added L2 regularization")
					regularization.Add(New L2Regularization(l2_Conflict))
				Else
					removeL2 = True
				End If
				Return Me
			End Function

			''' <summary>
			''' L1 regularization coefficient for the bias parameters
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1Bias(ByVal l1Bias_Conflict As Double) As Builder
				NetworkUtils.removeInstances(regularizationBias, GetType(L1Regularization))
				If l1Bias_Conflict > 0.0 Then
					regularizationBias.Add(New L1Regularization(l1Bias_Conflict))
				Else
					removeL1Bias = True
				End If
				Return Me
			End Function

			''' <summary>
			''' L2 regularization coefficient for the bias parameters<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecayBias(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l2Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2Bias(ByVal l2Bias_Conflict As Double) As Builder
				NetworkUtils.removeInstances(regularizationBias, GetType(L2Regularization))
				If l2Bias_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(regularizationBias, GetType(WeightDecay), "WeightDecay bias regularization removed: incompatible with added L2 regularization")
					regularizationBias.Add(New L2Regularization(l2Bias_Conflict))
				Else
					removeL2Bias = True
				End If
				Return Me
			End Function

			''' <summary>
			''' Add weight decay regularization for the network parameters (excluding biases).<br>
			''' This applies weight decay <i>with</i> multiplying the learning rate - see <seealso cref="WeightDecay"/> for more details.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <seealso cref= #weightDecay(double, boolean) </seealso>
			Public Overridable Function weightDecay(ByVal coefficient As Double) As Builder
				Return weightDecay(coefficient, True)
			End Function

			''' <summary>
			''' Add weight decay regularization for the network parameters (excluding biases). See <seealso cref="WeightDecay"/> for more details.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <param name="applyLR">     Whether the learning rate should be multiplied in when performing weight decay updates. See <seealso cref="WeightDecay"/> for more details. </param>
			''' <seealso cref= #weightDecay(double, boolean) </seealso>
			Public Overridable Function weightDecay(ByVal coefficient As Double, ByVal applyLR As Boolean) As Builder
				'Check if existing weight decay if it exists; if so, replace it. Also remove L2 - it doesn't make sense to use both
				NetworkUtils.removeInstances(Me.regularization, GetType(WeightDecay))
				If coefficient > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularization, GetType(L2Regularization), "L2 regularization removed: incompatible with added WeightDecay regularization")
					Me.regularization.Add(New WeightDecay(coefficient, applyLR))
				Else
					removeWD = True
				End If
				Return Me
			End Function

			''' <summary>
			''' Weight decay for the biases only - see <seealso cref="weightDecay(Double)"/> for more details.
			''' This applies weight decay <i>with</i> multiplying the learning rate.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <seealso cref= #weightDecayBias(double, boolean) </seealso>
			Public Overridable Function weightDecayBias(ByVal coefficient As Double) As Builder
				Return weightDecayBias(coefficient, True)
			End Function

			''' <summary>
			''' Weight decay for the biases only - see <seealso cref="weightDecay(Double)"/> for more details<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			Public Overridable Function weightDecayBias(ByVal coefficient As Double, ByVal applyLR As Boolean) As Builder
				'Check if existing weight decay if it exists; if so, replace it. Also remove L2 - it doesn't make sense to use both
				NetworkUtils.removeInstances(Me.regularizationBias, GetType(WeightDecay))
				If coefficient > 0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias, GetType(L2Regularization), "L2 bias regularization removed: incompatible with added WeightDecay regularization")
					Me.regularizationBias.Add(New WeightDecay(coefficient, applyLR))
				Else
					removeWDBias = True
				End If
				Return Me
			End Function

			''' <summary>
			''' Set the dropout
			''' </summary>
			''' <param name="dropout"> Dropout, such as <seealso cref="Dropout"/>, <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianDropout"/>,
			'''                <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianNoise"/> etc </param>
'JAVA TO VB CONVERTER NOTE: The parameter dropout was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dropout(ByVal dropout_Conflict As IDropout) As Builder
				Me.dropout_Conflict = [Optional].ofNullable(dropout_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Dropout probability. This is the probability of <it>retaining</it> each input activation value for a layer.
			''' dropOut(x) will keep an input activation with probability x, and set to 0 with probability 1-x.<br>
			''' dropOut(0.0) is a special value / special case - when set to 0.0., dropout is disabled (not applied). Note
			''' that a dropout value of 1.0 is functionally equivalent to no dropout: i.e., 100% probability of retaining
			''' each input activation.<br>
			''' <para>
			''' Note 1: Dropout is applied at training time only - and is automatically not applied at test time
			''' (for evaluation, etc)<br>
			''' Note 2: This sets the probability per-layer. Care should be taken when setting lower values for
			''' complex networks (too much information may be lost with aggressive (very low) dropout values).<br>
			''' Note 3: Frequently, dropout is not applied to (or, has higher retain probability for) input (first layer)
			''' layers. Dropout is also often not applied to output layers. This needs to be handled MANUALLY by the user
			''' - set .dropout(0) on those layers when using global dropout setting.<br>
			''' Note 4: Implementation detail (most users can ignore): DL4J uses inverted dropout, as described here:
			''' <a href="http://cs231n.github.io/neural-networks-2/">http://cs231n.github.io/neural-networks-2/</a>
			''' </para>
			''' </summary>
			''' <param name="inputRetainProbability"> Dropout probability (probability of retaining each input activation value for a layer) </param>
			''' <seealso cref= #dropout(IDropout) </seealso>
			Public Overridable Function dropOut(ByVal inputRetainProbability As Double) As Builder
				If inputRetainProbability = 0.0 Then
					Return dropout(Nothing)
				End If
				Return dropout(New Dropout(inputRetainProbability))
			End Function

			''' <summary>
			''' Set the weight noise (such as <seealso cref="org.deeplearning4j.nn.conf.weightnoise.DropConnect"/> and
			''' <seealso cref="org.deeplearning4j.nn.conf.weightnoise.WeightNoise"/>)
			''' </summary>
			''' <param name="weightNoise"> Weight noise instance to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter weightNoise was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightNoise(ByVal weightNoise_Conflict As IWeightNoise) As Builder
				Me.weightNoise_Conflict = [Optional].ofNullable(weightNoise_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Gradient updater configuration. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/>
			''' or <seealso cref="org.nd4j.linalg.learning.config.Nesterovs"/>
			''' </summary>
			''' <param name="updater"> Updater to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function updater(ByVal updater_Conflict As IUpdater) As Builder
				Me.updater_Conflict = updater_Conflict
				Return Me
			End Function

			''' @deprecated Use <seealso cref="updater(IUpdater)"/> 
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""updater(IUpdater)""/>")>
			Public Overridable Function updater(ByVal updater_Conflict As Updater) As Builder
				Return updater(updater_Conflict.getIUpdaterWithDefaultConfig())
			End Function

			''' <summary>
			''' Gradient updater configuration, for the biases only. If not set, biases will use the updater as
			''' set by <seealso cref="updater(IUpdater)"/>
			''' </summary>
			''' <param name="biasUpdater"> Updater to use for bias parameters </param>
'JAVA TO VB CONVERTER NOTE: The parameter biasUpdater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function biasUpdater(ByVal biasUpdater_Conflict As IUpdater) As Builder
				Me.biasUpdater_Conflict = biasUpdater_Conflict
				Return Me
			End Function

			''' <summary>
			''' Whether scores and gradients should be divided by the minibatch size.<br>
			''' Most users should leave this ast he default value of true.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter miniBatch was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function miniBatch(ByVal miniBatch_Conflict As Boolean) As Builder
				Me.miniBatch_Conflict = miniBatch_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter maxNumLineSearchIterations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxNumLineSearchIterations(ByVal maxNumLineSearchIterations_Conflict As Integer) As Builder
				Me.maxNumLineSearchIterations_Conflict = maxNumLineSearchIterations_Conflict
				Return Me
			End Function

			''' <summary>
			''' RNG seed for reproducibility </summary>
			''' <param name="seed"> RNG seed to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Long) As Builder
				Me.seed_Conflict = seed_Conflict
				Return Me
			End Function

			''' <summary>
			''' RNG seed for reproducibility </summary>
			''' <param name="seed"> RNG seed to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Integer) As Builder
				Return Me.seed(CLng(seed_Conflict))
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter optimizationAlgo was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function optimizationAlgo(ByVal optimizationAlgo_Conflict As OptimizationAlgorithm) As Builder
				Me.optimizationAlgo_Conflict = optimizationAlgo_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter stepFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stepFunction(ByVal stepFunction_Conflict As StepFunction) As Builder
				Me.stepFunction_Conflict = stepFunction_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter minimize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minimize(ByVal minimize_Conflict As Boolean) As Builder
				Me.minimize_Conflict = minimize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Gradient normalization strategy. Used to specify gradient renormalization, gradient clipping etc.
			''' See <seealso cref="GradientNormalization"/> for details
			''' </summary>
			''' <param name="gradientNormalization"> Type of normalization to use. Defaults to None. </param>
			''' <seealso cref= GradientNormalization </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter gradientNormalization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gradientNormalization(ByVal gradientNormalization_Conflict As GradientNormalization) As Builder
				Me.gradientNormalization_Conflict = [Optional].ofNullable(gradientNormalization_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Threshold for gradient normalization, only used for GradientNormalization.ClipL2PerLayer,
			''' GradientNormalization.ClipL2PerParamType, and GradientNormalization.ClipElementWiseAbsoluteValue<br>
			''' Not used otherwise.<br>
			''' L2 threshold for first two types of clipping, or absolute value threshold for last type of clipping
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter gradientNormalizationThreshold was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gradientNormalizationThreshold(ByVal gradientNormalizationThreshold_Conflict As Double) As Builder
				Me.gradientNormalizationThreshold_Conflict = gradientNormalizationThreshold_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the convolution mode for convolutional layers, which impacts padding and output sizes.
			''' See <seealso cref="ConvolutionMode"/> for details. Defaults to ConvolutionMode.TRUNCATE<br> </summary>
			''' <param name="convolutionMode"> Convolution mode to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter convolutionMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function convolutionMode(ByVal convolutionMode_Conflict As ConvolutionMode) As Builder
				Me.convolutionMode_Conflict = convolutionMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the cuDNN algo mode for convolutional layers, which impacts performance and memory usage of cuDNN.
			''' See <seealso cref="ConvolutionLayer.AlgoMode"/> for details.  Defaults to "PREFER_FASTEST", but "NO_WORKSPACE" uses less memory.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter cudnnAlgoMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function cudnnAlgoMode(ByVal cudnnAlgoMode_Conflict As ConvolutionLayer.AlgoMode) As Builder
				Me.cudnnAlgoMode_Conflict = cudnnAlgoMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to all layers. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all parameters of all layers </param>
'JAVA TO VB CONVERTER NOTE: The parameter constraints was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function constraints(ByVal constraints_Conflict As IList(Of LayerConstraint)) As Builder
				Me.constraints_Conflict = [Optional].ofNullable(constraints_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter pretrain was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function pretrain(ByVal pretrain_Conflict As Boolean) As Builder
				Me.pretrain_Conflict = pretrain_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter backprop was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function backprop(ByVal backprop_Conflict As Boolean) As Builder
				Me.backprop_Conflict = backprop_Conflict
				Return Me
			End Function

			''' <summary>
			''' The type of backprop. Default setting is used for most networks (MLP, CNN etc),
			''' but optionally truncated BPTT can be used for training recurrent neural networks.
			''' If using TruncatedBPTT make sure you set both tBPTTForwardLength() and tBPTTBackwardLength()
			''' </summary>
			''' <param name="backpropType"> Type of backprop. Default: BackpropType.Standard </param>
'JAVA TO VB CONVERTER NOTE: The parameter backpropType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function backpropType(ByVal backpropType_Conflict As BackpropType) As Builder
				Me.backpropType_Conflict = backpropType_Conflict
				Return Me
			End Function

			''' <summary>
			''' When doing truncated BPTT: how many steps of forward pass should we do
			''' before doing (truncated) backprop?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' Typically tBPTTForwardLength parameter is same as the tBPTTBackwardLength parameter,
			''' but may be larger than it in some circumstances (but never smaller)<br>
			''' Ideally your training data time series length should be divisible by this
			''' This is the k1 parameter on pg23 of
			''' <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="tbpttFwdLength"> Forward length > 0, >= backwardLength </param>
'JAVA TO VB CONVERTER NOTE: The parameter tbpttFwdLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tbpttFwdLength(ByVal tbpttFwdLength_Conflict As Integer) As Builder
				Me.tbpttFwdLength_Conflict = tbpttFwdLength_Conflict
				Return Me
			End Function

			''' <summary>
			''' When doing truncated BPTT: how many steps of backward should we do?<br>
			''' Only applicable when doing backpropType(BackpropType.TruncatedBPTT)<br>
			''' This is the k2 parameter on pg23 of
			''' <a href="http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf">http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf</a>
			''' </summary>
			''' <param name="tbpttBackLength"> <= forwardLength </param>
'JAVA TO VB CONVERTER NOTE: The parameter tbpttBackLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tbpttBackLength(ByVal tbpttBackLength_Conflict As Integer) As Builder
				Me.tbpttBackLength_Conflict = tbpttBackLength_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines Workspace mode being used during training:
			''' NONE: workspace won't be used
			''' ENABLED: workspaces will be used for training (reduced memory and better performance)
			''' </summary>
			''' <param name="trainingWorkspaceMode"> Workspace mode for training </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter trainingWorkspaceMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function trainingWorkspaceMode(ByVal trainingWorkspaceMode_Conflict As WorkspaceMode) As Builder
				Me.trainingWorkspaceMode_Conflict = trainingWorkspaceMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines Workspace mode being used during inference:<br>
			''' NONE: workspace won't be used<br>
			''' ENABLED: workspaces will be used for inference (reduced memory and better performance)
			''' </summary>
			''' <param name="inferenceWorkspaceMode"> Workspace mode for inference </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter inferenceWorkspaceMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inferenceWorkspaceMode(ByVal inferenceWorkspaceMode_Conflict As WorkspaceMode) As Builder
				Me.inferenceWorkspaceMode_Conflict = inferenceWorkspaceMode_Conflict
				Return Me
			End Function

			Public Overridable Function build() As FineTuneConfiguration
				Return New FineTuneConfiguration(activation_Conflict, weightInitFn, biasInit_Conflict, regularization, regularizationBias, removeL2, removeL2Bias, removeL1, removeL1Bias, removeWD, removeWDBias, dropout_Conflict, weightNoise_Conflict, updater_Conflict, biasUpdater_Conflict, miniBatch_Conflict, maxNumLineSearchIterations_Conflict, seed_Conflict, optimizationAlgo_Conflict, stepFunction_Conflict, minimize_Conflict, gradientNormalization_Conflict, gradientNormalizationThreshold_Conflict, convolutionMode_Conflict, cudnnAlgoMode_Conflict, constraints_Conflict, pretrain_Conflict, backprop_Conflict, backpropType_Conflict, tbpttFwdLength_Conflict, tbpttBackLength_Conflict, trainingWorkspaceMode_Conflict, inferenceWorkspaceMode_Conflict)
			End Function
		End Class


		Public Overridable Function appliedNeuralNetConfiguration(ByVal nnc As NeuralNetConfiguration) As NeuralNetConfiguration
			applyToNeuralNetConfiguration(nnc)
			nnc = (New NeuralNetConfiguration.Builder(nnc.clone())).build()
			Return nnc
		End Function

		Public Overridable Sub applyToNeuralNetConfiguration(ByVal nnc As NeuralNetConfiguration)

			Dim l As Layer = nnc.getLayer()
			Dim originalUpdater As Updater = Nothing
			Dim origWeightInit As WeightInit = Nothing

			If l IsNot Nothing Then
				'As per NeuralNetConfiguration.configureLayer and LayerValidation.configureBaseLayer: only copy dropout to base layers
				' this excludes things like subsampling and activation layers
				If dropout IsNot Nothing AndAlso TypeOf l Is BaseLayer Then
					Dim d As IDropout = dropout.orElse(Nothing)
					If d IsNot Nothing Then
						d = d.clone() 'Clone to avoid shared state between layers
					End If
					l.setIDropout(d)
				End If
				If constraints IsNot Nothing Then
					l.setConstraints(constraints.orElse(Nothing))
				End If
			End If

			If l IsNot Nothing AndAlso TypeOf l Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(l, BaseLayer)
				If activationFn IsNot Nothing Then
					bl.setActivationFn(activationFn)
				End If
				If weightInitFn IsNot Nothing Then
					bl.setWeightInitFn(weightInitFn)
				End If
				If biasInit IsNot Nothing Then
					bl.setBiasInit(biasInit)
				End If
				If regularization IsNot Nothing AndAlso regularization.Count > 0 Then
					bl.setRegularization(regularization)
				End If
				If regularizationBias IsNot Nothing AndAlso regularizationBias.Count > 0 Then
					bl.setRegularizationBias(regularizationBias)
				End If
				If removeL2 Then
					NetworkUtils.removeInstances(bl.getRegularization(), GetType(L2Regularization))
				End If
				If removeL2Bias Then
					NetworkUtils.removeInstances(bl.getRegularizationBias(), GetType(L2Regularization))
				End If
				If removeL1 Then
					NetworkUtils.removeInstances(bl.getRegularization(), GetType(L1Regularization))
				End If
				If removeL1Bias Then
					NetworkUtils.removeInstances(bl.getRegularizationBias(), GetType(L1Regularization))
				End If
				If removeWD Then
					NetworkUtils.removeInstances(bl.getRegularization(), GetType(WeightDecay))
				End If
				If removeWDBias Then
					NetworkUtils.removeInstances(bl.getRegularizationBias(), GetType(WeightDecay))
				End If
				If gradientNormalization IsNot Nothing Then
					bl.setGradientNormalization(gradientNormalization.orElse(Nothing))
				End If
				If gradientNormalizationThreshold IsNot Nothing Then
					bl.setGradientNormalizationThreshold(gradientNormalizationThreshold)
				End If
				If updater IsNot Nothing Then
					bl.setIUpdater(updater)
				End If
				If biasUpdater IsNot Nothing Then
					bl.setBiasUpdater(biasUpdater)
				End If
				If weightNoise IsNot Nothing Then
					bl.setWeightNoise(weightNoise.orElse(Nothing))
				End If
			End If
			If miniBatch IsNot Nothing Then
				nnc.setMiniBatch(miniBatch)
			End If
			If maxNumLineSearchIterations IsNot Nothing Then
				nnc.setMaxNumLineSearchIterations(maxNumLineSearchIterations)
			End If
			If seed IsNot Nothing Then
				nnc.setSeed(seed)
			End If
			If optimizationAlgo <> Nothing Then
				nnc.setOptimizationAlgo(optimizationAlgo)
			End If
			If stepFunction IsNot Nothing Then
				nnc.setStepFunction(stepFunction)
			End If
			If minimize IsNot Nothing Then
				nnc.setMinimize(minimize)
			End If

			If convolutionMode <> Nothing AndAlso TypeOf l Is ConvolutionLayer Then
				DirectCast(l, ConvolutionLayer).setConvolutionMode(convolutionMode)
			End If
			If cudnnAlgoMode <> Nothing AndAlso TypeOf l Is ConvolutionLayer Then
				DirectCast(l, ConvolutionLayer).setCudnnAlgoMode(cudnnAlgoMode)
			End If
			If convolutionMode <> Nothing AndAlso TypeOf l Is SubsamplingLayer Then
				DirectCast(l, SubsamplingLayer).setConvolutionMode(convolutionMode)
			End If

			'Perform validation
			If l IsNot Nothing Then
				LayerValidation.generalValidation(l.LayerName, l, get(dropout), regularization, regularizationBias, get(constraints), Nothing, Nothing)
			End If
		End Sub

		Private Shared Function get(Of T)(ByVal [optional] As [Optional](Of T)) As T
			If [optional] Is Nothing Then
				Return Nothing
			End If
			Return [optional].orElse(Nothing)
		End Function

		Public Overridable Sub applyToMultiLayerConfiguration(ByVal conf As MultiLayerConfiguration)
			If backpropType <> Nothing Then
				conf.setBackpropType(backpropType)
			End If
			If tbpttFwdLength IsNot Nothing Then
				conf.setTbpttFwdLength(tbpttFwdLength)
			End If
			If tbpttBackLength IsNot Nothing Then
				conf.setTbpttBackLength(tbpttBackLength)
			End If
		End Sub

		Public Overridable Sub applyToComputationGraphConfiguration(ByVal conf As ComputationGraphConfiguration)
			If backpropType <> Nothing Then
				conf.setBackpropType(backpropType)
			End If
			If tbpttFwdLength IsNot Nothing Then
				conf.setTbpttFwdLength(tbpttFwdLength)
			End If
			If tbpttBackLength IsNot Nothing Then
				conf.setTbpttBackLength(tbpttBackLength)
			End If
		End Sub

		Public Overridable Function appliedNeuralNetConfigurationBuilder() As NeuralNetConfiguration.Builder
			Dim confBuilder As New NeuralNetConfiguration.Builder()
			If activationFn IsNot Nothing Then
				confBuilder.setActivationFn(activationFn)
			End If
			If weightInitFn IsNot Nothing Then
				confBuilder.setWeightInitFn(weightInitFn)
			End If
			If biasInit IsNot Nothing Then
				confBuilder.setBiasInit(biasInit)
			End If
			If regularization IsNot Nothing Then
				confBuilder.setRegularization(regularization)
			End If
			If regularizationBias IsNot Nothing Then
				confBuilder.setRegularizationBias(regularizationBias)
			End If
			If dropout IsNot Nothing Then
				confBuilder.setIdropOut(dropout.orElse(Nothing))
			End If
			If updater IsNot Nothing Then
				confBuilder.updater(updater)
			End If
			If biasUpdater IsNot Nothing Then
				confBuilder.biasUpdater(biasUpdater)
			End If
			If miniBatch IsNot Nothing Then
				confBuilder.setMiniBatch(miniBatch)
			End If
			If maxNumLineSearchIterations IsNot Nothing Then
				confBuilder.setMaxNumLineSearchIterations(maxNumLineSearchIterations)
			End If
			If seed IsNot Nothing Then
				confBuilder.setSeed(seed)
			End If
			If optimizationAlgo <> Nothing Then
				confBuilder.setOptimizationAlgo(optimizationAlgo)
			End If
			If stepFunction IsNot Nothing Then
				confBuilder.setStepFunction(stepFunction)
			End If
			If minimize IsNot Nothing Then
				confBuilder.setMinimize(minimize)
			End If
			If gradientNormalization IsNot Nothing Then
				confBuilder.setGradientNormalization(gradientNormalization.orElse(Nothing))
			End If
			If gradientNormalizationThreshold IsNot Nothing Then
				confBuilder.setGradientNormalizationThreshold(gradientNormalizationThreshold)
			End If
			If trainingWorkspaceMode <> Nothing Then
				confBuilder.trainingWorkspaceMode(trainingWorkspaceMode)
			End If
			If inferenceWorkspaceMode <> Nothing Then
				confBuilder.inferenceWorkspaceMode(inferenceWorkspaceMode)
			End If
			Return confBuilder
		End Function


		Public Overridable Function toJson() As String
			Try
				Return NeuralNetConfiguration.mapper().writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function toYaml() As String
			Try
				Return NeuralNetConfiguration.mapperYaml().writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromJson(ByVal json As String) As FineTuneConfiguration
			Try
				Return NeuralNetConfiguration.mapper().readValue(json, GetType(FineTuneConfiguration))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As FineTuneConfiguration
			Try
				Return NeuralNetConfiguration.mapperYaml().readValue(yaml, GetType(FineTuneConfiguration))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace