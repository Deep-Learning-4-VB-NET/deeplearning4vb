Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.clip

    ''' <summary>The custom op <c>clipbyavgnorm</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.clip.ClipByAvgNorm</c>).</summary>
    Public Class ClipByAvgNorm
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private clipValue As Double

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New("clipbyavgnorm", sameDiff, New SDVariable(){x})
			Me.clipValue = clipValue
			Me.dimensions = dimensions
			addIArgument(dimensions)
			addTArgument(clipValue)
		End Sub
#End If

        Public Sub New(ByVal [in] As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
            Me.New([in], CType(Nothing, INDArray), clipValue, dimensions)
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal [out] As INDArray, ByVal clipValue As Double, ParamArray ByVal dimensions() As Integer)
            MyBase.New("clipbyavgnorm", New INDArray(){[in]}, wrapOrNull([out]), JavaCollections.singletonList(clipValue), dimensions)
        End Sub

        Public Overrides Function opName() As String
            Return "clipbyavgnorm"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not yet implemented")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
            Return inputDataTypes
        End Function

    End Class

End Namespace
