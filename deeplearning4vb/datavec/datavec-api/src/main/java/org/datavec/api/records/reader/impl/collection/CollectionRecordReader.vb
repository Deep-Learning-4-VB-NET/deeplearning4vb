Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.records.reader.impl.collection



	<Serializable>
	Public Class CollectionRecordReader
		Inherits BaseRecordReader

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private Iterator<? extends Collection<org.datavec.api.writable.Writable>> records;
		Private records As IEnumerator(Of ICollection(Of Writable))
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final Collection<? extends Collection<org.datavec.api.writable.Writable>> original;
		Private ReadOnly original As ICollection(Of ICollection(Of Writable))
		Private count As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public CollectionRecordReader(Collection<? extends Collection<org.datavec.api.writable.Writable>> records)
		Public Sub New(ByVal records As ICollection(Of ICollection(Of Writable)))
			Me.records = records.GetEnumerator()
			Me.original = records
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			initialize(split)
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim next_Conflict As ICollection(Of Writable) = records.next()
			Dim record As IList(Of Writable) = (If(TypeOf next_Conflict Is System.Collections.IList, CType(next_Conflict, IList(Of Writable)), New List(Of Writable)(next_Conflict)))
			invokeListeners(record)
			count += 1
			Return record
		End Function

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return records.hasNext()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose()

		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
    
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Sub reset()
			Me.records = original.GetEnumerator()
			Me.count = 0
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException("Generating records from DataInputStream not supported for CollectionRecordReader")
		End Function


		Public Overrides Function nextRecord() As Record
			Return New org.datavec.api.records.impl.Record([next](), New RecordMetaDataIndex(count - 1, Nothing, GetType(CollectionRecordReader)))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return loadFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim toLoad As ISet(Of Integer) = New HashSet(Of Integer)()
			For Each recordMetaData As RecordMetaData In recordMetaDatas
				If Not (TypeOf recordMetaData Is RecordMetaDataIndex) Then
					Throw New System.ArgumentException("Expected RecordMetaDataIndex; got: " & recordMetaData)
				End If
				Dim idx As Long = DirectCast(recordMetaData, RecordMetaDataIndex).getIndex()
				If idx >= original.Count Then
					Throw New System.InvalidOperationException("Cannot get index " & idx & " from collection: contains " & original & " elements")
				End If
				toLoad.Add(CInt(idx))
			Next recordMetaData

			Dim [out] As IList(Of Record) = New List(Of Record)()
			If TypeOf original Is System.Collections.IList Then
				Dim asList As IList(Of ICollection(Of Writable)) = CType(original, IList(Of ICollection(Of Writable)))
				For Each i As Integer? In toLoad
					Dim l As IList(Of Writable) = New List(Of Writable) From {i}
					Dim r As Record = New org.datavec.api.records.impl.Record(l, New RecordMetaDataIndex(i, Nothing, GetType(CollectionRecordReader)))
					[out].Add(r)
				Next i
			Else
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Iterator<? extends Collection<org.datavec.api.writable.Writable>> iter = original.iterator();
				Dim iter As IEnumerator(Of ICollection(Of Writable)) = original.GetEnumerator()
				Dim i As Integer = 0
				Do While iter.MoveNext()
					Dim c As ICollection(Of Writable) = iter.Current
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (!toLoad.contains(i++))
					If Not toLoad.Contains(i) Then
							i += 1
						Continue Do
						Else
							i += 1
						End If
					Dim l As IList(Of Writable) = (If(TypeOf c Is System.Collections.IList, (CType(c, IList(Of Writable))), New List(Of Writable)(c)))
					Dim r As Record = New org.datavec.api.records.impl.Record(l, New RecordMetaDataIndex(i - 1, Nothing, GetType(CollectionRecordReader)))
					[out].Add(r)
				Loop
			End If
			Return [out]
		End Function
	End Class

End Namespace