Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class FusedBatchNorm
		Inherits DynamicCustomOp

		Private outputDataType As DataType

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FusedBatchNorm(@NonNull INDArray x, @NonNull INDArray scale, @NonNull INDArray offset, int dataFormat, int isTraining, org.nd4j.linalg.api.ndarray.INDArray yOut, org.nd4j.linalg.api.ndarray.INDArray batchMeanOut, org.nd4j.linalg.api.ndarray.INDArray batchMeanVar)
		Public Sub New(ByVal x As INDArray, ByVal scale As INDArray, ByVal offset As INDArray, ByVal dataFormat As Integer, ByVal isTraining As Integer, ByVal yOut As INDArray, ByVal batchMeanOut As INDArray, ByVal batchMeanVar As INDArray)
			addInputArgument(x, scale, offset)
			addIArgument(dataFormat, isTraining)
			If yOut IsNot Nothing AndAlso batchMeanOut IsNot Nothing AndAlso batchMeanVar IsNot Nothing Then
				addOutputArgument(yOut, batchMeanOut, batchMeanVar)
			End If
			Me.outputDataType = x.dataType()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FusedBatchNorm(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable scale, @NonNull SDVariable offset, @NonNull SDVariable dataFormat, @NonNull SDVariable isTraining)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal scale As SDVariable, ByVal offset As SDVariable, ByVal dataFormat As SDVariable, ByVal isTraining As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){x, scale, offset, dataFormat, isTraining})
			Me.outputDataType = x.dataType()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FusedBatchNorm(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable scale, @NonNull SDVariable offset, int dataFormat, int isTraining)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal scale As SDVariable, ByVal offset As SDVariable, ByVal dataFormat As Integer, ByVal isTraining As Integer)
			MyBase.New("", sameDiff, New SDVariable(){x, scale, offset})
			addIArgument(dataFormat, isTraining)
			Me.outputDataType = x.dataType()
		End Sub

		Public Overrides Function opName() As String
			Return "fused_batch_norm"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"FusedBatchNormV2", "FusedBatchNormV3"}
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim isNchw As Boolean = attributesForNode.ContainsKey("data_format") AndAlso attributesForNode("data_format").getS().toStringUtf8().equalsIgnoreCase("NCHW")
			Dim training As Boolean = If(Not attributesForNode.ContainsKey("is_training"), True, attributesForNode("is_training").getB())
			addIArgument(If(isNchw, 1, 0))
			addIArgument(If(training, 1, 0))
			If attributesForNode.ContainsKey("T") Then
				outputDataType = TFGraphMapper.convertType(attributesForNode("T").getType())
			End If
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			If dArguments.Count > 0 Then
				Return New List(Of DataType) From {dArguments(0),dArguments(0),dArguments(0)}
			End If
			Return New List(Of DataType) From {If(outputDataType = Nothing, DataType.FLOAT, outputDataType),If(outputDataType = Nothing, DataType.FLOAT, outputDataType),If(outputDataType = Nothing, DataType.FLOAT, outputDataType)}
		End Function
	End Class

End Namespace