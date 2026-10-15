Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit

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

Namespace org.datavec.api.formats.input

    ''' <summary>
    ''' An input format creates the record reader for an input split. Like in Java, input formats are
    ''' themselves <see cref="writable.Writable"/>s.
    ''' </summary>
    Public Interface InputFormat
        Inherits writable.Writable

        ''' <summary>
        ''' Creates a reader from an input split </summary>
        ''' <param name="split"> the split to read </param>
        ''' <returns> the reader from the given input split </returns>
        Function createReader(ByVal split As InputSplit, ByVal conf As Configuration) As RecordReader

        ''' <summary>
        ''' Creates a reader from an input split </summary>
        ''' <param name="split"> the split to read </param>
        ''' <returns> the reader from the given input split </returns>
        Function createReader(ByVal split As InputSplit) As RecordReader

    End Interface

End Namespace
