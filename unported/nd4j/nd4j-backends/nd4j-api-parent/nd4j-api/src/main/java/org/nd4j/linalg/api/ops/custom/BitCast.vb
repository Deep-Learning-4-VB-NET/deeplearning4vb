Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
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


	Public Class BitCast
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Private dtype As DataType

		Public Sub New(ByVal [in] As INDArray, ByVal dataType As DataType, ByVal [out] As INDArray)
			Me.New([in], dataType.toInt(), [out])
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dataType As Integer, ByVal [out] As INDArray)
			inputArguments_Conflict.Add([in])
			outputArguments_Conflict.Add([out])
			iArguments.Add(Convert.ToInt64(dataType))

			dtype = DataType.fromInt(dataType)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dataType As DataType)
			Me.New([in], dataType.toInt())
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dataType As Integer)
			inputArguments_Conflict.Add([in])
			iArguments.Add(Convert.ToInt64(dataType))
			dtype = DataType.fromInt(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal dataType As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){[in], dataType})
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			Dim t As val = nodeDef.getAttrOrDefault("type", Nothing)
			Dim type As val = ArrayOptionsHelper.convertToDataType(t.getType())
			addIArgument(type.toInt())

			dtype = type
		End Sub

		Public Overrides Function opName() As String
			Return "bitcast"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Bitcast"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			If dtype = Nothing Then
				If iArguments.Count > 0 Then
					dtype = DataType.fromInt(iArguments(0).intValue())
				End If
			End If
			Return Collections.singletonList(dtype)
		End Function
	End Class
End Namespace