Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>bitcast</c> (Java <c>org.nd4j.linalg.api.ops.custom.BitCast</c>).</summary>
    Public Class BitCast
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private dtype As DataType

        Public Sub New(ByVal [in] As INDArray, ByVal dataType As DataType, ByVal [out] As INDArray)
            Me.New([in], dataType.toInt(), [out])
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal dataType As Integer, ByVal [out] As INDArray)
            inputArgumentsField.Add([in])
            outputArgumentsField.Add([out])
            iArguments.Add(Convert.ToInt64(dataType))

            dtype = DataTypeExtensions.DataTypeFromInt(dataType)
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal dataType As DataType)
            Me.New([in], dataType.toInt())
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal dataType As Integer)
            inputArgumentsField.Add([in])
            iArguments.Add(Convert.ToInt64(dataType))
            dtype = DataTypeExtensions.DataTypeFromInt(dataType)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal dataType As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){[in], dataType})
		End Sub
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			Dim t As val = nodeDef.getAttrOrDefault("type", Nothing)
			Dim type As val = ArrayOptionsHelper.convertToDataType(t.getType())
			addIArgument(type.toInt())

			dtype = type
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "bitcast"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Bitcast"
        End Function

#If SAMEDIFF Then
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
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
