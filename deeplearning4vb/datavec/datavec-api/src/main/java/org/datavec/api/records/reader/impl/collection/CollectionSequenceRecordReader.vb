Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = org.datavec.api.records.metadata.RecordMetaDataIndex
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
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
	Public Class CollectionSequenceRecordReader
		Inherits BaseRecordReader
		Implements SequenceRecordReader

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private Iterator<? extends Collection<? extends Collection<org.datavec.api.writable.Writable>>> records;
		Private records As IEnumerator(Of ICollection(Of ICollection(Of Writable)))
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private final Collection<? extends Collection<? extends Collection<org.datavec.api.writable.Writable>>> original;
		Private ReadOnly original As ICollection(Of ICollection(Of ICollection(Of Writable)))
		Private count As Integer = 0

		''' 
		''' <param name="records">    Collection of sequences. For example, List<List<List<Writable>>> where the inner  two lists
		'''                   are a sequence, and the outer list/collection is a list of sequences </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
'ORIGINAL LINE: public CollectionSequenceRecordReader(Collection<? extends Collection<? extends Collection<org.datavec.api.writable.Writable>>> records)
		Public Sub New(ByVal records As ICollection(Of ICollection(Of ICollection(Of Writable))))
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
			Throw New System.NotSupportedException("next() not supported for CollectionSequencRecordReader; use sequenceRecord()")
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
			Throw New System.NotSupportedException("Generating records from DataInputStream not supported for SequenceCollectionRecordReader")
		End Function

		Public Overrides Function nextRecord() As Record
			Return New org.datavec.api.records.impl.Record([next](), Nothing)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Throw New System.NotSupportedException("Loading from metadata not yet implemented")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Overloads Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Throw New System.NotSupportedException("Loading from metadata not yet implemented")
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public List<List<org.datavec.api.writable.Writable>> sequenceRecord()
		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim record As IList(Of IList(Of Writable)) = toList(records.next())
			invokeListeners(record)
			count += 1
			Return record
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))
			Throw New System.NotSupportedException("Generating records from DataInputStream not supported for SequenceCollectionRecordReader")
		End Function

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Return New org.datavec.api.records.impl.SequenceRecord(sequenceRecord(), New RecordMetaDataIndex(count - 1, Nothing, GetType(CollectionSequenceRecordReader)))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return loadSequenceFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord)
			Dim toLoad As ISet(Of Integer) = New LinkedHashSet(Of Integer)()
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

			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Iterator<? extends Collection<? extends Collection<org.datavec.api.writable.Writable>>> iter = original.iterator();
			Dim iter As IEnumerator(Of ICollection(Of ICollection(Of Writable))) = original.GetEnumerator()
			Dim i As Integer = 0
			Do While iter.MoveNext()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: Collection<? extends Collection<org.datavec.api.writable.Writable>> c = iter.Current;
				Dim c As ICollection(Of ICollection(Of Writable)) = iter.Current
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (!toLoad.contains(i++))
				If Not toLoad.Contains(i) Then
						i += 1
					Continue Do
					Else
						i += 1
					End If
				Dim record As IList(Of IList(Of Writable)) = toList(c)
				Dim r As SequenceRecord = New org.datavec.api.records.impl.SequenceRecord(record, New RecordMetaDataIndex(i - 1, Nothing, GetType(CollectionSequenceRecordReader)))
				[out].Add(r)
			Loop
			Return [out]
		End Function

		Private Shared Function toList(Of T1 As ICollection(Of Writable)(ByVal [next] As ICollection(Of T1)) As IList(Of IList(Of Writable))
			Dim record As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each c As ICollection(Of Writable) In [next]
				record.Add(New List(Of Writable)(c))
			Next c
			Return record
		End Function
	End Class

End Namespace