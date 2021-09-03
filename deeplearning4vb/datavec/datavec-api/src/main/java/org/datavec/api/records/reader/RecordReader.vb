Imports System.Collections.Generic
Imports Configurable = org.datavec.api.conf.Configurable
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
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

Namespace org.datavec.api.records.reader


	Public Interface RecordReader
		Inherits Closeable, Configurable

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String NAME_SPACE = RecordReader.class.getName();

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String APPEND_LABEL = NAME_SPACE + ".appendlabel";
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in VB:
'		String LABELS = NAME_SPACE + ".labels";

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException;
		Sub initialize(ByVal split As org.datavec.api.Split.InputSplit)

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="conf">  a configuration for initialization </param>
		''' <param name="split"> the split that defines the range of records to read </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException;
		Sub initialize(ByVal conf As Configuration, ByVal split As org.datavec.api.Split.InputSplit)

		''' <summary>
		''' This method returns true, if next(int) signature is supported by this RecordReader implementation.
		''' 
		''' @return
		''' </summary>
		Function batchesSupported() As Boolean

		''' <summary>
		''' This method will be used, if batchesSupported() returns true.
		''' </summary>
		''' <param name="num">
		''' @return </param>
		Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))

		''' <summary>
		''' Get the next record
		''' 
		''' @return
		''' </summary>
		Function [next]() As IList(Of Writable)



		''' <summary>
		''' Whether there are anymore records
		''' 
		''' @return
		''' </summary>
		Function hasNext() As Boolean

		''' <summary>
		''' List of label strings
		''' 
		''' @return
		''' </summary>
		ReadOnly Property Labels As IList(Of String)

		''' <summary>
		''' Reset record reader iterator
		''' 
		''' @return
		''' </summary>
		Sub reset()

		''' <returns> True if the record reader can be reset, false otherwise. Note that some record readers cannot be reset -
		''' for example, if they are backed by a non-resettable input split (such as certain types of streams) </returns>
		Function resetSupported() As Boolean

		''' <summary>
		''' Load the record from the given DataInputStream
		''' Unlike <seealso cref="next()"/> the internal state of the RecordReader is not modified
		''' Implementations of this method should not close the DataInputStream
		''' </summary>
		''' <exception cref="IOException"> if error occurs during reading from the input stream </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException;
		Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)


		''' <summary>
		''' Similar to <seealso cref="next()"/>, but returns a <seealso cref="Record"/> object, that may include metadata such as the source
		''' of the data
		''' </summary>
		''' <returns> next record </returns>
		Function nextRecord() As Record

		''' <summary>
		''' Load a single record from the given <seealso cref="RecordMetaData"/> instance<br>
		''' Note: that for data that isn't splittable (i.e., text data that needs to be scanned/split), it is more efficient to
		''' load multiple records at once using <seealso cref="loadFromMetaData(List)"/>
		''' </summary>
		''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
		''' <returns> Single record for the given RecordMetaData instance </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException;
		Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record

		''' <summary>
		''' Load multiple records from the given a list of <seealso cref="RecordMetaData"/> instances<br>
		''' </summary>
		''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
		''' <returns> Multiple records for the given RecordMetaData instances </returns>
		''' <exception cref="IOException"> If I/O error occurs during loading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException;
		Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)

		''' <summary>
		''' Get the record listeners for this record reader.
		''' </summary>
		Property Listeners As IList(Of RecordListener)


		''' <summary>
		''' Set the record listeners for this record reader.
		''' </summary>
		WriteOnly Property Listeners As ICollection(Of RecordListener)
	End Interface

End Namespace