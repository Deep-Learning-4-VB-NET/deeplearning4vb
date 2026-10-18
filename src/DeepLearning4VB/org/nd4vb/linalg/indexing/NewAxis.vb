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
    ''' An index that inserts a new dimension of size 1 into the result.
    ''' </summary>
    Public Class NewAxis
        Implements INDArrayIndex

        ''' <summary>0.</summary>
        Public Overridable Function [end]() As Long Implements INDArrayIndex.end
            Return 0
        End Function

        ''' <summary>0.</summary>
        Public Overridable Function offset() As Long Implements INDArrayIndex.offset
            Return 0
        End Function

        ''' <summary>0.</summary>
        Public Overridable Function length() As Long Implements INDArrayIndex.length
            Return 0
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

        Public Overrides Function ToString() As String
            Return "newAxis()"
        End Function

    End Class

End Namespace
