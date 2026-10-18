Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>layer_norm</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm</c>).</summary>
    Public Class LayerNorm
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private noBias As Boolean = False
        Private channelsFirst As Boolean

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LayerNorm(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable gain, org.nd4j.autodiff.samediff.SDVariable bias, boolean channelsFirst, int... dimensions)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal gain As SDVariable, ByVal bias As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(Nothing, sameDiff, wrapFilterNull(input, gain, bias), False)
			Me.noBias = bias Is Nothing
			Me.channelsFirst = channelsFirst
			Me.Dimensions = dimensions
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal gain As SDVariable, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(sameDiff, input, gain, Nothing, channelsFirst, dimensions)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal gain As INDArray, ByVal bias As INDArray, ByVal result As INDArray, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer)
            MyBase.New("layer_norm", wrapFilterNull(input, gain, bias), wrapOrNull(result))
            Me.noBias = bias Is Nothing
            Me.channelsFirst = channelsFirst
            setDimensions(dimensions)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal gain As INDArray, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(input, gain, CType(Nothing, INDArray), channelsFirst, dimensions)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal gain As INDArray, ByVal result As INDArray, ByVal channelsFirst As Boolean, ParamArray ByVal dimensions() As Integer)
            Me.New(input, gain, CType(Nothing, INDArray), result, channelsFirst, dimensions)
        End Sub

        Public Overrides Sub setDimensions(ParamArray ByVal dimensions() As Integer)
            Preconditions.checkArgument(dimensions IsNot Nothing, "LayerNorm: You have to provide dimensions")
            Preconditions.checkArgument(dimensions.Length > 0, "LayerNorm: You have to provide dimensions")

            Me.dimensionsField = dimensions
            Me.iArguments.Clear()
            addIArgument(dimensions)
            Me.bArguments.Clear()
            Me.bArguments.Add(channelsFirst)
        End Sub

        Public Overrides Function opName() As String
            Return "layer_norm"
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow name found for shape " & opName())
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			If noBias Then
				Return (New LayerNormBp(sameDiff, arg(0), arg(1), gradient(0), channelsFirst, dimensions)).outputs()
			Else
				Return (New LayerNormBp(sameDiff, arg(0), arg(1), arg(2), gradient(0), channelsFirst, dimensions)).outputs()
			End If
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count >= 2 AndAlso dataTypes.Count <= 3, "Expected exactly 2 or 3 input datatypes, got %s", dataTypes)
            Dim first As DataType = dataTypes(0)
            For Each dataType As DataType In dataTypes
                Preconditions.checkState(dataType.isFPType(), "Input %s datatype must be a floating point type, got datypes %s", dataTypes)
                Preconditions.checkState(first = dataType, "All datatypes must be same type, got input datatypes %s", dataTypes)
            Next dataType

            Return New List(Of DataType) From {first}
        End Function

        Public Overrides Function numOutputArguments() As Integer
            Return If(noBias, 2, 3)
        End Function

    End Class

End Namespace
