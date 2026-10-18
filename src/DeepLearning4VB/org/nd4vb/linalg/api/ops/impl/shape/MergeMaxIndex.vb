Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>mergemaxindex</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.MergeMaxIndex</c>).</summary>
    Public Class MergeMaxIndex
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private dataTypeField As DataType = DataType.INT32

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull SameDiff sameDiff, @NonNull SDVariable... inputs)
		Public Sub New(ByVal sameDiff As SameDiff, ParamArray ByVal inputs() As SDVariable)
			MyBase.New("mergemaxindex", sameDiff, inputs)
			addIArgument(dataType.toInt())

		End Sub
#End If

        Public Sub New(ParamArray ByVal inputs() As INDArray)
            MyBase.New("mergemaxindex", inputs, Nothing)
            Preconditions.checkArgument(areEqualShapes(inputs), "All inputs have to be equal shapes")
            addIArgument(dataTypeField.toInt())

        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeMaxIndex(@NonNull SameDiff sd, @NonNull SDVariable[] x, @NonNull DataType dataType)
		Public Sub New(ByVal sd As SameDiff, ByVal x() As SDVariable, ByVal dataType As DataType)
			MyBase.New("mergemaxindex", sd, x)
			Me.dataType = dataType
			addIArgument(dataType.toInt())
		End Sub
#End If

        Public Sub New(ByVal x() As INDArray, ByVal dataType As DataType)
            MyBase.New(x, Nothing)
            Preconditions.checkArgument(areEqualShapes(x), "All inputs have to be equal shapes")
            Me.dataTypeField = dataType
            addIArgument(dataType.toInt())

        End Sub

        Protected Friend Shared Function areEqualShapes(ParamArray ByVal inputs() As INDArray) As Boolean
            For Each input As INDArray In inputs
                If Not inputs(0).equalShapes(input) Then
                    Return False
                End If
            Next input
            Return True
        End Function

        Public Overrides Function opName() As String
            Return "mergemaxindex"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Return New List(Of DataType) From {Me.dataTypeField}
        End Function

    End Class

End Namespace
