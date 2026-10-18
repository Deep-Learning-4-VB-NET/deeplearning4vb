Imports System
Imports System.Collections.Generic

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.config

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig</c>.</summary>
    Public Class LSTMLayerConfig

        Public Sub New()
        End Sub

        Private lstmdataformat As LSTMDataFormat = LSTMDataFormat.TNS 'INT_ARG(0)

        ''' <summary>
        ''' direction <br>
        ''' FWD: 0 = fwd
        ''' BWD: 1 = bwd
        ''' BS: 2 = bidirectional sum
        ''' BC: 3 = bidirectional concat
        ''' BE: 4 = bidirectional extra output dim (in conjunction with format dataFormat = 3)
        ''' </summary>
        Private directionMode As LSTMDirectionMode = LSTMDirectionMode.FWD 'INT_ARG(1)

        ''' <summary>
        ''' Activation for input (i), forget (f) and output (o) gates
        ''' </summary>
        Private gateAct As LSTMActivations = LSTMActivations.SIGMOID ' INT_ARG(2)

        Private cellAct As LSTMActivations = LSTMActivations.TANH ' INT_ARG(3)

        Private outAct As LSTMActivations = LSTMActivations.TANH ' INT_ARG(4)

        ''' <summary>
        ''' indicates whether to return whole time sequence h {h_0, h_1, ... , h_sL-1}
        ''' </summary>
        Private retFullSequence As Boolean = True 'B_ARG(5)

        ''' <summary>
        ''' indicates whether to return output at last time step only,
        ''' in this case shape would be [bS, nOut] (exact shape depends on dataFormat argument)
        ''' </summary>
        Private retLastH As Boolean 'B_ARG(6)

        ''' <summary>
        ''' indicates whether to return cells state at last time step only,
        ''' in this case shape would be [bS, nOut] (exact shape depends on dataFormat argument)
        ''' </summary>
        Private retLastC As Boolean ' B_ARG(7)

        ''' <summary>
        ''' Cell clipping value, if it = 0 then do not apply clipping
        ''' </summary>
        Private cellClip As Double = 0 'T_ARG(0)

        Public Overridable Function toProperties(ByVal includeLSTMDataFormat As Boolean, ByVal includeLSTMDirectionMode As Boolean) As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("gateAct") = gateAct.ToString()
            ret("outAct") = outAct.ToString()
            ret("cellAct") = cellAct.ToString()
            ret("retFullSequence") = retFullSequence
            ret("retLastH") = retLastH
            ret("retLastC") = retLastC
            ret("cellClip") = cellClip

            If includeLSTMDataFormat Then
                ret("lstmdataformat") = lstmdataformat.ToString()
            End If
            If includeLSTMDirectionMode Then
                ret("directionMode") = directionMode.ToString()
            End If
            Return ret
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>lstmdataformat</c>.</summary>
        Public Overridable Function getLstmdataformat() As LSTMDataFormat
            Return lstmdataformat
        End Function

        ''' <summary>Lombok setter of <c>lstmdataformat</c>.</summary>
        Public Overridable Sub setLstmdataformat(ByVal value As LSTMDataFormat)
            Me.lstmdataformat = value
        End Sub

        ''' <summary>Lombok getter of <c>directionMode</c>.</summary>
        Public Overridable Function getDirectionMode() As LSTMDirectionMode
            Return directionMode
        End Function

        ''' <summary>Lombok setter of <c>directionMode</c>.</summary>
        Public Overridable Sub setDirectionMode(ByVal value As LSTMDirectionMode)
            Me.directionMode = value
        End Sub

        ''' <summary>Lombok getter of <c>gateAct</c>.</summary>
        Public Overridable Function getGateAct() As LSTMActivations
            Return gateAct
        End Function

        ''' <summary>Lombok setter of <c>gateAct</c>.</summary>
        Public Overridable Sub setGateAct(ByVal value As LSTMActivations)
            Me.gateAct = value
        End Sub

        ''' <summary>Lombok getter of <c>cellAct</c>.</summary>
        Public Overridable Function getCellAct() As LSTMActivations
            Return cellAct
        End Function

        ''' <summary>Lombok setter of <c>cellAct</c>.</summary>
        Public Overridable Sub setCellAct(ByVal value As LSTMActivations)
            Me.cellAct = value
        End Sub

        ''' <summary>Lombok getter of <c>outAct</c>.</summary>
        Public Overridable Function getOutAct() As LSTMActivations
            Return outAct
        End Function

        ''' <summary>Lombok setter of <c>outAct</c>.</summary>
        Public Overridable Sub setOutAct(ByVal value As LSTMActivations)
            Me.outAct = value
        End Sub

        ''' <summary>Lombok getter of <c>retFullSequence</c>.</summary>
        Public Overridable Function isRetFullSequence() As Boolean
            Return retFullSequence
        End Function

        ''' <summary>Lombok setter of <c>retFullSequence</c>.</summary>
        Public Overridable Sub setRetFullSequence(ByVal value As Boolean)
            Me.retFullSequence = value
        End Sub

        ''' <summary>Lombok getter of <c>retLastH</c>.</summary>
        Public Overridable Function isRetLastH() As Boolean
            Return retLastH
        End Function

        ''' <summary>Lombok setter of <c>retLastH</c>.</summary>
        Public Overridable Sub setRetLastH(ByVal value As Boolean)
            Me.retLastH = value
        End Sub

        ''' <summary>Lombok getter of <c>retLastC</c>.</summary>
        Public Overridable Function isRetLastC() As Boolean
            Return retLastC
        End Function

        ''' <summary>Lombok setter of <c>retLastC</c>.</summary>
        Public Overridable Sub setRetLastC(ByVal value As Boolean)
            Me.retLastC = value
        End Sub

        ''' <summary>Lombok getter of <c>cellClip</c>.</summary>
        Public Overridable Function getCellClip() As Double
            Return cellClip
        End Function

        ''' <summary>Lombok setter of <c>cellClip</c>.</summary>
        Public Overridable Sub setCellClip(ByVal value As Double)
            Me.cellClip = value
        End Sub

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As LSTMLayerConfig = TryCast(o, LSTMLayerConfig)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(lstmdataformat, other.lstmdataformat) Then
                Return False
            End If
            If Not Object.Equals(directionMode, other.directionMode) Then
                Return False
            End If
            If Not Object.Equals(gateAct, other.gateAct) Then
                Return False
            End If
            If Not Object.Equals(cellAct, other.cellAct) Then
                Return False
            End If
            If Not Object.Equals(outAct, other.outAct) Then
                Return False
            End If
            If Not Object.Equals(retFullSequence, other.retFullSequence) Then
                Return False
            End If
            If Not Object.Equals(retLastH, other.retLastH) Then
                Return False
            End If
            If Not Object.Equals(retLastC, other.retLastC) Then
                Return False
            End If
            If Not Object.Equals(cellClip, other.cellClip) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(lstmdataformat) Is Nothing, 43, lstmdataformat.GetHashCode())
            result = result * 59 + If(CObj(directionMode) Is Nothing, 43, directionMode.GetHashCode())
            result = result * 59 + If(CObj(gateAct) Is Nothing, 43, gateAct.GetHashCode())
            result = result * 59 + If(CObj(cellAct) Is Nothing, 43, cellAct.GetHashCode())
            result = result * 59 + If(CObj(outAct) Is Nothing, 43, outAct.GetHashCode())
            result = result * 59 + If(CObj(retFullSequence) Is Nothing, 43, retFullSequence.GetHashCode())
            result = result * 59 + If(CObj(retLastH) Is Nothing, 43, retLastH.GetHashCode())
            result = result * 59 + If(CObj(retLastC) Is Nothing, 43, retLastC.GetHashCode())
            result = result * 59 + If(CObj(cellClip) Is Nothing, 43, cellClip.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields.</summary>
        Public Overrides Function ToString() As String
            Return "LSTMLayerConfig(lstmdataformat=" & Arrays.FormatElement(lstmdataformat) & ", directionMode=" & Arrays.FormatElement(directionMode) & ", gateAct=" & Arrays.FormatElement(gateAct) & ", cellAct=" & Arrays.FormatElement(cellAct) & ", outAct=" & Arrays.FormatElement(outAct) & ", retFullSequence=" & Arrays.FormatElement(retFullSequence) & ", retLastH=" & Arrays.FormatElement(retLastH) & ", retLastC=" & Arrays.FormatElement(retLastC) & ", cellClip=" & Arrays.FormatElement(cellClip) & ")"
        End Function

        ''' <summary>Lombok's all-arguments constructor.</summary>
        Public Sub New(ByVal lstmdataformat As LSTMDataFormat, ByVal directionMode As LSTMDirectionMode, ByVal gateAct As LSTMActivations, ByVal cellAct As LSTMActivations, ByVal outAct As LSTMActivations, ByVal retFullSequence As Boolean, ByVal retLastH As Boolean, ByVal retLastC As Boolean, ByVal cellClip As Double)
            Me.lstmdataformat = lstmdataformat
            Me.directionMode = directionMode
            Me.gateAct = gateAct
            Me.cellAct = cellAct
            Me.outAct = outAct
            Me.retFullSequence = retFullSequence
            Me.retLastH = retLastH
            Me.retLastC = retLastC
            Me.cellClip = cellClip
        End Sub

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LSTMLayerConfigBuilder
            Return New LSTMLayerConfigBuilder()
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>lstmdataformat</c>.</summary>
        Private Shared Function Default_lstmdataformat() As LSTMDataFormat
            Return LSTMDataFormat.TNS
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>directionMode</c>.</summary>
        Private Shared Function Default_directionMode() As LSTMDirectionMode
            Return LSTMDirectionMode.FWD
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>gateAct</c>.</summary>
        Private Shared Function Default_gateAct() As LSTMActivations
            Return LSTMActivations.SIGMOID
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>cellAct</c>.</summary>
        Private Shared Function Default_cellAct() As LSTMActivations
            Return LSTMActivations.TANH
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>outAct</c>.</summary>
        Private Shared Function Default_outAct() As LSTMActivations
            Return LSTMActivations.TANH
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>retFullSequence</c>.</summary>
        Private Shared Function Default_retFullSequence() As Boolean
            Return True
        End Function

        ''' <summary>The <c>@Builder.Default</c> value of <c>cellClip</c>.</summary>
        Private Shared Function Default_cellClip() As Double
            Return 0
        End Function

        ''' <summary>Lombok's builder of <see cref="LSTMLayerConfig"/>; unset fields keep their defaults.</summary>
        Public Class LSTMLayerConfigBuilder

            Private _lstmdataformat As LSTMDataFormat = Default_lstmdataformat()
            Private _directionMode As LSTMDirectionMode = Default_directionMode()
            Private _gateAct As LSTMActivations = Default_gateAct()
            Private _cellAct As LSTMActivations = Default_cellAct()
            Private _outAct As LSTMActivations = Default_outAct()
            Private _retFullSequence As Boolean = Default_retFullSequence()
            Private _retLastH As Boolean
            Private _retLastC As Boolean
            Private _cellClip As Double = Default_cellClip()

            Public Function lstmdataformat(ByVal value As LSTMDataFormat) As LSTMLayerConfigBuilder
                _lstmdataformat = value
                Return Me
            End Function

            Public Function directionMode(ByVal value As LSTMDirectionMode) As LSTMLayerConfigBuilder
                _directionMode = value
                Return Me
            End Function

            Public Function gateAct(ByVal value As LSTMActivations) As LSTMLayerConfigBuilder
                _gateAct = value
                Return Me
            End Function

            Public Function cellAct(ByVal value As LSTMActivations) As LSTMLayerConfigBuilder
                _cellAct = value
                Return Me
            End Function

            Public Function outAct(ByVal value As LSTMActivations) As LSTMLayerConfigBuilder
                _outAct = value
                Return Me
            End Function

            Public Function retFullSequence(ByVal value As Boolean) As LSTMLayerConfigBuilder
                _retFullSequence = value
                Return Me
            End Function

            Public Function retLastH(ByVal value As Boolean) As LSTMLayerConfigBuilder
                _retLastH = value
                Return Me
            End Function

            Public Function retLastC(ByVal value As Boolean) As LSTMLayerConfigBuilder
                _retLastC = value
                Return Me
            End Function

            Public Function cellClip(ByVal value As Double) As LSTMLayerConfigBuilder
                _cellClip = value
                Return Me
            End Function

            Public Function build() As LSTMLayerConfig
                Return New LSTMLayerConfig(_lstmdataformat, _directionMode, _gateAct, _cellAct, _outAct, _retFullSequence, _retLastH, _retLastC, _cellClip)
            End Function

        End Class

    End Class

End Namespace
