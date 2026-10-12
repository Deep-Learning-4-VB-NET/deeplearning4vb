import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PartitionMode = org.nd4j.enums.PartitionMode
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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

	Public Class SDMath
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' Clips tensor values to a maximum average L2-norm.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValue"> Value for clipping </param>
	  ''' <param name="dimensions"> Dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByAvgNorm(ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("ClipByAvgNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByAvgNorm(sd,x, clipValue, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Clips tensor values to a maximum average L2-norm.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValue"> Value for clipping </param>
	  ''' <param name="dimensions"> Dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByAvgNorm(ByVal name As String, ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("ClipByAvgNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByAvgNorm(sd,x, clipValue, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Looks up ids in a list of embedding tensors.<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="indices"> A Tensor containing the ids to be looked up. (INT type) </param>
	  ''' <param name="PartitionMode"> partition_mode == 0 - i.e. 'mod' , 1 - 'div' </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function embeddingLookup(ByVal x As SDVariable, ByVal indices As SDVariable, ByVal PartitionMode As PartitionMode) As SDVariable
		SDValidation.validateNumerical("EmbeddingLookup", "x", x)
		SDValidation.validateInteger("EmbeddingLookup", "indices", indices)
		Return (New org.nd4j.linalg.api.ops.impl.shape.tensorops.EmbeddingLookup(sd,x, indices, PartitionMode)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Looks up ids in a list of embedding tensors.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="indices"> A Tensor containing the ids to be looked up. (INT type) </param>
	  ''' <param name="PartitionMode"> partition_mode == 0 - i.e. 'mod' , 1 - 'div' </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function embeddingLookup(ByVal name As String, ByVal x As SDVariable, ByVal indices As SDVariable, ByVal PartitionMode As PartitionMode) As SDVariable
		SDValidation.validateNumerical("EmbeddingLookup", "x", x)
		SDValidation.validateInteger("EmbeddingLookup", "indices", indices)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.tensorops.EmbeddingLookup(sd,x, indices, PartitionMode)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ByVal x() As SDVariable, ByVal dataType As DataType) As SDVariable
		SDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(sd,x, dataType)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ByVal name As String, ByVal x() As SDVariable, ByVal dataType As DataType) As SDVariable
		SDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(sd,x, dataType)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ParamArray ByVal x() As SDVariable) As SDVariable
		SDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(sd,x, DataType.INT)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ByVal name As String, ParamArray ByVal x() As SDVariable) As SDVariable
		SDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(sd,x, DataType.INT)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise absolute value operation: out = abs(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function abs(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("abs", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Abs(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise absolute value operation: out = abs(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function abs(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("abs", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Abs(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise acos (arccosine, inverse cosine) operation: out = arccos(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acos(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("acos", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ACos(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise acos (arccosine, inverse cosine) operation: out = arccos(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acos(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("acos", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ACos(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise acosh (inverse hyperbolic cosine) function: out = acosh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acosh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("acosh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ACosh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise acosh (inverse hyperbolic cosine) function: out = acosh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acosh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("acosh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ACosh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise addition operation, out = x + y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function add(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("add", "x", x)
		SDValidation.validateNumerical("add", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise addition operation, out = x + y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function add(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("add", "x", x)
		SDValidation.validateNumerical("add", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar add operation, out = in + scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function add(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("add", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarAdd(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar add operation, out = in + scalar<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function add(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("add", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarAdd(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Boolean AND operation: elementwise (x != 0) && (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [and](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("and", "x", x)
		SDValidation.validateBool("and", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Boolean AND operation: elementwise (x != 0) && (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [and](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("and", "x", x)
		SDValidation.validateBool("and", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise asin (arcsin, inverse sine) operation: out = arcsin(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asin(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("asin", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ASin(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise asin (arcsin, inverse sine) operation: out = arcsin(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asin(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("asin", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ASin(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise asinh (inverse hyperbolic sine) function: out = asinh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asinh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("asinh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ASinh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise asinh (inverse hyperbolic sine) function: out = asinh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asinh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("asinh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ASinh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.ASum(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.ASum(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.ASum(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.ASum(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = arctangent(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atan", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ATan(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = arctangent(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atan", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ATan(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = atan2(x,y).<br>
	  ''' Similar to atan(y/x) but sigts of x and y are used to determine the location of the result<br>
	  ''' </summary>
	  ''' <param name="y"> Input Y variable (NUMERIC type) </param>
	  ''' <param name="x"> Input X variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan2(ByVal y As SDVariable, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atan2", "y", y)
		SDValidation.validateNumerical("atan2", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.ATan2(sd,y, x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = atan2(x,y).<br>
	  ''' Similar to atan(y/x) but sigts of x and y are used to determine the location of the result<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="y"> Input Y variable (NUMERIC type) </param>
	  ''' <param name="x"> Input X variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan2(ByVal name As String, ByVal y As SDVariable, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atan2", "y", y)
		SDValidation.validateNumerical("atan2", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.ATan2(sd,y, x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise atanh (inverse hyperbolic tangent) function: out = atanh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atanh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atanh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.ATanh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise atanh (inverse hyperbolic tangent) function: out = atanh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atanh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("atanh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.ATanh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> input (NUMERIC type) </param>
	  ''' <param name="shift"> shift value (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShift(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShift", "x", x)
		SDValidation.validateNumerical("bitShift", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bit shift operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> input (NUMERIC type) </param>
	  ''' <param name="shift"> shift value (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShift(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShift", "x", x)
		SDValidation.validateNumerical("bitShift", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Right bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRight(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRight", "x", x)
		SDValidation.validateNumerical("bitShiftRight", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Right bit shift operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRight(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRight", "x", x)
		SDValidation.validateNumerical("bitShiftRight", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cyclic bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argy=ument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotl(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRotl", "x", x)
		SDValidation.validateNumerical("bitShiftRotl", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cyclic bit shift operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argy=ument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotl(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRotl", "x", x)
		SDValidation.validateNumerical("bitShiftRotl", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cyclic right shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> Shift argument (NUMERIC type) </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotr(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRotr", "x", x)
		SDValidation.validateNumerical("bitShiftRotr", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cyclic right shift operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> Shift argument (NUMERIC type) </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotr(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateNumerical("bitShiftRotr", "x", x)
		SDValidation.validateNumerical("bitShiftRotr", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise ceiling function: out = ceil(x).<br>
	  ''' Rounds each value up to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function ceil(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("ceil", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Ceil(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise ceiling function: out = ceil(x).<br>
	  ''' Rounds each value up to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function ceil(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("ceil", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Ceil(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Clipping by L2 norm, optionally along dimension(s)<br>
	  ''' if l2Norm(x,dimension) < clipValue, then input is returned unmodifed<br>
	  ''' Otherwise, out[i] = in[i] * clipValue / l2Norm(in, dimensions) where each value is clipped according<br>
	  ''' to the corresponding l2Norm along the specified dimensions<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValue"> Clipping value (maximum l2 norm) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByNorm(ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("clipByNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByNorm(sd,x, clipValue, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Clipping by L2 norm, optionally along dimension(s)<br>
	  ''' if l2Norm(x,dimension) < clipValue, then input is returned unmodifed<br>
	  ''' Otherwise, out[i] = in[i] * clipValue / l2Norm(in, dimensions) where each value is clipped according<br>
	  ''' to the corresponding l2Norm along the specified dimensions<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValue"> Clipping value (maximum l2 norm) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByNorm(ByVal name As String, ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("clipByNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByNorm(sd,x, clipValue, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise clipping function:<br>
	  ''' out[i] = in[i] if in[i] >= clipValueMin and in[i] <= clipValueMax<br>
	  ''' out[i] = clipValueMin if in[i] < clipValueMin<br>
	  ''' out[i] = clipValueMax if in[i] > clipValueMax<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValueMin"> Minimum value for clipping </param>
	  ''' <param name="clipValueMax"> Maximum value for clipping </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByValue(ByVal x As SDVariable, ByVal clipValueMin As Double, ByVal clipValueMax As Double) As SDVariable
		SDValidation.validateNumerical("clipByValue", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByValue(sd,x, clipValueMin, clipValueMax)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise clipping function:<br>
	  ''' out[i] = in[i] if in[i] >= clipValueMin and in[i] <= clipValueMax<br>
	  ''' out[i] = clipValueMin if in[i] < clipValueMin<br>
	  ''' out[i] = clipValueMax if in[i] > clipValueMax<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValueMin"> Minimum value for clipping </param>
	  ''' <param name="clipValueMax"> Maximum value for clipping </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByValue(ByVal name As String, ByVal x As SDVariable, ByVal clipValueMin As Double, ByVal clipValueMax As Double) As SDVariable
		SDValidation.validateNumerical("clipByValue", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByValue(sd,x, clipValueMin, clipValueMax)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values. This version assumes the number of classes is 1 + max(max(labels), max(pred))<br>
	  ''' For example, if labels = [0, 1, 1] and predicted = [0, 2, 1] then output is:<br>
	  ''' [1, 0, 0]<br>
	  ''' [0, 1, 1]<br>
	  ''' [0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal dataType As DataType) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Return (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, dataType)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values. This version assumes the number of classes is 1 + max(max(labels), max(pred))<br>
	  ''' For example, if labels = [0, 1, 1] and predicted = [0, 2, 1] then output is:<br>
	  ''' [1, 0, 0]<br>
	  ''' [0, 1, 1]<br>
	  ''' [0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal name As String, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal dataType As DataType) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, dataType)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values.<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1], and numClasses=4 then output is:<br>
	  ''' [1, 0, 0, 0]<br>
	  ''' [0, 1, 1, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="numClasses"> Number of classes </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal numClasses As Integer) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Return (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, numClasses)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values.<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1], and numClasses=4 then output is:<br>
	  ''' [1, 0, 0, 0]<br>
	  ''' [0, 1, 1, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="numClasses"> Number of classes </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal name As String, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal numClasses As Integer) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, numClasses)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values. This version assumes the number of classes is 1 + max(max(labels), max(pred))<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1] and weights = [1, 2, 3]<br>
	  ''' [1, 0, 0]<br>
	  ''' [0, 3, 2]<br>
	  ''' [0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="weights"> Weights - 1D array of values (may be real/decimal) representing the weight/contribution of each prediction. Must be same length as both labels and predictions arrays (NUMERIC type) </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		SDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, weights)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values. This version assumes the number of classes is 1 + max(max(labels), max(pred))<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1] and weights = [1, 2, 3]<br>
	  ''' [1, 0, 0]<br>
	  ''' [0, 3, 2]<br>
	  ''' [0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="weights"> Weights - 1D array of values (may be real/decimal) representing the weight/contribution of each prediction. Must be same length as both labels and predictions arrays (NUMERIC type) </param>
	  ''' <returns> output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal name As String, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		SDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, weights)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values.<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1], numClasses = 4, and weights = [1, 2, 3]<br>
	  ''' [1, 0, 0, 0]<br>
	  ''' [0, 3, 2, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="weights"> Weights - 1D array of values (may be real/decimal) representing the weight/contribution of each prediction. Must be same length as both labels and predictions arrays (NUMERIC type) </param>
	  ''' <param name="numClasses"> </param>
	  ''' <returns> output Output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable, ByVal numClasses As Integer) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		SDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Return (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, weights, numClasses)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Compute the 2d confusion matrix of size [numClasses, numClasses] from a pair of labels and predictions, both of<br>
	  ''' which are represented as integer values.<br>
	  ''' For example, if labels = [0, 1, 1], predicted = [0, 2, 1], numClasses = 4, and weights = [1, 2, 3]<br>
	  ''' [1, 0, 0, 0]<br>
	  ''' [0, 3, 2, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' [0, 0, 0, 0]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="labels"> Labels - 1D array of integer values representing label values (NUMERIC type) </param>
	  ''' <param name="pred"> Predictions - 1D array of integer values representing predictions. Same length as labels (NUMERIC type) </param>
	  ''' <param name="weights"> Weights - 1D array of values (may be real/decimal) representing the weight/contribution of each prediction. Must be same length as both labels and predictions arrays (NUMERIC type) </param>
	  ''' <param name="numClasses"> </param>
	  ''' <returns> output Output variable (2D, shape [numClasses, numClasses}) (NUMERIC type) </returns>
	  Public Overridable Function confusionMatrix(ByVal name As String, ByVal labels As SDVariable, ByVal pred As SDVariable, ByVal weights As SDVariable, ByVal numClasses As Integer) As SDVariable
		SDValidation.validateNumerical("confusionMatrix", "labels", labels)
		SDValidation.validateNumerical("confusionMatrix", "pred", pred)
		SDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(sd,labels, pred, weights, numClasses)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise cosine operation: out = cos(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cos(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cos", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Cos(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise cosine operation: out = cos(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cos(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cos", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Cos(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise cosh (hyperbolic cosine) operation: out = cosh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cosh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Cosh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise cosh (hyperbolic cosine) operation: out = cosh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cosh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Cosh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = 1.0 - cosineSimilarity(x,y)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "x", x)
		SDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cosine distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = 1.0 - cosineSimilarity(x,y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "x", x)
		SDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = 1.0 - cosineSimilarity(x,y)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "x", x)
		SDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cosine distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = 1.0 - cosineSimilarity(x,y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineDistance", "x", x)
		SDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine similarity pairwise reduction operation. The output contains the cosine similarity for each tensor/subset<br>
	  ''' along the specified dimensions:<br>
	  ''' out = (sum_i x[i] * y[i]) / ( sqrt(sum_i x[i]^2) * sqrt(sum_i y[i]^2)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineSimilarity(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineSimilarity", "x", x)
		SDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cosine similarity pairwise reduction operation. The output contains the cosine similarity for each tensor/subset<br>
	  ''' along the specified dimensions:<br>
	  ''' out = (sum_i x[i] * y[i]) / ( sqrt(sum_i x[i]^2) * sqrt(sum_i y[i]^2)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineSimilarity(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineSimilarity", "x", x)
		SDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Cosine similarity pairwise reduction operation. The output contains the cosine similarity for each tensor/subset<br>
	  ''' along the specified dimensions:<br>
	  ''' out = (sum_i x[i] * y[i]) / ( sqrt(sum_i x[i]^2) * sqrt(sum_i y[i]^2)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineSimilarity(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineSimilarity", "x", x)
		SDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Cosine similarity pairwise reduction operation. The output contains the cosine similarity for each tensor/subset<br>
	  ''' along the specified dimensions:<br>
	  ''' out = (sum_i x[i] * y[i]) / ( sqrt(sum_i x[i]^2) * sqrt(sum_i y[i]^2)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosineSimilarity(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("cosineSimilarity", "x", x)
		SDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Returns the pair-wise cross product of equal size arrays a and b: a x b = ||a||x||b|| sin(theta).<br>
	  ''' Can take rank 1 or above inputs (of equal shapes), but note that the last dimension must have dimension 3<br>
	  ''' </summary>
	  ''' <param name="a"> First input (NUMERIC type) </param>
	  ''' <param name="b"> Second input (NUMERIC type) </param>
	  ''' <returns> output Element-wise cross product (NUMERIC type) </returns>
	  Public Overridable Function cross(ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("cross", "a", a)
		SDValidation.validateNumerical("cross", "b", b)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Cross(sd,a, b)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Returns the pair-wise cross product of equal size arrays a and b: a x b = ||a||x||b|| sin(theta).<br>
	  ''' Can take rank 1 or above inputs (of equal shapes), but note that the last dimension must have dimension 3<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="a"> First input (NUMERIC type) </param>
	  ''' <param name="b"> Second input (NUMERIC type) </param>
	  ''' <returns> output Element-wise cross product (NUMERIC type) </returns>
	  Public Overridable Function cross(ByVal name As String, ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("cross", "a", a)
		SDValidation.validateNumerical("cross", "b", b)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Cross(sd,a, b)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise cube function: out = x^3<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cube(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cube", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Cube(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise cube function: out = x^3<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cube(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("cube", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Cube(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Returns an output variable with diagonal values equal to the specified values; off-diagonal values will be set to 0<br>
	  ''' For example, if input = [1,2,3], then output is given by:<br>
	  ''' [ 1, 0, 0]<br>
	  ''' [ 0, 2, 0]<br>
	  ''' [ 0, 0, 3]<br>
	  ''' <br>
	  ''' Higher input ranks are also supported: if input has shape [a,...,R-1] then output[i,...,k,i,...,k] = input[i,...,k].<br>
	  ''' i.e., for input rank R, output has rank 2R<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function diag(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Diag(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Returns an output variable with diagonal values equal to the specified values; off-diagonal values will be set to 0<br>
	  ''' For example, if input = [1,2,3], then output is given by:<br>
	  ''' [ 1, 0, 0]<br>
	  ''' [ 0, 2, 0]<br>
	  ''' [ 0, 0, 3]<br>
	  ''' <br>
	  ''' Higher input ranks are also supported: if input has shape [a,...,R-1] then output[i,...,k,i,...,k] = input[i,...,k].<br>
	  ''' i.e., for input rank R, output has rank 2R<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function diag(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Diag(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Extract the diagonal part from the input array.<br>
	  ''' If input is<br>
	  ''' [ 1, 0, 0]<br>
	  ''' [ 0, 2, 0]<br>
	  ''' [ 0, 0, 3]<br>
	  ''' then output is [1, 2, 3].<br>
	  ''' Supports higher dimensions: in general, out[i,...,k] = in[i,...,k,i,...,k]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Diagonal part of the input (NUMERIC type) </returns>
	  Public Overridable Function diagPart(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("diagPart", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.shape.DiagPart(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Extract the diagonal part from the input array.<br>
	  ''' If input is<br>
	  ''' [ 1, 0, 0]<br>
	  ''' [ 0, 2, 0]<br>
	  ''' [ 0, 0, 3]<br>
	  ''' then output is [1, 2, 3].<br>
	  ''' Supports higher dimensions: in general, out[i,...,k] = in[i,...,k,i,...,k]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Diagonal part of the input (NUMERIC type) </returns>
	  Public Overridable Function diagPart(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("diagPart", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.DiagPart(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise division operation, out = x / y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function div(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("div", "x", x)
		SDValidation.validateNumerical("div", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.DivOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise division operation, out = x / y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function div(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("div", "x", x)
		SDValidation.validateNumerical("div", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.DivOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar division operation, out = in / scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function div(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("div", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarDivision(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar division operation, out = in / scalar<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function div(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("div", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarDivision(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise Gaussian error function - out = erf(in)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erf(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("erf", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Erf(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise Gaussian error function - out = erf(in)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erf(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("erf", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Erf(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise complementary Gaussian error function - out = erfc(in) = 1 - erf(in)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erfc(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("erfc", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Erfc(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise complementary Gaussian error function - out = erfc(in) = 1 - erf(in)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erfc(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("erfc", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Erfc(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean distance (l2 norm, l2 distance) reduction operation. The output contains the Euclidean distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt( sum_i (x[i] - y[i])^2 )<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function euclideanDistance(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("euclideanDistance", "x", x)
		SDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean distance (l2 norm, l2 distance) reduction operation. The output contains the Euclidean distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt( sum_i (x[i] - y[i])^2 )<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function euclideanDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("euclideanDistance", "x", x)
		SDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean distance (l2 norm, l2 distance) reduction operation. The output contains the Euclidean distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt( sum_i (x[i] - y[i])^2 )<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function euclideanDistance(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("euclideanDistance", "x", x)
		SDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean distance (l2 norm, l2 distance) reduction operation. The output contains the Euclidean distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt( sum_i (x[i] - y[i])^2 )<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function euclideanDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("euclideanDistance", "x", x)
		SDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise exponent function: out = exp(x) = 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function exp(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("exp", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Exp(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise exponent function: out = exp(x) = 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function exp(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("exp", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Exp(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise 1.0 - exponent function: out = 1.0 - exp(x) = 1.0 - 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function expm1(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("expm1", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Expm1(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise 1.0 - exponent function: out = 1.0 - exp(x) = 1.0 - 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function expm1(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("expm1", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Expm1(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate an identity matrix with the specified number of rows and columns.<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As Integer) As SDVariable
		Return (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate an identity matrix with the specified number of rows and columns.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal name As String, ByVal rows As Integer) As SDVariable
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int, int, DataType) but with the default datatype, Eye.DEFAULT_DTYPE<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <param name="cols"> Number of columns </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As Integer, ByVal cols As Integer) As SDVariable
		Return (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int, int, DataType) but with the default datatype, Eye.DEFAULT_DTYPE<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <param name="cols"> Number of columns </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal name As String, ByVal rows As Integer, ByVal cols As Integer) As SDVariable
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Generate an identity matrix with the specified number of rows and columns<br>
	  ''' Example:<br>
	  ''' <pre><br>
	  ''' {@code INDArray eye = eye(3,2)<br>
	  ''' eye:<br>
	  ''' [ 1, 0]<br>
	  ''' [ 0, 1]<br>
	  ''' [ 0, 0]}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <param name="cols"> Number of columns </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As Integer, ByVal cols As Integer, ByVal dataType As DataType, ParamArray ByVal dimensions() As Integer) As SDVariable
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols, dataType, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Generate an identity matrix with the specified number of rows and columns<br>
	  ''' Example:<br>
	  ''' <pre><br>
	  ''' {@code INDArray eye = eye(3,2)<br>
	  ''' eye:<br>
	  ''' [ 1, 0]<br>
	  ''' [ 0, 1]<br>
	  ''' [ 0, 0]}<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <param name="cols"> Number of columns </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal name As String, ByVal rows As Integer, ByVal cols As Integer, ByVal dataType As DataType, ParamArray ByVal dimensions() As Integer) As SDVariable
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols, dataType, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' As per eye(int, int) bit with the number of rows/columns specified as scalar INDArrays<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <param name="cols"> Number of columns (INT type) </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As SDVariable, ByVal cols As SDVariable) As SDVariable
		SDValidation.validateInteger("eye", "rows", rows)
		SDValidation.validateInteger("eye", "cols", cols)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' As per eye(int, int) bit with the number of rows/columns specified as scalar INDArrays<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <param name="cols"> Number of columns (INT type) </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal name As String, ByVal rows As SDVariable, ByVal cols As SDVariable) As SDVariable
		SDValidation.validateInteger("eye", "rows", rows)
		SDValidation.validateInteger("eye", "cols", cols)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows, cols)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int) but with the number of rows specified as a scalar INDArray<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <returns> output SDVaribable identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As SDVariable) As SDVariable
		SDValidation.validateInteger("eye", "rows", rows)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int) but with the number of rows specified as a scalar INDArray<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <returns> output SDVaribable identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal name As String, ByVal rows As SDVariable) As SDVariable
		SDValidation.validateInteger("eye", "rows", rows)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Eye(sd,rows)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' First index reduction operation.<br>
	  ''' Returns a variable that contains the index of the first element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function firstIndex(ByVal [in] As SDVariable, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex(sd,[in], False, condition, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' First index reduction operation.<br>
	  ''' Returns a variable that contains the index of the first element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function firstIndex(ByVal name As String, ByVal [in] As SDVariable, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex(sd,[in], False, condition, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' First index reduction operation.<br>
	  ''' Returns a variable that contains the index of the first element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function firstIndex(ByVal [in] As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex(sd,[in], keepDims, condition, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' First index reduction operation.<br>
	  ''' Returns a variable that contains the index of the first element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function firstIndex(ByVal name As String, ByVal [in] As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex(sd,[in], keepDims, condition, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise floor function: out = floor(x).<br>
	  ''' Rounds each value down to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floor(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("floor", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Floor(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise floor function: out = floor(x).<br>
	  ''' Rounds each value down to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floor(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("floor", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Floor(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise floor division operation, out = floor(x / y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorDiv(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("floorDiv", "x", x)
		SDValidation.validateNumerical("floorDiv", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorDivOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise floor division operation, out = floor(x / y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorDiv(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("floorDiv", "x", x)
		SDValidation.validateNumerical("floorDiv", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorDivOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise Modulus division operation<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorMod(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("floorMod", "x", x)
		SDValidation.validateNumerical("floorMod", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorModOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise Modulus division operation<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorMod(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("floorMod", "x", x)
		SDValidation.validateNumerical("floorMod", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorModOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar floor modulus operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorMod(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("floorMod", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarFMod(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar floor modulus operation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorMod(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("floorMod", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarFMod(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Hamming distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = count( x[i] != y[i] )<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hammingDistance(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("hammingDistance", "x", x)
		SDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Hamming distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = count( x[i] != y[i] )<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hammingDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("hammingDistance", "x", x)
		SDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Hamming distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = count( x[i] != y[i] )<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hammingDistance(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("hammingDistance", "x", x)
		SDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Hamming distance reduction operation. The output contains the cosine distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = count( x[i] != y[i] )<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function hammingDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("hammingDistance", "x", x)
		SDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is finite operation: elementwise isFinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isFinite(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isFinite", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsFinite(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is finite operation: elementwise isFinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isFinite(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isFinite", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsFinite(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is infinite operation: elementwise isInfinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isInfinite(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isInfinite", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsInf(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is infinite operation: elementwise isInfinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isInfinite(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isInfinite", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsInf(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is maximum operation: elementwise x == max(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isMax(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isMax", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.any.IsMax(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is maximum operation: elementwise x == max(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isMax(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isMax", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.any.IsMax(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is Not a Number operation: elementwise isNaN(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isNaN(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isNaN", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsNaN(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is Not a Number operation: elementwise isNaN(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isNaN(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isNaN", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.bool.IsNaN(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is the array non decreasing?<br>
	  ''' An array is non-decreasing if for every valid i, x[i] <= x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if non-decreasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isNonDecreasing(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isNonDecreasing", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.IsNonDecreasing(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is the array non decreasing?<br>
	  ''' An array is non-decreasing if for every valid i, x[i] <= x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if non-decreasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isNonDecreasing(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isNonDecreasing", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.IsNonDecreasing(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Is the array strictly increasing?<br>
	  ''' An array is strictly increasing if for every valid i, x[i] < x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if strictly increasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isStrictlyIncreasing(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isStrictlyIncreasing", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.IsStrictlyIncreasing(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Is the array strictly increasing?<br>
	  ''' An array is strictly increasing if for every valid i, x[i] < x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if strictly increasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isStrictlyIncreasing(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("isStrictlyIncreasing", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.IsStrictlyIncreasing(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Jaccard similarity reduction operation. The output contains the Jaccard distance for each<br>
	  '''                 tensor along the specified dimensions.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function jaccardDistance(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("jaccardDistance", "x", x)
		SDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Jaccard similarity reduction operation. The output contains the Jaccard distance for each<br>
	  '''                 tensor along the specified dimensions.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function jaccardDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("jaccardDistance", "x", x)
		SDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Jaccard similarity reduction operation. The output contains the Jaccard distance for each<br>
	  '''                 tensor along the specified dimensions.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function jaccardDistance(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("jaccardDistance", "x", x)
		SDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Jaccard similarity reduction operation. The output contains the Jaccard distance for each<br>
	  '''                 tensor along the specified dimensions.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function jaccardDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("jaccardDistance", "x", x)
		SDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Last index reduction operation.<br>
	  ''' Returns a variable that contains the index of the last element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function lastIndex(ByVal [in] As SDVariable, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex(sd,[in], False, condition, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Last index reduction operation.<br>
	  ''' Returns a variable that contains the index of the last element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function lastIndex(ByVal name As String, ByVal [in] As SDVariable, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex(sd,[in], False, condition, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Last index reduction operation.<br>
	  ''' Returns a variable that contains the index of the last element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function lastIndex(ByVal [in] As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex(sd,[in], keepDims, condition, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Last index reduction operation.<br>
	  ''' Returns a variable that contains the index of the last element that matches the specified condition (for each<br>
	  ''' slice along the specified dimensions)<br>
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on input variable </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function lastIndex(ByVal name As String, ByVal [in] As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex(sd,[in], keepDims, condition, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates difference between inputs X and Y.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable X (NUMERIC type) </param>
	  ''' <param name="y"> Input variable Y (NUMERIC type) </param>
	  Public Overridable Function listDiff(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable()
		SDValidation.validateNumerical("listDiff", "x", x)
		SDValidation.validateNumerical("listDiff", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.ListDiff(sd,x, y)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Calculates difference between inputs X and Y.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="x"> Input variable X (NUMERIC type) </param>
	  ''' <param name="y"> Input variable Y (NUMERIC type) </param>
	  Public Overridable Function listDiff(ByVal names() As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable()
		SDValidation.validateNumerical("listDiff", "x", x)
		SDValidation.validateNumerical("listDiff", "y", y)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.ListDiff(sd,x, y)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (base e - natural logarithm): out = log(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("log", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Log(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (base e - natural logarithm): out = log(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("log", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Log(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (with specified base): out = log_{base}(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="base"> Logarithm base </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal x As SDVariable, ByVal base As Double) As SDVariable
		SDValidation.validateNumerical("log", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.LogX(sd,x, base)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (with specified base): out = log_{base}(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="base"> Logarithm base </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal name As String, ByVal x As SDVariable, ByVal base As Double) As SDVariable
		SDValidation.validateNumerical("log", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.LogX(sd,x, base)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise natural logarithm function: out = log_e (1 + x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log1p(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("log1p", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Log1p(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise natural logarithm function: out = log_e (1 + x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log1p(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("log1p", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Log1p(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Log-sum-exp reduction (optionally along dimension).<br>
	  ''' Computes log(sum(exp(x))<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Optional dimensions to reduce along (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSumExp(ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logSumExp", "input", input)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp(sd,input, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Log-sum-exp reduction (optionally along dimension).<br>
	  ''' Computes log(sum(exp(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Optional dimensions to reduce along (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSumExp(ByVal name As String, ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("logSumExp", "input", input)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp(sd,input, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Manhattan distance (l1 norm, l1 distance) reduction operation. The output contains the Manhattan distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sum_i abs(x[i]-y[i])<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function manhattanDistance(ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("manhattanDistance", "x", x)
		SDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Manhattan distance (l1 norm, l1 distance) reduction operation. The output contains the Manhattan distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sum_i abs(x[i]-y[i])<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to preserve original dimensions or not </param>
	  ''' <param name="isComplex"> Depending on the implementation, such as distance calculations, this can determine whether all distance calculations for all points should be done. </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function manhattanDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("manhattanDistance", "x", x)
		SDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(sd,x, y, keepDims, isComplex, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Manhattan distance (l1 norm, l1 distance) reduction operation. The output contains the Manhattan distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sum_i abs(x[i]-y[i])<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function manhattanDistance(ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("manhattanDistance", "x", x)
		SDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(sd,x, y, False, False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Manhattan distance (l1 norm, l1 distance) reduction operation. The output contains the Manhattan distance for each<br>
	  ''' tensor/subset along the specified dimensions:<br>
	  ''' out = sum_i abs(x[i]-y[i])<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function manhattanDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("manhattanDistance", "x", x)
		SDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(sd,x, y, False, False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Matrix determinant op. For 2D input, this returns the standard matrix determinant.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix determinant is returned for each <br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix determinant variable (NUMERIC type) </returns>
	  Public Overridable Function matrixDeterminant(ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("matrixDeterminant", "in", [in])
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixDeterminant(sd,[in])).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Matrix determinant op. For 2D input, this returns the standard matrix determinant.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix determinant is returned for each <br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix determinant variable (NUMERIC type) </returns>
	  Public Overridable Function matrixDeterminant(ByVal name As String, ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("matrixDeterminant", "in", [in])
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixDeterminant(sd,[in])).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Matrix inverse op. For 2D input, this returns the standard matrix inverse.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix inverse is returned for each<br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix inverse variable (NUMERIC type) </returns>
	  Public Overridable Function matrixInverse(ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("matrixInverse", "in", [in])
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixInverse(sd,[in])).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Matrix inverse op. For 2D input, this returns the standard matrix inverse.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix inverse is returned for each<br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix inverse variable (NUMERIC type) </returns>
	  Public Overridable Function matrixInverse(ByVal name As String, ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("matrixInverse", "in", [in])
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixInverse(sd,[in])).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise max operation, out = max(x, y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable, x (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable, y (NUMERIC type) </param>
	  ''' <returns> out Output (NUMERIC type) </returns>
	  Public Overridable Function max(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("max", "x", x)
		SDValidation.validateNumerical("max", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Max(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise max operation, out = max(x, y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input variable, x (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable, y (NUMERIC type) </param>
	  ''' <returns> out Output (NUMERIC type) </returns>
	  Public Overridable Function max(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("max", "x", x)
		SDValidation.validateNumerical("max", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Max(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		SDValidation.validateNumerical("mean", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		SDValidation.validateNumerical("mean", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		SDValidation.validateNumerical("mean", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("mean", "in", [in])
		SDValidation.validateNumerical("mean", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Merge add function: merges an arbitrary number of equal shaped arrays using element-wise addition:<br>
	  ''' out = sum_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAdd(ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeAdd", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MergeAddOp(sd,inputs)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Merge add function: merges an arbitrary number of equal shaped arrays using element-wise addition:<br>
	  ''' out = sum_i in[i]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAdd(ByVal name As String, ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeAdd", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MergeAddOp(sd,inputs)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Merge average function: merges an arbitrary number of equal shaped arrays using element-wise mean operation:<br>
	  ''' out = mean_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAvg(ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeAvg", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.MergeAvg(sd,inputs)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Merge average function: merges an arbitrary number of equal shaped arrays using element-wise mean operation:<br>
	  ''' out = mean_i in[i]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAvg(ByVal name As String, ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeAvg", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.MergeAvg(sd,inputs)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Merge max function: merges an arbitrary number of equal shaped arrays using element-wise maximum operation:<br>
	  ''' out = max_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeMax(ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeMax", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.MergeMax(sd,inputs)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Merge max function: merges an arbitrary number of equal shaped arrays using element-wise maximum operation:<br>
	  ''' out = max_i in[i]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeMax(ByVal name As String, ParamArray ByVal inputs() As SDVariable) As SDVariable
		SDValidation.validateNumerical("mergeMax", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.MergeMax(sd,inputs)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Broadcasts parameters for evaluation on an N-D grid.<br>
	  ''' </summary>
	  ''' <param name="inputs">  (NUMERIC type) </param>
	  ''' <param name="cartesian">  </param>
	  Public Overridable Function meshgrid(ByVal inputs() As SDVariable, ByVal cartesian As Boolean) As SDVariable()
		SDValidation.validateNumerical("meshgrid", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 0, "inputs has incorrect size/length. Expected: inputs.length >= 0, got %s", inputs.Length)
		Return (New org.nd4j.linalg.api.ops.impl.shape.MeshGrid(sd,inputs, cartesian)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Broadcasts parameters for evaluation on an N-D grid.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="inputs">  (NUMERIC type) </param>
	  ''' <param name="cartesian">  </param>
	  Public Overridable Function meshgrid(ByVal names() As String, ByVal inputs() As SDVariable, ByVal cartesian As Boolean) As SDVariable()
		SDValidation.validateNumerical("meshgrid", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 0, "inputs has incorrect size/length. Expected: inputs.length >= 0, got %s", inputs.Length)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.MeshGrid(sd,inputs, cartesian)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Pairwise max operation, out = min(x, y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable, x (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable, y (NUMERIC type) </param>
	  ''' <returns> out Output (NUMERIC type) </returns>
	  Public Overridable Function min(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("min", "x", x)
		SDValidation.validateNumerical("min", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Min(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise max operation, out = min(x, y)<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input variable, x (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable, y (NUMERIC type) </param>
	  ''' <returns> out Output (NUMERIC type) </returns>
	  Public Overridable Function min(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("min", "x", x)
		SDValidation.validateNumerical("min", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Min(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise modulus (remainder) operation, out = x % y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [mod](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mod", "x", x)
		SDValidation.validateNumerical("mod", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.ModOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise modulus (remainder) operation, out = x % y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [mod](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mod", "x", x)
		SDValidation.validateNumerical("mod", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.ModOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and (population) variance for the input variable, for the specified axis<br>
	  ''' </summary>
	  ''' <param name="input"> Input to calculate moments for (NUMERIC type) </param>
	  ''' <param name="axes"> Dimensions to perform calculation over (Size: AtLeast(min=0)) </param>
	  Public Overridable Function moments(ByVal input As SDVariable, ParamArray ByVal axes() As Integer) As SDVariable()
		SDValidation.validateNumerical("moments", "input", input)
		Preconditions.checkArgument(axes.Length >= 0, "axes has incorrect size/length. Expected: axes.length >= 0, got %s", axes.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.Moments(sd,input, axes)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and (population) variance for the input variable, for the specified axis<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="input"> Input to calculate moments for (NUMERIC type) </param>
	  ''' <param name="axes"> Dimensions to perform calculation over (Size: AtLeast(min=0)) </param>
	  Public Overridable Function moments(ByVal names() As String, ByVal input As SDVariable, ParamArray ByVal axes() As Integer) As SDVariable()
		SDValidation.validateNumerical("moments", "input", input)
		Preconditions.checkArgument(axes.Length >= 0, "axes has incorrect size/length. Expected: axes.length >= 0, got %s", axes.Length)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.Moments(sd,input, axes)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Pairwise multiplication operation, out = x * y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mul(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mul", "x", x)
		SDValidation.validateNumerical("mul", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise multiplication operation, out = x * y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mul(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mul", "x", x)
		SDValidation.validateNumerical("mul", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar multiplication operation, out = in * scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mul(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("mul", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar multiplication operation, out = in * scalar<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mul(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("mul", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise negative operation: out = -x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function neg(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("neg", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Negative(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise negative operation: out = -x<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function neg(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("neg", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Negative(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		SDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		SDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		SDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("norm1", "in", [in])
		SDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		SDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		SDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		SDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("norm2", "in", [in])
		SDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		SDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		SDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		SDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("normMax", "in", [in])
		SDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and variance from the sufficient statistics<br>
	  ''' </summary>
	  ''' <param name="counts"> Rank 0 (scalar) value with the total number of values used to calculate the sufficient statistics (NUMERIC type) </param>
	  ''' <param name="means"> Mean-value sufficient statistics: this is the SUM of all data values (NUMERIC type) </param>
	  ''' <param name="variances"> Variaance sufficient statistics: this is the squared sum of all data values (NUMERIC type) </param>
	  ''' <param name="shift"> Shift value, possibly 0, used when calculating the sufficient statistics (for numerical stability) </param>
	  Public Overridable Function normalizeMoments(ByVal counts As SDVariable, ByVal means As SDVariable, ByVal variances As SDVariable, ByVal shift As Double) As SDVariable()
		SDValidation.validateNumerical("normalizeMoments", "counts", counts)
		SDValidation.validateNumerical("normalizeMoments", "means", means)
		SDValidation.validateNumerical("normalizeMoments", "variances", variances)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.NormalizeMoments(sd,counts, means, variances, shift)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and variance from the sufficient statistics<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="counts"> Rank 0 (scalar) value with the total number of values used to calculate the sufficient statistics (NUMERIC type) </param>
	  ''' <param name="means"> Mean-value sufficient statistics: this is the SUM of all data values (NUMERIC type) </param>
	  ''' <param name="variances"> Variaance sufficient statistics: this is the squared sum of all data values (NUMERIC type) </param>
	  ''' <param name="shift"> Shift value, possibly 0, used when calculating the sufficient statistics (for numerical stability) </param>
	  Public Overridable Function normalizeMoments(ByVal names() As String, ByVal counts As SDVariable, ByVal means As SDVariable, ByVal variances As SDVariable, ByVal shift As Double) As SDVariable()
		SDValidation.validateNumerical("normalizeMoments", "counts", counts)
		SDValidation.validateNumerical("normalizeMoments", "means", means)
		SDValidation.validateNumerical("normalizeMoments", "variances", variances)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.NormalizeMoments(sd,counts, means, variances, shift)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Boolean OR operation: elementwise (x != 0) || (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [or](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("or", "x", x)
		SDValidation.validateBool("or", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Boolean OR operation: elementwise (x != 0) || (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [or](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("or", "x", x)
		SDValidation.validateBool("or", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise power function: out = x^value<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("pow", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.Pow(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise power function: out = x^value<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("pow", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.Pow(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise (broadcastable) power function: out = x[i]^y[i]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Power (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("pow", "x", x)
		SDValidation.validateNumerical("pow", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Pow(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise (broadcastable) power function: out = x[i]^y[i]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Power (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("pow", "x", x)
		SDValidation.validateNumerical("pow", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Pow(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		SDValidation.validateNumerical("prod", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		SDValidation.validateNumerical("prod", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		SDValidation.validateNumerical("prod", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("prod", "in", [in])
		SDValidation.validateNumerical("prod", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Rational Tanh Approximation elementwise function, as described in the paper:<br>
	  ''' Compact Convolutional Neural Network Cascade for Face Detection<br>
	  ''' This is a faster Tanh approximation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rationalTanh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rationalTanh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.RationalTanh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Rational Tanh Approximation elementwise function, as described in the paper:<br>
	  ''' Compact Convolutional Neural Network Cascade for Face Detection<br>
	  ''' This is a faster Tanh approximation<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rationalTanh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rationalTanh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.RationalTanh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise reverse division operation, out = y / x<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rdiv(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("rdiv", "x", x)
		SDValidation.validateNumerical("rdiv", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RDivOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise reverse division operation, out = y / x<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rdiv(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("rdiv", "x", x)
		SDValidation.validateNumerical("rdiv", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RDivOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar reverse division operation, out = scalar / in<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rdiv(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("rdiv", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseDivision(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar reverse division operation, out = scalar / in<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rdiv(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("rdiv", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseDivision(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) function: out[i] = 1 / in[i]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reciprocal(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("reciprocal", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Reciprocal(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) function: out[i] = 1 / in[i]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reciprocal(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("reciprocal", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Reciprocal(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Rectified tanh operation: max(0, tanh(in))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rectifiedTanh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rectifiedTanh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.RectifiedTanh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Rectified tanh operation: max(0, tanh(in))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rectifiedTanh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rectifiedTanh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.RectifiedTanh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		SDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		SDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		SDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAMax", "in", [in])
		SDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMax(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		SDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		SDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		SDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAmean", "in", [in])
		SDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		SDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		SDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		SDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceAmin", "in", [in])
		SDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.AMin(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		SDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		SDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		SDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceMax", "in", [in])
		SDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Max(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		SDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		SDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		SDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("reduceMin", "in", [in])
		SDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Min(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise round function: out = round(x).<br>
	  ''' Rounds (up or down depending on value) to the nearest integer value.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function round(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("round", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Round(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise round function: out = round(x).<br>
	  ''' Rounds (up or down depending on value) to the nearest integer value.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function round(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("round", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Round(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) of square root: out = 1.0 / sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsqrt(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rsqrt", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.floating.RSqrt(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) of square root: out = 1.0 / sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsqrt(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("rsqrt", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.floating.RSqrt(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise reverse subtraction operation, out = y - x<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsub(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("rsub", "x", x)
		SDValidation.validateNumerical("rsub", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RSubOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise reverse subtraction operation, out = y - x<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsub(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("rsub", "x", x)
		SDValidation.validateNumerical("rsub", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RSubOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar reverse subtraction operation, out = scalar - in<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsub(ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("rsub", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseSubtraction(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar reverse subtraction operation, out = scalar - in<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsub(ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("rsub", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseSubtraction(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Set the diagonal value to the specified values<br>
	  ''' If input is<br>
	  ''' [ a, b, c]<br>
	  ''' [ d, e, f]<br>
	  ''' [ g, h, i]<br>
	  ''' and diag = [ 1, 2, 3] then output is<br>
	  ''' [ 1, b, c]<br>
	  ''' [ d, 2, f]<br>
	  ''' [ g, h, 3]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="diag"> Diagonal (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function setDiag(ByVal [in] As SDVariable, ByVal diag As SDVariable) As SDVariable
		SDValidation.validateNumerical("setDiag", "in", [in])
		SDValidation.validateNumerical("setDiag", "diag", diag)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixSetDiag(sd,[in], diag)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Set the diagonal value to the specified values<br>
	  ''' If input is<br>
	  ''' [ a, b, c]<br>
	  ''' [ d, e, f]<br>
	  ''' [ g, h, i]<br>
	  ''' and diag = [ 1, 2, 3] then output is<br>
	  ''' [ 1, b, c]<br>
	  ''' [ d, 2, f]<br>
	  ''' [ g, h, 3]<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="diag"> Diagonal (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function setDiag(ByVal name As String, ByVal [in] As SDVariable, ByVal diag As SDVariable) As SDVariable
		SDValidation.validateNumerical("setDiag", "in", [in])
		SDValidation.validateNumerical("setDiag", "diag", diag)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixSetDiag(sd,[in], diag)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		SDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		SDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		SDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("shannonEntropy", "in", [in])
		SDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise sign (signum) function:<br>
	  ''' out = -1 if in < 0<br>
	  ''' out = 0 if in = 0<br>
	  ''' out = 1 if in > 0<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sign(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sign", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Sign(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise sign (signum) function:<br>
	  ''' out = -1 if in < 0<br>
	  ''' out = 0 if in = 0<br>
	  ''' out = 1 if in > 0<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sign(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sign", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Sign(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise sine operation: out = sin(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sin(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sin", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sin(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise sine operation: out = sin(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sin(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sin", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sin(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise sinh (hyperbolic sine) operation: out = sinh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sinh(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sinh", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sinh(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise sinh (hyperbolic sine) operation: out = sinh(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sinh(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sinh", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Sinh(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise square root function: out = sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sqrt(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sqrt", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise square root function: out = sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sqrt(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("sqrt", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Element-wise square function: out = x^2<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function square(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("square", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.same.Square(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Element-wise square function: out = x^2<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function square(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("square", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.same.Square(sd,x)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise squared difference operation.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function squaredDifference(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("squaredDifference", "x", x)
		SDValidation.validateNumerical("squaredDifference", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SquaredDifferenceOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise squared difference operation.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function squaredDifference(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("squaredDifference", "x", x)
		SDValidation.validateNumerical("squaredDifference", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SquaredDifferenceOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		SDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		SDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		SDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("squaredNorm", "in", [in])
		SDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Standardize input variable along given axis<br>
	  ''' <para><br>
	  ''' out = (x - mean) / stdev<br>
	  ''' </para>
	  ''' <para><br>
	  ''' with mean and stdev being calculated along the given dimension.<br>
	  ''' </para>
	  ''' <para><br>
	  ''' For example: given x as a mini batch of the shape [numExamples, exampleLength]:<br>
	  ''' <ul> <br>
	  ''' <li>use dimension 1 too use the statistics (mean, stdev) for each example</li><br>
	  ''' <li>use dimension 0 if you want to use the statistics for each column across all examples</li><br>
	  ''' <li>use dimensions 0,1 if you want to use the statistics across all columns and examples</li><br>
	  ''' </ul><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function standardize(ByVal x As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("standardize", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Standardize(sd,x, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Standardize input variable along given axis<br>
	  ''' <para><br>
	  ''' out = (x - mean) / stdev<br>
	  ''' </para>
	  ''' <para><br>
	  ''' with mean and stdev being calculated along the given dimension.<br>
	  ''' </para>
	  ''' <para><br>
	  ''' For example: given x as a mini batch of the shape [numExamples, exampleLength]:<br>
	  ''' <ul> <br>
	  ''' <li>use dimension 1 too use the statistics (mean, stdev) for each example</li><br>
	  ''' <li>use dimension 0 if you want to use the statistics for each column across all examples</li><br>
	  ''' <li>use dimensions 0,1 if you want to use the statistics across all columns and examples</li><br>
	  ''' </ul><br>
	  ''' 
	  ''' </para>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function standardize(ByVal name As String, ByVal x As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("standardize", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Standardize(sd,x, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise step function:<br>
	  ''' out(x) = 1 if x >= cutoff<br>
	  ''' out(x) = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [step](ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("step", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.Step(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise step function:<br>
	  ''' out(x) = 1 if x >= cutoff<br>
	  ''' out(x) = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [step](ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("step", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.Step(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Pairwise subtraction operation, out = x - y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [sub](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("sub", "x", x)
		SDValidation.validateNumerical("sub", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SubOp(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Pairwise subtraction operation, out = x - y<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [sub](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("sub", "x", x)
		SDValidation.validateNumerical("sub", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SubOp(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Scalar subtraction operation, out = in - scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [sub](ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("sub", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.scalar.ScalarSubtraction(sd,x, value)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Scalar subtraction operation, out = in - scalar<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [sub](ByVal name As String, ByVal x As SDVariable, ByVal value As Double) As SDVariable
		SDValidation.validateNumerical("sub", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.scalar.ScalarSubtraction(sd,x, value)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], keepDims, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal name As String, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], keepDims, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], False, dimensions)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal name As String, ByVal [in] As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], False, dimensions)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		SDValidation.validateNumerical("sum", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], dimensions, keepDims)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		SDValidation.validateNumerical("sum", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], dimensions, keepDims)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		SDValidation.validateNumerical("sum", "dimensions", dimensions)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], dimensions, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal name As String, ByVal [in] As SDVariable, ByVal dimensions As SDVariable) As SDVariable
		SDValidation.validateNumerical("sum", "in", [in])
		SDValidation.validateNumerical("sum", "dimensions", dimensions)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(sd,[in], dimensions, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Elementwise tangent operation: out = tan(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tan(ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("tan", "x", x)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.strict.Tan(sd,x)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Elementwise tangent operation: out = tan(x)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tan(ByVal name As String, ByVal x As SDVariable) As SDVariable
		SDValidation.validateNumerical("tan", "x", x)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.strict.Tan(sd,x)).outputVariable()
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

	  ''' <summary>
	  ''' Matrix trace operation<br>
	  ''' For rank 2 matrices, the output is a scalar vith the trace - i.e., sum of the main diagonal.<br>
	  ''' For higher rank inputs, output[a,b,c] = trace(in[a,b,c,:,:])<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Trace (NUMERIC type) </returns>
	  Public Overridable Function trace(ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("trace", "in", [in])
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Trace(sd,[in])).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Matrix trace operation<br>
	  ''' For rank 2 matrices, the output is a scalar vith the trace - i.e., sum of the main diagonal.<br>
	  ''' For higher rank inputs, output[a,b,c] = trace(in[a,b,c,:,:])<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Trace (NUMERIC type) </returns>
	  Public Overridable Function trace(ByVal name As String, ByVal [in] As SDVariable) As SDVariable
		SDValidation.validateNumerical("trace", "in", [in])
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Trace(sd,[in])).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Boolean XOR (exclusive OR) operation: elementwise (x != 0) XOR (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [xor](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("xor", "x", x)
		SDValidation.validateBool("xor", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Boolean XOR (exclusive OR) operation: elementwise (x != 0) XOR (y != 0)<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (BOOL type) </param>
	  ''' <param name="y"> Input 2 (BOOL type) </param>
	  ''' <returns> output INDArray with values 0 and 1 based on where the condition is satisfied (BOOL type) </returns>
	  Public Overridable Function [xor](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateBool("xor", "x", x)
		SDValidation.validateBool("xor", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Full array zero fraction array reduction operation, optionally along specified dimensions: out = (count(x == 0) / length(x))<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank 0 (scalar) (NUMERIC type) </returns>
	  Public Overridable Function zeroFraction(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("zeroFraction", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.ZeroFraction(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Full array zero fraction array reduction operation, optionally along specified dimensions: out = (count(x == 0) / length(x))<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank 0 (scalar) (NUMERIC type) </returns>
	  Public Overridable Function zeroFraction(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("zeroFraction", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.ZeroFraction(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace