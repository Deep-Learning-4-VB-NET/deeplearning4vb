Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.GRUWeights</c>.</summary>
    Public Class GRUWeights
        Inherits RNNWeights

#If SAMEDIFF Then
		''' <summary>
		''' Reset and Update gate weights, with a shape of [inSize + numUnits, 2*numUnits].
		''' 
		''' The reset weights are the [:, 0:numUnits] subset and the update weights are the [:, numUnits:2*numUnits] subset.
		''' </summary>
		Private ruWeight As SDVariable
#End If

        Private iRuWeights As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Cell gate weights, with a shape of [inSize + numUnits, numUnits]
		''' </summary>
		Private cWeight As SDVariable
#End If

        Private iCWeight As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Reset and Update gate bias, with a shape of [2*numUnits].  May be null.
		''' 
		''' The reset bias is the [0:numUnits] subset and the update bias is the [numUnits:2*numUnits] subset.
		''' </summary>
		Private ruBias As SDVariable
#End If

        Private iRUBias As INDArray

#If SAMEDIFF Then
		''' <summary>
		''' Cell gate bias, with a shape of [numUnits].  May be null.
		''' </summary>
		Private cBias As SDVariable
#End If

        Private iCBias As INDArray

#If SAMEDIFF Then
		Public Overrides Function args() As SDVariable()
			Return filterNonNull(ruWeight, cWeight, ruBias, cBias)
		End Function
