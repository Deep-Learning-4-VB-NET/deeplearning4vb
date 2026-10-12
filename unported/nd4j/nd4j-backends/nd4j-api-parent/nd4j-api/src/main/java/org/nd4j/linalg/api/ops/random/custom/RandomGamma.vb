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
'ORIGINAL LINE: @NoArgsConstructor public class RandomGamma extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class RandomGamma
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomGamma(@NonNull INDArray shape, @NonNull INDArray alpha, org.nd4j.linalg.api.ndarray.INDArray beta, int... seeds)
		Public Sub New(ByVal shape As INDArray, ByVal alpha As INDArray, ByVal beta As INDArray, ParamArray ByVal seeds() As Integer)
			If beta IsNot Nothing Then
				addInputArgument(shape,alpha,beta)
			End If
			addInputArgument(shape,alpha)
			addIArgument(seeds)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomGamma(@NonNull INDArray shape, @NonNull INDArray alpha, org.nd4j.linalg.api.ndarray.INDArray beta)
		Public Sub New(ByVal shape As INDArray, ByVal alpha As INDArray, ByVal beta As INDArray)

			Me.New(shape,alpha,beta,0,0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomGamma(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable alpha, org.nd4j.autodiff.samediff.SDVariable beta, int... seeds)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal alpha As SDVariable, ByVal beta As SDVariable, ParamArray ByVal seeds() As Integer)
			MyBase.New(Nothing, sameDiff,If(beta IsNot Nothing, New SDVariable(){shape, alpha, beta}, New SDVariable()){shape, alpha})
			addIArgument(seeds)
		End Sub

		Public Overrides Function opName() As String
			Return "random_gamma"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "RandomGamma"
		End Function

		Private outputDataType As DataType = DataType.FLOAT

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
				outputDataType = DataTypeAdapter.dtypeConv(attributesForNode("T").getType())
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing, "Expected exactly input datatypes for %s, got null", Me.GetType())
			Return Collections.singletonList(outputDataType)
		End Function
	End Class

End Namespace