Imports System
Imports System.Collections.Generic
Imports lombok
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
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


	''' <summary>
	''' A neural network layer.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @NoArgsConstructor public abstract class BaseLayer extends Layer implements java.io.Serializable, Cloneable
	<Serializable>
	Public MustInherit Class BaseLayer
		Inherits Layer
		Implements ICloneable

		Protected Friend activationFn As IActivation
		Protected Friend weightInitFn As IWeightInit
		Protected Friend biasInit As Double
		Protected Friend gainInit As Double
		Protected Friend regularization As IList(Of Regularization)
		Protected Friend regularizationBias As IList(Of Regularization)
		Protected Friend iUpdater As IUpdater
		Protected Friend biasUpdater As IUpdater
		Protected Friend weightNoise As IWeightNoise
'JAVA TO VB CONVERTER NOTE: The field gradientNormalization was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradientNormalization_Conflict As GradientNormalization = GradientNormalization.None 'Clipping, rescale based on l2 norm, etc
		Protected Friend gradientNormalizationThreshold As Double = 1.0 'Threshold for l2 and element-wise gradient clipping


		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.layerName = builder.layerName
			Me.activationFn = builder.activationFn
			Me.weightInitFn = builder.weightInitFn
			Me.biasInit = builder.biasInit
			Me.gainInit = builder.gainInit
			Me.regularization = builder.regularization
			Me.regularizationBias = builder.regularizationBias
			Me.iUpdater = builder.iupdater
			Me.biasUpdater = builder.biasUpdater
			Me.gradientNormalization_Conflict = builder.gradientNormalization
			Me.gradientNormalizationThreshold = builder.gradientNormalizationThreshold
			Me.weightNoise = builder.weightNoise
		End Sub

		''' <summary>
		''' Reset the learning related configs of the layer to default. When instantiated with a global neural network
		''' configuration the parameters specified in the neural network configuration will be used. For internal use with
		''' the transfer learning API. Users should not have to call this method directly.
		''' </summary>
		Public Overrides Sub resetLayerDefaultConfig()
			'clear the learning related params for all layers in the origConf and set to defaults
			Me.setIUpdater(Nothing)
			Me.setWeightInitFn(Nothing)
			Me.setBiasInit(Double.NaN)
			Me.setGainInit(Double.NaN)
			Me.regularization = Nothing
			Me.regularizationBias = Nothing
			Me.setGradientNormalization(GradientNormalization.None)
			Me.setGradientNormalizationThreshold(1.0)
			Me.iUpdater = Nothing
			Me.biasUpdater = Nothing
		End Sub

		Public Overrides Function clone() As BaseLayer
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim clone_Conflict As BaseLayer = CType(MyBase.clone(), BaseLayer)
			If clone_Conflict.iDropout IsNot Nothing Then
				clone_Conflict.iDropout = clone_Conflict.iDropout.clone()
			End If
			If regularization IsNot Nothing Then
				'Regularization fields are _usually_ thread safe and immutable, but let's clone to be sure
				clone_Conflict.regularization = New List(Of Regularization)(regularization.Count)
				For Each r As Regularization In regularization
					clone_Conflict.regularization.Add(r.clone())
				Next r
			End If
			If regularizationBias IsNot Nothing Then
				clone_Conflict.regularizationBias = New List(Of Regularization)(regularizationBias.Count)
				For Each r As Regularization In regularizationBias
					clone_Conflict.regularizationBias.Add(r.clone())
				Next r
			End If
			Return clone_Conflict
		End Function

		''' <summary>
		''' Get the updater for the given parameter. Typically the same updater will be used for all updaters, but this is
		''' not necessarily the case
		''' </summary>
		''' <param name="paramName"> Parameter name </param>
		''' <returns> IUpdater for the parameter </returns>
		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			If biasUpdater IsNot Nothing AndAlso initializer().isBiasParam(Me, paramName) Then
				Return biasUpdater
			End If
			Return iUpdater
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return gradientNormalization_Conflict
			End Get
		End Property

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			If initializer().isWeightParam(Me, paramName) Then
				Return regularization
			ElseIf initializer().isBiasParam(Me, paramName) Then
				Return regularizationBias
			End If
			Return Nothing
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @Getter @Setter public abstract static class Builder<T extends Builder<T>> extends Layer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits Layer.Builder(Of T)

			''' <summary>
			''' Set the activation function for the layer. This overload can be used for custom <seealso cref="IActivation"/>
			''' instances
			''' 
			''' </summary>
			Protected Friend activationFn As IActivation = Nothing

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' </summary>
			''' <seealso cref= IWeightInit </seealso>
			Protected Friend weightInitFn As IWeightInit = Nothing

			''' <summary>
			''' Bias initialization value, for layers with biases. Defaults to 0
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field biasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend biasInit_Conflict As Double = Double.NaN

			''' <summary>
			''' Gain initialization value, for layers with Layer Normalization. Defaults to 1
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field gainInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gainInit_Conflict As Double = Double.NaN

			''' <summary>
			''' Regularization for the parameters (excluding biases).
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field regularization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularization_Conflict As IList(Of Regularization) = New List(Of Regularization)()
			''' <summary>
			''' Regularization for the bias parameters only
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field regularizationBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularizationBias_Conflict As IList(Of Regularization) = New List(Of Regularization)()

			''' <summary>
			''' Gradient updater. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/> or {@link
			''' org.nd4j.linalg.learning.config.Nesterovs}
			''' 
			''' </summary>
			Protected Friend iupdater As IUpdater = Nothing

			''' <summary>
			''' Gradient updater configuration, for the biases only. If not set, biases will use the updater as set by {@link
			''' #updater(IUpdater)}
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field biasUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend biasUpdater_Conflict As IUpdater = Nothing

			''' <summary>
			''' Gradient normalization strategy. Used to specify gradient renormalization, gradient clipping etc.
			''' </summary>
			''' <seealso cref= GradientNormalization </seealso>
'JAVA TO VB CONVERTER NOTE: The field gradientNormalization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradientNormalization_Conflict As GradientNormalization = Nothing

			''' <summary>
			''' Threshold for gradient normalization, only used for GradientNormalization.ClipL2PerLayer,
			''' GradientNormalization.ClipL2PerParamType, and GradientNormalization.ClipElementWiseAbsoluteValue<br> Not used
			''' otherwise.<br> L2 threshold for first two types of clipping, or absolute value threshold for last type of
			''' clipping.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field gradientNormalizationThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradientNormalizationThreshold_Conflict As Double = Double.NaN

			''' <summary>
			''' Set the weight noise (such as <seealso cref="org.deeplearning4j.nn.conf.weightnoise.DropConnect"/> and {@link
			''' org.deeplearning4j.nn.conf.weightnoise.WeightNoise}) for this layer
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field weightNoise was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend weightNoise_Conflict As IWeightNoise

			''' <summary>
			''' Set the activation function for the layer. This overload can be used for custom <seealso cref="IActivation"/>
			''' instances
			''' </summary>
			''' <param name="activationFunction"> Activation function to use for the layer </param>
			Public Overridable Function activation(ByVal activationFunction As IActivation) As T
				Me.setActivationFn(activationFunction)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the activation function for the layer, from an <seealso cref="Activation"/> enumeration value.
			''' </summary>
			''' <param name="activation"> Activation function to use for the layer </param>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As T
				Return activation(activation_Conflict.getActivationFunction())
			End Function

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' </summary>
			''' <seealso cref= IWeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As IWeightInit) As T
				Me.setWeightInitFn(weightInit_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' </summary>
			''' <seealso cref= WeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As WeightInit) As T
				If weightInit_Conflict = WeightInit.DISTRIBUTION Then
					Throw New System.NotSupportedException("Not supported!, Use weightInit(Distribution distribution) instead!")
				End If

				Me.setWeightInitFn(weightInit_Conflict.getWeightInitFunction())
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set weight initialization scheme to random sampling via the specified distribution. Equivalent to: {@code
			''' .weightInit(new WeightInitDistribution(distribution))}
			''' </summary>
			''' <param name="distribution"> Distribution to use for weight initialization </param>
			Public Overridable Function weightInit(ByVal distribution As Distribution) As T
				Return weightInit(New WeightInitDistribution(distribution))
			End Function

			''' <summary>
			''' Bias initialization value, for layers with biases. Defaults to 0
			''' </summary>
			''' <param name="biasInit"> Value to use for initializing biases </param>
'JAVA TO VB CONVERTER NOTE: The parameter biasInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function biasInit(ByVal biasInit_Conflict As Double) As T
				Me.setBiasInit(biasInit_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Gain initialization value, for layers with Layer Normalization. Defaults to 1
			''' </summary>
			''' <param name="gainInit"> Value to use for initializing gain </param>
'JAVA TO VB CONVERTER NOTE: The parameter gainInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gainInit(ByVal gainInit_Conflict As Double) As T
				Me.gainInit_Conflict = gainInit_Conflict
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Distribution to sample initial weights from. Equivalent to: {@code .weightInit(new
			''' WeightInitDistribution(distribution))}
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter dist was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function dist(ByVal dist_Conflict As Distribution) As T
				Return weightInit(dist_Conflict)
			End Function

			''' <summary>
			''' L1 regularization coefficient (weights only). Use <seealso cref="l1Bias(Double)"/> to configure the l1 regularization
			''' coefficient for the bias.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1(ByVal l1_Conflict As Double) As T
				'Check if existing L1 exists; if so, replace it
				NetworkUtils.removeInstances(Me.regularization_Conflict, GetType(L1Regularization))
				If l1_Conflict > 0.0 Then
					Me.regularization_Conflict.Add(New L1Regularization(l1_Conflict))
				End If
				Return CType(Me, T)
			End Function

			''' <summary>
			''' L2 regularization coefficient (weights only). Use <seealso cref="l2Bias(Double)"/> to configure the l2 regularization
			''' coefficient for the bias.<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecay(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l2 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2(ByVal l2_Conflict As Double) As T
				'Check if existing L2 exists; if so, replace it. Also remove weight decay - it doesn't make sense to use both
				NetworkUtils.removeInstances(Me.regularization_Conflict, GetType(L2Regularization))
				If l2_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularization_Conflict, GetType(WeightDecay), "WeightDecay regularization removed: incompatible with added L2 regularization")
					Me.regularization_Conflict.Add(New L2Regularization(l2_Conflict))
				End If
				Return CType(Me, T)
			End Function

			''' <summary>
			''' L1 regularization coefficient for the bias. Default: 0. See also <seealso cref="l1(Double)"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1Bias(ByVal l1Bias_Conflict As Double) As T
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(L1Regularization))
				If l1Bias_Conflict > 0.0 Then
					Me.regularizationBias_Conflict.Add(New L1Regularization(l1Bias_Conflict))
				End If
				Return CType(Me, T)
			End Function

			''' <summary>
			''' L2 regularization coefficient for the bias. Default: 0. See also <seealso cref="l2(Double)"/><br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecayBias(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l2Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2Bias(ByVal l2Bias_Conflict As Double) As T
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(L2Regularization))
				If l2Bias_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(WeightDecay), "WeightDecay regularization removed: incompatible with added L2 regularization")
					Me.regularizationBias_Conflict.Add(New L2Regularization(l2Bias_Conflict))
				End If
				Return CType(Me, T)
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
				NetworkUtils.removeInstances(Me.regularization_Conflict, GetType(WeightDecay))
				If coefficient > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularization_Conflict, GetType(L2Regularization), "L2 regularization removed: incompatible with added WeightDecay regularization")
					Me.regularization_Conflict.Add(New WeightDecay(coefficient, applyLR))
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
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(WeightDecay))
				If coefficient > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(L2Regularization), "L2 regularization removed: incompatible with added WeightDecay regularization")
					Me.regularizationBias_Conflict.Add(New WeightDecay(coefficient, applyLR))
				End If
				Return Me
			End Function

			''' <summary>
			''' Set the regularization for the parameters (excluding biases) - for example <seealso cref="WeightDecay"/><br>
			''' </summary>
			''' <param name="regularization"> Regularization to apply for the network parameters/weights (excluding biases) </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularization(ByVal regularization_Conflict As IList(Of Regularization)) As Builder
				Me.setRegularization(regularization_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set the regularization for the biases only - for example <seealso cref="WeightDecay"/><br>
			''' </summary>
			''' <param name="regularizationBias"> Regularization to apply for the network biases only </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularizationBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularizationBias(ByVal regularizationBias_Conflict As IList(Of Regularization)) As Builder
				Me.setRegularizationBias(regularizationBias_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Gradient updater. For example, SGD for standard stochastic gradient descent, NESTEROV for Nesterov momentum,
			''' RSMPROP for RMSProp, etc.
			''' </summary>
			''' <seealso cref= Updater </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function updater(ByVal updater_Conflict As Updater) As T
				Return updater(updater_Conflict.getIUpdaterWithDefaultConfig())
			End Function

			''' <summary>
			''' Gradient updater. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/> or {@link
			''' org.nd4j.linalg.learning.config.Nesterovs}
			''' </summary>
			''' <param name="updater"> Updater to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function updater(ByVal updater_Conflict As IUpdater) As T
				Me.setIupdater(updater_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Gradient updater configuration, for the biases only. If not set, biases will use the updater as set by {@link
			''' #updater(IUpdater)}
			''' </summary>
			''' <param name="biasUpdater"> Updater to use for bias parameters </param>
'JAVA TO VB CONVERTER NOTE: The parameter biasUpdater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function biasUpdater(ByVal biasUpdater_Conflict As IUpdater) As T
				Me.setBiasUpdater(biasUpdater_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Gradient normalization strategy. Used to specify gradient renormalization, gradient clipping etc.
			''' </summary>
			''' <param name="gradientNormalization"> Type of normalization to use. Defaults to None. </param>
			''' <seealso cref= GradientNormalization </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter gradientNormalization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gradientNormalization(ByVal gradientNormalization_Conflict As GradientNormalization) As T
				Me.setGradientNormalization(gradientNormalization_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Threshold for gradient normalization, only used for GradientNormalization.ClipL2PerLayer,
			''' GradientNormalization.ClipL2PerParamType, and GradientNormalization.ClipElementWiseAbsoluteValue<br> Not used
			''' otherwise.<br> L2 threshold for first two types of clipping, or absolute value threshold for last type of
			''' clipping.
			''' </summary>
			Public Overridable Function gradientNormalizationThreshold(ByVal threshold As Double) As T
				Me.setGradientNormalizationThreshold(threshold)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set the weight noise (such as <seealso cref="org.deeplearning4j.nn.conf.weightnoise.DropConnect"/> and {@link
			''' org.deeplearning4j.nn.conf.weightnoise.WeightNoise}) for this layer
			''' </summary>
			''' <param name="weightNoise"> Weight noise instance to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter weightNoise was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightNoise(ByVal weightNoise_Conflict As IWeightNoise) As T
				Me.setWeightNoise(weightNoise_Conflict)
				Return CType(Me, T)
			End Function


		End Class
	End Class

End Namespace