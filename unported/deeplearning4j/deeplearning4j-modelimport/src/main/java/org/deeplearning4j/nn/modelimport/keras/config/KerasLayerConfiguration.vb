Imports Data = lombok.Data

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
	''' All relevant property fields of keras layers.
	''' <para>
	''' Empty String fields mean Keras 1 and 2 implementations differ,
	''' supplied fields stand for shared properties.
	''' 
	''' @author Max Pumperla
	''' </para>
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class KerasLayerConfiguration
	Public Class KerasLayerConfiguration

		Private ReadOnly LAYER_FIELD_KERAS_VERSION As String = "keras_version"
		Private ReadOnly LAYER_FIELD_CLASS_NAME As String = "class_name"
		Private ReadOnly LAYER_FIELD_LAYER As String = "layer"

		Private ReadOnly LAYER_CLASS_NAME_ACTIVATION As String = "Activation"
		Private ReadOnly LAYER_CLASS_NAME_INPUT As String = "InputLayer"
		Private ReadOnly LAYER_CLASS_NAME_PERMUTE As String = "Permute"
		Private ReadOnly LAYER_CLASS_NAME_DROPOUT As String = "Dropout"
		Private ReadOnly LAYER_CLASS_NAME_REPEAT As String = "RepeatVector"
		Private ReadOnly LAYER_CLASS_NAME_LAMBDA As String = "Lambda"
		Private ReadOnly LAYER_CLASS_NAME_MASKING As String = "Masking"


		Private ReadOnly LAYER_CLASS_NAME_SPATIAL_DROPOUT_1D As String = "SpatialDropout1D"
		Private ReadOnly LAYER_CLASS_NAME_SPATIAL_DROPOUT_2D As String = "SpatialDropout2D"
		Private ReadOnly LAYER_CLASS_NAME_SPATIAL_DROPOUT_3D As String = "SpatialDropout3D"
		Private ReadOnly LAYER_CLASS_NAME_ALPHA_DROPOUT As String = "AlphaDropout"
		Private ReadOnly LAYER_CLASS_NAME_GAUSSIAN_DROPOUT As String = "GaussianDropout"
		Private ReadOnly LAYER_CLASS_NAME_GAUSSIAN_NOISE As String = "GaussianNoise"
		Private ReadOnly LAYER_CLASS_NAME_DENSE As String = "Dense"

		Private ReadOnly LAYER_CLASS_NAME_LSTM As String = "LSTM"
		Private ReadOnly LAYER_CLASS_NAME_SIMPLE_RNN As String = "SimpleRNN"

		Private ReadOnly LAYER_CLASS_NAME_BIDIRECTIONAL As String = "Bidirectional"
		Private ReadOnly LAYER_CLASS_NAME_TIME_DISTRIBUTED As String = "TimeDistributed"


		Private ReadOnly LAYER_CLASS_NAME_MAX_POOLING_1D As String = "MaxPooling1D"
		Private ReadOnly LAYER_CLASS_NAME_MAX_POOLING_2D As String = "MaxPooling2D"
		Private ReadOnly LAYER_CLASS_NAME_MAX_POOLING_3D As String = "MaxPooling3D"
		Private ReadOnly LAYER_CLASS_NAME_AVERAGE_POOLING_1D As String = "AveragePooling1D"
		Private ReadOnly LAYER_CLASS_NAME_AVERAGE_POOLING_2D As String = "AveragePooling2D"
		Private ReadOnly LAYER_CLASS_NAME_AVERAGE_POOLING_3D As String = "AveragePooling3D"
		Private ReadOnly LAYER_CLASS_NAME_ZERO_PADDING_1D As String = "ZeroPadding1D"
		Private ReadOnly LAYER_CLASS_NAME_ZERO_PADDING_2D As String = "ZeroPadding2D"
		Private ReadOnly LAYER_CLASS_NAME_ZERO_PADDING_3D As String = "ZeroPadding3D"
		Private ReadOnly LAYER_CLASS_NAME_CROPPING_1D As String = "Cropping1D"
		Private ReadOnly LAYER_CLASS_NAME_CROPPING_2D As String = "Cropping2D"
		Private ReadOnly LAYER_CLASS_NAME_CROPPING_3D As String = "Cropping3D"


		Private ReadOnly LAYER_CLASS_NAME_FLATTEN As String = "Flatten"
		Private ReadOnly LAYER_CLASS_NAME_RESHAPE As String = "Reshape"
		Private ReadOnly LAYER_CLASS_NAME_MERGE As String = "Merge"
		Private ReadOnly LAYER_CLASS_NAME_ADD As String = "Add"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_ADD As String = "add"
		Private ReadOnly LAYER_CLASS_NAME_SUBTRACT As String = "Subtract"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_SUBTRACT As String = "subtract"
		Private ReadOnly LAYER_CLASS_NAME_MULTIPLY As String = "Multiply"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_MULTIPLY As String = "multiply"
		Private ReadOnly LAYER_CLASS_NAME_AVERAGE As String = "Average"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_AVERAGE As String = "average"
		Private ReadOnly LAYER_CLASS_NAME_MAXIMUM As String = "Maximum"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_MAXIMUM As String = "maximum"
		Private ReadOnly LAYER_CLASS_NAME_CONCATENATE As String = "Concatenate"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_CONCATENATE As String = "concatenate"
		Private ReadOnly LAYER_CLASS_NAME_DOT As String = "Dot"
		Private ReadOnly LAYER_CLASS_NAME_FUNCTIONAL_DOT As String = "dot"


		Private ReadOnly LAYER_CLASS_NAME_BATCHNORMALIZATION As String = "BatchNormalization"
		Private ReadOnly LAYER_CLASS_NAME_EMBEDDING As String = "Embedding"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_MAX_POOLING_1D As String = "GlobalMaxPooling1D"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_MAX_POOLING_2D As String = "GlobalMaxPooling2D"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_MAX_POOLING_3D As String = "GlobalMaxPooling3D"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_1D As String = "GlobalAveragePooling1D"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_2D As String = "GlobalAveragePooling2D"
		Private ReadOnly LAYER_CLASS_NAME_GLOBAL_AVERAGE_POOLING_3D As String = "GlobalAveragePooling3D"
		Private ReadOnly LAYER_CLASS_NAME_TIME_DISTRIBUTED_DENSE As String = "TimeDistributedDense" ' Keras 1 only
		Private ReadOnly LAYER_CLASS_NAME_ATROUS_CONVOLUTION_1D As String = "AtrousConvolution1D" ' Keras 1 only
		Private ReadOnly LAYER_CLASS_NAME_ATROUS_CONVOLUTION_2D As String = "AtrousConvolution2D" ' Keras 1 only
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_1D As String = "" ' 1: Convolution1D, 2: Conv1D
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_2D As String = "" ' 1: Convolution2D, 2: Conv2D
		Private ReadOnly LAYER_CLASS_NAME_CONVOLUTION_3D As String = "" ' 1: Convolution2D, 2: Conv2D
		Private ReadOnly LAYER_CLASS_NAME_LEAKY_RELU As String = "LeakyReLU"
		Private ReadOnly LAYER_CLASS_NAME_PRELU As String = "PReLU"
		Private ReadOnly LAYER_CLASS_NAME_THRESHOLDED_RELU As String = "ThresholdedReLU"
		Private ReadOnly LAYER_CLASS_NAME_RELU As String = "ReLU"
		Private ReadOnly LAYER_CLASS_NAME_ELU As String = "ELU"
		Private ReadOnly LAYER_CLASS_NAME_SOFTMAX As String = "Softmax"
		Private ReadOnly LAYER_CLASS_NAME_UPSAMPLING_1D As String = "UpSampling1D"
		Private ReadOnly LAYER_CLASS_NAME_UPSAMPLING_2D As String = "UpSampling2D"
		Private ReadOnly LAYER_CLASS_NAME_UPSAMPLING_3D As String = "UpSampling3D"
		Private ReadOnly LAYER_CLASS_NAME_DEPTHWISE_CONVOLUTION_2D As String = "DepthwiseConv2D" ' Keras 2 only
		Private ReadOnly LAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_1D As String = "SeparableConv1D" ' Keras 2 only
		Private ReadOnly LAYER_CLASS_NAME_SEPARABLE_CONVOLUTION_2D As String = "" ' 1: SeparableConvolution2D, 2: SeparableConv2D
		Private ReadOnly LAYER_CLASS_NAME_DECONVOLUTION_2D As String = "" ' 1: Deconvolution2D, 2: Conv2DTranspose
		Private ReadOnly LAYER_CLASS_NAME_DECONVOLUTION_3D As String = "Conv3DTranspose" ' Keras 2 only

		' Locally connected layers
		Private ReadOnly LAYER_CLASS_NAME_LOCALLY_CONNECTED_2D As String = "LocallyConnected2D"
		Private ReadOnly LAYER_CLASS_NAME_LOCALLY_CONNECTED_1D As String = "LocallyConnected1D"


		' Partially shared layer configurations. 
		Private ReadOnly LAYER_FIELD_INPUT_SHAPE As String = "input_shape"
		Private ReadOnly LAYER_FIELD_CONFIG As String = "config"
		Private ReadOnly LAYER_FIELD_NAME As String = "name"
		Private ReadOnly LAYER_FIELD_BATCH_INPUT_SHAPE As String = "batch_input_shape"
		Private ReadOnly LAYER_FIELD_INBOUND_NODES As String = "inbound_nodes"
		Private ReadOnly LAYER_FIELD_OUTBOUND_NODES As String = "outbound_nodes"
		Private ReadOnly LAYER_FIELD_DROPOUT As String = "dropout"
		Private ReadOnly LAYER_FIELD_ACTIVITY_REGULARIZER As String = "activity_regularizer"
		Private ReadOnly LAYER_FIELD_EMBEDDING_OUTPUT_DIM As String = "output_dim"
		Private ReadOnly LAYER_FIELD_OUTPUT_DIM As String = "" ' 1: output_dim, 2: units
		Private ReadOnly LAYER_FIELD_DROPOUT_RATE As String = "" ' 1: p, 2: rate
		Private ReadOnly LAYER_FIELD_USE_BIAS As String = "" ' 1: bias, 2: use_bias
		Private ReadOnly KERAS_PARAM_NAME_W As String = "" ' 1: W, 2: kernel
		Private ReadOnly KERAS_PARAM_NAME_B As String = "" ' 1: b, 2: bias
		Private ReadOnly KERAS_PARAM_NAME_RW As String = "" ' 1: U, 2: recurrent_kernel

		' Utils 
		Private ReadOnly LAYER_FIELD_REPEAT_MULTIPLIER As String = "n"

		' Keras dimension ordering for, e.g., convolutional layersOrdered. 
		Private ReadOnly LAYER_FIELD_BACKEND As String = "backend" ' not available in keras 1, caught in code
		Private ReadOnly LAYER_FIELD_DIM_ORDERING As String = "" ' 1: dim_ordering, 2: data_format
		Private ReadOnly DIM_ORDERING_THEANO As String = "" ' 1: th, 2: channels_first
		Private ReadOnly DIM_ORDERING_TENSORFLOW As String = "" ' 1: tf, 2: channels_last

		' Recurrent layers 
		Private ReadOnly LAYER_FIELD_DROPOUT_W As String = "" ' 1: dropout_W, 2: dropout
		Private ReadOnly LAYER_FIELD_DROPOUT_U As String = "" ' 2: dropout_U, 2: recurrent_dropout
		Private ReadOnly LAYER_FIELD_INNER_INIT As String = "" ' 1: inner_init, 2: recurrent_initializer
		Private ReadOnly LAYER_FIELD_RECURRENT_CONSTRAINT As String = "recurrent_constraint" ' keras 2 only
		Private ReadOnly LAYER_FIELD_RECURRENT_DROPOUT As String = "" ' 1: dropout_U, 2: recurrent_dropout
		Private ReadOnly LAYER_FIELD_INNER_ACTIVATION As String = "" ' 1: inner_activation, 2: recurrent_activation
		Private ReadOnly LAYER_FIELD_FORGET_BIAS_INIT As String = "forget_bias_init" ' keras 1 only: string
		Private ReadOnly LAYER_FIELD_UNIT_FORGET_BIAS As String = "unit_forget_bias"
		Private ReadOnly LAYER_FIELD_RETURN_SEQUENCES As String = "return_sequences"
		Private ReadOnly LAYER_FIELD_UNROLL As String = "unroll"

		' Embedding layer properties 
		Private ReadOnly LAYER_FIELD_INPUT_DIM As String = "input_dim"
		Private ReadOnly LAYER_FIELD_EMBEDDING_INIT As String = "" ' 1: "init", 2: "embeddings_initializer"
		Private ReadOnly LAYER_FIELD_EMBEDDING_WEIGHTS As String = "" ' 1: "W", 2: "embeddings"
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_REGULARIZER As String = "" ' 1: W_regularizer, 2: embeddings_regularizer
		Private ReadOnly LAYER_FIELD_EMBEDDINGS_CONSTRAINT As String = "" ' 1: W_constraint, 2: embeddings_constraint
		Private ReadOnly LAYER_FIELD_MASK_ZERO As String = "mask_zero"
		Private ReadOnly LAYER_FIELD_INPUT_LENGTH As String = "input_length"

		' Masking layer properties 
		Private ReadOnly LAYER_FIELD_MASK_VALUE As String = "mask_value"


		' Keras separable convolution types 
		Private ReadOnly LAYER_PARAM_NAME_DEPTH_WISE_KERNEL As String = "depthwise_kernel"
		Private ReadOnly LAYER_PARAM_NAME_POINT_WISE_KERNEL As String = "pointwise_kernel"
		Private ReadOnly LAYER_FIELD_DEPTH_MULTIPLIER As String = "depth_multiplier"


		Private ReadOnly LAYER_FIELD_DEPTH_WISE_INIT As String = "depthwise_initializer"
		Private ReadOnly LAYER_FIELD_POINT_WISE_INIT As String = "pointwise_initializer"

		Private ReadOnly LAYER_FIELD_DEPTH_WISE_REGULARIZER As String = "depthwise_regularizer"
		Private ReadOnly LAYER_FIELD_POINT_WISE_REGULARIZER As String = "pointwise_regularizer"

		Private ReadOnly LAYER_FIELD_DEPTH_WISE_CONSTRAINT As String = "depthwise_constraint"
		Private ReadOnly LAYER_FIELD_POINT_WISE_CONSTRAINT As String = "pointwise_constraint"

		' Normalisation layers 
		' Missing: keras 2 moving_mean_initializer, moving_variance_initializer
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MODE As String = "mode" ' keras 1 only
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_BETA_INIT As String = "" ' 1: beta_init, 2: beta_initializer
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_GAMMA_INIT As String = "" ' 1: gamma_init, 2: gamma_initializer
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_BETA_CONSTRAINT As String = "beta_constraint" ' keras 2 only
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_GAMMA_CONSTRAINT As String = "gamma_constraint" ' keras 2 only
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_MEAN As String = "" ' 1: running_mean, 2: moving_mean
		Private ReadOnly LAYER_FIELD_BATCHNORMALIZATION_MOVING_VARIANCE As String = "" ' 1: running_std, 2: moving_variance

		' Advanced activations 
		' Missing: LeakyReLU, PReLU, ThresholdedReLU, ParametricSoftplus, SReLu
		Private ReadOnly LAYER_FIELD_PRELU_INIT As String = "" ' 1: init, 2: alpha_initializer

		' Convolutional layer properties 
		Private ReadOnly LAYER_FIELD_NB_FILTER As String = "" ' 1: nb_filter, 2: filters
		Private ReadOnly LAYER_FIELD_NB_ROW As String = "nb_row" ' keras 1 only
		Private ReadOnly LAYER_FIELD_NB_COL As String = "nb_col" ' keras 1 only
		Private ReadOnly LAYER_FIELD_KERNEL_SIZE As String = "kernel_size" ' keras 2 only
		Private ReadOnly LAYER_FIELD_POOL_SIZE As String = "pool_size"
		Private ReadOnly LAYER_FIELD_CONVOLUTION_STRIDES As String = "" ' 1: subsample, 2: strides
		Private ReadOnly LAYER_FIELD_FILTER_LENGTH As String = "" ' 1: filter_length, 2: kernel_size
		Private ReadOnly LAYER_FIELD_SUBSAMPLE_LENGTH As String = "" ' 1: subsample_length, 2: strides
		Private ReadOnly LAYER_FIELD_DILATION_RATE As String = "" ' 1: atrous_rate, 2: dilation_rate
		Private ReadOnly LAYER_FIELD_ZERO_PADDING As String = "padding"
		Private ReadOnly LAYER_FIELD_CROPPING As String = "cropping"
		Private ReadOnly LAYER_FIELD_3D_KERNEL_1 As String = "kernel_dim1" ' keras 1 only
		Private ReadOnly LAYER_FIELD_3D_KERNEL_2 As String = "kernel_dim2" ' keras 1 only
		Private ReadOnly LAYER_FIELD_3D_KERNEL_3 As String = "kernel_dim3" ' keras 1 only


		' Pooling / Upsampling layer properties 
		Private ReadOnly LAYER_FIELD_POOL_STRIDES As String = "strides"
		Private ReadOnly LAYER_FIELD_POOL_1D_SIZE As String = "" ' 1: pool_length, 2: pool_size
		Private ReadOnly LAYER_FIELD_POOL_1D_STRIDES As String = "" ' 1: stride, 2: strides
		Private ReadOnly LAYER_FIELD_UPSAMPLING_1D_SIZE As String = "" ' 1: length, 2: size
		Private ReadOnly LAYER_FIELD_UPSAMPLING_2D_SIZE As String = "size"
		Private ReadOnly LAYER_FIELD_UPSAMPLING_3D_SIZE As String = "size"


		' Keras convolution border modes. 
		Private ReadOnly LAYER_FIELD_BORDER_MODE As String = "" ' 1: border_mode, 2: padding
		Private ReadOnly LAYER_BORDER_MODE_SAME As String = "same"
		Private ReadOnly LAYER_BORDER_MODE_VALID As String = "valid"
		Private ReadOnly LAYER_BORDER_MODE_FULL As String = "full"
		Private ReadOnly LAYER_BORDER_MODE_CAUSAL As String = "causal"

		' Noise layers 
		Private ReadOnly LAYER_FIELD_RATE As String = "rate"
		Private ReadOnly LAYER_FIELD_GAUSSIAN_VARIANCE As String = "" ' 1: sigma, 2: stddev

		' Layer wrappers 
		' Missing: TimeDistributed


		' Keras weight regularizers. 
		Private ReadOnly LAYER_FIELD_W_REGULARIZER As String = "" ' 1: W_regularizer, 2: kernel_regularizer
		Private ReadOnly LAYER_FIELD_B_REGULARIZER As String = "" ' 1: b_regularizer, 2: bias_regularizer
		Private ReadOnly REGULARIZATION_TYPE_L1 As String = "l1"
		Private ReadOnly REGULARIZATION_TYPE_L2 As String = "l2"

		' Keras constraints 
		Private ReadOnly LAYER_FIELD_MINMAX_NORM_CONSTRAINT As String = "MinMaxNorm"
		Private ReadOnly LAYER_FIELD_MINMAX_NORM_CONSTRAINT_ALIAS As String = "min_max_norm"
		Private ReadOnly LAYER_FIELD_MAX_NORM_CONSTRAINT As String = "MaxNorm"
		Private ReadOnly LAYER_FIELD_MAX_NORM_CONSTRAINT_ALIAS As String = "max_norm"
		Private ReadOnly LAYER_FIELD_MAX_NORM_CONSTRAINT_ALIAS_2 As String = "maxnorm"
		Private ReadOnly LAYER_FIELD_NON_NEG_CONSTRAINT As String = "NonNeg"
		Private ReadOnly LAYER_FIELD_NON_NEG_CONSTRAINT_ALIAS As String = "nonneg"
		Private ReadOnly LAYER_FIELD_NON_NEG_CONSTRAINT_ALIAS_2 As String = "non_neg"
		Private ReadOnly LAYER_FIELD_UNIT_NORM_CONSTRAINT As String = "UnitNorm"
		Private ReadOnly LAYER_FIELD_UNIT_NORM_CONSTRAINT_ALIAS As String = "unitnorm"
		Private ReadOnly LAYER_FIELD_UNIT_NORM_CONSTRAINT_ALIAS_2 As String = "unit_norm"
		Private ReadOnly LAYER_FIELD_CONSTRAINT_NAME As String = "" ' 1: name, 2: class_name
		Private ReadOnly LAYER_FIELD_W_CONSTRAINT As String = "" ' 1: W_constraint, 2: kernel_constraint
		Private ReadOnly LAYER_FIELD_B_CONSTRAINT As String = "" ' 1: b_constraint, 2: bias_constraint
		Private ReadOnly LAYER_FIELD_MAX_CONSTRAINT As String = "" ' 1: m, 2: max_value
		Private ReadOnly LAYER_FIELD_MINMAX_MIN_CONSTRAINT As String = "" ' 1: low, 2: min_value
		Private ReadOnly LAYER_FIELD_MINMAX_MAX_CONSTRAINT As String = "" ' 1: high, 2: max_value
		Private ReadOnly LAYER_FIELD_CONSTRAINT_DIM As String = "axis"
		Private ReadOnly LAYER_FIELD_CONSTRAINT_RATE As String = "rate"


		' Keras weight initializers. 
		Private ReadOnly LAYER_FIELD_INIT As String = "" ' 1: init, 2: kernel_initializer
		Private ReadOnly LAYER_FIELD_BIAS_INIT As String = "bias_initializer" ' keras 2 only
		Private ReadOnly LAYER_FIELD_INIT_MEAN As String = "mean"
		Private ReadOnly LAYER_FIELD_INIT_STDDEV As String = "stddev"
		Private ReadOnly LAYER_FIELD_INIT_SCALE As String = "scale"
		Private ReadOnly LAYER_FIELD_INIT_MINVAL As String = "minval"
		Private ReadOnly LAYER_FIELD_INIT_MAXVAL As String = "maxval"
		Private ReadOnly LAYER_FIELD_INIT_VALUE As String = "value"
		Private ReadOnly LAYER_FIELD_INIT_GAIN As String = "gain"
		Private ReadOnly LAYER_FIELD_INIT_MODE As String = "mode"
		Private ReadOnly LAYER_FIELD_INIT_DISTRIBUTION As String = "distribution"

		Private ReadOnly INIT_UNIFORM As String = "uniform"
		Private ReadOnly INIT_RANDOM_UNIFORM As String = "random_uniform"
		Private ReadOnly INIT_RANDOM_UNIFORM_ALIAS As String = "RandomUniform"
		Private ReadOnly INIT_ZERO As String = "zero"
		Private ReadOnly INIT_ZEROS As String = "zeros"
		Private ReadOnly INIT_ZEROS_ALIAS As String = "Zeros"
		Private ReadOnly INIT_ONE As String = "one"
		Private ReadOnly INIT_ONES As String = "ones"
		Private ReadOnly INIT_ONES_ALIAS As String = "Ones"
		Private ReadOnly INIT_CONSTANT As String = "constant"
		Private ReadOnly INIT_CONSTANT_ALIAS As String = "Constant"
		Private ReadOnly INIT_TRUNCATED_NORMAL As String = "truncated_normal"
		Private ReadOnly INIT_TRUNCATED_NORMAL_ALIAS As String = "TruncatedNormal"
		Private ReadOnly INIT_GLOROT_NORMAL As String = "glorot_normal"
		Private ReadOnly INIT_GLOROT_NORMAL_ALIAS As String = "GlorotNormal"
		Private ReadOnly INIT_GLOROT_UNIFORM As String = "glorot_uniform"
		Private ReadOnly INIT_GLOROT_UNIFORM_ALIAS As String = "GlorotUniform"
		Private ReadOnly INIT_HE_NORMAL As String = "he_normal"
		Private ReadOnly INIT_HE_NORMAL_ALIAS As String = "HeNormal"
		Private ReadOnly INIT_HE_UNIFORM As String = "he_uniform"
		Private ReadOnly INIT_HE_UNIFORM_ALIAS As String = "HeUniform"
		Private ReadOnly INIT_LECUN_UNIFORM As String = "lecun_uniform"
		Private ReadOnly INIT_LECUN_UNIFORM_ALIAS As String = "LecunUniform"
		Private ReadOnly INIT_LECUN_NORMAL As String = "lecun_normal"
		Private ReadOnly INIT_LECUN_NORMAL_ALIAS As String = "LecunNormal"
		Private ReadOnly INIT_NORMAL As String = "normal"
		Private ReadOnly INIT_RANDOM_NORMAL As String = "random_normal"
		Private ReadOnly INIT_RANDOM_NORMAL_ALIAS As String = "RandomNormal"
		Private ReadOnly INIT_ORTHOGONAL As String = "orthogonal"
		Private ReadOnly INIT_ORTHOGONAL_ALIAS As String = "Orthogonal"
		Private ReadOnly INIT_IDENTITY As String = "identity"
		Private ReadOnly INIT_IDENTITY_ALIAS As String = "Identity"
		Private ReadOnly INIT_VARIANCE_SCALING As String = "VarianceScaling" ' keras 2 only


		' Keras and DL4J activation types. 
		Private ReadOnly LAYER_FIELD_ACTIVATION As String = "activation"

		Private ReadOnly KERAS_ACTIVATION_SOFTMAX As String = "softmax"
		Private ReadOnly KERAS_ACTIVATION_SOFTPLUS As String = "softplus"
		Private ReadOnly KERAS_ACTIVATION_SOFTSIGN As String = "softsign"
		Private ReadOnly KERAS_ACTIVATION_RELU As String = "relu"
		Private ReadOnly KERAS_ACTIVATION_RELU6 As String = "relu6"
		Private ReadOnly KERAS_ACTIVATION_TANH As String = "tanh"
		Private ReadOnly KERAS_ACTIVATION_SIGMOID As String = "sigmoid"
		Private ReadOnly KERAS_ACTIVATION_HARD_SIGMOID As String = "hard_sigmoid"
		Private ReadOnly KERAS_ACTIVATION_LINEAR As String = "linear"
		Private ReadOnly KERAS_ACTIVATION_SWISH As String = "swish"
		Private ReadOnly KERAS_ACTIVATION_ELU As String = "elu" ' keras 2 only
		Private ReadOnly KERAS_ACTIVATION_SELU As String = "selu" ' keras 2 only

		' Keras loss functions. 
		Private ReadOnly KERAS_LOSS_MEAN_SQUARED_ERROR As String = "mean_squared_error"
		Private ReadOnly KERAS_LOSS_MSE As String = "mse"
		Private ReadOnly KERAS_LOSS_MEAN_ABSOLUTE_ERROR As String = "mean_absolute_error"
		Private ReadOnly KERAS_LOSS_MAE As String = "mae"
		Private ReadOnly KERAS_LOSS_MEAN_ABSOLUTE_PERCENTAGE_ERROR As String = "mean_absolute_percentage_error"
		Private ReadOnly KERAS_LOSS_MAPE As String = "mape"
		Private ReadOnly KERAS_LOSS_MEAN_SQUARED_LOGARITHMIC_ERROR As String = "mean_squared_logarithmic_error"
		Private ReadOnly KERAS_LOSS_MSLE As String = "msle"
		Private ReadOnly KERAS_LOSS_SQUARED_HINGE As String = "squared_hinge"
		Private ReadOnly KERAS_LOSS_HINGE As String = "hinge"
		Private ReadOnly KERAS_LOSS_CATEGORICAL_HINGE As String = "categorical_hinge" ' keras 2 only
		Private ReadOnly KERAS_LOSS_BINARY_CROSSENTROPY As String = "binary_crossentropy"
		Private ReadOnly KERAS_LOSS_CATEGORICAL_CROSSENTROPY As String = "categorical_crossentropy"
		Private ReadOnly KERAS_LOSS_SPARSE_CATEGORICAL_CROSSENTROPY As String = "sparse_categorical_crossentropy"
		Private ReadOnly KERAS_LOSS_KULLBACK_LEIBLER_DIVERGENCE As String = "kullback_leibler_divergence"
		Private ReadOnly KERAS_LOSS_KLD As String = "kld"
		Private ReadOnly KERAS_LOSS_POISSON As String = "poisson"
		Private ReadOnly KERAS_LOSS_COSINE_PROXIMITY As String = "cosine_proximity"
		Private ReadOnly KERAS_LOSS_LOG_COSH As String = "logcosh" ' keras 2 only

	End Class
End Namespace