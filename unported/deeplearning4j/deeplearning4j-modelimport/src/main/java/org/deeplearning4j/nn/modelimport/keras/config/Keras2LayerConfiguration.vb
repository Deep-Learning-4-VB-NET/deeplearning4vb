Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode

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

Namespace org.deeplearning4j.nn.modelimport.keras.config

	''' <summary>
	''' All relevant property fields of keras 2.x layers.
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class Keras2LayerConfiguration extends KerasLayerConfiguration
	Public Class Keras2LayerConfiguration
		Inherits KerasLayerConfiguration

		' Basic layer names 
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_1D As String = "Conv1D"
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_2D As String = "Conv2D"
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_3D As String = "Conv3D"

		Private ReadOnly LAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_2D As String = "SeparableConv2D"
		Private ReadOnly LAYER_CLASS_NAME_DECONVOLUTION_2D As String = "Conv2DTranspose"

		' Partially shared layer configurations. 
		Private ReadOnly LAYER_FIELD_OUTPUT_DIM As String = "units"
		Private ReadOnly LAYER_FIELD_DROPOUT_RATE As String = "rate"
		Private ReadOnly LAYER_FIELD_USE_BIAS As String = "use_bias"
		Private ReadOnly KERAS_PARAM_NAME_W As String = "kernel"
		Private ReadOnly KERAS_PARAM_NAME_B As String = "bias"
		Private ReadOnly KERAS_PARAM_NAME_RW As String = "recurrent_kernel"


		' Keras dimension ordering for, e.g., convolutional layersOrdered. 
		Private ReadOnly LAYER_FIELD_DIM_ORDERING As String = "data_format"
		Private ReadOnly DIM_ORDERING_THEANO As String = "channels_first"
		Private ReadOnly DIM_ORDERING_TENSORFLOW As String = "channels_last"

		' Recurrent layers 
		Private ReadOnly LAYER_FIELD_DROPOUT_W As String = "dropout"
		Private ReadOnly LAYER_FIELD_DROPOUT_U As String = "recurrent_dropout"
		Private ReadOnly LAYER_FIELD_INNER_INIT As String = "recurrent_initializer"
		Private ReadOnly LAYER_FIELD_INNER_ACTIVATION As String = "recurrent_activation"

		' Embedding layer properties 
		Private ReadOnly LAYER_FIELD_EMBEDDING_INIT As String = "embeddings_initializer"
		Private ReadOnly LAYER_FIELD_EMBEDDING_WEIGHTS As String = "embeddings"
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_REGULARIZER As String = "embeddings_regularizer"
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_CONSTRAINT As String = "embeddings_constraint"

		' Normalisation layers 
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_BETA_INIT As String = "beta_initializer"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_GAMMA_INIT As String = "gamma_initializer"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN As String = "moving_mean"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE As String = "moving_variance"

		' Advanced activations 
		Private ReadOnly LAYER_FIELD_PRELU_INIT As String = "alpha_initializer"

		' Convolutional layer properties 
		Private ReadOnly LAYER_FIELD_NB_FILTER As String = "filters"
		Private ReadOnly LAYER_FIELD_CONVOLUTION_STRIDES As String = "strides"
		Private ReadOnly LAYER_FIELD_FILTER_LENGTH As String = "kernel_size"
		Private ReadOnly LAYER_FIELD_SUBSAMPLE_LENGTH As String = "strides"
		Private ReadOnly LAYER_FIELD_DILATION_RATE As String = "dilation_rate"

		' Pooling / Upsampling layer properties 
		Private ReadOnly LAYER_FIELD_POOL_1D_SIZE As String = "pool_size"
		Private ReadOnly LAYER_FIELD_POOL_1D_STRIDES As String = "strides"
		Private ReadOnly LAYER_FIELD_UPSAMPLING_1D_SIZE As String = "size"

		' Keras convolution border modes. 
		Private ReadOnly LAYER_FIELD_BORDER_MODE As String = "padding"

		' Noise layers 
		Private ReadOnly LAYER_FIELD_GAUSSIAN_VARIANCE As String = "stddev"

		' Keras weight regularizers. 
		Private ReadOnly LAYER_FIELD_W_REGULARIZER As String = "kernel_regularizer"
		Private ReadOnly LAYER_FIELD_B_REGULARIZER As String = "bias_regularizer"

		' Keras constraints 
		Private ReadOnly LAYER_FIELD_CONSTRAINT_NAME As String = "class_name"
		Private ReadOnly LAYER_FIELD_W_CONSTRAINT As String = "kernel_constraint"
		Private ReadOnly LAYER_FIELD_B_CONSTRAINT As String = "bias_constraint"
		Private ReadOnly LAYER_FIELD_MAX_CONSTRAINT As String = "max_value"
		Private ReadOnly LAYER_FIELD_MINMAX_MIN_CONSTRAINT As String = "min_value"
		Private ReadOnly LAYER_FIELD_MINMAX_MAX_CONSTRAINT As String = "max_value"

		' Keras weight initializers. 
		Private ReadOnly LAYER_FIELD_INIT As String = "kernel_initializer"

		Private ReadOnly TENSORFLOW_OP_LAYER As String = "TensorFlowOpLayer"
	End Class
End Namespace