Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports AbstractSameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.AbstractSameDiffLayer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports StepFunction = org.deeplearning4j.nn.conf.stepfunctions.StepFunction
Imports IWeightNoise = org.deeplearning4j.nn.conf.weightnoise.IWeightNoise
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports WeightDecay = org.nd4j.linalg.learning.regularization.WeightDecay
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.nn.conf



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @Slf4j @EqualsAndHashCode(exclude = {"iterationCount", "epochCount"}) public class NeuralNetConfiguration implements java.io.Serializable, Cloneable
	<Serializable>
	Public Class NeuralNetConfiguration
		Implements ICloneable

		Protected Friend layer As Layer
		'batch size: primarily used for conv nets. Will be reinforced if set.
		Protected Friend miniBatch As Boolean = True
		'number of line search iterations
		Protected Friend maxNumLineSearchIterations As Integer
		Protected Friend seed As Long
		Protected Friend optimizationAlgo As OptimizationAlgorithm
		'gradient keys used for ensuring order when getting and setting the gradient
'JAVA TO VB CONVERTER NOTE: The field variables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend variables_Conflict As IList(Of String) = New List(Of String)()
		'whether to constrain the gradient to unit norm or not
		Protected Friend stepFunction As StepFunction
		'minimize or maximize objective
		Protected Friend minimize As Boolean = True

		' this field defines preOutput cache
		Protected Friend cacheMode As CacheMode

		Protected Friend dataType As DataType = DataType.FLOAT 'Default to float for deserialization of legacy format nets

		'Counter for the number of parameter updates so far for this layer.
		'Note that this is only used for pretrain layers (AE, VAE) - MultiLayerConfiguration and ComputationGraphConfiguration
		'contain counters for standard backprop training.
		' This is important for learning rate schedules, for example, and is stored here to ensure it is persisted
		' for Spark and model serialization
		Protected Friend iterationCount As Integer = 0

		'Counter for the number of epochs completed so far. Used for per-epoch schedules
		Protected Friend epochCount As Integer = 0


		''' <summary>
		''' Creates and returns a deep copy of the configuration.
		''' </summary>
		Public Overrides Function clone() As NeuralNetConfiguration
			Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim clone_Conflict As NeuralNetConfiguration = CType(MyBase.clone(), NeuralNetConfiguration)
				If clone_Conflict.layer IsNot Nothing Then
					clone_Conflict.layer = clone_Conflict.layer.clone()
				End If
				If clone_Conflict.stepFunction IsNot Nothing Then
					clone_Conflict.stepFunction = clone_Conflict.stepFunction.clone()
				End If
				If clone_Conflict.variables_Conflict IsNot Nothing Then
					clone_Conflict.variables_Conflict = New List(Of String)(clone_Conflict.variables_Conflict)
				End If
				Return clone_Conflict
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function variables() As IList(Of String)
			Return New List(Of String)(variables_Conflict)
		End Function

		Public Overridable Function variables(ByVal copy As Boolean) As IList(Of String)
			If copy Then
				Return variables()
			End If
			Return variables_Conflict
		End Function

		Public Overridable Sub addVariable(ByVal variable As String)
			If Not variables_Conflict.Contains(variable) Then
				variables_Conflict.Add(variable)
			End If
		End Sub

		Public Overridable Sub clearVariables()
			variables_Conflict.Clear()
		End Sub

		''' <summary>
		''' Fluent interface for building a list of configurations
		''' </summary>
		Public Class ListBuilder
			Inherits MultiLayerConfiguration.Builder

			Friend layerCounter As Integer = -1 'Used only for .layer(Layer) method
			Friend layerwise As IDictionary(Of Integer, Builder)
			Friend globalConfig As Builder

			' Constructor
			Public Sub New(ByVal globalConfig As Builder, ByVal layerMap As IDictionary(Of Integer, Builder))
				Me.globalConfig = globalConfig
				Me.layerwise = layerMap
			End Sub

			Public Sub New(ByVal globalConfig As Builder)
				Me.New(globalConfig, New Dictionary(Of Integer, Builder)())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ListBuilder layer(int ind, @NonNull Layer layer)
'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal ind As Integer, ByVal layer_Conflict As Layer) As ListBuilder
				If layerwise.ContainsKey(ind) Then
					log.info("Layer index {} already exists, layer of type {} will be replace by layer type {}", ind, layerwise(ind).GetType().Name, layer_Conflict.GetType().Name)
					layerwise(ind).layer(layer_Conflict)
				Else
					layerwise(ind) = globalConfig.clone().layer(layer_Conflict)
				End If
				If layerCounter < ind Then
					'Edge case: user is mixing .layer(Layer) and .layer(int, Layer) calls
					'This should allow a .layer(A, X) and .layer(Y) to work such that layer Y is index (A+1)
					layerCounter = ind
				End If
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layer_Conflict As Layer) As ListBuilder
				layerCounter += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return layer(++layerCounter, layer);
				Return Me.layer(layerCounter, layer_Conflict)
			End Function

			Public Overridable ReadOnly Property Layerwise As IDictionary(Of Integer, Builder)
				Get
					Return layerwise
				End Get
			End Property

'JAVA TO VB CONVERTER NOTE: The parameter overrideNinUponBuild was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function overrideNinUponBuild(ByVal overrideNinUponBuild_Conflict As Boolean) As ListBuilder
				MyBase.overrideNinUponBuild(overrideNinUponBuild_Conflict)
				Return Me
			End Function

			Public Overrides Function inputPreProcessor(ByVal layer As Integer?, ByVal processor As InputPreProcessor) As ListBuilder
				MyBase.inputPreProcessor(layer, processor)
				Return Me
			End Function

			Public Overrides Function inputPreProcessors(ByVal processors As IDictionary(Of Integer, InputPreProcessor)) As ListBuilder
				MyBase.inputPreProcessors(processors)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public ListBuilder cacheMode(@NonNull CacheMode cacheMode)
