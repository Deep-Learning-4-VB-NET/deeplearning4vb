Imports System
Imports com.google.flatbuffers

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

Namespace org.nd4j.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public final class UISummaryStatistics extends Table
	Public NotInheritable Class UISummaryStatistics
		Inherits Table

	  Public Shared Function getRootAsUISummaryStatistics(ByVal _bb As ByteBuffer) As UISummaryStatistics
		  Return getRootAsUISummaryStatistics(_bb, New UISummaryStatistics())
	  End Function
	  Public Shared Function getRootAsUISummaryStatistics(ByVal _bb As ByteBuffer, ByVal obj As UISummaryStatistics) As UISummaryStatistics
		  _bb.order(ByteOrder.LITTLE_ENDIAN)
		  Return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb))
	  End Function
	  Public Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
		  bb_pos = _i
		  bb = _bb
	  End Sub
	  Public Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As UISummaryStatistics
		  __init(_i, _bb)
		  Return Me
	  End Function

	  Public Function bitmask() As Long
		  Dim o As Integer = __offset(4)
		  Return If(o <> 0, CLng(Math.Truncate(bb.getInt(o + bb_pos))) And &HFFFFFFFFL, 0L)
	  End Function
	  Public Function min() As FlatArray
		  Return min(New FlatArray())
	  End Function
	  Public Function min(ByVal obj As FlatArray) As FlatArray
		  Dim o As Integer = __offset(6)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function max() As FlatArray
		  Return max(New FlatArray())
	  End Function
	  Public Function max(ByVal obj As FlatArray) As FlatArray
		  Dim o As Integer = __offset(8)
		  Return If(o <> 0, obj.__assign(__indirect(o + bb_pos), bb), Nothing)
	  End Function
	  Public Function mean() As Double
		  Dim o As Integer = __offset(10)
		  Return If(o <> 0, bb.getDouble(o + bb_pos), 0.0)
	  End Function
	  Public Function stdev() As Double
		  Dim o As Integer = __offset(12)
		  Return If(o <> 0, bb.getDouble(o + bb_pos), 0.0)
	  End Function
	  Public Function countzero() As Long
		  Dim o As Integer = __offset(14)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function countpositive() As Long
		  Dim o As Integer = __offset(16)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function countnegative() As Long
		  Dim o As Integer = __offset(18)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function countnan() As Long
		  Dim o As Integer = __offset(20)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function
	  Public Function countinf() As Long
		  Dim o As Integer = __offset(22)
		  Return If(o <> 0, bb.getLong(o + bb_pos), 0L)
	  End Function

	  Public Shared Function createUISummaryStatistics(ByVal builder As FlatBufferBuilder, ByVal bitmask As Long, ByVal minOffset As Integer, ByVal maxOffset As Integer, ByVal mean As Double, ByVal stdev As Double, ByVal countzero As Long, ByVal countpositive As Long, ByVal countnegative As Long, ByVal countnan As Long, ByVal countinf As Long) As Integer
		builder.startObject(10)
		UISummaryStatistics.addCountinf(builder, countinf)
		UISummaryStatistics.addCountnan(builder, countnan)
		UISummaryStatistics.addCountnegative(builder, countnegative)
		UISummaryStatistics.addCountpositive(builder, countpositive)
		UISummaryStatistics.addCountzero(builder, countzero)
		UISummaryStatistics.addStdev(builder, stdev)
		UISummaryStatistics.addMean(builder, mean)
		UISummaryStatistics.addMax(builder, maxOffset)
		UISummaryStatistics.addMin(builder, minOffset)
		UISummaryStatistics.addBitmask(builder, bitmask)
		Return UISummaryStatistics.endUISummaryStatistics(builder)
	  End Function

	  Public Shared Sub startUISummaryStatistics(ByVal builder As FlatBufferBuilder)
		  builder.startObject(10)
	  End Sub
	  Public Shared Sub addBitmask(ByVal builder As FlatBufferBuilder, ByVal bitmask As Long)
		  builder.addInt(0, CInt(bitmask), CInt(0L))
	  End Sub
	  Public Shared Sub addMin(ByVal builder As FlatBufferBuilder, ByVal minOffset As Integer)
		  builder.addOffset(1, minOffset, 0)
	  End Sub
	  Public Shared Sub addMax(ByVal builder As FlatBufferBuilder, ByVal maxOffset As Integer)
		  builder.addOffset(2, maxOffset, 0)
	  End Sub
	  Public Shared Sub addMean(ByVal builder As FlatBufferBuilder, ByVal mean As Double)
		  builder.addDouble(3, mean, 0.0)
	  End Sub
	  Public Shared Sub addStdev(ByVal builder As FlatBufferBuilder, ByVal stdev As Double)
		  builder.addDouble(4, stdev, 0.0)
	  End Sub
	  Public Shared Sub addCountzero(ByVal builder As FlatBufferBuilder, ByVal countzero As Long)
		  builder.addLong(5, countzero, 0L)
	  End Sub
	  Public Shared Sub addCountpositive(ByVal builder As FlatBufferBuilder, ByVal countpositive As Long)
		  builder.addLong(6, countpositive, 0L)
	  End Sub
	  Public Shared Sub addCountnegative(ByVal builder As FlatBufferBuilder, ByVal countnegative As Long)
		  builder.addLong(7, countnegative, 0L)
	  End Sub
	  Public Shared Sub addCountnan(ByVal builder As FlatBufferBuilder, ByVal countnan As Long)
		  builder.addLong(8, countnan, 0L)
	  End Sub
	  Public Shared Sub addCountinf(ByVal builder As FlatBufferBuilder, ByVal countinf As Long)
		  builder.addLong(9, countinf, 0L)
	  End Sub
	  Public Shared Function endUISummaryStatistics(ByVal builder As FlatBufferBuilder) As Integer
		Dim o As Integer = builder.endObject()
		Return o
	  End Function
	End Class


End Namespace