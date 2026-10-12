Imports System.Collections.Generic
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports DoubleArrayList = it.unimi.dsi.fastutil.doubles.DoubleArrayList
Imports NonNull = lombok.NonNull
Imports TimeSeriesWritableUtils = org.datavec.api.timeseries.util.TimeSeriesWritableUtils
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.datavec.api.util.ndarray


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class RecordConverter
		Private Sub New()
		End Sub

		''' <summary>
		''' Convert a record to an ndarray </summary>
		''' <param name="record"> the record to convert
		''' </param>
		''' <returns> the array </returns>
		Public Shared Function toArray(ByVal dataType As DataType, ByVal record As ICollection(Of Writable), ByVal size As Integer) As INDArray
			Return toArray(dataType, record)
		End Function

		''' <summary>
		''' Convert a set of records in to a matrix </summary>
		''' <param name="matrix"> the records ot convert </param>
		''' <returns> the matrix for the records </returns>
		Public Shared Function toRecords(ByVal matrix As INDArray) As IList(Of IList(Of Writable))
			Dim ret As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim i As Integer = 0
			Do While i < matrix.rows()
				ret.Add(RecordConverter.toRecord(matrix.getRow(i)))
				i += 1
			Loop

			Return ret
		End Function


		''' <summary>
		''' Convert a set of records in to a matrix </summary>
		''' <param name="records"> the records ot convert </param>
		''' <returns> the matrix for the records </returns>
		Public Shared Function toTensor(ByVal records As IList(Of IList(Of IList(Of Writable)))) As INDArray
		   Return TimeSeriesWritableUtils.convertWritablesSequence(records).First
		End Function

		''' <summary>
		''' Convert a set of records in to a matrix
		''' As per <seealso cref="toMatrix(DataType, List)"/> but hardcoded to Float datatype </summary>
		''' <param name="records"> the records ot convert </param>
		''' <returns> the matrix for the records </returns>
		Public Shared Function toMatrix(ByVal records As IList(Of IList(Of Writable))) As INDArray
			Return toMatrix(DataType.FLOAT, records)
		End Function

		''' <summary>
		''' Convert a set of records in to a matrix </summary>
		''' <param name="records"> the records ot convert </param>
		''' <returns> the matrix for the records </returns>
		Public Shared Function toMatrix(ByVal dataType As DataType, ByVal records As IList(Of IList(Of Writable))) As INDArray
			Dim toStack As IList(Of INDArray) = New List(Of INDArray)()
			For Each l As IList(Of Writable) In records
				toStack.Add(toArray(dataType, l))
			Next l

			Return Nd4j.vstack(toStack)
		End Function

		''' <summary>
		''' Convert a record to an INDArray. May contain a mix of Writables and row vector NDArrayWritables.
		''' As per <seealso cref="toArray(DataType, Collection)"/> but hardcoded to Float datatype </summary>
		''' <param name="record"> the record to convert </param>
		''' <returns> the array </returns>
		Public Shared Function toArray(Of T1 As Writable)(ByVal record As ICollection(Of T1)) As INDArray
			Return toArray(DataType.FLOAT, record)
		End Function

		''' <summary>
		''' Convert a record to an INDArray. May contain a mix of Writables and row vector NDArrayWritables. </summary>
		''' <param name="record"> the record to convert </param>
		''' <returns> the array </returns>
		Public Shared Function toArray(Of T1 As Writable)(ByVal dataType As DataType, ByVal record As ICollection(Of T1)) As INDArray
			Dim l As IList(Of Writable)
			If TypeOf record Is System.Collections.IList Then
				l = CType(record, IList(Of Writable))
			Else
				l = New List(Of Writable)(record)
			End If

			'Edge case: single NDArrayWritable
			If l.Count = 1 AndAlso TypeOf l(0) Is NDArrayWritable Then
				Return DirectCast(l(0), NDArrayWritable).get()
			End If

			Dim length As Integer = 0
			For Each w As Writable In record
				If TypeOf w Is NDArrayWritable Then
					Dim a As INDArray = DirectCast(w, NDArrayWritable).get()
					If Not a.RowVector Then
						Throw New System.NotSupportedException("Multiple writables present but NDArrayWritable is " & "not a row vector. Can only concat row vectors with other writables. Shape: " & Arrays.toString(a.shape()))
					End If
					length += a.length()
				Else
					'Assume all others are single value
					length += 1
				End If
			Next w

			Dim arr As INDArray = Nd4j.create(dataType, 1, length)

			Dim k As Integer = 0
			For Each w As Writable In record
				If TypeOf w Is NDArrayWritable Then
					Dim toPut As INDArray = DirectCast(w, NDArrayWritable).get()
					arr.put(New INDArrayIndex() {NDArrayIndex.point(0), NDArrayIndex.interval(k, k + toPut.length())}, toPut)
					k += toPut.length()
				Else
					arr.putScalar(0, k, w.toDouble())
					k += 1
				End If
			Next w

			Return arr
		End Function

		''' <summary>
		''' Convert a record to an INDArray, for use in minibatch training. That is, for an input record of length N, the output
		''' array has dimension 0 of size N (i.e., suitable for minibatch training in DL4J, for example).<br>
		''' The input list of writables must all be the same type (i.e., all NDArrayWritables or all non-array writables such
		''' as DoubleWritable etc).<br>
		''' Note that for NDArrayWritables, they must have leading dimension 1, and all other dimensions must match. <br>
		''' For example, row vectors are valid NDArrayWritables, as are 3d (usually time series) with shape [1, x, y], or
		''' 4d (usually images) with shape [1, x, y, z] where (x,y,z) are the same for all inputs </summary>
		''' <param name="l"> the records to convert </param>
		''' <returns> the array </returns>
		''' <seealso cref= #toArray(Collection) for the "single example concatenation" version of this method </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray toMinibatchArray(@NonNull List<? extends Writable> l)
		Public Shared Function toMinibatchArray(Of T1 As Writable)(ByVal l As IList(Of T1)) As INDArray
			Preconditions.checkArgument(l.size() > 0, "Cannot convert empty list")

			'Edge case: single NDArrayWritable
			If l.size() = 1 AndAlso TypeOf l.get(0) Is NDArrayWritable Then
				Return CType(l.get(0), NDArrayWritable).get()
			End If

			'Check: all NDArrayWritable or all non-writable
			Dim toConcat As IList(Of INDArray) = Nothing
			Dim list As DoubleArrayList = Nothing
			For Each w As Writable In l
				If TypeOf w Is NDArrayWritable Then
					Dim a As INDArray = DirectCast(w, NDArrayWritable).get()
					If a.size(0) <> 1 Then
						Throw New System.NotSupportedException("NDArrayWritable must have leading dimension 1 for this " & "method. Received array with shape: " & Arrays.toString(a.shape()))
					End If
					If toConcat Is Nothing Then
						toConcat = New List(Of INDArray)()
					End If
					toConcat.Add(a)
				Else
					'Assume all others are single value
					If list Is Nothing Then
						list = New DoubleArrayList()
					End If
					list.add(w.toDouble())
				End If
			Next w


			If toConcat IsNot Nothing AndAlso list IsNot Nothing Then
				Throw New System.InvalidOperationException("Error converting writables: found both NDArrayWritable and single value" & " (DoubleWritable etc) in the one list. All writables must be NDArrayWritables or " & "single value writables only for this method")
			End If

			If toConcat IsNot Nothing Then
				Return Nd4j.concat(0, CType(toConcat, List(Of INDArray)).ToArray())
			Else
				Return Nd4j.create(list.toArray(New Double(list.size() - 1){}), New Long(){list.size(), 1}, DataType.FLOAT)
			End If
		End Function

		''' <summary>
		''' Convert an ndarray to a record </summary>
		''' <param name="array"> the array to convert </param>
		''' <returns> the record </returns>
		Public Shared Function toRecord(ByVal array As INDArray) As IList(Of Writable)
			Dim writables As IList(Of Writable) = New List(Of Writable)()
			writables.Add(New NDArrayWritable(array))
			Return writables
		End Function

		''' <summary>
		'''  Convert a collection into a `List<Writable>`, i.e. a record that can be used with other datavec methods.
		'''  Uses a schema to decide what kind of writable to use.
		''' </summary>
		''' <returns> a record </returns>
		Public Shared Function toRecord(ByVal schema As Schema, ByVal source As IList(Of Object)) As IList(Of Writable)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Writable> record = new java.util.ArrayList<>(source.size());
			Dim record As IList(Of Writable) = New List(Of Writable)(source.Count)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.datavec.api.transform.metadata.ColumnMetaData> columnMetaData = schema.getColumnMetaData();
			Dim columnMetaData As IList(Of ColumnMetaData) = schema.getColumnMetaData()

			If columnMetaData.Count <> source.Count Then
				Throw New System.ArgumentException("Schema and source list don't have the same length!")
			End If

			For i As Integer = 0 To columnMetaData.Count - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.api.transform.metadata.ColumnMetaData metaData = columnMetaData.get(i);
				Dim metaData As ColumnMetaData = columnMetaData(i)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Object data = source.get(i);
				Dim data As Object = source(i)
				If Not metaData.isValid(data) Then
					Throw New System.ArgumentException("Element " & i & ": " & data & " is not valid for Column """ & metaData.Name & """ (" & metaData.ColumnType & ")")
				End If

				Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Writable writable;
					Dim writable As Writable
					Select Case metaData.ColumnType.getWritableType()
						Case Single?
							writable = New FloatWritable(DirectCast(data, Single?))
						Case Double?
							writable = New DoubleWritable(DirectCast(data, Double?))
						Case Int
							writable = New IntWritable(DirectCast(data, Integer?))
						Case SByte?
							writable = New ByteWritable(DirectCast(data, SByte?))
						Case Boolean?
							writable = New BooleanWritable(DirectCast(data, Boolean?))
						Case Long?
							writable = New LongWritable(DirectCast(data, Long?))
						Case Null
							writable = New NullWritable()
						Case Bytes
							writable = New BytesWritable(DirectCast(data, SByte()))
						Case NDArray
							writable = New NDArrayWritable(DirectCast(data, INDArray))
						Case Text
							If TypeOf data Is String Then
								writable = New Text(DirectCast(data, String))
							ElseIf TypeOf data Is Text Then
								writable = New Text(DirectCast(data, Text))
							ElseIf TypeOf data Is SByte() Then
								writable = New Text(DirectCast(data, SByte()))
							Else
								Throw New System.ArgumentException("Element " & i & ": " & data & " is not usable for Column """ & metaData.Name & """ (" & metaData.ColumnType & ")")
							End If
						Case Else
							Throw New System.ArgumentException("Element " & i & ": " & data & " is not usable for Column """ & metaData.Name & """ (" & metaData.ColumnType & ")")
					End Select
					record.Add(writable)
				Catch e As System.InvalidCastException
					Throw New System.ArgumentException("Element " & i & ": " & data & " is not usable for Column """ & metaData.Name & """ (" & metaData.ColumnType & ")", e)
				End Try
			Next i

			Return record
		End Function

		''' <summary>
		''' Convert a DataSet to a matrix </summary>
		''' <param name="dataSet"> the DataSet to convert </param>
		''' <returns> the matrix for the records </returns>
		Public Shared Function toRecords(ByVal dataSet As DataSet) As IList(Of IList(Of Writable))
			If isClassificationDataSet(dataSet) Then
				Return getClassificationWritableMatrix(dataSet)
			Else
				Return getRegressionWritableMatrix(dataSet)
			End If
		End Function

		Private Shared Function isClassificationDataSet(ByVal dataSet As DataSet) As Boolean
			Dim labels As INDArray = dataSet.Labels

			Return labels.sum(0, 1).getInt(0) = dataSet.numExamples() AndAlso labels.shape()(1) > 1
		End Function

		Private Shared Function getClassificationWritableMatrix(ByVal dataSet As DataSet) As IList(Of IList(Of Writable))
			Dim writableMatrix As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			Dim i As Integer = 0
			Do While i < dataSet.numExamples()
				Dim writables As IList(Of Writable) = toRecord(dataSet.Features.getRow(i, True))
				writables.Add(New IntWritable(Nd4j.argMax(dataSet.Labels.getRow(i)).getInt(0)))

				writableMatrix.Add(writables)
				i += 1
			Loop

			Return writableMatrix
		End Function

		Private Shared Function getRegressionWritableMatrix(ByVal dataSet As DataSet) As IList(Of IList(Of Writable))
			Dim writableMatrix As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()

			Dim i As Integer = 0
			Do While i < dataSet.numExamples()
				Dim writables As IList(Of Writable) = toRecord(dataSet.Features.getRow(i))
				Dim labelRow As INDArray = dataSet.Labels.getRow(i)

				Dim j As Integer = 0
				Do While j < labelRow.shape()(1)
					writables.Add(New DoubleWritable(labelRow.getDouble(j)))
					j += 1
				Loop

				writableMatrix.Add(writables)
				i += 1
			Loop

			Return writableMatrix
		End Function
	End Class

End Namespace