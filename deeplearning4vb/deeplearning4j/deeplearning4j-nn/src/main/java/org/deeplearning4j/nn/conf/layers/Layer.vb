Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @Data @NoArgsConstructor public abstract class Layer implements org.deeplearning4j.nn.api.TrainingConfig, java.io.Serializable, Cloneable
	<Serializable>
	Public MustInherit Class Layer
		Implements TrainingConfig, ICloneable

		Public MustOverride ReadOnly Property GradientNormalizationThreshold As Double Implements TrainingConfig.getGradientNormalizationThreshold
		Public MustOverride ReadOnly Property GradientNormalization As org.deeplearning4j.nn.conf.GradientNormalization Implements TrainingConfig.getGradientNormalization
		Public MustOverride ReadOnly Property LayerName As String Implements TrainingConfig.getLayerName

		Protected Friend layerName As String
		Protected Friend iDropout As IDropout
		Protected Friend constraints As IList(Of LayerConstraint)


		Public Sub New(ByVal builder As Builder)
			Me.layerName = builder.layerName
			Me.iDropout = builder.iDropout
		End Sub

		''' <summary>
		''' Initialize the weight constraints. Should be called last, in the outer-most constructor
		''' </summary>
		Protected Friend Overridable Sub initializeConstraints(Of T1)(ByVal builder As Builder(Of T1))
			'Note: this has to be done AFTER all constructors have finished - otherwise the required
			' fields may not yet be set yet
			Dim allConstraints As IList(Of LayerConstraint) = New List(Of LayerConstraint)()
			If builder.allParamConstraints IsNot Nothing AndAlso initializer().paramKeys(Me).Count > 0 Then
				For Each c As LayerConstraint In builder.allParamConstraints
					Dim c2 As LayerConstraint = c.clone()
					c2.Params = New HashSet(Of String)(initializer().paramKeys(Me))
					allConstraints.Add(c2)
				Next c
			End If

			If builder.weightConstraints IsNot Nothing AndAlso initializer().weightKeys(Me).Count > 0 Then
				For Each c As LayerConstraint In builder.weightConstraints
					Dim c2 As LayerConstraint = c.clone()
					c2.Params = New HashSet(Of String)(initializer().weightKeys(Me))
					allConstraints.Add(c2)
				Next c
			End If

			If builder.biasConstraints IsNot Nothing AndAlso initializer().biasKeys(Me).Count > 0 Then
				For Each c As LayerConstraint In builder.biasConstraints
					Dim c2 As LayerConstraint = c.clone()
					c2.Params = New HashSet(Of String)(initializer().biasKeys(Me))
					allConstraints.Add(c2)
				Next c
			End If
			If allConstraints.Count > 0 Then
				Me.constraints = allConstraints
			Else
				Me.constraints = Nothing
			End If
			Me.iDropout = builder.iDropout
		End Sub

		''' <summary>
		''' Reset the learning related configs of the layer to default. When instantiated with a global
		''' neural network configuration the parameters specified in the neural network configuration
		''' will be used. For internal use with the transfer learning API. Users should not have to call
		''' this method directly.
		''' </summary>
		Public Overridable Sub resetLayerDefaultConfig()
			'clear the learning related params for all layers in the origConf and set to defaults
			Me.iDropout = Nothing
			Me.constraints = Nothing
		End Sub

		Public Overrides Function clone() As Layer
			Try
				Dim ret As Layer = CType(MyBase.clone(), Layer)
				'Let's check for any INDArray fields and dup them (in case cloned layer will be used in different threads on CUDA...
				' we don't want it being relocated contantly between devices)
				Dim c As Type = Me.GetType()
				Do While c <> GetType(Object)
					Dim fields() As System.Reflection.FieldInfo = c.GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
					For Each f As System.Reflection.FieldInfo In fields
						If f.getType() = GetType(INDArray) Then
							f.setAccessible(True)
							Dim toClone As INDArray
							Try
								toClone = DirectCast(f.get(Me), INDArray)
							Catch e As Exception
								Throw New Exception(e)
							End Try
							If toClone IsNot Nothing Then
								Try
									f.set(Me, toClone.dup())
								Catch e As Exception
									Throw New Exception(e)
								End Try
							End If
						End If
					Next f

					c = c.BaseType
				Loop

				Return ret
			Catch e As CloneNotSupportedException
				Throw New Exception(e)
			End Try
		End Function

		Public MustOverride Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

		''' <returns> The parameter initializer for this model </returns>
		Public MustOverride Function initializer() As ParamInitializer

		''' <summary>
		''' For a given type of input to this layer, what is the type of the output?
		''' </summary>
		''' <param name="layerIndex"> Index of the layer </param>
		''' <param name="inputType"> Type of input for the layer </param>
		''' <returns> Type of output from the layer </returns>
		''' <exception cref="IllegalStateException"> if input type is invalid for this layer </exception>
		Public MustOverride Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType

		''' <summary>
		''' Set the nIn value (number of inputs, or input channels for CNNs) based on the given input
		''' type
		''' </summary>
		''' <param name="inputType"> Input type for this layer </param>
		''' <param name="override"> If false: only set the nIn value if it's not already set. If true: set it
		''' regardless of whether it's already set or not. </param>
		''' <exception cref="IllegalStateException"> if input type is invalid for this layer </exception>
		Public MustOverride Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)


		''' <summary>
		''' For the given type of input to this layer, what preprocessor (if any) is required?<br>
		''' Returns null if no preprocessor is required, otherwise returns an appropriate {@link
		''' InputPreProcessor} for this layer, such as a <seealso cref="org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor"/>
		''' </summary>
		''' <param name="inputType"> InputType to this layer </param>
		''' <returns> Null if no preprocessor is required, otherwise the type of preprocessor necessary for
		''' this layer/input combination </returns>
		''' <exception cref="IllegalStateException"> if input type is invalid for this layer </exception>
		Public MustOverride Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor

		''' <summary>
		''' Get the regularization types (l1/l2/weight decay) for the given parameter. Different parameters may have different
		''' regularization types.
		''' </summary>
		''' <param name="paramName"> Parameter name ("W", "b" etc) </param>
		''' <returns> Regularization types (if any) for the specified parameter </returns>
		Public MustOverride Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)

		''' <summary>
		''' Is the specified parameter a layerwise pretraining only parameter?<br> For example, visible
		''' bias params in an autoencoder (or, decoder params in a variational autoencoder) aren't used
		''' during supervised backprop.<br> Layers (like DenseLayer, etc) with no pretrainable parameters
		''' will return false for all (valid) inputs.
		''' </summary>
		''' <param name="paramName"> Parameter name/key </param>
		''' <returns> True if the parameter is for layerwise pretraining only, false otherwise </returns>
		Public MustOverride Function isPretrainParam(ByVal paramName As String) As Boolean Implements TrainingConfig.isPretrainParam

		''' <summary>
		''' Get the updater for the given parameter. Typically the same updater will be used for all
		''' updaters, but this is not necessarily the case
		''' </summary>
		''' <param name="paramName"> Parameter name </param>
		''' <returns> IUpdater for the parameter </returns>
		Public Overridable Function getUpdaterByParam(ByVal paramName As String) As IUpdater Implements TrainingConfig.getUpdaterByParam
			Throw New System.NotSupportedException("Not supported: all layers with parameters should override this method")
		End Function

		Public Overridable WriteOnly Property DataType Implements TrainingConfig.setDataType As DataType
			Set(ByVal dataType As DataType)
				'No-op for most layers
			End Set
		End Property

		''' <summary>
		''' This is a report of the estimated memory consumption for the given layer
		''' </summary>
		''' <param name="inputType"> Input type to the layer. Memory consumption is often a function of the input
		''' type </param>
		''' <returns> Memory report for the layer </returns>
		Public MustOverride Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @Getter @Setter public abstract static class Builder<T extends Builder<T>>
		Public MustInherit Class Builder(Of T As Builder(Of T))

			Protected Friend layerName As String = Nothing

			Protected Friend allParamConstraints As IList(Of LayerConstraint)

			Protected Friend weightConstraints As IList(Of LayerConstraint)

			Protected Friend biasConstraints As IList(Of LayerConstraint)

			Protected Friend iDropout As IDropout

			''' <summary>
			''' Layer name assigns layer string name. Allows easier differentiation between layers.
			''' </summary>
			Public Overridable Function name(ByVal layerName As String) As T
				Me.setLayerName(layerName)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Dropout probability. This is the probability of <it>retaining</it> each input activation
			''' value for a layer. dropOut(x) will keep an input activation with probability x, and set
			''' to 0 with probability 1-x.<br> dropOut(0.0) is a special value / special case - when set
			''' to 0.0., dropout is disabled (not applied). Note that a dropout value of 1.0 is
			''' functionally equivalent to no dropout: i.e., 100% probability of retaining each input
			''' activation.<br> When useDropConnect(boolean) is set to true (false by default), this
			''' method sets the drop connect probability instead.
			''' <para>
			''' Note 1: Dropout is applied at training time only - and is automatically not applied at
			''' test time (for evaluation, etc)<br> Note 2: This sets the probability per-layer. Care
			''' should be taken when setting lower values for complex networks (too much information may
			''' be lost with aggressive (very low) dropout values).<br> Note 3: Frequently, dropout is
			''' not applied to (or, has higher retain probability for) input (first layer) layers.
			''' Dropout is also often not applied to output layers. This needs to be handled MANUALLY by
			''' the user - set .dropout(0) on those layers when using global dropout setting.<br> Note 4:
			''' Implementation detail (most users can ignore): DL4J uses inverted dropout, as described
			''' here:
			''' <a href="http://cs231n.github.io/neural-networks-2/">http://cs231n.github.io/neural-networks-2/</a>
			''' </para>
			''' </summary>
			''' <param name="inputRetainProbability"> Dropout probability (probability of retaining each input
			''' activation value for a layer) </param>
			''' <seealso cref= #dropOut(IDropout) </seealso>
			Public Overridable Function dropOut(ByVal inputRetainProbability As Double) As T
				If inputRetainProbability = 0.0 Then
					Return dropOut(Nothing)
				End If
				Return dropOut(New Dropout(inputRetainProbability))
			End Function

			''' <summary>
			''' Set the dropout for all layers in this network
			''' </summary>
			''' <param name="dropout"> Dropout, such as <seealso cref="Dropout"/>, <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianDropout"/>,
			''' <seealso cref="org.deeplearning4j.nn.conf.dropout.GaussianNoise"/> etc </param>
