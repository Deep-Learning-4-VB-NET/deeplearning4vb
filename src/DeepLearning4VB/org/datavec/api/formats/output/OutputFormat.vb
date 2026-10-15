Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter

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

Namespace org.datavec.api.formats.output

    ''' <summary>
    ''' An output format creates record writers. The <c>OUTPUT_PATH</c> constant the Java interface
    ''' declared lives in <see cref="OutputFormatConstants"/> (VB interfaces cannot hold fields).
    ''' </summary>
    Public Interface OutputFormat

        ''' <summary>
        ''' Create a record writer </summary>
        ''' <returns> the created writer </returns>
        ''' <exception cref="org.datavec.api.exceptions.DataVecException"> </exception>
        Function createWriter(ByVal conf As Configuration) As RecordWriter

    End Interface

    ''' <summary>The constant the Java <c>OutputFormat</c> interface declared.</summary>
    Public Module OutputFormatConstants

        ''' <summary>Configuration key of the output path (<c>OutputFormat.OUTPUT_PATH</c> in Java).</summary>
        Public Const OUTPUT_PATH As String = "org.nd4j.outputpath"

    End Module

End Namespace
