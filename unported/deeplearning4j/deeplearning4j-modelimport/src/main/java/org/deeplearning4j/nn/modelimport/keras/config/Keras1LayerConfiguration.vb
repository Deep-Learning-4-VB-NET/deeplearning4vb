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
	''' All relevant property fields of keras 1.x layers.
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = false) public class Keras1LayerConfiguration extends KerasLayerConfiguration
	Public Class Keras1LayerConfiguration
		Inherits KerasLayerConfiguration

		' Basic layer names 
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_1D As String = "Convolution1D"
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_2D As String = "Convolution2D"
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_3D As String = "Convolution3D"

		Private ReadOnly LAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_2D As String = "SeparableConvolution2D"
		Private ReadOnly LAYER_CLASS_NAME_DECONVOLUTION_2D As String = "Deconvolution2D"

		' Partially shared layer configurations. 
		Private ReadOnly LAYER_FIELD_OUTPUT_DIM As String = "output_dim"
		Private ReadOnly LAYER_FIELD_DROPOUT_RATE As String = "p"
		Private ReadOnly LAYER_FIELD_USE_BIAS As String = "bias"
		Private ReadOnly KERAS_PARAM_NAME_W As String = "W"
		Private ReadOnly KERAS_PARAM_NAME_B As String = "b"
		Private ReadOnly KERAS_PARAM_NAME_RW As String = "U"


		' Keras dimension ordering for, e.g., convolutional layersOrdered. 
		Private ReadOnly LAYER_FIELD_DIM_ORDERING As String = "dim_ordering"
		Private ReadOnly DIM_ORDERING_THEANO As String = "th"
		Private ReadOnly DIM_ORDERING_TENSORFLOW As String = "tf"

		' Recurrent layers 
		Private ReadOnly LAYER_FIELD_DROPOUT_W As String = "dropout_W"
		Private ReadOnly LAYER_FIELD_DROPOUT_U As String = "dropout_U"
		Private ReadOnly LAYER_FIELD_INNER_INIT As String = "inner_init"
		Private ReadOnly LAYER_FIELD_INNER_ACTIVATION As String = "inner_activation"

		' Embedding layer properties 
		Private ReadOnly LAYER_FIELD_EMBEDDING_INIT As String = "init"
		Private ReadOnly LAYER_FIELD_EMBEDDING_WEIGHTS As String = "W"
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_REGULARIZER As String = "W_regularizer"
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_CONSTRAINT As String = "W_constraint"

		' Normalisation layers 
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_BETA_INIT As String = "beta_init"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_GAMMA_INIT As String = "gamma_init"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN As String = "running_mean"
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE As String = "running_std"

		' Advanced activations 
		Private ReadOnly LAYER_FIELD_PRELU_INIT As String = "init"

		' Convolutional layer properties 
		Private ReadOnly LAYER_FIELD_NB_FILTER As String = "nb_filter"
		Private ReadOnly LAYER_FIELD_CONVOLUTION_STRIDES As String = "subsample"
		Private ReadOnly LAYER_FIELD_FILTER_LENGTH As String = "filter_length"
		Private ReadOnly LAYER_FIELD_SUBSAMPLE_LENGTH As String = "subsample_length"
		Private ReadOnly LAYER_FIELD_DILATION_RATE As String = "atrous_rate"


		' Pooling / Upsampling layer properties 
		Private ReadOnly LAYER_FIELD_POOL_1D_SIZE As String = "pool_length"
		Private ReadOnly LAYER_FIELD_POOL_1D_STRIDES As String = "stride"
		Private ReadOnly LAYER_FIELD_UPSAMPLING_1D_SIZE As String = "length"

		' Keras convolution border modes. 
		Private ReadOnly LAYER_FIELD_BORDER_MODE As String = "border_mode"

		' Noise layers 
		Private ReadOnly LAYER_FIELD_GAUSSIAN_VARIANCE As String = "sigma"

		' Keras weight regularizers. 
		Private ReadOnly LAYER_FIELD_W_REGULARIZER As String = "W_regularizer"
		Private ReadOnly LAYER_FIELD_B_REGULARIZER As String = "b_regularizer"

		' Keras constraints 
		Private ReadOnly LAYER_FIELD_CONSTRAINT_NAME As String = "name"
		Private ReadOnly LAYER_FIELD_W_CONSTRAINT As String = "W_constraint"
		Private ReadOnly LAYER_FIELD_B_CONSTRAINT As String = "b_constraint"
		Private ReadOnly LAYER_FIELD_MAX_CONSTRAINT As String = "m"
		Private ReadOnly LAYER_FIELD_MINMAX_MIN_CONSTRAINT As String = "low"
		Private ReadOnly LAYER_FIELD_MINMAX_MAX_CONSTRAINT As String = "high"


		' Keras weight initializers. 
		Private ReadOnly LAYER_FIELD_INIT As String = "init"

	End Class

End Namespace