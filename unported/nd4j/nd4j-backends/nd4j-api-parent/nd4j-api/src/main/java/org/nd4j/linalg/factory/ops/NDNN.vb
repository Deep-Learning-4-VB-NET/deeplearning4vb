import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PadMode = org.nd4j.enums.PadMode
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.linalg.factory.ops

	Public Class NDNN
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Concatenates a ReLU which selects only the positive part of the activation with a ReLU which selects only the negative part of the activation. Note that as a result this non-linearity doubles the depth of the activations.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cReLU(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("CReLU", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CReLU(x))(0)
	  End Function

	  ''' <summary>
	  ''' Neural network batch normalization operation.<br>
	  ''' For details, see <a href="https://arxiv.org/abs/1502.03167">https://arxiv.org/abs/1502.03167</a><br>
	  ''' </summary>
	  ''' <param name="input"> Input variable. (NUMERIC type) </param>
	  ''' <param name="mean"> Mean value. For 1d axis, this should match input.size(axis) (NUMERIC type) </param>
	  ''' <param name="variance"> Variance value. For 1d axis, this should match input.size(axis) (NUMERIC type) </param>
	  ''' <param name="gamma"> Gamma value. For 1d axis, this should match input.size(axis) (NUMERIC type) </param>
	  ''' <param name="beta"> Beta value. For 1d axis, this should match input.size(axis) (NUMERIC type) </param>
	  ''' <param name="epsilon"> Epsilon constant for numerical stability (to avoid division by 0) </param>
	  ''' <param name="axis"> For 2d CNN activations: 1 for NCHW format activations, or 3 for NHWC format activations.
	  ''' For 3d CNN activations: 1 for NCDHW format, 4 for NDHWC
	  ''' For 1d/RNN activations: 1 for NCW format, 2 for NWC (Size: AtLeast(min=1)) </param>
	  ''' <returns> output variable for batch normalization (NUMERIC type) </returns>
	  Public Overridable Function batchNorm(ByVal input As INDArray, ByVal mean As INDArray, ByVal variance As INDArray, ByVal gamma As INDArray, ByVal beta As INDArray, ByVal epsilon As Double, ParamArray ByVal axis() As Integer) As INDArray
		NDValidation.validateNumerical("batchNorm", "input", input)
		NDValidation.validateNumerical("batchNorm", "mean", mean)
		NDValidation.validateNumerical("batchNorm", "variance", variance)
		NDValidation.validateNumerical("batchNorm", "gamma", gamma)
		NDValidation.validateNumerical("batchNorm", "beta", beta)
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNorm(input, mean, variance, gamma, beta, epsilon, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Bias addition operation: a special case of addition, typically used with CNN 4D activations and a 1D bias vector<br>
	  ''' </summary>
	  ''' <param name="input"> 4d input variable (NUMERIC type) </param>
	  ''' <param name="bias"> 1d bias (NUMERIC type) </param>
	  ''' <param name="nchw"> The format - nchw=true means [minibatch, channels, height, width] format; nchw=false - [minibatch, height, width, channels].
	  ''' Unused for 2d inputs </param>
	  ''' <returns> output Output variable, after applying bias add operation (NUMERIC type) </returns>
	  Public Overridable Function biasAdd(ByVal input As INDArray, ByVal bias As INDArray, ByVal nchw As Boolean) As INDArray
		NDValidation.validateNumerical("biasAdd", "input", input)
		NDValidation.validateNumerical("biasAdd", "bias", bias)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.broadcast.BiasAdd(input, bias, nchw))(0)
	  End Function

	  ''' <summary>
	  ''' This operation performs dot product attention on the given timeseries input with the given queries<br>
	  ''' out = sum(similarity(k_i, q) * v_i)<br>
	  ''' <br>
	  ''' similarity(k, q) = softmax(k * q) where x * q is the dot product of x and q<br>
	  ''' <br>
	  ''' Optionally with normalization step:<br>
	  ''' similarity(k, q) = softmax(k * q / sqrt(size(q))<br>
	  ''' <br>
	  ''' See also "Attention is all you need" (https://arxiv.org/abs/1706.03762, p. 4, eq. 1)<br>
	  ''' <br>
	  ''' Note: This supports multiple queries at once, if only one query is available the queries vector still has to<br>
	  ''' be 3D but can have queryCount = 1<br>
	  ''' <br>
	  ''' Note: keys and values usually is the same array. If you want to use it as the same array, simply pass it for<br>
	  ''' both.<br>
	  ''' <br>
	  ''' Note: Queries, keys and values must either be all rank 3 or all rank 4 arrays. Mixing them doesn't work. The<br>
	  ''' output rank will depend on the input rank.<br>
	  ''' </summary>
	  ''' <param name="queries"> input 3D array "queries" of shape [batchSize, featureKeys, queryCount]
	  ''' or 4D array of shape [batchSize, numHeads, featureKeys, queryCount] (NUMERIC type) </param>
	  ''' <param name="keys"> input 3D array "keys" of shape [batchSize, featureKeys, timesteps]
	  ''' or 4D array of shape [batchSize, numHeads, featureKeys, timesteps] (NUMERIC type) </param>
	  ''' <param name="values"> input 3D array "values" of shape [batchSize, featureValues, timesteps]
	  ''' or 4D array of shape [batchSize, numHeads, featureValues, timesteps] (NUMERIC type) </param>
	  ''' <param name="mask"> OPTIONAL; array that defines which values should be skipped of shape [batchSize, timesteps] (NUMERIC type) </param>
	  ''' <param name="scaled"> normalization, false -> do not apply normalization, true -> apply normalization </param>
	  ''' <returns> output  Attention result arrays of shape [batchSize, featureValues, queryCount] or [batchSize, numHeads, featureValues, queryCount],
	  ''' (optionally) Attention Weights of shape [batchSize, timesteps, queryCount] or [batchSize, numHeads, timesteps, queryCount] (NUMERIC type) </returns>
	  Public Overridable Function dotProductAttention(ByVal queries As INDArray, ByVal keys As INDArray, ByVal values As INDArray, ByVal mask As INDArray, ByVal scaled As Boolean) As INDArray
		NDValidation.validateNumerical("dotProductAttention", "queries", queries)
		NDValidation.validateNumerical("dotProductAttention", "keys", keys)
		NDValidation.validateNumerical("dotProductAttention", "values", values)
		NDValidation.validateNumerical("dotProductAttention", "mask", mask)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.DotProductAttention(queries, keys, values, mask, scaled, False))(0)
	  End Function

	  ''' <summary>
	  ''' Dropout operation<br>
	  ''' </summary>
	  ''' <param name="input"> Input array (NUMERIC type) </param>
	  ''' <param name="inputRetainProbability"> Probability of retaining an input (set to 0 with probability 1-p) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function dropout(ByVal input As INDArray, ByVal inputRetainProbability As Double) As INDArray
		NDValidation.validateNumerical("dropout", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.random.impl.DropOut(input, inputRetainProbability))
	  End Function

	  ''' <summary>
	  ''' Element-wise exponential linear unit (ELU) function:<br>
	  ''' out = x if x > 0<br>
	  ''' out = a * (exp(x) - 1) if x <= 0<br>
	  ''' with constant a = 1.0<br>
	  ''' <para><br>
	  ''' See: <a href="https://arxiv.org/abs/1511.07289">https://arxiv.org/abs/1511.07289</a><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function elu(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("elu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ELU(x))(0)
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the sigmoid approximation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function gelu(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("gelu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.GELU(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise hard sigmoid function:<br>
	  ''' out[i] = 0 if in[i] <= -2.5<br>
	  ''' out[1] = 0.2*in[i]+0.5 if -2.5 < in[i] < 2.5<br>
	  ''' out[i] = 1 if in[i] >= 2.5<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardSigmoid(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("hardSigmoid", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.HardSigmoid(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise hard tanh function:<br>
	  ''' out[i] = -1 if in[i] <= -1<br>
	  ''' out[1] = in[i] if -1 < in[i] < 1<br>
	  ''' out[i] = 1 if in[i] >= 1<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("hardTanh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.HardTanh(x))
	  End Function

	  ''' <summary>
	  ''' Derivative (dOut/dIn) of the element-wise hard Tanh function - hardTanh(INDArray)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanhDerivative(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("hardTanhDerivative", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhDerivative(x))
	  End Function

	  ''' <summary>
	  ''' Apply Layer Normalization<br>
	  ''' <br>
	  ''' y = gain * standardize(x) + bias<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="gain"> Gain (NUMERIC type) </param>
	  ''' <param name="bias"> Bias (NUMERIC type) </param>
	  ''' <param name="channelsFirst"> For 2D input - unused. True for NCHW (minibatch, channels, height, width), false for NHWC data </param>
	  ''' <param name="dimensions"> Dimensions to perform layer norm over - dimension=1 for 2d/MLP data, dimension=1,2,3 for CNNs (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function layerNorm(ByVal input As INDArray, ByVal gain As INDArray, ByVal bias As INDArray, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("layerNorm", "input", input)
		NDValidation.validateNumerical("layerNorm", "gain", gain)
		NDValidation.validateNumerical("layerNorm", "bias", bias)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(input, gain, bias, channelsFirst, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Apply Layer Normalization<br>
	  ''' <br>
	  ''' y = gain * standardize(x) + bias<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="gain"> Gain (NUMERIC type) </param>
	  ''' <param name="channelsFirst"> For 2D input - unused. True for NCHW (minibatch, channels, height, width), false for NHWC data </param>
	  ''' <param name="dimensions"> Dimensions to perform layer norm over - dimension=1 for 2d/MLP data, dimension=1,2,3 for CNNs (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function layerNorm(ByVal input As INDArray, ByVal gain As INDArray, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("layerNorm", "input", input)
		NDValidation.validateNumerical("layerNorm", "gain", gain)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(input, gain, Nothing, channelsFirst, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise leaky ReLU function:<br>
	  ''' out = x if x >= 0.0<br>
	  ''' out = alpha * x if x < cutoff<br>
	  ''' Alpha value is most commonly set to 0.01<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="alpha"> Cutoff - commonly 0.01 </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function leakyRelu(ByVal x As INDArray, ByVal alpha As Double) As INDArray
		NDValidation.validateNumerical("leakyRelu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU(x, alpha))
	  End Function

	  ''' <summary>
	  ''' Leaky ReLU derivative: dOut/dIn given input.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="alpha"> Cutoff - commonly 0.01 </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function leakyReluDerivative(ByVal x As INDArray, ByVal alpha As Double) As INDArray
		NDValidation.validateNumerical("leakyReluDerivative", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative(x, alpha))
	  End Function

	  ''' <summary>
	  ''' Linear layer operation: out = mmul(in,w) + bias<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable, shape [nIn, nOut] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function linear(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray) As INDArray
		NDValidation.validateNumerical("linear", "input", input)
		NDValidation.validateNumerical("linear", "weights", weights)
		NDValidation.validateNumerical("linear", "bias", bias)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.XwPlusB(input, weights, bias))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = log(sigmoid(in[i]))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSigmoid(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("logSigmoid", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.LogSigmoid(x))
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="x">  (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("logSoftmax", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(x))(0)
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply log softmax </param>
	  ''' <returns> output Output - log(softmax(input)) (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal x As INDArray, ByVal dimension As Integer) As INDArray
		NDValidation.validateNumerical("logSoftmax", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(x, dimension))(0)
	  End Function

	  ''' <summary>
	  ''' This performs multi-headed dot product attention on the given timeseries input<br>
	  ''' out = concat(head_1, head_2, ..., head_n) * Wo<br>
	  ''' head_i = dot_product_attention(Wq_i*q, Wk_i*k, Wv_i*v)<br>
	  ''' <br>
	  ''' Optionally with normalization when calculating the attention for each head.<br>
	  ''' <br>
	  ''' See also "Attention is all you need" (https://arxiv.org/abs/1706.03762, pp. 4,5, "3.2.2 Multi-Head Attention")<br>
	  ''' <br>
	  ''' This makes use of dot_product_attention OP support for rank 4 inputs.<br>
	  ''' see dotProductAttention(INDArray, INDArray, INDArray, INDArray, boolean, boolean)<br>
	  ''' </summary>
	  ''' <param name="queries"> input 3D array "queries" of shape [batchSize, featureKeys, queryCount] (NUMERIC type) </param>
	  ''' <param name="keys"> input 3D array "keys" of shape [batchSize, featureKeys, timesteps] (NUMERIC type) </param>
	  ''' <param name="values"> input 3D array "values" of shape [batchSize, featureValues, timesteps] (NUMERIC type) </param>
	  ''' <param name="Wq"> input query projection weights of shape [numHeads, projectedKeys, featureKeys] (NUMERIC type) </param>
	  ''' <param name="Wk"> input key projection weights of shape [numHeads, projectedKeys, featureKeys] (NUMERIC type) </param>
	  ''' <param name="Wv"> input value projection weights of shape [numHeads, projectedValues, featureValues] (NUMERIC type) </param>
	  ''' <param name="Wo"> output projection weights of shape [numHeads * projectedValues, outSize] (NUMERIC type) </param>
	  ''' <param name="mask"> OPTIONAL; array that defines which values should be skipped of shape [batchSize, timesteps] (NUMERIC type) </param>
	  ''' <param name="scaled"> normalization, false -> do not apply normalization, true -> apply normalization </param>
	  ''' <returns> output Attention result arrays of shape [batchSize, outSize, queryCount]
	  ''' (optionally) Attention Weights of shape [batchSize, numHeads, timesteps, queryCount] (NUMERIC type) </returns>
	  Public Overridable Function multiHeadDotProductAttention(ByVal queries As INDArray, ByVal keys As INDArray, ByVal values As INDArray, ByVal Wq As INDArray, ByVal Wk As INDArray, ByVal Wv As INDArray, ByVal Wo As INDArray, ByVal mask As INDArray, ByVal scaled As Boolean) As INDArray
		NDValidation.validateNumerical("multiHeadDotProductAttention", "queries", queries)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "keys", keys)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "values", values)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "Wq", Wq)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "Wk", Wk)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "Wv", Wv)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "Wo", Wo)
		NDValidation.validateNumerical("multiHeadDotProductAttention", "mask", mask)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.MultiHeadDotProductAttention(queries, keys, values, Wq, Wk, Wv, Wo, mask, scaled, False))(0)
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="PadMode"> Padding format </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal input As INDArray, ByVal padding As INDArray, ByVal PadMode As PadMode, ByVal constant As Double) As INDArray
		NDValidation.validateNumerical("pad", "input", input)
		NDValidation.validateNumerical("pad", "padding", padding)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.Pad(input, padding, PadMode, constant))(0)
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal input As INDArray, ByVal padding As INDArray, ByVal constant As Double) As INDArray
		NDValidation.validateNumerical("pad", "input", input)
		NDValidation.validateNumerical("pad", "padding", padding)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.Pad(input, padding, PadMode.CONSTANT, constant))(0)
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the precise method<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function preciseGelu(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("preciseGelu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELU(x))
	  End Function

	  ''' <summary>
	  ''' PReLU (Parameterized Rectified Linear Unit) operation.  Like LeakyReLU with a learnable alpha:<br>
	  ''' out[i] = in[i] if in[i] >= 0<br>
	  ''' out[i] = in[i] * alpha[i] otherwise<br>
	  ''' <br>
	  ''' sharedAxes allows you to share learnable parameters along axes.<br>
	  ''' For example, if the input has shape [batchSize, channels, height, width]<br>
	  ''' and you want each channel to have its own cutoff, use sharedAxes = [2, 3] and an<br>
	  ''' alpha with shape [channels].<br>
	  ''' </summary>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="alpha"> The cutoff variable.  Note that the batch dimension (the 0th, whether it is batch or not) should not be part of alpha. (NUMERIC type) </param>
	  ''' <param name="sharedAxes"> Which axes to share cutoff parameters along. (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function prelu(ByVal input As INDArray, ByVal alpha As INDArray, ParamArray ByVal sharedAxes() As Integer) As INDArray
		NDValidation.validateNumerical("prelu", "input", input)
		NDValidation.validateNumerical("prelu", "alpha", alpha)
		Preconditions.checkArgument(sharedAxes.Length >= 1, "sharedAxes has incorrect size/length. Expected: sharedAxes.length >= 1, got %s", sharedAxes.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.PRelu(input, alpha, sharedAxes))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise rectified linear function with specified cutoff:<br>
	  ''' out[i] = in[i] if in[i] >= cutoff<br>
	  ''' out[i] = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation - x > cutoff ? x : 0. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu(ByVal x As INDArray, ByVal cutoff As Double) As INDArray
		NDValidation.validateNumerical("relu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear(x, cutoff))
	  End Function

	  ''' <summary>
	  ''' Element-wise "rectified linear 6" function with specified cutoff:<br>
	  ''' out[i] = min(max(in, cutoff), 6)<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu6(ByVal x As INDArray, ByVal cutoff As Double) As INDArray
		NDValidation.validateNumerical("relu6", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.Relu6(x, cutoff))
	  End Function

	  ''' <summary>
	  ''' ReLU (Rectified Linear Unit) layer operation: out = relu(mmul(in,w) + bias)<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reluLayer(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray) As INDArray
		NDValidation.validateNumerical("reluLayer", "input", input)
		NDValidation.validateNumerical("reluLayer", "weights", weights)
		NDValidation.validateNumerical("reluLayer", "bias", bias)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.ReluLayer(input, weights, bias))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise SeLU function - Scaled exponential Lineal Unit: see <a href="https://arxiv.org/abs/1706.02515">Self-Normalizing Neural Networks</a><br>
	  ''' <br>
	  ''' out[i] = scale * alpha * (exp(in[i])-1) if in[i]>0, or 0 if in[i] <= 0<br>
	  ''' Uses default scale and alpha values.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function selu(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("selu", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.SELU(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = 1.0/(1+exp(-in[i]))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoid(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("sigmoid", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Sigmoid(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function derivative: dL/dIn given input and dL/dOut<br>
	  ''' </summary>
	  ''' <param name="x"> Input Variable (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at the output - dL/dOut. Must have same shape as the input (NUMERIC type) </param>
	  ''' <returns> output Output (gradient at input of sigmoid) (NUMERIC type) </returns>
	  Public Overridable Function sigmoidDerivative(ByVal x As INDArray, ByVal wrt As INDArray) As INDArray
		NDValidation.validateNumerical("sigmoidDerivative", "x", x)
		NDValidation.validateNumerical("sigmoidDerivative", "wrt", wrt)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.gradient.SigmoidDerivative(x, wrt))(0)
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply softmax </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal x As INDArray, ByVal dimension As Integer) As INDArray
		NDValidation.validateNumerical("softmax", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(x, dimension))(0)
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("softmax", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(x, -1))(0)
	  End Function

	  ''' <summary>
	  ''' Softmax derivative function<br>
	  ''' </summary>
	  ''' <param name="x"> Softmax input (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at output, dL/dx (NUMERIC type) </param>
	  ''' <param name="dimension"> Softmax dimension </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function softmaxDerivative(ByVal x As INDArray, ByVal wrt As INDArray, ByVal dimension As Integer) As INDArray
		NDValidation.validateNumerical("softmaxDerivative", "x", x)
		NDValidation.validateNumerical("softmaxDerivative", "wrt", wrt)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftmaxBp(x, wrt, dimension))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise softplus function: out = log(exp(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softplus(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("softplus", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftPlus(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise softsign function: out = x / (abs(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softsign(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("softsign", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftSign(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise derivative (dOut/dIn) of the softsign function softsign(INDArray)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function softsignDerivative(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("softsignDerivative", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignDerivative(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise "swish" function: out = x * sigmoid(b*x) with b=1.0<br>
	  ''' See: <a href="https://arxiv.org/abs/1710.05941">https://arxiv.org/abs/1710.05941</a><br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function swish(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("swish", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Swish(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise tanh (hyperbolic tangent) operation: out = tanh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tanh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("tanh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Tanh(x))
	  End Function
	End Class

End Namespace