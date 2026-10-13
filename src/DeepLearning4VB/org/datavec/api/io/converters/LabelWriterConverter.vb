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

Imports System.Collections.Generic
Imports WritableConverter = DeepLearning4VB.org.datavec.api.io.WritableConverter
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable

Namespace org.datavec.api.io.converters

    ''' <summary>Converts a label (the writable's string form) to its index in a list of labels (-1 if unknown).</summary>
    Public Class LabelWriterConverter
        Implements WritableConverter

        Private ReadOnly labels As IList(Of String)

        Public Sub New(ByVal labels As IList(Of String))
            Me.labels = labels
        End Sub

        Public Overridable Function convert(ByVal writable As writable.Writable) As writable.Writable Implements WritableConverter.convert
            Dim label As String = writable.ToString()
            Return New IntWritable(labels.IndexOf(label))
        End Function
    End Class

End Namespace
