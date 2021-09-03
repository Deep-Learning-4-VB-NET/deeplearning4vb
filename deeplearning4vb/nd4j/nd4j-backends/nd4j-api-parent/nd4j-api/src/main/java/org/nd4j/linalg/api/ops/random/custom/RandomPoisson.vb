Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataTypeAdapter = org.nd4j.imports.descriptors.properties.adapters.DataTypeAdapter
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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
Namespace org.nd4j.linalg.api.ops.random.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class RandomPoisson extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class RandomPoisson
		Inherits DynamicCustomOp

		Private outputDataType As DataType = DataType.FLOAT

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomPoisson(@NonNull INDArray shape, @NonNull INDArray rate, int... seeds)
		Public Sub New(ByVal shape As INDArray, ByVal rate As INDArray, ParamArray ByVal seeds() As Integer)
			addInputArgument(shape, rate)
			addIArgument(seeds)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomPoisson(@NonNull INDArray shape, @NonNull INDArray rate)
		Public Sub New(ByVal shape As INDArray, ByVal rate As INDArray)
			Me.New(shape, rate, 0,0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomPoisson(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable rate, int... seeds)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal rate As SDVariable, ParamArray ByVal seeds() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){shape, rate})
			addIArgument(seeds)
		End Sub

		Public Overrides Function opName() As String
			Return "random_poisson"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"RandomPoisson", "RandomPoissonV2"}
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
		   'TODO: change op descriptor to have proper data type matching java
			If attributesForNode.ContainsKey("dtype") Then
				outputDataType = DataTypeAdapter.dtypeConv(attributesForNode("dtype").getType())
			End If
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes.Count)

			If dArguments.Count > 0 Then
				Return New List(Of DataType) From {dArguments(0)}
			End If
			Return Collections.singletonList(outputDataType)
		End Function
	End Class

End Namespace