import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PadMode = org.nd4j.enums.PadMode

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

Namespace org.nd4j.autodiff.samediff.ops

	Public Class SDNN
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' Concatenates a ReLU which selects only the positive part of the activation with a ReLU which selects only the negative part of the activation. Note that as a result this non-linearity doubles the depth of the activations.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cReLU(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("CReLU", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CReLU(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Concatenates a ReLU which selects only the positive part of the activation with a ReLU which selects only the negative part of the activation. Note that as a result this non-linearity doubles the depth of the activations.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cReLU(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("CReLU", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CReLU(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function batchNorm(ByVal input As SDVariable, ByVal mean As SDVariable, ByVal variance As SDVariable, ByVal gamma As SDVariable, ByVal beta As SDVariable, ByVal epsilon As Double, ParamArray ByVal axis() As Integer) As SDVariable
		SDValidation.validateNumerical("batchNorm", "input", input)
		SDValidation.validateNumerical("batchNorm", "mean", mean)
		SDValidation.validateNumerical("batchNorm", "variance", variance)
		SDValidation.validateNumerical("batchNorm", "gamma", gamma)
		SDValidation.validateNumerical("batchNorm", "beta", beta)
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return (New org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNorm(sd,input, mean, variance, gamma, beta, epsilon, axis)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Neural network batch normalization operation.<br>
	  ''' For details, see <a href="https://arxiv.org/abs/1502.03167">https://arxiv.org/abs/1502.03167</a><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
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
	  Public Overridable Function batchNorm(ByVal name As String, ByVal input As SDVariable, ByVal mean As SDVariable, ByVal variance As SDVariable, ByVal gamma As SDVariable, ByVal beta As SDVariable, ByVal epsilon As Double, ParamArray ByVal axis() As Integer) As SDVariable
		SDValidation.validateNumerical("batchNorm", "input", input)
		SDValidation.validateNumerical("batchNorm", "mean", mean)
		SDValidation.validateNumerical("batchNorm", "variance", variance)
		SDValidation.validateNumerical("batchNorm", "gamma", gamma)
		SDValidation.validateNumerical("batchNorm", "beta", beta)
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.layers.convolution.BatchNorm(sd,input, mean, variance, gamma, beta, epsilon, axis)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bias addition operation: a special case of addition, typically used with CNN 4D activations and a 1D bias vector<br>
	  ''' </summary>
	  ''' <param name="input"> 4d input variable (NUMERIC type) </param>
	  ''' <param name="bias"> 1d bias (NUMERIC type) </param>
	  ''' <param name="nchw"> The format - nchw=true means [minibatch, channels, height, width] format; nchw=false - [minibatch, height, width, channels].
	  ''' Unused for 2d inputs </param>
	  ''' <returns> output Output variable, after applying bias add operation (NUMERIC type) </returns>
	  Public Overridable Function biasAdd(ByVal input As SDVariable, ByVal bias As SDVariable, ByVal nchw As Boolean) As SDVariable
		SDValidation.validateNumerical("biasAdd", "input", input)
		SDValidation.validateNumerical("biasAdd", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.broadcast.BiasAdd(sd,input, bias, nchw)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bias addition operation: a special case of addition, typically used with CNN 4D activations and a 1D bias vector<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> 4d input variable (NUMERIC type) </param>
	  ''' <param name="bias"> 1d bias (NUMERIC type) </param>
	  ''' <param name="nchw"> The format - nchw=true means [minibatch, channels, height, width] format; nchw=false - [minibatch, height, width, channels].
	  ''' Unused for 2d inputs </param>
	  ''' <returns> output Output variable, after applying bias add operation (NUMERIC type) </returns>
	  Public Overridable Function biasAdd(ByVal name As String, ByVal input As SDVariable, ByVal bias As SDVariable, ByVal nchw As Boolean) As SDVariable
		SDValidation.validateNumerical("biasAdd", "input", input)
		SDValidation.validateNumerical("biasAdd", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.broadcast.BiasAdd(sd,input, bias, nchw)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function dotProductAttention(ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean) As SDVariable
		SDValidation.validateNumerical("dotProductAttention", "queries", queries)
		SDValidation.validateNumerical("dotProductAttention", "keys", keys)
		SDValidation.validateNumerical("dotProductAttention", "values", values)
		SDValidation.validateNumerical("dotProductAttention", "mask", mask)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.DotProductAttention(sd,queries, keys, values, mask, scaled, False)).outputVariable()
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
	  ''' <param name="name"> name May be null. Name for the output variable </param>
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
	  Public Overridable Function dotProductAttention(ByVal name As String, ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean) As SDVariable
		SDValidation.validateNumerical("dotProductAttention", "queries", queries)
		SDValidation.validateNumerical("dotProductAttention", "keys", keys)
		SDValidation.validateNumerical("dotProductAttention", "values", values)
		SDValidation.validateNumerical("dotProductAttention", "mask", mask)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.DotProductAttention(sd,queries, keys, values, mask, scaled, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Dropout operation<br>
	  ''' </summary>
	  ''' <param name="input"> Input array (NUMERIC type) </param>
	  ''' <param name="inputRetainProbability"> Probability of retaining an input (set to 0 with probability 1-p) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function dropout(ByVal input As SDVariable, ByVal inputRetainProbability As Double) As SDVariable
		SDValidation.validateNumerical("dropout", "input", input)
		Return (New org.nd4j.linalg.api.ops.random.impl.DropOut(sd,input, inputRetainProbability)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Dropout operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input array (NUMERIC type) </param>
	  ''' <param name="inputRetainProbability"> Probability of retaining an input (set to 0 with probability 1-p) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function dropout(ByVal name As String, ByVal input As SDVariable, ByVal inputRetainProbability As Double) As SDVariable
		SDValidation.validateNumerical("dropout", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.DropOut(sd,input, inputRetainProbability)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function elu(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("elu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ELU(sd,x)).outputVariable()
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
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function elu(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("elu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ELU(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the sigmoid approximation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function gelu(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("gelu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.GELU(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the sigmoid approximation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function gelu(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("gelu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.GELU(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise hard sigmoid function:<br>
	  ''' out[i] = 0 if in[i] <= -2.5<br>
	  ''' out[1] = 0.2*in[i]+0.5 if -2.5 < in[i] < 2.5<br>
	  ''' out[i] = 1 if in[i] >= 2.5<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardSigmoid(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardSigmoid", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.HardSigmoid(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise hard sigmoid function:<br>
	  ''' out[i] = 0 if in[i] <= -2.5<br>
	  ''' out[1] = 0.2*in[i]+0.5 if -2.5 < in[i] < 2.5<br>
	  ''' out[i] = 1 if in[i] >= 2.5<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardSigmoid(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardSigmoid", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.HardSigmoid(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise hard tanh function:<br>
	  ''' out[i] = -1 if in[i] <= -1<br>
	  ''' out[1] = in[i] if -1 < in[i] < 1<br>
	  ''' out[i] = 1 if in[i] >= 1<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardTanh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.HardTanh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise hard tanh function:<br>
	  ''' out[i] = -1 if in[i] <= -1<br>
	  ''' out[1] = in[i] if -1 < in[i] < 1<br>
	  ''' out[i] = 1 if in[i] >= 1<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardTanh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.HardTanh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Derivative (dOut/dIn) of the element-wise hard Tanh function - hardTanh(INDArray)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanhDerivative(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardTanhDerivative", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhDerivative(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Derivative (dOut/dIn) of the element-wise hard Tanh function - hardTanh(INDArray)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hardTanhDerivative(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("hardTanhDerivative", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhDerivative(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function layerNorm(ByVal input As SDVariable, ByVal gain As SDVariable, ByVal bias As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("layerNorm", "input", input)
		SDValidation.validateNumerical("layerNorm", "gain", gain)
		SDValidation.validateNumerical("layerNorm", "bias", bias)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(sd,input, gain, bias, channelsFirst, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Apply Layer Normalization<br>
	  ''' <br>
	  ''' y = gain * standardize(x) + bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="gain"> Gain (NUMERIC type) </param>
	  ''' <param name="bias"> Bias (NUMERIC type) </param>
	  ''' <param name="channelsFirst"> For 2D input - unused. True for NCHW (minibatch, channels, height, width), false for NHWC data </param>
	  ''' <param name="dimensions"> Dimensions to perform layer norm over - dimension=1 for 2d/MLP data, dimension=1,2,3 for CNNs (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function layerNorm(ByVal name As String, ByVal input As SDVariable, ByVal gain As SDVariable, ByVal bias As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("layerNorm", "input", input)
		SDValidation.validateNumerical("layerNorm", "gain", gain)
		SDValidation.validateNumerical("layerNorm", "bias", bias)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(sd,input, gain, bias, channelsFirst, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function layerNorm(ByVal input As SDVariable, ByVal gain As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("layerNorm", "input", input)
		SDValidation.validateNumerical("layerNorm", "gain", gain)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(sd,input, gain, Nothing, channelsFirst, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Apply Layer Normalization<br>
	  ''' <br>
	  ''' y = gain * standardize(x) + bias<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="gain"> Gain (NUMERIC type) </param>
	  ''' <param name="channelsFirst"> For 2D input - unused. True for NCHW (minibatch, channels, height, width), false for NHWC data </param>
	  ''' <param name="dimensions"> Dimensions to perform layer norm over - dimension=1 for 2d/MLP data, dimension=1,2,3 for CNNs (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function layerNorm(ByVal name As String, ByVal input As SDVariable, ByVal gain As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("layerNorm", "input", input)
		SDValidation.validateNumerical("layerNorm", "gain", gain)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm(sd,input, gain, Nothing, channelsFirst, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function leakyRelu(ByVal x As SDVariable, ByVal alpha As Double) As SDVariable
		SDValidation.validateNumerical("leakyRelu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU(sd,x, alpha)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise leaky ReLU function:<br>
	  ''' out = x if x >= 0.0<br>
	  ''' out = alpha * x if x < cutoff<br>
	  ''' Alpha value is most commonly set to 0.01<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="alpha"> Cutoff - commonly 0.01 </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function leakyRelu(ByVal name As String, ByVal x As SDVariable, ByVal alpha As Double) As SDVariable
		SDValidation.validateNumerical("leakyRelu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU(sd,x, alpha)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Leaky ReLU derivative: dOut/dIn given input.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="alpha"> Cutoff - commonly 0.01 </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function leakyReluDerivative(ByVal x As SDVariable, ByVal alpha As Double) As SDVariable
		SDValidation.validateNumerical("leakyReluDerivative", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative(sd,x, alpha)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Leaky ReLU derivative: dOut/dIn given input.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="alpha"> Cutoff - commonly 0.01 </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function leakyReluDerivative(ByVal name As String, ByVal x As SDVariable, ByVal alpha As Double) As SDVariable
		SDValidation.validateNumerical("leakyReluDerivative", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUDerivative(sd,x, alpha)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Linear layer operation: out = mmul(in,w) + bias<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable, shape [nIn, nOut] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function linear(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable) As SDVariable
		SDValidation.validateNumerical("linear", "input", input)
		SDValidation.validateNumerical("linear", "weights", weights)
		SDValidation.validateNumerical("linear", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.XwPlusB(sd,input, weights, bias)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Linear layer operation: out = mmul(in,w) + bias<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable, shape [nIn, nOut] (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function linear(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable) As SDVariable
		SDValidation.validateNumerical("linear", "input", input)
		SDValidation.validateNumerical("linear", "weights", weights)
		SDValidation.validateNumerical("linear", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.XwPlusB(sd,input, weights, bias)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = log(sigmoid(in[i]))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSigmoid(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("logSigmoid", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.LogSigmoid(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = log(sigmoid(in[i]))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSigmoid(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("logSigmoid", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.LogSigmoid(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="x">  (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("logSoftmax", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x">  (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("logSoftmax", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply log softmax </param>
	  ''' <returns> output Output - log(softmax(input)) (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal x As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("logSoftmax", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(sd,x, dimension)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Log softmax activation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply log softmax </param>
	  ''' <returns> output Output - log(softmax(input)) (NUMERIC type) </returns>
	  Public Overridable Function logSoftmax(ByVal name As String, ByVal x As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("logSoftmax", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.LogSoftMax(sd,x, dimension)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function multiHeadDotProductAttention(ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal Wq As SDVariable, ByVal Wk As SDVariable, ByVal Wv As SDVariable, ByVal Wo As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean) As SDVariable
		SDValidation.validateNumerical("multiHeadDotProductAttention", "queries", queries)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "keys", keys)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "values", values)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wq", Wq)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wk", Wk)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wv", Wv)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wo", Wo)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "mask", mask)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.MultiHeadDotProductAttention(sd,queries, keys, values, Wq, Wk, Wv, Wo, mask, scaled, False)).outputVariable()
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
	  ''' <param name="name"> name May be null. Name for the output variable </param>
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
	  Public Overridable Function multiHeadDotProductAttention(ByVal name As String, ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal Wq As SDVariable, ByVal Wk As SDVariable, ByVal Wv As SDVariable, ByVal Wo As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean) As SDVariable
		SDValidation.validateNumerical("multiHeadDotProductAttention", "queries", queries)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "keys", keys)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "values", values)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wq", Wq)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wk", Wk)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wv", Wv)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "Wo", Wo)
		SDValidation.validateNumerical("multiHeadDotProductAttention", "mask", mask)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.MultiHeadDotProductAttention(sd,queries, keys, values, Wq, Wk, Wv, Wo, mask, scaled, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="PadMode"> Padding format </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal input As SDVariable, ByVal padding As SDVariable, ByVal PadMode As PadMode, ByVal constant As Double) As SDVariable
		SDValidation.validateNumerical("pad", "input", input)
		SDValidation.validateNumerical("pad", "padding", padding)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.Pad(sd,input, padding, PadMode, constant)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="PadMode"> Padding format </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal name As String, ByVal input As SDVariable, ByVal padding As SDVariable, ByVal PadMode As PadMode, ByVal constant As Double) As SDVariable
		SDValidation.validateNumerical("pad", "input", input)
		SDValidation.validateNumerical("pad", "padding", padding)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.Pad(sd,input, padding, PadMode, constant)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal input As SDVariable, ByVal padding As SDVariable, ByVal constant As Double) As SDVariable
		SDValidation.validateNumerical("pad", "input", input)
		SDValidation.validateNumerical("pad", "padding", padding)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.Pad(sd,input, padding, PadMode.CONSTANT, constant)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Padding operation <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input tensor (NUMERIC type) </param>
	  ''' <param name="padding"> Padding value (NUMERIC type) </param>
	  ''' <param name="constant"> Padding constant </param>
	  ''' <returns> output Padded input (NUMERIC type) </returns>
	  Public Overridable Function pad(ByVal name As String, ByVal input As SDVariable, ByVal padding As SDVariable, ByVal constant As Double) As SDVariable
		SDValidation.validateNumerical("pad", "input", input)
		SDValidation.validateNumerical("pad", "padding", padding)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.Pad(sd,input, padding, PadMode.CONSTANT, constant)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the precise method<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function preciseGelu(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("preciseGelu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELU(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' GELU activation function - Gaussian Error Linear Units<br>
	  ''' For more details, see <i>Gaussian Error Linear Units (GELUs)</i> - <a href="https://arxiv.org/abs/1606.08415">https://arxiv.org/abs/1606.08415</a><br>
	  ''' This method uses the precise method<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function preciseGelu(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("preciseGelu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELU(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function prelu(ByVal input As SDVariable, ByVal alpha As SDVariable, ParamArray ByVal sharedAxes() As Integer) As SDVariable
		SDValidation.validateNumerical("prelu", "input", input)
		SDValidation.validateNumerical("prelu", "alpha", alpha)
		Preconditions.checkArgument(sharedAxes.Length >= 1, "sharedAxes has incorrect size/length. Expected: sharedAxes.length >= 1, got %s", sharedAxes.Length)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.PRelu(sd,input, alpha, sharedAxes)).outputVariable()
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
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="alpha"> The cutoff variable.  Note that the batch dimension (the 0th, whether it is batch or not) should not be part of alpha. (NUMERIC type) </param>
	  ''' <param name="sharedAxes"> Which axes to share cutoff parameters along. (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function prelu(ByVal name As String, ByVal input As SDVariable, ByVal alpha As SDVariable, ParamArray ByVal sharedAxes() As Integer) As SDVariable
		SDValidation.validateNumerical("prelu", "input", input)
		SDValidation.validateNumerical("prelu", "alpha", alpha)
		Preconditions.checkArgument(sharedAxes.Length >= 1, "sharedAxes has incorrect size/length. Expected: sharedAxes.length >= 1, got %s", sharedAxes.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.PRelu(sd,input, alpha, sharedAxes)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise rectified linear function with specified cutoff:<br>
	  ''' out[i] = in[i] if in[i] >= cutoff<br>
	  ''' out[i] = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation - x > cutoff ? x : 0. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu(ByVal x As SDVariable, ByVal cutoff As Double) As SDVariable
		SDValidation.validateNumerical("relu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear(sd,x, cutoff)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise rectified linear function with specified cutoff:<br>
	  ''' out[i] = in[i] if in[i] >= cutoff<br>
	  ''' out[i] = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation - x > cutoff ? x : 0. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu(ByVal name As String, ByVal x As SDVariable, ByVal cutoff As Double) As SDVariable
		SDValidation.validateNumerical("relu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear(sd,x, cutoff)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise "rectified linear 6" function with specified cutoff:<br>
	  ''' out[i] = min(max(in, cutoff), 6)<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu6(ByVal x As SDVariable, ByVal cutoff As Double) As SDVariable
		SDValidation.validateNumerical("relu6", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.Relu6(sd,x, cutoff)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise "rectified linear 6" function with specified cutoff:<br>
	  ''' out[i] = min(max(in, cutoff), 6)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="cutoff"> Cutoff value for ReLU operation. Usually 0 </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function relu6(ByVal name As String, ByVal x As SDVariable, ByVal cutoff As Double) As SDVariable
		SDValidation.validateNumerical("relu6", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.Relu6(sd,x, cutoff)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' ReLU (Rectified Linear Unit) layer operation: out = relu(mmul(in,w) + bias)<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reluLayer(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable) As SDVariable
		SDValidation.validateNumerical("reluLayer", "input", input)
		SDValidation.validateNumerical("reluLayer", "weights", weights)
		SDValidation.validateNumerical("reluLayer", "bias", bias)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.ReluLayer(sd,input, weights, bias)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' ReLU (Rectified Linear Unit) layer operation: out = relu(mmul(in,w) + bias)<br>
	  ''' Note that bias array is optional<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input data (NUMERIC type) </param>
	  ''' <param name="weights"> Weights variable (NUMERIC type) </param>
	  ''' <param name="bias"> Optional bias variable (may be null) (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reluLayer(ByVal name As String, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable) As SDVariable
		SDValidation.validateNumerical("reluLayer", "input", input)
		SDValidation.validateNumerical("reluLayer", "weights", weights)
		SDValidation.validateNumerical("reluLayer", "bias", bias)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.ReluLayer(sd,input, weights, bias)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise SeLU function - Scaled exponential Lineal Unit: see <a href="https://arxiv.org/abs/1706.02515">Self-Normalizing Neural Networks</a><br>
	  ''' <br>
	  ''' out[i] = scale * alpha * (exp(in[i])-1) if in[i]>0, or 0 if in[i] <= 0<br>
	  ''' Uses default scale and alpha values.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function selu(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("selu", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.SELU(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise SeLU function - Scaled exponential Lineal Unit: see <a href="https://arxiv.org/abs/1706.02515">Self-Normalizing Neural Networks</a><br>
	  ''' <br>
	  ''' out[i] = scale * alpha * (exp(in[i])-1) if in[i]>0, or 0 if in[i] <= 0<br>
	  ''' Uses default scale and alpha values.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function selu(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("selu", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.SELU(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = 1.0/(1+exp(-in[i]))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoid(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoid", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sigmoid(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function: out[i] = 1.0/(1+exp(-in[i]))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sigmoid(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoid", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sigmoid(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function derivative: dL/dIn given input and dL/dOut<br>
	  ''' </summary>
	  ''' <param name="x"> Input Variable (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at the output - dL/dOut. Must have same shape as the input (NUMERIC type) </param>
	  ''' <returns> output Output (gradient at input of sigmoid) (NUMERIC type) </returns>
	  Public Overridable Function sigmoidDerivative(ByVal x As SDVariable, ByVal wrt As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoidDerivative", "x", x)
		SDValidation.validateNumerical("sigmoidDerivative", "wrt", wrt)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SigmoidDerivative(sd,x, wrt)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise sigmoid function derivative: dL/dIn given input and dL/dOut<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input Variable (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at the output - dL/dOut. Must have same shape as the input (NUMERIC type) </param>
	  ''' <returns> output Output (gradient at input of sigmoid) (NUMERIC type) </returns>
	  Public Overridable Function sigmoidDerivative(ByVal name As String, ByVal x As SDVariable, ByVal wrt As SDVariable) As SDVariable
		SDValidation.validateNumerical("sigmoidDerivative", "x", x)
		SDValidation.validateNumerical("sigmoidDerivative", "wrt", wrt)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SigmoidDerivative(sd,x, wrt)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply softmax </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal x As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("softmax", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(sd,x, dimension)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension along which to apply softmax </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal name As String, ByVal x As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("softmax", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(sd,x, dimension)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softmax", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(sd,x, -1)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Softmax activation, along the specified dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softmax(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softmax", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax(sd,x, -1)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Softmax derivative function<br>
	  ''' </summary>
	  ''' <param name="x"> Softmax input (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at output, dL/dx (NUMERIC type) </param>
	  ''' <param name="dimension"> Softmax dimension </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function softmaxDerivative(ByVal x As SDVariable, ByVal wrt As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("softmaxDerivative", "x", x)
		SDValidation.validateNumerical("softmaxDerivative", "wrt", wrt)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftmaxBp(sd,x, wrt, dimension)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Softmax derivative function<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Softmax input (NUMERIC type) </param>
	  ''' <param name="wrt"> Gradient at output, dL/dx (NUMERIC type) </param>
	  ''' <param name="dimension"> Softmax dimension </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function softmaxDerivative(ByVal name As String, ByVal x As SDVariable, ByVal wrt As SDVariable, ByVal dimension As Integer) As SDVariable
		SDValidation.validateNumerical("softmaxDerivative", "x", x)
		SDValidation.validateNumerical("softmaxDerivative", "wrt", wrt)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftmaxBp(sd,x, wrt, dimension)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise softplus function: out = log(exp(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softplus(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softplus", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftPlus(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise softplus function: out = log(exp(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softplus(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softplus", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftPlus(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise softsign function: out = x / (abs(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softsign(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softsign", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftSign(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise softsign function: out = x / (abs(x) + 1)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function softsign(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softsign", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.SoftSign(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise derivative (dOut/dIn) of the softsign function softsign(INDArray)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function softsignDerivative(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softsignDerivative", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignDerivative(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise derivative (dOut/dIn) of the softsign function softsign(INDArray)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function softsignDerivative(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("softsignDerivative", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignDerivative(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise "swish" function: out = x * sigmoid(b*x) with b=1.0<br>
	  ''' See: <a href="https://arxiv.org/abs/1710.05941">https://arxiv.org/abs/1710.05941</a><br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function swish(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("swish", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Swish(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise "swish" function: out = x * sigmoid(b*x) with b=1.0<br>
	  ''' See: <a href="https://arxiv.org/abs/1710.05941">https://arxiv.org/abs/1710.05941</a><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function swish(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("swish", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Swish(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise tanh (hyperbolic tangent) operation: out = tanh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tanh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("tanh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Tanh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise tanh (hyperbolic tangent) operation: out = tanh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tanh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("tanh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Tanh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace