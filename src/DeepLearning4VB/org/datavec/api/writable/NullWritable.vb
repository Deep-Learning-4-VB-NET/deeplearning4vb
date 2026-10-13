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
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.io

Namespace org.datavec.api.writable

    ''' <summary>A writable with no value. All instances are equal; <see cref="INSTANCE"/> is the shared one.</summary>
    <Serializable>
    Public Class NullWritable
        Implements WritableComparable

        Public Shared ReadOnly INSTANCE As New NullWritable()

        Public Sub New()
        End Sub

        Public Overridable Function CompareTo(ByVal o As Object) As Integer Implements IComparable.CompareTo
            If Me Is o Then
                Return 0
            End If
            If Not (TypeOf o Is NullWritable) Then
                Throw New ArgumentException("Cannot compare NullWritable to " & If(o Is Nothing, "null", o.GetType().ToString()))
            End If
            Return 0
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Return TypeOf o Is NullWritable
        End Function

        ''' <summary>Constant, since every NullWritable is equal (Java used the identity hash).</summary>
        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function

        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            'No op
        End Sub

        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            'No op
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Null.typeIdx())
        End Sub

        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Throw New NotSupportedException("Cannot convert NullWritable to other values")
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Throw New NotSupportedException("Cannot convert NullWritable to other values")
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Throw New NotSupportedException("Cannot convert NullWritable to other values")
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Throw New NotSupportedException("Cannot convert NullWritable to other values")
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Null
        End Function

        Public Overrides Function ToString() As String
            Return "NullWritable"
        End Function
    End Class

End Namespace
