import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PartitionMode = org.nd4j.enums.PartitionMode
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.factory.ops

	Public Class NDMath
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Clips tensor values to a maximum average L2-norm.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="clipValue"> Value for clipping </param>
	  ''' <param name="dimensions"> Dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function clipByAvgNorm(ByVal x As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("ClipByAvgNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByAvgNorm(x, clipValue, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Looks up ids in a list of embedding tensors.<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="indices"> A Tensor containing the ids to be looked up. (INT type) </param>
	  ''' <param name="PartitionMode"> partition_mode == 0 - i.e. 'mod' , 1 - 'div' </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function embeddingLookup(ByVal x As INDArray, ByVal indices As INDArray, ByVal PartitionMode As PartitionMode) As INDArray
		NDValidation.validateNumerical("EmbeddingLookup", "x", x)
		NDValidation.validateInteger("EmbeddingLookup", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.tensorops.EmbeddingLookup(x, indices, PartitionMode))(0)
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ByVal x() As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(x, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Return array of max elements indices with along tensor dimensions <br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <returns> output Array max elements indices with along dimensions. (INT type) </returns>
	  Public Overridable Function mergeMaxIndex(ParamArray ByVal x() As INDArray) As INDArray
		NDValidation.validateNumerical("MergeMaxIndex", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex(x, DataType.INT))(0)
	  End Function

	  ''' <summary>
	  ''' Elementwise absolute value operation: out = abs(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function abs(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("abs", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Abs(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise acos (arccosine, inverse cosine) operation: out = arccos(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acos(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("acos", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ACos(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise acosh (inverse hyperbolic cosine) function: out = acosh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function acosh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("acosh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ACosh(x))
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
	  Public Overridable Function add(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("add", "x", x)
		NDValidation.validateNumerical("add", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar add operation, out = in + scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function add(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("add", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarAdd(x, value))
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
	  Public Overridable Function [and](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateBool("and", "x", x)
		NDValidation.validateBool("and", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.And(x, y))
	  End Function

	  ''' <summary>
	  ''' Elementwise asin (arcsin, inverse sine) operation: out = arcsin(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asin(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("asin", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ASin(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise asinh (inverse hyperbolic sine) function: out = asinh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function asinh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("asinh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ASinh(x))
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.ASum([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute sum array reduction operation, optionally along specified dimensions: out = sum(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function asum(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("asum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.ASum([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = arctangent(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("atan", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ATan(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise atan (arctangent, inverse tangent) operation: out = atan2(x,y).<br>
	  ''' Similar to atan(y/x) but sigts of x and y are used to determine the location of the result<br>
	  ''' </summary>
	  ''' <param name="y"> Input Y variable (NUMERIC type) </param>
	  ''' <param name="x"> Input X variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atan2(ByVal y As INDArray, ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("atan2", "y", y)
		NDValidation.validateNumerical("atan2", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ATan2(y, x))(0)
	  End Function

	  ''' <summary>
	  ''' Elementwise atanh (inverse hyperbolic tangent) function: out = atanh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function atanh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("atanh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.ATanh(x))
	  End Function

	  ''' <summary>
	  ''' Bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> input (NUMERIC type) </param>
	  ''' <param name="shift"> shift value (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShift(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateNumerical("bitShift", "x", x)
		NDValidation.validateNumerical("bitShift", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Right bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRight(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateNumerical("bitShiftRight", "x", x)
		NDValidation.validateNumerical("bitShiftRight", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Cyclic bit shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> shift argy=ument (NUMERIC type) </param>
	  ''' <returns> output shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotl(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateNumerical("bitShiftRotl", "x", x)
		NDValidation.validateNumerical("bitShiftRotl", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Cyclic right shift operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input tensor (NUMERIC type) </param>
	  ''' <param name="shift"> Shift argument (NUMERIC type) </param>
	  ''' <returns> output Shifted output (NUMERIC type) </returns>
	  Public Overridable Function bitShiftRotr(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateNumerical("bitShiftRotr", "x", x)
		NDValidation.validateNumerical("bitShiftRotr", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise ceiling function: out = ceil(x).<br>
	  ''' Rounds each value up to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function ceil(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("ceil", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Ceil(x))
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
	  Public Overridable Function clipByNorm(ByVal x As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("clipByNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByNorm(x, clipValue, dimensions))(0)
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
	  Public Overridable Function clipByValue(ByVal x As INDArray, ByVal clipValueMin As Double, ByVal clipValueMax As Double) As INDArray
		NDValidation.validateNumerical("clipByValue", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByValue(x, clipValueMin, clipValueMax))(0)
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
	  Public Overridable Function confusionMatrix(ByVal labels As INDArray, ByVal pred As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("confusionMatrix", "labels", labels)
		NDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(labels, pred, dataType))(0)
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
	  Public Overridable Function confusionMatrix(ByVal labels As INDArray, ByVal pred As INDArray, ByVal numClasses As Integer) As INDArray
		NDValidation.validateNumerical("confusionMatrix", "labels", labels)
		NDValidation.validateNumerical("confusionMatrix", "pred", pred)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(labels, pred, numClasses))(0)
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
	  Public Overridable Function confusionMatrix(ByVal labels As INDArray, ByVal pred As INDArray, ByVal weights As INDArray) As INDArray
		NDValidation.validateNumerical("confusionMatrix", "labels", labels)
		NDValidation.validateNumerical("confusionMatrix", "pred", pred)
		NDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(labels, pred, weights))(0)
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
	  Public Overridable Function confusionMatrix(ByVal labels As INDArray, ByVal pred As INDArray, ByVal weights As INDArray, ByVal numClasses As Integer) As INDArray
		NDValidation.validateNumerical("confusionMatrix", "labels", labels)
		NDValidation.validateNumerical("confusionMatrix", "pred", pred)
		NDValidation.validateNumerical("confusionMatrix", "weights", weights)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ConfusionMatrix(labels, pred, weights, numClasses))(0)
	  End Function

	  ''' <summary>
	  ''' Elementwise cosine operation: out = cos(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cos(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("cos", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Cos(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise cosh (hyperbolic cosine) operation: out = cosh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cosh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("cosh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Cosh(x))
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
	  Public Overridable Function cosineDistance(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("cosineDistance", "x", x)
		NDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(x, y, keepDims, isComplex, dimensions))
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
	  Public Overridable Function cosineDistance(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("cosineDistance", "x", x)
		NDValidation.validateNumerical("cosineDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.CosineDistance(x, y, False, False, dimensions))
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
	  Public Overridable Function cosineSimilarity(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("cosineSimilarity", "x", x)
		NDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(x, y, keepDims, isComplex, dimensions))
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
	  Public Overridable Function cosineSimilarity(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("cosineSimilarity", "x", x)
		NDValidation.validateNumerical("cosineSimilarity", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.CosineSimilarity(x, y, False, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Count non zero array reduction operation, optionally along specified dimensions: out = count(x != 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countNonZero(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("countNonZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.CountNonZero([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Count zero array reduction operation, optionally along specified dimensions: out = count(x == 0)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function countZero(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("countZero", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.CountZero([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Returns the pair-wise cross product of equal size arrays a and b: a x b = ||a||x||b|| sin(theta).<br>
	  ''' Can take rank 1 or above inputs (of equal shapes), but note that the last dimension must have dimension 3<br>
	  ''' </summary>
	  ''' <param name="a"> First input (NUMERIC type) </param>
	  ''' <param name="b"> Second input (NUMERIC type) </param>
	  ''' <returns> output Element-wise cross product (NUMERIC type) </returns>
	  Public Overridable Function cross(ByVal a As INDArray, ByVal b As INDArray) As INDArray
		NDValidation.validateNumerical("cross", "a", a)
		NDValidation.validateNumerical("cross", "b", b)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Cross(a, b))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise cube function: out = x^3<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cube(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("cube", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Cube(x))
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
	  Public Overridable Function diag(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("diag", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Diag(x))(0)
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
	  Public Overridable Function diagPart(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("diagPart", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.DiagPart(x))(0)
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
	  Public Overridable Function div(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("div", "x", x)
		NDValidation.validateNumerical("div", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.DivOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar division operation, out = in / scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function div(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("div", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarDivision(x, value))
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Entropy reduction: -sum(x * log(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function entropy(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("entropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Entropy([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Element-wise Gaussian error function - out = erf(in)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erf(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("erf", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Erf(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise complementary Gaussian error function - out = erfc(in) = 1 - erf(in)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function erfc(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("erfc", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Erfc(x))
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
	  Public Overridable Function euclideanDistance(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("euclideanDistance", "x", x)
		NDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(x, y, keepDims, isComplex, dimensions))
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
	  Public Overridable Function euclideanDistance(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("euclideanDistance", "x", x)
		NDValidation.validateNumerical("euclideanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.EuclideanDistance(x, y, False, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Elementwise exponent function: out = exp(x) = 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function exp(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("exp", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Exp(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise 1.0 - exponent function: out = 1.0 - exp(x) = 1.0 - 2.71828...^x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function expm1(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("expm1", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Expm1(x))
	  End Function

	  ''' <summary>
	  ''' Generate an identity matrix with the specified number of rows and columns.<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As Integer) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Eye(rows))(0)
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int, int, DataType) but with the default datatype, Eye.DEFAULT_DTYPE<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows </param>
	  ''' <param name="cols"> Number of columns </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As Integer, ByVal cols As Integer) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Eye(rows, cols))(0)
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
	  Public Overridable Function eye(ByVal rows As Integer, ByVal cols As Integer, ByVal dataType As DataType, ParamArray ByVal dimensions() As Integer) As INDArray
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Eye(rows, cols, dataType, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' As per eye(int, int) bit with the number of rows/columns specified as scalar INDArrays<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <param name="cols"> Number of columns (INT type) </param>
	  ''' <returns> output Identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As INDArray, ByVal cols As INDArray) As INDArray
		NDValidation.validateInteger("eye", "rows", rows)
		NDValidation.validateInteger("eye", "cols", cols)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Eye(rows, cols))(0)
	  End Function

	  ''' <summary>
	  ''' As per eye(String, int) but with the number of rows specified as a scalar INDArray<br>
	  ''' </summary>
	  ''' <param name="rows"> Number of rows (INT type) </param>
	  ''' <returns> output SDVaribable identity matrix (NUMERIC type) </returns>
	  Public Overridable Function eye(ByVal rows As INDArray) As INDArray
		NDValidation.validateInteger("eye", "rows", rows)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Eye(rows))(0)
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
	  Public Overridable Function firstIndex(ByVal [in] As INDArray, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex([in], False, condition, dimensions))
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
	  Public Overridable Function firstIndex(ByVal [in] As INDArray, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("firstIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex([in], keepDims, condition, dimensions))
	  End Function

	  ''' <summary>
	  ''' Element-wise floor function: out = floor(x).<br>
	  ''' Rounds each value down to the nearest integer value (if not already an integer)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floor(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("floor", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Floor(x))
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
	  Public Overridable Function floorDiv(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("floorDiv", "x", x)
		NDValidation.validateNumerical("floorDiv", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorDivOp(x, y))(0)
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
	  Public Overridable Function floorMod(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("floorMod", "x", x)
		NDValidation.validateNumerical("floorMod", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.FloorModOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar floor modulus operation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function floorMod(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("floorMod", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarFMod(x, value))
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
	  Public Overridable Function hammingDistance(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("hammingDistance", "x", x)
		NDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(x, y, keepDims, isComplex, dimensions))
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
	  Public Overridable Function hammingDistance(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("hammingDistance", "x", x)
		NDValidation.validateNumerical("hammingDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.HammingDistance(x, y, False, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax([in], False, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Index of the max absolute value: argmax(abs(in))<br>
	  ''' see argmax(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamax(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("iamax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax([in], keepDims, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin([in], False, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Index of the min absolute value: argmin(abs(in))<br>
	  ''' see argmin(String, INDArray, boolean, int...)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function iamin(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("iamin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin([in], keepDims, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Is finite operation: elementwise isFinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isFinite(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isFinite", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.bool.IsFinite(x))
	  End Function

	  ''' <summary>
	  ''' Is infinite operation: elementwise isInfinite(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isInfinite(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isInfinite", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.bool.IsInf(x))
	  End Function

	  ''' <summary>
	  ''' Is maximum operation: elementwise x == max(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isMax(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isMax", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.any.IsMax(x))(0)
	  End Function

	  ''' <summary>
	  ''' Is Not a Number operation: elementwise isNaN(x)<br>
	  ''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or<br>
	  ''' value 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function isNaN(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isNaN", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.bool.IsNaN(x))
	  End Function

	  ''' <summary>
	  ''' Is the array non decreasing?<br>
	  ''' An array is non-decreasing if for every valid i, x[i] <= x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if non-decreasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isNonDecreasing(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isNonDecreasing", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.IsNonDecreasing(x))(0)
	  End Function

	  ''' <summary>
	  ''' Is the array strictly increasing?<br>
	  ''' An array is strictly increasing if for every valid i, x[i] < x[i+1]. For Rank 2+ arrays, values are compared<br>
	  ''' in 'c' (row major) order<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Scalar variable with value 1 if strictly increasing, or 0 otherwise (NUMERIC type) </returns>
	  Public Overridable Function isStrictlyIncreasing(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isStrictlyIncreasing", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.IsStrictlyIncreasing(x))(0)
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
	  Public Overridable Function jaccardDistance(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("jaccardDistance", "x", x)
		NDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(x, y, keepDims, isComplex, dimensions))
	  End Function

	  ''' <summary>
	  ''' Jaccard similarity reduction operation. The output contains the Jaccard distance for each<br>
	  '''                 tensor along the specified dimensions.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function jaccardDistance(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("jaccardDistance", "x", x)
		NDValidation.validateNumerical("jaccardDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.JaccardDistance(x, y, False, False, dimensions))
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
	  Public Overridable Function lastIndex(ByVal [in] As INDArray, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex([in], False, condition, dimensions))
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
	  Public Overridable Function lastIndex(ByVal [in] As INDArray, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("lastIndex", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex([in], keepDims, condition, dimensions))
	  End Function

	  ''' <summary>
	  ''' Calculates difference between inputs X and Y.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable X (NUMERIC type) </param>
	  ''' <param name="y"> Input variable Y (NUMERIC type) </param>
	  Public Overridable Function listDiff(ByVal x As INDArray, ByVal y As INDArray) As INDArray()
		NDValidation.validateNumerical("listDiff", "x", x)
		NDValidation.validateNumerical("listDiff", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ListDiff(x, y))
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (base e - natural logarithm): out = log(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("log", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Log(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise logarithm function (with specified base): out = log_{base}(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="base"> Logarithm base </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log(ByVal x As INDArray, ByVal base As Double) As INDArray
		NDValidation.validateNumerical("log", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.LogX(x, base))
	  End Function

	  ''' <summary>
	  ''' Elementwise natural logarithm function: out = log_e (1 + x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function log1p(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("log1p", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Log1p(x))
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Log entropy reduction: log(-sum(x * log(x)))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function logEntropy(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("logEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.LogEntropy([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Log-sum-exp reduction (optionally along dimension).<br>
	  ''' Computes log(sum(exp(x))<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Optional dimensions to reduce along (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function logSumExp(ByVal input As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("logSumExp", "input", input)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.custom.LogSumExp(input, dimensions))(0)
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
	  Public Overridable Function manhattanDistance(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("manhattanDistance", "x", x)
		NDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(x, y, keepDims, isComplex, dimensions))
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
	  Public Overridable Function manhattanDistance(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("manhattanDistance", "x", x)
		NDValidation.validateNumerical("manhattanDistance", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.ManhattanDistance(x, y, False, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Matrix determinant op. For 2D input, this returns the standard matrix determinant.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix determinant is returned for each <br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix determinant variable (NUMERIC type) </returns>
	  Public Overridable Function matrixDeterminant(ByVal [in] As INDArray) As INDArray
		NDValidation.validateNumerical("matrixDeterminant", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixDeterminant([in]))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix inverse op. For 2D input, this returns the standard matrix inverse.<br>
	  ''' For higher dimensional input with shape [..., m, m] the matrix inverse is returned for each<br>
	  ''' shape [m,m] sub-matrix.<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <returns> output Matrix inverse variable (NUMERIC type) </returns>
	  Public Overridable Function matrixInverse(ByVal [in] As INDArray) As INDArray
		NDValidation.validateNumerical("matrixInverse", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixInverse([in]))(0)
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
	  Public Overridable Function max(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("max", "x", x)
		NDValidation.validateNumerical("max", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Max(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("mean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("mean", "in", [in])
		NDValidation.validateNumerical("mean", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("mean", "in", [in])
		NDValidation.validateNumerical("mean", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Merge add function: merges an arbitrary number of equal shaped arrays using element-wise addition:<br>
	  ''' out = sum_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAdd(ParamArray ByVal inputs() As INDArray) As INDArray
		NDValidation.validateNumerical("mergeAdd", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MergeAddOp(inputs))(0)
	  End Function

	  ''' <summary>
	  ''' Merge average function: merges an arbitrary number of equal shaped arrays using element-wise mean operation:<br>
	  ''' out = mean_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeAvg(ParamArray ByVal inputs() As INDArray) As INDArray
		NDValidation.validateNumerical("mergeAvg", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.MergeAvg(inputs))(0)
	  End Function

	  ''' <summary>
	  ''' Merge max function: merges an arbitrary number of equal shaped arrays using element-wise maximum operation:<br>
	  ''' out = max_i in[i]<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mergeMax(ParamArray ByVal inputs() As INDArray) As INDArray
		NDValidation.validateNumerical("mergeMax", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.MergeMax(inputs))(0)
	  End Function

	  ''' <summary>
	  ''' Broadcasts parameters for evaluation on an N-D grid.<br>
	  ''' </summary>
	  ''' <param name="inputs">  (NUMERIC type) </param>
	  ''' <param name="cartesian">  </param>
	  Public Overridable Function meshgrid(ByVal inputs() As INDArray, ByVal cartesian As Boolean) As INDArray()
		NDValidation.validateNumerical("meshgrid", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 0, "inputs has incorrect size/length. Expected: inputs.length >= 0, got %s", inputs.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.MeshGrid(inputs, cartesian))
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
	  Public Overridable Function min(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("min", "x", x)
		NDValidation.validateNumerical("min", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Min(x, y))(0)
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
	  Public Overridable Function [mod](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("mod", "x", x)
		NDValidation.validateNumerical("mod", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.ModOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and (population) variance for the input variable, for the specified axis<br>
	  ''' </summary>
	  ''' <param name="input"> Input to calculate moments for (NUMERIC type) </param>
	  ''' <param name="axes"> Dimensions to perform calculation over (Size: AtLeast(min=0)) </param>
	  Public Overridable Function moments(ByVal input As INDArray, ParamArray ByVal axes() As Integer) As INDArray()
		NDValidation.validateNumerical("moments", "input", input)
		Preconditions.checkArgument(axes.Length >= 0, "axes has incorrect size/length. Expected: axes.length >= 0, got %s", axes.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Moments(input, axes))
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
	  Public Overridable Function mul(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("mul", "x", x)
		NDValidation.validateNumerical("mul", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar multiplication operation, out = in * scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function mul(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("mul", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication(x, value))
	  End Function

	  ''' <summary>
	  ''' Elementwise negative operation: out = -x<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function neg(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("neg", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Negative(x))
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm1", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("norm1", "in", [in])
		NDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Sum of absolute differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("norm1", "in", [in])
		NDValidation.validateNumerical("norm1", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm2", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("norm2", "in", [in])
		NDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Euclidean norm: euclidean distance of a vector from the origin<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("norm2", "in", [in])
		NDValidation.validateNumerical("norm2", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("normMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("normMax", "in", [in])
		NDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Differences between max absolute value<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function normMax(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("normMax", "in", [in])
		NDValidation.validateNumerical("normMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Calculate the mean and variance from the sufficient statistics<br>
	  ''' </summary>
	  ''' <param name="counts"> Rank 0 (scalar) value with the total number of values used to calculate the sufficient statistics (NUMERIC type) </param>
	  ''' <param name="means"> Mean-value sufficient statistics: this is the SUM of all data values (NUMERIC type) </param>
	  ''' <param name="variances"> Variaance sufficient statistics: this is the squared sum of all data values (NUMERIC type) </param>
	  ''' <param name="shift"> Shift value, possibly 0, used when calculating the sufficient statistics (for numerical stability) </param>
	  Public Overridable Function normalizeMoments(ByVal counts As INDArray, ByVal means As INDArray, ByVal variances As INDArray, ByVal shift As Double) As INDArray()
		NDValidation.validateNumerical("normalizeMoments", "counts", counts)
		NDValidation.validateNumerical("normalizeMoments", "means", means)
		NDValidation.validateNumerical("normalizeMoments", "variances", variances)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.NormalizeMoments(counts, means, variances, shift))
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
	  Public Overridable Function [or](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateBool("or", "x", x)
		NDValidation.validateBool("or", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Or(x, y))
	  End Function

	  ''' <summary>
	  ''' Element-wise power function: out = x^value<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("pow", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.Pow(x, value))
	  End Function

	  ''' <summary>
	  ''' Element-wise (broadcastable) power function: out = x[i]^y[i]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Power (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function pow(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("pow", "x", x)
		NDValidation.validateNumerical("pow", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Pow(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("prod", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("prod", "in", [in])
		NDValidation.validateNumerical("prod", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' The product of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("prod", "in", [in])
		NDValidation.validateNumerical("prod", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Rational Tanh Approximation elementwise function, as described in the paper:<br>
	  ''' Compact Convolutional Neural Network Cascade for Face Detection<br>
	  ''' This is a faster Tanh approximation<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rationalTanh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("rationalTanh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.RationalTanh(x))
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
	  Public Overridable Function rdiv(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("rdiv", "x", x)
		NDValidation.validateNumerical("rdiv", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RDivOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar reverse division operation, out = scalar / in<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rdiv(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("rdiv", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseDivision(x, value))
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) function: out[i] = 1 / in[i]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reciprocal(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("reciprocal", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Reciprocal(x))
	  End Function

	  ''' <summary>
	  ''' Rectified tanh operation: max(0, tanh(in))<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rectifiedTanh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("rectifiedTanh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.RectifiedTanh(x))
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMax([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMax([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("reduceAMax", "in", [in])
		NDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMax([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Absolute max array reduction operation, optionally along specified dimensions: out = max(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAMax(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("reduceAMax", "in", [in])
		NDValidation.validateNumerical("reduceAMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMax([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAmean", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("reduceAmean", "in", [in])
		NDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Absolute mean array reduction operation, optionally along specified dimensions: out = mean(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmean(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("reduceAmean", "in", [in])
		NDValidation.validateNumerical("reduceAmean", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.AMean([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMin([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceAmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMin([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("reduceAmin", "in", [in])
		NDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMin([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Absolute min array reduction operation, optionally along specified dimensions: out = min(abs(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceAmin(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("reduceAmin", "in", [in])
		NDValidation.validateNumerical("reduceAmin", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.AMin([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' The max of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceMax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("reduceMax", "in", [in])
		NDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' The max of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMax(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("reduceMax", "in", [in])
		NDValidation.validateNumerical("reduceMax", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' The minimum of an array along each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reduceMin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("reduceMin", "in", [in])
		NDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' The minimum of an array long each dimension<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function reduceMin(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("reduceMin", "in", [in])
		NDValidation.validateNumerical("reduceMin", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Element-wise round function: out = round(x).<br>
	  ''' Rounds (up or down depending on value) to the nearest integer value.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function round(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("round", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Round(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise reciprocal (inverse) of square root: out = 1.0 / sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsqrt(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("rsqrt", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.floating.RSqrt(x))
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
	  Public Overridable Function rsub(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("rsub", "x", x)
		NDValidation.validateNumerical("rsub", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RSubOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar reverse subtraction operation, out = scalar - in<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function rsub(ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("rsub", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarReverseSubtraction(x, value))
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
	  Public Overridable Function setDiag(ByVal [in] As INDArray, ByVal diag As INDArray) As INDArray
		NDValidation.validateNumerical("setDiag", "in", [in])
		NDValidation.validateNumerical("setDiag", "diag", diag)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixSetDiag([in], diag))(0)
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("shannonEntropy", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("shannonEntropy", "in", [in])
		NDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Shannon Entropy reduction: -sum(x * log2(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function shannonEntropy(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("shannonEntropy", "in", [in])
		NDValidation.validateNumerical("shannonEntropy", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.ShannonEntropy([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Element-wise sign (signum) function:<br>
	  ''' out = -1 if in < 0<br>
	  ''' out = 0 if in = 0<br>
	  ''' out = 1 if in > 0<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sign(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("sign", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Sign(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise sine operation: out = sin(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sin(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("sin", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Sin(x))
	  End Function

	  ''' <summary>
	  ''' Elementwise sinh (hyperbolic sine) operation: out = sinh(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sinh(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("sinh", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Sinh(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise square root function: out = sqrt(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sqrt(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("sqrt", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.floating.Sqrt(x))
	  End Function

	  ''' <summary>
	  ''' Element-wise square function: out = x^2<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function square(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("square", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Square(x))
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
	  Public Overridable Function squaredDifference(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("squaredDifference", "x", x)
		NDValidation.validateNumerical("squaredDifference", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SquaredDifferenceOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("squaredNorm", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("squaredNorm", "in", [in])
		NDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Sum of squared differences.<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("squaredNorm", "in", [in])
		NDValidation.validateNumerical("squaredNorm", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm([in], dimensions, False))
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
	  Public Overridable Function standardize(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("standardize", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 1, "dimensions has incorrect size/length. Expected: dimensions.length >= 1, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Standardize(x, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Elementwise step function:<br>
	  ''' out(x) = 1 if x >= cutoff<br>
	  ''' out(x) = 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [step](ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("step", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.Step(x, value))
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
	  Public Overridable Function [sub](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("sub", "x", x)
		NDValidation.validateNumerical("sub", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.SubOp(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Scalar subtraction operation, out = in - scalar<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value for op </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function [sub](ByVal x As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("sub", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarSubtraction(x, value))
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum([in], keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("sum", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum([in], False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <param name="keepDims"> Whether to keep the original  dimensions or produce a shrunk array with less dimensions </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean) As INDArray
		NDValidation.validateNumerical("sum", "in", [in])
		NDValidation.validateNumerical("sum", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum([in], dimensions, keepDims))
	  End Function

	  ''' <summary>
	  ''' Sum of an array, optionally along specified dimensions: out = sum(x))<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce along (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal [in] As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("sum", "in", [in])
		NDValidation.validateNumerical("sum", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum([in], dimensions, False))
	  End Function

	  ''' <summary>
	  ''' Elementwise tangent operation: out = tan(x)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tan(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("tan", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.strict.Tan(x))
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

	  ''' <summary>
	  ''' Matrix trace operation<br>
	  ''' For rank 2 matrices, the output is a scalar vith the trace - i.e., sum of the main diagonal.<br>
	  ''' For higher rank inputs, output[a,b,c] = trace(in[a,b,c,:,:])<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Trace (NUMERIC type) </returns>
	  Public Overridable Function trace(ByVal [in] As INDArray) As INDArray
		NDValidation.validateNumerical("trace", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Trace([in]))(0)
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
	  Public Overridable Function [xor](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateBool("xor", "x", x)
		NDValidation.validateBool("xor", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor(x, y))
	  End Function

	  ''' <summary>
	  ''' Full array zero fraction array reduction operation, optionally along specified dimensions: out = (count(x == 0) / length(x))<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Reduced array of rank 0 (scalar) (NUMERIC type) </returns>
	  Public Overridable Function zeroFraction(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("zeroFraction", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.ZeroFraction(input))(0)
	  End Function
	End Class

End Namespace