'JAVA TO VB CONVERTER NOTE: The parameter cacheMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function cacheMode(ByVal cacheMode_Conflict As CacheMode) As ListBuilder
				MyBase.cacheMode(cacheMode_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public MultiLayerConfiguration.Builder backpropType(@NonNull BackpropType type)
			Public Overrides Function backpropType(ByVal type As BackpropType) As MultiLayerConfiguration.Builder
				MyBase.backpropType(type)
				Return Me
			End Function

			Public Overrides Function tBPTTLength(ByVal bpttLength As Integer) As ListBuilder
				MyBase.tBPTTLength(bpttLength)
				Return Me
			End Function

			Public Overrides Function tBPTTForwardLength(ByVal forwardLength As Integer) As ListBuilder
				MyBase.tBPTTForwardLength(forwardLength)
				Return Me
			End Function

			Public Overrides Function tBPTTBackwardLength(ByVal backwardLength As Integer) As ListBuilder
				 MyBase.tBPTTBackwardLength(backwardLength)
				 Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter confs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function confs(ByVal confs_Conflict As IList(Of NeuralNetConfiguration)) As ListBuilder
				 MyBase.confs(confs_Conflict)
				 Return Me
			End Function

			Public Overrides Function validateOutputLayerConfig(ByVal validate As Boolean) As ListBuilder
				 MyBase.validateOutputLayerConfig(validate)
				 Return Me
			End Function

			Public Overrides Function validateTbpttConfig(ByVal validate As Boolean) As ListBuilder
				MyBase.validateTbpttConfig(validate)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public ListBuilder dataType(@NonNull DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter dataType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function dataType(ByVal dataType_Conflict As DataType) As ListBuilder
				 MyBase.dataType(dataType_Conflict)
				 Return Me
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void finalize() throws Throwable
			Protected Overrides Sub Finalize()
				MyBase.Finalize()
			End Sub

			Public Overrides Function setInputType(ByVal inputType As InputType) As ListBuilder
				Return CType(MyBase.setInputType(inputType), ListBuilder)
			End Function

			''' <summary>
			''' A convenience method for setting input types: note that for example .inputType().convolutional(h,w,d)
			''' is equivalent to .setInputType(InputType.convolutional(h,w,d))
			''' </summary>
			Public Overridable Function inputType() As InputTypeBuilder
				Return New InputTypeBuilder(Me)
			End Function

			''' <summary>
			''' For the (perhaps partially constructed) network configuration, return a list of activation sizes for each
			''' layer in the network.<br>
			''' Note: To use this method, the network input type must have been set using <seealso cref="setInputType(InputType)"/> first </summary>
			''' <returns> A list of activation types for the network, indexed by layer number </returns>
			Public Overridable ReadOnly Property LayerActivationTypes As IList(Of InputType)
				Get
					Preconditions.checkState(inputType_Conflict IsNot Nothing, "Can only calculate activation types if input type has" & "been set. Use setInputType(InputType)")
    
					Dim conf As MultiLayerConfiguration
					Try
						conf = build()
					Catch e As Exception
						Throw New Exception("Error calculating layer activation types: error instantiating MultiLayerConfiguration", e)
					End Try
    
					Return conf.getLayerActivationTypes(inputType_Conflict)
				End Get
			End Property

			''' <summary>
			''' Build the multi layer network
			''' based on this neural network and
			''' overr ridden parameters
			''' </summary>
			''' <returns> the configuration to build </returns>
			Public Overrides Function build() As MultiLayerConfiguration
				Dim list As IList(Of NeuralNetConfiguration) = New List(Of NeuralNetConfiguration)()
				If layerwise.Count = 0 Then
					Throw New System.InvalidOperationException("Invalid configuration: no layers defined")
				End If
				Dim i As Integer = 0
				Do While i < layerwise.Count
					If layerwise(i) Is Nothing Then
						Throw New System.InvalidOperationException("Invalid configuration: layer number " & i & " not specified. Expect layer " & "numbers to be 0 to " & (layerwise.Count - 1) & " inclusive (number of layers defined: " & layerwise.Count & ")")
					End If
					If layerwise(i).getLayer() Is Nothing Then
						Throw New System.InvalidOperationException("Cannot construct network: Layer config for" & "layer with index " & i & " is not defined)")
					End If

					'Layer names: set to default, if not set
					If layerwise(i).getLayer().getLayerName() Is Nothing Then
						layerwise(i).getLayer().setLayerName("layer" & i)
					End If

					list.Add(layerwise(i).build())
					i += 1
				Loop

				Dim wsmTrain As WorkspaceMode = (If(globalConfig.setTWM, globalConfig.trainingWorkspaceMode_Conflict, trainingWorkspaceMode_Conflict))
				Dim wsmTest As WorkspaceMode = (If(globalConfig.setIWM, globalConfig.inferenceWorkspaceMode_Conflict, inferenceWorkspaceMode_Conflict))


				Return (New MultiLayerConfiguration.Builder()).inputPreProcessors(inputPreProcessors_Conflict).backpropType(backpropType_Conflict).tBPTTForwardLength(tbpttFwdLength).tBPTTBackwardLength(tbpttBackLength).setInputType(Me.inputType_Conflict).trainingWorkspaceMode(wsmTrain).cacheMode(globalConfig.cacheMode_Conflict).inferenceWorkspaceMode(wsmTest).confs(list).validateOutputLayerConfig(validateOutputConfig).dataType(globalConfig.dataType_Conflict).build()
			End Function

			''' <summary>
			''' Helper class for setting input types </summary>
			Public Class InputTypeBuilder
				Private ReadOnly outerInstance As NeuralNetConfiguration.ListBuilder

				Public Sub New(ByVal outerInstance As NeuralNetConfiguration.ListBuilder)
					Me.outerInstance = outerInstance
				End Sub

				''' <summary>
				''' See <seealso cref="InputType.convolutional(Long, Long, Long)"/>
				''' </summary>
				Public Overridable Function convolutional(ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer) As ListBuilder
					Return outerInstance.setInputType(InputType.convolutional(height, width, depth))
				End Function

				''' <summary>
				''' * See <seealso cref="InputType.convolutionalFlat(Long, Long, Long)"/>
				''' </summary>
				Public Overridable Function convolutionalFlat(ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer) As ListBuilder
					Return outerInstance.setInputType(InputType.convolutionalFlat(height, width, depth))
				End Function

				''' <summary>
				''' See <seealso cref="InputType.feedForward(Long)"/>
				''' </summary>
				Public Overridable Function feedForward(ByVal size As Integer) As ListBuilder
					Return outerInstance.setInputType(InputType.feedForward(size))
				End Function

				''' <summary>
				''' See <seealso cref="InputType.recurrent(Long)"/>}
				''' </summary>
				Public Overridable Function recurrent(ByVal size As Integer) As ListBuilder
					Return outerInstance.setInputType(InputType.recurrent(size))
				End Function
			End Class
		End Class

		''' <summary>
		''' Return this configuration as json
		''' </summary>
		''' <returns> this configuration represented as json </returns>
		Public Overridable Function toYaml() As String
			Dim mapper As ObjectMapper = mapperYaml()

			Try
				Dim ret As String = mapper.writeValueAsString(Me)
				Return ret

			Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Create a neural net configuration from json
		''' </summary>
		''' <param name="json"> the neural net configuration from json
		''' @return </param>
		Public Shared Function fromYaml(ByVal json As String) As NeuralNetConfiguration
			Dim mapper As ObjectMapper = mapperYaml()
			Try
				Dim ret As NeuralNetConfiguration = mapper.readValue(json, GetType(NeuralNetConfiguration))
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Return this configuration as json
		''' </summary>
		''' <returns> this configuration represented as json </returns>
		Public Overridable Function toJson() As String
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapper()

			Try
				Return mapper.writeValueAsString(Me)
			Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Create a neural net configuration from json
		''' </summary>
		''' <param name="json"> the neural net configuration from json
		''' @return </param>
		Public Shared Function fromJson(ByVal json As String) As NeuralNetConfiguration
			Dim mapper As ObjectMapper = NeuralNetConfiguration.mapper()
			Try
				Dim ret As NeuralNetConfiguration = mapper.readValue(json, GetType(NeuralNetConfiguration))
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Object mapper for serialization of configurations
		''' 
		''' @return
		''' </summary>
		Public Shared Function mapperYaml() As ObjectMapper
			Return JsonMappers.MapperYaml
		End Function

		''' <summary>
		''' Object mapper for serialization of configurations
		''' 
		''' @return
		''' </summary>
		Public Shared Function mapper() As ObjectMapper
			Return JsonMappers.Mapper
		End Function

		''' <summary>
		''' NeuralNetConfiguration builder, used as a starting point for creating a MultiLayerConfiguration or
		''' ComputationGraphConfiguration.<br>
		''' Note that values set here on the layer will be applied to all relevant layers - unless the value is overridden
		''' on a layer's configuration
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class Builder implements Cloneable
		Public Class Builder
			Implements ICloneable

			Protected Friend activationFn As IActivation = New ActivationSigmoid()
			Protected Friend weightInitFn As IWeightInit = New WeightInitXavier()
'JAVA TO VB CONVERTER NOTE: The field biasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend biasInit_Conflict As Double = 0.0
			Protected Friend gainInit As Double = 1.0
'JAVA TO VB CONVERTER NOTE: The field regularization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularization_Conflict As IList(Of Regularization) = New List(Of Regularization)()
'JAVA TO VB CONVERTER NOTE: The field regularizationBias was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend regularizationBias_Conflict As IList(Of Regularization) = New List(Of Regularization)()
			Protected Friend idropOut As IDropout
'JAVA TO VB CONVERTER NOTE: The field weightNoise was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend weightNoise_Conflict As IWeightNoise
			Protected Friend iUpdater As IUpdater = New Sgd()
'JAVA TO VB CONVERTER NOTE: The field biasUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend biasUpdater_Conflict As IUpdater = Nothing
'JAVA TO VB CONVERTER NOTE: The field layer was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend layer_Conflict As Layer
'JAVA TO VB CONVERTER NOTE: The field miniBatch was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend miniBatch_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field maxNumLineSearchIterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend maxNumLineSearchIterations_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long = DateTimeHelper.CurrentUnixTimeMillis()
'JAVA TO VB CONVERTER NOTE: The field optimizationAlgo was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend optimizationAlgo_Conflict As OptimizationAlgorithm = OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT
'JAVA TO VB CONVERTER NOTE: The field stepFunction was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend stepFunction_Conflict As StepFunction = Nothing
'JAVA TO VB CONVERTER NOTE: The field minimize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minimize_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field gradientNormalization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradientNormalization_Conflict As GradientNormalization = GradientNormalization.None
'JAVA TO VB CONVERTER NOTE: The field gradientNormalizationThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gradientNormalizationThreshold_Conflict As Double = 1.0
			Protected Friend allParamConstraints As IList(Of LayerConstraint)
			Protected Friend weightConstraints As IList(Of LayerConstraint)
			Protected Friend biasConstraints As IList(Of LayerConstraint)

'JAVA TO VB CONVERTER NOTE: The field trainingWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend trainingWorkspaceMode_Conflict As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER NOTE: The field inferenceWorkspaceMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend inferenceWorkspaceMode_Conflict As WorkspaceMode = WorkspaceMode.ENABLED
			Protected Friend setTWM As Boolean = False
			Protected Friend setIWM As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field cacheMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cacheMode_Conflict As CacheMode = CacheMode.NONE
'JAVA TO VB CONVERTER NOTE: The field dataType was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend dataType_Conflict As DataType = DataType.FLOAT

'JAVA TO VB CONVERTER NOTE: The field convolutionMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend convolutionMode_Conflict As ConvolutionMode = ConvolutionMode.Truncate
'JAVA TO VB CONVERTER NOTE: The field cudnnAlgoMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend cudnnAlgoMode_Conflict As ConvolutionLayer.AlgoMode = ConvolutionLayer.AlgoMode.PREFER_FASTEST

			Public Sub New()
				'
			End Sub

			Public Sub New(ByVal newConf As NeuralNetConfiguration)
				If newConf IsNot Nothing Then
					minimize_Conflict = newConf.minimize
					maxNumLineSearchIterations_Conflict = newConf.maxNumLineSearchIterations
					layer_Conflict = newConf.layer
					optimizationAlgo_Conflict = newConf.optimizationAlgo
					seed_Conflict = newConf.seed
					stepFunction_Conflict = newConf.stepFunction
					miniBatch_Conflict = newConf.miniBatch
				End If
			End Sub

			''' <summary>
			''' Process input as minibatch vs full dataset.
			''' Default set to true.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter miniBatch was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function miniBatch(ByVal miniBatch_Conflict As Boolean) As Builder
				Me.miniBatch_Conflict = miniBatch_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines Workspace mode being used during training:<br>
			''' NONE: workspace won't be used<br>
			''' ENABLED: workspaces will be used for training (reduced memory and better performance)
			''' </summary>
			''' <param name="workspaceMode"> Workspace mode for training </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder trainingWorkspaceMode(@NonNull WorkspaceMode workspaceMode)
			Public Overridable Function trainingWorkspaceMode(ByVal workspaceMode As WorkspaceMode) As Builder
				Me.trainingWorkspaceMode_Conflict = workspaceMode
				Me.setTWM = True
				Return Me
			End Function

			''' <summary>
			''' This method defines Workspace mode being used during inference:<br>
			''' NONE: workspace won't be used<br>
			''' ENABLED: workspaces will be used for inference (reduced memory and better performance)
			''' </summary>
			''' <param name="workspaceMode"> Workspace mode for inference </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder inferenceWorkspaceMode(@NonNull WorkspaceMode workspaceMode)
			Public Overridable Function inferenceWorkspaceMode(ByVal workspaceMode As WorkspaceMode) As Builder
				Me.inferenceWorkspaceMode_Conflict = workspaceMode
				Me.setIWM = True
				Return Me
			End Function

			''' <summary>
			''' This method defines how/if preOutput cache is handled:
			''' NONE: cache disabled (default value)
			''' HOST: Host memory will be used
			''' DEVICE: GPU memory will be used (on CPU backends effect will be the same as for HOST)
			''' </summary>
			''' <param name="cacheMode"> Cache mode to use </param>
			''' <returns> Builder </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder cacheMode(@NonNull CacheMode cacheMode)
'JAVA TO VB CONVERTER NOTE: The parameter cacheMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function cacheMode(ByVal cacheMode_Conflict As CacheMode) As Builder
				Me.cacheMode_Conflict = cacheMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Objective function to minimize or maximize cost function
			''' Default set to minimize true.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter minimize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minimize(ByVal minimize_Conflict As Boolean) As Builder
				Me.minimize_Conflict = minimize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Maximum number of line search iterations.
			''' Only applies for line search optimizers: Line Search SGD, Conjugate Gradient, LBFGS
			''' is NOT applicable for standard SGD
			''' </summary>
			''' <param name="maxNumLineSearchIterations"> > 0
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter maxNumLineSearchIterations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxNumLineSearchIterations(ByVal maxNumLineSearchIterations_Conflict As Integer) As Builder
				Me.maxNumLineSearchIterations_Conflict = maxNumLineSearchIterations_Conflict
				Return Me
			End Function


			''' <summary>
			''' Layer class.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layer_Conflict As Layer) As Builder
				Me.layer_Conflict = layer_Conflict
				Return Me
			End Function

			''' <summary>
			''' Step function to apply for back track line search.
			''' Only applies for line search optimizers: Line Search SGD, Conjugate Gradient, LBFGS
			''' Options: DefaultStepFunction (default), NegativeDefaultStepFunction
			''' GradientStepFunction (for SGD), NegativeGradientStepFunction
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter stepFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function stepFunction(ByVal stepFunction_Conflict As StepFunction) As Builder
				Me.stepFunction_Conflict = stepFunction_Conflict
				Return Me
			End Function

			''' <summary>
			''' Create a ListBuilder (for creating a MultiLayerConfiguration)<br>
			''' Usage:<br>
			''' <pre>
			''' {@code .list()
			''' .layer(new DenseLayer.Builder()...build())
			''' ...
			''' .layer(new OutputLayer.Builder()...build())
			''' }
			''' </pre>
			''' </summary>
			Public Overridable Function list() As ListBuilder
				Return New ListBuilder(Me)
			End Function

			''' <summary>
			''' Create a ListBuilder (for creating a MultiLayerConfiguration) with the specified layers<br>
			''' Usage:<br>
			''' <pre>
			''' {@code .list(
			'''      new DenseLayer.Builder()...build(),
			'''      ...,
			'''      new OutputLayer.Builder()...build())
			''' }
			''' </pre>
			''' </summary>
			''' <param name="layers"> The layer configurations for the network </param>
			Public Overridable Function list(ParamArray ByVal layers() As Layer) As ListBuilder
				If layers Is Nothing OrElse layers.Length = 0 Then
					Throw New System.ArgumentException("Cannot create network with no layers")
				End If
				Dim layerMap As IDictionary(Of Integer, Builder) = New Dictionary(Of Integer, Builder)()
				For i As Integer = 0 To layers.Length - 1
					Dim b As Builder = Me.clone()
					b.layer(layers(i))
					layerMap(i) = b
				Next i
				Return New ListBuilder(Me, layerMap)

			End Function

			''' <summary>
			''' Create a GraphBuilder (for creating a ComputationGraphConfiguration).
			''' </summary>
			Public Overridable Function graphBuilder() As ComputationGraphConfiguration.GraphBuilder
				Return New ComputationGraphConfiguration.GraphBuilder(Me)
			End Function

			''' <summary>
			''' Random number generator seed. Used for reproducability between runs
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Long) As Builder
				Me.seed_Conflict = seed_Conflict
				Nd4j.Random.Seed = seed_Conflict
				Return Me
			End Function

			''' <summary>
			''' Optimization algorithm to use. Most common: OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT
			''' </summary>
			''' <param name="optimizationAlgo"> Optimization algorithm to use when training </param>
'JAVA TO VB CONVERTER NOTE: The parameter optimizationAlgo was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function optimizationAlgo(ByVal optimizationAlgo_Conflict As OptimizationAlgorithm) As Builder
				Me.optimizationAlgo_Conflict = optimizationAlgo_Conflict
				Return Me
			End Function

			Public Overrides Function clone() As Builder
				Try
'JAVA TO VB CONVERTER NOTE: The local variable clone was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
					Dim clone_Conflict As Builder = CType(MyBase.clone(), Builder)
					If clone_Conflict.layer_Conflict IsNot Nothing Then
						clone_Conflict.layer_Conflict = clone_Conflict.layer_Conflict.clone()
					End If
					If clone_Conflict.stepFunction_Conflict IsNot Nothing Then
						clone_Conflict.stepFunction_Conflict = clone_Conflict.stepFunction_Conflict.clone()
					End If

					Return clone_Conflict

				Catch e As CloneNotSupportedException
					Throw New Exception(e)
				End Try
			End Function

			''' <summary>
			''' Activation function / neuron non-linearity<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <seealso cref= #activation(Activation) </seealso>
			Public Overridable Function activation(ByVal activationFunction As IActivation) As Builder
				Me.activationFn = activationFunction
				Return Me
			End Function

			''' <summary>
			''' Activation function / neuron non-linearity<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter activation was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function activation(ByVal activation_Conflict As Activation) As Builder
				Return activation(activation_Conflict.getActivationFunction())
			End Function


			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <seealso cref= IWeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As IWeightInit) As Builder
				Me.weightInitFn = weightInit_Conflict
				Return Me
			End Function

			''' <summary>
			''' Weight initialization scheme to use, for initial weight values
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <seealso cref= WeightInit </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter weightInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightInit(ByVal weightInit_Conflict As WeightInit) As Builder
				If weightInit_Conflict = WeightInit.DISTRIBUTION Then
					'   throw new UnsupportedOperationException("Not supported!, Use weightInit(Distribution distribution) instead!");
				End If

				Me.weightInitFn = weightInit_Conflict.getWeightInitFunction()
				Return Me
			End Function

			''' <summary>
			''' Set weight initialization scheme to random sampling via the specified distribution.
			''' Equivalent to: {@code .weightInit(new WeightInitDistribution(distribution))}
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="distribution"> Distribution to use for weight initialization </param>
			Public Overridable Function weightInit(ByVal distribution As Distribution) As Builder
				Return weightInit(New WeightInitDistribution(distribution))
			End Function

			''' <summary>
			''' Constant for bias initialization. Default: 0.0<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="biasInit"> Constant for bias initialization </param>