'JAVA TO VB CONVERTER NOTE: The parameter dropout was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dropOut(ByVal dropout_Conflict As IDropout) As T
				Me.setIDropout(dropout_Conflict)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set constraints to be applied to this layer. Default: no constraints.<br> Constraints can
			''' be used to enforce certain conditions (non-negativity of parameters, max-norm
			''' regularization, etc). These constraints are applied at each iteration, after the
			''' parameters have been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all parameters of this layer </param>
			Public Overridable Function constrainAllParameters(ParamArray ByVal constraints() As LayerConstraint) As T
				Me.setAllParamConstraints(java.util.Arrays.asList(constraints))
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set constraints to be applied to bias parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of
			''' parameters, max-norm regularization, etc). These constraints are applied at each
			''' iteration, after the parameters have been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all bias parameters of this layer </param>
			Public Overridable Function constrainBias(ParamArray ByVal constraints() As LayerConstraint) As T
				Me.setBiasConstraints(java.util.Arrays.asList(constraints))
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Set constraints to be applied to the weight parameters of this layer. Default: no
			''' constraints.<br> Constraints can be used to enforce certain conditions (non-negativity of
			''' parameters, max-norm regularization, etc). These constraints are applied at each
			''' iteration, after the parameters have been updated.
			''' </summary>
			''' <param name="constraints"> Constraints to apply to all weight parameters of this layer </param>
			Public Overridable Function constrainWeights(ParamArray ByVal constraints() As LayerConstraint) As T
				Me.setWeightConstraints(java.util.Arrays.asList(constraints))
				Return CType(Me, T)
			End Function

			Public MustOverride Function build(Of E As Layer)() As E
		End Class
	End Class

End Namespace