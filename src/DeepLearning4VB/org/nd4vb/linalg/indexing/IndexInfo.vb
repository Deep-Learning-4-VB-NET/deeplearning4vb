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

Namespace org.nd4vb.linalg.indexing

    ''' <summary>
    ''' Counts the point and new-axis indexes of an index list. Java's getters <c>getNumNewAxes()</c> and
    ''' <c>getNumPoints()</c> are the read-only properties <see cref="NumNewAxes"/> and <see cref="NumPoints"/>.
    ''' </summary>
    ''' @author Adam Gibson
    Public Class IndexInfo

        Private ReadOnly indexes() As INDArrayIndex
        Private ReadOnly newAxisCount As Integer = 0
        Private ReadOnly pointCount As Integer = 0

        ''' <param name="indexes"> the indexes to count </param>
        Public Sub New(ParamArray ByVal indexes() As INDArrayIndex)
            Me.indexes = indexes
            For i As Integer = 0 To indexes.Length - 1
                If TypeOf indexes(i) Is PointIndex Then
                    pointCount += 1
                End If
                If TypeOf indexes(i) Is NewAxis Then
                    newAxisCount += 1
                End If
            Next i
        End Sub

        ''' <summary>The number of <see cref="NewAxis"/> indexes.</summary>
        Public Overridable ReadOnly Property NumNewAxes As Integer
            Get
                Return newAxisCount
            End Get
        End Property

        ''' <summary>The number of <see cref="PointIndex"/> indexes.</summary>
        Public Overridable ReadOnly Property NumPoints As Integer
            Get
                Return pointCount
            End Get
        End Property

    End Class

End Namespace
