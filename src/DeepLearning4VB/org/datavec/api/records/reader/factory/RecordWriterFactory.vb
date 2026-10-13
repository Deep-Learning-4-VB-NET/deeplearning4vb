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
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter

Namespace org.datavec.api.records.reader.factory

    ''' <summary>Creates record writers for URIs.</summary>
    Public Interface RecordWriterFactory

        ''' <param name="uri"> destination for saving model </param>
        ''' <returns> record writer instance </returns>
        ''' <exception cref="Exception"> </exception>
        Function create(ByVal uri As Uri) As RecordWriter
    End Interface

End Namespace
