Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports PadMode = org.nd4j.enums.PadMode
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

Namespace org.nd4j.linalg.api.ops.impl.transforms


	Public Class Pad
		Inherits DynamicCustomOp

		Public Enum Mode
			CONSTANT
			REFLECT
			SYMMETRIC

		End Enum
		Private mode As Mode
		Private constant As Double

		Public Sub New()
		End Sub

		Private Shared Function adaptMode(ByVal mode As PadMode) As Mode
			Dim legacyMode As Mode = Mode.CONSTANT

			If mode = PadMode.CONSTANT Then
				legacyMode = Mode.CONSTANT
			ElseIf mode = PadMode.REFLECT Then
				legacyMode = Mode.REFLECT
			ElseIf mode = PadMode.SYMMETRIC Then
				legacyMode = Mode.SYMMETRIC
			End If
			Return legacyMode
		End Function

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal padding As SDVariable, ByVal mode As PadMode, ByVal padValue As Double)
			Me.New(sd, [in], padding, adaptMode(mode), padValue)
		End Sub
		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal padding As SDVariable, ByVal mode As Mode, ByVal padValue As Double)
			MyBase.New(sd, New SDVariable(){[in], padding}, False)
			Preconditions.checkState(padding.dataType().isIntType(), "Padding array must be an integer datatype, got %s", padding.dataType())
			Me.mode = mode
			addIArgument(mode.ordinal())
			addTArgument(padValue)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal padding As SDVariable, ByVal padValue As Double)
			Me.New(sd, [in], padding, Mode.CONSTANT, padValue)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pad(@NonNull INDArray in, @NonNull INDArray padding, double padValue)
		Public Sub New(ByVal [in] As INDArray, ByVal padding As INDArray, ByVal padValue As Double)
			Me.New([in], padding, Nothing, Mode.CONSTANT, padValue)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pad(@NonNull INDArray in, @NonNull INDArray padding, @NonNull PadMode mode, double padValue)
		Public Sub New(ByVal [in] As INDArray, ByVal padding As INDArray, ByVal mode As PadMode, ByVal padValue As Double)
			Me.New([in], padding, Nothing, adaptMode(mode), padValue)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pad(@NonNull INDArray in, @NonNull INDArray padding, org.nd4j.linalg.api.ndarray.INDArray out, @NonNull Mode mode, double padValue)
		Public Sub New(ByVal [in] As INDArray, ByVal padding As INDArray, ByVal [out] As INDArray, ByVal mode As Mode, ByVal padValue As Double)
			MyBase.New(Nothing, New INDArray(){[in], padding},If([out] Is Nothing, Nothing, New INDArray()){[out]})
			Preconditions.checkState(padding.dataType().isIntType(), "Padding array must be an integer datatype, got %s", padding.dataType())
			Me.mode = mode
			addIArgument(mode.ordinal())
			addTArgument(padValue)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Pad(@NonNull INDArray in, @NonNull INDArray padding, org.nd4j.linalg.api.ndarray.INDArray out, @NonNull PadMode mode, double padValue)
		Public Sub New(ByVal [in] As INDArray, ByVal padding As INDArray, ByVal [out] As INDArray, ByVal mode As PadMode, ByVal padValue As Double)
			Me.New([in], padding, [out], adaptMode(mode), padValue)
		End Sub

		Public Overrides Function opName() As String
			Return "pad"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Pad", "PadV2"}
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'Based on TF codebase: gen_array_ops.mirror_pad is osed for BOTH REFLECT and SYMMETRIC mode. Hence only constant being imported here
			Me.mode = Mode.CONSTANT
			addIArgument(mode.ordinal())
			'Constant value is resolved just before execution
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Pad backprop: it's basically slice op...
			'Inputs to pad: input array (rank N), and padding array (rank 2, shape [N,2])
			'Begin values for slice: given by column 0 of padding array; size is given by input array

			Dim shape As SDVariable = arg().shape()
			Dim begin As SDVariable = arg(1).get(SDIndex.all(), SDIndex.point(0))

			Dim gradAtIn As SDVariable = sameDiff.slice(i_v(0), begin, shape)
			Dim zeros As SDVariable = sameDiff.zerosLike(arg(1))

			Return New List(Of SDVariable) From {gradAtIn, zeros}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count >= 1 AndAlso inputDataTypes.Count <= 3), "Expected 1-3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes) 'input, padding, pad value
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace