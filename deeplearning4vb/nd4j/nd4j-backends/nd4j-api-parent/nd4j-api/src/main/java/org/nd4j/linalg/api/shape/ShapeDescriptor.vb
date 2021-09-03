Imports System.Text
Imports System.Linq
Imports Getter = lombok.Getter

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


	Public Class ShapeDescriptor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private char order;
		Private order As Char
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long offset;
		Private offset As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int ews;
		Private ews As Integer
		Private hashShape As Long = 0
		Private hashStride As Long = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int[] shape;
'JAVA TO VB CONVERTER NOTE: The variable shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private shape_Conflict() As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int[] stride;
		Private stride() As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long extras;
		Private extras As Long

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ByVal shape_Conflict() As Integer, ByVal stride() As Integer, ByVal offset As Long, ByVal ews As Integer, ByVal order As Char, ByVal extras As Long)
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
			Me.shape_Conflict = Arrays.CopyOf(shape_Conflict, shape_Conflict.Length)
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

			Dim that As ShapeDescriptor = DirectCast(o, ShapeDescriptor)

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
			If Not shape_Conflict.SequenceEqual(that.shape_Conflict) Then
				Return False
			End If
			Return stride.SequenceEqual(that.stride)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = AscW(order)

			result = 31 * result + longHashCode(offset)
			result = 31 * result + longHashCode(extras)
			result = 31 * result + ews
			result = 31 * result + Arrays.hashCode(shape_Conflict)
			result = 31 * result + Arrays.hashCode(stride)
			Return result
		End Function

		Private Function longHashCode(ByVal v As Long) As Integer
			' impl from j8
			Return CInt(v Xor (CLng(CULng(v) >> 32)))
		End Function

		Public Overrides Function ToString() As String

			Dim builder As New StringBuilder()

			builder.Append(shape_Conflict.Length).Append(",").Append(Arrays.toString(shape_Conflict)).Append(",").Append(Arrays.toString(stride)).Append(",").Append(offset).Append(",").Append(ews).Append(",").Append(order)

			Dim result As String = builder.ToString().replaceAll("\]", "").replaceAll("\[", "")
			result = "[" & result & "]"

			Return result
		End Function
	End Class

End Namespace