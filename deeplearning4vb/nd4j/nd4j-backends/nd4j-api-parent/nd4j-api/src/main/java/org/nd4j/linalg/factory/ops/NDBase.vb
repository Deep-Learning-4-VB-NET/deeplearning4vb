import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports Preconditions = org.nd4j.common.base.Preconditions
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

	Public Class NDBase
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Boolean and array reduction operation, optionally along specified dimensions<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NDARRAY type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (BOOL type) </returns>
	  Public Overridable Function all(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.bool.All(x, dimensions))
	  End Function

	  ''' <summary>
	  ''' Boolean or array reduction operation, optionally along specified dimensions<br>
	  ''' </summary>
	  ''' <param name="x">  Input variable (NDARRAY type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (BOOL type) </returns>
	  Public Overridable Function any(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.bool.Any(x, dimensions))
	  End Function

	  ''' <summary>
	  ''' Argmax array reduction operation, optionally along specified dimensions.<br>
	  ''' Output values are the index of the maximum value of each slice along the specified dimension.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or
	  '''  of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function argmax(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("argmax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax([in], keepDims, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Argmax array reduction operation, optionally along specified dimensions.<br>
	  ''' Output values are the index of the maximum value of each slice along the specified dimension.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or
	  '''  of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function argmax(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("argmax", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax([in], False, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Argmin array reduction operation, optionally along specified dimensions.<br>
	  ''' Output values are the index of the minimum value of each slice along the specified dimension.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function argmin(ByVal [in] As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("argmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin([in], keepDims, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Argmin array reduction operation, optionally along specified dimensions.<br>
	  ''' Output values are the index of the minimum value of each slice along the specified dimension.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function argmin(ByVal [in] As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("argmin", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin([in], False, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix multiply a batch of matrices. matricesA and matricesB have to be arrays of same<br>
	  ''' length and each pair taken from these sets has to have dimensions (M, N) and (N, K),<br>
	  ''' respectively. If transposeA is true, matrices from matricesA will have shape (N, M) instead.<br>
	  ''' Likewise, if transposeB is true, matrices from matricesB will have shape (K, N).<br>
	  ''' <br>
	  ''' The result of this operation will be a batch of multiplied matrices. The<br>
	  ''' result has the same length as both input batches and each output matrix is of shape (M, K).<br>
	  ''' </summary>
	  ''' <param name="inputsA"> First array of input matrices, all of shape (M, N) or (N, M) (NUMERIC type) </param>
	  ''' <param name="inputsB">  Second array of input matrices, all of shape (N, K) or (K, N) (NUMERIC type) </param>
	  ''' <param name="transposeA"> Whether to transpose A arrays or not </param>
	  ''' <param name="transposeB"> Whether to transpose B arrays or not </param>
	  Public Overridable Function batchMmul(ByVal inputsA() As INDArray, ByVal inputsB() As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean) As INDArray()
		NDValidation.validateNumerical("batchMmul", "inputsA", inputsA)
		Preconditions.checkArgument(inputsA.Length >= 1, "inputsA has incorrect size/length. Expected: inputsA.length >= 1, got %s", inputsA.Length)
		NDValidation.validateNumerical("batchMmul", "inputsB", inputsB)
		Preconditions.checkArgument(inputsB.Length >= 1, "inputsB has incorrect size/length. Expected: inputsB.length >= 1, got %s", inputsB.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.custom.BatchMmul(inputsA, inputsB, transposeA, transposeB))
	  End Function

	  ''' <summary>
	  ''' Matrix multiply a batch of matrices. matricesA and matricesB have to be arrays of same<br>
	  ''' length and each pair taken from these sets has to have dimensions (M, N) and (N, K),<br>
	  ''' respectively. If transposeA is true, matrices from matricesA will have shape (N, M) instead.<br>
	  ''' Likewise, if transposeB is true, matrices from matricesB will have shape (K, N).<br>
	  ''' <br>
	  ''' The result of this operation will be a batch of multiplied matrices. The<br>
	  ''' result has the same length as both input batches and each output matrix is of shape (M, K).<br>
	  ''' </summary>
	  ''' <param name="inputsA"> First array of input matrices, all of shape (M, N) or (N, M) (NUMERIC type) </param>
	  ''' <param name="inputsB">  Second array of input matrices, all of shape (N, K) or (K, N) (NUMERIC type) </param>
	  Public Overridable Function batchMmul(ByVal inputsA() As INDArray, ParamArray ByVal inputsB() As INDArray) As INDArray()
		NDValidation.validateNumerical("batchMmul", "inputsA", inputsA)
		Preconditions.checkArgument(inputsA.Length >= 1, "inputsA has incorrect size/length. Expected: inputsA.length >= 1, got %s", inputsA.Length)
		NDValidation.validateNumerical("batchMmul", "inputsB", inputsB)
		Preconditions.checkArgument(inputsB.Length >= 1, "inputsB has incorrect size/length. Expected: inputsB.length >= 1, got %s", inputsB.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.custom.BatchMmul(inputsA, inputsB, False, False))
	  End Function

	  ''' <summary>
	  ''' Cast the array to a new datatype - for example, Integer -> Float<br>
	  ''' </summary>
	  ''' <param name="arg"> Input variable to cast (NDARRAY type) </param>
	  ''' <param name="datatype"> Datatype to cast to </param>
	  ''' <returns> output Output array (after casting) (NDARRAY type) </returns>
	  Public Overridable Function castTo(ByVal arg As INDArray, ByVal datatype As DataType) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.dtype.Cast(arg, datatype))(0)
	  End Function

	  ''' <summary>
	  ''' Concatenate a set of inputs along the specified dimension.<br>
	  ''' Note that inputs must have identical rank and identical dimensions, other than the dimension to stack on.<br>
	  ''' For example, if 2 inputs have shape [a, x, c] and [a, y, c] and dimension = 1, then the output has shape [a, x+y, c]<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Input arrays must all be the same datatype: isSameType(inputs)<br>
	  ''' </summary>
	  ''' <param name="inputs"> Input variables (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension to concatenate on </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function concat(ByVal dimension As Integer, ParamArray ByVal inputs() As INDArray) As INDArray
		NDValidation.validateNumerical("concat", "inputs", inputs)
		Preconditions.checkArgument(inputs.Length >= 1, "inputs has incorrect size/length. Expected: inputs.length >= 1, got %s", inputs.Length)
		Preconditions.checkArgument(isSameType(inputs), "Input arrays must all be the same datatype")
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Concat(inputs, dimension))(0)
	  End Function

	  ''' <summary>
	  ''' Cumulative product operation.<br>
	  ''' For input: [ a, b, c], output is:<br>
	  ''' exclusive=false, reverse=false: [a, a*b, a*b*c]<br>
	  ''' exclusive=true, reverse=false, [0, a, a*b]<br>
	  ''' exclusive=false, reverse=true: [a*b*c, b*c, c]<br>
	  ''' exclusive=true, reverse=true: [b*c, c, 0]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="exclusive"> If true: exclude the first value </param>
	  ''' <param name="reverse"> If true: reverse the direction of the accumulation </param>
	  ''' <param name="axis"> Scalar axis argument for dimension to perform cumululative sum operations along (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cumprod(ByVal [in] As INDArray, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer) As INDArray
		NDValidation.validateNumerical("cumprod", "in", [in])
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CumProd([in], exclusive, reverse, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Cumulative product operation.<br>
	  ''' For input: [ a, b, c], output is:<br>
	  ''' exclusive=false, reverse=false: [a, a*b, a*b*c]<br>
	  ''' exclusive=true, reverse=false, [0, a, a*b]<br>
	  ''' exclusive=false, reverse=true: [a*b*c, b*c, c]<br>
	  ''' exclusive=true, reverse=true: [b*c, c, 0]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="axis"> Scalar axis argument for dimension to perform cumululative sum operations along (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function cumprod(ByVal [in] As INDArray, ParamArray ByVal axis() As Integer) As INDArray
		NDValidation.validateNumerical("cumprod", "in", [in])
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CumProd([in], False, False, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Cumulative sum operation.<br>
	  ''' For input: [ a, b, c], output is:<br>
	  ''' exclusive=false, reverse=false: [a, a+b, a+b+c]<br>
	  ''' exclusive=true, reverse=false, [0, a, a+b]<br>
	  ''' exclusive=false, reverse=true: [a+b+c, b+c, c]<br>
	  ''' exclusive=true, reverse=true: [b+c, c, 0]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="exclusive"> If true: exclude the first value </param>
	  ''' <param name="reverse"> If true: reverse the direction of the accumulation </param>
	  ''' <param name="axis"> Scalar axis argument for dimension to perform cumululative sum operations along (Size: AtLeast(min=1)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function cumsum(ByVal [in] As INDArray, ByVal exclusive As Boolean, ByVal reverse As Boolean, ParamArray ByVal axis() As Integer) As INDArray
		NDValidation.validateNumerical("cumsum", "in", [in])
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CumSum([in], exclusive, reverse, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Cumulative sum operation.<br>
	  ''' For input: [ a, b, c], output is:<br>
	  ''' exclusive=false, reverse=false: [a, a+b, a+b+c]<br>
	  ''' exclusive=true, reverse=false, [0, a, a+b]<br>
	  ''' exclusive=false, reverse=true: [a+b+c, b+c, c]<br>
	  ''' exclusive=true, reverse=true: [b+c, c, 0]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="axis"> Scalar axis argument for dimension to perform cumululative sum operations along (Size: AtLeast(min=1)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function cumsum(ByVal [in] As INDArray, ParamArray ByVal axis() As Integer) As INDArray
		NDValidation.validateNumerical("cumsum", "in", [in])
		Preconditions.checkArgument(axis.Length >= 1, "axis has incorrect size/length. Expected: axis.length >= 1, got %s", axis.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CumSum([in], False, False, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Pairwise dot product reduction along dimension<br>
	  ''' output = sum(i=0 ... size(dim)-1) x[i] * y[i]<br>
	  ''' </summary>
	  ''' <param name="x"> first input (NUMERIC type) </param>
	  ''' <param name="y"> second input (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output output variable (NUMERIC type) </returns>
	  Public Overridable Function dot(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("dot", "x", x)
		NDValidation.validateNumerical("dot", "y", y)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce3.Dot(x, y, dimensions))
	  End Function

	  ''' <summary>
	  ''' Dynamically partition the input variable values into the specified number of paritions, using the indices.<br>
	  ''' Example:<br>
	  ''' <pre><br>
	  ''' input = [1,2,3,4,5]<br>
	  ''' numPartitions = 2<br>
	  ''' partitions = [1,0,0,1,0]<br>
	  ''' out[0] = [2,3,5]<br>
	  ''' out[1] = [1,4] }<br>
	  ''' </pre><br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="partitions"> 1D input with values 0 to numPartitions-1 (INT type) </param>
	  ''' <param name="numPartitions"> Number of partitions, >= 1 </param>
	  Public Overridable Function dynamicPartition(ByVal x As INDArray, ByVal partitions As INDArray, ByVal numPartitions As Integer) As INDArray()
		NDValidation.validateNumerical("dynamicPartition", "x", x)
		NDValidation.validateInteger("dynamicPartition", "partitions", partitions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.DynamicPartition(x, partitions, numPartitions))
	  End Function

	  ''' <summary>
	  ''' Dynamically merge the specified input arrays into a single array, using the specified indices<br>
	  ''' </summary>
	  ''' <param name="indices"> Indices to use when merging. Must be >= 1, same length as input variables (INT type) </param>
	  ''' <param name="x"> Input variables. (NUMERIC type) </param>
	  ''' <returns> output Merged output variable (NUMERIC type) </returns>
	  Public Overridable Function dynamicStitch(ByVal indices() As INDArray, ParamArray ByVal x() As INDArray) As INDArray
		NDValidation.validateInteger("dynamicStitch", "indices", indices)
		Preconditions.checkArgument(indices.Length >= 1, "indices has incorrect size/length. Expected: indices.length >= 1, got %s", indices.Length)
		NDValidation.validateNumerical("dynamicStitch", "x", x)
		Preconditions.checkArgument(x.Length >= 1, "x has incorrect size/length. Expected: x.length >= 1, got %s", x.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.DynamicStitch(indices, x))(0)
	  End Function

	  ''' <summary>
	  ''' Equals operation: elementwise x == y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function eq(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("eq", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarEquals(x, y))
	  End Function

	  ''' <summary>
	  ''' Equal to operation: elementwise x == y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function eq(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("eq", "x", x)
		NDValidation.validateNumerical("eq", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.EqualTo(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Reshape the input by adding a 1 at the specified location.<br>
	  ''' For example, if input has shape [a, b], then output shape is:<br>
	  ''' axis = 0: [1, a, b]<br>
	  ''' axis = 1: [a, 1, b]<br>
	  ''' axis = 2: [a, b, 1]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NDARRAY type) </param>
	  ''' <param name="axis"> Axis to expand </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function expandDims(ByVal x As INDArray, ByVal axis As Integer) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ExpandDims(x, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Generate an output variable with the specified (dynamic) shape with all elements set to the specified value<br>
	  ''' </summary>
	  ''' <param name="shape"> Shape: must be a 1D array/variable (INT type) </param>
	  ''' <param name="dataType"> Datatype of the output array </param>
	  ''' <param name="value"> Value to set all elements to </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function fill(ByVal shape As INDArray, ByVal dataType As DataType, ByVal value As Double) As INDArray
		NDValidation.validateInteger("fill", "shape", shape)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Fill(shape, dataType, value))(0)
	  End Function

	  ''' <summary>
	  ''' Gather slices from the input variable where the indices are specified as fixed int[] values.<br>
	  ''' Output shape is same as input shape, except for axis dimension, which has size equal to indices.length.<br>
	  ''' </summary>
	  ''' <param name="df"> Input variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices to get (Size: AtLeast(min=1)) </param>
	  ''' <param name="axis"> Axis that the indices refer to </param>
	  ''' <returns> output Output variable with slices pulled from the specified axis (NUMERIC type) </returns>
	  Public Overridable Function gather(ByVal df As INDArray, ByVal indices() As Integer, ByVal axis As Integer) As INDArray
		NDValidation.validateNumerical("gather", "df", df)
		Preconditions.checkArgument(indices.Length >= 1, "indices has incorrect size/length. Expected: indices.length >= 1, got %s", indices.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Gather(df, indices, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Gather slices from the input variable where the indices are specified as dynamic array values.<br>
	  ''' Output shape is same as input shape, except for axis dimension, which has size equal to indices.length.<br>
	  ''' </summary>
	  ''' <param name="df"> Input variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices to get slices for. Rank 0 or 1 input (INT type) </param>
	  ''' <param name="axis"> Axis that the indices refer to </param>
	  ''' <returns> output Output variable with slices pulled from the specified axis (NUMERIC type) </returns>
	  Public Overridable Function gather(ByVal df As INDArray, ByVal indices As INDArray, ByVal axis As Integer) As INDArray
		NDValidation.validateNumerical("gather", "df", df)
		NDValidation.validateInteger("gather", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Gather(df, indices, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Gather slices from df with shape specified by indices. <br>
	  ''' </summary>
	  ''' <param name="df">  (NUMERIC type) </param>
	  ''' <param name="indices">  (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function gatherNd(ByVal df As INDArray, ByVal indices As INDArray) As INDArray
		NDValidation.validateNumerical("gatherNd", "df", df)
		NDValidation.validateNumerical("gatherNd", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.GatherNd(df, indices))(0)
	  End Function

	  ''' <summary>
	  ''' Greater than operation: elementwise x > y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function gt(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("gt", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarGreaterThan(x, y))
	  End Function

	  ''' <summary>
	  ''' Greater than operation: elementwise x > y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output Output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function gt(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("gt", "x", x)
		NDValidation.validateNumerical("gt", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThan(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Greater than or equals operation: elementwise x >= y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function gte(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("gte", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarGreaterThanOrEqual(x, y))
	  End Function

	  ''' <summary>
	  ''' Greater than or equal to operation: elementwise x >= y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function gte(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("gte", "x", x)
		NDValidation.validateNumerical("gte", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.GreaterThanOrEqual(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Elementwise identity operation: out = x<br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function identity(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("identity", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.same.Identity(input))(0)
	  End Function

	  ''' <summary>
	  ''' Compute the inverse permutation indices for a permutation operation<br>
	  ''' Example: if input is [2, 0, 1] then output is [1, 2, 0]<br>
	  ''' The idea is that x.permute(input).permute(invertPermutation(input)) == x<br>
	  ''' </summary>
	  ''' <param name="input"> 1D indices for permutation (INT type) </param>
	  ''' <returns> output 1D inverted permutation (INT type) </returns>
	  Public Overridable Function invertPermutation(ByVal input As INDArray) As INDArray
		NDValidation.validateInteger("invertPermutation", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.InvertPermutation(input))(0)
	  End Function

	  ''' <summary>
	  ''' Is the director a numeric tensor? In the current version of ND4J/SameDiff, this always returns true/1<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <returns> output scalar boolean with value true or false (NDARRAY type) </returns>
	  Public Overridable Function isNumericTensor(ByVal x As INDArray) As INDArray
		NDValidation.validateNumerical("isNumericTensor", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.IsNumericTensor(x))(0)
	  End Function

	  ''' <summary>
	  ''' Create a new 1d array with values evenly spaced between values 'start' and 'stop'<br>
	  ''' For example, linspace(start=3.0, stop=4.0, number=3) will generate [3.0, 3.5, 4.0]<br>
	  ''' </summary>
	  ''' <param name="dataType"> Data type of the output array </param>
	  ''' <param name="start"> Start value </param>
	  ''' <param name="stop"> Stop value </param>
	  ''' <param name="number"> Number of values to generate </param>
	  ''' <returns> output INDArray  with linearly spaced elements (NUMERIC type) </returns>
	  Public Overridable Function linspace(ByVal dataType As DataType, ByVal start As Double, ByVal [stop] As Double, ByVal number As Long) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Linspace(dataType, start, [stop], number))(0)
	  End Function

	  ''' <summary>
	  ''' Create a new 1d array with values evenly spaced between values 'start' and 'stop'<br>
	  ''' For example, linspace(start=3.0, stop=4.0, number=3) will generate [3.0, 3.5, 4.0]<br>
	  ''' </summary>
	  ''' <param name="start"> Start value (NUMERIC type) </param>
	  ''' <param name="stop"> Stop value (NUMERIC type) </param>
	  ''' <param name="number"> Number of values to generate (LONG type) </param>
	  ''' <param name="dataType"> Data type of the output array </param>
	  ''' <returns> output INDArray  with linearly spaced elements (NUMERIC type) </returns>
	  Public Overridable Function linspace(ByVal start As INDArray, ByVal [stop] As INDArray, ByVal number As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("linspace", "start", start)
		NDValidation.validateNumerical("linspace", "stop", [stop])
		NDValidation.validateInteger("linspace", "number", number)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Linspace(start, [stop], number, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Less than operation: elementwise x < y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function lt(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("lt", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarLessThan(x, y))
	  End Function

	  ''' <summary>
	  ''' Less than operation: elementwise x < y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output Output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function lt(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("lt", "x", x)
		NDValidation.validateNumerical("lt", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LessThan(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Less than or equals operation: elementwise x <= y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function lte(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("lte", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarLessThanOrEqual(x, y))
	  End Function

	  ''' <summary>
	  ''' Less than or equal to operation: elementwise x <= y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output Output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function lte(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("lte", "x", x)
		NDValidation.validateNumerical("lte", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.LessThanOrEqual(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Returns a boolean mask of equal shape to the input, where the condition is satisfied - value 1 where satisfied, 0 otherwise<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <param name="condition"> Condition </param>
	  ''' <returns> output Boolean mask (NUMERIC type) </returns>
	  Public Overridable Function matchCondition(ByVal [in] As INDArray, ByVal condition As Condition) As INDArray
		NDValidation.validateNumerical("matchCondition", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform([in], condition))
	  End Function

	  ''' <summary>
	  ''' Returns a count of the number of elements that satisfy the condition<br>
	  ''' </summary>
	  ''' <param name="in"> Input (NUMERIC type) </param>
	  ''' <param name="condition"> Condition </param>
	  ''' <returns> output Number of elements that the condition is satisfied for (NUMERIC type) </returns>
	  Public Overridable Function matchConditionCount(ByVal [in] As INDArray, ByVal condition As Condition) As INDArray
		NDValidation.validateNumerical("matchConditionCount", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition([in], condition))
	  End Function

	  ''' <summary>
	  ''' Returns a count of the number of elements that satisfy the condition (for each slice along the specified dimensions)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition </param>
	  ''' <param name="keepDim"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Number of elements that the condition is satisfied for (NUMERIC type) </returns>
	  Public Overridable Function matchConditionCount(ByVal [in] As INDArray, ByVal condition As Condition, ByVal keepDim As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("matchConditionCount", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition([in], condition, keepDim, dimensions))
	  End Function

	  ''' <summary>
	  ''' Returns a count of the number of elements that satisfy the condition (for each slice along the specified dimensions)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="condition"> Condition </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Number of elements that the condition is satisfied for (NUMERIC type) </returns>
	  Public Overridable Function matchConditionCount(ByVal [in] As INDArray, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("matchConditionCount", "in", [in])
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition([in], condition, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Max array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function max(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("max", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Max array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function max(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("max", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Max(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Element-wise maximum operation: out[i] = max(first[i], second[i])<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="first"> First input array (NUMERIC type) </param>
	  ''' <param name="second"> Second input array (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function max(ByVal first As INDArray, ByVal second As INDArray) As INDArray
		NDValidation.validateNumerical("max", "first", first)
		NDValidation.validateNumerical("max", "second", second)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Max(first, second))(0)
	  End Function

	  ''' <summary>
	  ''' Mean (average) array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("mean", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Mean (average) array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function mean(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("mean", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Mean(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' The merge operation is a control operation that forwards the either of the inputs to the output, when<br>
	  ''' the first of them becomes available. If both are available, the output is undefined (either input could<br>
	  ''' be forwarded to the output)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="y"> Input variable (NUMERIC type) </param>
	  ''' <returns> output Output (NUMERIC type) </returns>
	  Public Overridable Function merge(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("merge", "x", x)
		NDValidation.validateNumerical("merge", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.controlflow.compat.Merge(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Minimum array reduction operation, optionally along specified dimensions. out = min(in)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function min(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("min", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Minimum array reduction operation, optionally along specified dimensions. out = min(in)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function min(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("min", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Min(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Element-wise minimum operation: out[i] = min(first[i], second[i])<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' </summary>
	  ''' <param name="first"> First input array (NUMERIC type) </param>
	  ''' <param name="second"> Second input array (NUMERIC type) </param>
	  ''' <returns> output Second input array (NUMERIC type) </returns>
	  Public Overridable Function min(ByVal first As INDArray, ByVal second As INDArray) As INDArray
		NDValidation.validateNumerical("min", "first", first)
		NDValidation.validateNumerical("min", "second", second)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Min(first, second))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <param name="transposeX"> Transpose x (first argument) </param>
	  ''' <param name="transposeY"> Transpose y (second argument) </param>
	  ''' <param name="transposeZ"> Transpose result array </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal x As INDArray, ByVal y As INDArray, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean) As INDArray
		NDValidation.validateNumerical("mmul", "x", x)
		NDValidation.validateNumerical("mmul", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Mmul(x, y, transposeX, transposeY, transposeZ))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("mmul", "x", x)
		NDValidation.validateNumerical("mmul", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Mmul(x, y, False, False, False))(0)
	  End Function

	  ''' <summary>
	  ''' Not equals operation: elementwise x != y<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input array (NUMERIC type) </param>
	  ''' <param name="y"> Double value argument to use in operation </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function neq(ByVal x As INDArray, ByVal y As Double) As INDArray
		NDValidation.validateNumerical("neq", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarNotEquals(x, y))
	  End Function

	  ''' <summary>
	  ''' Not equal to operation: elementwise x != y<br>
	  ''' If x and y arrays have equal shape, the output shape is the same as these inputs.<br>
	  ''' 
	  ''' Note: supports broadcasting if x and y have different shapes and are broadcastable.<br>
	  ''' For example, if X has shape [1,10] and Y has shape [5,10] then op(X,Y) has output shape [5,10]<br>
	  ''' Broadcast rules are the same as NumPy: https://docs.scipy.org/doc/numpy/user/basics.broadcasting.html<br>
	  ''' 
	  ''' Return boolean array with values true where satisfied, or false otherwise.<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (NUMERIC type) </param>
	  ''' <param name="y"> Input 2 (NUMERIC type) </param>
	  ''' <returns> output Boolean array out, with values true/false based on where the condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function neq(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("neq", "x", x)
		NDValidation.validateNumerical("neq", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.NotEqualTo(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Norm1 (L1 norm) reduction operation: The output contains the L1 norm for each tensor/subset along the specified dimensions: <br>
	  ''' out = sum_i abs(x[i])<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm1", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Norm1 (L1 norm) reduction operation: The output contains the L1 norm for each tensor/subset along the specified dimensions: <br>
	  ''' out = sum_i abs(x[i])<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function norm1(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm1", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm1(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Norm2 (L2 norm) reduction operation: The output contains the L2 norm for each tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt(sum_i x[i]^2)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> dimensions dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm2", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Norm2 (L2 norm) reduction operation: The output contains the L2 norm for each tensor/subset along the specified dimensions:<br>
	  ''' out = sqrt(sum_i x[i]^2)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> dimensions dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function norm2(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("norm2", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.Norm2(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Max norm (infinity norm) reduction operation: The output contains the max norm for each tensor/subset along the<br>
	  ''' specified dimensions:<br>
	  ''' out = max(abs(x[i]))<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function normmax(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("normmax", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Max norm (infinity norm) reduction operation: The output contains the max norm for each tensor/subset along the<br>
	  ''' specified dimensions:<br>
	  ''' out = max(abs(x[i]))<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> dimensions to reduce over (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function normmax(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("normmax", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.NormMax(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Convert the array to a one-hot array with values and  for each entry<br>
	  ''' If input has shape [ a, ..., n] then output has shape [ a, ..., n, depth],<br>
	  ''' with {out[i, ..., j, in[i,...,j]]  with other values being set to<br>
	  ''' </summary>
	  ''' <param name="indices"> Indices - value 0 to depth-1 (NUMERIC type) </param>
	  ''' <param name="depth"> Number of classes </param>
	  ''' <param name="axis"> </param>
	  ''' <param name="on"> </param>
	  ''' <param name="off"> </param>
	  ''' <param name="dataType"> Output data type </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function oneHot(ByVal indices As INDArray, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("oneHot", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.OneHot(indices, depth, axis, [on], off, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Convert the array to a one-hot array with values and  for each entry<br>
	  ''' If input has shape [ a, ..., n] then output has shape [ a, ..., n, depth],<br>
	  ''' with {out[i, ..., j, in[i,...,j]]  with other values being set to<br>
	  ''' </summary>
	  ''' <param name="indices"> Indices - value 0 to depth-1 (NUMERIC type) </param>
	  ''' <param name="depth"> Number of classes </param>
	  ''' <param name="axis"> </param>
	  ''' <param name="on"> </param>
	  ''' <param name="off"> </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function oneHot(ByVal indices As INDArray, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double) As INDArray
		NDValidation.validateNumerical("oneHot", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.OneHot(indices, depth, axis, [on], off, DataType.FLOAT))(0)
	  End Function

	  ''' <summary>
	  ''' Convert the array to a one-hot array with values 0 and 1 for each entry<br>
	  ''' If input has shape [ a, ..., n] then output has shape [ a, ..., n, depth],<br>
	  ''' with out[i, ..., j, in[i,...,j]] = 1 with other values being set to 0<br>
	  ''' see oneHot(SDVariable, int, int, double, double)<br>
	  ''' </summary>
	  ''' <param name="indices"> Indices - value 0 to depth-1 (NUMERIC type) </param>
	  ''' <param name="depth"> Number of classes </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function oneHot(ByVal indices As INDArray, ByVal depth As Integer) As INDArray
		NDValidation.validateNumerical("oneHot", "indices", indices)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.OneHot(indices, depth))(0)
	  End Function

	  ''' <summary>
	  ''' Return a variable of all 1s, with the same shape as the input variable. Note that this is dynamic:<br>
	  ''' if the input shape changes in later execution, the returned variable's shape will also be updated<br>
	  ''' </summary>
	  ''' <param name="input"> Input INDArray  (NUMERIC type) </param>
	  ''' <returns> output A new INDArray  with the same (dynamic) shape as the input (NUMERIC type) </returns>
	  Public Overridable Function onesLike(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("onesLike", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.OnesLike(input))(0)
	  End Function

	  ''' <summary>
	  ''' As per onesLike(String, SDVariable) but the output datatype may be specified<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function onesLike(ByVal input As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("onesLike", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.OnesLike(input, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Array permutation operation: permute the dimensions according to the specified permutation indices.<br>
	  ''' Example: if input has shape [a,b,c] and dimensions = [2,0,1] the output has shape [c,a,b]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Permute dimensions (INT type) </param>
	  ''' <returns> output Output variable (permuted input) (NUMERIC type) </returns>
	  Public Overridable Function permute(ByVal x As INDArray, ByVal dimensions As INDArray) As INDArray
		NDValidation.validateNumerical("permute", "x", x)
		NDValidation.validateInteger("permute", "dimensions", dimensions)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Permute(x, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Array permutation operation: permute the dimensions according to the specified permutation indices.<br>
	  ''' Example: if input has shape [a,b,c] and dimensions = [2,0,1] the output has shape [c,a,b]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (permuted input) (NUMERIC type) </returns>
	  Public Overridable Function permute(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("permute", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Permute(x, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Product array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("prod", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Product array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function prod(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("prod", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Prod(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Create a new variable with a 1d array, where the values start at from and increment by step<br>
	  ''' up to (but not including) limit.<br>
	  ''' For example, range(1.0, 3.0, 0.5) will return [1.0, 1.5, 2.0, 2.5]<br>
	  ''' </summary>
	  ''' <param name="from"> Initial/smallest value </param>
	  ''' <param name="to"> Largest value (exclusive) </param>
	  ''' <param name="step"> Step size </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output INDArray  with the specified values (NUMERIC type) </returns>
	  Public Overridable Function range(ByVal from As Double, ByVal [to] As Double, ByVal [step] As Double, ByVal dataType As DataType) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.random.impl.Range(from, [to], [step], dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Create a new variable with a 1d array, where the values start at from and increment by step<br>
	  ''' up to (but not including) limit.<br>
	  ''' For example, range(1.0, 3.0, 0.5) will return [1.0, 1.5, 2.0, 2.5]<br>
	  ''' </summary>
	  ''' <param name="from"> Initial/smallest value (NUMERIC type) </param>
	  ''' <param name="to"> Largest value (exclusive) (NUMERIC type) </param>
	  ''' <param name="step"> Step size (NUMERIC type) </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output INDArray  with the specified values (NUMERIC type) </returns>
	  Public Overridable Function range(ByVal from As INDArray, ByVal [to] As INDArray, ByVal [step] As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("range", "from", from)
		NDValidation.validateNumerical("range", "to", [to])
		NDValidation.validateNumerical("range", "step", [step])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.random.impl.Range(from, [to], [step], dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Returns the rank (number of dimensions, i.e., length(shape)) of the specified INDArray  as a 0D scalar variable<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <returns> output (scalar) output variable with value equal to the rank of the input variable (NUMERIC type) </returns>
	  Public Overridable Function rank(ByVal [in] As INDArray) As INDArray
		NDValidation.validateNumerical("rank", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Rank([in]))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise replace where condition:<br>
	  ''' out[i] = from[i] if condition(update[i]) is satisfied, or<br>
	  ''' out[i] = update[i] if condition(update[i]) is NOT satisfied<br>
	  ''' </summary>
	  ''' <param name="update"> Source array (NUMERIC type) </param>
	  ''' <param name="from"> Replacement values array (used conditionally). Must be same shape as 'update' array (NUMERIC type) </param>
	  ''' <param name="condition"> Condition to check on update array elements </param>
	  ''' <returns> output New array with values replaced where condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function replaceWhere(ByVal update As INDArray, ByVal from As INDArray, ByVal condition As Condition) As INDArray
		NDValidation.validateNumerical("replaceWhere", "update", update)
		NDValidation.validateNumerical("replaceWhere", "from", from)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndReplace(update, from, condition))
	  End Function

	  ''' <summary>
	  ''' Element-wise replace where condition:<br>
	  ''' out[i] = value if condition(update[i]) is satisfied, or<br>
	  ''' out[i] = update[i] if condition(update[i]) is NOT satisfied<br>
	  ''' </summary>
	  ''' <param name="update"> Source array (NUMERIC type) </param>
	  ''' <param name="value"> Value to set at the output, if the condition is satisfied </param>
	  ''' <param name="condition"> Condition to check on update array elements </param>
	  ''' <returns> output New array with values replaced where condition is satisfied (NUMERIC type) </returns>
	  Public Overridable Function replaceWhere(ByVal update As INDArray, ByVal value As Double, ByVal condition As Condition) As INDArray
		NDValidation.validateNumerical("replaceWhere", "update", update)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndSet(update, value, condition))
	  End Function

	  ''' <summary>
	  ''' Reshape the input variable to the specified (fixed) shape. The output variable will have the same values as the<br>
	  ''' input, but with the specified shape.<br>
	  ''' Note that prod(shape) must match length(input) == prod(input.shape)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="shape"> New shape for variable (NUMERIC type) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reshape(ByVal x As INDArray, ByVal shape As INDArray) As INDArray
		NDValidation.validateNumerical("reshape", "x", x)
		NDValidation.validateNumerical("reshape", "shape", shape)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Reshape(x, shape))(0)
	  End Function

	  ''' <summary>
	  ''' Reshape the input variable to the specified (fixed) shape. The output variable will have the same values as the<br>
	  ''' input, but with the specified shape.<br>
	  ''' Note that prod(shape) must match length(input) == prod(input.shape)<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="shape"> New shape for variable (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reshape(ByVal x As INDArray, ParamArray ByVal shape() As Long) As INDArray
		NDValidation.validateNumerical("reshape", "x", x)
		Preconditions.checkArgument(shape.Length >= 0, "shape has incorrect size/length. Expected: shape.length >= 0, got %s", shape.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Reshape(x, shape))(0)
	  End Function

	  ''' <summary>
	  ''' Reverse the values of an array for the specified dimensions<br>
	  ''' If input is:<br>
	  ''' [ 1, 2, 3]<br>
	  ''' [ 4, 5, 6]<br>
	  ''' then<br>
	  ''' reverse(in, 0):<br>
	  ''' [3, 2, 1]<br>
	  ''' [6, 5, 4]<br>
	  ''' reverse(in, 1):<br>
	  ''' [4, 5, 6]<br>
	  ''' [1, 2 3]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Input variable (Size: AtLeast(min=0)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function reverse(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("reverse", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse(x, dimensions))(0)
	  End Function

	  ''' <summary>
	  ''' Reverse sequence op: for each slice along dimension seqDimension, the first seqLength values are reversed<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="seq_lengths"> Length of the sequences (INT type) </param>
	  ''' <param name="seqDim"> Sequence dimension </param>
	  ''' <param name="batchDim"> Batch dimension </param>
	  ''' <returns> output Reversed sequences (NUMERIC type) </returns>
	  Public Overridable Function reverseSequence(ByVal x As INDArray, ByVal seq_lengths As INDArray, ByVal seqDim As Integer, ByVal batchDim As Integer) As INDArray
		NDValidation.validateNumerical("reverseSequence", "x", x)
		NDValidation.validateInteger("reverseSequence", "seq_lengths", seq_lengths)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ReverseSequence(x, seq_lengths, seqDim, batchDim))(0)
	  End Function

	  ''' <summary>
	  ''' Reverse sequence op: for each slice along dimension seqDimension, the first seqLength values are reversed<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="seq_lengths"> Length of the sequences (INT type) </param>
	  ''' <returns> output Reversed sequences (NUMERIC type) </returns>
	  Public Overridable Function reverseSequence(ByVal x As INDArray, ByVal seq_lengths As INDArray) As INDArray
		NDValidation.validateNumerical("reverseSequence", "x", x)
		NDValidation.validateInteger("reverseSequence", "seq_lengths", seq_lengths)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ReverseSequence(x, seq_lengths, -1, 0))(0)
	  End Function

	  ''' <summary>
	  ''' Element-wise scalar floor modulus operation: out = floorMod(in, value).<br>
	  ''' i.e., returns the remainder after division by 'value'<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value to compare </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function scalarFloorMod(ByVal [in] As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("scalarFloorMod", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarFMod([in], value))
	  End Function

	  ''' <summary>
	  ''' Element-wise scalar maximum operation: out = max(in, value)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value to compare </param>
	  ''' <returns> output Scalar value to compare (NUMERIC type) </returns>
	  Public Overridable Function scalarMax(ByVal [in] As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("scalarMax", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarMax([in], value))
	  End Function

	  ''' <summary>
	  ''' Element-wise scalar minimum operation: out = min(in, value)<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="value"> Scalar value to compare </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function scalarMin(ByVal [in] As INDArray, ByVal value As Double) As INDArray
		NDValidation.validateNumerical("scalarMin", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarMin([in], value))
	  End Function

	  ''' <summary>
	  ''' Return a variable with equal shape to the input, but all elements set to value 'set'<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="set"> Value to set </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function scalarSet(ByVal [in] As INDArray, ByVal set As Double) As INDArray
		NDValidation.validateNumerical("scalarSet", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scalar.ScalarSet([in], set))
	  End Function

	  ''' <summary>
	  ''' Scatter addition operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterAdd(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterAdd", "ref", ref)
		NDValidation.validateNumerical("scatterAdd", "indices", indices)
		NDValidation.validateNumerical("scatterAdd", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterAdd(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter division operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterDiv(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterDiv", "ref", ref)
		NDValidation.validateNumerical("scatterDiv", "indices", indices)
		NDValidation.validateNumerical("scatterDiv", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterDiv(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter max operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterMax(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterMax", "ref", ref)
		NDValidation.validateNumerical("scatterMax", "indices", indices)
		NDValidation.validateNumerical("scatterMax", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterMax(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter min operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterMin(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterMin", "ref", ref)
		NDValidation.validateNumerical("scatterMin", "indices", indices)
		NDValidation.validateNumerical("scatterMin", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterMin(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter multiplication operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterMul(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterMul", "ref", ref)
		NDValidation.validateNumerical("scatterMul", "indices", indices)
		NDValidation.validateNumerical("scatterMul", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterMul(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter subtraction operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterSub(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterSub", "ref", ref)
		NDValidation.validateNumerical("scatterSub", "indices", indices)
		NDValidation.validateNumerical("scatterSub", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterSub(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Scatter update operation.<br>
	  ''' 
	  ''' If indices is rank 0 (a scalar), then out[index, ...] = out[index, ...] + op(updates[...])<br>
	  ''' If indices is rank 1 (a vector), then for each position i, out[indices[i], ...] = out[indices[i], ...] + op(updates[i, ...])<br>
	  ''' If indices is rank 2+, then for each position (i,...,k), out[indices[i], ..., indices[k], ...] = out[indices[i], ..., indices[k], ...]  + op(updates[i, ..., k, ...]) <br>
	  ''' Note that if multiple indices refer to the same location, the contributions from each is handled correctly. <br>
	  ''' </summary>
	  ''' <param name="ref"> Initial/source variable (NUMERIC type) </param>
	  ''' <param name="indices"> Indices array (NUMERIC type) </param>
	  ''' <param name="updates"> Updates to add to the initial/source array (NUMERIC type) </param>
	  ''' <returns> output The updated variable (NUMERIC type) </returns>
	  Public Overridable Function scatterUpdate(ByVal ref As INDArray, ByVal indices As INDArray, ByVal updates As INDArray) As INDArray
		NDValidation.validateNumerical("scatterUpdate", "ref", ref)
		NDValidation.validateNumerical("scatterUpdate", "indices", indices)
		NDValidation.validateNumerical("scatterUpdate", "updates", updates)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate(ref, indices, updates))(0)
	  End Function

	  ''' <summary>
	  ''' Segment max operation.<br>
	  ''' 
	  ''' If data =     [3, 6, 1, 4, 9, 2, 8]<br>
	  ''' segmentIds =  [0, 0, 1, 1, 1, 2, 2]<br>
	  ''' then output = [6, 9, 8] = [op(3,6), op(1,4,9), op(2,8)]<br>
	  ''' Note that the segment IDs must be sorted from smallest to largest segment.<br>
	  ''' See {unsortedSegment (String, SDVariable, SDVariable, int) ops<br>
	  ''' for the same op without this sorted requirement<br>
	  ''' </summary>
	  ''' <param name="data"> Data to perform segment max on (NDARRAY type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <returns> output Segment output (NUMERIC type) </returns>
	  Public Overridable Function segmentMax(ByVal data As INDArray, ByVal segmentIds As INDArray) As INDArray
		NDValidation.validateNumerical("segmentMax", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.segment.SegmentMax(data, segmentIds))(0)
	  End Function

	  ''' <summary>
	  ''' Segment mean operation.<br>
	  ''' 
	  ''' If data =     [3, 6, 1, 4, 9, 2, 8]<br>
	  ''' segmentIds =  [0, 0, 1, 1, 1, 2, 2]<br>
	  ''' then output = [6, 9, 8] = [op(3,6), op(1,4,9), op(2,8)]<br>
	  ''' Note that the segment IDs must be sorted from smallest to largest segment.<br>
	  ''' See {unsortedSegment (String, SDVariable, SDVariable, int) ops<br>
	  ''' for the same op without this sorted requirement<br>
	  ''' </summary>
	  ''' <param name="data"> Data to perform segment max on (NDARRAY type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <returns> output Segment output (NUMERIC type) </returns>
	  Public Overridable Function segmentMean(ByVal data As INDArray, ByVal segmentIds As INDArray) As INDArray
		NDValidation.validateNumerical("segmentMean", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.segment.SegmentMean(data, segmentIds))(0)
	  End Function

	  ''' <summary>
	  ''' Segment min operation.<br>
	  ''' 
	  ''' If data =     [3, 6, 1, 4, 9, 2, 8]<br>
	  ''' segmentIds =  [0, 0, 1, 1, 1, 2, 2]<br>
	  ''' then output = [6, 9, 8] = [op(3,6), op(1,4,9), op(2,8)]<br>
	  ''' Note that the segment IDs must be sorted from smallest to largest segment.<br>
	  ''' See {unsortedSegment (String, SDVariable, SDVariable, int) ops<br>
	  ''' for the same op without this sorted requirement<br>
	  ''' </summary>
	  ''' <param name="data"> Data to perform segment max on (NDARRAY type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <returns> output Segment output (NUMERIC type) </returns>
	  Public Overridable Function segmentMin(ByVal data As INDArray, ByVal segmentIds As INDArray) As INDArray
		NDValidation.validateNumerical("segmentMin", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.segment.SegmentMin(data, segmentIds))(0)
	  End Function

	  ''' <summary>
	  ''' Segment product operation.<br>
	  ''' 
	  ''' If data =     [3, 6, 1, 4, 9, 2, 8]<br>
	  ''' segmentIds =  [0, 0, 1, 1, 1, 2, 2]<br>
	  ''' then output = [6, 9, 8] = [op(3,6), op(1,4,9), op(2,8)]<br>
	  ''' Note that the segment IDs must be sorted from smallest to largest segment.<br>
	  ''' See {unsortedSegment (String, SDVariable, SDVariable, int) ops<br>
	  ''' for the same op without this sorted requirement<br>
	  ''' </summary>
	  ''' <param name="data"> Data to perform segment max on (NDARRAY type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <returns> output Segment output (NUMERIC type) </returns>
	  Public Overridable Function segmentProd(ByVal data As INDArray, ByVal segmentIds As INDArray) As INDArray
		NDValidation.validateNumerical("segmentProd", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.segment.SegmentProd(data, segmentIds))(0)
	  End Function

	  ''' <summary>
	  ''' Segment sum operation.<br>
	  ''' 
	  ''' If data =     [3, 6, 1, 4, 9, 2, 8]<br>
	  ''' segmentIds =  [0, 0, 1, 1, 1, 2, 2]<br>
	  ''' then output = [6, 9, 8] = [op(3,6), op(1,4,9), op(2,8)]<br>
	  ''' Note that the segment IDs must be sorted from smallest to largest segment.<br>
	  ''' See {unsortedSegment (String, SDVariable, SDVariable, int) ops<br>
	  ''' for the same op without this sorted requirement<br>
	  ''' </summary>
	  ''' <param name="data"> Data to perform segment max on (NDARRAY type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <returns> output Segment output (NUMERIC type) </returns>
	  Public Overridable Function segmentSum(ByVal data As INDArray, ByVal segmentIds As INDArray) As INDArray
		NDValidation.validateNumerical("segmentSum", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.segment.SegmentSum(data, segmentIds))(0)
	  End Function

	  ''' <summary>
	  ''' Generate a sequence mask (with values 0 or 1) based on the specified lengths <br>
	  ''' Specifically, out[i, ..., k, j] = (j < lengths[i, ..., k] ? 1.0 : 0.0)<br>
	  ''' </summary>
	  ''' <param name="lengths"> Lengths of the sequences (NUMERIC type) </param>
	  ''' <param name="maxLen"> Maximum sequence length </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sequenceMask(ByVal lengths As INDArray, ByVal maxLen As Integer, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("sequenceMask", "lengths", lengths)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.SequenceMask(lengths, maxLen, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Generate a sequence mask (with values 0 or 1) based on the specified lengths <br>
	  ''' Specifically, out[i, ..., k, j] = (j < lengths[i, ..., k] ? 1.0 : 0.0)<br>
	  ''' </summary>
	  ''' <param name="lengths"> Lengths of the sequences (NUMERIC type) </param>
	  ''' <param name="maxLen"> Maximum sequence length (INT type) </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function sequenceMask(ByVal lengths As INDArray, ByVal maxLen As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("sequenceMask", "lengths", lengths)
		NDValidation.validateInteger("sequenceMask", "maxLen", maxLen)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.SequenceMask(lengths, maxLen, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' see sequenceMask(String, SDVariable, SDVariable, DataType)<br>
	  ''' </summary>
	  ''' <param name="lengths">  (NUMERIC type) </param>
	  ''' <param name="dataType"> </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function sequenceMask(ByVal lengths As INDArray, ByVal dataType As DataType) As INDArray
		NDValidation.validateNumerical("sequenceMask", "lengths", lengths)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.SequenceMask(lengths, dataType))(0)
	  End Function

	  ''' <summary>
	  ''' Returns the shape of the specified INDArray  as a 1D INDArray <br>
	  ''' </summary>
	  ''' <param name="input"> Input variable (NUMERIC type) </param>
	  ''' <returns> output 1D output variable with contents equal to the shape of the input (NUMERIC type) </returns>
	  Public Overridable Function shape(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("shape", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Shape(input))(0)
	  End Function

	  ''' <summary>
	  ''' Returns the size (number of elements, i.e., prod(shape)) of the specified INDArray  as a 0D scalar variable<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <returns> output 0D (scalar) output variable with value equal to the number of elements in the specified array (NUMERIC type) </returns>
	  Public Overridable Function size(ByVal [in] As INDArray) As INDArray
		NDValidation.validateNumerical("size", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Size([in]))(0)
	  End Function

	  ''' <summary>
	  ''' Returns a rank 0 (scalar) variable for the size of the specified dimension.<br>
	  ''' For example, if X has shape [10,20,30] then sizeAt(X,1)=20. Similarly, sizeAt(X,-1)=30<br>
	  ''' </summary>
	  ''' <param name="in"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimension"> Dimension to get size of </param>
	  ''' <returns> output Scalar INDArray  for size at specified variable (NUMERIC type) </returns>
	  Public Overridable Function sizeAt(ByVal [in] As INDArray, ByVal dimension As Integer) As INDArray
		NDValidation.validateNumerical("sizeAt", "in", [in])
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.SizeAt([in], dimension))(0)
	  End Function

	  ''' <summary>
	  ''' Get a subset of the specified input, by specifying the first element and the size of the array.<br>
	  ''' For example, if input is:<br>
	  ''' [a, b, c]<br>
	  ''' [d, e, f]<br>
	  ''' then slice(input, begin=[0,1], size=[2,1] will return:<br>
	  ''' [b]<br>
	  ''' [e]<br>
	  ''' Note that for each dimension i, begin[i] + size[i] <= input.size(i)<br>
	  ''' </summary>
	  ''' <param name="input"> input Variable to get subset of (NUMERIC type) </param>
	  ''' <param name="begin"> Beginning index. Must be same length as rank of input array (Size: AtLeast(min=1)) </param>
	  ''' <param name="size"> Size of the output array. Must be same length as rank of input array (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Subset of the input (NUMERIC type) </returns>
	  Public Overridable Function slice(ByVal input As INDArray, ByVal begin() As Integer, ParamArray ByVal size() As Integer) As INDArray
		NDValidation.validateNumerical("slice", "input", input)
		Preconditions.checkArgument(begin.Length >= 1, "begin has incorrect size/length. Expected: begin.length >= 1, got %s", begin.Length)
		Preconditions.checkArgument(size.Length >= 1, "size has incorrect size/length. Expected: size.length >= 1, got %s", size.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Slice(input, begin, size))(0)
	  End Function

	  ''' <summary>
	  ''' Get a subset of the specified input, by specifying the first element and the size of the array.<br>
	  ''' For example, if input is:<br>
	  ''' [a, b, c]<br>
	  ''' [d, e, f]<br>
	  ''' then slice(input, begin=[0,1], size=[2,1] will return:<br>
	  ''' [b]<br>
	  ''' [e]<br>
	  ''' Note that for each dimension i, begin[i] + size[i] <= input.size(i)<br>
	  ''' </summary>
	  ''' <param name="input"> input Variable to get subset of (NUMERIC type) </param>
	  ''' <param name="begin"> Beginning index. Must be same length as rank of input array (INT type) </param>
	  ''' <param name="size"> Size of the output array. Must be same length as rank of input array (INT type) </param>
	  ''' <returns> output Subset of the input (NUMERIC type) </returns>
	  Public Overridable Function slice(ByVal input As INDArray, ByVal begin As INDArray, ByVal size As INDArray) As INDArray
		NDValidation.validateNumerical("slice", "input", input)
		NDValidation.validateInteger("slice", "begin", begin)
		NDValidation.validateInteger("slice", "size", size)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Slice(input, begin, size))(0)
	  End Function

	  ''' <summary>
	  ''' Split a value in to a list of ndarrays.<br>
	  ''' </summary>
	  ''' <param name="input"> Input to split (NUMERIC type) </param>
	  ''' <param name="numSplit"> Number of splits </param>
	  ''' <param name="splitDim"> The dimension to split on </param>
	  Public Overridable Function split(ByVal input As INDArray, ByVal numSplit As Integer, ByVal splitDim As Integer) As INDArray()
		NDValidation.validateNumerical("split", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Split(input, numSplit, splitDim))
	  End Function

	  ''' <summary>
	  ''' Squared L2 norm: see norm2(String, SDVariable, boolean, int...)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x">  (NUMERIC type) </param>
	  ''' <param name="keepDims"> </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=0)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("squaredNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Squared L2 norm: see norm2(String, SDVariable, boolean, int...)<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x">  (NUMERIC type) </param>
	  ''' <param name="dimensions">  (Size: AtLeast(min=0)) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function squaredNorm(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("squaredNorm", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.floating.SquaredNorm(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Remove a single dimension of size 1.<br>
	  ''' For example, if input has shape [a,b,1,c] then squeeze(input, 2) returns an array of shape [a,b,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="axis"> Size 1 dimension to remove </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function squeeze(ByVal x As INDArray, ByVal axis As Integer) As INDArray
		NDValidation.validateNumerical("squeeze", "x", x)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Squeeze(x, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Stack a set of N INDArray of rank X into one rank X+1 variable.<br>
	  ''' If inputs have shape [a,b,c] then output has shape:<br>
	  ''' axis = 0: [N,a,b,c]<br>
	  ''' axis = 1: [a,N,b,c]<br>
	  ''' axis = 2: [a,b,N,c]<br>
	  ''' axis = 3: [a,b,c,N]<br>
	  ''' see unstack(String[], SDVariable, int, int)<br>
	  ''' </summary>
	  ''' <param name="values"> Input variables to stack. Must have the same shape for all inputs (NDARRAY type) </param>
	  ''' <param name="axis"> Axis to stack on </param>
	  ''' <returns> output Output variable (NDARRAY type) </returns>
	  Public Overridable Function stack(ByVal axis As Integer, ParamArray ByVal values() As INDArray) As INDArray
		Preconditions.checkArgument(values.Length >= 1, "values has incorrect size/length. Expected: values.length >= 1, got %s", values.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Stack(values, axis))(0)
	  End Function

	  ''' <summary>
	  ''' Stardard deviation array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="biasCorrected"> If true: divide by (N-1) (i.e., sample stdev). If false: divide by N (population stdev) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function standardDeviation(ByVal x As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("standardDeviation", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.summarystats.StandardDeviation(x, biasCorrected, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Stardard deviation array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="biasCorrected"> If true: divide by (N-1) (i.e., sample stdev). If false: divide by N (population stdev) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function standardDeviation(ByVal x As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("standardDeviation", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.summarystats.StandardDeviation(x, biasCorrected, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Get a subset of the specified input, by specifying the first element, last element, and the strides.<br>
	  ''' For example, if input is:<br>
	  ''' [a, b, c]<br>
	  ''' [d, e, f]<br>
	  ''' [g, h, i]<br>
	  ''' then stridedSlice(input, begin=[0,1], end=[2,2], strides=[2,1], all masks = 0) will return:<br>
	  ''' [b, c]<br>
	  ''' [h, i]<br>
	  ''' </summary>
	  ''' <param name="in"> Variable to get subset of (NUMERIC type) </param>
	  ''' <param name="begin"> Beginning index (Size: AtLeast(min=1)) </param>
	  ''' <param name="end"> End index (Size: AtLeast(min=1)) </param>
	  ''' <param name="strides"> Stride ("step size") for each dimension. For example, stride of 2 means take every second element. (Size: AtLeast(min=1)) </param>
	  ''' <param name="beginMask"> Bit mask: If the ith bit is set to 1, then the value in the begin long[] is ignored, and a value of 0 is used instead for the beginning index for that dimension </param>
	  ''' <param name="endMask"> Bit mask: If the ith bit is set to 1, then the value in the end long[] is ignored, and a value of size(i)-1 is used instead for the end index for that dimension </param>
	  ''' <param name="ellipsisMask"> Bit mask: only one non-zero value is allowed here. If a non-zero value is set, then other dimensions are inserted as required at the specified position </param>
	  ''' <param name="newAxisMask"> Bit mask: if the ith bit is set to 1, then the begin/end/stride values are ignored, and a size 1 dimension is inserted at this point </param>
	  ''' <param name="shrinkAxisMask"> Bit mask: if the ith bit is set to 1, then the begin/end/stride values are ignored, and a size 1 dimension is removed at this point. Note that begin/end/stride values must result in a size 1 output for these dimensions </param>
	  ''' <returns> output A subset of the input array (NUMERIC type) </returns>
	  Public Overridable Function stridedSlice(ByVal [in] As INDArray, ByVal begin() As Long, ByVal [end]() As Long, ByVal strides() As Long, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer) As INDArray
		NDValidation.validateNumerical("stridedSlice", "in", [in])
		Preconditions.checkArgument(begin.Length >= 1, "begin has incorrect size/length. Expected: begin.length >= 1, got %s", begin.Length)
		Preconditions.checkArgument([end].Length >= 1, "end has incorrect size/length. Expected: end.length >= 1, got %s", [end].Length)
		Preconditions.checkArgument(strides.Length >= 1, "strides has incorrect size/length. Expected: strides.length >= 1, got %s", strides.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.StridedSlice([in], begin, [end], strides, beginMask, endMask, ellipsisMask, newAxisMask, shrinkAxisMask))(0)
	  End Function

	  ''' <summary>
	  ''' Get a subset of the specified input, by specifying the first element, last element, and the strides.<br>
	  ''' For example, if input is:<br>
	  ''' [a, b, c]<br>
	  ''' [d, e, f]<br>
	  ''' [g, h, i]<br>
	  ''' then stridedSlice(input, begin=[0,1], end=[2,2], strides=[2,1], all masks = 0) will return:<br>
	  ''' [b, c]<br>
	  ''' [h, i]<br>
	  ''' </summary>
	  ''' <param name="in"> Variable to get subset of (NUMERIC type) </param>
	  ''' <param name="begin"> Beginning index (Size: AtLeast(min=1)) </param>
	  ''' <param name="end"> End index (Size: AtLeast(min=1)) </param>
	  ''' <param name="strides"> Stride ("step size") for each dimension. For example, stride of 2 means take every second element. (Size: AtLeast(min=1)) </param>
	  ''' <returns> output A subset of the input array (NUMERIC type) </returns>
	  Public Overridable Function stridedSlice(ByVal [in] As INDArray, ByVal begin() As Long, ByVal [end]() As Long, ParamArray ByVal strides() As Long) As INDArray
		NDValidation.validateNumerical("stridedSlice", "in", [in])
		Preconditions.checkArgument(begin.Length >= 1, "begin has incorrect size/length. Expected: begin.length >= 1, got %s", begin.Length)
		Preconditions.checkArgument([end].Length >= 1, "end has incorrect size/length. Expected: end.length >= 1, got %s", [end].Length)
		Preconditions.checkArgument(strides.Length >= 1, "strides has incorrect size/length. Expected: strides.length >= 1, got %s", strides.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.StridedSlice([in], begin, [end], strides, 0, 0, 0, 0, 0))(0)
	  End Function

	  ''' <summary>
	  ''' Sum array reduction operation, optionally along specified dimensions.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("sum", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(x, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Sum array reduction operation, optionally along specified dimensions.<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) if keepDims = false, or of rank (input rank) if keepdims = true (NUMERIC type) </returns>
	  Public Overridable Function sum(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("sum", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.same.Sum(x, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Switch operation<br>
	  ''' Predictate - if false, values are output to left (first) branch/output; if true, to right (second) branch/output<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NDARRAY type) </param>
	  ''' <param name="predicate"> Predictate - if false, values are output to left (first) branch/output; if true, to right (second) branch/output (BOOL type) </param>
	  Public Overridable Function switchOp(ByVal x As INDArray, ByVal predicate As INDArray) As INDArray()
		NDValidation.validateBool("switchOp", "predicate", predicate)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.controlflow.compat.Switch(x, predicate))
	  End Function

	  ''' <summary>
	  ''' //TODO: Ops must be documented.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensionsX"> dimensions for first input array (x) (Size: AtLeast(min=1)) </param>
	  ''' <param name="dimensionsY"> dimensions for second input array (y) (Size: AtLeast(min=1)) </param>
	  ''' <param name="transposeX"> Transpose x (first argument) </param>
	  ''' <param name="transposeY"> Transpose y (second argument) </param>
	  ''' <param name="transposeZ"> Transpose result array </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tensorMmul(ByVal x As INDArray, ByVal y As INDArray, ByVal dimensionsX() As Integer, ByVal dimensionsY() As Integer, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean) As INDArray
		NDValidation.validateNumerical("tensorMmul", "x", x)
		NDValidation.validateNumerical("tensorMmul", "y", y)
		Preconditions.checkArgument(dimensionsX.Length >= 1, "dimensionsX has incorrect size/length. Expected: dimensionsX.length >= 1, got %s", dimensionsX.Length)
		Preconditions.checkArgument(dimensionsY.Length >= 1, "dimensionsY has incorrect size/length. Expected: dimensionsY.length >= 1, got %s", dimensionsY.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.TensorMmul(x, y, dimensionsX, dimensionsY, transposeX, transposeY, transposeZ))(0)
	  End Function

	  ''' <summary>
	  ''' //TODO: Ops must be documented.<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable x (NUMERIC type) </param>
	  ''' <param name="y"> Input variable y (NUMERIC type) </param>
	  ''' <param name="dimensionsX"> dimensions for first input array (x) (Size: AtLeast(min=1)) </param>
	  ''' <param name="dimensionsY"> dimensions for second input array (y) (Size: AtLeast(min=1)) </param>
	  ''' <returns> output Output variable (NUMERIC type) </returns>
	  Public Overridable Function tensorMmul(ByVal x As INDArray, ByVal y As INDArray, ByVal dimensionsX() As Integer, ParamArray ByVal dimensionsY() As Integer) As INDArray
		NDValidation.validateNumerical("tensorMmul", "x", x)
		NDValidation.validateNumerical("tensorMmul", "y", y)
		Preconditions.checkArgument(dimensionsX.Length >= 1, "dimensionsX has incorrect size/length. Expected: dimensionsX.length >= 1, got %s", dimensionsX.Length)
		Preconditions.checkArgument(dimensionsY.Length >= 1, "dimensionsY has incorrect size/length. Expected: dimensionsY.length >= 1, got %s", dimensionsY.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.TensorMmul(x, y, dimensionsX, dimensionsY, False, False, False))(0)
	  End Function

	  ''' <summary>
	  ''' Repeat (tile) the input tensor the specified number of times.<br>
	  ''' For example, if input is<br>
	  ''' [1, 2]<br>
	  ''' [3, 4]<br>
	  ''' and repeat is [2, 3]<br>
	  ''' then output is<br>
	  ''' [1, 2, 1, 2, 1, 2]<br>
	  ''' [3, 4, 3, 4, 3, 4]<br>
	  ''' [1, 2, 1, 2, 1, 2]<br>
	  ''' [3, 4, 3, 4, 3, 4]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NDARRAY type) </param>
	  ''' <param name="repeat"> Number of times to repeat in each axis. Must have length equal to the rank of the input array (INT type) </param>
	  ''' <returns> output Output variable (NDARRAY type) </returns>
	  Public Overridable Function tile(ByVal x As INDArray, ByVal repeat As INDArray) As INDArray
		NDValidation.validateInteger("tile", "repeat", repeat)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Tile(x, repeat))(0)
	  End Function

	  ''' <summary>
	  ''' see tile(String, SDVariable, int...)<br>
	  ''' </summary>
	  ''' <param name="x">  (NDARRAY type) </param>
	  ''' <param name="repeat">  (Size: AtLeast(min=1)) </param>
	  ''' <returns> output  (NDARRAY type) </returns>
	  Public Overridable Function tile(ByVal x As INDArray, ParamArray ByVal repeat() As Integer) As INDArray
		Preconditions.checkArgument(repeat.Length >= 1, "repeat has incorrect size/length. Expected: repeat.length >= 1, got %s", repeat.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Tile(x, repeat))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix transpose operation: If input has shape [a,b] output has shape [b,a]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NDARRAY type) </param>
	  ''' <returns> output transposed input (NDARRAY type) </returns>
	  Public Overridable Function transpose(ByVal x As INDArray) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Transpose(x))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment max operation. As per segmentMax(String, SDVariable, SDVariable) but without<br>
	  ''' the requirement for the indices to be sorted.<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [6, 9, 8] = [max(3,6), max(1,4,9), max(2,8)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentMax(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentMax", "data", data)
		NDValidation.validateNumerical("unsortedSegmentMax", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentMax(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment mean operation. As per segmentMean(String, SDVariable, SDVariable) but without<br>
	  ''' the requirement for the indices to be sorted.<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [4.5, 4.666, 5] = [mean(3,6), mean(1,4,9), mean(2,8)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentMean(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentMean", "data", data)
		NDValidation.validateNumerical("unsortedSegmentMean", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentMean(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment min operation. As per segmentMin(String, SDVariable, SDVariable) but without<br>
	  ''' the requirement for the indices to be sorted.<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [3, 1, 2] = [min(3,6), min(1,4,9), min(2,8)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentMin(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentMin", "data", data)
		NDValidation.validateNumerical("unsortedSegmentMin", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentMin(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment product operation. As per segmentProd(String, SDVariable, SDVariable) but without<br>
	  ''' the requirement for the indices to be sorted.<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [4.5, 4.666, 5] = [mean(3,6), mean(1,4,9), mean(2,8)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentProd(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentProd", "data", data)
		NDValidation.validateNumerical("unsortedSegmentProd", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentProd(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment sqrtN operation. Simply returns the sqrt of the count of the number of values in each segment<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [1.414, 1.732, 1.414] = [sqrt(2), sqrtN(3), sqrtN(2)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentSqrtN(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentSqrtN", "data", data)
		NDValidation.validateNumerical("unsortedSegmentSqrtN", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentSqrtN(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unsorted segment sum operation. As per segmentSum(String, SDVariable, SDVariable) but without<br>
	  ''' the requirement for the indices to be sorted.<br>
	  ''' If data =     [1, 3, 2, 6, 4, 9, 8]<br>
	  ''' segmentIds =  [1, 0, 2, 0, 1, 1, 2]<br>
	  ''' then output = [9, 14, 10] = [sum(3,6), sum(1,4,9), sum(2,8)]<br>
	  ''' </summary>
	  ''' <param name="data"> Data (variable) to perform unsorted segment max on (NUMERIC type) </param>
	  ''' <param name="segmentIds"> Variable for the segment IDs (NUMERIC type) </param>
	  ''' <param name="numSegments"> Number of segments </param>
	  ''' <returns> output Unsorted segment output (NUMERIC type) </returns>
	  Public Overridable Function unsortedSegmentSum(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer) As INDArray
		NDValidation.validateNumerical("unsortedSegmentSum", "data", data)
		NDValidation.validateNumerical("unsortedSegmentSum", "segmentIds", segmentIds)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentSum(data, segmentIds, numSegments))(0)
	  End Function

	  ''' <summary>
	  ''' Unstack a variable of rank X into N rank X-1 variables by taking slices along the specified axis.<br>
	  ''' If input has shape [a,b,c] then output has shape:<br>
	  ''' axis = 0: [b,c]<br>
	  ''' axis = 1: [a,c]<br>
	  ''' axis = 2: [a,b]<br>
	  ''' </summary>
	  ''' <param name="value"> Input variable to unstack (NDARRAY type) </param>
	  ''' <param name="axis"> Axis to unstack on </param>
	  ''' <param name="num"> Number of output variables </param>
	  Public Overridable Function unstack(ByVal value As INDArray, ByVal axis As Integer, ByVal num As Integer) As INDArray()
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Unstack(value, axis, num))
	  End Function

	  ''' <summary>
	  ''' Variance array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="biasCorrected"> If true: divide by (N-1) (i.e., sample variable). If false: divide by N (population variance) </param>
	  ''' <param name="keepDims"> If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function variance(ByVal x As INDArray, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("variance", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.summarystats.Variance(x, biasCorrected, keepDims, dimensions))
	  End Function

	  ''' <summary>
	  ''' Variance array reduction operation, optionally along specified dimensions<br>
	  ''' 
	  ''' Note that if keepDims = true, the output variable has the same rank as the input variable,<br>
	  ''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting<br>
	  ''' the mean along a dimension).<br>
	  ''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:<br>
	  ''' keepDims = true: [a,1,c]<br>
	  ''' keepDims = false: [a,c]<br>
	  ''' </summary>
	  ''' <param name="x"> Input variable (NUMERIC type) </param>
	  ''' <param name="biasCorrected"> If true: divide by (N-1) (i.e., sample variable). If false: divide by N (population variance) </param>
	  ''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed (Size: AtLeast(min=0)) </param>
	  ''' <returns> output reduced array of rank (input rank - num dimensions) (NUMERIC type) </returns>
	  Public Overridable Function variance(ByVal x As INDArray, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer) As INDArray
		NDValidation.validateNumerical("variance", "x", x)
		Preconditions.checkArgument(dimensions.Length >= 0, "dimensions has incorrect size/length. Expected: dimensions.length >= 0, got %s", dimensions.Length)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.summarystats.Variance(x, biasCorrected, False, dimensions))
	  End Function

	  ''' <summary>
	  ''' Return a variable of all 0s, with the same shape as the input variable. Note that this is dynamic:<br>
	  ''' if the input shape changes in later execution, the returned variable's shape will also be updated<br>
	  ''' </summary>
	  ''' <param name="input"> Input  (NUMERIC type) </param>
	  ''' <returns> output A new Variable with the same (dynamic) shape as the input (NUMERIC type) </returns>
	  Public Overridable Function zerosLike(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("zerosLike", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.ZerosLike(input))(0)
	  End Function
	End Class

End Namespace