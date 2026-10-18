Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports DataTypeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.adapters.DataTypeAdapter
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
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

Namespace org.nd4vb.linalg.api.ops.random.custom

    ''' <summary>The custom op <c>random_poisson</c> (Java <c>org.nd4j.linalg.api.ops.random.custom.RandomPoisson</c>).</summary>
    Public Class RandomPoisson
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private outputDataType As DataType = DataType.FLOAT

        Public Sub New(ByVal shape As INDArray, ByVal rate As INDArray, ParamArray ByVal seeds() As Integer)
            addInputArgument(shape, rate)
            addIArgument(seeds)
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal rate As INDArray)
            Me.New(shape, rate, 0,0)
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomPoisson(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable rate, int... seeds)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal rate As SDVariable, ParamArray ByVal seeds() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){shape, rate})
			addIArgument(seeds)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "random_poisson"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"RandomPoisson", "RandomPoissonV2"}
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
		   'TODO: change op descriptor to have proper data type matching java
			If attributesForNode.ContainsKey("dtype") Then
				outputDataType = DataTypeAdapter.dtypeConv(attributesForNode("dtype").getType())
			End If
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes.Count)

            If dArguments.Count > 0 Then
                Return New List(Of DataType) From {dArguments(0)}
            End If
            Return New List(Of DataType) From {outputDataType}
        End Function

    End Class

End Namespace