#End If

        Public Overrides Function arrayArgs() As INDArray()
            Return filterNonNull(iRuWeights, iCWeight, iRUBias, iCBias)
        End Function

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>ruWeight</c>.</summary>
        Public Overridable Function getRuWeight() As SDVariable
            Return ruWeight
        End Function

        ''' <summary>Lombok setter of <c>ruWeight</c>.</summary>
        Public Overridable Sub setRuWeight(ByVal value As SDVariable)
            Me.ruWeight = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iRuWeights</c>.</summary>
        Public Overridable Function getIRuWeights() As INDArray
            Return iRuWeights
        End Function

        ''' <summary>Lombok setter of <c>iRuWeights</c>.</summary>
        Public Overridable Sub setIRuWeights(ByVal value As INDArray)
            Me.iRuWeights = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>cWeight</c>.</summary>
        Public Overridable Function getCWeight() As SDVariable
            Return cWeight
        End Function

        ''' <summary>Lombok setter of <c>cWeight</c>.</summary>
        Public Overridable Sub setCWeight(ByVal value As SDVariable)
            Me.cWeight = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iCWeight</c>.</summary>
        Public Overridable Function getICWeight() As INDArray
            Return iCWeight
        End Function

        ''' <summary>Lombok setter of <c>iCWeight</c>.</summary>
        Public Overridable Sub setICWeight(ByVal value As INDArray)
            Me.iCWeight = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>ruBias</c>.</summary>
        Public Overridable Function getRuBias() As SDVariable
            Return ruBias
        End Function

        ''' <summary>Lombok setter of <c>ruBias</c>.</summary>
        Public Overridable Sub setRuBias(ByVal value As SDVariable)
            Me.ruBias = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iRUBias</c>.</summary>
        Public Overridable Function getIRUBias() As INDArray
            Return iRUBias
        End Function

        ''' <summary>Lombok setter of <c>iRUBias</c>.</summary>
        Public Overridable Sub setIRUBias(ByVal value As INDArray)
            Me.iRUBias = value
        End Sub

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>cBias</c>.</summary>
        Public Overridable Function getCBias() As SDVariable
            Return cBias
        End Function

        ''' <summary>Lombok setter of <c>cBias</c>.</summary>
        Public Overridable Sub setCBias(ByVal value As SDVariable)
            Me.cBias = value
        End Sub
#End If

        ''' <summary>Lombok getter of <c>iCBias</c>.</summary>
        Public Overridable Function getICBias() As INDArray
            Return iCBias
        End Function

        ''' <summary>Lombok setter of <c>iCBias</c>.</summary>
        Public Overridable Sub setICBias(ByVal value As INDArray)
            Me.iCBias = value
        End Sub

        ''' <summary>Lombok equals over the fields and the base class.</summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As GRUWeights = TryCast(o, GRUWeights)
            If other Is Nothing Then
                Return False
            End If
            If Not MyBase.Equals(o) Then
                Return False
            End If
            If Not Object.Equals(iRuWeights, other.iRuWeights) Then
                Return False
            End If
            If Not Object.Equals(iCWeight, other.iCWeight) Then
                Return False
            End If
            If Not Object.Equals(iRUBias, other.iRUBias) Then
                Return False
            End If
            If Not Object.Equals(iCBias, other.iCBias) Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>Lombok hashCode over the fields.</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = MyBase.GetHashCode()
            result = result * 59 + If(CObj(iRuWeights) Is Nothing, 43, iRuWeights.GetHashCode())
            result = result * 59 + If(CObj(iCWeight) Is Nothing, 43, iCWeight.GetHashCode())
            result = result * 59 + If(CObj(iRUBias) Is Nothing, 43, iRUBias.GetHashCode())
            result = result * 59 + If(CObj(iCBias) Is Nothing, 43, iCBias.GetHashCode())
            Return result
        End Function

        ''' <summary>Lombok toString over the fields (fields inside a fence are left out).</summary>
        Public Overrides Function ToString() As String
            Return "GRUWeights(iRuWeights=" & Arrays.FormatElement(iRuWeights) & ", iCWeight=" & Arrays.FormatElement(iCWeight) & ", iRUBias=" & Arrays.FormatElement(iRUBias) & ", iCBias=" & Arrays.FormatElement(iCBias) & ")"
        End Function

        ''' <summary>Lombok's builder.</summary>
        Public Shared Function builder() As GRUWeightsBuilder
            Return New GRUWeightsBuilder()
        End Function

        ''' <summary>Lombok's builder of <see cref="GRUWeights"/>; unset fields keep their defaults.</summary>
        Public Class GRUWeightsBuilder

#If SAMEDIFF Then
            Private _ruWeight As SDVariable
#End If
            Private _iRuWeights As INDArray
#If SAMEDIFF Then
            Private _cWeight As SDVariable
#End If
            Private _iCWeight As INDArray
#If SAMEDIFF Then
            Private _ruBias As SDVariable
#End If
            Private _iRUBias As INDArray
#If SAMEDIFF Then
            Private _cBias As SDVariable
#End If
            Private _iCBias As INDArray

#If SAMEDIFF Then
            Public Function ruWeight(ByVal value As SDVariable) As GRUWeightsBuilder
                _ruWeight = value
                Return Me
            End Function

#End If
            Public Function iRuWeights(ByVal value As INDArray) As GRUWeightsBuilder
                _iRuWeights = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function cWeight(ByVal value As SDVariable) As GRUWeightsBuilder
                _cWeight = value
                Return Me
            End Function

#End If
            Public Function iCWeight(ByVal value As INDArray) As GRUWeightsBuilder
                _iCWeight = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function ruBias(ByVal value As SDVariable) As GRUWeightsBuilder
                _ruBias = value
                Return Me
            End Function

#End If
            Public Function iRUBias(ByVal value As INDArray) As GRUWeightsBuilder
                _iRUBias = value
                Return Me
            End Function

#If SAMEDIFF Then
            Public Function cBias(ByVal value As SDVariable) As GRUWeightsBuilder
                _cBias = value
                Return Me
            End Function

#End If
            Public Function iCBias(ByVal value As INDArray) As GRUWeightsBuilder
                _iCBias = value
                Return Me
            End Function

            Public Function build() As GRUWeights
                Dim result As New GRUWeights()
#If SAMEDIFF Then
                result.ruWeight = _ruWeight
#End If
                result.iRuWeights = _iRuWeights
#If SAMEDIFF Then
                result.cWeight = _cWeight
#End If
                result.iCWeight = _iCWeight
#If SAMEDIFF Then
                result.ruBias = _ruBias
#End If
                result.iRUBias = _iRUBias
#If SAMEDIFF Then
                result.cBias = _cBias
#End If
                result.iCBias = _iCBias
                Return result
            End Function

        End Class

    End Class

End Namespace
