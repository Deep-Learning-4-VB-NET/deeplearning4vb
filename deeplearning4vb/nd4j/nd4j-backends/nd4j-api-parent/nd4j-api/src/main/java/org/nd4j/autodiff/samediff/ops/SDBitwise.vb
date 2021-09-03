import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions

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

	Public Class SDBitwise
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
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
	  Public Overridable Function [and](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("and", "x", x)
		SDValidation.validateInteger("and", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseAnd(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise AND operation. Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> Second input array (INT type) </param>
	  ''' <returns> output Bitwise AND array (INT type) </returns>
	  Public Overridable Function [and](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("and", "x", x)
		SDValidation.validateInteger("and", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseAnd(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the left, i.e. var << 4 | var >> (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotl(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitRotl", "x", x)
		SDValidation.validateInteger("bitRotl", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the left, i.e. var << 4 | var >> (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotl(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitRotl", "x", x)
		SDValidation.validateInteger("bitRotl", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the right, i.e. var >> 4 | var << (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotr(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitRotr", "x", x)
		SDValidation.validateInteger("bitRotr", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Roll integer bits to the right, i.e. var >> 4 | var << (32 - 4)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitRotr(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitRotr", "x", x)
		SDValidation.validateInteger("bitRotr", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the left, i.e. var << 4<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShift(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitShift", "x", x)
		SDValidation.validateInteger("bitShift", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the left, i.e. var << 4<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShift(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitShift", "x", x)
		SDValidation.validateInteger("bitShift", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the right, i.e. var >> 4<br>
	  ''' </summary>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShiftRight(ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitShiftRight", "x", x)
		SDValidation.validateInteger("bitShiftRight", "shift", shift)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, shift)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Shift integer bits to the right, i.e. var >> 4<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input 1 (INT type) </param>
	  ''' <param name="shift"> Number of bits to shift. (INT type) </param>
	  ''' <returns> output SDVariable with shifted bits (INT type) </returns>
	  Public Overridable Function bitShiftRight(ByVal name As String, ByVal x As SDVariable, ByVal shift As SDVariable) As SDVariable
		SDValidation.validateInteger("bitShiftRight", "x", x)
		SDValidation.validateInteger("bitShiftRight", "shift", shift)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, shift)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function bitsHammingDistance(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("bitsHammingDistance", "x", x)
		SDValidation.validateInteger("bitsHammingDistance", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitsHammingDistance(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise Hamming distance reduction over all elements of both input arrays.<br>
	  ''' For example, if x=01100000 and y=1010000 then the bitwise Hamming distance is 2 (due to differences at positions 0 and 1)<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input array. (INT type) </param>
	  ''' <param name="y"> Second input array. (INT type) </param>
	  ''' <returns> output bitwise Hamming distance (INT type) </returns>
	  Public Overridable Function bitsHammingDistance(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("bitsHammingDistance", "x", x)
		SDValidation.validateInteger("bitsHammingDistance", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitsHammingDistance(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bitwise left shift operation. Supports broadcasting.<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function leftShift(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("leftShift", "x", x)
		SDValidation.validateInteger("leftShift", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise left shift operation. Supports broadcasting.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function leftShift(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("leftShift", "x", x)
		SDValidation.validateInteger("leftShift", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.ShiftBits(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bitwise left cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike #leftShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code leftShiftCyclic(01110000, 2) -> 11000001}<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function leftShiftCyclic(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("leftShiftCyclic", "x", x)
		SDValidation.validateInteger("leftShiftCyclic", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise left cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike #leftShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code leftShiftCyclic(01110000, 2) -> 11000001}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function leftShiftCyclic(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("leftShiftCyclic", "x", x)
		SDValidation.validateInteger("leftShiftCyclic", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicShiftBits(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function [or](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("or", "x", x)
		SDValidation.validateInteger("or", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseOr(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise OR operation. Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> First input array (INT type) </param>
	  ''' <returns> output Bitwise OR array (INT type) </returns>
	  Public Overridable Function [or](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("or", "x", x)
		SDValidation.validateInteger("or", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseOr(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bitwise right shift operation. Supports broadcasting. <br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function rightShift(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("rightShift", "x", x)
		SDValidation.validateInteger("rightShift", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise right shift operation. Supports broadcasting. <br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise shifted input x (INT type) </returns>
	  Public Overridable Function rightShift(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("rightShift", "x", x)
		SDValidation.validateInteger("rightShift", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.RShiftBits(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Bitwise right cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike rightShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code rightShiftCyclic(00001110, 2) -> 10000011}<br>
	  ''' </summary>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function rightShiftCyclic(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("rightShiftCyclic", "x", x)
		SDValidation.validateInteger("rightShiftCyclic", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise right cyclical shift operation. Supports broadcasting.<br>
	  ''' Unlike rightShift(INDArray, INDArray) the bits will "wrap around":<br>
	  ''' {@code rightShiftCyclic(00001110, 2) -> 10000011}<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> Input to be bit shifted (INT type) </param>
	  ''' <param name="y"> Amount to shift elements of x array (INT type) </param>
	  ''' <returns> output Bitwise cyclic shifted input x (INT type) </returns>
	  Public Overridable Function rightShiftCyclic(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("rightShiftCyclic", "x", x)
		SDValidation.validateInteger("rightShiftCyclic", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.CyclicRShiftBits(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function [xor](ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("xor", "x", x)
		SDValidation.validateInteger("xor", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseXor(sd,x, y)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Bitwise XOR operation (exclusive OR). Supports broadcasting.<br>
	  ''' 
	  ''' Inputs must satisfy the following constraints: <br>
	  ''' Must be same types: isSameType(x, y)<br>
	  ''' Must have broadcastable shapes: isBroadcastableShapes(x, y)<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input array (INT type) </param>
	  ''' <param name="y"> First input array (INT type) </param>
	  ''' <returns> output Bitwise XOR array (INT type) </returns>
	  Public Overridable Function [xor](ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateInteger("xor", "x", x)
		SDValidation.validateInteger("xor", "y", y)
		Preconditions.checkArgument(isSameType(x, y), "Must be same types")
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.BitwiseXor(sd,x, y)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace