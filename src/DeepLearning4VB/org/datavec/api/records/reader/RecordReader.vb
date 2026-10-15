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

Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.JavaCompat
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit

Namespace org.datavec.api.records.reader

    ''' <summary>
    ''' A record reader: reads records (lists of writables) from an <see cref="InputSplit"/>.
    ''' Java's <c>Closeable</c> is <see cref="IDisposable"/>. The constants the Java interface declared
    ''' (<c>NAME_SPACE</c>, <c>APPEND_LABEL</c>, <c>LABELS</c>, the last one as <c>LABELS_KEY</c>) live on <see cref="BaseRecordReader"/>,
    ''' since VB interfaces cannot hold fields.
    ''' </summary>
    Public Interface RecordReader
        Inherits Configurable, IDisposable

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Sub initialize(ByVal split As InputSplit)

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="conf">  a configuration for initialization </param>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)

        ''' <summary>
        ''' This method returns true, if next(int) signature is supported by this RecordReader implementation.
        ''' </summary>
        Function batchesSupported() As Boolean

        ''' <summary>
        ''' This method will be used, if batchesSupported() returns true.
        ''' </summary>
        Function [next](ByVal num As Integer) As IList(Of IList(Of writable.Writable))

        ''' <summary>
        ''' Get the next record
        ''' </summary>
        Function [next]() As IList(Of writable.Writable)

        ''' <summary>
        ''' Whether there are anymore records
        ''' </summary>
        Function hasNext() As Boolean

        ''' <summary>
        ''' List of label strings
        ''' </summary>
        ReadOnly Property Labels As IList(Of String)

        ''' <summary>
        ''' Reset record reader iterator
        ''' </summary>
        Sub reset()

        ''' <returns> True if the record reader can be reset, false otherwise. Note that some record readers cannot be reset -
        ''' for example, if they are backed by a non-resettable input split (such as certain types of streams) </returns>
        Function resetSupported() As Boolean

        ''' <summary>
        ''' Load the record from the given DataInputStream
        ''' Unlike <see cref="next()"/> the internal state of the RecordReader is not modified
        ''' Implementations of this method should not close the DataInputStream
        ''' </summary>
        ''' <exception cref="System.IO.IOException"> if error occurs during reading from the input stream </exception>
        Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)

        ''' <summary>
        ''' Similar to <see cref="next()"/>, but returns a <see cref="Record"/> object, that may include metadata such as the source
        ''' of the data
        ''' </summary>
        ''' <returns> next record </returns>
        Function nextRecord() As Record

        ''' <summary>
        ''' Load a single record from the given <see cref="RecordMetaData"/> instance<br/>
        ''' Note: that for data that isn't splittable (i.e., text data that needs to be scanned/split), it is more efficient to
        ''' load multiple records at once using <see cref="loadFromMetaData(IList(Of RecordMetaData))"/>
        ''' </summary>
        ''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
        ''' <returns> Single record for the given RecordMetaData instance </returns>
        ''' <exception cref="System.IO.IOException"> If I/O error occurs during loading </exception>
        Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record

        ''' <summary>
        ''' Load multiple records from the given a list of <see cref="RecordMetaData"/> instances<br/>
        ''' </summary>
        ''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
        ''' <returns> Multiple records for the given RecordMetaData instances </returns>
        ''' <exception cref="System.IO.IOException"> If I/O error occurs during loading </exception>
        Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)

        ''' <summary>
        ''' The record listeners for this record reader (<c>getListeners()</c>/<c>setListeners(Collection)</c> in Java).
        ''' </summary>
        Property Listeners As IList(Of RecordListener)

        ''' <summary>
        ''' Set the record listeners for this record reader.
        ''' </summary>
        Sub setListeners(ByVal ParamArray listeners() As RecordListener)

        ''' <summary>
        ''' Set the record listeners for this record reader.
        ''' </summary>
        Sub setListeners(ByVal listeners As ICollection(Of RecordListener))

    End Interface

End Namespace
