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

Imports WritableConverter = DeepLearning4VB.org.datavec.api.io.WritableConverter
Imports DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.io.converters

    ''' <summary>Converts text and numeric writables to a <see cref="FloatWritable"/>.</summary>
    Public Class FloatWritableConverter
        Implements WritableConverter

        Public Overridable Function convert(ByVal writable As writable.Writable) As writable.Writable Implements WritableConverter.convert
            If TypeOf writable Is Text OrElse TypeOf writable Is DoubleWritable OrElse TypeOf writable Is IntWritable OrElse TypeOf writable Is FloatWritable Then
                Return New FloatWritable(writable.toFloat())
            End If

            Throw New WritableConverterException("Unable to convert type " & DirectCast(writable, Object).GetType().ToString())
        End Function
    End Class

End Namespace
