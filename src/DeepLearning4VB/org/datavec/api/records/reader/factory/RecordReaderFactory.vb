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
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader

Namespace org.datavec.api.records.reader.factory

    ''' <summary>Creates record readers for URIs.</summary>
    Public Interface RecordReaderFactory
        ''' <summary>
        ''' Creates instance of RecordReader
        ''' </summary>
        ''' <param name="uri"> </param>
        ''' <returns> record reader instance </returns>
        ''' <exception cref="org.datavec.api.exceptions.UnknownFormatException"> </exception>
        Function create(ByVal uri As Uri) As RecordReader
    End Interface

End Namespace
