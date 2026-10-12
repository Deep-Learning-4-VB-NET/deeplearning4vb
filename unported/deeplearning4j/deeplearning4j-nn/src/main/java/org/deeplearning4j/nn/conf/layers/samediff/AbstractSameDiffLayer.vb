Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports SameDiffParamInitializer = org.deeplearning4j.nn.params.SameDiffParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.conf.layers.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data @EqualsAndHashCode(callSuper = true, doNotUseGetters = true) public abstract class AbstractSameDiffLayer extends org.deeplearning4j.nn.conf.layers.Layer
	<Serializable>
	Public MustInherit Class AbstractSameDiffLayer
		Inherits Layer

		Protected Friend regularization As IList(Of Regularization)
		Protected Friend regularizationBias As IList(Of Regularization)
		Protected Friend updater As IUpdater
		Protected Friend biasUpdater As IUpdater
		Protected Friend gradientNormalization As GradientNormalization
		Protected Friend gradientNormalizationThreshold As Double = Double.NaN

'JAVA TO VB CONVERTER NOTE: The field layerParams was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private layerParams_Conflict As SDLayerParams

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			If layerParams_Conflict.isWeightParam(paramName) Then
				Return regularization
			ElseIf layerParams_Conflict.isBiasParam(paramName) Then
				Return regularizationBias
			End If
			Return Nothing
		End Function

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.regularization = builder.regularization
			Me.regularizationBias = builder.regularizationBias
			Me.updater = builder.updater
			Me.biasUpdater = builder.biasUpdater

			'Check that this class has a no-arg constructor for JSON: better to detect this now provide useful information
			' to pre-empt a failure later for users, which will have a more difficult to understand message
			Try
				Me.GetType().getDeclaredConstructor()
			Catch e As NoSuchMethodException
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				log.warn("***SameDiff layer {} does not have a zero argument (no-arg) constructor.***" & vbLf & "A no-arg constructor " & "is required for JSON deserialization, which is used for both model saving and distributed (Spark) " & "training." & vbLf & "A no-arg constructor (private, protected or public) as well as setters (or simply a " & "Lombok @Data annotation) should be added to avoid JSON errors later.", Me.GetType().FullName)
			Catch e As SecurityException
				'Ignore
			End Try
		End Sub

		Protected Friend Sub New()
			'No op constructor for Jackson
		End Sub

		Public Overridable ReadOnly Property LayerParams As SDLayerParams
			Get
				If layerParams_Conflict Is Nothing Then
					layerParams_Conflict = New SDLayerParams()
					defineParameters(layerParams_Conflict)
				End If
				Return layerParams_Conflict
			End Get
		End Property

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'Default implementation: no-op
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			'Default implementation: no-op
			Return Nothing
		End Function


		Public Overridable Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)
			'Default implementation: no op
		End Sub

		''' <summary>
		''' Define the parameters for the network. Use <seealso cref="SDLayerParams.addWeightParam(String, Long...)"/> and {@link
		''' SDLayerParams#addBiasParam(String, long...)}
		''' </summary>
		''' <param name="params"> Object used to set parameters for this layer </param>
		Public MustOverride Sub defineParameters(ByVal params As SDLayerParams)

		''' <summary>
		''' Set the initial parameter values for this layer, if required
		''' </summary>
		''' <param name="params"> Parameter arrays that may be initialized </param>
		Public MustOverride Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))

		Public MustOverride Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

		'==================================================================================================================

		Public Overrides Function initializer() As ParamInitializer
			Return SameDiffParamInitializer.Instance
		End Function

		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			If biasUpdater IsNot Nothing AndAlso initializer().isBiasParam(Me, paramName) Then
				Return biasUpdater
			ElseIf initializer().isBiasParam(Me, paramName) OrElse initializer().isWeightParam(Me, paramName) Then
				Return updater
			End If
			Throw New System.InvalidOperationException("Unknown parameter key: " & paramName)
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return New LayerMemoryReport() 'TODO
		End Function

		''' <summary>
		''' Returns the memory layout ('c' or 'f' order - i.e., row/column major) of the parameters. In most cases, this
		''' can/should be left
		''' </summary>
		''' <param name="param"> Name of the parameter </param>
		''' <returns> Memory layout ('c' or 'f') of the parameter </returns>
		Public Overridable Function paramReshapeOrder(ByVal param As String) As Char
			Return "c"c
		End Function

		Protected Friend Overridable Sub initWeights(ByVal fanIn As Integer, ByVal fanOut As Integer, ByVal weightInit As WeightInit, ByVal array As INDArray)
			WeightInitUtil.initWeights(fanIn, fanOut, array.shape(), weightInit, Nothing, paramReshapeOrder(Nothing), array)
		End Sub

		Public Overridable Sub applyGlobalConfig(ByVal b As NeuralNetConfiguration.Builder)
			If regularization Is Nothing OrElse regularization.Count = 0 Then
				regularization = b.getRegularization()
			End If
			If regularizationBias Is Nothing OrElse regularizationBias.Count = 0 Then
				regularizationBias = b.getRegularizationBias()
			End If
			If updater Is Nothing Then
				updater = b.getIUpdater()
			End If
			If biasUpdater Is Nothing Then
				biasUpdater = b.getBiasUpdater()
			End If
			If gradientNormalization = Nothing Then
				gradientNormalization = b.getGradientNormalization()
			End If
			If Double.IsNaN(gradientNormalizationThreshold) Then
				gradientNormalizationThreshold = b.getGradientNormalizationThreshold()
			End If

			applyGlobalConfigToLayer(b)
		End Sub

		''' <summary>
		''' This method generates an "all ones" mask array for use in the SameDiff model when none is provided. </summary>
		''' <param name="input"> Input to the layer </param>
		''' <returns> A mask array - should be same datatype as the input (usually) </returns>
		Public Overridable Function onesMaskForInput(ByVal input As INDArray) As INDArray
			If input.rank() = 2 Then
				Return Nd4j.ones(input.dataType(), input.size(0), 1)
			ElseIf input.rank() = 3 Then
				Return Nd4j.ones(input.dataType(), input.size(0), input.size(2)) 'mask: [mb, length] vs. input [mb, nIn, length]
			ElseIf input.rank() = 4 Then
				'CNN style - return [mb, 1, 1, 1] for broadcast...
				Return Nd4j.ones(input.dataType(), input.size(0), 1, 1, 1)
			ElseIf input.rank() = 5 Then
				'CNN3D style - return [mb, 1, 1, 1, 1] for broadcast...
				Return Nd4j.ones(input.dataType(), input.size(0), 1, 1, 1, 1)
			Else
				Throw New System.InvalidOperationException("When using masking with rank 1 or 6+ inputs, the onesMaskForInput method must be implemented, " & "in order to determine the correct mask shape for this layer")
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends org.deeplearning4j.nn.conf.layers.Layer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits Layer.Builder(Of T)

'JAVA TO VB CONVERTER NOTE: The field regularization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularization_Conflict As IList(Of Regularization) = New List(Of Regularization)()
'JAVA TO VB CONVERTER NOTE: The field regularizationBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularizationBias_Conflict As IList(Of Regularization) = New List(Of Regularization)()

			''' <summary>
			''' Gradient updater. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/> or {@link
			''' org.nd4j.linalg.learning.config.Nesterovs}
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field updater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend updater_Conflict As IUpdater = Nothing

			''' <summary>
			''' Gradient updater configuration, for the biases only. If not set, biases will use the updater as set by {@link
			''' #updater(IUpdater)}
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field biasUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend biasUpdater_Conflict As IUpdater = Nothing

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
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(WeightDecay), "WeightDecay bias regularization removed: incompatible with added L2 regularization")
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
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(L2Regularization), "L2 bias regularization removed: incompatible with added WeightDecay regularization")
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
			''' Gradient updater. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/> or {@link
			''' org.nd4j.linalg.learning.config.Nesterovs}
			''' </summary>
			''' <param name="updater"> Updater to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function updater(ByVal updater_Conflict As IUpdater) As T
				Me.setUpdater(updater_Conflict)
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
		End Class
	End Class

End Namespace