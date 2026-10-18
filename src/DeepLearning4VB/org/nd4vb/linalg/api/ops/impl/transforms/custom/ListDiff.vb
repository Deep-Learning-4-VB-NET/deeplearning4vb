Imports System
Imports System.Collections.Generic
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

    ''' <summary>The custom op <c>listdiff</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ListDiff</c>).</summary>
    Public Class ListDiff
        Inherits DynamicCustomOp

        Public Sub New()
            '
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ListDiff(@NonNull SameDiff sd, @NonNull SDVariable x, @NonNull SDVariable y)
		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sd, New SDVariable(){x, y})
		End Sub
#End If

        Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
            MyBase.New(New INDArray(){x, y}, Nothing)
        End Sub

        Public Overrides Function tensorflowName() As String
            Return "ListDiff" 'Note: Seems to be renamed to tf.setdiff1d in public API?
        End Function

        Public Overrides Function opName() As String
            Return "listdiff"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            'TODO make this configurable
            Return New List(Of DataType) From {inputDataTypes(0), DataType.INT}
        End Function

    End Class

End Namespace
