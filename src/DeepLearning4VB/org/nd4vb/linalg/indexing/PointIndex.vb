Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

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

Namespace org.nd4vb.linalg.indexing

    ''' <summary>
    ''' An index that selects one position along a dimension; the dimension does not appear in the result.
    ''' Lombok's <c>@EqualsAndHashCode</c> members are written out.
    ''' </summary>
    ''' @author Adam Gibson
    Public Class PointIndex
        Implements INDArrayIndex

        Private ReadOnly point As Long

        ''' <param name="point"> the position to select </param>
        Public Sub New(ByVal point As Long)
            Me.point = point
        End Sub

        ''' <summary>The point.</summary>
        Public Overridable Function [end]() As Long Implements INDArrayIndex.end
            Return point
        End Function

        ''' <summary>The point.</summary>
        Public Overridable Function offset() As Long Implements INDArrayIndex.offset
            Return point
        End Function

        ''' <summary>1.</summary>
        Public Overridable Function length() As Long Implements INDArrayIndex.length
            Return 1
        End Function

        ''' <summary>1.</summary>
        Public Overridable Function stride() As Long Implements INDArrayIndex.stride
            Return 1
        End Function

        ''' <summary>A no-op.</summary>
        Public Overridable Sub reverse() Implements INDArrayIndex.reverse
        End Sub

        ''' <summary>False.</summary>
        Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.Interval
            Get
                Return False
            End Get
        End Property

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As PointIndex = TryCast(o, PointIndex)
            Return other IsNot Nothing AndAlso point = other.point
        End Function

        ''' <summary>Lombok's hash code: <c>59 + (int)(point &gt;&gt;&gt; 32 ^ point)</c>.</summary>
        Public Overrides Function GetHashCode() As Integer
            Return 59 + Arrays.WrapToInt32(point Xor (point >> 32))
        End Function

        Public Overrides Function ToString() As String
            Return "Point(" & point & ")"
        End Function

    End Class

End Namespace
