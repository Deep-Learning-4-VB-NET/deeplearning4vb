Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.api.ops.impl.image


	Public Class ExtractImagePatches
		Inherits DynamicCustomOp

		Private kSizes() As Integer
		Private strides() As Integer
		Private rates() As Integer
		Private isSameMode As Boolean

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExtractImagePatches(@NonNull SameDiff samediff, @NonNull SDVariable input, int kH, int kW, int sH, int sW, int rH, int rW, boolean sameMode)
		Public Sub New(ByVal samediff As SameDiff, ByVal input As SDVariable, ByVal kH As Integer, ByVal kW As Integer, ByVal sH As Integer, ByVal sW As Integer, ByVal rH As Integer, ByVal rW As Integer, ByVal sameMode As Boolean)
			Me.New(samediff, input, New Integer(){kH, kW}, New Integer(){sH, sW}, New Integer(){rH, rW}, sameMode)

		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExtractImagePatches(@NonNull SameDiff samediff, @NonNull SDVariable input, @NonNull int[] kSizes, @NonNull int[] strides, @NonNull int[] rates, boolean sameMode)
		Public Sub New(ByVal samediff As SameDiff, ByVal input As SDVariable, ByVal kSizes() As Integer, ByVal strides() As Integer, ByVal rates() As Integer, ByVal sameMode As Boolean)
			MyBase.New(samediff, input)
			Preconditions.checkState(kSizes.Length = 2, "Expected exactly 2 kernel sizes, got %s", kSizes)
			Preconditions.checkState(strides.Length = 2, "Expected exactly 2 strides, got %s", strides)
			Preconditions.checkState(rates.Length = 2, "Expected exactly 2 rate values, got %s", rates)
			Me.kSizes = kSizes
			Me.strides = strides
			Me.rates = rates
			Me.isSameMode = sameMode
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExtractImagePatches(@NonNull INDArray input, @NonNull int[] kSizes, @NonNull int[] strides, @NonNull int[] rates, boolean sameMode)
		Public Sub New(ByVal input As INDArray, ByVal kSizes() As Integer, ByVal strides() As Integer, ByVal rates() As Integer, ByVal sameMode As Boolean)
			MyBase.New(New INDArray(){input}, Nothing)
			Preconditions.checkState(kSizes.Length = 2, "Expected exactly 2 kernel sizes, got %s", kSizes)
			Preconditions.checkState(strides.Length = 2, "Expected exactly 2 strides, got %s", strides)
			Preconditions.checkState(rates.Length = 2, "Expected exactly 2 rate values, got %s", rates)
			Me.kSizes = kSizes
			Me.strides = strides
			Me.rates = rates
			Me.isSameMode = sameMode
			addArgs()
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal kH As Integer, ByVal kW As Integer, ByVal sH As Integer, ByVal sW As Integer, ByVal rH As Integer, ByVal rW As Integer, ByVal sameMode As Boolean)
			Me.New(input, New Integer(){kH, kW}, New Integer(){sH, sW}, New Integer(){rH, rW}, sameMode)
		End Sub


		Public Overrides Function opName() As String
			Return "extract_image_patches"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ExtractImagePatches"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'TF includes redundant leading and training 1s for kSizes, strides, rates (positions 0/3)
			kSizes = parseIntList(attributesForNode("ksizes").getList())
			strides = parseIntList(attributesForNode("strides").getList())
			rates = parseIntList(attributesForNode("rates").getList())
			Dim s As String = attributesForNode("padding").getS().toStringUtf8()
			isSameMode = s.Equals("SAME", StringComparison.OrdinalIgnoreCase)
			addArgs()
		End Sub

		Protected Friend Overridable Sub addArgs()
			iArguments.Clear()
			addIArgument(kSizes)
			addIArgument(strides)
			addIArgument(rates)
			addIArgument(If(isSameMode, 1, 0))
			addIArgument()
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException()
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property

		Private Shared Function parseIntList(ByVal ilist As AttrValue.ListValue) As Integer()
			'TF includes redundant leading and training 1s for kSizes, strides, rates (positions 0/3)
			Return New Integer(){CInt(Math.Truncate(ilist.getI(1))), CInt(Math.Truncate(ilist.getI(2)))}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected exactly 1 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace