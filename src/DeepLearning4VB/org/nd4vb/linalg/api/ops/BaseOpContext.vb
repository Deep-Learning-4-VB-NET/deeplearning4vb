Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' Implementation of common methods for OpContext: the arguments and the input and output arrays are kept in
    ''' managed lists and dictionaries (Java's "fastpath" fields). The managed <c>OpContext</c> of the backend adds the
    ''' RNG states, the execution flags and <c>Dispose</c>.
    ''' <para>
    ''' Differences from Java: an array index that was never set reads as Nothing, as Java's <c>Map.get</c> does;
    ''' the native-only <c>contextPointer()</c> is not ported; the field <c>executionMode</c> is named
    ''' <see cref="executionModeField"/> and its accessors are
    ''' <see cref="getExecutionMode"/> and <see cref="setExecutionMode"/>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseOpContext
        Implements OpContext

        Protected Friend fastpath_in As IDictionary(Of Integer, INDArray) = New Dictionary(Of Integer, INDArray)()
        Protected Friend fastpath_out As IDictionary(Of Integer, INDArray) = New Dictionary(Of Integer, INDArray)()

        Protected Friend fastpath_t As IList(Of Double) = New List(Of Double)()
        Protected Friend fastpath_b As IList(Of Boolean) = New List(Of Boolean)()
        Protected Friend fastpath_i As IList(Of Long) = New List(Of Long)()
        Protected Friend fastpath_d As IList(Of DataType) = New List(Of DataType)()

        ''' <summary>The execution mode (Java field <c>executionMode</c>).</summary>
        Protected Friend executionModeField As ExecutionMode = ExecutionMode.UNDEFINED

        Public MustOverride Sub shapeFunctionOverride(ByVal reallyOverride As Boolean) Implements OpContext.shapeFunctionOverride

        Public MustOverride Sub allowHelpers(ByVal reallyAllow As Boolean) Implements OpContext.allowHelpers

        Public MustOverride Sub markInplace(ByVal reallyInplace As Boolean) Implements OpContext.markInplace

        Public MustOverride ReadOnly Property RngStates As Pair(Of Long, Long) Implements OpContext.RngStates

        Public MustOverride Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long) Implements OpContext.setRngStates

        Public MustOverride Sub Dispose() Implements IDisposable.Dispose

        Public Overridable Sub setIArguments(ParamArray ByVal arguments() As Long) Implements OpContext.setIArguments
            fastpath_i.Clear()
            For Each v As Long In arguments
                fastpath_i.Add(v)
            Next v
        End Sub

        Public Overridable Function getIArguments() As IList(Of Long) Implements OpContext.getIArguments
            Return fastpath_i
        End Function

        Public Overridable Function numIArguments() As Integer Implements OpContext.numIArguments
            Return fastpath_i.Count
        End Function

        Public Overridable Sub setTArguments(ParamArray ByVal arguments() As Double) Implements OpContext.setTArguments
            fastpath_t.Clear()
            For Each v As Double In arguments
                fastpath_t.Add(v)
            Next v
        End Sub

        Public Overridable Function getTArguments() As IList(Of Double) Implements OpContext.getTArguments
            Return fastpath_t
        End Function

        Public Overridable Function numTArguments() As Integer Implements OpContext.numTArguments
            Return fastpath_t.Count
        End Function

        Public Overridable Sub setBArguments(ParamArray ByVal arguments() As Boolean) Implements OpContext.setBArguments
            fastpath_b.Clear()
            For Each v As Boolean In arguments
                fastpath_b.Add(v)
            Next v
        End Sub

        Public Overridable Function getBArguments() As IList(Of Boolean) Implements OpContext.getBArguments
            Return fastpath_b
        End Function

        Public Overridable Function numBArguments() As Integer Implements OpContext.numBArguments
            Return fastpath_b.Count
        End Function

        Public Overridable Sub setDArguments(ParamArray ByVal arguments() As DataType) Implements OpContext.setDArguments
            fastpath_d.Clear()
            For Each v As DataType In arguments
                fastpath_d.Add(v)
            Next v
        End Sub

        Public Overridable Function getDArguments() As IList(Of DataType) Implements OpContext.getDArguments
            Return fastpath_d
        End Function

        Public Overridable Function numDArguments() As Integer Implements OpContext.numDArguments
            Return fastpath_d.Count
        End Function

        Public Overridable Sub setInputArray(ByVal index As Integer, ByVal array As INDArray) Implements OpContext.setInputArray
            If array Is Nothing Then
                Throw New ArgumentNullException(NameOf(array))
            End If
            fastpath_in(index) = array
        End Sub

        ''' <summary>The input arrays from index 0 up to the first index that is not set.</summary>
        Public Overridable Function getInputArrays() As IList(Of INDArray) Implements OpContext.getInputArrays
            Dim result As New List(Of INDArray)()
            For e As Integer = 0 To Integer.MaxValue - 1
                Dim arr As INDArray = fastpath_in.GetValueOrNull(e)
                If arr IsNot Nothing Then
                    result.Add(arr)
                Else
                    Exit For
                End If
            Next e

            Return result
        End Function

        Public Overridable Sub setInputArrays(ByVal arrays As IList(Of INDArray)) Implements OpContext.setInputArrays
            For e As Integer = 0 To arrays.Count - 1
                setInputArray(e, arrays(e))
            Next e
        End Sub

        Public Overridable Sub setInputArrays(ParamArray ByVal arrays() As INDArray) Implements OpContext.setInputArrays
            For e As Integer = 0 To arrays.Length - 1
                setInputArray(e, arrays(e))
            Next e
        End Sub

        Public Overridable Function numInputArguments() As Integer Implements OpContext.numInputArguments
            Return fastpath_in.Count
        End Function

        ''' <summary>The input array at the index, or Nothing when it is not set.</summary>
        Public Overridable Function getInputArray(ByVal idx As Integer) As INDArray Implements OpContext.getInputArray
            Return fastpath_in.GetValueOrNull(idx)
        End Function

        ''' <summary>The output arrays from index 0 up to the first index that is not set.</summary>
        Public Overridable Function getOutputArrays() As IList(Of INDArray) Implements OpContext.getOutputArrays
            Dim result As New List(Of INDArray)()
            For e As Integer = 0 To Integer.MaxValue - 1
                Dim arr As INDArray = fastpath_out.GetValueOrNull(e)
                If arr IsNot Nothing Then
                    result.Add(arr)
                Else
                    Exit For
                End If
            Next e

            Return result
        End Function

        Public Overridable Sub setOutputArray(ByVal index As Integer, ByVal array As INDArray) Implements OpContext.setOutputArray
            If array Is Nothing Then
                Throw New ArgumentNullException(NameOf(array))
            End If
            fastpath_out(index) = array
        End Sub

        Public Overridable Sub setOutputArrays(ByVal arrays As IList(Of INDArray)) Implements OpContext.setOutputArrays
            For e As Integer = 0 To arrays.Count - 1
                setOutputArray(e, arrays(e))
            Next e
        End Sub

        Public Overridable Sub setOutputArrays(ParamArray ByVal arrays() As INDArray) Implements OpContext.setOutputArrays
            For e As Integer = 0 To arrays.Length - 1
                setOutputArray(e, arrays(e))
            Next e
        End Sub

        ''' <summary>The output array at the index, or Nothing when it is not set.</summary>
        Public Overridable Function getOutputArray(ByVal i As Integer) As INDArray Implements OpContext.getOutputArray
            Return fastpath_out.GetValueOrNull(i)
        End Function

        Public Overridable Function numOutputArguments() As Integer Implements OpContext.numOutputArguments
            Return fastpath_out.Count
        End Function

        Public Overridable Function getExecutionMode() As ExecutionMode Implements OpContext.getExecutionMode
            Return executionModeField
        End Function

        Public Overridable Sub setExecutionMode(ByVal executionMode As ExecutionMode) Implements OpContext.setExecutionMode
            Me.executionModeField = executionMode
        End Sub

        Public Overridable Sub purge() Implements OpContext.purge
            fastpath_in.Clear()
            fastpath_out.Clear()
        End Sub

    End Class

End Namespace
