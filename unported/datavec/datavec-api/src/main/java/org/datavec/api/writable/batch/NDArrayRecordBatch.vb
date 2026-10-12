Imports System.Collections.Generic
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.datavec.api.writable.batch


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NDArrayRecordBatch extends AbstractWritableRecordBatch
	Public Class NDArrayRecordBatch
		Inherits AbstractWritableRecordBatch

		Private arrays As IList(Of INDArray)
'JAVA TO VB CONVERTER NOTE: The field size was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private size_Conflict As Long

		Public Sub New(ParamArray ByVal arrays() As INDArray)
			Me.New(Arrays.asList(arrays))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayRecordBatch(@NonNull List<org.nd4j.linalg.api.ndarray.INDArray> arrays)
		Public Sub New(ByVal arrays As IList(Of INDArray))
			Preconditions.checkArgument(arrays.size() > 0, "Input list must not be empty")
			Me.arrays = arrays
			Me.size_Conflict = arrays.get(0).size(0)

			'Check that dimension 0 matches:
			If arrays.size() > 1 Then
				size_Conflict = arrays.get(0).size(0)
				For i As Integer = 1 To arrays.size() - 1
					If size_Conflict <> arrays.get(i).size(0) Then
						Throw New System.ArgumentException("Invalid input arrays: all arrays must have same size for" & "dimension 0. arrays.get(0).size(0)=" & size_Conflict & ", arrays.get(" & i & ").size(0)=" & arrays.get(i).size(0))
					End If
				Next i
			End If
		End Sub

		Public Overridable ReadOnly Property Count As Integer
			Get
				Return CInt(size_Conflict)
			End Get
		End Property

		Public Overrides Function get(ByVal index As Integer) As IList(Of Writable)
			Preconditions.checkArgument(index >= 0 AndAlso index < size_Conflict, "Invalid index: " & index & ", size = " & size_Conflict)
			Dim [out] As IList(Of Writable) = New List(Of Writable)(CInt(size_Conflict))
			For Each orig As INDArray In arrays
				Dim view As INDArray = getExample(index, orig)
				[out].Add(New NDArrayWritable(view))
			Next orig
			Return [out]
		End Function


		Private Shared Function getExample(ByVal idx As Integer, ByVal from As INDArray) As INDArray
			Dim idxs(from.rank() - 1) As INDArrayIndex
			idxs(0) = NDArrayIndex.interval(idx, idx, True) 'Use interval to avoid collapsing point dimension
			Dim i As Integer=1
			Do While i<from.rank()
				idxs(i) = NDArrayIndex.all()
				i += 1
			Loop
			Return from.get(idxs)
		End Function
	End Class

End Namespace