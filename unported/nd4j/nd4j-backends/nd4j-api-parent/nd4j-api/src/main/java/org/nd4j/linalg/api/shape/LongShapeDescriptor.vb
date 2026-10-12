Imports System.Text
Imports System.Linq
Imports lombok
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = org.nd4j.linalg.api.shape.options.ArrayType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.shape


	Public Class LongShapeDescriptor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private char order;
		Private order As Char

		Private offset As Long

		Private ews As Long

		Private hashShape As Long = 0
		Private hashStride As Long = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long[] shape;
		Private shape() As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long[] stride;
		Private stride() As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private long extras;
		Private extras As Long

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ByVal shape_Conflict() As Long, ByVal stride() As Long, ByVal offset As Long, ByVal ews As Long, ByVal order As Char, ByVal extras As Long)
	'        
	'        if (shape != null) {
	'            hashShape = shape[0];
	'            for (int i = 1; i < shape.length; i++)
	'                hashShape = 31 * hashShape + shape[i];
	'        }
	'        
	'        if (stride != null) {
	'            hashStride = stride[0];
	'            for (int i = 1; i < stride.length; i++)
	'                hashStride = 31 * hashStride + stride[i];
	'        }
	'        
			Me.shape = Arrays.CopyOf(shape_Conflict, shape_Conflict.Length)
			Me.stride = Arrays.CopyOf(stride, stride.Length)

			Me.offset = offset
			Me.ews = ews
			Me.order = order

			Me.extras = extras
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As LongShapeDescriptor = DirectCast(o, LongShapeDescriptor)

			If extras <> that.extras Then
				Return False
			End If
			If order <> that.order Then
				Return False
			End If
			If offset <> that.offset Then
				Return False
			End If
			If ews <> that.ews Then
				Return False
			End If
			If Not shape.SequenceEqual(that.shape) Then
				Return False
			End If
			Return stride.SequenceEqual(that.stride)

		End Function

		Public Overridable Function rank() As Integer
			Return If(shape Is Nothing, 0, shape.Length)
		End Function

		Public Overridable Function dataType() As DataType
			Return ArrayOptionsHelper.dataType(extras)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = AscW(order)

			result = 31 * result + longHashCode(offset)
			result = 31 * result + longHashCode(ews)
			result = 31 * result + longHashCode(extras)
			result = 31 * result + Arrays.hashCode(shape)
			result = 31 * result + Arrays.hashCode(stride)
			Return result
		End Function

		Public Overrides Function ToString() As String

			Dim builder As New StringBuilder()

			builder.Append(shape.Length).Append(",").Append(Arrays.toString(shape)).Append(",").Append(Arrays.toString(stride)).Append(",").Append(extras).Append(",").Append(ews).Append(",").Append(order)

			Dim result As String = builder.ToString().replaceAll("\]", "").replaceAll("\[", "")
			result = "[" & result & "]"

			Return result
		End Function

		Private Function longHashCode(ByVal v As Long) As Integer
			' impl from j8
			Return CInt(v Xor (CLng(CULng(v) >> 32)))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static LongShapeDescriptor fromShapeDescriptor(@NonNull ShapeDescriptor descriptor)
		Public Shared Function fromShapeDescriptor(ByVal descriptor As ShapeDescriptor) As LongShapeDescriptor
			Return New LongShapeDescriptor(ArrayUtil.toLongArray(descriptor.getShape()), ArrayUtil.toLongArray(descriptor.getStride()), descriptor.getOffset(), descriptor.getEws(), descriptor.getOrder(), descriptor.getExtras())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static LongShapeDescriptor fromShape(int[] shape, @NonNull DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function fromShape(ByVal shape_Conflict() As Integer, ByVal dataType As DataType) As LongShapeDescriptor
			Return fromShape(ArrayUtil.toLongArray(shape_Conflict), dataType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static LongShapeDescriptor fromShape(long[] shape, @NonNull DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function fromShape(ByVal shape_Conflict() As Long, ByVal dataType As DataType) As LongShapeDescriptor
			Return fromShape(shape_Conflict, Nd4j.getStrides(shape_Conflict, Nd4j.order()), 1, Nd4j.order(), dataType, False)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static LongShapeDescriptor empty(@NonNull DataType dataType)
		Public Shared Function empty(ByVal dataType As DataType) As LongShapeDescriptor
			Dim l(-1) As Long
			Return fromShape(l, l, 1, "c"c, dataType, True)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static LongShapeDescriptor fromShape(@NonNull long[] shape, @NonNull long[] strides, long ews, char order, @NonNull DataType dataType, boolean empty)
'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function fromShape(ByVal shape_Conflict() As Long, ByVal strides() As Long, ByVal ews As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As LongShapeDescriptor
			Dim extras As Long = 0L
			extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
			If empty Then
				extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
			End If

			Return New LongShapeDescriptor(shape_Conflict, strides, 0, ews, order, extras)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function fromShape(ByVal shape_Conflict() As Long, ByVal extras As Long) As LongShapeDescriptor
			Return New LongShapeDescriptor(shape_Conflict, Nd4j.getStrides(shape_Conflict, Nd4j.order()), 0, 1, Nd4j.order(), extras)
		End Function

		''' <summary>
		''' Return a new LongShapeDescriptor with the same shape, strides, order etc but with the specified datatype instead </summary>
		''' <param name="dataType"> Datatype of the returned descriptor </param>
		Public Overridable Function asDataType(ByVal dataType As DataType) As LongShapeDescriptor
			Dim extras As Long = 0L
			extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
			If Empty Then
				extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
			End If
			Return New LongShapeDescriptor(shape, stride, offset, ews, order, extras)
		End Function

		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return ArrayOptionsHelper.hasBitSet(extras, ArrayOptionsHelper.ATYPE_EMPTY_BIT)
			End Get
		End Property
	End Class

End Namespace