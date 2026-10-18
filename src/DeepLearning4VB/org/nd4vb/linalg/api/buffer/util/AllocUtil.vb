Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.buffer.util

    ''' <summary>
    ''' Used for manipulating the allocation mode from the configuration (Java <c>AllocUtil</c>).
    ''' </summary>
    ''' <remarks>
    ''' Java stored the mode in the <c>Nd4jContext</c> configuration; ND4VB stores it in the backend properties
    ''' (<c>Nd4vb.Backend.Properties</c>, see <c>Nd4vbBackend.Properties</c>). As in Java, new buffers always use
    ''' <see cref="DataBuffer.AllocationMode.MIXED_DATA_TYPES"/>. The Java setters <c>setAllocationModeForContext</c> are
    ''' methods (VB properties cannot be overloaded by type).
    ''' </remarks>
    Public Class AllocUtil

        ''' <summary>The configuration key of the allocation mode.</summary>
        Public Const ALLOC_PROPERTY As String = "alloc"

        ''' <summary>
        ''' Get the allocation mode from the context: "heap", "javacpp" or "direct" (anything else gives JAVACPP).
        ''' </summary>
        Public Shared Function getAllocationModeFromContext(ByVal allocMode As String) As DataBuffer.AllocationMode
            Select Case allocMode
                Case "heap"
                    Return DataBuffer.AllocationMode.HEAP
                Case "javacpp"
                    Return DataBuffer.AllocationMode.JAVACPP
                Case "direct"
                    Return DataBuffer.AllocationMode.DIRECT
                Case Else
                    Return DataBuffer.AllocationMode.JAVACPP
            End Select
        End Function

        ''' <summary>Gets the name of the allocation mode ("heap", "javacpp" or "direct"; JAVACPP for the others).</summary>
        Public Shared Function getAllocModeName(ByVal allocationMode As DataBuffer.AllocationMode) As String
            Select Case allocationMode
                Case DataBuffer.AllocationMode.HEAP
                    Return "heap"
                Case DataBuffer.AllocationMode.JAVACPP
                    Return "javacpp"
                Case DataBuffer.AllocationMode.DIRECT
                    Return "direct"
                Case Else
                    Return "javacpp"
            End Select
        End Function

        ''' <summary>The allocation mode of new buffers: always MIXED_DATA_TYPES, as in Java.</summary>
        Public Shared ReadOnly Property AllocationModeFromContext As DataBuffer.AllocationMode
            Get
                Return DataBuffer.AllocationMode.MIXED_DATA_TYPES
            End Get
        End Property

        ''' <summary>Java <c>getAllocationModeFromContext()</c>.</summary>
        Public Shared Function getAllocationModeFromContext() As DataBuffer.AllocationMode
            Return AllocationModeFromContext
        End Function

        ''' <summary>Stores the name of <paramref name="allocationModeForContext"/> in the backend properties.</summary>
        Public Shared Sub setAllocationModeForContext(ByVal allocationModeForContext As DataBuffer.AllocationMode)
            setAllocationModeForContext(getAllocModeName(allocationModeForContext))
        End Sub

        ''' <summary>Stores the allocation mode ("heap", "javacpp" or "direct") in the backend properties.</summary>
        ''' <exception cref="ArgumentException">Another value was given.</exception>
        Public Shared Sub setAllocationModeForContext(ByVal allocationModeForContext As String)
            If Not "heap".Equals(allocationModeForContext) AndAlso Not "javacpp".Equals(allocationModeForContext) AndAlso Not "direct".Equals(allocationModeForContext) Then
                Throw New ArgumentException("Allocation mode must be one of: heap,javacpp, or direct")
            End If
            Nd4vbT.Backend.Properties.put(ALLOC_PROPERTY, allocationModeForContext)
        End Sub

    End Class

End Namespace
