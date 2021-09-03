Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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

Namespace org.nd4j.linalg.api.ops.impl.shape


	Public Class Linspace
		Inherits DynamicCustomOp

		Private dataType As DataType
		Private start As Double
		Private [stop] As Double
		Private elements As Long

		Public Sub New(ByVal sameDiff As SameDiff, ByVal dataType As DataType, ByVal start As Double, ByVal [stop] As Double, ByVal number As Long)
			Me.New(sameDiff, sameDiff.constant(start), sameDiff.constant([stop]), sameDiff.constant(number), dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal from As SDVariable, ByVal [to] As SDVariable, ByVal length As SDVariable, ByVal dataType As DataType)
			MyBase.New(sameDiff, New SDVariable(){from, [to], length})
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal start As Double, ByVal [stop] As Double, ByVal number As Long)
			Me.New(start, [stop], number, dataType)
		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal start As INDArray, ByVal [stop] As INDArray, ByVal number As INDArray)
			Me.New(start, [stop], number, dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Linspace(@NonNull INDArray start, @NonNull INDArray stop, @NonNull INDArray number, @NonNull DataType dataType)
		Public Sub New(ByVal start As INDArray, ByVal [stop] As INDArray, ByVal number As INDArray, ByVal dataType As DataType)
			MyBase.New(New INDArray(){start, [stop], number}, Nothing)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Linspace(double start, double stop, long number, @NonNull DataType dataType)
		Public Sub New(ByVal start As Double, ByVal [stop] As Double, ByVal number As Long, ByVal dataType As DataType)
			MyBase.New(New INDArray(){}, Nothing)
			Me.dataType = dataType
			addDArgument(dataType)

			Me.start = start
			Me.stop = [stop]
			Me.elements = number

			addTArgument(Me.start, Me.stop)
			addIArgument(elements)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "lin_space"
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Return Collections.singletonList(dataType)
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "LinSpace"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			dataType = TFGraphMapper.convertType(attributesForNode("T").getType())
		End Sub

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg(0)), sameDiff.zerosLike(arg(1)), sameDiff.zerosLike(arg(2))}
		End Function
	End Class

End Namespace