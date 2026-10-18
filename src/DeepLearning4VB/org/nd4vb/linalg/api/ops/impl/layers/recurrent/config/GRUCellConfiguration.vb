Imports System
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If

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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.config.GRUCellConfiguration</c>.</summary>
    Public Class GRUCellConfiguration

#If SAMEDIFF Then
	'    
	'    Inputs:
	'    x        input [bS x inSize]
	'    hLast    previous cell output [bS x numUnits],  that is at previous time step t-1
	'    Wru      RU weights - [bS, 2*numUnits] - reset and update gates
	'    Wc       C weights - [bS, numUnits] - cell gate
	'    bru      r and u biases, [2*numUnits] - reset and update gates
	'    bc       c biases, [numUnits] - cell gate
	'     

		Private xt, hLast, Wru, Wc, bru, bc As SDVariable

		Public Overridable Function args() As SDVariable()
			Return New SDVariable() {xt, hLast, Wru, Wc, bru, bc}
		End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>xt</c>.</summary>
        Public Overridable Function getXt() As SDVariable
            Return xt
        End Function

        ''' <summary>Lombok setter of <c>xt</c>.</summary>
        Public Overridable Sub setXt(ByVal value As SDVariable)
            Me.xt = value
        End Sub

        ''' <summary>Lombok getter of <c>hLast</c>.</summary>
        Public Overridable Function getHLast() As SDVariable
            Return hLast
        End Function

        ''' <summary>Lombok setter of <c>hLast</c>.</summary>
        Public Overridable Sub setHLast(ByVal value As SDVariable)
            Me.hLast = value
        End Sub

        ''' <summary>Lombok getter of <c>Wru</c>.</summary>
        Public Overridable Function getWru() As SDVariable
            Return Wru
        End Function

        ''' <summary>Lombok setter of <c>Wru</c>.</summary>
        Public Overridable Sub setWru(ByVal value As SDVariable)
            Me.Wru = value
        End Sub

        ''' <summary>Lombok getter of <c>Wc</c>.</summary>
        Public Overridable Function getWc() As SDVariable
            Return Wc
        End Function

        ''' <summary>Lombok setter of <c>Wc</c>.</summary>
        Public Overridable Sub setWc(ByVal value As SDVariable)
            Me.Wc = value
        End Sub

        ''' <summary>Lombok getter of <c>bru</c>.</summary>
        Public Overridable Function getBru() As SDVariable
            Return bru
        End Function

        ''' <summary>Lombok setter of <c>bru</c>.</summary>
        Public Overridable Sub setBru(ByVal value As SDVariable)
            Me.bru = value
        End Sub

        ''' <summary>Lombok getter of <c>bc</c>.</summary>
        Public Overridable Function getBc() As SDVariable
            Return bc
        End Function

        ''' <summary>Lombok setter of <c>bc</c>.</summary>
        Public Overridable Sub setBc(ByVal value As SDVariable)
            Me.bc = value
        End Sub
#End If

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As GRUCellConfiguration = TryCast(o, GRUCellConfiguration)
            If other Is Nothing Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "GRUCellConfiguration()"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As GRUCellConfigurationBuilder
            Return New GRUCellConfigurationBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="GRUCellConfiguration"/>; unset fields keep their defaults.</summary>
        Public Class GRUCellConfigurationBuilder

#If SAMEDIFF Then
            Private _xt As SDVariable
            Private _hLast As SDVariable
            Private _Wru As SDVariable
            Private _Wc As SDVariable
            Private _bru As SDVariable
            Private _bc As SDVariable
#End If

#If SAMEDIFF Then
            Public Function xt(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _xt = value
                Return Me
            End Function

            Public Function hLast(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _hLast = value
                Return Me
            End Function

            Public Function Wru(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _Wru = value
                Return Me
            End Function

            Public Function Wc(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _Wc = value
                Return Me
            End Function

            Public Function bru(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _bru = value
                Return Me
            End Function

            Public Function bc(ByVal value As SDVariable) As GRUCellConfigurationBuilder
                _bc = value
                Return Me
            End Function

#End If
            Public Function build() As GRUCellConfiguration
                Dim result As New GRUCellConfiguration()
#If SAMEDIFF Then
                result.xt = _xt
                result.hLast = _hLast
                result.Wru = _Wru
                result.Wc = _Wc
                result.bru = _bru
                result.bc = _bc
#End If
                Return result
            End Function

        End Class

    End Class

End Namespace
