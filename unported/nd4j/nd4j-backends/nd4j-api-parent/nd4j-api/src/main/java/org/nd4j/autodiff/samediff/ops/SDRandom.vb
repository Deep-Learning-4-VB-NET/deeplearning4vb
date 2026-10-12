import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

	Public Class SDRandom
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Bernoulli distribution,<br>
	  ''' with the specified probability. Array values will have value 1 with probability P and value 0 with probability<br>
	  ''' 1-P.<br>
	  ''' </summary>
	  ''' <param name="p"> Probability of value 1 </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function bernoulli(ByVal p As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution(sd,p, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Bernoulli distribution,<br>
	  ''' with the specified probability. Array values will have value 1 with probability P and value 0 with probability<br>
	  ''' 1-P.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="p"> Probability of value 1 </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function bernoulli(ByVal name As String, ByVal p As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution(sd,p, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Binomial distribution,<br>
	  ''' with the specified number of trials and probability.<br>
	  ''' </summary>
	  ''' <param name="nTrials"> Number of trials parameter for the binomial distribution </param>
	  ''' <param name="p"> Probability of success for each trial </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function binomial(ByVal nTrials As Integer, ByVal p As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.BinomialDistribution(sd,nTrials, p, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Binomial distribution,<br>
	  ''' with the specified number of trials and probability.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="nTrials"> Number of trials parameter for the binomial distribution </param>
	  ''' <param name="p"> Probability of success for each trial </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function binomial(ByVal name As String, ByVal nTrials As Integer, ByVal p As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.BinomialDistribution(sd,nTrials, p, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a exponential distribution:<br>
	  ''' P(x) = lambda * exp(-lambda * x)<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be positive: lambda > 0<br>
	  ''' </summary>
	  ''' <param name="lambda"> lambda parameter </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function exponential(ByVal lambda As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Preconditions.checkArgument(lambda > 0, "Must be positive")
		Return (New org.nd4j.linalg.api.ops.random.custom.RandomExponential(sd,lambda, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a exponential distribution:<br>
	  ''' P(x) = lambda * exp(-lambda * x)<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be positive: lambda > 0<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="lambda"> lambda parameter </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function exponential(ByVal name As String, ByVal lambda As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Preconditions.checkArgument(lambda > 0, "Must be positive")
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.custom.RandomExponential(sd,lambda, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Log Normal distribution,<br>
	  ''' i.e., {@code log(x) ~ N(mean, stdev)}<br>
	  ''' </summary>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function logNormal(ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Log Normal distribution,<br>
	  ''' i.e., {@code log(x) ~ N(mean, stdev)}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function logNormal(ByVal name As String, ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Gaussian (normal) distribution,<br>
	  ''' N(mean, stdev)<br>
	  ''' </summary>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function normal(ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.GaussianDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Gaussian (normal) distribution,<br>
	  ''' N(mean, stdev)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function normal(ByVal name As String, ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.GaussianDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Gaussian (normal) distribution,<br>
	  ''' N(mean, stdev). However, any values more than 1 standard deviation from the mean are dropped and re-sampled<br>
	  ''' </summary>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function normalTruncated(ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a Gaussian (normal) distribution,<br>
	  ''' N(mean, stdev). However, any values more than 1 standard deviation from the mean are dropped and re-sampled<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="mean"> Mean value for the random array </param>
	  ''' <param name="stddev"> Standard deviation for the random array </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function normalTruncated(ByVal name As String, ByVal mean As Double, ByVal stddev As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.TruncatedNormalDistribution(sd,mean, stddev, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a uniform distribution,<br>
	  ''' U(min,max)<br>
	  ''' </summary>
	  ''' <param name="min"> Minimum value </param>
	  ''' <param name="max"> Maximum value. </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function uniform(ByVal min As Double, ByVal max As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return (New org.nd4j.linalg.api.ops.random.impl.UniformDistribution(sd,min, max, datatype, shape)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate a new random INDArray, where values are randomly sampled according to a uniform distribution,<br>
	  ''' U(min,max)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="min"> Minimum value </param>
	  ''' <param name="max"> Maximum value. </param>
	  ''' <param name="datatype"> Data type of the output variable </param>
	  ''' <param name="shape"> Shape of the new random INDArray, as a 1D array (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Tensor with the given shape where values are randomly sampled according to a %OP_NAME% distribution (NUMERIC type) </returns>
	  Public Overridable Function uniform(ByVal name As String, ByVal min As Double, ByVal max As Double, ByVal datatype As DataType, ParamArray ByVal shape() As Long) As SDVariable
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.random.impl.UniformDistribution(sd,min, max, datatype, shape)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace