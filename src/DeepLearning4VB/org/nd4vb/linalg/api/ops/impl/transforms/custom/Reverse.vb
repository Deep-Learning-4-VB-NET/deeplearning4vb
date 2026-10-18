Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

    ''' <summary>The custom op <c>reverse</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse</c>).</summary>
    Public Class Reverse
        Inherits DynamicCustomOp

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Reverse(@NonNull SameDiff sameDiff, @NonNull SDVariable i_v, @NonNull int... dimensions)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, New SDVariable(){i_v})
			Me.dimensions = dimensions
			addIArgument(dimensions)
		End Sub
#End If

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Inplace reverse.  See <seealso cref="Reverse(INDArray, INDArray)"/>
        ''' </summary>
        Public Sub New(ByVal x As INDArray)
            Me.New(x, x)
            Me.inPlace = True
        End Sub

        ''' <summary>
        ''' This constructor allows to specify axis for Reverse operation </summary>
        ''' <param name="x"> </param>
        ''' <param name="axis"> </param>
        Public Sub New(ByVal x As INDArray, ParamArray ByVal axis() As Integer)
            MyBase.New(New INDArray(){x}, New INDArray(){})
            Me.inPlace = False
            Me.dimensionsField = axis
            addIArgument(axis)
        End Sub

        ''' <summary>
        ''' This constructor allows to specify axis for Reverse operation </summary>
        ''' <param name="x"> </param>
        ''' <param name="axis"> </param>
        Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal axis() As Integer)
            MyBase.New(New INDArray(){x}, New INDArray() {z})
            Me.inPlace = False
            Me.dimensionsField = axis
            addIArgument(axis)
        End Sub

        ''' <summary>
        ''' Reverses whole array for compatibility with OldReverse.
        ''' 
        ''' Note that otherwise, passing null or empty dimensions will result in a noop.
        ''' </summary>
        Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
            MyBase.New(New INDArray(){x}, New INDArray(){z})
            Me.dimensionsField = New Integer(x.rank() - 1){}
            For i As Integer = 0 To Me.dimensionsField.Length - 1
                Me.dimensionsField(i) = i
            Next i
            addIArgument(dimensionsField)
        End Sub

        Public Overrides Function opName() As String
            Return "reverse"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Reverse"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New ReverseBp(sameDiff, arg(0), f1(0), dimensions)).outputs()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 1 OrElse dataTypes.Count = 2), "Expected 1 so 2 input datatype for %s, got %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
