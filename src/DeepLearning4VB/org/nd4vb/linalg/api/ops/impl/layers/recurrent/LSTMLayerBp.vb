Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LSTMLayerConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports LSTMLayerWeights = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>lstmLayer_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayerBp</c>).</summary>
    Public Class LSTMLayerBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private configuration As LSTMLayerConfig

        Private weights As LSTMLayerWeights

#If SAMEDIFF Then
		Private cLast As SDVariable
		Private yLast As SDVariable
		Private maxTSLength As SDVariable

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LSTMLayerBp(@NonNull SameDiff sameDiff, @NonNull SDVariable x, org.nd4j.autodiff.samediff.SDVariable cLast, org.nd4j.autodiff.samediff.SDVariable yLast, org.nd4j.autodiff.samediff.SDVariable maxTSLength, @NonNull LSTMLayerWeights weights, @NonNull LSTMLayerConfig configuration, org.nd4j.autodiff.samediff.SDVariable dLdh, org.nd4j.autodiff.samediff.SDVariable dLdhL, org.nd4j.autodiff.samediff.SDVariable dLdcL)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal maxTSLength As SDVariable, ByVal weights As LSTMLayerWeights, ByVal configuration As LSTMLayerConfig, ByVal dLdh As SDVariable, ByVal dLdhL As SDVariable, ByVal dLdcL As SDVariable)
			MyBase.New("lstmLayer_bp", sameDiff, wrapFilterNull(x, weights.getWeights(), weights.getRWeights(), weights.getBias(), maxTSLength, yLast, cLast, weights.getPeepholeWeights(), dLdh, dLdhL, dLdcL))
			Me.configuration = configuration
			Me.weights = weights
			Me.cLast = cLast
			Me.yLast = yLast
			Me.maxTSLength = maxTSLength
			addIArgument(iArgs())
			addTArgument(tArgs())
			addBArgument(bArgs(weights, maxTSLength, yLast, cLast))

			Preconditions.checkState(Me.configuration.isRetLastH() OrElse Me.configuration.isRetLastC() OrElse Me.configuration.isRetFullSequence(), "You have to specify at least one output you want to return. Use isRetLastC, isRetLast and isRetFullSequence  methods  in LSTMLayerConfig builder to specify them")

		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)

            Dim dt As DataType = inputDataTypes(1)
            Preconditions.checkState(dt.isFPType(), "Input type 1 must be a floating point type, got %s", dt)
            Dim list As New List(Of DataType)()
            list.Add(dt) ' dLdx
            list.Add(dt) ' dLdWx
            list.Add(dt) ' dLdWr

            If Me.weights.hasBias() Then
                list.Add(dt)
            End If ' dLdb

#If SAMEDIFF Then
            If Me.maxTSLength IsNot Nothing Then
                list.Add(dt)
            End If ' dLdSl
            If Me.yLast IsNot Nothing Then
                list.Add(dt)
            End If 'dLdhI
            If Me.cLast IsNot Nothing Then
                list.Add(dt)
            End If ' dLdcI
#End If
            If Me.weights.hasPH() Then
                list.Add(dt)
            End If ' dLdWp

            Return list
        End Function

        Public Overrides Function opName() As String
            Return "lstmLayer_bp"
        End Function

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Return configuration.toProperties(True, True)
        End Function

        Public Overrides Function iArgs() As Long()
            Return New Long(){ CInt(configuration.getLstmdataformat()), CInt(configuration.getDirectionMode()), CInt(configuration.getGateAct()), CInt(configuration.getOutAct()), CInt(configuration.getCellAct()) }
        End Function

        Public Overrides Function tArgs() As Double()
            Return New Double(){Me.configuration.getCellClip()} ' T_ARG(0)
        End Function

        Protected Friend Overridable Overloads Function bArgs(Of T)(ByVal weights As LSTMLayerWeights, ByVal maxTSLength As T, ByVal yLast As T, ByVal cLast As T) As Boolean()
            Return New Boolean(){ weights.hasBias(), maxTSLength IsNot Nothing, yLast IsNot Nothing, cLast IsNot Nothing, weights.hasPH(), configuration.isRetFullSequence(), configuration.isRetLastH(), configuration.isRetLastC() }

        End Function

        Public Overrides ReadOnly Property ConfigProperties As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Function configFieldName() As String
            Return "configuration"
        End Function

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
    
#If SAMEDIFF Then
                Return {True, True, True, weights.hasBias(), Me.maxTSLength IsNot Nothing, Me.yLast IsNot Nothing, Me.cLast IsNot Nothing, weights.hasPH()}.Count(Function(b) b)
#Else
                Return {True, True, True, weights.hasBias(), weights.hasPH()}.Count(Function(b) b)
#End If
            End Get
        End Property

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>configuration</c>.</summary>
        Public Overridable Function getConfiguration() As LSTMLayerConfig
            Return configuration
        End Function

        ''' <summary>Lombok getter of <c>weights</c>.</summary>
        Public Overridable Function getWeights() As LSTMLayerWeights
            Return weights
        End Function

    End Class

End Namespace
