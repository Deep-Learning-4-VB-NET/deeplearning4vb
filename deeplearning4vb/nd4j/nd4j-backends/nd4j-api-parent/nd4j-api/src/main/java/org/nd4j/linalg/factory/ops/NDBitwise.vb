import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports Preconditions = org.nd4j.common.base.Preconditions
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

	Public Class NDBitwise
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Bitwise AND operation. Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> Second input array (INT type) </param>
	  ''' <returns> output Bitwise AND array (INT type) </returns>
	  Public Overridable Function [and](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("and", "x", x)
		NDValidation.validateInteger("and", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseAnd(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the left, i.e. var << 4 | var >> (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotl(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateInteger("bitRotl", "x", x)
		NDValidation.validateInteger("bitRotl", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the right, i.e. var >> 4 | var << (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotr(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateInteger("bitRotr", "x", x)
		NDValidation.validateInteger("bitRotr", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the left, i.e. var << 4<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShift(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateInteger("bitShift", "x", x)
		NDValidation.validateInteger("bitShift", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the right, i.e. var >> 4<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShiftRight(ByVal x As INDArray, ByVal shift As INDArray) As INDArray
		NDValidation.validateInteger("bitShiftRight", "x", x)
		NDValidation.validateInteger("bitShiftRight", "shift", shift)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(x, shift))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise Hamming distance reduction over all elements of both input arrays.<br>
	  ''' For example, if x=01100000 and y=1010000 then the bitwise Hamming distance is 2 (due to differences at positions 0 and 1)<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' </summary>
	  ''' <param name="x"> First input array. (INT type) </param>
	  ''' <param name="y"> Second input array. (INT type) </param>
	  ''' <returns> output bitwise Hamming distance (INT type) </returns>
	  Public Overridable Function bitsHammingDistance(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("bitsHammingDistance", "x", x)
		NDValidation.validateInteger("bitsHammingDistance", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.BitsHammingDistance(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise left shift operation. Supports broadcasting.<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function leftShift(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("leftShift", "x", x)
		NDValidation.validateInteger("leftShift", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise left cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike #leftShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code leftShiftCyclic(01110000, 2) -> 11000001}<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function leftShiftCyclic(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("leftShiftCyclic", "x", x)
		NDValidation.validateInteger("leftShiftCyclic", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise OR operation. Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> First input array (INT type) </param>
	  ''' <returns> output Bitwise OR array (INT type) </returns>
	  Public Overridable Function [or](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("or", "x", x)
		NDValidation.validateInteger("or", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseOr(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise right shift operation. Supports broadcasting. <br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function rightShift(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("rightShift", "x", x)
		NDValidation.validateInteger("rightShift", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise right cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike rightShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code rightShiftCyclic(00001110, 2) -> 10000011}<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function rightShiftCyclic(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("rightShiftCyclic", "x", x)
		NDValidation.validateInteger("rightShiftCyclic", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(x, y))(0)
	  End Function

	  ''' <summary>
	  ''' Bitwise XOR operation (exclusive OR). Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> First input array (INT type) </param>
	  ''' <returns> output Bitwise XOR array (INT type) </returns>
	  Public Overridable Function [xor](ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateInteger("xor", "x", x)
		NDValidation.validateInteger("xor", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseXor(x, y))(0)
	  End Function
	End Class

End Namespace