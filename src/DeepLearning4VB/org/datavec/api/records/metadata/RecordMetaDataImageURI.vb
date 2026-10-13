Imports System

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

Namespace org.datavec.api.records.metadata

    ''' <summary>
    ''' Record metadata for an image: the source URI plus the original channels/height/width.
    ''' As in the Java original (Lombok <c>@Data</c> without <c>callSuper</c>), <see cref="Equals"/>,
    ''' <see cref="GetHashCode"/> and <see cref="ToString"/> only consider the three image fields.
    ''' </summary>
    <Serializable>
    Public Class RecordMetaDataImageURI
        Inherits RecordMetaDataURI

        Private origCValue As Integer
        Private origHValue As Integer
        Private origWValue As Integer

        Public Sub New(ByVal uri As Uri, ByVal readerClass As Type, ByVal origC As Integer, ByVal origH As Integer, ByVal origW As Integer)
            MyBase.New(uri, readerClass)
            Me.origCValue = origC
            Me.origHValue = origH
            Me.origWValue = origW
        End Sub

        Public Function getOrigC() As Integer
            Return origCValue
        End Function

        Public Function getOrigH() As Integer
            Return origHValue
        End Function

        Public Function getOrigW() As Integer
            Return origWValue
        End Function

        Public Sub setOrigC(ByVal origC As Integer)
            origCValue = origC
        End Sub

        Public Sub setOrigH(ByVal origH As Integer)
            origHValue = origH
        End Sub

        Public Sub setOrigW(ByVal origW As Integer)
            origWValue = origW
        End Sub

        Public Property OrigC As Integer
            Get
                Return origCValue
            End Get
            Set(ByVal value As Integer)
                origCValue = value
            End Set
        End Property

        Public Property OrigH As Integer
            Get
                Return origHValue
            End Get
            Set(ByVal value As Integer)
                origHValue = value
            End Set
        End Property

        Public Property OrigW As Integer
            Get
                Return origWValue
            End Get
            Set(ByVal value As Integer)
                origWValue = value
            End Set
        End Property

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other As RecordMetaDataImageURI = TryCast(o, RecordMetaDataImageURI)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return origCValue = other.origCValue AndAlso origHValue = other.origHValue AndAlso origWValue = other.origWValue
        End Function

        Protected Overrides Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataImageURI
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + origCValue
            result = result * 59 + origHValue
            result = result * 59 + origWValue
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "RecordMetaDataImageURI(origC=" & origCValue & ", origH=" & origHValue & ", origW=" & origWValue & ")"
        End Function

    End Class

End Namespace
