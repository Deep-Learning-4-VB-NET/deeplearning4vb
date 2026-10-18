Imports System
Imports System.Collections.Generic
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMCellConfiguration</c>.</summary>
    Public Class LSTMCellConfiguration

        ''' <summary>
        '''   NDArray<T>* xt   = INPUT_VARIABLE(0);                   // input [batchSize x inSize]
        ''' NDArray<T>* ht_1 = INPUT_VARIABLE(1);                   // previous cell output [batchSize x numProj],  that is at previous time step t-1, in case of projection=false -> numProj=numUnits!!!
        ''' NDArray<T>* ct_1 = INPUT_VARIABLE(2);                   // previous cell state  [batchSize x numUnits], that is at previous time step t-1
        ''' 
        ''' NDArray<T>* Wx   = INPUT_VARIABLE(3);                   // input-to-hidden  weights, [inSize  x 4*numUnits]
        ''' NDArray<T>* Wh   = INPUT_VARIABLE(4);                   // hidden-to-hidden weights, [numProj x 4*numUnits]
        ''' NDArray<T>* Wc   = INPUT_VARIABLE(5);                   // diagonal weights for peephole connections [1 x 3*numUnits]
        ''' NDArray<T>* Wp   = INPUT_VARIABLE(6);                   // projection weights [numUnits x numProj]
        ''' NDArray<T>* b    = INPUT_VARIABLE(7);                   // biases, [1 x 4*numUnits]
        ''' 
        ''' NDArray<T>* ht   =  OUTPUT_VARIABLE(0);                  // current cell output [batchSize x numProj], that is at current time step t
        ''' NDArray<T>* ct   =  OUTPUT_VARIABLE(1);                  // current cell state  [batchSize x numUnits], that is at current time step t
        ''' 
        ''' const bool peephole   = (bool)INT_ARG(0);               // if true, provide peephole connections
        ''' const bool projection = (bool)INT_ARG(1);               // if true, then projection is performed, if false then numProj==numUnits is mandatory!!!!
        ''' T clippingCellValue   = T_ARG(0);                       // clipping value for ct, if it is not equal to zero, then cell state is clipped
        ''' T clippingProjValue   = T_ARG(1);                       // clipping value for projected ht, if it is not equal to zero, then projected cell output is clipped
        ''' const T forgetBias    = T_ARG(2);
        ''' 
        ''' </summary>

        Private peepHole As Boolean
        Private projection As Boolean
        Private clippingCellValue As Double
        Private clippingProjValue As Double
        Private forgetBias As Double
#If SAMEDIFF Then
		'input variables
		Private xt, ht_1, ct_1, Wx, Wh, Wc, Wp, b As SDVariable
#End If

        Public Overridable Function toProperties() As IDictionary(Of String, Object)
            Dim ret As IDictionary(Of String, Object) = New JavaCompat.LinkedHashMap(Of String, Object)()
            ret("peepHole") = peepHole
            ret("projection") = projection
            ret("clippingCellValue") = clippingCellValue
            ret("clippingProjValue") = clippingProjValue
            ret("forgetBias") = forgetBias
            Return ret
        End Function

#If SAMEDIFF Then
		Public Overridable Function args() As SDVariable()
			Return New SDVariable() {xt, ht_1, ct_1, Wx, Wh, Wc, Wp, b}
		End Function
#End If

        Public Overridable Function iArgs() As Integer()
            Return New Integer() {ArrayUtil.fromBoolean(peepHole), ArrayUtil.fromBoolean(projection)}
        End Function

        Public Overridable Function tArgs() As Double()
            Return New Double() {clippingCellValue, clippingProjValue, forgetBias}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>peepHole</c>.</summary>
        Public Overridable Function isPeepHole() As Boolean
            Return peepHole
        End Function

        ''' <summary>Lombok setter of <c>peepHole</c>.</summary>
        Public Overridable Sub setPeepHole(ByVal value As Boolean)
            Me.peepHole = value
        End Sub

        ''' <summary>Lombok getter of <c>projection</c>.</summary>
        Public Overridable Function isProjection() As Boolean
            Return projection
        End Function

        ''' <summary>Lombok setter of <c>projection</c>.</summary>
        Public Overridable Sub setProjection(ByVal value As Boolean)
            Me.projection = value
        End Sub

        ''' <summary>Lombok getter of <c>clippingCellValue</c>.</summary>
        Public Overridable Function getClippingCellValue() As Double
            Return clippingCellValue
        End Function

        ''' <summary>Lombok setter of <c>clippingCellValue</c>.</summary>
        Public Overridable Sub setClippingCellValue(ByVal value As Double)
            Me.clippingCellValue = value
        End Sub

        ''' <summary>Lombok getter of <c>clippingProjValue</c>.</summary>
        Public Overridable Function getClippingProjValue() As Double
            Return clippingProjValue
        End Function

        ''' <summary>Lombok setter of <c>clippingProjValue</c>.</summary>
        Public Overridable Sub setClippingProjValue(ByVal value As Double)
            Me.clippingProjValue = value
        End Sub

        ''' <summary>Lombok getter of <c>forgetBias</c>.</summary>
        Public Overridable Function getForgetBias() As Double
            Return forgetBias
        End Function

        ''' <summary>Lombok setter of <c>forgetBias</c>.</summary>
        Public Overridable Sub setForgetBias(ByVal value As Double)
            Me.forgetBias = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>xt</c>.</summary>
        Public Overridable Function getXt() As SDVariable
            Return xt
        End Function

        ''' <summary>Lombok setter of <c>xt</c>.</summary>
        Public Overridable Sub setXt(ByVal value As SDVariable)
            Me.xt = value
        End Sub

        ''' <summary>Lombok getter of <c>ht_1</c>.</summary>
        Public Overridable Function getHt_1() As SDVariable
            Return ht_1
        End Function

        ''' <summary>Lombok setter of <c>ht_1</c>.</summary>
        Public Overridable Sub setHt_1(ByVal value As SDVariable)
            Me.ht_1 = value
        End Sub

        ''' <summary>Lombok getter of <c>ct_1</c>.</summary>
        Public Overridable Function getCt_1() As SDVariable
            Return ct_1
        End Function

        ''' <summary>Lombok setter of <c>ct_1</c>.</summary>
        Public Overridable Sub setCt_1(ByVal value As SDVariable)
            Me.ct_1 = value
        End Sub

        ''' <summary>Lombok getter of <c>Wx</c>.</summary>
        Public Overridable Function getWx() As SDVariable
            Return Wx
        End Function

        ''' <summary>Lombok setter of <c>Wx</c>.</summary>
        Public Overridable Sub setWx(ByVal value As SDVariable)
            Me.Wx = value
        End Sub

        ''' <summary>Lombok getter of <c>Wh</c>.</summary>
        Public Overridable Function getWh() As SDVariable
            Return Wh
        End Function

        ''' <summary>Lombok setter of <c>Wh</c>.</summary>
        Public Overridable Sub setWh(ByVal value As SDVariable)
            Me.Wh = value
        End Sub

        ''' <summary>Lombok getter of <c>Wc</c>.</summary>
        Public Overridable Function getWc() As SDVariable
            Return Wc
        End Function

        ''' <summary>Lombok setter of <c>Wc</c>.</summary>
        Public Overridable Sub setWc(ByVal value As SDVariable)
            Me.Wc = value
        End Sub

        ''' <summary>Lombok getter of <c>Wp</c>.</summary>
        Public Overridable Function getWp() As SDVariable
            Return Wp
        End Function

        ''' <summary>Lombok setter of <c>Wp</c>.</summary>
        Public Overridable Sub setWp(ByVal value As SDVariable)
            Me.Wp = value
        End Sub

        ''' <summary>Lombok getter of <c>b</c>.</summary>
        Public Overridable Function getB() As SDVariable
            Return b
        End Function

        ''' <summary>Lombok setter of <c>b</c>.</summary>
        Public Overridable Sub setB(ByVal value As SDVariable)
            Me.b = value
        End Sub
#End If

        ''' <summary>Lombok equals over the fields.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As LSTMCellConfiguration = TryCast(o, LSTMCellConfiguration)
            If other Is Nothing Then
                Return False
            End If
            If Not Object.Equals(peepHole, other.peepHole) Then
                Return False
            End If
            If Not Object.Equals(projection, other.projection) Then
                Return False
            End If
            If Not Object.Equals(clippingCellValue, other.clippingCellValue) Then
                Return False
            End If
            If Not Object.Equals(clippingProjValue, other.clippingProjValue) Then
                Return False
            End If
            If Not Object.Equals(forgetBias, other.forgetBias) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(CObj(peepHole) Is Nothing, 43, peepHole.GetHashCode())
            result = result * 59 + If(CObj(projection) Is Nothing, 43, projection.GetHashCode())
            result = result * 59 + If(CObj(clippingCellValue) Is Nothing, 43, clippingCellValue.GetHashCode())
            result = result * 59 + If(CObj(clippingProjValue) Is Nothing, 43, clippingProjValue.GetHashCode())
            result = result * 59 + If(CObj(forgetBias) Is Nothing, 43, forgetBias.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "LSTMCellConfiguration(peepHole=" & Arrays.FormatElement(peepHole) & ", projection=" & Arrays.FormatElement(projection) & ", clippingCellValue=" & Arrays.FormatElement(clippingCellValue) & ", clippingProjValue=" & Arrays.FormatElement(clippingProjValue) & ", forgetBias=" & Arrays.FormatElement(forgetBias) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As LSTMCellConfigurationBuilder
            Return New LSTMCellConfigurationBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="LSTMCellConfiguration"/>; unset fields keep their defaults.</summary>
        Public Class LSTMCellConfigurationBuilder

            Private _peepHole As Boolean
            Private _projection As Boolean
            Private _clippingCellValue As Double
            Private _clippingProjValue As Double
            Private _forgetBias As Double
#If SAMEDIFF Then
            Private _xt As SDVariable
            Private _ht_1 As SDVariable
            Private _ct_1 As SDVariable
            Private _Wx As SDVariable
            Private _Wh As SDVariable
            Private _Wc As SDVariable
            Private _Wp As SDVariable
            Private _b As SDVariable
#End If

            Public Function peepHole(ByVal value As Boolean) As LSTMCellConfigurationBuilder
                _peepHole = value
                Return Me
            End Function

            Public Function projection(ByVal value As Boolean) As LSTMCellConfigurationBuilder
                _projection = value
                Return Me
            End Function

            Public Function clippingCellValue(ByVal value As Double) As LSTMCellConfigurationBuilder
                _clippingCellValue = value
                Return Me
            End Function

            Public Function clippingProjValue(ByVal value As Double) As LSTMCellConfigurationBuilder
                _clippingProjValue = value
                Return Me
            End Function

            Public Function forgetBias(ByVal value As Double) As LSTMCellConfigurationBuilder
                _forgetBias = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function xt(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _xt = value
                Return Me
            End Function

            Public Function ht_1(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _ht_1 = value
                Return Me
            End Function

            Public Function ct_1(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _ct_1 = value
                Return Me
            End Function

            Public Function Wx(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _Wx = value
                Return Me
            End Function

            Public Function Wh(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _Wh = value
                Return Me
            End Function

            Public Function Wc(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _Wc = value
                Return Me
            End Function

            Public Function Wp(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _Wp = value
                Return Me
            End Function

            Public Function b(ByVal value As SDVariable) As LSTMCellConfigurationBuilder
                _b = value
                Return Me
            End Function

#End If
            Public Function build() As LSTMCellConfiguration
                Dim result As New LSTMCellConfiguration()
                result.peepHole = _peepHole
                result.projection = _projection
                result.clippingCellValue = _clippingCellValue
                result.clippingProjValue = _clippingProjValue
                result.forgetBias = _forgetBias
#If SAMEDIFF Then
                result.xt = _xt
                result.ht_1 = _ht_1
                result.ct_1 = _ct_1
                result.Wx = _Wx
                result.Wh = _Wh
                result.Wc = _Wc
                result.Wp = _Wp
                result.b = _b
#End If
                Return result
            End Function

        End Class

    End Class

End Namespace