'JAVA TO VB CONVERTER NOTE: The parameter biasInit was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function biasInit(ByVal biasInit_Conflict As Double) As Builder
				Me.biasInit_Conflict = biasInit_Conflict
				Return Me
			End Function

			''' <summary>
			''' Distribution to sample initial weights from.
			''' Equivalent to: {@code .weightInit(new WeightInitDistribution(distribution))}.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <seealso cref= #weightInit(Distribution) </seealso>
			''' @deprecated Use <seealso cref="weightInit(Distribution)"/> 
'JAVA TO VB CONVERTER NOTE: The parameter dist was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""weightInit(Distribution)""/>")>
			Public Overridable Function dist(ByVal dist_Conflict As Distribution) As Builder
				Return weightInit(dist_Conflict)
			End Function

			''' <summary>
			''' L1 regularization coefficient for the weights (excluding biases).<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1(ByVal l1_Conflict As Double) As Builder
				'Check if existing L1 exists; if so, replace it
				NetworkUtils.removeInstances(Me.regularization_Conflict, GetType(L1Regularization))
				If l1_Conflict > 0.0 Then
					Me.regularization_Conflict.Add(New L1Regularization(l1_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' L2 regularization coefficient for the weights (excluding biases).<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecay(Double)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' Note: L2 regularization and weight decay usually should not be used together; if any weight decay (or L2) has
			''' been added for the biases, these will be removed first.
			''' </summary>
			''' <seealso cref= #weightDecay(double, boolean) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter l2 was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2(ByVal l2_Conflict As Double) As Builder
				'Check if existing L2 exists; if so, replace it. Also remove weight decay - it doesn't make sense to use both
				NetworkUtils.removeInstances(Me.regularization_Conflict, GetType(L2Regularization))
				If l2_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularization_Conflict, GetType(WeightDecay), "WeightDecay regularization removed: incompatible with added L2 regularization")
					Me.regularization_Conflict.Add(New L2Regularization(l2_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' L1 regularization coefficient for the bias.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter l1Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l1Bias(ByVal l1Bias_Conflict As Double) As Builder
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(L1Regularization))
				If l1Bias_Conflict > 0.0 Then
					Me.regularizationBias_Conflict.Add(New L1Regularization(l1Bias_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' L2 regularization coefficient for the bias.<br>
			''' <b>Note</b>: Generally, <seealso cref="WeightDecay"/> (set via <seealso cref="weightDecayBias(Double,Boolean)"/> should be preferred to
			''' L2 regularization. See <seealso cref="WeightDecay"/> javadoc for further details.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' Note: L2 regularization and weight decay usually should not be used together; if any weight decay (or L2) has
			''' been added for the biases, these will be removed first.
			''' </summary>
			''' <seealso cref= #weightDecayBias(double, boolean) </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter l2Bias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function l2Bias(ByVal l2Bias_Conflict As Double) As Builder
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(L2Regularization))
				If l2Bias_Conflict > 0.0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(WeightDecay), "L2 bias regularization removed: incompatible with added WeightDecay regularization")
					Me.regularizationBias_Conflict.Add(New L2Regularization(l2Bias_Conflict))
				End If
				Return Me
			End Function

			''' <summary>
			''' Add weight decay regularization for the network parameters (excluding biases).<br>
			''' This applies weight decay <i>with</i> multiplying the learning rate - see <seealso cref="WeightDecay"/> for more details.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <seealso cref= #weightDecay(double, boolean) </seealso>
			Public Overridable Function weightDecay(ByVal coefficient As Double) As Builder
				Return weightDecay(coefficient, True)
			End Function

			''' <summary>
			''' Add weight decay regularization for the network parameters (excluding biases). See <seealso cref="WeightDecay"/> for more details.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
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
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			''' <seealso cref= #weightDecayBias(double, boolean) </seealso>
			Public Overridable Function weightDecayBias(ByVal coefficient As Double) As Builder
				Return weightDecayBias(coefficient, True)
			End Function

			''' <summary>
			''' Weight decay for the biases only - see <seealso cref="weightDecay(Double)"/> for more details<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="coefficient"> Weight decay regularization coefficient </param>
			Public Overridable Function weightDecayBias(ByVal coefficient As Double, ByVal applyLR As Boolean) As Builder
				'Check if existing weight decay if it exists; if so, replace it. Also remove L2 - it doesn't make sense to use both
				NetworkUtils.removeInstances(Me.regularizationBias_Conflict, GetType(WeightDecay))
				If coefficient > 0 Then
					NetworkUtils.removeInstancesWithWarning(Me.regularizationBias_Conflict, GetType(L2Regularization), "L2 bias regularization removed: incompatible with added WeightDecay regularization")
					Me.regularizationBias_Conflict.Add(New WeightDecay(coefficient, applyLR))
				End If
				Return Me
			End Function

			''' <summary>
			''' Set the regularization for the parameters (excluding biases) - for example <seealso cref="WeightDecay"/><br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="regularization"> Regularization to apply for the network parameters/weights (excluding biases) </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularization(ByVal regularization_Conflict As IList(Of Regularization)) As Builder
				Me.regularization_Conflict = regularization_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the regularization for the biases only - for example <seealso cref="WeightDecay"/><br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.<br>
			''' </summary>
			''' <param name="regularizationBias"> Regularization to apply for the network biases only </param>
'JAVA TO VB CONVERTER NOTE: The parameter regularizationBias was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function regularizationBias(ByVal regularizationBias_Conflict As IList(Of Regularization)) As Builder
				Me.regularizationBias_Conflict = regularizationBias_Conflict
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
			''' <br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="inputRetainProbability"> Dropout probability (probability of retaining each input activation value for a layer) </param>
			''' <seealso cref= #dropOut(IDropout) </seealso>
			Public Overridable Function dropOut(ByVal inputRetainProbability As Double) As Builder
				If inputRetainProbability = 0.0 Then
					Return dropOut(Nothing)
				End If
				Return dropOut(New Dropout(inputRetainProbability))
			End Function

			''' <summary>
			''' Set the dropout for all layers in this network<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="dropout"> Dropout, such as <seealso cref="Dropout"/>, <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianDropout"/>,
			'''                <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianNoise"/> etc
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter dropout was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dropOut(ByVal dropout_Conflict As IDropout) As Builder
				'Clone: Dropout is stateful usually - don't want to have the same instance shared in multiple places
				Me.idropOut = (If(dropout_Conflict Is Nothing, Nothing, dropout_Conflict.clone()))
				Return Me
			End Function

			''' <summary>
			''' Set the weight noise (such as <seealso cref="org.deeplearning4j.nn.conf.weightnoise.DropConnect"/> and
			''' <seealso cref="org.deeplearning4j.nn.conf.weightnoise.WeightNoise"/>) for the layers in this network.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="weightNoise"> Weight noise instance to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter weightNoise was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function weightNoise(ByVal weightNoise_Conflict As IWeightNoise) As Builder
				Me.weightNoise_Conflict = weightNoise_Conflict
				Return Me
			End Function


			''' @deprecated Use <seealso cref="updater(IUpdater)"/> 
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""updater(IUpdater)""/>")>
			Public Overridable Function updater(ByVal updater_Conflict As Updater) As Builder
				Return updater(updater_Conflict.getIUpdaterWithDefaultConfig())
			End Function

			''' <summary>
			''' Gradient updater configuration. For example, <seealso cref="org.nd4j.linalg.learning.config.Adam"/>
			''' or <seealso cref="org.nd4j.linalg.learning.config.Nesterovs"/><br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="updater"> Updater to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter updater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function updater(ByVal updater_Conflict As IUpdater) As Builder
				Me.iUpdater = updater_Conflict
				Return Me
			End Function

			''' <summary>
			''' Gradient updater configuration, for the biases only. If not set, biases will use the updater as
			''' set by <seealso cref="updater(IUpdater)"/><br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="updater"> Updater to use for bias parameters </param>
			Public Overridable Function biasUpdater(ByVal updater As IUpdater) As Builder
				Me.biasUpdater_Conflict = updater
				Return Me
			End Function

			''' <summary>
			''' Gradient normalization strategy. Used to specify gradient renormalization, gradient clipping etc.
			''' See <seealso cref="GradientNormalization"/> for details<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="gradientNormalization"> Type of normalization to use. Defaults to None. </param>
			''' <seealso cref= GradientNormalization </seealso>
'JAVA TO VB CONVERTER NOTE: The parameter gradientNormalization was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gradientNormalization(ByVal gradientNormalization_Conflict As GradientNormalization) As Builder
				Me.gradientNormalization_Conflict = gradientNormalization_Conflict
				Return Me
			End Function

			''' <summary>
			''' Threshold for gradient normalization, only used for GradientNormalization.ClipL2PerLayer,
			''' GradientNormalization.ClipL2PerParamType, and GradientNormalization.ClipElementWiseAbsoluteValue<br>
			''' Not used otherwise.<br>
			''' L2 threshold for first two types of clipping, or absolute value threshold for last type of clipping.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			Public Overridable Function gradientNormalizationThreshold(ByVal threshold As Double) As Builder
				Me.gradientNormalizationThreshold_Conflict = threshold
				Return Me
			End Function

			''' <summary>
			''' Sets the convolution mode for convolutional layers, which impacts padding and output sizes.
			''' See <seealso cref="ConvolutionMode"/> for details. Defaults to ConvolutionMode.TRUNCATE<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis. </summary>
			''' <param name="convolutionMode"> Convolution mode to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter convolutionMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function convolutionMode(ByVal convolutionMode_Conflict As ConvolutionMode) As Builder
				Me.convolutionMode_Conflict = convolutionMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets the cuDNN algo mode for convolutional layers, which impacts performance and memory usage of cuDNN.
			''' See <seealso cref="ConvolutionLayer.AlgoMode"/> for details.  Defaults to "PREFER_FASTEST", but "NO_WORKSPACE" uses less memory.
			''' <br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis. </summary>
			''' <param name="cudnnAlgoMode"> cuDNN algo mode to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter cudnnAlgoMode was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function cudnnAlgoMode(ByVal cudnnAlgoMode_Conflict As ConvolutionLayer.AlgoMode) As Builder
				Me.cudnnAlgoMode_Conflict = cudnnAlgoMode_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to all layers. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all parameters of all layers </param>
			Public Overridable Function constrainAllParameters(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.allParamConstraints = New List(Of LayerConstraint) From {constraints}
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to all layers. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all bias parameters of all layers </param>
			Public Overridable Function constrainBias(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.biasConstraints = New List(Of LayerConstraint) From {constraints}
				Return Me
			End Function

			''' <summary>
			''' Set constraints to be applied to all layers. Default: no constraints.<br>
			''' Constraints can be used to enforce certain conditions (non-negativity of parameters, max-norm regularization,
			''' etc). These constraints are applied at each iteration, after the parameters have been updated.<br>
			''' Note: values set by this method will be applied to all applicable layers in the network, unless a different
			''' value is explicitly set on a given layer. In other words: values set via this method are used as the default
			''' value, and can be overridden on a per-layer basis.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all weight parameters of all layers </param>
			Public Overridable Function constrainWeights(ParamArray ByVal constraints() As LayerConstraint) As Builder
				Me.weightConstraints = New List(Of LayerConstraint) From {constraints}
				Return Me
			End Function


			''' <summary>
			''' Set the DataType for the network parameters and activations. Must be a floating point type: <seealso cref="DataType.DOUBLE"/>,
			''' <seealso cref="DataType.FLOAT"/> or <seealso cref="DataType.HALF"/>.<br>
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder dataType(@NonNull DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter dataType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataType(ByVal dataType_Conflict As DataType) As Builder
				Preconditions.checkState(dataType_Conflict = DataType.DOUBLE OrElse dataType_Conflict = DataType.FLOAT OrElse dataType_Conflict = DataType.HALF, "Data type must be a floating point type: one of DOUBLE, FLOAT, or HALF. Got datatype: %s", dataType_Conflict)
				Me.dataType_Conflict = dataType_Conflict
				Return Me
			End Function

			''' <summary>
			''' Return a configuration based on this builder
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As NeuralNetConfiguration

				Dim conf As New NeuralNetConfiguration()
				conf.minimize = minimize_Conflict
				conf.maxNumLineSearchIterations = maxNumLineSearchIterations_Conflict
				conf.layer = layer_Conflict
				conf.optimizationAlgo = optimizationAlgo_Conflict
				conf.seed = seed_Conflict
				conf.stepFunction = stepFunction_Conflict
				conf.miniBatch = miniBatch_Conflict
				conf.cacheMode = Me.cacheMode_Conflict
				conf.dataType = Me.dataType_Conflict

				configureLayer(layer_Conflict)
				If TypeOf layer_Conflict Is FrozenLayer Then
					configureLayer(DirectCast(layer_Conflict, FrozenLayer).getLayer())
				End If

				If TypeOf layer_Conflict Is FrozenLayerWithBackprop Then
					configureLayer(DirectCast(layer_Conflict, FrozenLayerWithBackprop).getUnderlying())
				End If

				Return conf
			End Function

			Friend Overridable Sub configureLayer(ByVal layer As Layer)
				Dim layerName As String
				If layer Is Nothing OrElse layer.LayerName Is Nothing Then
					layerName = "Layer not named"
				Else
					layerName = layer.LayerName
				End If

				If TypeOf layer Is AbstractSameDiffLayer Then
					Dim sdl As AbstractSameDiffLayer = DirectCast(layer, AbstractSameDiffLayer)
					sdl.applyGlobalConfig(Me)
				End If

				If layer IsNot Nothing Then
					copyConfigToLayer(layerName, layer)
				End If

				If TypeOf layer Is FrozenLayer Then
					copyConfigToLayer(layerName, DirectCast(layer, FrozenLayer).getLayer())
				End If

				If TypeOf layer Is FrozenLayerWithBackprop Then
					copyConfigToLayer(layerName, DirectCast(layer, FrozenLayerWithBackprop).getUnderlying())
				End If

				If TypeOf layer Is Bidirectional Then
					Dim b As Bidirectional = DirectCast(layer, Bidirectional)
					copyConfigToLayer(b.getFwd().getLayerName(), b.getFwd())
					copyConfigToLayer(b.getBwd().getLayerName(), b.getBwd())
				End If

				If TypeOf layer Is BaseWrapperLayer Then
					Dim bwr As BaseWrapperLayer = DirectCast(layer, BaseWrapperLayer)
					configureLayer(bwr.getUnderlying())
				End If

				If TypeOf layer Is ConvolutionLayer Then
					Dim cl As ConvolutionLayer = DirectCast(layer, ConvolutionLayer)
					If cl.getConvolutionMode() Is Nothing Then
						cl.setConvolutionMode(convolutionMode_Conflict)
					End If
					If cl.getCudnnAlgoMode() Is Nothing Then
						cl.setCudnnAlgoMode(cudnnAlgoMode_Conflict)
					End If
				End If
				If TypeOf layer Is SubsamplingLayer Then
					Dim sl As SubsamplingLayer = DirectCast(layer, SubsamplingLayer)
					If sl.getConvolutionMode() Is Nothing Then
						sl.setConvolutionMode(convolutionMode_Conflict)
					End If
				End If

				LayerValidation.generalValidation(layerName, layer, idropOut, regularization_Conflict, regularizationBias_Conflict, allParamConstraints, weightConstraints, biasConstraints)
			End Sub

			Friend Overridable Sub copyConfigToLayer(ByVal layerName As String, ByVal layer As Layer)

				If layer.getIDropout() Is Nothing Then
					'Dropout is stateful usually - don't want to have the same instance shared by multiple layers
					layer.setIDropout(If(idropOut Is Nothing, Nothing, idropOut.clone()))
				End If

				If TypeOf layer Is BaseLayer Then
					Dim bLayer As BaseLayer = DirectCast(layer, BaseLayer)
					If bLayer.getRegularization() Is Nothing OrElse bLayer.getRegularization().isEmpty() Then
						bLayer.setRegularization(New List(Of )(regularization_Conflict))
					End If
					If bLayer.getRegularizationBias() Is Nothing OrElse bLayer.getRegularizationBias().isEmpty() Then
						bLayer.setRegularizationBias(New List(Of )(regularizationBias_Conflict))
					End If
					If bLayer.getActivationFn() Is Nothing Then
						bLayer.setActivationFn(activationFn)
					End If
					If bLayer.getWeightInitFn() Is Nothing Then
						bLayer.setWeightInitFn(weightInitFn)
					End If
					If Double.IsNaN(bLayer.getBiasInit()) Then
						bLayer.setBiasInit(biasInit_Conflict)
					End If
					If Double.IsNaN(bLayer.getGainInit()) Then
						bLayer.setGainInit(gainInit)
					End If

					'Configure weight noise:
					If weightNoise_Conflict IsNot Nothing AndAlso DirectCast(layer, BaseLayer).getWeightNoise() Is Nothing Then
						DirectCast(layer, BaseLayer).setWeightNoise(weightNoise_Conflict.clone())
					End If

					'Configure updaters:
					If iUpdater IsNot Nothing AndAlso bLayer.getIUpdater() Is Nothing Then
						bLayer.setIUpdater(iUpdater.clone()) 'Clone the updater to avoid shared instances - in case of setLearningRate calls later
					End If
					If biasUpdater_Conflict IsNot Nothing AndAlso bLayer.getBiasUpdater() Is Nothing Then
						bLayer.setBiasUpdater(biasUpdater_Conflict.clone()) 'Clone the updater to avoid shared instances - in case of setLearningRate calls later
					End If

					If bLayer.getIUpdater() Is Nothing AndAlso iUpdater Is Nothing AndAlso bLayer.initializer().numParams(bLayer) > 0 Then
						'No updater set anywhere
						Dim u As IUpdater = New Sgd()
						bLayer.setIUpdater(u)
						log.warn("*** No updater configuration is set for layer {} - defaulting to {} ***", layerName, u)
					End If

					If bLayer.GradientNormalization = Nothing Then
						bLayer.setGradientNormalization(gradientNormalization_Conflict)
					End If
					If Double.IsNaN(bLayer.GradientNormalizationThreshold) Then
						bLayer.setGradientNormalizationThreshold(gradientNormalizationThreshold_Conflict)
					End If
				End If

				If TypeOf layer Is ActivationLayer Then
					Dim al As ActivationLayer = DirectCast(layer, ActivationLayer)
					If al.getActivationFn() Is Nothing Then
						al.setActivationFn(activationFn)
					End If
				End If
			End Sub
		End Class

	End Class

End Namespace