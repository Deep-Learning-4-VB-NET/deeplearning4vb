Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BasePretrainNetwork = org.deeplearning4j.nn.conf.layers.BasePretrainNetwork
Imports LayerValidation = org.deeplearning4j.nn.conf.layers.LayerValidation
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports VariationalAutoencoderParamInitializer = org.deeplearning4j.nn.params.VariationalAutoencoderParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.deeplearning4j.nn.conf.layers.variational


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @EqualsAndHashCode(callSuper = true) public class VariationalAutoencoder extends org.deeplearning4j.nn.conf.layers.BasePretrainNetwork
	<Serializable>
	Public Class VariationalAutoencoder
		Inherits BasePretrainNetwork

		Private encoderLayerSizes() As Integer
		Private decoderLayerSizes() As Integer
		Private outputDistribution As ReconstructionDistribution
		Private pzxActivationFn As IActivation
		Private numSamples As Integer

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.encoderLayerSizes = builder.encoderLayerSizes_Conflict
			Me.decoderLayerSizes = builder.decoderLayerSizes_Conflict
			Me.outputDistribution = builder.outputDistribution
			Me.pzxActivationFn = builder.pzxActivationFn_Conflict
			Me.numSamples = builder.numSamples_Conflict
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("VariationalAutoencoder", LayerName, layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.variational.VariationalAutoencoder(conf, networkDataType)

			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return VariationalAutoencoderParamInitializer.Instance
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			If paramName.StartsWith(VariationalAutoencoderParamInitializer.DECODER_PREFIX, StringComparison.Ordinal) Then
				Return True
			End If
			If paramName.StartsWith(VariationalAutoencoderParamInitializer.PZX_LOGSTD2_PREFIX, StringComparison.Ordinal) Then
				Return True
			End If
			If paramName.StartsWith(VariationalAutoencoderParamInitializer.PXZ_PREFIX, StringComparison.Ordinal) Then
				Return True
			End If
			Return False
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'For training: we'll assume unsupervised pretraining, as this has higher memory requirements

			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim actElementsPerEx As val = outputType.arrayElementsPerExample()
			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As Integer = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Dim inferenceWorkingMemSizePerEx As Integer = 0
			'Forward pass size through the encoder:
			For i As Integer = 1 To encoderLayerSizes.Length - 1
				inferenceWorkingMemSizePerEx += encoderLayerSizes(i)
			Next i

			'Forward pass size through the decoder, during training
			'p(Z|X) mean and stdev; pzxSigmaSquared, pzxSigma -> all size equal to nOut
			Dim decoderFwdSizeWorking As Long = 4 * nOut
			'plus, nSamples * decoder size
			'For each decoding: random sample (nOut), z (nOut), activations for each decoder layer
			decoderFwdSizeWorking += numSamples * (2 * nOut + ArrayUtil.sum(getDecoderLayerSizes()))
			'Plus, component of score
			decoderFwdSizeWorking += nOut

			'Backprop size through the decoder and decoder: approx. 2x forward pass size
			Dim trainWorkingMemSize As Long = 2 * (inferenceWorkingMemSizePerEx + decoderFwdSizeWorking)

			If getIDropout() IsNot Nothing Then
				If False Then
					'TODO drop connect
					'Dup the weights... note that this does NOT depend on the minibatch size...
				Else
					'Assume we dup the input
					trainWorkingMemSize += inputType.arrayElementsPerExample()
				End If
			End If

			Return (New LayerMemoryReport.Builder(layerName, GetType(VariationalAutoencoder), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, inferenceWorkingMemSizePerEx, 0, trainWorkingMemSize).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.BasePretrainNetwork.Builder<Builder>
		Public Class Builder
			Inherits BasePretrainNetwork.Builder(Of Builder)

			''' <summary>
			''' Size of the encoder layers, in units. Each encoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers (set via
			''' <seealso cref="decoderLayerSizes(Integer...)"/> is similar to the encoder layers.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field encoderLayerSizes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend encoderLayerSizes_Conflict() As Integer = {100}

			''' <summary>
			''' Size of the decoder layers, in units. Each decoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers is similar
			''' to the encoder layers (set via <seealso cref="encoderLayerSizes(Integer...)"/>.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field decoderLayerSizes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend decoderLayerSizes_Conflict() As Integer = {100}
			''' <summary>
			''' The reconstruction distribution for the data given the hidden state - i.e., P(data|Z).<br> This should be
			''' selected carefully based on the type of data being modelled. For example:<br> - {@link
			''' GaussianReconstructionDistribution} + {identity or tanh} for real-valued (Gaussian) data<br> - {@link
			''' BernoulliReconstructionDistribution} + sigmoid for binary-valued (0 or 1) data<br>
			''' 
			''' </summary>
			Friend outputDistribution As ReconstructionDistribution = New GaussianReconstructionDistribution(Activation.TANH)

			''' <summary>
			''' Activation function for the input to P(z|data).<br> Care should be taken with this, as some activation
			''' functions (relu, etc) are not suitable due to being bounded in range [0,infinity).
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field pzxActivationFn was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend pzxActivationFn_Conflict As IActivation = New ActivationIdentity()

			''' <summary>
			''' Set the number of samples per data point (from VAE state Z) used when doing pretraining. Default value: 1.
			''' <para>
			''' This is parameter L from Kingma and Welling: "In our experiments we found that the number of samples L per
			''' datapoint can be set to 1 as long as the minibatch size M was large enough, e.g. M = 100."
			''' 
			''' </para>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field numSamples was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend numSamples_Conflict As Integer = 1


			''' <summary>
			''' Size of the encoder layers, in units. Each encoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers (set via
			''' <seealso cref="decoderLayerSizes(Integer...)"/> is similar to the encoder layers.
			''' </summary>
			''' <param name="encoderLayerSizes"> Size of each encoder layer in the variational autoencoder </param>
'JAVA TO VB CONVERTER NOTE: The parameter encoderLayerSizes was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function encoderLayerSizes(ParamArray ByVal encoderLayerSizes_Conflict() As Integer) As Builder
				Me.EncoderLayerSizes = encoderLayerSizes_Conflict
				Return Me
			End Function


			''' <summary>
			''' Size of the encoder layers, in units. Each encoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers (set via
			''' <seealso cref="decoderLayerSizes(Integer...)"/> is similar to the encoder layers.
			''' </summary>
			''' <param name="encoderLayerSizes"> Size of each encoder layer in the variational autoencoder </param>
			Public Overridable WriteOnly Property EncoderLayerSizes As Integer()
				Set(ByVal encoderLayerSizes() As Integer)
					If encoderLayerSizes Is Nothing OrElse encoderLayerSizes.Length < 1 Then
						Throw New System.ArgumentException("Encoder layer sizes array must have length > 0")
					End If
					Me.encoderLayerSizes_Conflict = encoderLayerSizes
				End Set
			End Property


			''' <summary>
			''' Size of the decoder layers, in units. Each decoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers is similar
			''' to the encoder layers (set via <seealso cref="encoderLayerSizes(Integer...)"/>.
			''' </summary>
			''' <param name="decoderLayerSizes"> Size of each deccoder layer in the variational autoencoder </param>
'JAVA TO VB CONVERTER NOTE: The parameter decoderLayerSizes was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function decoderLayerSizes(ParamArray ByVal decoderLayerSizes_Conflict() As Integer) As Builder
				Me.DecoderLayerSizes = decoderLayerSizes_Conflict
				Return Me
			End Function

			''' <summary>
			''' Size of the decoder layers, in units. Each decoder layer is functionally equivalent to a {@link
			''' org.deeplearning4j.nn.conf.layers.DenseLayer}. Typically the number and size of the decoder layers is similar
			''' to the encoder layers (set via <seealso cref="encoderLayerSizes(Integer...)"/>.
			''' </summary>
			''' <param name="decoderLayerSizes"> Size of each deccoder layer in the variational autoencoder </param>
			Public Overridable WriteOnly Property DecoderLayerSizes As Integer()
				Set(ByVal decoderLayerSizes() As Integer)
					If decoderLayerSizes Is Nothing OrElse decoderLayerSizes.Length < 1 Then
						Throw New System.ArgumentException("Decoder layer sizes array must have length > 0")
					End If
					Me.decoderLayerSizes_Conflict = decoderLayerSizes
				End Set
			End Property


			''' <summary>
			''' The reconstruction distribution for the data given the hidden state - i.e., P(data|Z).<br> This should be
			''' selected carefully based on the type of data being modelled. For example:<br> - {@link
			''' GaussianReconstructionDistribution} + {identity or tanh} for real-valued (Gaussian) data<br> - {@link
			''' BernoulliReconstructionDistribution} + sigmoid for binary-valued (0 or 1) data<br>
			''' </summary>
			''' <param name="distribution"> Reconstruction distribution </param>
			Public Overridable Function reconstructionDistribution(ByVal distribution As ReconstructionDistribution) As Builder
				Me.setOutputDistribution(distribution)
				Return Me
			End Function

			''' <summary>
			''' Configure the VAE to use the specified loss function for the reconstruction, instead of a
			''' ReconstructionDistribution. Note that this is NOT following the standard VAE design (as per Kingma &
			''' Welling), which assumes a probabilistic output - i.e., some p(x|z). It is however a valid network
			''' configuration, allowing for optimization of more traditional objectives such as mean squared error.<br> Note:
			''' clearly, setting the loss function here will override any previously set recontruction distribution
			''' </summary>
			''' <param name="outputActivationFn"> Activation function for the output/reconstruction </param>
			''' <param name="lossFunction"> Loss function to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function lossFunction(ByVal outputActivationFn As IActivation, ByVal lossFunction_Conflict As LossFunctions.LossFunction) As Builder
				Return lossFunction(outputActivationFn, lossFunction_Conflict.getILossFunction())
			End Function

			''' <summary>
			''' Configure the VAE to use the specified loss function for the reconstruction, instead of a
			''' ReconstructionDistribution. Note that this is NOT following the standard VAE design (as per Kingma &
			''' Welling), which assumes a probabilistic output - i.e., some p(x|z). It is however a valid network
			''' configuration, allowing for optimization of more traditional objectives such as mean squared error.<br> Note:
			''' clearly, setting the loss function here will override any previously set recontruction distribution
			''' </summary>
			''' <param name="outputActivationFn"> Activation function for the output/reconstruction </param>
			''' <param name="lossFunction"> Loss function to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function lossFunction(ByVal outputActivationFn As Activation, ByVal lossFunction_Conflict As LossFunctions.LossFunction) As Builder
				Return lossFunction(outputActivationFn.getActivationFunction(), lossFunction_Conflict.getILossFunction())
			End Function

			''' <summary>
			''' Configure the VAE to use the specified loss function for the reconstruction, instead of a
			''' ReconstructionDistribution. Note that this is NOT following the standard VAE design (as per Kingma &
			''' Welling), which assumes a probabilistic output - i.e., some p(x|z). It is however a valid network
			''' configuration, allowing for optimization of more traditional objectives such as mean squared error.<br> Note:
			''' clearly, setting the loss function here will override any previously set recontruction distribution
			''' </summary>
			''' <param name="outputActivationFn"> Activation function for the output/reconstruction </param>
			''' <param name="lossFunction"> Loss function to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter lossFunction was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Overloads Function lossFunction(ByVal outputActivationFn As IActivation, ByVal lossFunction_Conflict As ILossFunction) As Builder
				Return reconstructionDistribution(New LossFunctionWrapper(outputActivationFn, lossFunction_Conflict))
			End Function

			''' <summary>
			''' Activation function for the input to P(z|data).<br> Care should be taken with this, as some activation
			''' functions (relu, etc) are not suitable due to being bounded in range [0,infinity).
			''' </summary>
			''' <param name="activationFunction"> Activation function for p(z|x) </param>
			Public Overridable Function pzxActivationFn(ByVal activationFunction As IActivation) As Builder
				Me.setPzxActivationFn(activationFunction)
				Return Me
			End Function

			''' <summary>
			''' Activation function for the input to P(z|data).<br> Care should be taken with this, as some activation
			''' functions (relu, etc) are not suitable due to being bounded in range [0,infinity).
			''' </summary>
			''' <param name="activation"> Activation function for p(z|x) </param>
			Public Overridable Function pzxActivationFunction(ByVal activation As Activation) As Builder
				Return pzxActivationFn(activation.getActivationFunction())
			End Function

			''' <summary>
			''' Set the size of the VAE state Z. This is the output size during standard forward pass, and the size of the
			''' distribution P(Z|data) during pretraining.
			''' </summary>
			''' <param name="nOut"> Size of P(Z|data) and output size </param>
'JAVA TO VB CONVERTER NOTE: The parameter nOut was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function nOut(ByVal nOut_Conflict As Integer) As Builder
				MyBase.nOut(nOut_Conflict)
				Return Me
			End Function

			''' <summary>
			''' Set the number of samples per data point (from VAE state Z) used when doing pretraining. Default value: 1.
			''' <para>
			''' This is parameter L from Kingma and Welling: "In our experiments we found that the number of samples L per
			''' datapoint can be set to 1 as long as the minibatch size M was large enough, e.g. M = 100."
			''' 
			''' </para>
			''' </summary>
			''' <param name="numSamples"> Number of samples per data point for pretraining </param>
'JAVA TO VB CONVERTER NOTE: The parameter numSamples was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function numSamples(ByVal numSamples_Conflict As Integer) As Builder
				Me.setNumSamples(numSamples_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public VariationalAutoencoder build()
			Public Overrides Function build() As VariationalAutoencoder
				Return New VariationalAutoencoder(Me)
			End Function
		End Class
	End Class

End Namespace